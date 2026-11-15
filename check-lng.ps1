# Prüft die Übersetzungen von drums4me (deutscher Text = resx-Schlüssel, Vorbild PDFlight):
# LNG001: Ein übersetzbarer deutscher Text (Lng.T im Code, Text/ToolTipText/AccessibleDescription/AccessibleName/
#         ShortcutKeyDisplayString-/PlaceholderText-Zuweisungen, SetToolTip-Literale) hat in lng.en.resx keinen Eintrag. Vermutlich
#         wurde deutscher Text geändert, ohne den Schlüssel nachzuziehen.
# LNG002: Ein resx-Schlüssel kommt im Code nicht mehr vor, eine Altlast nach einer Umformulierung.
# Grenzen: Mehrzeilige Texte laufen über explizite Schlüssel (Lng.T("Schlüssel", "…")), deren Text sieht der
# Scanner nicht. Ausgabe im MSBuild-Warnungsformat; läuft als Build-Target nach jedem Build (siehe drums4me.csproj)
# und jederzeit von Hand:  powershell -ExecutionPolicy Bypass -File check-lng.ps1

# -Strict (Release-Build): Funde als Fehler statt Warnungen melden und mit Exit-Code 1 abbrechen
param([switch]$Strict)
$severity = if ($Strict) { "error" } else { "warning" }
$root = $PSScriptRoot
$languages = @("en")

# Texte, die absichtlich in keiner Sprachdatei stehen: Namen, sprachneutrale Angaben und Designer-Platzhalter,
# die zur Laufzeit sofort überschrieben werden
$ignore = @(
    "drums4me", "BPM", "Pattern", "Swing", "Kit", "Kits", "Demo", "Demos", "OK", "Tempo", "Name", "Audio", "System", "Start",
    "Deutsch", "English",   # die Sprachwahl nennt jede Sprache in ihrer eigenen
    "alle"                  # Tempotrainer „alle … Takte“: läuft über den Schlüssel TrainerEvery, weil „Alle“ schon vergeben ist
)

# ---------------------------------------------------------------- Schlüssel der Sprachdateien
$langKeys = @{}
foreach ($code in $languages) {
    $file = Join-Path $root "Languages\lng.$code.resx"
    [xml]$xml = Get-Content $file -Raw -Encoding UTF8
    $keys = New-Object 'System.Collections.Generic.HashSet[string]'
    foreach ($data in $xml.root.data) { [void]$keys.Add([string]$data.name) }
    $langKeys[$code] = $keys
}

# ---------------------------------------------------------------- verwendete Schlüssel sammeln
function ConvertFrom-CSharpLiteral([string]$s) {
    [regex]::Replace($s, '\\u([0-9A-Fa-f]{4})|\\(.)', { param($m)
        if ($m.Groups[1].Success) { return [string][char][Convert]::ToInt32($m.Groups[1].Value, 16) }
        switch ($m.Groups[2].Value) { 'n' { "`n" } 'r' { "`r" } 't' { "`t" } default { $m.Groups[2].Value } } })
}

