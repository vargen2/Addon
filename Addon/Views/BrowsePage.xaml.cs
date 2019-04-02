using System;
using System.Linq;
using Addon.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Addon.Views
{
    public sealed partial class BrowsePage : Page
    {
        public BrowseViewModel ViewModel { get; } = new BrowseViewModel();

        public BrowsePage()
        {
            InitializeComponent();
        }

        private void AutoSuggestBox_OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput) return;

            var filtered = ViewModel.StoreAddons
                .Where(storeAddon => storeAddon.Title.Contains(sender.Text, StringComparison.CurrentCultureIgnoreCase))
                .ToList();
            ListView.ItemsSource = filtered;

        }
    }
}
