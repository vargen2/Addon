using System;

using Addon.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Addon.Views
{
    public sealed partial class BrowsePage : Page
    {
        public BrowseViewModel ViewModel { get; } = new BrowseViewModel();

        public BrowsePage()
        {
            InitializeComponent();
        }
    }
}
