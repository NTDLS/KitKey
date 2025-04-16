#define AppVersion "2.0.5"

[Setup]
;-- Main Setup Information
 AppName                          = KitKey
 AppVersion                       = {#AppVersion}
 AppVerName                       = KitKey {#AppVersion}
 AppCopyright                     = Copyright © 1995-2025 NetworkDLS.
 DefaultDirName                   = {commonpf}\NetworkDLS\KitKey
 DefaultGroupName                 = NetworkDLS\KitKey
 UninstallDisplayIcon             = {app}\KitKey.Service.exe
 SetupIconFile                    = "..\Images\Logo.ico"
 PrivilegesRequired               = admin
 Uninstallable                    = Yes
 MinVersion                       = 0.0,7.0
 Compression                      = bZIP/9
 ChangesAssociations              = Yes
 OutputBaseFilename               = KitKey.windows.x64
 ArchitecturesInstallIn64BitMode  = x64compatible
 AppPublisher                     = NetworkDLS
 AppPublisherURL                  = http://www.NetworkDLS.com/
 AppUpdatesURL                    = http://www.NetworkDLS.com/

[Files]
 Source: "publish\win-x64\wwwroot\*.*"; DestDir: "{app}\wwwroot"; Flags: IgnoreVersion recursesubdirs;
 Source: "publish\win-x64\*.exe"; DestDir: "{app}"; Flags: IgnoreVersion;
 Source: "publish\win-x64\*.dll"; DestDir: "{app}"; Flags: IgnoreVersion;
 Source: "publish\win-x64\*.json"; DestDir: "{app}"; Flags: IgnoreVersion;
 Source: "..\Images\Logo.ico"; DestDir: "{app}"; Flags: IgnoreVersion;

[Icons]
 Name: "{commondesktop}\KitKey Manager"; Filename: "http://localhost:45487/"; IconFilename: "{app}\Logo.ico"
 Name: "{group}\KitKey Manager"; Filename: "http://localhost:45487/"; IconFilename: "{app}\Logo.ico"

[Run]
 Filename: "{app}\KitKey.Service.exe"; Parameters: "install"; Flags: runhidden; StatusMsg: "Installing service...";
 Filename: "{app}\KitKey.Service.exe"; Parameters: "start"; Flags: runhidden; StatusMsg: "Starting service...";
 Filename: "http://localhost:45487/"; Description: "Run KitKey Manager now?"; Flags: postinstall nowait skipifsilent shellexec;

[UninstallRun]
 Filename: "{app}\KitKey.Service.exe"; Parameters: "uninstall"; Flags: runhidden; StatusMsg: "Installing service..."; RunOnceId: "ServiceRemoval";
