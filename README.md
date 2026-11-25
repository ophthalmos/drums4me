# drums4me

Step-Sequencer und Metronom für Windows, gedacht zum Üben am Instrument. Das Programm läuft komplett offline.

## Funktionen

- 8 Spuren mit Lautstärke, Panorama und Tonhöhe, 20 bis 300 BPM
- Swing wie im Notensatz: gerade notiert, im Triolengefühl gespielt, wahlweise für Achtel oder Sechzehntel
- Akzente und Ghost Notes: Jeder Schritt ist leise, normal oder betont. Ein Rechtsklick auf einen gesetzten Schritt wechselt seine Stärke
- Kopieren im Programm: ein Pattern auf ein anderes ziehen oder per Rechtsklick kopieren und einfügen; eine Zählzeit mit Klick auf ihre Zahl markieren und auf eine andere ziehen, per Rechtsklick auch auf alle Zählzeiten übertragen. Strg+C und Strg+V wirken auf die markierte Zählzeit, sonst auf das Pattern
- Spuren einzeln stumm schalten mit dem Schalter unter dem Spurnamen; mit gedrückter Strg-Taste spielt nur diese Spur (Solo), ein zweiter Klick schaltet wieder alle ein. Stumme Spuren kommen auch nicht in die Audiodatei
- Lautstärke und Tonhöhe aller Spuren gemeinsam ändern: Regler mit gedrückter Strg-Taste ziehen
- 4 Patterns je Projekt, jedes mit eigener Taktart: 2/4, 3/4, 4/4, 5/4 (3+2 und 2+3), 6/8, 7/8, 9/8 und 12/8
- Abfolge der Patterns, wahlweise mit Wiederholung: eine Ziffer je Takt, `1x8` für acht gleiche Takte, `0` für einen stummen Takt, in dem man allein weiterspielt; oben links im Raster steht das gezeigte Pattern und während einer Abfolge die Stelle darin, etwa „Pattern 4 4/5“
- Vorlagen (`.d4mt`): Wie bei Office beginnt das Öffnen einer Vorlage ein neues, unbenanntes Projekt mit ihrem Inhalt, auch per Doppelklick im Explorer. Installierte und eigene Vorlagen liegen gemeinsam in `%ProgramData%\drums4me\Vorlagen` und gelten für alle Benutzer des PCs. „Als Vorlage speichern…“ legt eine Kopie des Projekts dort ab, „Vorlagenordner“ zeigt alle in der Projektliste; eigene lassen sich dort umbenennen, duplizieren und löschen, installierte nicht
- MIDI-Grooves importieren: Aus dem Schlagzeug einer MIDI-Datei (Kanal 10, bei einzelnen Grooves auch Kanal 1) werden bis zu vier Takte zu den Patterns eines neuen Projekts, auf Sechzehntel gerundet und nach den Spurnamen auf das gewählte Kit verteilt. Die Anschlagstärke wird zu leise, normal und betont, triolische Grooves bekommen Swing-Achtel. Ganze Songs lassen sich so nicht übernehmen
- Als Audiodatei speichern (WAV, FLAC oder MP3), was die Wiedergabe spielt; ein Fenster fragt nach Länge und Format und erklärt, was in die Datei kommt. Wiederholt sich das Ganze, endet die Datei genau auf der Taktgrenze und lässt sich als WAV oder FLAC nahtlos wiederholen, eine Abfolge ohne Wiederholung klingt am Ende aus
- Vorzählen mit einstellbarer Anzahl von Takten; die Lautstärke der Klicks steht in den Einstellungen
- Tempotrainer: Das Tempo steigt beim Spielen schrittweise bis zu einem Ziel
- Tempo ändern mit Plus und Minus (1 BPM) sowie Strg+Plus und Strg+Minus (5 BPM), Gesamtlautstärke mit Alt+Plus und Alt+Minus (5 %, der Wert erscheint in der Statuszeile), Start und Stopp mit der Leertaste
- Tempo ermitteln in einem Fenster (Strg+T): im Takt auf die Leertaste tippen oder einen gerade in Windows laufenden Song mithören lassen; die Analyse bleibt auf dem Rechner und speichert nichts
- optionales Aufblitzen der Anzeige beim Taktwechsel (Taktblitz, auf der Karte Start)
- Zwei Schalter oben rechts neben Rückgängig: „Immer im Vordergrund“ hält das Fenster über anderen Programmen, „Bildschirm anlassen“ verhindert, dass Windows den Bildschirm abschaltet, solange die Wiedergabe läuft
- Darstellung hell, dunkel oder wie Windows
- Deutsch und Englisch; das Setup übernimmt beim ersten Start seine Sprache, umstellen lässt sie sich in den Einstellungen
- Statuszeile mit Hinweisen zum Element unter der Maus sowie Taktzähler und Spielzeit
- Rückgängig und Wiederholen, beim Schließen eine Liste der tatsächlichen Änderungen
- Projekte als `.d4mx`-Dateien, die zuletzt benutzte Datei wird auf Wunsch beim Start geladen; ein neues Projekt beginnt mit dem einfachen Metronom oder mit einer Vorlage, einstellbar unter „Programmstart“
- Fenstergröße und -lage bleiben von Sitzung zu Sitzung erhalten
- Registerkarten statt Menü: Datei mit Befehlsspalte (Neu, Speichern, als Vorlage speichern, als Audiodatei speichern, Schließen, anderer Ordner und zurück zum Standardordner, Vorlagenordner, Vorlage öffnen, MIDI importieren, Fensterlayout zurücksetzen, Beenden), Wahl des Speicherorts für Projekte und Projektübersicht mit Kontextmenü (im Editor bearbeiten, Dateipfad kopieren, Umbenennen, Duplizieren, Löschen in den Papierkorb, im Ordner anzeigen – auch in Directory Opus –, Eigenschaften), Suche, Filtern nach Kit und Taktart und sortierbaren Spalten, auch für andere Ordner; Start mit allen Bedienelementen zum Üben, darunter der Auswahl, welche Kits in der Liste erscheinen; Einstellungen mit Wahl des Ausgabegeräts, etwa eines Audio-Interfaces, und der Latenz der Tonausgabe

