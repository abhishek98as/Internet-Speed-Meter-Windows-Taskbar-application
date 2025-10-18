; SpeedoMeter - Windows Installer Script for Inno Setup
; Inno Setup 6.0 or higher required
; Download from: https://jrsoftware.org/isinfo.php

#define MyAppName "SpeedoMeter"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "SpeedoMeter Team"
#define MyAppURL "https://github.com/abhishek98as/Internet-Speed-Meter-Windows-Taskbar-application"
#define MyAppExeName "SpeedoMeter.exe"
#define MyAppDescription "Real-time Network Speed Monitor for Windows Taskbar"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
AppId={{B5F8D7E2-9C4A-4B3F-8E1D-2A6F9C8B5D4E}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=LICENSE
InfoBeforeFile=INSTALLATION_INFO.txt
OutputDir=installer
OutputBaseFilename=SpeedoMeter_Setup_v{#MyAppVersion}
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
UninstallDisplayIcon={app}\{#MyAppExeName}
UninstallDisplayName={#MyAppName}
SetupIconFile=Resources\app.ico
; WizardImageFile=compiler:WizModernImage-IS.bmp
; WizardSmallImageFile=compiler:WizModernSmallImage-IS.bmp

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[CustomMessages]
english.LaunchProgram=Launch %1 after installation
english.CreateDesktopIcon=Create a &desktop shortcut
english.AutoStartOption=Start %1 automatically with Windows

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "Additional icons:"
Name: "autostart"; Description: "{cm:AutoStartOption,{#MyAppName}}"; GroupDescription: "Startup options:"
Name: "quicklaunchicon"; Description: "Create a &Quick Launch shortcut"; GroupDescription: "Additional icons:"; Flags: unchecked

[Files]
; Main executable (single-file publish)
Source: "bin\Release\net8.0-windows\win-x64\publish\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
; Optional: Include PDB for debugging
Source: "bin\Release\net8.0-windows\win-x64\publish\*.pdb"; DestDir: "{app}"; Flags: ignoreversion; Attribs: hidden

; Documentation files
Source: "README.md"; DestDir: "{app}"; Flags: ignoreversion isreadme
Source: "CHANGELOG.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "LICENSE"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Comment: "{#MyAppDescription}"
Name: "{group}\README"; Filename: "{app}\README.md"; Comment: "SpeedoMeter Documentation"
Name: "{group}\Quick Start Guide"; Filename: "{app}\QUICKSTART.md"; Comment: "Quick Start Guide"
Name: "{group}\Uninstall {#MyAppName}"; Filename: "{uninstallexe}"; Comment: "Uninstall SpeedoMeter"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon; Comment: "{#MyAppDescription}"
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon; Comment: "{#MyAppDescription}"

[Registry]
; Auto-start registry entry (only if user selected the option)
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "{#MyAppName}"; ValueData: """{app}\{#MyAppExeName}"""; Tasks: autostart; Flags: uninsdeletevalue

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent

[UninstallRun]
; Stop the application before uninstalling (if running)
Filename: "{cmd}"; Parameters: "/C taskkill /F /IM {#MyAppExeName} /T"; Flags: runhidden; RunOnceId: "StopSpeedoMeter"

[UninstallDelete]
Type: files; Name: "{userappdata}\SpeedoMeter\settings.json"
Type: dirifempty; Name: "{userappdata}\SpeedoMeter"

[Code]
// Check if .NET 8.0 Runtime is installed
function IsDotNet8Installed(): Boolean;
var
  ResultCode: Integer;
begin
  // Check if dotnet command exists and version
  Result := Exec('cmd.exe', '/C dotnet --list-runtimes | findstr "Microsoft.WindowsDesktop.App 8."', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  if not Result or (ResultCode <> 0) then
    Result := False
  else
    Result := True;
end;

function InitializeSetup(): Boolean;
begin
  Result := True;
  
  // Check for .NET 8.0 Runtime (self-contained, so this is informational)
  if not IsDotNet8Installed() then
  begin
    if MsgBox('This version of SpeedoMeter is self-contained and does not require .NET 8.0 Runtime.' + #13#10 + #13#10 +
              'However, if you prefer a smaller installation size, you can install the .NET 8.0 Runtime separately.' + #13#10 + #13#10 +
              'Do you want to continue with the installation?', mbInformation, MB_YESNO) = IDNO then
    begin
      Result := False;
    end;
  end;
end;

// Check if the application is running before installation
function PrepareToInstall(var NeedsRestart: Boolean): String;
var
  ResultCode: Integer;
begin
  Result := '';
  NeedsRestart := False;
  
  // Try to close the application gracefully
  if Exec('cmd.exe', '/C tasklist | findstr /I "{#MyAppExeName}"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
  begin
    if ResultCode = 0 then
    begin
      if MsgBox('SpeedoMeter is currently running. Setup will close it to continue.' + #13#10 + #13#10 +
                'Click OK to close SpeedoMeter and continue, or Cancel to exit setup.', mbConfirmation, MB_OKCANCEL) = IDOK then
      begin
        Exec('cmd.exe', '/C taskkill /F /IM {#MyAppExeName} /T', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
        Sleep(1000); // Wait for process to close
      end
      else
      begin
        Result := 'Installation cancelled by user.';
      end;
    end;
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    // Create application data directory if it doesn't exist
    ForceDirectories(ExpandConstant('{userappdata}\SpeedoMeter'));
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  DialogResult: Integer;
begin
  if CurUninstallStep = usPostUninstall then
  begin
    // Ask user if they want to keep settings
    DialogResult := MsgBox('Do you want to keep your SpeedoMeter settings?' + #13#10 + #13#10 +
                          'Click Yes to keep your configuration for future installations.' + #13#10 +
                          'Click No to remove all settings.', mbConfirmation, MB_YESNO);
    
    if DialogResult = IDNO then
    begin
      // Delete settings file and folder
      DeleteFile(ExpandConstant('{userappdata}\SpeedoMeter\settings.json'));
      RemoveDir(ExpandConstant('{userappdata}\SpeedoMeter'));
    end;
  end;
end;
