# Configuration
# Update these variables to match your specific project folder names
$BlazorProjectName = "BlazorLib"         # The folder name of your Blazor WASM project
$DesktopProjectName = "CobolStudioDesktop" # The folder name of your Desktop project
$Framework = "net10.0"                      # The .NET version (check your .csproj)
$Configuration = "Release"                 # Build configuration

# Paths
$LibsDir = '..'
$ProjectsDir = '../../cobol-studio-desktop'
$BlazorProjectDir = Join-Path $LibsDir $BlazorProjectName
$SourceDir = Join-Path $BlazorProjectDir "bin\$Configuration\$Framework\browser-wasm\publish\wwwroot"
$DestDir = Join-Path $ProjectsDir "$DesktopProjectName\wwwroot"

echo $SourceDir
echo $DestDir

# 1. Publish the Blazor Project
# Write-Host "Publishing $BlazorProjectName..." -ForegroundColor Cyan
# if (Test-Path $BlazorProjectDir) {
#     Push-Location $BlazorProjectDir
#     dotnet publish -c $Configuration
#     if ($LASTEXITCODE -ne 0) { Write-Error "Publish failed."; Pop-Location; exit }
#     Pop-Location
# } else {
#     Write-Error "Blazor project directory not found: $BlazorProjectDir"
#     exit
# }

# 2. Clean the Desktop wwwroot directory
Write-Host "Cleaning destination: $DestDir" -ForegroundColor Yellow
if (Test-Path $DestDir) {
    # Remove contents but keep the directory
    Remove-Item -Path "$DestDir\*" -Recurse -Force
} else {
    New-Item -ItemType Directory -Path $DestDir | Out-Null
}

# 3. Copy the published files
Write-Host "Copying files from publish directory..." -ForegroundColor Cyan
Copy-Item -Path "$SourceDir\*" -Destination $DestDir -Recurse -Force

Write-Host "Deployment to Desktop wwwroot complete!" -ForegroundColor Green
