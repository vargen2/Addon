using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Management.Deployment;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Data;
using Windows.Web.Http;
using Windows.Storage;
using Addon.Core.Helpers;
using Addon.Core.Models;
using Addon.Services;
using Windows.UI.ViewManagement;
using Addon.Helpers;
using Addon.Logic;
using Addon.ViewModels;

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
            var button = e.OriginalSource as Button;
            var addon = button.Tag as Core.Models.Addon;
            await Task.Delay(100);
        }

        private void UIElement_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private async void Temp_ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("https://aftonbladet.se");
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var task = httpClient.GetStringAsync(uri);

                    task.Progress += async (installResult, progress) => await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        StatusProgressBar.Value = ((int)(progress.Stage));
                    });

                    var result = await task;
                    StatusProgressBar.Value = 0;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                }
            }
        }

        private async void TempTest_ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            //var addon = ViewModel.Session.SelectedGame.Addons.First();
            var addon = ViewModel.Session.SelectedGame.Addons.First(a => a.FolderName.ToLower().Equals("details"));

            await Tasks.FindProjectUrlAndDownLoadVersionsFor(addon);

            //if (addon.Status == Core.Models.Addon.INITIALIZED)
            //{
            //    addon.Progress = 0;
            //    addon.Status = Core.Models.Addon.DOWNLOADING_VERSIONS;
            //}
            //else if (addon.Status == Core.Models.Addon.DOWNLOADING_VERSIONS)
            //{
            //    addon.Status = Core.Models.Addon.UPDATEABLE;
            //}
            //else if (addon.Status == Core.Models.Addon.UPDATEABLE)
            //{
            //    addon.Progress = 50;
            //    addon.Status = Core.Models.Addon.UPDATING;
            //}
            //else if (addon.Status == Core.Models.Addon.UPDATING)
            //{
            //    addon.Status = Core.Models.Addon.UP_TO_DATE;
            //}
            //else if (addon.Status == Core.Models.Addon.UP_TO_DATE)
            //{
            //    addon.Status = Core.Models.Addon.UNKNOWN;
            //}
            //else if (addon.Status == Core.Models.Addon.UNKNOWN)
            //{
            //    addon.Status = Core.Models.Addon.INITIALIZED;
            //}

        }





        private async void VersionsMenuFlyout_OnLoaded(object sender, RoutedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var versionMenu = sender as MenuFlyoutSubItem;
                var addon = versionMenu.Tag as Core.Models.Addon;
                foreach (var download in addon.Downloads)
                {
                    versionMenu.Items.Add(new MenuFlyoutItem() { Text = download.ToString() });
                }
            });
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
    }

}
