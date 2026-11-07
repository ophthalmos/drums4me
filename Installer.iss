; ============================================================================
; drums4me – Inno-Setup-Skript (Inno Setup 7)
;
; Vor dem Kompilieren die Release-Version bauen (Release|x64).
;
; Voraussetzung auf dem Zielrechner:
;   - .NET Desktop Runtime 11 (x64) – fehlt sie, zeigt Windows beim ersten Start
;     selbst einen Dialog mit Download-Link, daher keine Prüfung hier.
;
; Kits und Demos gehören bewusst nicht zum Setup, sie kommen aus eigenen Inhaltspaketen
; (Ordner drumKits neben dem Projekt). Das Programm liest sie aus %ProgramData%\drums4me
; und eigene Kits aus %APPDATA%\drums4me\Kits. Findet es keine, meldet es das beim Start.
; Eigene Kits, die Einstellungen und die Projekte bleiben bei der Deinstallation erhalten.
; ============================================================================

#define appName "drums4me"
#define appVersion "1.0.0"
#define releaseDir "bin\x64\Release\net11.0-windows7.0"

[Setup]
AppId={{C4E2A9D6-7B13-4F58-9E60-2D1F8A37B5C4}
AppName={#appName}
AppVersion={#appVersion}
AppVerName={#appName} {#appVersion} (64-Bit)
VersionInfoVersion={#appVersion}
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
AppPublisher=Wilhelm Happe
AppCopyright=© 2026 Wilhelm Happe
UsePreviousAppDir=yes
DefaultDirName={autopf}\{#appName}
DefaultGroupName={#appName}
DisableWelcomePage=yes
DisableReadyPage=yes
DisableProgramGroupPage=yes
SetupIconFile=drums4me.ico
UninstallDisplayIcon={app}\{#appName}.exe
OutputDir=.
OutputBaseFilename={#appName}Setup
Compression=lzma2/ultra
SolidCompression=yes
DirExistsWarning=no
CloseApplications=yes
SetupMutex={#appName}_SetupMutex
; Windows wird nach der Installation ueber die Dateizuordnung informiert
ChangesAssociations=yes
WizardStyle=modern
; Windows 10 Version 1607 (Build 14393): Wert aus der .NET-10-Zeit, für .NET 11 nicht eigens geprüft
MinVersion=10.0.14393

[Languages]
Name: de; MessagesFile: "compiler:Languages\German.isl"

[Messages]
de.WinVersionTooLowError=Dieses Programm benötigt Windows 10 (Version 1607) oder höher.
de.ConfirmUninstall=Bist du sicher, dass du %1 entfernen möchtest? Deine Projekte, Kits und Einstellungen bleiben erhalten. Vor einem Update ist keine Deinstallation erforderlich.

[CustomMessages]
de.Run={#appName} starten
de.DesktopIcon=Verknüpfung auf dem Desktop anlegen

[Tasks]
Name: desktopicon; Description: "{cm:DesktopIcon}"; Flags: unchecked

[Files]
Source: "{#releaseDir}\*"; Excludes: "*.pdb"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "NOTICE.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "LICENSE.txt"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#appName}"; Filename: "{app}\{#appName}.exe"
Name: "{autodesktop}\{#appName}"; Filename: "{app}\{#appName}.exe"; Tasks: desktopicon

[Registry]
; Dateizuordnung .d4m => drums4me; HKA = HKLM bei Installation fuer alle Benutzer, sonst HKCU
Root: HKA; Subkey: "Software\Classes\.d4m"; ValueType: string; ValueName: ""; ValueData: "{#appName}.Project"; Flags: uninsdeletevalue uninsdeletekeyifempty
Root: HKA; Subkey: "Software\Classes\.d4m\OpenWithProgids"; ValueType: string; ValueName: "{#appName}.Project"; ValueData: ""; Flags: uninsdeletevalue uninsdeletekeyifempty
Root: HKA; Subkey: "Software\Classes\{#appName}.Project"; ValueType: string; ValueName: ""; ValueData: "{#appName}-Projekt"; Flags: uninsdeletekey
Root: HKA; Subkey: "Software\Classes\{#appName}.Project\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#appName}.exe,0"
Root: HKA; Subkey: "Software\Classes\{#appName}.Project\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#appName}.exe"" ""%1"""

[Run]
Filename: "{app}\{#appName}.exe"; Description: "{cm:Run}"; Flags: nowait postinstall skipifsilent runasoriginaluser
