using Addon.Logic;
using Addon.ViewModels;
using System;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Addon.Views
{
    public sealed partial class MasterDetailPage : Page
    {
        // private Object oldSelected;

        public MasterDetailViewModel ViewModel { get; } = new MasterDetailViewModel();

        public MasterDetailPage()
        {
            InitializeComponent();
            Loaded += MasterDetailPage_Loaded;

        }

        private async void MasterDetailPage_Loaded(object sender, RoutedEventArgs e)
        {

            await ViewModel.LoadDataAsync(MasterDetailsViewControl.ViewState);
            //MyMasterDetailPage = this;
            //Window.Current.Content.PointerPressed +=ForegroundElement_PointerPressed;
            //MasterDetailsViewControl.SelectionChanged += MasterDetailsViewControl_SelectionChanged;
        }

        //private void MasterDetailsViewControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    Debug.WriteLine("SELECTION CHANGED "+e.ToString());
        //    if (MasterDetailsViewControl.SelectedItem != null)
        //    {
        //        oldSelected = MasterDetailsViewControl.SelectedItem;
        //    }
        //    else if (MasterDetailsViewControl.SelectedItem == null)
        //    {
        //        Debug.WriteLine("NULL selected");
        //        if (ViewModel.Session.SelectedGame != null && ViewModel.Session.SelectedGame.Addons.Count > 0 && oldSelected != null)
        //        {
        //            MasterDetailsViewControl.SelectedItem = oldSelected;

        //        }
        //    }
        //}

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
            var addons = ViewModel.Session.SelectedGame.Addons;
            await Tasks.FindProjectUrlAndDownLoadVersionsFor(addons);
            Debug.WriteLine("Version downloaded for all addons");
        }

        private async void RefreshTocFileForAllInSelectedGame(object sender, RoutedEventArgs e)
        {
            var addons = ViewModel.Session.SelectedGame.Addons.ToList();
            await Tasks.RefreshTocFileFor(addons);
            Debug.WriteLine("Refreshed toc files for all addons");
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

        private async void RemoveSelectedGame(object sender, RoutedEventArgs e)
        {
            var res = ResourceLoader.GetForCurrentView();
            var appName = res.GetString("AppDisplayName");
            ContentDialog dialog = new ContentDialog()
            {
                Title = "Remove Game?",
                Content = "Remove " + ViewModel.Session.SelectedGame.AbsolutePath + " from " + appName + "?",
                PrimaryButtonText = "Ok",
                CloseButtonText = "Cancel"
            };

            var response = await dialog.ShowAsync();
            if (response == ContentDialogResult.Primary)
            {
                var game = ViewModel.Session.SelectedGame;
                if (game != null && ViewModel.Session.Games.Contains(game))
                {
                    ViewModel.Session.Games.Remove(game);
                    if (ViewModel.Session.Games.Count == 0)
                    {
                        ViewModel.Session.SelectedGame = new Core.Models.Game("No Game Found");
                    }
                    else
                    {
                        ViewModel.Session.SelectedGame=ViewModel.Session.Games.First();
                    }

                }

            }

        }





        //public void MyResize(double xDelta)
        //{
        //    //Debug.WriteLine(ContentArea.ActualWidth);
        //    var elements = ContentArea.Children;
        //    foreach (var item in elements)
        //    {
        //        var mast = item as MasterDetailsView;
        //        mast.MasterPaneWidth = mast.MasterPaneWidth + xDelta;
        //        //Debug.WriteLine(mast.ActualWidth + ", " + mast.MasterPaneWidth);
        //        //Debug.WriteLine(mast.Parent.ToString());
        //    }
        //}

        // private void ForegroundElement_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        //{

        //    Debug.WriteLine("pressed");
        //    //isPressed = true;
        //    //xPrev = e.GetCurrentPoint(sender as UIElement).Position.X;
        //    ////var grid = this.Parent as Grid;
        //    //var gridParent = grid.Parent;

        //    //Debug.WriteLine(gridParent.ToString());
        //    //var elements = grid.Children;
        //    //foreach (var item in elements)
        //    //{
        //    //    Debug.WriteLine(item.ToString());
        //    //}

        //}
    }
}
