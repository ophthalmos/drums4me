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

    private Meter(string name, int[] beats, int[] groups)
    {
        Name = name;
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

    /// <summary>Unbekannte Namen, etwa aus von Hand geänderten Dateien, ergeben 4/4.</summary>
    public static Meter Find(string? name) => All.FirstOrDefault(meter => meter.Name == name) ?? FourFour;

    public override string ToString() => Name;
}
