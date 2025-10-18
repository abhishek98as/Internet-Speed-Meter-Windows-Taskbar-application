# Quick Start Guide

## 🚀 Get SpeedoMeter Running in 60 Seconds

### Step 1: Build the Application

Open PowerShell and run:

```powershell
cd X:\SpeedoMeter
dotnet build -c Release
```

### Step 2: Run the Application

```powershell
cd bin\Release\net8.0-windows
.\SpeedoMeter.exe
```

Or simply:

```powershell
dotnet run -c Release
```

### Step 3: You're Done! ✅

SpeedoMeter is now running and showing your network speeds in the taskbar.

---

## First Time Setup (30 seconds)

1. **Right-click** the speed display
2. Select **Settings**
3. Choose your preferences:
   - Pick a color you like
   - Adjust font size if needed
   - Set your preferred unit (MB/s or Mbps)
4. Click **Save**

---

## Quick Tips

### Move the Display
- **Unlock** it first (Right-click → Lock Position)
- **Drag** it to where you want
- **Lock** it again to keep it there

### Change Units
Right-click → **Toggle Unit (MB/s ↔ Mbps)**

### Auto-start with Windows
Right-click → **Auto-start with Windows** (checkmark it)

---

## Publishing a Standalone Executable

Create a single-file executable that works without .NET installed:

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

**Output**: `bin\Release\net8.0-windows\win-x64\publish\SpeedoMeter.exe`

This single file can be copied anywhere and run immediately!

---

## Troubleshooting

**"Can't find .NET SDK"**
→ Download from https://dotnet.microsoft.com/download/dotnet/8.0

**Display shows 0.0 MB/s**
→ Normal when idle. Open a website to see activity.

**Can't move the display**
→ Right-click and uncheck "Lock Position"

---

## Next Steps

- Read the full [User Guide](USERGUIDE.md) for detailed features
- Check [Build Instructions](BUILD.md) for advanced options
- See [README.md](README.md) for complete documentation

---

**Enjoy monitoring your network speeds!** 📊
