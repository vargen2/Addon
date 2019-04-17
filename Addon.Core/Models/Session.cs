using Addon.Core.Storage;
using Addon.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;


namespace Addon.Core.Models
{
    public class Session : Observable
    {
        private Game _selectedGame;
        public Game SelectedGame
        {
            get => _selectedGame;
            set => Set(ref _selectedGame, value);
        }

        public Session()
        {
            _selectedGame = new Game("No Game Found")
            {
                IsLoading = false
            };
        }

        public ObservableCollection<Game> Games { get; } = new ObservableCollection<Game>();

        public ObservableCollection<StoreAddon> StoreAddons { get; set; } = new ObservableCollection<StoreAddon>();

        public HashSet<string> KnownSubFolders { get; } = new HashSet<string>();

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
