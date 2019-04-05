using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Addon.Core.Storage;


namespace Addon.Core.Models
{
    public class Session
    {

        public Game SelectedGame { get; set; }
        public ObservableCollection<Game> Games { get; } = new ObservableCollection<Game>();

        public ObservableCollection<StoreAddon> StoreAddons { get; set; }// = new ObservableCollection<StoreAddon>();
        
        public SaveableSession AsSaveableSession()
        {
            return new SaveableSession()
            {
                SelectedGame = this.SelectedGame.AsSaveableGame(),
                Games = this.Games.Select(g => g.AsSaveableGame()).ToList()
            };
        }
    }
}
