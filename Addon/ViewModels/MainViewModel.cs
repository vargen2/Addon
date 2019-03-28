using System;
using System.Diagnostics;
using Addon.Core.Models;
using Addon.Helpers;

namespace Addon.ViewModels
{
    public class MainViewModel : Observable
    {
        public Game Game { get; set; }

        public MainViewModel()
        {
            Game = new Game { AbsolutePath = "C:/Program Files/Wow"        };
            Debug.WriteLineIf(Game != null, Game.ToString());
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
            Game.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });

        }
    }
}
