using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;


namespace Addon.Core.Models
{
    public class Session
    {
        public Game SelectedGame { get; set; }
        public ObservableCollection<Game> Games { get; } = new ObservableCollection<Game>();

        public ObservableCollection<StoreAddon> StoreAddons { get; set; }// = new ObservableCollection<StoreAddon>();

        public Session()
        {
            //SelectedGame = new Game { AbsolutePath = "C:/Program Files/Wow" };
            //Debug.WriteLineIf(SelectedGame != null, "SESSION Constructor " + SelectedGame.ToString());
            //for (int i = 0; i < 30; i++)
            //{
            //    SelectedGame.Addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Updateable", GameVersion = "80100" });

            //}
            //Games.Add(SelectedGame);

            //var SecondGame = new Game { AbsolutePath = "F:/Games/Wow/__ptr__" };

            //for (int i = 0; i < 5; i++)
            //{
            //    SecondGame.Addons.Add(new Core.Models.Addon { Title = "Details", Version = "1.24", Status = "Updateable", GameVersion = "80100" });

            //}
            //Games.Add(SecondGame);


        }
    }
}
