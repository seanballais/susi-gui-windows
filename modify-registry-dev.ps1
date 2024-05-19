param (
	[Parameter(Mandatory=$true)][string]$BuildOutput,
	[string]$ModificationType = "Addition"
)

if ($ModificationType -eq "Addition") {
	$AbsoluteBuildOutputPath = Join-Path (Get-Item .).FullName $BuildOutput;
	if (-not (Test-Path $AbsoluteBuildOutputPath)) {
		Write-Host "Error: `"$AbsoluteBuildOutputPath`" does not exist.";
		Exit;
	}

	$ExePath = Join-Path $AbsoluteBuildOutputPath "Susi.exe";
	if (-not (Test-Path $ExePath)) {
		Write-Host "Error: `"$ExePath`" does not exist.";
		Exit;
	}

	# Add SeanBallais.Susi keys.
	if (-not (Test-Path 'HKCU:\Software\Classes\SeanBallais.Susi')) {
		New-Item -Path 'HKCU:\Software\Classes\SeanBallais.Susi' -Force | Out-Null
	}

	New-ItemProperty -Path 'HKCU:\Software\Classes\SeanBallais.Susi' -Name '(Default)' -Value "Susi" -PropertyType String -Force;

	if (-not (Test-Path 'HKCU:\Software\Classes\SeanBallais.Susi\DefaultIcon')) {
		New-Item -Path 'HKCU:\Software\Classes\SeanBallais.Susi\DefaultIcon' -Force | Out-Null
	}
	
	New-ItemProperty -Path 'HKCU:\Software\Classes\SeanBallais.Susi\DefaultIcon' -Name '(Default)' -Value "$ExePath,0" -PropertyType String -Force;

	# Add .ssef keys.
	if (-not (Test-Path 'HKCU:\Software\Classes\.ssef')) {
		New-Item -Path 'HKCU:\Software\Classes\.ssef' -Force | Out-Null
	}

	New-ItemProperty -Path 'HKCU:\Software\Classes\.ssef' -Name '(Default)' -Value "SeanBallais.Susi" -PropertyType String -Force;

	# Add Applications\SeanBallais.Susi keys.
	if (-not (Test-Path 'HKCU:\Software\Classes\Applications\SeanBallais.Susi')) {
		New-Item -Path 'HKCU:\Software\Classes\Applications\SeanBallais.Susi' -Force | Out-Null
	}

	if (-not (Test-Path 'HKCU:\Software\Classes\Applications\SeanBallais.Susi\Shell')) {
		New-Item -Path 'HKCU:\Software\Classes\Applications\SeanBallais.Susi\Shell' -Force | Out-Null
	}

	if (-not (Test-Path 'HKCU:\Software\Classes\Applications\SeanBallais.Susi\Shell\open')) {
		New-Item -Path 'HKCU:\Software\Classes\Applications\SeanBallais.Susi\Shell\open' -Force | Out-Null
	}

	if (-not (Test-Path 'HKCU:\Software\Classes\Applications\SeanBallais.Susi\Shell\open\command')) {
		New-Item -Path 'HKCU:\Software\Classes\Applications\SeanBallais.Susi\Shell\open\command' -Force | Out-Null
	}

	New-ItemProperty -Path 'HKCU:\Software\Classes\Applications\SeanBallais.Susi\Shell\open\command' -Name '(Default)' -Value "`"$ExePath`" `"`%1`""
}
elseif ($ModificationType -eq "Clean") {
	Remove-Item -Path 'HKCU:\Software\Classes\SeanBallais.Susi' -Force
	Remove-Item -Path 'HKCU:\Software\Classes\.ssef' -Force
	Remove-Item -Path 'HKCU:\Software\Classes\Applications\SeanBallais.Susi' -Force
}
else {
	Write-Host "Error: Unknown modification type specified.";
}
