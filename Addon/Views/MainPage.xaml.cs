using Addon.Logic;
using Addon.ViewModels;
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
            await Logic.Storage.SaveTask();
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
                    submenu.Items.Add(new MenuFlyoutItem() { Text = download.ToString() });
                }
                menuflyuout.Items.Insert(menuflyuout.Items.Count - 1, submenu);
            }
        }
    }

}
