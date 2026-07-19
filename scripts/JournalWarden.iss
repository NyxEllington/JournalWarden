; Inno Setup Script for JournalWarden

#define MyAppName "JournalWarden"
#define MyAppVersion "1.0"
#define MyAppPublisher "JournalWarden"
#define MyAppURL "https://github.com"
#define MyAppExeName "JournalWarden.exe"
#define PublishDir "..\artifacts\prerelease\JournalWarden-win-x64"

[Setup]
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={localappdata}\Programs\{#MyAppName}
DefaultGroupName={#MyAppName}
Compression=lzma2
SolidCompression=yes
OutputDir=..\artifacts\installer
OutputBaseFilename=JournalWarden-Setup
PrivilegesRequired=lowest
UninstallDisplayIcon={app}\{#MyAppExeName}
CreateAppDir=yes
DisableProgramGroupPage=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
