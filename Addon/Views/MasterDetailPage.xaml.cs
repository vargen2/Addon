using Addon.Core.Models;
using Addon.Logic;
using Addon.ViewModels;
using System;
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
        public MasterDetailViewModel ViewModel { get; } = new MasterDetailViewModel();

        public MasterDetailPage()
        {
            InitializeComponent();
            Loaded += MasterDetailPage_Loaded;
        }

        private async void MasterDetailPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadDataAsync(MasterDetailsViewControl.ViewState);
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
            if (ViewModel.Session.SelectedGame.AbsolutePath.Equals(Session.EMPTY_GAME))
            {
                return;
            }
            var button = sender as Button;
            button.IsEnabled = false;
            var progressRing = (button.Content as StackPanel).Children.OfType<ProgressRing>().FirstOrDefault();//.FindName("RefreshButtonProgressRing") as ProgressRing;
            var textBlock = (button.Content as StackPanel).Children.OfType<TextBlock>().FirstOrDefault();//.FindName("RefreshButtonProgressRing") as ProgressRing;
            progressRing.IsActive = true;
            textBlock.Visibility = Visibility.Collapsed;
            progressRing.Visibility = Visibility.Visible;

            var addons = ViewModel.Session.SelectedGame.Addons;
            await Tasks.FindProjectUrlAndDownLoadVersionsFor(addons);
            progressRing.IsActive = false;
            progressRing.Visibility = Visibility.Collapsed;
            textBlock.Visibility = Visibility.Visible;

            button.IsEnabled = true;
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
                        if (addon.IsIgnored)
                        {
                            ContentDialog updateAddonDialog = new ContentDialog()
                            {
                                Title = "Can't update ignored addon",
                                PrimaryButtonText = "Ok"
                            };
                            var response = await updateAddonDialog.ShowAsync();
                        }
                        else
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
                        }

                    };
                    submenu.Items.Add(menuItem);
                }
                menuflyuout.Items.Insert(menuflyuout.Items.Count - 1, submenu);
            }
        }

        private async void RemoveSelectedGame(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Session.SelectedGame.AbsolutePath.Equals(Session.EMPTY_GAME))
            {
                return;
            }

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
                        ViewModel.Session.SelectedGame = new Game(Session.EMPTY_GAME);
                        ViewModel.Selected = null;
                    }
                    else
                    {
                        ViewModel.Session.SelectedGame = ViewModel.Session.Games.First();
                    }

                }

            }

        }

        private async void MenuFlyoutItem_Click_Open_Edit_URL_DIALOG(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuFlyoutItem;
            Core.Models.Addon addon = menuItem.Tag as Core.Models.Addon;

            TextBlock textBlock = new TextBlock()
            {
                Margin = new Thickness(0, 0, 0, 16),
                TextWrapping = TextWrapping.Wrap,
                Text = "Example: For https://www.curseforge.com/wow/addons/deadly-boss-mods You would type: deadly-boss-mods"
            };
            TextBlock currentValueTextBlock = new TextBlock()
            {
                Margin = new Thickness(0, 0, 0, 16),
                TextWrapping = TextWrapping.Wrap,
                Text = "Current URL: " + addon.ProjectUrl
            };
            TextBlock testResultTextBlock = new TextBlock()
            {
                Margin = new Thickness(0, 0, 0, 16),
                TextWrapping = TextWrapping.Wrap,
                Text = ""
            };
            TextBox textBox = new TextBox() { HorizontalAlignment = HorizontalAlignment.Stretch };
            Button testButton = new Button() { HorizontalAlignment = HorizontalAlignment.Stretch, Content = "Test", Margin = new Thickness(8, 0, 0, 0) };


            Grid.SetColumn(testButton, 1);
            Grid row = new Grid()
            {
                Margin = new Thickness(0, 0, 0, 16),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            row.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100, GridUnitType.Pixel) });
            row.Children.Add(textBox);
            row.Children.Add(testButton);

            StackPanel stackPanel = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(currentValueTextBlock);
            stackPanel.Children.Add(row);
            stackPanel.Children.Add(testResultTextBlock);

            ContentDialog dialog = new ContentDialog()
            {
                Title = "Set URL for " + addon.FolderName,
                Content = stackPanel,
                PrimaryButtonText = "Save",
                CloseButtonText = "Cancel",
                IsPrimaryButtonEnabled = false
            };
            string successUrl = string.Empty;
            var progressRing = new ProgressRing() { IsEnabled = true, IsActive = true };
            testButton.Click += async (a, b) =>
            {
                testButton.Content = progressRing;
                testResultTextBlock.Text = "Testing...";
                testButton.IsEnabled = false;
                var foundUrl = await Logic.Version.FindProjectUrlFor(textBox.Text.Trim());
                if (string.IsNullOrEmpty(foundUrl))
                {
                    testResultTextBlock.Text = "We didn't find any url";
                    dialog.IsPrimaryButtonEnabled = false;
                }
                else
                {
                    testResultTextBlock.Text = foundUrl;
                    successUrl = foundUrl;
                    dialog.IsPrimaryButtonEnabled = true;
                }
                testButton.Content = "Test";
                testButton.IsEnabled = true;
            };

            var response = await dialog.ShowAsync();
            if (response == ContentDialogResult.Primary)
            {
                if (!string.IsNullOrEmpty(successUrl))
                {
                    addon.ProjectUrl = successUrl;
                }
            }
        }

        private async void MenuFlyoutItem_Click_Remove_Addon(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuFlyoutItem;
            Core.Models.Addon addon = menuItem.Tag as Core.Models.Addon;
            var folderNames = addon.FolderName + "\r\n";
            folderNames += string.Join("\r\n", addon.SubFolders);

            ContentDialog dialog = new ContentDialog()
            {
                Title = "Remove " + addon.FolderName + "?",
                Content = folderNames,
                PrimaryButtonText = "Remove",
                CloseButtonText = "Cancel"
            };

            var response = await dialog.ShowAsync();
            if (response == ContentDialogResult.Primary)
            {
                await Tasks.Remove(addon);
            }
        }








    }
}
