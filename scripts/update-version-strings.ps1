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

Write-Host "TODO: Update version string in Package.appxmanifest.";
