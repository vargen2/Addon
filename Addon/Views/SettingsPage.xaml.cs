using System;

using Addon.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Addon.Core.Helpers;

namespace Addon.Views
{
    // TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere
    public sealed partial class SettingsPage : Page
    {
        //public SettingsViewModel ViewModel { get; } = new SettingsViewModel();
        public SettingsViewModel ViewModel { get; } = Singleton<SettingsViewModel>.Instance;

        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            //ViewModel.Initialize();
            await ViewModel.EnsureInstanceInitializedAsync();
        }
    }
}
