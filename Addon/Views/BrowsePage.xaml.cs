using Addon.Core.Models;
using Addon.Logic;
using Addon.ViewModels;
using System;
using System.Linq;
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

            ListView.ItemsSource = ViewModel.Session.StoreAddons
                .Where(storeAddon => storeAddon.Title.Contains(sender.Text, StringComparison.CurrentCultureIgnoreCase))
                .ToList();
        }

        private async void Install_Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var button = sender as Button;
            var storeAddon = button.Tag as StoreAddon;
            await Install.InstallAddon(storeAddon);
        }
    }
}
