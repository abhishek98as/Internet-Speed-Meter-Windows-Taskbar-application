# Build Instructions

## Prerequisites

- **Windows 10/11** (64-bit recommended)
- **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Visual Studio 2022** or **Visual Studio Code** (optional, for development)

## Quick Build

### Command Line Build

1. **Open PowerShell or Command Prompt**

2. **Navigate to the project directory**
   ```powershell
   cd X:\SpeedoMeter
   ```

3. **Restore dependencies**
   ```powershell
   dotnet restore
   ```

4. **Build the project**
   ```powershell
   dotnet build -c Release
   ```

5. **Run the application**
   ```powershell
   dotnet run -c Release
   ```

   Or navigate to `bin\Release\net8.0-windows\` and run `SpeedoMeter.exe`

### Visual Studio Build

1. **Open the solution**
   - Double-click `SpeedoMeter.csproj` or open Visual Studio and select "Open a project or solution"

2. **Select Release configuration**
   - In the toolbar, change from "Debug" to "Release"

3. **Build the solution**
   - Press `Ctrl+Shift+B` or select "Build → Build Solution"

4. **Run the application**
   - Press `F5` or click the "Start" button

## Publishing for Distribution

### Self-Contained Executable

Create a single-file executable that includes the .NET runtime:

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

**Output location**: `bin\Release\net8.0-windows\win-x64\publish\SpeedoMeter.exe`

**Benefits**:
- No .NET runtime installation required
- Single executable file
- Ready for distribution

### Framework-Dependent Executable

Create a smaller executable that requires .NET 8.0 runtime:

```powershell
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true
```

**Output location**: `bin\Release\net8.0-windows\win-x64\publish\SpeedoMeter.exe`

**Benefits**:
- Smaller file size
- Faster build time
- Requires .NET 8.0 runtime on target machine

## Build Configurations

### Debug Configuration
- Includes debugging symbols
- No optimization
- Useful for development and troubleshooting

```powershell
dotnet build -c Debug
```

### Release Configuration
- Optimized for performance
- Smaller file size
- Recommended for distribution

```powershell
dotnet build -c Release
```

## Clean Build

Remove all build artifacts:

```powershell
dotnet clean
```

## Troubleshooting

### "SDK not found" error
- Install .NET 8.0 SDK from [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/8.0)
- Verify installation: `dotnet --version`

### Build errors
1. **Clean the solution**:
   ```powershell
   dotnet clean
   ```

2. **Restore NuGet packages**:
   ```powershell
   dotnet restore
   ```

3. **Rebuild**:
   ```powershell
   dotnet build -c Release
   ```

### NuGet package restore issues
- Clear NuGet cache:
  ```powershell
  dotnet nuget locals all --clear
  ```
- Restore packages:
  ```powershell
  dotnet restore
  ```

## Project Dependencies

The project uses the following NuGet packages:
- **Newtonsoft.Json** (13.0.3) - JSON serialization for settings

Dependencies are automatically restored during build.

## Development Setup

### Recommended Tools

- **Visual Studio 2022** (Community Edition or higher)
  - Workload: .NET Desktop Development
  
- **Visual Studio Code** with extensions:
  - C# Dev Kit
  - .NET Extension Pack

### Code Formatting

The project follows standard C# coding conventions. Use Visual Studio or VS Code formatting tools:

**Visual Studio**: `Ctrl+K, Ctrl+D` (Format Document)
**VS Code**: `Shift+Alt+F` (Format Document)

## Performance Profiling

To analyze performance:

```powershell
dotnet build -c Release
dotnet run -c Release --no-build
```

Monitor CPU and memory usage in Task Manager.

## Creating an Installer (Optional)

You can create an installer using tools like:
- **Inno Setup** - Free Windows installer creator
- **WiX Toolset** - Windows Installer XML
- **Advanced Installer** - Commercial solution

## Continuous Integration

Example GitHub Actions workflow:

```yaml
name: Build

on: [push, pull_request]

jobs:
  build:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build -c Release --no-restore
    
    - name: Test
      run: dotnet test -c Release --no-build
```

---

For more information, see the main [README.md](README.md).
