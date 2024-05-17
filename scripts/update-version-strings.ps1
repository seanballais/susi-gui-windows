cls

Write-Host "Updating version strings...";

Write-Host "[=] Getting the current version specified in the Susi GUI Windows project...";

$projectDirPath = (Get-Item .).FullName;

Write-Host "Current Working Directory: $projectDirPath";

$projectFileName = "susi-gui-windows.csproj";
$projectPath = Join-Path -Path $projectDirPath -ChildPath $projectFileName;

Write-Host "Project File Found: $projectPath"

[xml] $projectContents = Get-Content -Path $projectPath;

$projectVersionPrefix = $projectContents.SelectSingleNode("//Project/PropertyGroup[1]/VersionPrefix").InnerText;
$projectVersionSuffix = $projectContents.SelectSingleNode("//Project/PropertyGroup[1]/VersionSuffix").InnerText;
$projectVersion = "$projectVersionPrefix-$projectVersionSuffix";

Write-Host "Found Version: $projectVersion";

# TODO: Update version string in Package.appxmanifest.

Write-Host "[=] Updating version string in the installer project..."

$installerDirPath = (Get-Item ../installer).FullName;
cd $installerDirPath;

$cwd = (Get-Item .).FullName;
Write-Host "Current Working Directory: $cwd";

$installerProjectFileName = "installer.wixproj";
$installerProjectPath = Join-Path -Path $installerDirPath -ChildPath $installerProjectFileName;

Write-Host "Installer Project File to Update: $installerProjectPath";

[xml] $installerProjectContents = Get-Content -Path $installerProjectPath;
$installerProjectConstants = $installerProjectContents.SelectSingleNode("//Project/Target[@Name='PreBuild']/PropertyGroup[1]/DefineConstants");
$installerProjectConstants.InnerText = "VersionPrefix=$projectVersionPrefix;VersionSuffix=$projectVersionSuffix;Version=`$(VersionPrefix)-`$(VersionSuffix)";

$installerProjectContents.Save($installerProjectPath);

Write-Host "Updated installer project file."
