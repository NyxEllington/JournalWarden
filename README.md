# Journal Warden

A real-time Elite Dangerous journal event monitor and development tool built with C# and WPF.

![Status](https://img.shields.io/badge/status-ready-green) ![.NET](https://img.shields.io/badge/.NET-8.0-blue) ![Platform](https://img.shields.io/badge/platform-Windows-lightgrey)

## Features

✨ **Real-time Monitoring**: Watch journal files as events occur  
🔍 **Event Filtering**: Search and filter events by type or content  
🎯 **Event Type Filters**: Check/uncheck which event types to show or hide  
🚫 **Quick Presets**: One-click hide common noise events (Music, Friends, Status)  
📜 **Historical Playback**: Load and view past journal files  
📊 **Event Statistics**: Track unique event types and counts  
👨‍💻 **Developer-Friendly**: JSON viewer with syntax highlighting  
🎯 **Auto-Detection**: Automatically finds Elite Dangerous journal directory  
🔄 **Auto-Selection**: Automatically selects and displays the latest event (can be toggled off)

## Quick Start

```powershell
# Clone or navigate to the project
cd c:\Users\nyx\Documents\Projects\JournalWarden

# Run the application
dotnet run
```

## Screenshots

### Main Interface
- **Left Panel**: Real-time event stream with timestamps
- **Right Panel**: Detailed JSON view of selected event
- **Search**: Filter events by type or any JSON content
- **Status Bar**: Event counts and monitoring status

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Windows 10/11
- Elite Dangerous (for journal files)

### Building

```powershell
dotnet restore
dotnet build
```

### Running

```powershell
dotnet run
```

Or build and run the executable:

```powershell
dotnet publish -c Release
.\bin\Release\net8.0-windows\publish\JournalWarden.exe
```

### Prerelease package (recommended for GitHub releases)

The raw Debug output is meant for development and is not a great distribution artifact. The release packaging script produces a smaller framework-dependent build that is easier to upload to GitHub and install on Windows. It still requires the .NET 8 runtime to be present.

```powershell
.\scripts\Publish-Prerelease.ps1
```

That creates a zip file in `artifacts/prerelease/` such as `JournalWarden-win-x64.zip`. Users can extract it and run `JournalWarden.exe` directly.

### Windows installer (optional)

If you want a proper setup experience instead of a zip, you can build an installer executable:

```powershell
.\scripts\Publish-Installer.ps1
```

That produces `artifacts/installer/JournalWarden-Setup.exe`, which can be uploaded as a GitHub prerelease asset.

If Windows shows an access-denied or blocked-file error when launching the installer, run:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\Install-JournalWarden.ps1
```

That script unblocks the downloaded installer and launches it.

## Usage

1. **Set Journal Path**: The app automatically detects the default Elite Dangerous journal location (`%USERPROFILE%\Saved Games\Frontier Developments\Elite Dangerous`). You can change this using the Browse button.

2. **Start Monitoring**: Click "Start Monitoring" to begin watching for events.

3. **Filter Events**: Use the search box to filter events by:
   - Event type (e.g., "FSDJump", "Docked")
   - Any JSON content (e.g., "Sol", "Federation")

4. **View Details**: Click any event in the list to see its full JSON structure in the details pane.

5. **Clear Events**: Use the "Clear" button to reset the event list.

## Architecture

- **MVVM Pattern**: Clean separation of UI and logic
- **Real-time File Watching**: `FileSystemWatcher` for instant updates
- **Observable Collections**: Automatic UI updates as events arrive
- **JSON Parsing**: Newtonsoft.Json for robust event parsing

## Project Structure

```
JournalWarden/
├── Models/              # Data models
│   ├── JournalEvent.cs
│   └── JournalFile.cs
├── Services/            # Background services
│   └── JournalWatcherService.cs
├── ViewModels/          # MVVM view models
│   └── MainViewModel.cs
├── Views/               # WPF UI
│   ├── MainWindow.xaml
│   └── MainWindow.xaml.cs
└── Styles/              # UI styling
    └── Colors.xaml
```

## Future Enhancements

- Event type statistics and charts
- Export filtered events to file
- Custom event bookmarks
- Multi-file replay with timeline
- Event type documentation lookup
- Integration with ED API
- Plugin system for custom analyzers

## Development Notes

This tool is designed as a development aid for building Elite Dangerous integrations. The JSON viewer makes it easy to:
- Understand event structure
- Debug event timing
- Test event filtering logic
- Prototype new features

## License

MIT License - Feel free to use this for your own ED projects!
