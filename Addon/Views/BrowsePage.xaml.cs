using Addon.Core.Models;
using Addon.Logic;
using Addon.ViewModels;
using Microsoft.Toolkit.Uwp.UI;
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

            ViewModel.StoreAddons.Filter = obj =>
            {
                if (obj is StoreAddon storeAddon)
                {
                    return storeAddon.AddonData.Title.Contains(sender.Text, StringComparison.CurrentCultureIgnoreCase);
                }
                return false;
            };            
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

        public void RefreshStoreAddonStatus()
        {
            //var addons = new HashSet<string>(ViewModel.Session.SelectedGame.Addons.SelectMany(a =>
            //{
            //    var nameList = new List<string>() { a.FolderName.ToLower(), a.Title.ToLower() };
            //    if (Logic.Version.PROJECT_URLS.TryGetValue(a.FolderName.ToLower(), out List<string> list))
            //    {
            //        nameList.InsertRange(0, list);
            //    }
            //    string urlFromAddonData = Parse.GetFromAddonDataFor(a);

            //    if (!string.IsNullOrEmpty(urlFromAddonData))
            //    {
            //        nameList.Insert(0, urlFromAddonData);
            //    }
            //    return nameList;
            //}).ToList());
            var addons = new HashSet<string>(ViewModel.Session.SelectedGame.Addons.Select(a=>a.ProjectUrl)).ToHashSet();
            foreach (var storeAddon in ViewModel.Session.StoreAddons)
            {
                if (storeAddon.Status.Equals(StoreAddon.INSTALLING))
                {
                    continue;
                }

                if (addons.Contains(storeAddon.AddonData.ProjectUrl))
                {
                    storeAddon.Status = StoreAddon.INSTALLED;
                }
                else
                {
                    storeAddon.Status = StoreAddon.NOTINSTALLED;
                }
            }           
            ViewModel.StoreAddons.Refresh();
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

        private void Sort(SortDescription sortDescription)
        {
            ViewModel.StoreAddons.SortDescriptions.Clear();
            ViewModel.StoreAddons.SortDescriptions.Add(sortDescription);
        }

        private void Title_Header_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            ViewModel.TitleSortDirection = Opposite(ViewModel.TitleSortDirection);
            Sort(new SortDescription("Title", ViewModel.TitleSortDirection));
        }

        private void Download_Header_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            ViewModel.DownloadSortDirection = Opposite(ViewModel.DownloadSortDirection);
            Sort(new SortDescription("NrOfDownloads", ViewModel.DownloadSortDirection, StringComparer.OrdinalIgnoreCase));
        }

        private void Status_Header_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            ViewModel.StatusSortDirection = Opposite(ViewModel.StatusSortDirection);
            Sort(new SortDescription("Status", ViewModel.StatusSortDirection));
        }

        private void Updated_Header_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            ViewModel.UpdatedSortDirection = Opposite(ViewModel.UpdatedSortDirection);
            Sort(new SortDescription("Updated", ViewModel.UpdatedSortDirection));
        }

        private void Created_Header_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {            
            ViewModel.CreatedSortDirection = Opposite(ViewModel.CreatedSortDirection);
            Sort(new SortDescription("Created", ViewModel.CreatedSortDirection));
        }

        private static SortDirection Opposite(SortDirection sortDirection)
        {
            return sortDirection == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;
        }
    }
}
