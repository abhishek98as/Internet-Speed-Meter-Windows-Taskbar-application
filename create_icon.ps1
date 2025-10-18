# SpeedoMeter Icon Generator
# Creates a beautiful icon using .NET and Windows APIs

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  SpeedoMeter Icon Generator" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Create a simple ICO file using .NET Drawing
Add-Type -AssemblyName System.Drawing

$iconSizes = @(16, 32, 48, 64, 128, 256)
$outputPath = "Resources\app.ico"

Write-Host "Generating speedometer icon..." -ForegroundColor Yellow
Write-Host ""

# Create bitmap for the largest size
$size = 256
$bmp = New-Object System.Drawing.Bitmap($size, $size)
$graphics = [System.Drawing.Graphics]::FromImage($bmp)
$graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

# Background - Blue gradient circle
$centerX = $size / 2
$centerY = $size / 2
$radius = $size / 2 - 10

$brushBg = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
    [System.Drawing.Point]::new(0, 0),
    [System.Drawing.Point]::new($size, $size),
    [System.Drawing.Color]::FromArgb(255, 30, 58, 138),
    [System.Drawing.Color]::FromArgb(255, 59, 130, 246)
)
$graphics.FillEllipse($brushBg, 10, 10, $size - 20, $size - 20)

# Inner circle border
$penBorder = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(255, 30, 64, 175), 3)
$graphics.DrawEllipse($penBorder, 20, 20, $size - 40, $size - 40)

# Speedometer arc background (dark)
$penArcBg = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(255, 51, 65, 85), 16)
$penArcBg.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
$penArcBg.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
$graphics.DrawArc($penArcBg, 50, 80, $size - 100, $size - 100, 180, 180)

# Speed indicator arc (green)
$penArcGreen = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(255, 16, 185, 129), 16)
$penArcGreen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
$penArcGreen.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
$graphics.DrawArc($penArcGreen, 50, 80, $size - 100, $size - 100, 180, 120)

# Center hub
$brushHub = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 30, 41, 59))
$graphics.FillEllipse($brushHub, $centerX - 12, 158 - 12, 24, 24)

# Needle (red)
$penNeedle = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(255, 239, 68, 68), 4)
$graphics.DrawLine($penNeedle, $centerX, 158, $centerX + 50, 100)

# Center dot
$brushCenter = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 239, 68, 68))
$graphics.FillEllipse($brushCenter, $centerX - 6, 158 - 6, 12, 12)

# Download arrow (green)
$penDownload = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(255, 16, 185, 129), 5)
$penDownload.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
$penDownload.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
$graphics.DrawLine($penDownload, 85, 175, 85, 195)
$graphics.DrawLine($penDownload, 80, 190, 85, 195)
$graphics.DrawLine($penDownload, 90, 190, 85, 195)

# Upload arrow (blue)
$penUpload = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(255, 59, 130, 246), 5)
$penUpload.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
$penUpload.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
$graphics.DrawLine($penUpload, 171, 195, 171, 175)
$graphics.DrawLine($penUpload, 166, 180, 171, 175)
$graphics.DrawLine($penUpload, 176, 180, 171, 175)

# Text "SPEED"
$font = New-Object System.Drawing.Font("Arial", 18, [System.Drawing.FontStyle]::Bold)
$brushText = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 226, 232, 240))
$textFormat = New-Object System.Drawing.StringFormat
$textFormat.Alignment = [System.Drawing.StringAlignment]::Center
$graphics.DrawString("SPEED", $font, $brushText, $centerX, 210, $textFormat)

# Save as PNG first
$pngPath = "Resources\app.png"
$bmp.Save($pngPath, [System.Drawing.Imaging.ImageFormat]::Png)

Write-Host "✓ Created 256x256 PNG icon" -ForegroundColor Green

# Now convert to ICO using the PNG
try {
    # Convert PNG to ICO
    $icon = [System.Drawing.Icon]::FromHandle($bmp.GetHicon())
    $fileStream = [System.IO.File]::OpenWrite($outputPath)
    $icon.Save($fileStream)
    $fileStream.Close()
    
    Write-Host "✓ Created ICO file: $outputPath" -ForegroundColor Green
}
catch {
    Write-Host "Warning: Could not create ICO file directly" -ForegroundColor Yellow
    Write-Host "The PNG file has been created at: $pngPath" -ForegroundColor Yellow
    Write-Host "You can use an online converter to create the ICO file" -ForegroundColor Yellow
}

# Cleanup
$graphics.Dispose()
$bmp.Dispose()

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Icon Generation Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Icon saved to: $outputPath" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Rebuild the application" -ForegroundColor White
Write-Host "2. Republish with: dotnet publish -c Release" -ForegroundColor White
Write-Host "3. Rebuild installer with: .\build_installer.ps1" -ForegroundColor White
