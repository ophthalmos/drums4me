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

        Compare("Tempo", saved.Bpm, current.Bpm, bpm => $"{bpm} BPM");
        Compare("Swing", saved.SwingPercent, current.SwingPercent, percent => $"{percent} %");
        Compare("Lautstärke", saved.VolumePercent, current.VolumePercent, percent => $"{percent} %");
        Compare("Kit", saved.KitName, current.KitName);
        Compare("Vorzählen", saved.CountInBars, current.CountInBars, bars => bars == 1 ? "1 Takt" : $"{bars} Takte");

        if (saved.TrainerEnabled != current.TrainerEnabled)
        {
            changes.Add(current.TrainerEnabled ? "Tempotrainer: eingeschaltet" : "Tempotrainer: ausgeschaltet");
        }
        Compare("Tempotrainer, Schritt", saved.TrainerStepBpm, current.TrainerStepBpm, bpm => $"{bpm} BPM");
        Compare("Tempotrainer, Abstand", saved.TrainerEveryBars, current.TrainerEveryBars, bars => bars == 1 ? "1 Takt" : $"{bars} Takte");
        Compare("Tempotrainer, Ziel", saved.TrainerTargetBpm, current.TrainerTargetBpm, bpm => $"{bpm} BPM");

        if (saved.SequenceEnabled != current.SequenceEnabled)
        {
            changes.Add(current.SequenceEnabled ? "Abfolge: eingeschaltet" : "Abfolge: ausgeschaltet");
        }
        Compare("Abfolge", saved.Sequence, current.Sequence, sequence => sequence.Length == 0 ? "(leer)" : sequence);
        Compare("Wiederholen", saved.SequenceRepeat, current.SequenceRepeat, repeat => repeat ? "ein" : "aus");

        for (var index = 0; index < Project.PatternCount; index++)
        {
            DescribePattern(changes, index, saved.Patterns[index], current.Patterns[index]);
        }

        for (var track = 0; track < Project.TrackCount; track++)
        {
            var before = saved.Tracks[track];
            var after = current.Tracks[track];
            var name = track < trackNames.Length && !string.IsNullOrEmpty(trackNames[track]) ? trackNames[track] : $"Spur {track + 1}";
            Compare($"{name}, Lautstärke", Percent(before.Volume), Percent(after.Volume));
            Compare($"{name}, Panorama", Percent(before.Pan), Percent(after.Pan), FormatPan);
            Compare($"{name}, Tonhöhe", before.Pitch, after.Pitch, FormatPitch);
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
            lines.Add(hidden == 1 ? "  und 1 weitere Änderung" : $"  und {hidden} weitere Änderungen");
        }
        return $"Geändert in „{projectName}“:\n{string.Join('\n', lines)}";
    }

    private static void DescribePattern(List<string> changes, int index, Pattern saved, Pattern current)
    {
        var label = $"Pattern {index + 1}";
        if (!saved.Used && current.Used)
        {
            changes.Add($"{label}: neu angelegt");
            return;
        }

        if (saved.MeterName != current.MeterName)
        {
            changes.Add($"{label}: Taktart {saved.MeterName} → {current.MeterName}");
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
            changes.Add(different == 1 ? $"{label}: 1 Schritt geändert" : $"{label}: {different} Schritte geändert");
        }
    }

    /// <summary>Verglichen wird wie angezeigt, in ganzen Prozent. Was man nicht sieht, gilt nicht als Änderung.</summary>
    private static int Percent(float value) => (int)MathF.Round(value * 100f);

    private static string FormatPan(int pan) => pan == 0 ? "Mitte" : pan < 0 ? $"L{-pan}" : $"R{pan}";

    private static string FormatPitch(int pitch) => pitch > 0 ? $"+{pitch}" : pitch.ToString();
}
