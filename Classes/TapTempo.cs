namespace Drums4me.Classes;

/// <summary>
/// Ermittelt das Tempo aus getippten Schlägen. Einzelne verrutschte Schläge fallen dabei nicht ins Gewicht,
/// und weil nur die letzten Abstände zählen, folgt das Ergebnis auch einem bewussten Tempowechsel.
/// </summary>
internal sealed class TapTempo
{
    /// <summary>Länger als der Abstand eines Schlags beim langsamsten Tempo von 20 BPM (3 Sekunden).</summary>
    public const double ResetSeconds = 3.5;

    private const int MaxIntervals = 16;

    private readonly List<double> _intervals = [];
    private double? _lastTap;

    public int TapCount { get; private set; }

    public bool HasTempo => _intervals.Count > 0;

    /// <summary>Das Tempo aus den letzten Abständen, ohne das schnellste und langsamste Viertel. Vor dem zweiten Schlag 0.</summary>
    public double Bpm
    {
        get
        {
            if (!HasTempo)
            {
                return 0;
            }

            var sorted = _intervals.Order().ToArray();
            var skip = sorted.Length / 4;
            var kept = sorted.Skip(skip).Take(sorted.Length - (2 * skip));
            return 60.0 / kept.Average();
        }
    }

    /// <summary>Die Pause war so lang, dass der nächste Schlag neu beginnt.</summary>
    public bool IsExpired(double now) => _lastTap is { } last && now - last > ResetSeconds;

    public void Tap(double now)
    {
        if (IsExpired(now))
        {
            Reset();
        }

        if (_lastTap is { } last)
        {
            _intervals.Add(now - last);
            if (_intervals.Count > MaxIntervals)
            {
                _intervals.RemoveAt(0);
            }
        }

        _lastTap = now;
        TapCount++;
    }

    public void Reset()
    {
        _intervals.Clear();
        _lastTap = null;
        TapCount = 0;
    }
}
