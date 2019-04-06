using System;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Addon.ViewModels;

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

            ListView.ItemsSource = ViewModel.Session.StoreAddons
                .Where(storeAddon => storeAddon.Title.Contains(sender.Text, StringComparison.CurrentCultureIgnoreCase))
                .ToList();
        }
    }
}
