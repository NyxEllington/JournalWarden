# Quick Start Guide

## Running the Application

```powershell
dotnet run
```

Or double-click the executable after publishing:
```powershell
dotnet publish -c Release
.\bin\Release\net8.0-windows\publish\JournalWarden.exe
```

## Finding Your Elite Dangerous Journals

By default, Elite Dangerous saves journals to:
```
%USERPROFILE%\Saved Games\Frontier Developments\Elite Dangerous
```

The app will auto-detect this location, but you can change it with the Browse button.

## Using the App

1. **Set Journal Path**: The app automatically detects the default Elite Dangerous journal location. Use the Browse button to change it if needed.

2. **Start Monitoring**: Click "Start Monitoring" to begin watching for events in real-time.

3. **Filter by Event Type**: Use the left sidebar to check/uncheck which event types to display:
   - **All** - Show all event types
   - **None** - Hide all event types  
   - **Hide Noise** - Quick filter to hide Music, Friends, Status, and other common noise events
   - Individual checkboxes show event names with counts

4. **Search Filter**: Use the search box to filter events by:
   - Event type (e.g., "FSDJump", "Docked")
   - Any JSON content (e.g., "Sol", "Federation")

5. **Auto-Select Latest**: By default, the app automatically selects and displays the most recent event as it arrives. Uncheck "Auto-select latest event" if you want to browse older events without being interrupted by new ones.

6. **View Details**: Click any event in the list to see its full JSON structure in the details pane.

7. **Clear Events**: Use the "Clear" button to reset the event list.

## Sample Elite Dangerous Events

Here are some common event types you'll see:

### System Events
- **Fileheader** - Journal file metadata (game version, build)
- **LoadGame** - Commander loaded, ship info
- **Location** - Current star system location
- **Docked** - Docked at a station
- **Undocked** - Left a station

### Travel Events
- **FSDJump** - Jumped to a new system
- **SupercruiseEntry** - Entered supercruise
- **SupercruiseExit** - Dropped from supercruise
- **Liftoff** - Took off from surface
- **Touchdown** - Landed on surface

### Combat Events
- **ShipTargeted** - Targeted a ship
- **UnderAttack** - Being fired upon
- **ShieldsUp/Down** - Shield status
- **FighterDestroyed** - Ship fighter destroyed
- **PVPKill** - Killed another player

### Economy Events
- **MarketBuy/Sell** - Commodity trading
- **MissionAccepted** - New mission
- **MissionCompleted** - Mission finished
- **BountyReward** - Bounty collected

### Exploration Events
- **Scan** - Scanned a body
- **SAAScanComplete** - Detailed surface scan
- **CodexEntry** - Codex discovery
- **Screenshot** - Screenshot taken

## Development Tips

### Filtering Events
Use the search box to filter:
- By event type: `FSDJump`
- By system name: `Sol`
- By commodity: `Gold`
- By ship name: `Anaconda`
- Any JSON content

### Understanding Event Structure
Click any event to see its full JSON structure in the right panel. This helps you:
- Find specific property names
- Understand nested data
- Build your own parsers
- Debug event timing

### Testing Your Code
1. Start the app
2. Play Elite Dangerous
3. Perform the actions you want to track
4. Watch the events stream in real-time
5. Copy the JSON structure for your code

### Historical Analysis
To analyze old sessions:
1. Stop monitoring current journal
2. Browse to select an old journal file
3. The app will load all events from that file
4. Use filters to find specific events

## Example Event JSON

### FSDJump Event
```json
{
  "timestamp": "2024-01-20T12:30:45Z",
  "event": "FSDJump",
  "StarSystem": "Sol",
  "StarPos": [0.0, 0.0, 0.0],
  "SystemAddress": 10477373803,
  "StarClass": "G",
  "Body": "Sol",
  "JumpDist": 9.464,
  "FuelUsed": 0.907520,
  "FuelLevel": 31.092480
}
```

### Docked Event
```json
{
  "timestamp": "2024-01-20T12:35:20Z",
  "event": "Docked",
  "StationName": "Abraham Lincoln",
  "StationType": "Orbis",
  "StarSystem": "Sol",
  "SystemAddress": 10477373803,
  "MarketID": 128666762,
  "StationFaction": {
    "Name": "Mother Gaia"
  },
  "StationGovernment": "$government_Democracy;",
  "StationEconomy": "$economy_Refinery;"
}
```

## Next Steps

Once you understand the event structure, you can:

1. **Build Event Filters** - Create custom filters for specific workflows
2. **Track Statistics** - Count jumps, trading profit, combat kills
3. **Map Trade Routes** - Analyze MarketBuy/Sell events
4. **Monitor Exploration** - Track scanned systems and bodies
5. **Create Alerts** - Trigger actions on specific events
6. **Export Data** - Save event streams for analysis
7. **Build Integrations** - Connect to external tools/APIs

## Resources

- [Elite Dangerous Journal Documentation](https://elite-journal.readthedocs.io/)
- [Frontier Developer Community](https://forums.frontier.co.uk/forums/elite-api/)
- [EDCD - Elite Dangerous Community Developers](https://github.com/EDCD)
