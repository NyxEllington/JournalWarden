# Developer Notes

## Using This as a Base for Other Projects

This project structure is ideal for adapting to similar real-time monitoring tools:

### Adaptable Components

#### JournalWatcherService Pattern
Can be reused for monitoring:
- Log files from any application
- Configuration files
- Data exports
- CSV/XML/JSON streams
- Any file-based data source

**Key Features to Reuse:**
- Incremental file reading (memory efficient)
- FileSystemWatcher with proper file sharing
- Automatic new file detection
- Event-based notification pattern

#### MVVM Structure
Copy this pattern for any data-driven WPF app:
- Clean separation of concerns
- Easy to test ViewModels
- Automatic UI updates via data binding
- Command pattern for user actions

#### Search/Filter Pattern
The filtering system works for any data:
```csharp
// Generic pattern
private bool ShouldIncludeItem<T>(T item, string filter)
{
    // Your custom logic here
}
```

### Extending for Elite Dangerous Projects

#### 1. Route Planner
**Idea**: Track FSDJump events to build optimal routes
**What to add**:
- Graph of visited systems
- Path-finding algorithm
- Fuel range calculator
- Route visualization

**Files to modify**:
- Create `Models/StarSystem.cs`
- Create `Services/RouteCalculationService.cs`
- Add `ViewModels/RouteViewModel.cs`
- New view: `Views/RouteWindow.xaml`

#### 2. Trading Helper
**Idea**: Track MarketBuy/Sell events to find profitable routes
**What to add**:
- Commodity price tracking
- Profit calculator
- Station database
- Market trends

**Files to modify**:
- Create `Models/TradingData.cs`
- Create `Services/PriceAnalysisService.cs`
- Add filtering for MarketBuy/MarketSell events

#### 3. Combat Logger
**Idea**: Track combat events and ship builds
**What to add**:
- Ship loadout tracking
- Combat statistics
- Win/loss ratio
- Enemy ship database

**Files to modify**:
- Create `Models/CombatEvent.cs`
- Add combat-specific filters
- Create statistics aggregation

#### 4. Exploration Tracker
**Idea**: Track scanned systems and discoveries
**What to add**:
- First discovery tracking
- System catalog
- Screenshot correlation
- Codex integration

**Files to modify**:
- Create `Models/ExplorationData.cs`
- Add scan event aggregation
- Create discovery report generator

### Code Snippets for Common Tasks

#### Adding a New Event Type Specific Handler

```csharp
// In JournalWatcherService.cs
private void ProcessEventLine(string line, string filePath)
{
    try
    {
        var json = JObject.Parse(line);
        var eventType = json["event"]?.ToString();
        
        // Specific handler
        if (eventType == "FSDJump")
        {
            HandleFSDJump(json);
        }
        
        // Generic handler
        var evt = new JournalEvent { /* ... */ };
        EventReceived?.Invoke(this, evt);
    }
    catch { /* ... */ }
}

private void HandleFSDJump(JObject data)
{
    // Extract specific fields
    var system = data["StarSystem"]?.ToString();
    var jumpDist = data["JumpDist"]?.ToObject<double>();
    
    // Emit specialized event
    FSDJumpDetected?.Invoke(this, new FSDJumpEventArgs 
    { 
        System = system, 
        Distance = jumpDist 
    });
}
```

#### Adding Database Persistence

```csharp
// Add to Services folder
public class EventDatabaseService
{
    private readonly SQLiteConnection _db;
    
    public void SaveEvent(JournalEvent evt)
    {
        _db.Insert(new EventRecord 
        {
            Timestamp = evt.Timestamp,
            Type = evt.Event,
            Data = evt.FormattedJson
        });
    }
    
    public IEnumerable<JournalEvent> QueryEvents(
        DateTime start, 
        DateTime end, 
        string eventType = null)
    {
        // Query implementation
    }
}
```

#### Adding Export Functionality

```csharp
// Add to MainViewModel
[RelayCommand]
private async Task ExportEvents()
{
    var dialog = new SaveFileDialog
    {
        Filter = "JSON|*.json|CSV|*.csv",
        DefaultExt = "json"
    };
    
    if (dialog.ShowDialog() != true) return;
    
    var ext = Path.GetExtension(dialog.FileName);
    
    if (ext == ".json")
    {
        var json = JsonConvert.SerializeObject(
            FilteredEvents, 
            Formatting.Indented);
        await File.WriteAllTextAsync(dialog.FileName, json);
    }
    else if (ext == ".csv")
    {
        // CSV export logic
    }
    
    StatusMessage = $"Exported {FilteredEvents.Count} events";
}
```

