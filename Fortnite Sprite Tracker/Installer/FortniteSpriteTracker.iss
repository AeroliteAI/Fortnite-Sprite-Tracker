#define AppName "Fortnite Sprite Tracker"
#define AppVersion "0.1.3"
#define AppPublisher "Aerolite"
#define AppExeName "FortniteSpriteTracker.exe"
#define AppId "{{A3F2D8C1-7E4B-4F92-B035-6D1A2E9C4F87}"

[Setup]
AppId={#AppId}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={localappdata}\{#AppName}
DefaultGroupName={#AppName}
AllowNoIcons=yes
OutputDir=Output
OutputBaseFilename=FortniteSpriteTrackerSetup_v{#AppVersion}
SetupIconFile=AppFiles\Assets\FNTracker.ico
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
ArchitecturesInstallIn64BitMode=x64compatible
ArchitecturesAllowed=x64compatible
UninstallDisplayIcon={app}\{#AppExeName}
UninstallDisplayName={#AppName}
VersionInfoVersion={#AppVersion}
VersionInfoCompany={#AppPublisher}
VersionInfoDescription={#AppName} Installer

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop shortcut"; GroupDescription: "Additional icons:"

[Dirs]
; Create Sprites and Data folders inside the install dir on first install.
; uninsneverdelete preserves user sprites/saves when uninstalling.
Name: "{app}\Sprites"; Flags: uninsneveruninstall
Name: "{app}\Data";    Flags: uninsneveruninstall

[InstallDelete]
; Wipe the Sprites folder before installing so upgrades don't leave stale/duplicate sprites.
Type: filesandordirs; Name: "{app}\Sprites"

[Files]
Source: "AppFiles\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#AppName}";          Filename: "{app}\{#AppExeName}"
Name: "{group}\Uninstall {#AppName}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#AppName}";    Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExeName}"; Description: "Launch {#AppName}"; Flags: nowait postinstall skipifsilent
