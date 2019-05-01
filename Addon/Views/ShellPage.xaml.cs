using Addon.Core.Models;
using Addon.Logic;
using Addon.ViewModels;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Addon.Views
{
    // TODO Acrylic
    public sealed partial class ShellPage : Page
    {
        public ShellViewModel ViewModel { get; } = new ShellViewModel();

        public ShellPage()
        {
            InitializeComponent();
            DataContext = ViewModel;

            /*
             *
             *      Color fgColor = safe_cast<SolidColorBrush^>(Application::Current->Resources->Lookup("SystemControlPageTextBaseHighBrush"))->Color;
            Color inactivefgColor = safe_cast<SolidColorBrush^>(Application::Current->Resources->Lookup("SystemControlForegroundChromeDisabledLowBrush"))->Color;
            Color hoverbgColor = safe_cast<SolidColorBrush^>(Application::Current->Resources->Lookup("SystemControlBackgroundListLowBrush"))->Color;
            Color hoverfgColor = safe_cast<SolidColorBrush^>(Application::Current->Resources->Lookup("SystemControlForegroundBaseHighBrush"))->Color;
            Color pressedbgColor = safe_cast<SolidColorBrush^>(Application::Current->Resources->Lookup("SystemControlBackgroundListMediumBrush"))->Color;
            Color pressedfgCoolor = safe_cast<SolidColorBrush^>(Application::Current->Resources->Lookup("SystemControlForegroundBaseHighBrush"))->Color;
            applicationTitleBar->ButtonBackgroundColor = bgColor;
            applicationTitleBar->ButtonForegroundColor = fgColor;
            applicationTitleBar->ButtonInactiveBackgroundColor = bgColor;
            applicationTitleBar->ButtonInactiveForegroundColor = inactivefgColor;
            applicationTitleBar->ButtonHoverBackgroundColor = hoverbgColor;
            applicationTitleBar->ButtonHoverForegroundColor = hoverfgColor;
            applicationTitleBar->ButtonPressedBackgroundColor = pressedbgColor;
            applicationTitleBar->ButtonPressedForegroundColor = pressedfgCoolor;
             *
             */


            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            var half = Windows.UI.Colors.White;
            half.A = 90;
            // Set active window colors
            titleBar.ButtonForegroundColor = Windows.UI.Colors.Black;
            titleBar.ButtonBackgroundColor = Windows.UI.Colors.Transparent;
            titleBar.ButtonHoverForegroundColor = Windows.UI.Colors.Black;
            titleBar.ButtonHoverBackgroundColor = half;
            titleBar.ButtonPressedForegroundColor = Windows.UI.Colors.Gray;
            titleBar.ButtonPressedBackgroundColor = Windows.UI.Colors.Transparent;

            // Set inactive window colors
            titleBar.ButtonInactiveForegroundColor = Windows.UI.Colors.Gray;
            titleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.Transparent;




            // Hide default title bar.
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;

            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);

            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(AppTitleBar);

            // Register a handler for when the size of the overlaid caption control changes.
            // For example, when the app moves to a screen with a different DPI.
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            // Register a handler for when the title bar visibility changes.
            // For example, when the title bar is invoked in full screen mode.
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;


            ViewModel.Initialize(shellFrame, navigationView, KeyboardAccelerators);

            //ApplicationView.GetForCurrentView().Title = Singleton<Session>.Instance.SelectedGame.AbsolutePath;
            // Singleton<Session>.Instance.PropertyChanged += Session_PropertyChanged;


        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            var clickedGame = listBox.SelectedValue as Game;
            if (clickedGame == ViewModel.Session.SelectedGame)
                return;

            ViewModel.Session.SelectedGame = listBox.SelectedValue as Game;
            // NavigationService.ForceNavigateMainPage();

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

                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(folder);

                var game = new Game(folder.Path);

                //   Debug.WriteLine("Picked folder: " + folder.Name);
                await Task.Run(() => Tasks.RefreshGameFolder(game));
                await Task.Run(() => Tasks.FindProjectUrlAndDownLoadVersionsFor(game.Addons));
                ViewModel.Session.Games.Add(game);

                ViewModel.Session.SelectedGame = game;
            }
            else
            {
                Debug.WriteLine("Operation cancelled.");
            }


        }


        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Get the size of the caption controls area and back button 
            // (returned in logical pixels), and move your content around as necessary.
            LeftPaddingColumn.Width = new GridLength(coreTitleBar.SystemOverlayLeftInset);
            RightPaddingColumn.Width = new GridLength(coreTitleBar.SystemOverlayRightInset);
            // TitleBarButton.Margin = new Thickness(0, 0, coreTitleBar.SystemOverlayRightInset, 0);

            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = coreTitleBar.Height;
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (sender.IsVisible)
            {
                AppTitleBar.Visibility = Visibility.Visible;
            }
            else
            {
                AppTitleBar.Visibility = Visibility.Collapsed;
            }
        }

        //private void Session_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName.Equals("SelectedGame"))
        //    {
        //        var game = Singleton<Session>.Instance.SelectedGame;
        //        if (game != null)
        //        {
        //            ApplicationView.GetForCurrentView().Title = Singleton<Session>.Instance.SelectedGame.AbsolutePath;
        //        }
        //    }
        //}
    }
}
