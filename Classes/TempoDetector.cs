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
    private const int MaxBpm = 240;
    private const double MaxSeconds = 45;
    private const double MinSeconds = 6;
    private const float SilenceLevel = 0.0005f;
    private const double SureStrength = 0.18;
    private const double MaxPauseSeconds = 2.5;
    private const double PulseShare = 0.55;
    private const double PreferredMin = 50;
    private const double PreferredMax = 210;

    private readonly object _lock = new();
    private readonly int _hop;
    private readonly double _frameRate;
    private readonly int _maxFrames;
    private readonly float[] _window = new float[FftSize];
    private readonly Complex[] _fft = new Complex[FftSize];
    private readonly float[] _previous = new float[FftSize / 2];
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

        // Zusammengepresste Beträge, damit leise Hi-Hats neben der lauten Kick mitzählen
        var flux = 0f;
        for (var bin = 1; bin < FftSize / 2; bin++)
        {
            var magnitude = MathF.Log(1f + (20000f * MathF.Sqrt((_fft[bin].X * _fft[bin].X) + (_fft[bin].Y * _fft[bin].Y))));
            var rise = magnitude - _previous[bin];
            if (rise > 0)
            {
                flux += rise;
            }
            _previous[bin] = magnitude;
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
        var minLag = (int)(60.0 * _frameRate / MaxBpm);
        var maxLag = Math.Min((int)(60.0 * _frameRate / MinBpm), (onset.Length / 2) - 1);
        var longest = Math.Min(maxLag * 4, onset.Length - 2);
        var acf = Autocorrelate(onset, longest + 1);
        if (acf[0] <= 0)
        {
            return null;
        }

        // Der kleinste deutliche Abstand ist der feinste Notenwert, etwa Achtel oder Sechzehntel.
        // Das Tempo muss ein einfaches Vielfaches davon sein. Das verhindert Treffer wie 80 statt 120 BPM.
        var pulse = Refine(acf, FindPulse(acf, (int)(0.085 * _frameRate), Math.Min(maxLag, longest)), longest);
        var ternary = IsTernary(acf, (int)Math.Round(pulse), longest);
        int[] multiples = ternary ? [1, 3, 6, 12] : [1, 2, 4, 8];
        var bestLag = (int)Math.Round(pulse);
        var bestScore = double.MinValue;
        foreach (var multiple in multiples)
        {
            var lag = (int)Math.Round(pulse * multiple);
            var bpmHere = 60.0 * _frameRate / lag;
            if (bpmHere < PreferredMin || bpmHere > PreferredMax || lag * 2 > longest)
            {
                continue;
            }

            // Wiederholt sich der Rhythmus auf mehreren Vielfachen, ist es ein tragender Abstand.
            double on = 0;
            var count = 0;
            for (var k = 1; k <= 4 && k * lag <= longest; k++)
            {
                on += acf[(int)Math.Round(k * pulse * multiple)];
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

        // Genauer wird es über den möglichst fernen Vielfachen: Dort fällt ein kleiner Fehler stärker ins Gewicht.
        var refined = Refine(acf, bestLag, longest);
        var bpm = 60.0 * _frameRate / refined;
        var strength = Math.Clamp(acf[bestLag] / acf[0], 0, 1);
        return new TempoEstimate(bpm, strength, strength >= SureStrength);
    }

    private static int FindPulse(double[] acf, int from, int to)
    {
        var strongest = 0.0;
        for (var lag = from; lag <= to; lag++)
        {
            strongest = Math.Max(strongest, acf[lag]);
        }

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

    /// <summary>Teilt sich der Rhythmus in Dreien, etwa im 6/8-Takt oder Shuffle?</summary>
    private static bool IsTernary(double[] acf, int pulse, int longest)
    {
        if (pulse * 6 > longest)
        {
            return false;
        }
        var three = acf[pulse * 3] + acf[pulse * 6];
        var two = acf[pulse * 2] + acf[pulse * 4];
        return three > 1.25 * two;
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
