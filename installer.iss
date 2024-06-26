; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define AppName "Susi"
#define AppVersion "0.1.0.0"
#define AppPublisher "Sean Francis N. Ballais"
#define AppURL "https://github.com/seanballais/susi-gui-windows"
#define AppPackagePath "bin\build\susi-gui-windows\x64\Release\net8.0-windows10.0.19041.0\win-x64\AppPackages\susi-gui-windows_0.1.0.0_x64_Test"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{C24506BE-0143-48C3-A591-EBEAD957F362}
AppName={#AppName}
AppVersion={#AppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}
CreateAppDir=no
LicenseFile=LICENSE.md
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
AlwaysRestart=yes
PrivilegesRequired=admin
OutputDir=bin\build\installer
OutputBaseFilename=susi-0.1.0.0-setup-win-x64
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
ArchitecturesInstallIn64BitMode=x64
ArchitecturesAllowed=x64

[Files]
Source: "installer\download-and-install-prerequisites.ps1"; DestDir: "{tmp}"
Source: "{#AppPackagePath}\susi-gui-windows_0.1.0.0_x64.cer"; DestDir: "{tmp}"
Source: "{#AppPackagePath}\susi-gui-windows_0.1.0.0_x64.msix"; DestDir: "{tmp}"; BeforeInstall: RunPreInstall; AfterInstall: RunSusiAppPostInstall

[Code]
// Parts of the code is from: https://stackoverflow.com/a/32266687/1116098
#ifdef UNICODE
#define AW "W"
#else
#define AW "A"
#endif

const
  WAIT_OBJECT_0           = $0;
  WAIT_TIMEOUT            = $00000102;
  SEE_MASK_NOCLOSEPROCESS = $00000040;
  INFINITE                = $FFFFFFFF;
  PM_REMOVE               = 1;

type
  TShellExecuteInfo = record
    cbSize:       DWORD;
    fMask:        Cardinal;
    Wnd:          HWND;
    lpVerb:       string;
    lpFile:       string;
    lpParameters: string;
    lpDirectory:  string;
    nShow:        Integer;
    hInstApp:     THandle;
    lpIDList:     DWORD;
    lpClass:      string;
    hkeyClass:    THandle;
    dwHotKey:     DWORD;
    hMonitor:     THandle;
    hProcess:     THandle;
  end;

type
  TMsg = record
    hwnd:    HWND;
    message: UINT;
    wParam:  LongInt;
    lParam:  LongInt;
    time:    DWORD;
    pt:      TPoint;
  end;

function ShellExecuteEx(var lpExecInfo: TShellExecuteInfo): BOOL;
  external 'ShellExecuteEx{#AW}@shell32.dll stdcall';
function WaitForSingleObject(hHandle: THandle; dwMilliseconds: DWORD): DWORD;
  external 'WaitForSingleObject@kernel32.dll stdcall';
function CloseHandle(hObject: THandle): BOOL;
  external 'CloseHandle@kernel32.dll stdcall';
function PeekMessage(var lpMsg: TMsg; hWnd: HWND; wMsgFilterMin, wMsgFilterMax, wRemoveMsg: UINT): BOOL;
  external 'PeekMessageA@user32.dll stdcall';
function TranslateMessage(const lpMsg: TMsg): BOOL;
  external 'TranslateMessage@user32.dll stdcall';
function DispatchMessage(const lpMsg: TMsg): LongInt;
  external 'DispatchMessageA@user32.dll stdcall';

procedure AppProcessMessage();
var
  Msg: TMsg;
begin
  while PeekMessage(Msg, WizardForm.Handle, 0, 0, PM_REMOVE) do begin
    TranslateMessage(Msg);
    DispatchMessage(Msg);
  end;
end;

var CancelWithoutPrompt: boolean;

procedure RunPreInstall();
var
  ResultCode: Integer;
  HasEncounteredError: Boolean;
  ExecInfo: TShellExecuteInfo;
begin
  HasEncounteredError := False;

  WizardForm.StatusLabel.Caption := 'Checking for missing dependencies and installing those missing...'

  ExecInfo.cbSize       := SizeOf(ExecInfo);
  ExecInfo.fMask        := SEE_MASK_NOCLOSEPROCESS;
  ExecInfo.Wnd          := 0;
  ExecInfo.lpFile       := ExpandConstant('{sys}\WindowsPowerShell\v1.0\powershell.exe');
  ExecInfo.lpParameters := ExpandConstant('-WindowStyle Hidden -ExecutionPolicy Bypass -File "{tmp}\download-and-install-prerequisites.ps1" -TempDir "{tmp}"');
  ExecInfo.nShow        := SW_HIDE;

  if ShellExecuteEx(ExecInfo) then begin
    while WaitForSingleObject(ExecInfo.hProcess, 16) = WAIT_TIMEOUT do begin
      AppProcessMessage();
      WizardForm.Refresh();
    end;

    CloseHandle(ExecInfo.hProcess);
  end
  else begin
    HasEncounteredError := True;

    MsgBox('Something went wrong during the checking or installation of dependencies. Installation will be cancelled.', mbError, MB_OK);
    CancelWithoutPrompt := true;
    WizardForm.Close;
  end; 

  if not HasEncounteredError then begin
    WizardForm.StatusLabel.Caption := 'Adding certificate for Susi...'
    Exec(
      'certutil.exe', ExpandConstant('-addstore "TrustedPeople" "{tmp}\susi-gui-windows_0.1.0.0_x64.cer"'),
      '', SW_SHOW, ewWaitUntilTerminated, ResultCode
    );

    if ResultCode > 0 then begin
      HasEncounteredError := True;
        
      MsgBox('Something went wrong while adding certificate for Susi. Installation will be cancelled.', mbError, MB_OK);
      CancelWithoutPrompt := true;
      WizardForm.Close;
    end;
  end;
end;

procedure RunSusiAppPostInstall();
var
  ResultCode: Integer;
  ExecInfo: TShellExecuteInfo;
begin
  WizardForm.StatusLabel.Caption := 'Installing Susi...'

  ExecInfo.cbSize       := SizeOf(ExecInfo);
  ExecInfo.fMask        := SEE_MASK_NOCLOSEPROCESS;
  ExecInfo.Wnd          := 0;
  ExecInfo.lpFile       := ExpandConstant('{sys}\WindowsPowerShell\v1.0\powershell.exe');
  ExecInfo.lpParameters := ExpandConstant('Add-AppPackage -Path "{tmp}\susi-gui-windows_0.1.0.0_x64.msix"');
  ExecInfo.nShow        := SW_HIDE;

  if not ShellExecuteEx(ExecInfo) then begin
    MsgBox('Something went wrong while installing Susi. Installation will be cancelled.', mbError, MB_OK);
    CancelWithoutPrompt := true;
    WizardForm.Close;
  end;
end;

function InitializeSetup: Boolean;
begin
  CancelWithoutPrompt := false;
  Result := true;
end;

procedure CancelButtonClick(CurPageID: Integer; var Cancel, Confirm: Boolean);
begin
  if CurPageID = wpInstalling then begin
    Confirm := not CancelWithoutPrompt;
  end;
end;

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

