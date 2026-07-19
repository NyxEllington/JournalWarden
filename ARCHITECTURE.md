# Architecture Overview

## Design Pattern: MVVM (Model-View-ViewModel)

### Models (`/Models`)
Pure data classes representing domain entities:
- **JournalEvent**: Individual event from journal file
- **JournalFile**: Metadata about journal files

### Views (`/Views`)
XAML UI definitions:
- **MainWindow**: Primary application window
  - Event list panel (left)
  - Event details panel (right)
  - Control toolbar (top)
  - Status bar (bottom)

### ViewModels (`/ViewModels`)
UI logic and data binding:
- **MainViewModel**: Main window logic
  - Commands (Start/Stop/Clear/Browse)
  - Observable collections (AllEvents, FilteredEvents)
  - Search filter implementation
  - Event statistics

### Services (`/Services`)
Background operations:
- **JournalWatcherService**: File system monitoring
  - FileSystemWatcher for real-time updates
  - Incremental file reading (tracks position)
  - Event parsing and notification
  - Automatic new file detection

## Data Flow

```
Elite Dangerous
    ↓ (writes JSON events)
Journal File (*.log)
    ↓ (FileSystemWatcher)
JournalWatcherService
    ↓ (EventReceived event)
MainViewModel
    ↓ (ObservableCollection.Add)
WPF Data Binding
    ↓ (UI update)
MainWindow Display
```

## Key Technologies

### WPF (Windows Presentation Foundation)
- XAML for declarative UI
- Data binding for automatic updates
- MVVM support

### CommunityToolkit.Mvvm
- `ObservableProperty` source generator
- `RelayCommand` for ICommand implementation
- Reduces boilerplate code

### Newtonsoft.Json
- JSON parsing and serialization
- Dynamic JSON object handling (JObject)
- Pretty-printing for display

### FileSystemWatcher
- Monitors directory for changes
- Detects file modifications and creation
- Minimal resource usage

## Performance Considerations

### Memory Management
- ObservableCollections hold events in memory
- Consider paging for large sessions (10K+ events)
- Weak event handlers to prevent leaks

### File Reading
- Incremental reads (track file position)
- Shared file access (ReadWrite share mode)
- Async file operations to prevent UI blocking

### UI Updates
- Dispatcher.Invoke for cross-thread updates
- Virtualization for large lists (WPF default)
- Lazy loading for JSON formatting

### Filtering Performance
- LINQ for flexible queries
- Consider index for large datasets
- Debounce search input for performance

## Threading Model

```
Main UI Thread
├─ WPF rendering
├─ Data binding updates
└─ User interactions

FileSystemWatcher Thread
├─ File change notifications
└─ New file detection

Background Tasks
└─ File reading (via Task.Run)
```

## Extension Points

### Adding New Event Types
1. Parse in `JournalWatcherService.ProcessEventLine`
2. Create model if needed (or use generic JournalEvent)
3. ViewModel automatically handles display

### Adding Filters
1. Add property to MainViewModel
2. Update `ShouldIncludeEvent` method
3. Bind UI control to property

### Adding Export
1. Create new command in MainViewModel
2. Serialize FilteredEvents to desired format
3. File save dialog and write

### Plugin System (Future)
```csharp
public interface IEventAnalyzer
{
    string Name { get; }
    void ProcessEvent(JournalEvent evt);
    object GetResults();
}
```

## Testing Strategy

### Unit Tests
- JournalEvent parsing
- Filter logic
- Event type detection

### Integration Tests
- File watcher with mock files
- ViewModel command execution
- Filter application

### UI Tests
- Window opening
- Event list population
- Search functionality

## Build Configuration

### Debug
- Full debugging symbols
- No optimization
- Console window for diagnostics

### Release
- Optimizations enabled
- Single-file publish option
- Trimmed dependencies
