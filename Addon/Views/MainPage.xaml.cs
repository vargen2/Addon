using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Management.Deployment;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Addon.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Data;
using Windows.Web.Http;
using Addon.Helpers;
using Windows.Storage;
using Addon.Core.Helpers;
using Addon.Core.Models;
using Addon.Services;

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
            var addon = ViewModel.Game.Addons.First();

            await AppHelper.FindProjectUrlAndDownLoadVersionsFor(addon);

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

        private  void Addon_FlyoutBase_OnOpened(object sender, object e)
        {
            // TODO FIXA ASYNC
            var menuFlyout = sender as MenuFlyout;
            var versionMenu=menuFlyout.Items.First(item => item.Name.Equals("VersionsMenuFlyout")) as MenuFlyoutSubItem;
            var addon=versionMenu.Tag as Core.Models.Addon;
            foreach (var download in addon.Downloads)
            {
                versionMenu.Items.Add(new MenuFlyoutItem(){Text = download.ToString()});    
            }
        }
    }

}
