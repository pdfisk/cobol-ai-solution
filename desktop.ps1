# Launch CobolAiDesktop
param(
    [string]$Configuration = "Debug"
)

$ExePath = Join-Path $PSScriptRoot "CobolAiDesktop\bin\$Configuration\net10.0-windows10.0.19041.0\win-x64\CobolAiDesktop.exe"

if (-not (Test-Path $ExePath)) {
    Write-Host "Executable not found: $ExePath" -ForegroundColor Red
    Write-Host "Run build-cobolaigui.ps1 first to build the project." -ForegroundColor Yellow
    exit 1
}

Write-Host "Launching CobolAiDesktop ($Configuration)..." -ForegroundColor Cyan
Start-Process -FilePath $ExePath
