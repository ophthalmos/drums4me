using NAudio.Dsp;
using System.Diagnostics;

namespace Drums4me.Classes;

/// <summary>
/// Schätzt das Tempo aus laufendem Ton. Aus den Lautstärkesprüngen je Frequenzband entsteht eine Einsatzkurve,
/// deren Wiederholungsabstand über die Autokorrelation das Tempo ergibt. Der Ton wird nur fortlaufend ausgewertet,
/// gespeichert wird nur die Einsatzkurve im Speicher.
/// </summary>
internal sealed class TempoDetector
{
    private const int FftOrder = 10;
    private const int FftSize = 1 << FftOrder;
    private const double HopSeconds = 0.0058;
    private const int MinBpm = 40;
    private const double MaxSeconds = 45;
    private const double MinSeconds = 6;
    private const float SilenceLevel = 0.0005f;
    private const double SureStrength = 0.18;
    private const double MaxPauseSeconds = 2.5;
    private const double PulseShare = 0.40;   // so stark wie dieser Anteil der stärksten Wiederholung muss der feinste Notenwert sein
    private const double PeakShare = 0.30;    // und so stark ein Abstand, der nur ersatzweise in Frage kommt
    private const double PreferredMin = 50;
    private const double PreferredMax = 210;

    // Bandzahl und PulseShare sind an Songs und Übungsstücken mit bekanntem Tempo ausgemessen (November 2026):
    // 128 Bänder und 0,40 trafen am besten, von 96 bis 200 Bändern und 0,35 bis 0,45 ändert sich wenig.
    private const int BandCount = 128;
    private const double BandMinHertz = 40;
    private const double BandMaxHertz = 16000;
    private const float Compression = 20000f;

    private readonly Lock _lock = new();
    private readonly int _hop;
    private readonly double _frameRate;
    private readonly int _maxFrames;
    private readonly float[] _window = new float[FftSize];
    private readonly Complex[] _fft = new Complex[FftSize];
    private readonly float[] _previous = new float[BandCount];
    private readonly int[] _bandStart = new int[BandCount + 1];
    private readonly float[] _pending = new float[FftSize];
    private readonly List<float> _envelope = [];
    private int _pendingCount;
    private int _quietFrames;
    private long _heardSamples;
    private long _lastAdd;
    private readonly int _sampleRate;

    public TempoDetector(int sampleRate)
    {
        _sampleRate = sampleRate;
        _hop = Math.Max(1, (int)(sampleRate * HopSeconds));
        _frameRate = sampleRate / (double)_hop;
        _maxFrames = (int)(MaxSeconds * _frameRate);
        for (var i = 0; i < FftSize; i++)
        {
            _window[i] = 0.5f - (0.5f * MathF.Cos(2f * MathF.PI * i / (FftSize - 1)));
        }

        // Die Bänder sind nach der Mel-Skala verteilt, also unten schmal und oben breit, wie das Ohr Tonhöhen unterscheidet.
        // Jedes Band zählt gleich viel. Bei gleich breiten Frequenzlinien lägen drei Viertel über 5 kHz, und Becken und
        // Hi-Hat übertönten Kick, Bass und Snare, an denen man den Takt erkennt. So ist es noch ein Drittel.
        // Bis etwa 1 kHz ist ein Band eine einzelne Frequenzlinie, feiner löst die Analyse nicht auf.
        static double Mel(double hertz) => 2595.0 * Math.Log10(1.0 + (hertz / 700.0));
        static double Hertz(double mel) => 700.0 * (Math.Pow(10.0, mel / 2595.0) - 1.0);
        var lowest = Mel(BandMinHertz);
        var highest = Mel(Math.Min(BandMaxHertz, sampleRate / 2.0));
        var binWidth = sampleRate / (double)FftSize;
        for (var band = 0; band <= BandCount; band++)
        {
            var bin = (int)Math.Round(Hertz(lowest + ((highest - lowest) * band / BandCount)) / binWidth);
            _bandStart[band] = Math.Clamp(band == 0 ? Math.Max(1, bin) : Math.Max(_bandStart[band - 1] + 1, bin), 1, FftSize / 2);
        }
    }

