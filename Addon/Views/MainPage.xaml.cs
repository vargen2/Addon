using System;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Addon.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Data;
using Windows.Web.Http;
using Addon.Helpers;

namespace Addon.Views
{


    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();

        }


        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {

            var button = e.OriginalSource as Button;
            var addon = button.Tag as Core.Models.Addon;

            addon.Status = "Up to date";
            Debug.WriteLine("NEW " + addon.ToString());
        }

        private static readonly Random Random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }



        //private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        //{

        //    Debug.WriteLine("CLICK " + e.ClickedItem.ToString());

        //}

        //private void FlyoutBase_OnOpening(object sender, object e)
        //{
        //    var menuflyout = sender as MenuFlyout;

        //    Debug.WriteLine("openeing " + sender.ToString() + " " + e.ToString());
        //}



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
                    
                    task.Progress = async (installResult, progress) => await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        try
                        {
                            
                            //Debug.WriteLine("progg");
                            //Debug.WriteLine("current: "+progress.BytesReceived+" totla: "+progress.TotalBytesToReceive);
                            ////StatusTextBlock.Text ="Stage: "+progress.Stage+ " Progress: " + (progress.BytesReceived /progress.TotalBytesToReceive);
                            StatusProgressBar.Value= ((int)(progress.Stage));
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                            Console.WriteLine(exception.StackTrace);
                            
                        }
                    });

                    //return await task;
                    var result = await task;
                   // Debug.WriteLine(result);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                }
            }
            //var result =await AppHelper.DoCurlAsync();
            //Debug.WriteLine(result);
        }
    }

}
