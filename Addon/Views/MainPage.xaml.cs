using System;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Addon.ViewModels;

using Windows.UI.Xaml.Controls;

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
            var omega = button.Tag as Core.Models.Addon;
            Debug.WriteLine(omega.ToString());
           
            var length = ViewModel.Game.Addons.Count;
            var addon = ViewModel.Game.Addons.ElementAt(Random.Next(length));
            addon.Title = RandomString(9);
            Debug.WriteLine("NEW "+addon.Title);
        }
        private static readonly Random Random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
    
}