    /// <summary>So lange war bisher Ton zu hören. Stille zählt nicht mit, etwa vor dem Abspielen.</summary>
    public double HeardSeconds
    {
        get
        {
            lock (_lock)
            {
                return _heardSamples / (double)_sampleRate;
            }
        }
    }

    public void Reset()
    {
        lock (_lock)
        {
            StartOver();
        }
    }

    private void StartOver()
    {
        _envelope.Clear();
        _pendingCount = 0;
        _quietFrames = 0;
        _heardSamples = 0;
        Array.Clear(_previous);
    }

    /// <summary>Nimmt Ton in Mono an, in beliebig großen Stücken. Darf aus dem Aufnahme-Thread kommen.</summary>
    public void Add(ReadOnlySpan<float> mono)
    {
        lock (_lock)
        {
            // Spielt nichts, liefert Windows oft gar keinen Ton statt Stille. Auch dann beginnt das nächste Stück von vorn.
            var now = Stopwatch.GetTimestamp();
            if (_lastAdd != 0 && now - _lastAdd > MaxPauseSeconds * Stopwatch.Frequency)
            {
                StartOver();
            }
            _lastAdd = now;

            var offset = 0;
            while (offset < mono.Length)
            {
                var count = Math.Min(FftSize - _pendingCount, mono.Length - offset);
                mono.Slice(offset, count).CopyTo(_pending.AsSpan(_pendingCount));
                _pendingCount += count;
                offset += count;
                if (_pendingCount == FftSize)
                {
                    ProcessFrame();
                    Array.Copy(_pending, _hop, _pending, 0, FftSize - _hop);
                    _pendingCount = FftSize - _hop;
                }
            }
        }
    }

    private void ProcessFrame()
    {
        var level = 0f;
        for (var i = 0; i < FftSize; i++)
        {
            level = Math.Max(level, MathF.Abs(_pending[i]));
        }
        if (level < SilenceLevel)
        {
            // Kurze Pausen gehören zum Rhythmus. Eine lange Stille beendet das Stück, sonst mischte sich das nächste mit diesem.
            _quietFrames++;
            if (_envelope.Count > 0)
            {
                if (_quietFrames < MaxPauseSeconds * _frameRate)
                {
                    _heardSamples += _hop;
                    AddFlux(0f);
                }
                else
                {
                    ForgetPiece();
                }
            }
            return;
        }

        _quietFrames = 0;
        _heardSamples += _hop;
        for (var i = 0; i < FftSize; i++)
        {
            _fft[i].X = _pending[i] * _window[i];
            _fft[i].Y = 0f;
        }
        FastFourierTransform.FFT(true, FftOrder, _fft);

        // Je Band der zusammengepresste Pegel, damit leise Einsätze neben lauten mitzählen. Ein Einsatz ist der Anstieg im Band.
        var flux = 0f;
        for (var band = 0; band < BandCount; band++)
        {
            var sum = 0f;
            for (var bin = _bandStart[band]; bin < _bandStart[band + 1]; bin++)
            {
                sum += MathF.Sqrt((_fft[bin].X * _fft[bin].X) + (_fft[bin].Y * _fft[bin].Y));
            }

            var bandLevel = MathF.Log(1f + (Compression * sum));
            var rise = bandLevel - _previous[band];
            if (rise > 0)
            {
                flux += rise;
            }
            _previous[band] = bandLevel;
        }

        AddFlux(flux);
    }

    /// <summary>Wie <see cref="StartOver"/>, aber mitten im Verarbeiten eines Blocks: Der laufende Puffer bleibt erhalten.</summary>
    private void ForgetPiece()
    {
        _envelope.Clear();
        _heardSamples = 0;
        Array.Clear(_previous);
    }

    private void AddFlux(float flux)
    {
        _envelope.Add(flux);
        if (_envelope.Count > _maxFrames)
        {
            _envelope.RemoveRange(0, _envelope.Count - _maxFrames);
        }
    }

