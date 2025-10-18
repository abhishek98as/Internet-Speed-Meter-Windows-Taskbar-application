# User Guide - SpeedoMeter

## Table of Contents
1. [Getting Started](#getting-started)
2. [First Launch](#first-launch)
3. [Understanding the Display](#understanding-the-display)
4. [Customization](#customization)
5. [Position Management](#position-management)
6. [System Tray](#system-tray)
7. [Performance Tuning](#performance-tuning)
8. [Troubleshooting](#troubleshooting)

## Getting Started

### System Requirements
- Windows 10 (version 1809 or later) or Windows 11
- .NET 8.0 Runtime (automatically included in self-contained builds)
- Active network connection

### Installation

**Option 1: Self-Contained Build (Recommended)**
1. Download `SpeedoMeter.exe` from the releases page
2. Run the executable - no installation required
3. The application will appear in your taskbar

**Option 2: Framework-Dependent Build**
1. Install .NET 8.0 Runtime if not already installed
2. Download and run `SpeedoMeter.exe`

## First Launch

When you first run SpeedoMeter:

1. **Initial Display**: The application appears with default green text showing `↓0.0MB/s ↑0.0MB/s`
2. **Default Position**: Top-left corner of the screen
3. **Default Colors**: Bright green (#00FF00) on transparent background
4. **System Tray**: A tray icon appears for quick access

### Quick Setup

Right-click the speed display → **Settings** to customize:
- Choose your preferred color scheme
- Select a font (Consolas recommended for clarity)
- Adjust font size for readability
- Position the display where you want it
- Lock the position to prevent accidental moves

## Understanding the Display

### Speed Format

The display shows two values:
- **↓ Download Speed**: Data coming into your computer
- **↑ Upload Speed**: Data leaving your computer

Example: `↓15.2MB/s ↑2.1MB/s`
- You're downloading at 15.2 megabytes per second
- You're uploading at 2.1 megabytes per second

### Units

**MB/s (Megabytes per second)**
- Default unit
- 1 MB/s = 8 Mbps
- More intuitive for file downloads
- Example: `↓12.5MB/s` means downloading 12.5 megabytes per second

**Mbps (Megabits per second)**
- Common in ISP advertising
- 8 Mbps = 1 MB/s
- Useful for comparing with internet plan speeds
- Example: `↓100Mbps` on a 100Mbps internet connection

**Toggle between units**: Right-click → Toggle Unit (MB/s ↔ Mbps)

### Reading Speeds

**Download Speeds**:
- `0-1 MB/s`: Light browsing, idle
- `1-10 MB/s`: Streaming, normal downloads
- `10-50 MB/s`: Large file downloads, HD streaming
- `50+ MB/s`: Very fast downloads, multiple streams

**Upload Speeds**:
- `0-0.5 MB/s`: Light activity
- `0.5-5 MB/s`: Video calls, file uploads
- `5+ MB/s`: Heavy uploading, streaming

## Customization

### Accessing Settings

**Method 1**: Right-click the display → Settings
**Method 2**: Double-click the system tray icon

### Appearance Settings

#### Text Color
Choose from 12 predefined colors:
- **Bright Green**: Classic terminal look (default)
- **Cyan**: Modern, high visibility
- **Magenta/Purple**: Stylish contrast
- **Yellow/Orange**: Warm tones
- **White**: Clean, minimal
- **Red**: Alert-style
- **Sky Blue**: Cool, easy on eyes
- **Pink**: Soft, distinctive
- **Light Gray**: Subtle

**Tip**: Choose colors that contrast well with your taskbar theme.

#### Font Selection
- **Consolas** (default): Excellent clarity, monospaced
- **Courier New**: Classic monospaced
- **Lucida Console**: Clear and readable
- **Cascadia Code/Mono**: Modern coding font
- **Any system font**: Hundreds of options

**Recommendation**: Stick with monospaced fonts for aligned numbers.

#### Font Size
- Range: 8-16 pixels
- **8-10px**: Compact, minimal space
- **11-12px**: Balanced (default: 11)
- **13-16px**: Large, easy to read

#### Background Opacity
- **0.0** (default): Fully transparent
- **0.1-0.5**: Subtle background
- **0.6-1.0**: More visible background

**Use case**: Add slight opacity if text blends too much with taskbar.

### Display Options

#### Speed Unit
- **MB/s**: Megabytes per second (file transfer speed)
- **Mbps**: Megabits per second (network speed)

#### Decimal Places
- **0**: Whole numbers only (`↓15MB/s`)
- **1**: One decimal (default: `↓15.2MB/s`)
- **2**: Two decimals (`↓15.23MB/s`)
- **3**: Three decimals (`↓15.234MB/s`)

**Recommendation**: 1-2 decimals for good balance.

#### Show/Hide Speeds
- **Show Download Speed**: Toggle download display
- **Show Upload Speed**: Toggle upload display
- **Both enabled**: See complete picture (recommended)
- **One enabled**: Minimize display width

**Note**: At least one must be enabled.

### Performance Settings

#### Update Interval
- **1ms**: Maximum responsiveness (default)
- **2-3ms**: Balanced performance
- **4-5ms**: Lower CPU usage

**CPU Impact**:
- 1ms: ~0.5-1% CPU (modern systems)
- 5ms: ~0.1-0.3% CPU

**Recommendation**: Start with 1ms; increase only if CPU usage concerns.

### System Settings

#### Auto-start with Windows
- **Enabled**: SpeedoMeter launches at Windows startup
- **Disabled**: Manual launch required

**How it works**: Adds registry entry to Windows startup folder.

## Position Management

### Moving the Display

1. **Unlock** if locked (Right-click → Lock Position to uncheck)
2. **Click and drag** the display to desired location
3. Release to drop
4. Position is automatically saved

### Locking Position

**Why lock?**
- Prevents accidental movement
- Keeps display in exact spot
- Professional look

**How to lock:**
- Right-click → Lock Position (checkmark appears)
- To unlock: Right-click → Lock Position (remove checkmark)

### Recommended Positions

**Top-right corner**: Classic, out of the way
**Near system tray**: Quick glance with clock
**Center top**: Prominent visibility
**Bottom corner**: Near taskbar start

**Tip**: Position where you naturally glance for info.

## System Tray

### Tray Icon Features

The system tray icon provides:
- **Single-click**: No action (shows tooltip)
- **Double-click**: Opens Settings
- **Right-click**: Context menu

### Context Menu

**Settings**: Open customization window
**Lock Position**: Toggle position lock
**Toggle Unit**: Switch MB/s ↔ Mbps
**Auto-start with Windows**: Enable/disable startup
**Exit**: Close application

## Performance Tuning

### Optimizing Resource Usage

**Low CPU Usage** (~0.5-1%):
- Already optimized out of the box
- Uses efficient asynchronous monitoring
- Minimal UI updates (20 FPS)

**Low Memory Usage** (~15-20 MB):
- Lightweight WPF application
- No memory leaks (proper disposal)
- Cached resources

### If You Experience Issues

**High CPU Usage**:
1. Open Settings
2. Increase Update Interval to 3-5ms
3. Check for other system issues

**Delayed Updates**:
1. Decrease Update Interval to 1ms
2. Ensure network adapter is functioning

## Troubleshooting

### Display Shows 0.0 MB/s

**Cause**: No network activity
**Solution**: 
- Normal when idle
- Open a website or download to see values
- Check network connection

### Can't Move Display

**Cause**: Position is locked
**Solution**: Right-click → Unlock Position

### Application Won't Start

**Possible causes**:
1. .NET 8.0 Runtime missing
   - **Solution**: Download from Microsoft
2. Already running
   - **Solution**: Check system tray, close and restart
3. Corrupted settings
   - **Solution**: Delete `%APPDATA%\SpeedoMeter\settings.json`

### Incorrect Speed Readings

**Cause**: Network adapter detection issue
**Solution**:
1. Restart application
2. Verify in Task Manager → Performance → Network
3. Ensure primary network adapter is active

### Auto-start Doesn't Work

**Cause**: Registry permission or incorrect path
**Solution**:
1. Run as Administrator once
2. Re-enable Auto-start in Settings
3. Check: `Win+R` → `shell:startup`

### Text Not Visible

**Cause**: Color matches taskbar
**Solution**: 
1. Open Settings
2. Change Text Color (try White or Cyan)
3. Adjust Background Opacity slightly

### Display Cut Off

**Cause**: Font too large or position off-screen
**Solution**:
1. Open Settings
2. Reduce Font Size
3. Reset position (drag back on-screen)

## Advanced Tips

### Best Practices

1. **Choose contrasting colors**: Ensure text stands out
2. **Lock after positioning**: Prevent accidental moves
3. **Monospaced fonts**: Better number alignment
4. **1-2 decimal places**: Good balance of precision
5. **Enable both speeds**: Complete network picture

### Keyboard Shortcuts

While SpeedoMeter doesn't have built-in shortcuts, you can:
- Pin to taskbar for quick access
- Create desktop shortcut
- Use Windows shortcuts to manage window

### Multiple Monitors

SpeedoMeter works across multiple monitors:
- Position on any monitor
- Drag between monitors freely
- Position persists per monitor

### Taskbar Configurations

**Auto-hide taskbar**: 
- SpeedoMeter may hide with taskbar
- Position outside taskbar area if needed

**Taskbar position** (top/bottom/left/right):
- Works with all positions
- Adjust placement accordingly

### Power Users

**Minimal display**:
- Show only download speed
- 0 decimal places
- Small font (8-9px)

**Maximum info**:
- Both speeds enabled
- 2-3 decimal places
- Larger font (13-14px)

## Settings File

### Location
`%APPDATA%\SpeedoMeter\settings.json`

### Manual Editing

You can manually edit settings:
1. Close SpeedoMeter
2. Navigate to settings file
3. Edit JSON carefully
4. Restart SpeedoMeter

**Example**:
```json
{
  "TextColor": "#00FFFF",
  "FontSize": 12.0,
  "DisplayUnit": "MBps"
}
```

### Backup Settings

**To backup**: Copy `settings.json`
**To restore**: Replace `settings.json` and restart

## Support

For issues not covered in this guide:
- Check GitHub Issues
- Create new issue with details
- Include settings.json if relevant

---

**Happy Monitoring!** 🚀
