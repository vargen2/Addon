using Addon.Logic;
using Addon.ViewModels;
using System;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Addon.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
        }

        private async void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var addon = button.Tag as Core.Models.Addon;
            await Tasks.UpdateAddon(addon);

        }

        private void UIElement_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private async void DownloadVersionsForAllAddonsInSelectedGame(object sender, RoutedEventArgs e)
        {
            var addons = ViewModel.Session.SelectedGame.Addons.ToList();
            await Tasks.FindProjectUrlAndDownLoadVersionsFor(addons);
            Debug.WriteLine("Version downloaded for all addons");
            await Logic.Storage.SaveTask();
        }

        private async void RefreshTocFileForAllInSelectedGame(object sender, RoutedEventArgs e)
        {
            var addons = ViewModel.Session.SelectedGame.Addons.ToList();
            await Tasks.RefreshTocFileFor(addons);
            Debug.WriteLine("Refreshed toc files for all addons");
            await Logic.Storage.SaveTask();
        }


        private void FlyoutBase_OnOpening(object sender, object e)
        {
            var menuflyuout = sender as MenuFlyout;
            Core.Models.Addon addon = menuflyuout.Items.First().Tag as Core.Models.Addon;
            MenuFlyoutItemBase temp = menuflyuout.Items.FirstOrDefault(item => item.Name.Equals("VersionsMenuFlyout"));

            if (temp != null)
            {
                menuflyuout.Items.Remove(temp);
                var submenu = new MenuFlyoutSubItem()
                {
                    Name = "VersionsMenuFlyout",
                    Text = "Versions"
                };
                foreach (var download in addon.Downloads)
                {
                    var menuItem = new MenuFlyoutItem() { Text = download.ToString() };
                    menuItem.Click += async (a, b) =>
                    {
                        ContentDialog updateAddonDialog = new ContentDialog()
                        {
                            Title = "Update Addon?",
                            Content = "Update to " + download.ReleaseType + " " + download.Version + "?",
                            PrimaryButtonText = "Ok",
                            CloseButtonText = "Cancel"
                        };

                        var response = await updateAddonDialog.ShowAsync();
                        if (response == ContentDialogResult.Primary)
                        {
                            await Tasks.UpdateAddon(addon, download);
                        }
                    };
                    submenu.Items.Add(menuItem);
                }
                menuflyuout.Items.Insert(menuflyuout.Items.Count - 1, submenu);
            }
        }

        private async void RemoveAllGames(object sender, RoutedEventArgs e)
        {

            ViewModel.Session.Games.Clear();
            ViewModel.Session.SelectedGame = new Core.Models.Game("No Game Found");
            await Storage.SaveTask();

        }        
    }

}