    /// <summary>Die aktuelle Schätzung. Null, solange zu wenig Ton zu hören war.</summary>
    public TempoEstimate? Estimate()
    {
        float[] envelope;
        lock (_lock)
        {
            if (_envelope.Count < MinSeconds * _frameRate)
            {
                return null;
            }
            envelope = [.. _envelope];
        }

        var onset = Flatten(envelope);
        var maxLag = Math.Min((int)(60.0 * _frameRate / MinBpm), (onset.Length / 2) - 1);
        var longest = Math.Min(maxLag * 4, onset.Length - 2);
        var acf = Autocorrelate(onset, longest + 1);
        if (acf[0] <= 0)
        {
            return null;
        }

        var from = (int)(0.085 * _frameRate);
        var to = Math.Min(maxLag, longest);
        var strongest = 0.0;
        for (var lag = from; lag <= to; lag++)
        {
            strongest = Math.Max(strongest, acf[lag]);
        }

        // Der kleinste deutliche Abstand ist der feinste Notenwert, etwa Achtel oder Sechzehntel.
        // Das Tempo muss ein einfaches Vielfaches davon sein. Das verhindert Treffer wie 80 statt 120 BPM.
        var pulse = Refine(acf, FindPulse(acf, from, to, strongest), longest);
        var bestLag = pulse;
        var bestScore = double.MinValue;

        void Try(double lag)
        {
            var bpmHere = 60.0 * _frameRate / lag;
            if (bpmHere < PreferredMin || bpmHere > PreferredMax || lag * 2 > longest)
            {
                return;
            }

            // Wiederholt sich der Rhythmus auf mehreren Vielfachen, ist es ein tragender Abstand.
            double on = 0;
            var count = 0;
            for (var k = 1; k <= 4 && (int)Math.Round(k * lag) <= longest; k++)
            {
                on += acf[(int)Math.Round(k * lag)];
                count++;
            }
            var score = on / count / acf[0];

            // Zwischen halbem und doppeltem Tempo entscheidet die übliche Lage um 110 BPM.
            var octaves = Math.Log2(bpmHere / 110.0);
            score *= Math.Exp(-0.5 * octaves * octaves / (0.7 * 0.7));
            if (score > bestScore)
            {
                bestScore = score;
                bestLag = lag;
            }
        }

        var ternary = IsTernary(acf, pulse, longest);
        if (!ternary)
        {
            // Bei Rhythmen wie 3+3+2 ist der feinste deutliche Abstand selbst ein punktierter Wert. Der Schlag liegt dann
            // bei vier Dritteln davon. Er zählt nur mit, wenn dort wirklich eine eigene Wiederholung steht.
            foreach (var dotted in new[] { 2.0 / 3, 4.0 / 3, 8.0 / 3 })
            {
                if (IsPeak(acf, pulse * dotted, longest, strongest))
                {
                    Try(pulse * dotted);
                }
            }
        }

        foreach (var multiple in ternary ? new[] { 1, 3, 6, 12 } : [1, 2, 4, 8])
        {
            Try(pulse * multiple);
        }

        // Nichts im üblichen Tempobereich: Der feinste deutliche Abstand war schon länger als ein Schlag, etwa ein halber
        // Takt bei sparsamem Schlagzeug. Dann liegt der Schlag bei der Hälfte oder einem Viertel.
        if (bestScore == double.MinValue)
        {
            Try(pulse / 2);
            Try(pulse / 4);
        }

        // Genauer wird es über den möglichst fernen Vielfachen: Dort fällt ein kleiner Fehler stärker ins Gewicht.
        var lagIndex = Math.Clamp((int)Math.Round(bestLag), 1, longest);
        var refined = Refine(acf, lagIndex, longest);
        var bpm = 60.0 * _frameRate / refined;
        var strength = Math.Clamp(acf[lagIndex] / acf[0], 0, 1);
        return new TempoEstimate(bpm, strength, strength >= SureStrength);
    }

    private static int FindPulse(double[] acf, int from, int to, double strongest)
    {
        for (var lag = from + 1; lag < to; lag++)
        {
            // Ein echter Notenwert kehrt auch im doppelten Abstand wieder, ein Nachhall im Klang selbst nicht.
            if (acf[lag] >= strongest * PulseShare && acf[lag] >= acf[lag - 1] && acf[lag] >= acf[lag + 1]
                && (lag * 2) + 1 < acf.Length
                && Math.Max(acf[lag * 2], Math.Max(acf[(lag * 2) - 1], acf[(lag * 2) + 1])) >= strongest * PulseShare)
            {
                return lag;
            }
        }
        return Math.Max(from, 1);
    }

