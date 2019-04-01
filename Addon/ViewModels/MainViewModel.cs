using System;
using System.Diagnostics;
using Addon.Core.Helpers;
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
            //Game = ShellPage.StaticReference.Session.SelectedGame;
            Game = Singleton<Session>.Instance.SelectedGame;
        }

    }
}
