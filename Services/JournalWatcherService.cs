using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JournalWarden.Models;
using Newtonsoft.Json.Linq;

namespace JournalWarden.Services
{
    /// <summary>
    /// Monitors Elite Dangerous journal directory for new events
    /// </summary>
    public class JournalWatcherService : IDisposable
    {
        private FileSystemWatcher? _watcher;
        private string? _currentJournalPath;
        private long _lastPosition;

        public event EventHandler<JournalEvent>? EventReceived;
        public event EventHandler<string>? StatusChanged;

        /// <summary>
        /// Start monitoring the Elite Dangerous journal directory
        /// </summary>
        public void StartWatching(string journalDirectory)
        {
            if (!Directory.Exists(journalDirectory))
            {
                StatusChanged?.Invoke(this, $"Directory not found: {journalDirectory}");
                return;
            }

            // Find the most recent journal file
            var journalFiles = Directory.GetFiles(journalDirectory, "Journal.*.log")
                .OrderByDescending(f => File.GetLastWriteTime(f))
                .ToList();

            if (!journalFiles.Any())
            {
                StatusChanged?.Invoke(this, "No journal files found");
                return;
            }

            _currentJournalPath = journalFiles.First();
            StatusChanged?.Invoke(this, $"Monitoring: {Path.GetFileName(_currentJournalPath)}");

            // Read existing events
            LoadExistingEvents(_currentJournalPath);

            // Watch for new files and changes
            _watcher = new FileSystemWatcher(journalDirectory, "Journal.*.log")
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size,
                EnableRaisingEvents = true
            };

            _watcher.Changed += OnJournalChanged;
            _watcher.Created += OnJournalCreated;
        }

        private void LoadExistingEvents(string journalPath)
        {
            try
            {
                using var fileStream = new FileStream(journalPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(fileStream);
                
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    ProcessEventLine(line, journalPath);
                }

                _lastPosition = fileStream.Position;
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, $"Error loading events: {ex.Message}");
            }
        }

        private void OnJournalChanged(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath != _currentJournalPath) return;

            Task.Run(() => ReadNewEvents());
        }

        private void OnJournalCreated(object sender, FileSystemEventArgs e)
        {
            // New journal file created - switch to it
            _currentJournalPath = e.FullPath;
            _lastPosition = 0;
            StatusChanged?.Invoke(this, $"New journal: {Path.GetFileName(e.FullPath)}");
            ReadNewEvents();
        }

        private void ReadNewEvents()
        {
            if (string.IsNullOrEmpty(_currentJournalPath)) return;

            try
            {
                using var fileStream = new FileStream(_currentJournalPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                
                if (fileStream.Length < _lastPosition)
                {
                    // File was truncated or replaced
                    _lastPosition = 0;
                }

                fileStream.Seek(_lastPosition, SeekOrigin.Begin);
                using var reader = new StreamReader(fileStream);

                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    ProcessEventLine(line, _currentJournalPath);
                }

                _lastPosition = fileStream.Position;
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, $"Error reading events: {ex.Message}");
            }
        }

        private void ProcessEventLine(string line, string filePath)
        {
            try
            {
                var json = JObject.Parse(line);
                var evt = new JournalEvent
                {
                    Timestamp = json["timestamp"]?.ToObject<DateTime>() ?? DateTime.Now,
                    Event = json["event"]?.ToString() ?? "Unknown",
                    RawData = json,
                    FileName = Path.GetFileName(filePath)
                };

                EventReceived?.Invoke(this, evt);
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, $"Error parsing event: {ex.Message}");
            }
        }

        public void StopWatching()
        {
            _watcher?.Dispose();
            _watcher = null;
            StatusChanged?.Invoke(this, "Monitoring stopped");
        }

        public void Dispose()
        {
            StopWatching();
        }
    }
}
