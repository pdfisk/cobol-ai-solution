# Build and publish CobolAiGui (Blazor WASM) and copy output to CobolAiDesktop wwwroot
param(
    [string]$Configuration = "Debug",
    [string]$Framework = "net10.0"
)

$BlazorProjectDir  = Join-Path $PSScriptRoot "CobolAiGui"
$DesktopWwwroot    = Join-Path $PSScriptRoot "CobolAiDesktop\bin\$Configuration\net10.0-windows10.0.19041.0\win-x64\wwwroot"
$PublishWwwroot    = Join-Path $BlazorProjectDir "bin\$Configuration\$Framework\publish\wwwroot"

# 1. Publish Blazor WASM
Write-Host "Publishing CobolAiGui ($Configuration)..." -ForegroundColor Cyan
dotnet publish "$BlazorProjectDir\CobolAiGui.csproj" --configuration $Configuration --framework $Framework

if ($LASTEXITCODE -ne 0) {
    Write-Host "Publish failed." -ForegroundColor Red
    exit $LASTEXITCODE
}

# 2. Clean Desktop wwwroot
Write-Host "Cleaning: $DesktopWwwroot" -ForegroundColor Yellow
if (Test-Path $DesktopWwwroot) {
    Remove-Item -Path "$DesktopWwwroot\*" -Recurse -Force
} else {
    New-Item -ItemType Directory -Path $DesktopWwwroot | Out-Null
}

# 3. Copy published output to Desktop wwwroot
Write-Host "Copying published files to Desktop wwwroot..." -ForegroundColor Cyan
Copy-Item -Path "$PublishWwwroot\*" -Destination $DesktopWwwroot -Recurse -Force

Write-Host "Done." -ForegroundColor Green
