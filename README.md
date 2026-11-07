# drums4me

Step-Sequencer und Metronom für Windows, gedacht zum Üben am Instrument. Das Programm läuft komplett offline.

## Funktionen

- 8 Spuren mit Lautstärke, Panorama und Tonhöhe, dazu Swing und 20 bis 300 BPM
- Lautstärke und Tonhöhe aller Spuren gemeinsam ändern: Regler mit gedrückter Strg-Taste ziehen
- 4 Patterns je Projekt, jedes mit eigener Taktart: 2/4, 3/4, 4/4, 5/4 (3+2 und 2+3), 6/8, 7/8, 9/8 und 12/8
- Abfolge der Patterns, wahlweise mit Wiederholung
- Vorzählen mit einstellbarer Anzahl von Takten
- Tempotrainer: Das Tempo steigt beim Spielen schrittweise bis zu einem Ziel
- Tempo ändern mit Plus und Minus (1 BPM) sowie Strg+Plus und Strg+Minus (5 BPM), Lautstärke mit Alt+Plus und Alt+Minus (5 %), Start und Stopp mit der Leertaste
- Tempo ermitteln in einem Fenster (Strg+T): im Takt auf die Leertaste tippen oder einen gerade in Windows laufenden Song mithören lassen; die Analyse bleibt auf dem Rechner und speichert nichts
- optionales Aufblitzen der Anzeige beim Taktwechsel
- Darstellung hell, dunkel oder wie Windows
- Statuszeile mit Hinweisen zum Element unter der Maus sowie Taktzähler und Spielzeit
- Rückgängig und Wiederholen, beim Schließen eine Liste der tatsächlichen Änderungen
- Projekte als `.d4m`-Dateien, die zuletzt benutzte Datei wird auf Wunsch beim Start geladen
- Registerkarten statt Menü: Datei mit Projektübersicht, Suche, Filtern nach Kit und Taktart und sortierbaren Spalten, auch für andere Ordner; Start mit allen Bedienelementen zum Üben; Einstellungen

## Kits

Das Programm bringt keine Sounds mit. Ein Kit ist ein Ordner mit bis zu acht WAV-Dateien, eine Datei je Spur. Gesucht wird an zwei Orten:

| Ort | Inhalt |
|---|---|
| `%ProgramData%\drums4me\Kits\<Sammlung>` | Kits aus Inhaltspaketen, die Sammlung erscheint in der Auswahlliste in Klammern |
| `%APPDATA%\drums4me\Kits` | eigene Kits, haben bei gleichem Namen Vorrang |

Die Spuren erscheinen in alphabetischer Reihenfolge der Dateinamen. Eine Zahl am Anfang des Namens legt die Reihenfolge fest und wird nicht angezeigt, zum Beispiel `1 Crash.wav`. Das gilt auch für die Ordnernamen der Kits.

Ein Klick auf einen Spurnamen spielt den Klang zum Probehören. Mit der rechten Maustaste lassen sich die Spuren eigener Kits umbenennen. Eigene Kits stehen am Ende der Auswahlliste und sind dort als „(eigen)“ markiert.

## Bauen

Benötigt das .NET 11 SDK.

```bash
dotnet build drums4me.csproj -c Release -p:Platform=x64
```

Das Setup entsteht danach aus `Installer.iss` mit Inno Setup 7.

## Lizenz

drums4me steht unter der [GNU General Public License, Version 3](LICENSE.txt). Die verwendeten Fremdkomponenten und ihre Lizenzen stehen in [NOTICE.txt](NOTICE.txt). Kits und Demos gehören nicht zum Programm, für sie gelten eigene Lizenzen.
