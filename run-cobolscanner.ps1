$exe = Join-Path $PSScriptRoot "Work\csharp\CobolScanner\publish\CobolScanner.exe"

if (-not (Test-Path $exe)) {
    Write-Host "CobolScanner.exe not found. Run build-cobolscanner.ps1 first." -ForegroundColor Yellow
    exit 1
}

Write-Host "Running CobolScanner..." -ForegroundColor Cyan
& "$exe" @args
