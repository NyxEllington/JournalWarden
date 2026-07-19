using CommunityToolkit.Mvvm.ComponentModel;

namespace JournalWarden.Models
{
    /// <summary>
    /// Represents a filterable event type
    /// </summary>
    public partial class EventTypeFilter : ObservableObject
    {
        [ObservableProperty]
        private string _eventName = string.Empty;

        [ObservableProperty]
        private bool _isVisible = true;

        [ObservableProperty]
        private int _count;
    }
}
