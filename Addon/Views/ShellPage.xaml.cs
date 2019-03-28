using System;
using System.Diagnostics;
using Addon.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Addon.Views
{
    // TODO WTS: Change the icons and titles for all NavigationViewItems in ShellPage.xaml.
    public sealed partial class ShellPage : Page
    {
        public ShellViewModel ViewModel { get; } = new ShellViewModel();

        public ShellPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.Initialize(shellFrame, navigationView, KeyboardAccelerators);
            
        }

        /*
         *
         *
         * <ComboBox ItemsSource="{x:Bind Fonts}" DisplayMemberPath="Item1" SelectedValuePath="Item2"
           Header="Font" Width="200" Loaded="Combo2_Loaded"/>

        <ListBox ItemsSource="{x:Bind Fonts}" DisplayMemberPath="Item1" SelectedValuePath="Item2" Height="164" Loaded="ListBox2_Loaded"/>
         */
    }
}
