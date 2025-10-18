# SpeedoMeter - Windows Taskbar Network Speed Monitor

A production-ready, high-performance Windows application that displays real-time network upload and download speeds directly in your taskbar.

![SpeedoMeter](https://img.shields.io/badge/Platform-Windows-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![License](https://img.shields.io/badge/License-MIT-green)

## Features

### Core Functionality
- **Real-time Speed Monitoring**: Ultra-responsive network speed tracking with 1-5ms update intervals
- **Dual Unit Support**: Toggle between MB/s (Megabytes) and Mbps (Megabits)
- **Simultaneous Display**: Shows both upload and download speeds at once
- **High Precision**: Uses native .NET `NetworkInterface` class for accurate measurements

### Visual Customization
- **12 Predefined Color Schemes**: Bright Green, Cyan, Magenta, Yellow, Orange, White, Red, Sky Blue, Purple, Mint Green, Pink, Light Gray
- **Font Selection**: Choose from all system-installed fonts
- **Adjustable Font Size**: Range from 8-16px
- **Transparent Background**: Seamlessly blends with your taskbar
- **Opacity Control**: Adjustable background opacity (0-100%)

### User Interaction
- **Drag & Drop Positioning**: Move the display anywhere within your taskbar area
- **Lock/Unlock Toggle**: Prevent accidental movement when locked
- **Position Persistence**: Your preferred location is saved across restarts
- **Right-Click Context Menu**: Quick access to all features

### System Integration
- **Auto-Start**: Optional launch on Windows startup
- **System Tray Icon**: Minimizes to tray with full functionality
- **Low Resource Usage**: 
  - Memory: < 20MB RAM
  - CPU: < 1% average on modern systems
- **Optimized Performance**: Asynchronous operations and efficient data structures

## Installation

### 🎯 Easy Installation (Recommended)

**Download the Windows Installer** - Professional installer with setup wizard:

1. Download `SpeedoMeter_Setup_v1.0.0.exe`
2. Run the installer
3. Choose your installation preferences
4. Click Install and you're done!

**Features:**
- ✅ Custom installation path
- ✅ Desktop shortcut creation
- ✅ Auto-start with Windows option
- ✅ Start Menu integration
- ✅ Clean uninstaller with settings preservation

📖 **See [INSTALLATION.md](INSTALLATION.md) for detailed installation guide**

### 📋 Requirements
- Windows 10 (version 1809+) or Windows 11
- 64-bit operating system
- 50 MB free disk space
- Self-contained installer - **No .NET runtime required!**

### 🔧 Build from Source

1. **Clone the repository**
   ```bash
   git clone https://github.com/abhishek98as/Internet-Speed-Meter-Windows-Taskbar-application.git
   cd SpeedoMeter
   ```

2. **Build the project**
   ```bash
   dotnet build -c Release
   ```

3. **Run the application**
   ```bash
   dotnet run -c Release
   ```

   Or navigate to `bin\Release\net8.0-windows\` and run `SpeedoMeter.exe`

## Usage

### First Launch
1. The application will appear in your taskbar showing network speeds
2. Right-click the display to access the context menu
3. Select **Settings** to customize appearance and behavior

### 🎨 Enhanced Context Menu

Right-click the display for comprehensive quick access:

**⚡ Quick Configuration** (NEW!)
- **🎨 Color Scheme** - 12 colors with instant preview
- **📏 Font Size** - 9 preset sizes (8-16px)
- **🔢 Decimal Places** - 0-3 decimals
- **👁 Background Opacity** - 11 levels (0-100%)

**📊 Display Options** (NEW!)
- Toggle download speed display
- Toggle upload speed display

**Standard Options**
- **⚙ Settings** - Full configuration window
- **🔒 Lock Position** - Prevent accidental movement
- **🔄 Toggle Unit** - Switch MB/s ↔ Mbps instantly
- **🚀 Auto-start** - Launch with Windows
- **ℹ About** - Version and info
- **❌ Exit** - Close application

📖 **See [CONTEXT_MENU_GUIDE.md](CONTEXT_MENU_GUIDE.md) for detailed context menu features**

### Customization

#### Appearance Settings
- **Text Color**: Choose from 12 predefined colors
- **Font Family**: Select any installed system font (monospace recommended)
- **Font Size**: Adjust from 8-16px
- **Background Opacity**: Control transparency (0 = fully transparent)

#### Display Options
- **Speed Unit**: Choose MB/s or Mbps
- **Decimal Places**: 0-3 decimal places for precision
- **Show Download Speed**: Toggle download speed display
- **Show Upload Speed**: Toggle upload speed display

#### Performance Settings
- **Update Interval**: 1-5ms (lower = more responsive, higher CPU usage)

#### System Settings
- **Start with Windows**: Automatically launch on system startup

### Positioning the Display
1. **Unlock** the position via context menu (if locked)
2. **Click and drag** the speed display to your preferred location
3. **Lock** the position to prevent accidental movement
4. Your position is automatically saved

## Technical Details

### Architecture
- **Framework**: .NET 8.0 with WPF
- **Language**: C# 12
- **Pattern**: MVVM-inspired architecture
- **Configuration**: JSON-based settings storage

### Key Components
1. **NetworkMonitor**: High-performance speed monitoring service
   - Uses `System.Net.NetworkInformation.NetworkInterface`
   - Asynchronous background processing
   - Automatic adapter failover

2. **MainWindow**: Transparent taskbar overlay
   - Borderless, always-on-top window
   - Double-buffered rendering
   - Efficient UI updates (20 FPS)

3. **SettingsWindow**: Comprehensive configuration UI
   - Real-time preview
   - Input validation
   - Settings persistence

4. **AutoStartManager**: Windows registry integration
   - Safe registry operations
   - Auto-start toggle functionality

### Performance Optimizations
- **Efficient Timer Management**: Separate threads for network monitoring and UI updates
- **Resource Pooling**: Cached brushes and fonts
- **Minimal Allocations**: Reused data structures
- **Proper Disposal**: IDisposable pattern throughout
- **Exception Handling**: Graceful degradation on errors

### File Locations
- **Settings**: `%APPDATA%\SpeedoMeter\settings.json`
- **Auto-start Registry**: `HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run`

## Configuration File

Settings are stored in JSON format at `%APPDATA%\SpeedoMeter\settings.json`:

```json
{
  "WindowLeft": 100.0,
  "WindowTop": 100.0,
  "IsPositionLocked": false,
  "TextColor": "#00FF00",
  "FontFamily": "Consolas",
  "FontSize": 11.0,
  "DisplayUnit": "MBps",
  "AutoStart": false,
  "UpdateIntervalMs": 1,
  "ShowUploadSpeed": true,
  "ShowDownloadSpeed": true,
  "DecimalPlaces": 1,
  "BackgroundOpacity": 0.0
}
```

## Troubleshooting

### Application doesn't start
- Ensure .NET 8.0 Runtime is installed
- Run as Administrator if permission issues occur

### No network speed displayed
- Check if network adapter is active
- Verify internet connectivity
- Try restarting the application

### High CPU usage
- Increase update interval in settings (Performance → Update Interval)
- Recommended: 1-2ms for most systems

### Auto-start not working
- Verify registry permissions
- Manually check: `Win+R` → `shell:startup`

### Speed readings seem incorrect
- Compare with Task Manager → Performance → Network
- Ensure correct network adapter is active
- Restart application to reinitialize monitoring

## Development

### Project Structure
```
SpeedoMeter/
├── Models/
│   └── AppSettings.cs          # Configuration model
├── Services/
│   ├── NetworkMonitor.cs       # Speed monitoring service
│   └── AutoStartManager.cs     # Registry management
├── MainWindow.xaml/cs          # Main overlay window
├── SettingsWindow.xaml/cs      # Configuration UI
├── App.xaml/cs                 # Application entry point
└── SpeedoMeter.csproj          # Project file
```

### Building for Distribution

```bash
# Publish as self-contained executable
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

# Output will be in: bin\Release\net8.0-windows\win-x64\publish\
```

## Contributing

Contributions are welcome! Please follow these guidelines:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Code Standards
- Follow C# coding conventions
- Add XML documentation for public APIs
- Write clean, maintainable code
- Test on Windows 10 and 11

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Built with .NET 8.0 and WPF
- Uses Newtonsoft.Json for configuration management
- Inspired by various network monitoring tools

## Support

For issues, questions, or suggestions:
- Open an issue on GitHub
- Check existing issues for solutions

## Version History

### v1.0.0 (Initial Release)
- Real-time network speed monitoring (1-5ms intervals)
- Dual unit support (MB/s and Mbps)
- 12 predefined color schemes
- Customizable fonts and sizes
- Drag & drop positioning with lock/unlock
- Auto-start with Windows
- System tray integration
- Comprehensive settings UI
- JSON configuration persistence
- Low resource usage (< 20MB RAM, < 1% CPU)

---

**Made with ❤️ for Windows power users**
#   I n t e r n e t - S p e e d - M e t e r - W i n d o w s - T a s k b a r - a p p l i c a t i o n 
 
 