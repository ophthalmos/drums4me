using System.Diagnostics;

namespace Drums4me.Classes;

/// <summary>
/// Der Verlauf für Rückgängig und Wiederholen. Er hält vollständige Projektstände und vergleicht sie mit dem
/// aktuellen Stand, sodass nur echte Änderungen einen Schritt ergeben.
/// </summary>
internal sealed class UndoHistory
{
    public const int MaxSteps = 100;

    /// <summary>Änderungen derselben Art innerhalb dieser Zeit zählen als ein Schritt, etwa mehrere Klicks ins Tempofeld.</summary>
    public static readonly TimeSpan MergeWindow = TimeSpan.FromSeconds(1);

    private static readonly string[] NoTrackNames = [];

    private readonly List<Project> _undo = [];
    private readonly List<Project> _redo = [];
    private Project _current;
    private string? _lastMergeKey;
    private long _lastCommit;

    public UndoHistory(Project start) => _current = start.Snapshot();

    public bool CanUndo => _undo.Count > 0;

    public bool CanRedo => _redo.Count > 0;

    /// <summary>Beginnt einen neuen Verlauf, etwa nach dem Öffnen eines Projekts.</summary>
    public void Reset(Project start)
    {
        _undo.Clear();
        _redo.Clear();
        _current = start.Snapshot();
        _lastMergeKey = null;
    }

    /// <summary>Hält eine abgeschlossene Änderung fest. Ohne echten Unterschied passiert nichts.</summary>
    public void Commit(Project project, string? mergeKey = null) => Commit(project, mergeKey, Stopwatch.GetTimestamp());

    internal void Commit(Project project, string? mergeKey, long timestamp)
    {
        if (ProjectChanges.Describe(_current, project, NoTrackNames).Count == 0)
        {
            return;
        }

        var merge = _undo.Count > 0 && mergeKey is not null && mergeKey == _lastMergeKey
            && Stopwatch.GetElapsedTime(_lastCommit, timestamp) <= MergeWindow;
        if (!merge)
        {
            _undo.Add(_current);
            if (_undo.Count > MaxSteps)
            {
                _undo.RemoveAt(0);
            }
        }

        _redo.Clear();
        _current = project.Snapshot();
        _lastMergeKey = mergeKey;
        _lastCommit = timestamp;
    }

    /// <summary>Liefert den vorherigen Stand, oder null, wenn es keinen gibt.</summary>
    public Project? Undo(Project project)
    {
        if (!CanUndo)
        {
            return null;
        }

        _redo.Add(project.Snapshot());
        return Step(_undo, project);
    }

    /// <summary>Liefert den zuletzt zurückgenommenen Stand, oder null, wenn es keinen gibt.</summary>
    public Project? Redo(Project project)
    {
        if (!CanRedo)
        {
            return null;
        }

        _undo.Add(project.Snapshot());
        return Step(_redo, project);
    }

    private Project Step(List<Project> source, Project project)
    {
        var target = source[^1];
        source.RemoveAt(source.Count - 1);
        target.ActivePattern = AffectedPattern(project, target);
        _current = target.Snapshot();
        _lastMergeKey = null;
        return target;
    }

    /// <summary>
    /// Welches Pattern nach dem Schritt angezeigt wird, damit die Änderung zu sehen ist: das aktuelle, wenn es
    /// betroffen ist oder gar kein Pattern betroffen ist, sonst das erste betroffene.
    /// </summary>
    internal static int AffectedPattern(Project before, Project after)
    {
        var first = -1;
        for (var index = 0; index < Project.PatternCount && first != before.ActivePattern; index++)
        {
            if (after.Patterns[index].Used && PatternDiffers(before.Patterns[index], after.Patterns[index]))
            {
                first = first < 0 || index == before.ActivePattern ? index : first;
            }
        }

        if (first >= 0)
        {
            return first;
        }

        // Ein zurückgenommenes neues Pattern gibt es im Zielstand nicht mehr, angezeigt wird dann ein vorhandenes.
        return after.Patterns[before.ActivePattern].Used ? before.ActivePattern : after.ActivePattern;
    }

    private static bool PatternDiffers(Pattern a, Pattern b)
    {
        if (a.MeterName != b.MeterName || a.Used != b.Used)
        {
            return true;
        }

        for (var track = 0; track < Project.TrackCount; track++)
        {
            if (!a.Steps[track].AsSpan().SequenceEqual(b.Steps[track]))
            {
                return true;
            }
        }
        return false;
    }
}
