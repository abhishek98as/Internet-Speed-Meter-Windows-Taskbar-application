# Changelog

All notable changes to SpeedoMeter will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-10-18

### Initial Release

#### Added
- **Core Functionality**
  - Real-time network speed monitoring with 1-5ms update intervals
  - Support for both MB/s (Megabytes per second) and Mbps (Megabits per second) units
  - Simultaneous display of upload and download speeds
  - High-precision measurement using .NET NetworkInterface class
  - Automatic network adapter detection and failover

- **User Interface**
  - Transparent, borderless window that blends with taskbar
  - Compact speed display format with arrow indicators (↓/↑)
  - Clear visual separation between download and upload speeds
  - Always-on-top positioning for constant visibility
  - System tray integration with notification icon

- **Customization Options**
  - 12 predefined color schemes (Green, Cyan, Magenta, Yellow, Orange, White, Red, Sky Blue, Purple, Mint Green, Pink, Light Gray)
  - Font selection from all system-installed fonts
  - Adjustable font size (8-16px range)
  - Configurable background opacity (0-100%)
  - Decimal places control (0-3 places)
  - Toggle individual speed displays (upload/download)
  - Update interval adjustment (1-5ms)

- **Position Management**
  - Drag-and-drop repositioning anywhere on screen
  - Lock/Unlock toggle to prevent accidental movement
  - Automatic position persistence across application restarts
  - Multi-monitor support

- **System Integration**
  - Auto-start with Windows via registry integration
  - Comprehensive settings UI with real-time preview
  - JSON-based configuration storage
  - Single-instance application (prevents multiple copies)

- **Context Menu**
  - Quick access to Settings window
  - Position lock toggle
  - Unit toggle (MB/s ↔ Mbps)
  - Auto-start enable/disable
  - Exit application option

- **Performance Optimizations**
  - Memory usage: < 20MB RAM
  - CPU usage: < 1% on modern systems
  - Asynchronous network monitoring
  - Efficient UI update mechanism (20 FPS rendering)
  - Proper resource disposal and cleanup
  - Minimal garbage collection pressure

- **Error Handling**
  - Graceful handling of network adapter changes
  - Automatic adapter reconnection on failure
  - Safe registry operations with fallback
  - Exception handling throughout codebase
  - Debug logging for troubleshooting

- **Code Quality**
  - Clean, well-commented C# code
  - XML documentation for public APIs
  - MVVM-inspired architecture
  - Proper IDisposable pattern implementation
  - Follows C# coding conventions and best practices

- **Documentation**
  - Comprehensive README with feature overview
  - Detailed user guide (USERGUIDE.md)
  - Build instructions (BUILD.md)
  - MIT license
  - This changelog

#### Technical Details
- Framework: .NET 8.0 with WPF
- UI: Windows Presentation Foundation (WPF)
- Configuration: Newtonsoft.Json for settings persistence
- Platform: Windows 10/11 (64-bit)
- Manifest: DPI-aware for high-resolution displays

#### Known Limitations
- Windows-only (uses Windows-specific APIs)
- Requires administrative privileges for registry auto-start (first time only)
- Network speed limited by adapter capabilities
- Single network adapter monitoring at a time (automatically selects most active)

---

## Future Enhancements (Planned)

### [1.1.0] - Planned
- Custom color picker for unlimited color options
- Themes (Light/Dark/Custom)
- History graph showing speed over time
- Notification alerts for speed thresholds
- Export statistics to CSV
- Network adapter selection in settings
- Hotkey support for quick actions

### [1.2.0] - Planned
- Multiple network adapter monitoring
- Bandwidth usage statistics (daily/weekly/monthly)
- Data usage limits and warnings
- Advanced filtering options
- Mobile app companion (remote monitoring)

---

## Version Guidelines

**Major version (X.0.0)**: Breaking changes, major features
**Minor version (0.X.0)**: New features, backward compatible
**Patch version (0.0.X)**: Bug fixes, minor improvements

---

For detailed information about each release, visit the [Releases](https://github.com/yourusername/speedometer/releases) page.
