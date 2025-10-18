# Icon Generator for SpeedoMeter
# Converts SVG to ICO format with multiple resolutions

try:
    from PIL import Image
    import cairosvg
    import io
    print("All required packages are installed!")
except ImportError as e:
    print("Missing required package. Installing...")
    print("\nPlease run:")
    print("pip install Pillow cairosvg")
    import sys
    sys.exit(1)

def create_ico_from_svg(svg_path, ico_path):
    """Convert SVG to ICO with multiple sizes"""
    
    # Icon sizes for Windows (standard sizes)
    sizes = [16, 32, 48, 64, 128, 256]
    images = []
    
    print(f"Converting {svg_path} to {ico_path}...")
    
    for size in sizes:
        print(f"  Generating {size}x{size}...")
        
        # Convert SVG to PNG at specific size
        png_data = cairosvg.svg2png(
            url=svg_path,
            output_width=size,
            output_height=size
        )
        
        # Load PNG into PIL Image
        image = Image.open(io.BytesIO(png_data))
        images.append(image)
    
    # Save as ICO with all sizes
    print(f"  Saving ICO file...")
    images[0].save(
        ico_path,
        format='ICO',
        sizes=[(img.width, img.height) for img in images],
        append_images=images[1:]
    )
    
    print(f"✓ Icon created successfully: {ico_path}")
    print(f"  Sizes included: {', '.join([f'{s}x{s}' for s in sizes])}")

if __name__ == "__main__":
    import os
    
    # Get the script directory
    script_dir = os.path.dirname(os.path.abspath(__file__))
    resources_dir = os.path.join(script_dir, "Resources")
    
    svg_file = os.path.join(resources_dir, "speedometer_icon.svg")
    ico_file = os.path.join(resources_dir, "app.ico")
    
    if not os.path.exists(svg_file):
        print(f"Error: SVG file not found: {svg_file}")
        import sys
        sys.exit(1)
    
    create_ico_from_svg(svg_file, ico_file)
    print("\n✓ Icon generation complete!")
    print(f"\nThe icon has been saved to: {ico_file}")
    print("The application will now use this icon for:")
    print("  - Application window")
    print("  - System tray")
    print("  - Desktop shortcut")
    print("  - Start menu")
    print("  - Installer")
