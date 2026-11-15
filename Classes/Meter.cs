namespace Drums4me.Classes;

/// <summary>
/// Eine Taktart: wie viele 16tel-Schritte ein Takt hat, wie sie sich auf Zählzeiten verteilen
/// und wie diese Zählzeiten gruppiert sind. Die Grundeinheit ist immer die 16tel.
/// </summary>
internal sealed class Meter
{
    public const int ClickNone = 0;
    public const int ClickBeat = 1;
    public const int ClickGroup = 2;
    public const int ClickBar = 3;

    public static readonly Meter FourFour = new("4/4", [4, 4, 4, 4], [1, 1, 1, 1]);

    /// <summary>Alle wählbaren Taktarten. Eine weitere braucht nur einen Eintrag in dieser Liste.</summary>
    public static readonly IReadOnlyList<Meter> All =
    [
        new("2/4", [4, 4], [1, 1]),
        new("3/4", [4, 4, 4], [1, 1, 1]),
        FourFour,
        new("5/4 (3+2)", [4, 4, 4, 4, 4], [3, 2]),
        new("5/4 (2+3)", [4, 4, 4, 4, 4], [2, 3]),
        new("6/8", [6, 6], [1, 1]),
        new("7/8 (2+2+3)", [4, 4, 6], [1, 1, 1]),
        new("9/8", [6, 6, 6], [1, 1, 1]),
        new("12/8", [6, 6, 6, 6], [1, 1, 1, 1]),
    ];

    // Swing teilt zwei gleich notierte Noten im Verhältnis 2:1, zusammen bleiben sie so lang wie gerade gespielt.
    private const double SwingLong = 4.0 / 3.0;
    private const double SwingShort = 2.0 / 3.0;

    private readonly bool _countsQuarters;

    private Meter(string name, int[] beats, int[] groups)
    {
        Name = name;
        _countsQuarters = beats.All(beat => beat == 4);
        BeatCount = beats.Length;
        Steps = beats.Sum();
        BeatOf = new int[Steps];
        GroupOf = new int[Steps];
        IsBeatStart = new bool[Steps];
        ClickLevels = new int[Steps];

        // Nur echte Gruppen wie 3+2 betonen, sonst wäre jede Zählzeit ein Gruppenanfang.
        var hasGroups = groups.Length < beats.Length;
        var step = 0;
        var beat = 0;
        for (var group = 0; group < groups.Length; group++)
        {
            for (var inGroup = 0; inGroup < groups[group]; inGroup++, beat++)
            {
                IsBeatStart[step] = true;
                ClickLevels[step] = step == 0 ? ClickBar : hasGroups && inGroup == 0 ? ClickGroup : ClickBeat;
                for (var inBeat = 0; inBeat < beats[beat]; inBeat++, step++)
                {
                    BeatOf[step] = beat;
                    GroupOf[step] = group;
                }
            }
        }
    }

    public string Name { get; }

    public int Steps { get; }

    public int BeatCount { get; }

    /// <summary>Die Zählzeit jedes Schritts, ab 0.</summary>
    public int[] BeatOf { get; }

    /// <summary>Die Gruppe jedes Schritts, ab 0.</summary>
    public int[] GroupOf { get; }

    public bool[] IsBeatStart { get; }

    /// <summary>Welcher Klick beim Vorzählen auf jedem Schritt liegt.</summary>
    public int[] ClickLevels { get; }

    /// <summary>
    /// Wie lang ein Schritt mit Swing ist, gemessen an der geraden Sechzehntel. Zählzeiten und Taktlänge bleiben, wie sie sind.
    /// Swing-Achtel gibt es nur, wo die Zählzeit eine Viertel aus zwei Achteln ist: In 6/8, 7/8, 9/8 und 12/8 bleibt der Rhythmus gerade.
    /// </summary>
    public double StepFactor(SwingFeel swing, int step) => swing switch
    {
        SwingFeel.Sixteenths => step % 2 == 0 ? SwingLong : SwingShort,
        SwingFeel.Eighths when _countsQuarters => step % 4 < 2 ? SwingLong : SwingShort,
        _ => 1.0,
    };

    /// <summary>Der erste Schritt einer Zählzeit und die Zahl ihrer Schritte.</summary>
    public (int Start, int Count) StepsOfBeat(int beat) => (Array.IndexOf(BeatOf, beat), BeatOf.Count(value => value == beat));

    /// <summary>Unbekannte Namen, etwa aus von Hand geänderten Dateien, ergeben 4/4.</summary>
    public static Meter Find(string? name) => All.FirstOrDefault(meter => meter.Name == name) ?? FourFour;

    public override string ToString() => Name;
}
