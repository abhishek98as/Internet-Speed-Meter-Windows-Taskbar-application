#!/usr/bin/env python3
"""
SpeedoMeter Icon Generator
Creates a beautiful speedometer icon for the application
"""

try:
    from PIL import Image, ImageDraw, ImageFont
    import os
except ImportError:
    print("Installing required package...")
    import subprocess
    subprocess.check_call(["pip", "install", "Pillow"])
    from PIL import Image, ImageDraw, ImageFont
    import os

def create_speedometer_icon(output_path, size=256):
    """Create a beautiful speedometer icon"""
    
    # Create image with transparency
    img = Image.new('RGBA', (size, size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    
    # Center point
    cx, cy = size // 2, size // 2
    
    # Scale everything based on size
    margin = max(int(size * 0.08), 2)
    border_width = max(int(size * 0.015), 1)
    
    # Draw background circle with blue gradient effect
    for i in range(size//2 - margin, 0, -2):
        alpha = int(255 * (i / (size//2 - margin)))
        color = (30 + i//5, 58 + i//2, min(255, 138 + i), alpha)
        if cx-i >= 0 and cy-i >= 0:
            draw.ellipse([(cx-i, cy-i), (cx+i, cy+i)], fill=color)
    
    # Draw outer border
    border_margin = int(size * 0.08)
    if size - 2*border_margin > 0:
        draw.ellipse([(border_margin, border_margin), (size-border_margin, size-border_margin)], 
                     outline=(30, 64, 175, 255), width=border_width)
    
    # Draw speedometer arc background (dark gray)
    arc_margin = int(size * 0.2)
    arc_y = int(cy * 0.6)
    arc_bottom = int(cy * 1.3)
    if size >= 48 and arc_margin < (size - arc_margin):  # Only draw arcs for larger sizes with valid coordinates
        draw.arc([(arc_margin, arc_y), (size-arc_margin, arc_bottom)], 
                 180, 360, fill=(51, 65, 85, 255), width=max(2, size//15))
        
        # Draw speed indicator arc (green)
        draw.arc([(arc_margin, arc_y), (size-arc_margin, arc_bottom)], 
                 180, 300, fill=(16, 185, 129, 255), width=max(2, size//15))
    
    # Draw tick marks
    tick_positions = [
        (48, 158, 58, 150),
        (70, 98, 78, 105),
        (cx, 68, cx, 80),
        (186, 98, 178, 105),
        (208, 158, 198, 150)
    ]
    for x1, y1, x2, y2 in tick_positions:
        draw.line([(x1, y1), (x2, y2)], fill=(226, 232, 240, 255), width=4)
    
    # Draw center hub
    needle_base_y = int(cy * 1.23)
    draw.ellipse([(cx-15, needle_base_y-15), (cx+15, needle_base_y+15)], 
                 fill=(30, 41, 59, 255), outline=(71, 85, 105, 255), width=2)
    
    # Draw needle (red pointer)
    needle_tip_x = cx + 60
    needle_tip_y = cy - 30
    needle_points = [
        (cx-4, needle_base_y),
        (cx+4, needle_base_y),
        (needle_tip_x, needle_tip_y)
    ]
    draw.polygon(needle_points, fill=(239, 68, 68, 255))
    
    # Draw center dot
    draw.ellipse([(cx-8, needle_base_y-8), (cx+8, needle_base_y+8)], 
                 fill=(239, 68, 68, 255))
    draw.ellipse([(cx-4, needle_base_y-4), (cx+4, needle_base_y+4)], 
                 fill=(254, 242, 242, 255))
    
    # Draw download arrow (green)
    dl_x, dl_y = int(cx * 0.66), int(cy * 1.4)
    draw.line([(dl_x, dl_y-10), (dl_x, dl_y+10)], fill=(16, 185, 129, 255), width=6)
    draw.line([(dl_x-8, dl_y+4), (dl_x, dl_y+10)], fill=(16, 185, 129, 255), width=6)
    draw.line([(dl_x+8, dl_y+4), (dl_x, dl_y+10)], fill=(16, 185, 129, 255), width=6)
    
    # Draw upload arrow (blue)
    ul_x, ul_y = int(cx * 1.34), int(cy * 1.4)
    draw.line([(ul_x, ul_y+10), (ul_x, ul_y-10)], fill=(59, 130, 246, 255), width=6)
    draw.line([(ul_x-8, ul_y-4), (ul_x, ul_y-10)], fill=(59, 130, 246, 255), width=6)
    draw.line([(ul_x+8, ul_y-4), (ul_x, ul_y-10)], fill=(59, 130, 246, 255), width=6)
    
    # Draw text "SPEED"
    try:
        font = ImageFont.truetype("arial.ttf", 20)
    except:
        font = ImageFont.load_default()
    
    text = "SPEED"
    bbox = draw.textbbox((0, 0), text, font=font)
    text_width = bbox[2] - bbox[0]
    text_x = (size - text_width) // 2
    text_y = int(cy * 1.6)
    draw.text((text_x, text_y), text, fill=(226, 232, 240, 255), font=font)
    
    return img

def main():
    print("=" * 50)
    print("  SpeedoMeter Icon Generator")
    print("=" * 50)
    print()
    
    # Create Resources directory if it doesn't exist
    os.makedirs("Resources", exist_ok=True)
    
    # Generate icon at multiple sizes
    sizes = [256, 128, 64, 48, 32, 16]
    images = []
    
    for size in sizes:
        print(f"Generating {size}x{size} icon...")
        img = create_speedometer_icon("", size)
        images.append(img)
    
    # Save as ICO file
    ico_path = "Resources\\app.ico"
    print(f"\nSaving as {ico_path}...")
    images[0].save(
        ico_path,
        format='ICO',
        sizes=[(img.width, img.height) for img in images]
    )
    
    # Also save as PNG for preview
    png_path = "Resources\\app.png"
    images[0].save(png_path, format='PNG')
    
    print("\n" + "=" * 50)
    print("  Icon Generation Complete!")
    print("=" * 50)
    print(f"\n✓ ICO file: {ico_path}")
    print(f"✓ PNG preview: {png_path}")
    print(f"\nSizes included: {', '.join([f'{s}x{s}' for s in sizes])}")
    print("\nNext steps:")
    print("1. Rebuild: dotnet build -c Release")
    print("2. Republish: dotnet publish -c Release -r win-x64 --self-contained")
    print("3. Rebuild installer: .\\build_installer.ps1")

if __name__ == "__main__":
    main()
