using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JournalWarden.Models;
using JournalWarden.Services;
using System.Collections.Generic;

namespace JournalWarden.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly JournalWatcherService _watcherService;

        [ObservableProperty]
        private string _journalPath = GetDefaultJournalPath();

        [ObservableProperty]
        private string _statusMessage = "Ready";

        [ObservableProperty]
        private string _searchFilter = string.Empty;

        [ObservableProperty]
        private JournalEvent? _selectedEvent;

        [ObservableProperty]
        private bool _isMonitoring;

        [ObservableProperty]
        private bool _autoSelectLatest = true;

        [ObservableProperty]
        private int _totalEvents;

        [ObservableProperty]
        private int _filteredEventsCount;

        public ObservableCollection<JournalEvent> AllEvents { get; } = new();
        public ObservableCollection<JournalEvent> FilteredEvents { get; } = new();
        public ObservableCollection<string> EventTypes { get; } = new();
        public ObservableCollection<EventTypeFilter> EventTypeFilters { get; } = new();
        
        private readonly Dictionary<string, EventTypeFilter> _eventTypeFilterMap = new();

        public MainViewModel()
        {
            _watcherService = new JournalWatcherService();
            _watcherService.EventReceived += OnEventReceived;
            _watcherService.StatusChanged += OnStatusChanged;
        }

        [RelayCommand]
        private void StartMonitoring()
        {
            if (string.IsNullOrWhiteSpace(JournalPath) || !Directory.Exists(JournalPath))
            {
                StatusMessage = "Invalid journal directory";
                return;
            }

            _watcherService.StartWatching(JournalPath);
            IsMonitoring = true;
        }

        [RelayCommand]
        private void StopMonitoring()
        {
            _watcherService.StopWatching();
            IsMonitoring = false;
        }

        [RelayCommand]
        private void ClearEvents()
        {
            AllEvents.Clear();
            FilteredEvents.Clear();
            EventTypes.Clear();
            EventTypeFilters.Clear();
            _eventTypeFilterMap.Clear();
            TotalEvents = 0;
            FilteredEventsCount = 0;
            StatusMessage = "Events cleared";
        }
        
        [RelayCommand]
        private void ShowAllEventTypes()
        {
            foreach (var filter in EventTypeFilters)
            {
                filter.IsVisible = true;
            }
        }
        
        [RelayCommand]
        private void HideAllEventTypes()
        {
            foreach (var filter in EventTypeFilters)
            {
                filter.IsVisible = false;
            }
        }
        
        [RelayCommand]
        private void HideNoiseEvents()
        {
            // Hide common "noise" events: Music, Friends, Status, ReceiveText
            var noiseEvents = new[] { "Music", "Friends", "FriendsStatus", "ReceiveText", "Status", "Commander" };
            foreach (var filter in EventTypeFilters.Where(f => noiseEvents.Contains(f.EventName)))
            {
                filter.IsVisible = false;
            }
        }

        [RelayCommand]
        private void BrowseFolder()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select Journal Directory",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Select Folder"
            };

            if (dialog.ShowDialog() == true)
            {
                JournalPath = Path.GetDirectoryName(dialog.FileName) ?? JournalPath;
            }
        }

        partial void OnSearchFilterChanged(string value)
        {
            ApplyFilter();
        }

        private void OnEventReceived(object? sender, JournalEvent evt)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                AllEvents.Add(evt);
                TotalEvents = AllEvents.Count;

                // Track unique event types
                if (!EventTypes.Contains(evt.Event))
                {
                    EventTypes.Add(evt.Event);
                    
                    // Add to filter list
                    var filter = new EventTypeFilter 
                    { 
                        EventName = evt.Event, 
                        IsVisible = true, 
                        Count = 0 
                    };
                    filter.PropertyChanged += (s, e) => 
                    {
                        if (e.PropertyName == nameof(EventTypeFilter.IsVisible))
                        {
                            ApplyFilter();
                        }
                    };
                    EventTypeFilters.Add(filter);
                    _eventTypeFilterMap[evt.Event] = filter;
                }
                
                // Update count
                if (_eventTypeFilterMap.TryGetValue(evt.Event, out var eventFilter))
                {
                    eventFilter.Count++;
                }

                // Apply filter to new event
                if (ShouldIncludeEvent(evt))
                {
                    FilteredEvents.Add(evt);
                    FilteredEventsCount = FilteredEvents.Count;
                    
                    // Automatically select the most recent event if enabled
                    if (AutoSelectLatest)
                    {
                        SelectedEvent = evt;
                    }
                }
            });
        }

        private void OnStatusChanged(object? sender, string status)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                StatusMessage = status;
            });
        }

        private void ApplyFilter()
        {
            FilteredEvents.Clear();

            foreach (var evt in AllEvents.Where(ShouldIncludeEvent))
            {
                FilteredEvents.Add(evt);
            }

            FilteredEventsCount = FilteredEvents.Count;
        }

        private bool ShouldIncludeEvent(JournalEvent evt)
        {
            // Check if event type is hidden
            if (_eventTypeFilterMap.TryGetValue(evt.Event, out var filter))
            {
                if (!filter.IsVisible)
                    return false;
            }
            
            // Check search filter
            if (string.IsNullOrWhiteSpace(SearchFilter))
                return true;

            var searchFilter = SearchFilter.ToLowerInvariant();

            // Search in event type
            if (evt.Event.ToLowerInvariant().Contains(searchFilter))
                return true;

            // Search in JSON content
            if (evt.FormattedJson.ToLowerInvariant().Contains(searchFilter))
                return true;

            return false;
        }

        private static string GetDefaultJournalPath()
        {
            var savedGamesPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Saved Games", "Frontier Developments", "Elite Dangerous");

            return Directory.Exists(savedGamesPath) ? savedGamesPath : "";
        }
    }
}