#### Adding Event Statistics

```csharp
// Add to MainViewModel
public class EventStatistics : ObservableObject
{
    public Dictionary<string, int> EventCounts { get; set; }
    public DateTime FirstEvent { get; set; }
    public DateTime LastEvent { get; set; }
    public int UniqueEventTypes { get; set; }
    
    public void Calculate(IEnumerable<JournalEvent> events)
    {
        var list = events.ToList();
        EventCounts = list.GroupBy(e => e.Event)
            .ToDictionary(g => g.Key, g => g.Count());
        FirstEvent = list.Min(e => e.Timestamp);
        LastEvent = list.Max(e => e.Timestamp);
        UniqueEventTypes = EventCounts.Count;
    }
}

// In MainViewModel
[ObservableProperty]
private EventStatistics _statistics = new();

private void UpdateStatistics()
{
    Statistics.Calculate(AllEvents);
    OnPropertyChanged(nameof(Statistics));
}
```

### Performance Optimization Tips

#### For Large Event Counts (100K+)

1. **Virtual Scrolling** (already enabled by WPF)
2. **Paging**: Load events in chunks
```csharp
public class PagedEventCollection : ObservableCollection<JournalEvent>
{
    private const int PageSize = 1000;
    private int _currentPage;
    
    public void LoadNextPage()
    {
        var start = _currentPage * PageSize;
        var page = _allEvents.Skip(start).Take(PageSize);
        foreach (var evt in page) Add(evt);
        _currentPage++;
    }
}
```

3. **Background filtering**:
```csharp
partial void OnSearchFilterChanged(string value)
{
    Task.Run(() => {
        var filtered = AllEvents.Where(ShouldIncludeEvent).ToList();
        Dispatcher.Invoke(() => {
            FilteredEvents.Clear();
            foreach (var evt in filtered) 
                FilteredEvents.Add(evt);
        });
    });
}
```

#### Memory Management

1. **Clear old events periodically**
2. **Limit event retention** (e.g., last 10K events)
3. **Use weak event handlers** for services
4. **Dispose properly**:
```csharp
public void Dispose()
{
    _watcherService.EventReceived -= OnEventReceived;
    _watcherService.StatusChanged -= OnStatusChanged;
    _watcherService.Dispose();
}
```

### Testing Against Live Data

1. **Run Elite Dangerous**
2. **Start Journal Warden**
3. **Perform test actions**:
   - Jump to new system
   - Dock at station
   - Buy/sell commodities
   - Scan bodies
   - Enter combat

4. **Verify**:
   - Events appear immediately
   - JSON structure is correct
   - Filtering works
   - No crashes/lag

### Debugging Tips

#### Enable Console Output
```csharp
// In App.xaml.cs
protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);
    
#if DEBUG
    AllocConsole(); // Win32 API to show console
#endif
}
```

#### Add Logging
```csharp
// Use Microsoft.Extensions.Logging
private readonly ILogger _logger;

public JournalWatcherService(ILogger<JournalWatcherService> logger)
{
    _logger = logger;
}

private void OnJournalChanged(object sender, FileSystemEventArgs e)
{
    _logger.LogDebug("Journal changed: {Path}", e.FullPath);
    // ...
}
```

#### Test with Mock Data
```csharp
// Create test journal files
var testEvents = new[]
{
    @"{""timestamp"":""2024-01-20T12:00:00Z"",""event"":""FSDJump"",""StarSystem"":""Sol""}",
    @"{""timestamp"":""2024-01-20T12:05:00Z"",""event"":""Docked"",""StationName"":""Abraham Lincoln""}"
};

File.WriteAllLines(@"C:\Temp\TestJournal.log", testEvents);
```

## Useful Resources

- [WPF Tutorial](https://wpf-tutorial.com/)
- [MVVM Light/Toolkit](https://github.com/CommunityToolkit/dotnet)
- [Elite Dangerous Journal Docs](https://elite-journal.readthedocs.io/)
- [FileSystemWatcher Docs](https://docs.microsoft.com/en-us/dotnet/api/system.io.filesystemwatcher)

## Common Issues & Solutions

### Issue: Events not appearing
**Solution**: Check file sharing mode (should be `FileShare.ReadWrite`)

### Issue: App crashes when ED closes
**Solution**: Handle file deletion in FileSystemWatcher

### Issue: Memory grows indefinitely
**Solution**: Implement event limit or paging

### Issue: Slow filtering with many events
**Solution**: Move filtering to background thread

### Issue: UI freezes during file load
**Solution**: Use async file reading with progress reporting
