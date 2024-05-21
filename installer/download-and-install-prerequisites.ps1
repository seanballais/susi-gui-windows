# Susi
# Copyright (C) 2024  Sean Francis N. Ballais
#
# This program is free software: you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation, either version 3 of the License, or
# (at your option) any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program.  If not, see <http://www.gnu.org/licenses/>.
param (
	[Parameter(Mandatory = $true)]
	[string] $TempDir
)

$ErrorActionPreference = "Stop";
$ProgressPreference = 'Continue';

Write-Host "[=] Downloading missing required dependencies...";

# Check if Visual C++ 2015-2022 Redistributable is installed.
$depName = "Visual C++ 2015-2022 Redistributable";
Write-Host "[=] Checking if $depName is installed already...";


$vc2022Version = $null;
try {
	$global:vc2022Version = (Get-ItemProperty -Path 'HKLM:\SOFTWARE\Classes\Installer\Dependencies\VC,redist.x64,amd64,14.*,bundle').Version;
} catch {
	$global:vc2022Version = $null;
}
if ($vc2022Version -eq $null) {
	Write-Host "$depName is not installed. Downloading and installing...";

	$installerPath = Join-Path "$TempDir" 'vc_redist.x64.exe'
	$uri = 'https://aka.ms/vs/17/release/vc_redist.x64.exe';
	Invoke-WebRequest $uri -Method 'GET' -OutFile "$installerPath";

	Start-Process "$installerPath" -ArgumentList "/install /passive /norestart" -Wait
} else {
	Write-Host "$depName exists. Skipping."
}

# Check if .NET 8 Desktop Runtime is installed.
$depName = ".NET 8 Desktop Runtime";
Write-Host "[=] Checking if $depName is installed already...";

$isNet8DesktopRTInstalled = $false;
try {
	if (Get-Command 'dotnet') {
		$isNet8DesktopRTInstalled = dotnet --list-runtimes | Select-String "Microsoft.WindowsDesktop.App 8." -Quiet;
	}
}
catch {
	$isNet8DesktopRTInstalled = $false;
}

if ($isNet8DesktopRTInstalled) {
	Write-Host "$depName exists. Skipping."	
} else {
	Write-Host "$depName is not installed. Downloading and installing...";

	$installerPath = Join-Path "$TempDir" 'windowsdesktop-runtime-8.0.5-win-x64.exe'
	$uri = 'https://download.visualstudio.microsoft.com/download/pr/0ff148e7-bbf6-48ed-bdb6-367f4c8ea14f/bd35d787171a1f0de7da6b57cc900ef5/windowsdesktop-runtime-8.0.5-win-x64.exe';
	Invoke-WebRequest $uri -Method 'GET' -OutFile "$installerPath";

	Start-Process "$installerPath" -ArgumentList "/install /passive /norestart" -Wait
}

# Check if Windows App SDK 1.5 Runtime is installed.
$depName = "Windows App SDK 1.5 Runtime";
Write-Host "[=] Checking if $depName is installed already...";

$appSDKPackages = ((Get-AppXPackage Microsoft.WindowsAppRuntime.1.5*) | Where-Object Architecture -eq "X64").PackageFullName;
if ($appSDKPackages -eq $null) {
	Write-Host "$depName is not installed. Downloading and installing...";

	$installerPath = Join-Path "$TempDir" 'windowsappruntimeinstall-x64.exe'
	$uri = 'https://aka.ms/windowsappsdk/1.5/1.5.240428000/windowsappruntimeinstall-x64.exe';
	Invoke-WebRequest $uri -Method 'GET' -OutFile "$installerPath";

	Start-Process "$installerPath" -ArgumentList "--quiet" -Wait
} else {
	Write-Host "$depName exists. Skipping."
}

Write-Host "[=] Finished downloading redistributables.";