$literal = '"((?:[^"\\]|\\.)*)"'
$tokens = '@"(?<v>(?:[^"]|"")*)"|"(?<s>(?:[^"\\]|\\.)*)"|' + "'" + '(?:\\.|[^' + "'" + '\\])' + "'" + '|//[^\r\n]*|/\*[\s\S]*?\*/'
$used = New-Object 'System.Collections.Generic.HashSet[string]'
$explicit = New-Object 'System.Collections.Generic.HashSet[string]'
$sources = Get-ChildItem $root -Recurse -Include *.cs -File | Where-Object { $_.FullName -notmatch '\\(obj|bin|\.claude)\\' }
foreach ($file in $sources) {
    $text = Get-Content $file.FullName -Raw -Encoding UTF8
    # 1) explizite Schlüssel: Lng.T("Schlüssel", "Text") – der Schlüssel zählt, der Text nicht
    foreach ($m in [regex]::Matches($text, "Lng\.T\(\s*$literal\s*,")) {
        [void]$explicit.Add((ConvertFrom-CSharpLiteral $m.Groups[1].Value))
    }
    # 2) direkte Lng.T-Aufrufe mit einem einzigen Literal (auch innerhalb interpolierter Strings)
    foreach ($m in [regex]::Matches($text, "Lng\.T\(\s*$literal\s*\)")) {
        [void]$used.Add((ConvertFrom-CSharpLiteral $m.Groups[1].Value))
    }
    # 3) Bedingungen: Lng.T(bedingung ? "A" : "B") – beide Zweige sind Schlüssel
    foreach ($m in [regex]::Matches($text, "Lng\.T\([^`"()]*\?\s*$literal\s*:\s*$literal\s*\)")) {
        [void]$used.Add((ConvertFrom-CSharpLiteral $m.Groups[1].Value))
        [void]$used.Add((ConvertFrom-CSharpLiteral $m.Groups[2].Value))
    }
    # 4) übersetzte Eigenschafts-Zuweisungen (Designer wie Code; nur reine Literale bis zum Semikolon)
    foreach ($m in [regex]::Matches($text, "\b(?:Text|ToolTipText|ShortcutKeyDisplayString|AccessibleDescription|AccessibleName|PlaceholderText|Description|Filter|Title)\s*=\s*$literal\s*;")) {
        [void]$used.Add((ConvertFrom-CSharpLiteral $m.Groups[1].Value))
    }
    # 5) Tooltips der ToolTip-Komponente im Designer
    foreach ($m in [regex]::Matches($text, "\.SetToolTip\([^,]+,\s*$literal\s*\)")) {
        [void]$used.Add((ConvertFrom-CSharpLiteral $m.Groups[1].Value))
    }
    # 6) Rettungsregel gegen Fehlalarme: Jedes Literal, das genau einem vorhandenen resx-Schlüssel entspricht, gilt als
    #    verwendet. Das deckt Konstanten, switch-Ausdrücke und Listen ab, ohne LNG001 aufzuweichen, denn es zählen nur
    #    Texte, die schon übersetzt sind. Strings, Kommentare und Zeichenliterale werden gemeinsam zerlegt, damit
    #    Anführungszeichen in Kommentaren die Paarung nicht verschieben.
    foreach ($m in [regex]::Matches($text, $tokens)) {
        $value = $null
        if ($m.Groups['v'].Success) { $value = $m.Groups['v'].Value.Replace('""', '"') }
        elseif ($m.Groups['s'].Success) { $value = ConvertFrom-CSharpLiteral $m.Groups['s'].Value }
        if ($value -and ($languages | Where-Object { $langKeys[$_].Contains($value) })) { [void]$used.Add($value) }
    }
}

# ---------------------------------------------------------------- LNG001: fehlende Übersetzungen
$findings = 0
$all = New-Object 'System.Collections.Generic.SortedSet[string]' ([StringComparer]::Ordinal)   # Sort-Object -Unique fasste „Alle“ und „alle“ zusammen
$all.UnionWith([string[]]@($used)); $all.UnionWith([string[]]@($explicit))
foreach ($key in $all) {
    if ($key -notmatch '\p{L}') { continue }     # ohne Buchstaben (z. B. "0/0") gibt es nichts zu übersetzen
    if ($key -match "`n") { continue }           # mehrzeilig taugt nicht als Schlüssel, läuft über explizite Schlüssel
    if ($key -match '\{\D') { continue }         # Bruchstück eines interpolierten Strings ({0} bleibt erlaubt)
    if ($key -match '^(F\d+|Alt\+[A-Z0-9])$') { continue } # sprachneutrale Kürzel
    if ($ignore -contains $key) { continue }
    foreach ($code in ($languages | Where-Object { -not $langKeys[$_].Contains($key) })) {
        Write-Output "Languages\lng.$code.resx : $severity LNG001: Übersetzung fehlt für Schlüssel: `"$key`""
        $findings++
    }
}

# ---------------------------------------------------------------- LNG002: verwaiste resx-Schlüssel
foreach ($code in $languages) {
    foreach ($key in $langKeys[$code] | Sort-Object) {
        if ($ignore -contains $key) { continue }
        if (-not $used.Contains($key) -and -not $explicit.Contains($key)) {
            Write-Output "Languages\lng.$code.resx : $severity LNG002: Verwaister Schlüssel (im Code nicht gefunden): `"$key`""
            $findings++
        }
    }
}

if ($findings -eq 0) { Write-Output "check-lng: Alle Übersetzungen konsistent ($($used.Count + $explicit.Count) Schlüssel geprüft)." }
if ($Strict -and $findings -gt 0) { exit 1 }
exit 0
