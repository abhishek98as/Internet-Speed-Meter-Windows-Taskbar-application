# SpeedoMeter Installer Build Script
# This script compiles the Inno Setup installer

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  SpeedoMeter Installer Builder" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if Inno Setup is installed
$innoSetupPaths = @(
    "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
    "${env:ProgramFiles}\Inno Setup 6\ISCC.exe",
    "${env:ProgramFiles(x86)}\Inno Setup 5\ISCC.exe",
    "${env:ProgramFiles}\Inno Setup 5\ISCC.exe"
)

$isccPath = $null
foreach ($path in $innoSetupPaths) {
    if (Test-Path $path) {
        $isccPath = $path
        Write-Host "Found Inno Setup at: $path" -ForegroundColor Green
        break
    }
}

if (-not $isccPath) {
    Write-Host "Inno Setup not found!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please install Inno Setup from:" -ForegroundColor Yellow
    Write-Host "https://jrsoftware.org/isinfo.php" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "After installation, run this script again." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Alternatively, you can run the published executable directly from:" -ForegroundColor Yellow
    Write-Host "bin\Release\net8.0-windows\win-x64\publish\SpeedoMeter.exe" -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

# Check if publish folder exists
$publishFolder = "bin\Release\net8.0-windows\win-x64\publish"
if (-not (Test-Path $publishFolder)) {
    Write-Host "Published files not found!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please publish the application first:" -ForegroundColor Yellow
    Write-Host "dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true" -ForegroundColor Cyan
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "Published files found" -ForegroundColor Green
Write-Host ""

# Check if installer script exists
if (-not (Test-Path "installer_script.iss")) {
    Write-Host "Installer script not found!" -ForegroundColor Red
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "Installer script found" -ForegroundColor Green
Write-Host ""

# Create installer directory if it doesn't exist
if (-not (Test-Path "installer")) {
    New-Item -ItemType Directory -Path "installer" | Out-Null
    Write-Host "Created installer output directory" -ForegroundColor Green
}

# Compile the installer
Write-Host "Building installer..." -ForegroundColor Yellow
Write-Host ""

try {
    & $isccPath "installer_script.iss"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Green
        Write-Host "  Installer built successfully!" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Green
        Write-Host ""
        
        # Find the installer
        $installer = Get-ChildItem -Path "installer" -Filter "SpeedoMeter_Setup*.exe" | Select-Object -First 1
        
        if ($installer) {
            Write-Host "Installer location:" -ForegroundColor Green
            Write-Host "  $($installer.FullName)" -ForegroundColor Cyan
            Write-Host ""
            Write-Host "Installer size: $([math]::Round($installer.Length / 1MB, 2)) MB" -ForegroundColor Green
            Write-Host ""
            
            # Ask if user wants to run the installer
            $response = Read-Host "Do you want to run the installer now? (Y/N)"
            if ($response -eq 'Y' -or $response -eq 'y') {
                Write-Host ""
                Write-Host "Launching installer..." -ForegroundColor Yellow
                Start-Process -FilePath $installer.FullName
                Write-Host "Installer launched!" -ForegroundColor Green
            }
            else {
                Write-Host ""
                Write-Host "You can run the installer manually from:" -ForegroundColor Yellow
                Write-Host "$($installer.FullName)" -ForegroundColor Cyan
            }
        }
    }
    else {
        Write-Host ""
        Write-Host "Installer build failed!" -ForegroundColor Red
        Write-Host "Check the output above for errors." -ForegroundColor Yellow
    }
}
catch {
    Write-Host ""
    Write-Host "Error building installer: $_" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""
Write-Host "Press Enter to exit" -ForegroundColor Gray
Read-Host
