$projectDir = Join-Path $PSScriptRoot "Work\csharp\CobolScanner"
$outputDir  = Join-Path $PSScriptRoot "Work\csharp\CobolScanner\publish"

Write-Host "Building CobolScanner..." -ForegroundColor Cyan

dotnet publish "$projectDir\CobolScanner.csproj" `
    --configuration Release `
    --runtime win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    --output "$outputDir"

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build succeeded. Output: $outputDir\CobolScanner.exe" -ForegroundColor Green
} else {
    Write-Host "Build failed." -ForegroundColor Red
    exit $LASTEXITCODE
}
