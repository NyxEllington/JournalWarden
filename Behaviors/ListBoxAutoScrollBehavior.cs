using System.Windows;
using System.Windows.Controls;

namespace JournalWarden.Behaviors
{
    /// <summary>
    /// Attached behavior to auto-scroll ListBox to selected item
    /// </summary>
    public static class ListBoxAutoScrollBehavior
    {
        public static readonly DependencyProperty AutoScrollToSelectedItemProperty =
            DependencyProperty.RegisterAttached(
                "AutoScrollToSelectedItem",
                typeof(bool),
                typeof(ListBoxAutoScrollBehavior),
                new PropertyMetadata(false, OnAutoScrollToSelectedItemChanged));

        public static bool GetAutoScrollToSelectedItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollToSelectedItemProperty);
        }

        public static void SetAutoScrollToSelectedItem(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToSelectedItemProperty, value);
        }

        private static void OnAutoScrollToSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListBox listBox)
            {
                if ((bool)e.NewValue)
                {
                    listBox.SelectionChanged += ListBox_SelectionChanged;
                }
                else
                {
                    listBox.SelectionChanged -= ListBox_SelectionChanged;
                }
            }
        }

        private static void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem != null)
            {
                listBox.ScrollIntoView(listBox.SelectedItem);
            }
        }
    }
}
