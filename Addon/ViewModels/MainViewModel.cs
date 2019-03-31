using System;
using System.Diagnostics;
using Addon.Core.Models;
using Addon.Helpers;
using Addon.Views;

namespace Addon.ViewModels
{
    public class MainViewModel : Observable
    {
        public Game Game { get; set; }

        public MainViewModel()
        {
            Game = ShellPage.StaticReference.SelectedGame;
        }

    }
}
