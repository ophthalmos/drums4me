; ============================================================================
; drums4me – Inno-Setup-Skript (Inno Setup 7)
;
; Vor dem Kompilieren die Release-Version bauen (Release|x64).
;
; Voraussetzung auf dem Zielrechner:
;   - .NET Desktop Runtime 11 (x64) – fehlt sie, zeigt Windows beim ersten Start
;     selbst einen Dialog mit Download-Link, daher keine Prüfung hier.
;
; Kits und Vorlagen gehören bewusst nicht zum Setup, sie kommen aus eigenen Inhaltspaketen
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
; Die Ordnerseite erscheint immer. Bei einem Update zeigt sie den bisherigen Ordner nur an (siehe InitializeWizard).
DisableDirPage=no
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
; Das Setup wählt die Sprache nach der Windows-Sprache; die erste ist der Rückfall. Die Wahl gilt beim ersten
; Start auch für das Programm (setup.default, siehe CurStepChanged und AppSettings.ApplyInstallerDefaults).
Name: en; MessagesFile: "compiler:Default.isl"
Name: de; MessagesFile: "compiler:Languages\German.isl"

[Messages]
en.WinVersionTooLowError=This program requires Windows 10 (version 1607) or later.
en.ConfirmUninstall=Are you sure you want to remove %1? Your projects, kits and settings are kept. You do not need to uninstall before an update.
de.WinVersionTooLowError=Dieses Programm benötigt Windows 10 (Version 1607) oder höher.
de.SelectDirLabel3=drums4me wird in diesen Ordner installiert.
de.SelectDirBrowseLabel=Klicke auf „Weiter“, um fortzufahren. Mit „Durchsuchen“ wählst du einen anderen Ordner.
de.ConfirmUninstall=Bist du sicher, dass du %1 entfernen möchtest? Deine Projekte, Kits und Einstellungen bleiben erhalten. Vor einem Update ist keine Deinstallation erforderlich.

[CustomMessages]
en.Run=Launch {#appName}
en.DesktopIcon=Create a desktop shortcut
en.ProjectFile={#appName} project
en.TemplateFile={#appName} template
en.UpdateDirLabel=drums4me is already installed. The update goes into the same folder.
en.UpdateBrowseLabel=Click Next to continue.
en.UpdatePageName=Destination folder
en.UpdatePageDescription=This is where drums4me is installed.
de.ProjectFile={#appName}-Projekt
de.TemplateFile={#appName}-Vorlage
de.Run={#appName} starten
de.DesktopIcon=Verknüpfung auf dem Desktop anlegen
de.UpdateDirLabel=drums4me ist schon installiert. Das Update kommt in denselben Ordner.
de.UpdateBrowseLabel=Klicke auf „Weiter“, um fortzufahren.
de.UpdatePageName=Ziel-Ordner
de.UpdatePageDescription=Hier liegt drums4me bereits.

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
; Dateizuordnung .d4mx => drums4me; HKA = HKLM bei Installation fuer alle Benutzer, sonst HKCU
Root: HKA; Subkey: "Software\Classes\.d4mx"; ValueType: string; ValueName: ""; ValueData: "{#appName}.Project"; Flags: uninsdeletevalue uninsdeletekeyifempty
Root: HKA; Subkey: "Software\Classes\.d4mx\OpenWithProgids"; ValueType: string; ValueName: "{#appName}.Project"; ValueData: ""; Flags: uninsdeletevalue uninsdeletekeyifempty
Root: HKA; Subkey: "Software\Classes\{#appName}.Project"; ValueType: string; ValueName: ""; ValueData: "{cm:ProjectFile}"; Flags: uninsdeletekey
Root: HKA; Subkey: "Software\Classes\{#appName}.Project\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#appName}.exe,0"
Root: HKA; Subkey: "Software\Classes\{#appName}.Project\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#appName}.exe"" ""%1"""
; Vorlagen .d4mt: Öffnen beginnt in drums4me ein neues Projekt mit ihrem Inhalt
Root: HKA; Subkey: "Software\Classes\.d4mt"; ValueType: string; ValueName: ""; ValueData: "{#appName}.Template"; Flags: uninsdeletevalue uninsdeletekeyifempty
Root: HKA; Subkey: "Software\Classes\.d4mt\OpenWithProgids"; ValueType: string; ValueName: "{#appName}.Template"; ValueData: ""; Flags: uninsdeletevalue uninsdeletekeyifempty
Root: HKA; Subkey: "Software\Classes\{#appName}.Template"; ValueType: string; ValueName: ""; ValueData: "{cm:TemplateFile}"; Flags: uninsdeletekey
Root: HKA; Subkey: "Software\Classes\{#appName}.Template\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#appName}.exe,0"
Root: HKA; Subkey: "Software\Classes\{#appName}.Template\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#appName}.exe"" ""%1"""

[Run]
Filename: "{app}\{#appName}.exe"; Description: "{cm:Run}"; Flags: nowait postinstall skipifsilent runasoriginaluser

[InstallDelete]
; Die früheren Demos hat das Paket „drums4me Demos – Standard“ hierher kopiert. Heute heißen sie Vorlagen und kommen aus dem Paket „drums4me Vorlagen“.
Type: filesandordirs; Name: "{commonappdata}\{#appName}\Demos"

[UninstallDelete]
Type: files; Name: "{app}\setup.default"

[Code]
{ Ein Update gehört in den bisherigen Ordner, sonst bliebe die alte Installation daneben liegen. }
function IsUpdate: Boolean;
begin
  Result := WizardForm.PrevAppDir <> '';
end;

{ Beim Update zeigt die Ordnerseite den Ordner nur an: Feld schreibgeschützt, ohne Durchsuchen. }
procedure InitializeWizard;
begin
  if IsUpdate then
  begin
    WizardForm.SelectDirLabel.Caption := CustomMessage('UpdateDirLabel');
    WizardForm.SelectDirBrowseLabel.Caption := CustomMessage('UpdateBrowseLabel');
    WizardForm.DirEdit.ReadOnly := True;
    WizardForm.DirEdit.TabStop := False;
    WizardForm.DirBrowseButton.Visible := False;
    WizardForm.DirEdit.Width := WizardForm.SelectDirPage.Width - WizardForm.DirEdit.Left;
  end;
end;

{ Sonst stünde der Fokus im Ordnerfeld und der Pfad wäre markiert, als sollte man ihn ändern. }
procedure CurPageChanged(CurPageID: Integer);
begin
  if (CurPageID = wpSelectDir) and IsUpdate then
  begin
    WizardForm.PageNameLabel.Caption := CustomMessage('UpdatePageName');
    WizardForm.PageDescriptionLabel.Caption := CustomMessage('UpdatePageDescription');
    WizardForm.ActiveControl := WizardForm.NextButton;
  end;
end;

{ Die Sprache des Setups hinterlegen. drums4me übernimmt sie beim ersten Start nach dieser Installation
  (AppSettings.ApplyInstallerDefaults); eine spätere Umstellung in den Einstellungen bleibt bis zum nächsten Setup. }
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
    SaveStringToFile(ExpandConstant('{app}\setup.default'), 'language=' + ActiveLanguage + #13#10, False);
end;