    /// <summary>Steht bei diesem Abstand eine eigene, deutliche Wiederholung, also eine Spitze der Autokorrelation?</summary>
    private static bool IsPeak(double[] acf, double lag, int longest, double strongest)
    {
        var center = (int)Math.Round(lag);
        if (center < 3 || center + 3 > longest)
        {
            return false;
        }

        var top = center;
        for (var index = center - 2; index <= center + 2; index++)
        {
            if (acf[index] > acf[top])
            {
                top = index;
            }
        }
        return acf[top] >= strongest * PeakShare && acf[top] >= acf[top - 1] && acf[top] >= acf[top + 1];
    }

    /// <summary>
    /// Teilt sich der Rhythmus in Dreien, etwa im 6/8-Takt oder Shuffle? Dann wiederholen sich drei und sechs feinste
    /// Notenwerte besser als zwei und vier, und auch der Takt passt dazu: Zwölf (vier Dreiergruppen) wiederholen sich besser
    /// als acht oder sechzehn. Bei 3+3+2 im geraden Takt gilt nur das Erste, deshalb reicht es allein nicht.
    /// </summary>
    private static bool IsTernary(double[] acf, double pulse, int longest)
    {
        var whole = (int)Math.Round(pulse);
        if (whole * 6 > longest || acf[whole * 3] + acf[whole * 6] <= 1.25 * (acf[whole * 2] + acf[whole * 4]))
        {
            return false;
        }
        if ((int)Math.Round(pulse * 16) + 1 > longest)
        {
            return true;   // zu wenig Ton für die Probe am Takt
        }

        double At(double lag)
        {
            var index = (int)Math.Round(lag);
            return Math.Max(acf[index], Math.Max(acf[index - 1], acf[index + 1]));
        }
        return At(pulse * 12) > At(pulse * 16) && At(pulse * 12) > At(pulse * 8);
    }

    /// <summary>Entfernt langsame Lautstärkeänderungen, nur die Einsätze selbst bleiben.</summary>
    private double[] Flatten(float[] envelope)
    {
        var radius = (int)(0.25 * _frameRate);
        var result = new double[envelope.Length];
        double sum = 0;
        var from = 0;
        var to = -1;
        for (var i = 0; i < envelope.Length; i++)
        {
            while (to < Math.Min(envelope.Length - 1, i + radius))
            {
                sum += envelope[++to];
            }
            while (from < i - radius)
            {
                sum -= envelope[from++];
            }
            result[i] = Math.Max(0, envelope[i] - (sum / (to - from + 1)));
        }
        return result;
    }

    private static double[] Autocorrelate(double[] signal, int lags)
    {
        var mean = signal.Average();
        var centered = signal.Select(value => value - mean).ToArray();
        var result = new double[lags];
        for (var lag = 0; lag < lags; lag++)
        {
            double sum = 0;
            for (var i = lag; i < centered.Length; i++)
            {
                sum += centered[i] * centered[i - lag];
            }
            result[lag] = sum / (centered.Length - lag);
        }
        return result;
    }

    private static double Refine(double[] acf, int lag, int longest)
    {
        var refined = (double)lag;
        for (var multiple = 2; multiple <= 8; multiple *= 2)
        {
            var center = (int)Math.Round(refined * multiple);
            if (center + 2 >= longest)
            {
                break;
            }

            var best = center;
            for (var candidate = center - 2; candidate <= center + 2; candidate++)
            {
                if (candidate > 0 && acf[candidate] > acf[best])
                {
                    best = candidate;
                }
            }

            var left = acf[best - 1];
            var middle = acf[best];
            var right = acf[best + 1];
            var divisor = left - (2 * middle) + right;
            var shift = Math.Abs(divisor) < 1e-12 ? 0 : 0.5 * (left - right) / divisor;
            refined = (best + Math.Clamp(shift, -0.5, 0.5)) / multiple;
        }
        return refined;
    }
}

/// <param name="Strength">Wie klar sich der Rhythmus wiederholt, von 0 bis 1.</param>
internal sealed record TempoEstimate(double Bpm, double Strength, bool IsSure);
