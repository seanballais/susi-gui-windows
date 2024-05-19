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

	# Add Susi.EncryptedFile keys.
	if (-not (Test-Path 'HKCU:\Software\Classes\Susi.EncryptedFile')) {
		New-Item -Path 'HKCU:\Software\Classes\Susi.EncryptedFile' -Force | Out-Null
	}

	New-ItemProperty -Path 'HKCU:\Software\Classes\Susi.EncryptedFile' -Name '(Default)' -Value "Susi Encrypted File" -PropertyType String -Force;

	if (-not (Test-Path 'HKCU:\Software\Classes\Susi.EncryptedFile\DefaultIcon')) {
		New-Item -Path 'HKCU:\Software\Classes\Susi.EncryptedFile\DefaultIcon' -Force | Out-Null
	}
	
	New-ItemProperty -Path 'HKCU:\Software\Classes\Susi.EncryptedFile\DefaultIcon' -Name '(Default)' -Value "$ExePath,0" -PropertyType String -Force;

	# Add .ssef keys.
	if (-not (Test-Path 'HKCU:\Software\Classes\.ssef')) {
		New-Item -Path 'HKCU:\Software\Classes\.ssef' -Force | Out-Null
	}

	New-ItemProperty -Path 'HKCU:\Software\Classes\.ssef' -Name '(Default)' -Value "Susi.EncryptedFile" -PropertyType String -Force;

	if (-not (Test-Path 'HKCU:\Software\Classes\.ssef\OpenWithProgids')) {
		New-Item -Path 'HKCU:\Software\Classes\.ssef\OpenWithProgids' -Force | Out-Null
	}

	New-ItemProperty -Path 'HKCU:\Software\Classes\.ssef\OpenWithProgids' -Name 'Susi.EncryptedFile' -Value "" -PropertyType String -Force;

	# Add Applications\Susi.exe keys.
	if (-not (Test-Path 'HKCU:\Software\Classes\Applications\Susi.exe')) {
		New-Item -Path 'HKCU:\Software\Classes\Applications\Susi.exe' -Force | Out-Null
	}

	if (-not (Test-Path 'HKCU:\Software\Classes\Applications\Susi.exe\SupportedTypes')) {
		New-Item -Path 'HKCU:\Software\Classes\Applications\Susi.exe\SupportedTypes' -Force | Out-Null
	}

	New-ItemProperty -Path 'HKCU:\Software\Classes\Applications\Susi.exe\SupportedTypes' -Name ".ssef" -Value "" -PropertyType String -Force;

	if (-not (Test-Path 'HKCU:\Software\Classes\Applications\Susi.exe\Shell')) {
		New-Item -Path 'HKCU:\Software\Classes\Applications\Susi.exe\Shell' -Force | Out-Null
	}

	if (-not (Test-Path 'HKCU:\Software\Classes\Applications\Susi.exe\Shell\open')) {
		New-Item -Path 'HKCU:\Software\Classes\Applications\Susi.exe\Shell\open' -Force | Out-Null
	}

	if (-not (Test-Path 'HKCU:\Software\Classes\Applications\Susi.exe\Shell\open\command')) {
		New-Item -Path 'HKCU:\Software\Classes\Applications\Susi.exe\Shell\open\command' -Force | Out-Null
	}

	New-ItemProperty -Path 'HKCU:\Software\Classes\Applications\Susi.exe\Shell\open\command' -Name '(Default)' -Value "`"$ExePath`" `"`%1`""
}
elseif ($ModificationType -eq "Clean") {
	Remove-Item -Path 'HKCU:\Software\Classes\Susi.EncryptedFile' -Force
	Remove-Item -Path 'HKCU:\Software\Classes\.ssef' -Force
	Remove-Item -Path 'HKCU:\Software\Classes\Applications\Susi.exe' -Force
}
else {
	Write-Host "Error: Unknown modification type specified.";
}
