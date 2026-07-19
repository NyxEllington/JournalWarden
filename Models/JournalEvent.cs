using System;
using Newtonsoft.Json.Linq;

namespace JournalWarden.Models
{
    /// <summary>
    /// Represents a single event from an Elite Dangerous journal file
    /// </summary>
    public class JournalEvent
    {
        public DateTime Timestamp { get; set; }
        public string Event { get; set; } = string.Empty;
        public JObject RawData { get; set; } = new JObject();
        public string FileName { get; set; } = string.Empty;
        
        /// <summary>
        /// Formatted display of the event for quick viewing
        /// </summary>
        public string Summary => $"[{Timestamp:HH:mm:ss}] {Event}";
        
        /// <summary>
        /// Pretty-printed JSON for detailed view
        /// </summary>
        public string FormattedJson => RawData.ToString(Newtonsoft.Json.Formatting.Indented);
    }
}
