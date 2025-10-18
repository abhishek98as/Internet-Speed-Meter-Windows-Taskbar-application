# SpeedoMeter Direct Run Script
# This script runs the published executable without installation

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  SpeedoMeter Launcher" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$exePath = "bin\Release\net8.0-windows\win-x64\publish\SpeedoMeter.exe"

if (-not (Test-Path $exePath)) {
    Write-Host "SpeedoMeter.exe not found!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please publish the application first:" -ForegroundColor Yellow
    Write-Host "dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true" -ForegroundColor Cyan
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

$fileInfo = Get-Item $exePath
Write-Host "Found SpeedoMeter:" -ForegroundColor Green
Write-Host "  Location: $($fileInfo.FullName)" -ForegroundColor Cyan
Write-Host "  Size: $([math]::Round($fileInfo.Length / 1MB, 2)) MB" -ForegroundColor Cyan
Write-Host "  Modified: $($fileInfo.LastWriteTime)" -ForegroundColor Cyan
Write-Host ""
Write-Host "Launching SpeedoMeter..." -ForegroundColor Yellow

Start-Process -FilePath $fileInfo.FullName

Write-Host "SpeedoMeter launched!" -ForegroundColor Green
Write-Host ""
Write-Host "Look at your taskbar for the network speed display." -ForegroundColor Yellow
Write-Host "Right-click the display to access settings and configuration." -ForegroundColor Yellow
