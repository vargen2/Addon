using Addon.Core.Models;
using Addon.Logic;
using Addon.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            RefreshStoreAddonStatus();
            ViewModel.Session.PropertyChanged += Session_PropertyChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModel.Session.PropertyChanged -= Session_PropertyChanged;
        }

        private void RefreshStoreAddonStatus()
        {
            var addons = new HashSet<string>(ViewModel.Session.SelectedGame.Addons.SelectMany(a =>
            {
                var nameList = new List<string>() { a.FolderName.ToLower(), a.Title.ToLower() };
                if (Logic.Version.PROJECT_URLS.TryGetValue(a.FolderName.ToLower(), out List<string> list))
                {
                    nameList.InsertRange(0, list);
                }
                return nameList;
            }).ToList());

            foreach (var storeAddon in ViewModel.Session.StoreAddons)
            {
                if(storeAddon.Status.Equals(StoreAddon.INSTALLING))
                {
                    continue;
                }

                if (addons.Contains(storeAddon.Url.ToLower()) || addons.Contains(storeAddon.Title.ToLower()))
                {
                    storeAddon.Status = StoreAddon.INSTALLED;
                }
                else
                {
                    storeAddon.Status = StoreAddon.NOTINSTALLED;
                }
            }
        }

        private void Session_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("SelectedGame"))
            {
                var game = ViewModel.Session.SelectedGame;
                if (game != null)
                {
                    RefreshStoreAddonStatus();
                }
            }
        }
    }
}
