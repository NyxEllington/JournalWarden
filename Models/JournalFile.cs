using System;

namespace JournalWarden.Models
{
    /// <summary>
    /// Represents a journal file being monitored
    /// </summary>
    public class JournalFile
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public DateTime LastModified { get; set; }
        public long FileSize { get; set; }
        public bool IsActive { get; set; }
    }
}
