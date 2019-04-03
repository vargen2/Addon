using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Addon.Core.Models;
using Addon.Helpers;
using Addon.Services;
using Addon.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Addon.Core.Helpers;

namespace Addon.Views
{
    // TODO WTS: Change the icons and titles for all NavigationViewItems in ShellPage.xaml.
    public sealed partial class ShellPage : Page
    {
        public ShellViewModel ViewModel { get; } = new ShellViewModel();

        public ShellPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.Initialize(shellFrame, navigationView, KeyboardAccelerators);

        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            var clickedGame = listBox.SelectedValue as Game;
            if (clickedGame == ViewModel.Session.SelectedGame)
                return;

            ViewModel.Session.SelectedGame = listBox.SelectedValue as Game;
            NavigationService.ForceNavigateMainPage();

            GameSelectorFlyout.Hide();
        }

        private void ListBox_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            GameSelectorFlyout.Hide();
        }


        private async void OpenFolder_OnClick(object sender, RoutedEventArgs e)
        {

            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");

            Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {

                if (ViewModel.Session.Games.Any(g => g.AbsolutePath.Equals(folder.Path))) return;

                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                var game = await AppHelper.FolderToGame(folder);
                ViewModel.Session.Games.Add(game);
                if (ViewModel.Session.Games.Count == 1)
                {
                    ViewModel.Session.SelectedGame = game;
                    NavigationService.ForceNavigateMainPage();
                }
                Debug.WriteLine("Picked folder: " + folder.Name);
                AppHelper.RefreshGameFolder(game);
            }
            else
            {
                Debug.WriteLine("Operation cancelled.");
            }


        }
    }
}