## Kits

Das Programm bringt keine Sounds mit. Ein Kit ist ein Ordner mit bis zu acht WAV-Dateien, eine Datei je Spur. Gesucht wird an zwei Orten:

| Ort | Inhalt |
|---|---|
| `%ProgramData%\drums4me\Kits\<Sammlung>` | Kits aus Inhaltspaketen, die Sammlung erscheint in der Auswahlliste in Klammern |
| `%APPDATA%\drums4me\Kits` | eigene Kits, haben bei gleichem Namen Vorrang |

Die Spuren erscheinen in alphabetischer Reihenfolge der Dateinamen. Eine Zahl am Anfang des Namens legt die Reihenfolge fest und wird nicht angezeigt, zum Beispiel `1 Crash.wav`. Das gilt auch für die Ordnernamen der Kits.

Wie am Schlagzeug stoppt ein Hi-Hat-Schlag die noch klingende offene Hi-Hat. Erkannt werden die Spuren am Namen, etwa „Hat Open“ und „Hat Closed“, „Open Hihat“ und „Hihat“ oder „Hi-Hat offen“ und „Hi-Hat“.

Ein Klick auf einen Spurnamen spielt den Klang zum Probehören. Mit der rechten Maustaste lassen sich die Spuren eigener Kits umbenennen. Eigene Kits stehen am Ende der Auswahlliste und sind dort als „(eigen)“ markiert.

## Bauen

Benötigt das .NET 11 SDK.

```bash
dotnet build drums4me.csproj -c Release -p:Platform=x64
```

Die englischen Texte stehen in `Languages\lng.en.resx`, Schlüssel ist jeweils der deutsche Text. Nach jedem Build prüft `check-lng.ps1`, ob Übersetzungen fehlen oder übrig sind; im Release-Build bricht der Build dann ab.

Das Setup entsteht danach aus `Installer.iss` mit Inno Setup 7.

## Lizenz

drums4me steht unter der [GNU General Public License, Version 3](LICENSE.txt). Die verwendeten Fremdkomponenten und ihre Lizenzen stehen in [NOTICE.txt](NOTICE.txt). Kits und Vorlagen gehören nicht zum Programm, für sie gelten eigene Lizenzen.
