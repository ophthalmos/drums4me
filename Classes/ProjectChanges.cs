namespace Drums4me.Classes;

/// <summary>
/// Vergleicht zwei Projektstände und beschreibt die Unterschiede für den Benutzer.
/// Welches Pattern gerade angezeigt wird, gilt nicht als Änderung.
/// </summary>
internal static class ProjectChanges
{
    private const int MaxListedChanges = 8;

    public static List<string> Describe(Project saved, Project current, string[] trackNames)
    {
        var changes = new List<string>();

        void Compare<T>(string label, T before, T after, Func<T, string>? format = null)
        {
            if (!EqualityComparer<T>.Default.Equals(before, after))
            {
                var text = format ?? (value => value?.ToString() ?? string.Empty);
                changes.Add($"{label}: {text(before)} → {text(after)}");
            }
        }

        Compare(Lng.T("Tempo"), saved.Bpm, current.Bpm, bpm => $"{bpm} BPM");
        Compare(Lng.T("Swing"), saved.Swing, current.Swing, SwingName);
        Compare(Lng.T("Gesamtlautstärke"), saved.VolumePercent, current.VolumePercent, percent => $"{percent} %");
        Compare(Lng.T("Kit"), saved.KitName, current.KitName);
        Compare(Lng.T("Vorzählen"), saved.CountInBars, current.CountInBars, Bars);

        if (saved.TrainerEnabled != current.TrainerEnabled)
        {
            changes.Add(current.TrainerEnabled ? Lng.T("Tempotrainer: eingeschaltet") : Lng.T("Tempotrainer: ausgeschaltet"));
        }
        Compare(Lng.T("Tempotrainer, Schritt"), saved.TrainerStepBpm, current.TrainerStepBpm, bpm => $"{bpm} BPM");
        Compare(Lng.T("Tempotrainer, Abstand"), saved.TrainerEveryBars, current.TrainerEveryBars, Bars);
        Compare(Lng.T("Tempotrainer, Ziel"), saved.TrainerTargetBpm, current.TrainerTargetBpm, bpm => $"{bpm} BPM");

        if (saved.SequenceEnabled != current.SequenceEnabled)
        {
            changes.Add(current.SequenceEnabled ? Lng.T("Abfolge: eingeschaltet") : Lng.T("Abfolge: ausgeschaltet"));
        }
        Compare(Lng.T("Abfolge"), saved.Sequence, current.Sequence, sequence => sequence.Length == 0 ? Lng.T("(leer)") : sequence);
        Compare(Lng.T("Wiederholen"), saved.SequenceRepeat, current.SequenceRepeat, repeat => repeat ? Lng.T("ein") : Lng.T("aus"));

        for (var index = 0; index < Project.PatternCount; index++)
        {
            DescribePattern(changes, index, saved.Patterns[index], current.Patterns[index]);
        }

        for (var track = 0; track < Project.TrackCount; track++)
        {
            var before = saved.Tracks[track];
            var after = current.Tracks[track];
            var name = track < trackNames.Length && !string.IsNullOrEmpty(trackNames[track]) ? trackNames[track] : string.Format(Lng.T("Spur {0}"), track + 1);
            Compare(string.Format(Lng.T("{0}, Lautstärke"), name), Percent(before.Volume), Percent(after.Volume));
            Compare(string.Format(Lng.T("{0}, Panorama"), name), Percent(before.Pan), Percent(after.Pan), FormatPan);
            Compare(string.Format(Lng.T("{0}, Tonhöhe"), name), before.Pitch, after.Pitch, FormatPitch);
            if (before.Muted != after.Muted)
            {
                changes.Add(string.Format(after.Muted ? Lng.T("{0}: stumm geschaltet") : Lng.T("{0}: wieder eingeschaltet"), name));
            }
        }

        return changes;
    }

    /// <summary>Der Text für die Rückfrage. Lange Listen werden gekürzt, damit der Dialog handlich bleibt.</summary>
    public static string Summarize(string projectName, IReadOnlyList<string> changes)
    {
        var lines = changes.Take(MaxListedChanges).Select(change => $"  • {change}").ToList();
        var hidden = changes.Count - MaxListedChanges;
        if (hidden > 0)
        {
            lines.Add("  " + (hidden == 1 ? Lng.T("und 1 weitere Änderung") : string.Format(Lng.T("und {0} weitere Änderungen"), hidden)));
        }
        return string.Format(Lng.T("Geändert in „{0}“:"), projectName) + "\n" + string.Join('\n', lines);
    }

    private static void DescribePattern(List<string> changes, int index, Pattern saved, Pattern current)
    {
        var label = $"Pattern {index + 1}";
        if (!saved.Used && current.Used)
        {
            changes.Add(string.Format(Lng.T("{0}: neu angelegt"), label));
            return;
        }

        if (saved.MeterName != current.MeterName)
        {
            changes.Add(string.Format(Lng.T("{0}: Taktart {1} → {2}"), label, saved.MeterName, current.MeterName));
        }

        var different = 0;
        for (var track = 0; track < Project.TrackCount; track++)
        {
            for (var step = 0; step < Project.MaxStepCount; step++)
            {
                if (saved.Steps[track][step] != current.Steps[track][step])
                {
                    different++;
                }
            }
        }

        if (different > 0)
        {
            changes.Add(different == 1 ? string.Format(Lng.T("{0}: 1 Schritt geändert"), label) : string.Format(Lng.T("{0}: {1} Schritte geändert"), label, different));
        }
    }

    /// <summary>Verglichen wird wie angezeigt, in ganzen Prozent. Was man nicht sieht, gilt nicht als Änderung.</summary>
    private static int Percent(float value) => (int)MathF.Round(value * 100f);

    private static string FormatPan(int pan) => pan == 0 ? Lng.T("Mitte") : pan < 0 ? $"L{-pan}" : $"R{pan}";

    private static string Bars(int bars) => bars == 1 ? Lng.T("1 Takt") : string.Format(Lng.T("{0} Takte"), bars);

    private static string SwingName(SwingFeel swing) => swing switch
    {
        SwingFeel.Eighths => Lng.T("Achtel"),
        SwingFeel.Sixteenths => Lng.T("Sechzehntel"),
        _ => Lng.T("Gerade"),
    };

    private static string FormatPitch(int pitch) => pitch > 0 ? $"+{pitch}" : pitch.ToString();
}
