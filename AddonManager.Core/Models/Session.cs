using AddonManager.Core.Helpers;
using AddonManager.Core.Storage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AddonManager.Core.Models
{
    public class Session : Observable
    {
        public const string EMPTY_GAME = "No Game found";
        private Game _selectedGame;
        public Game SelectedGame
        {
            get => _selectedGame;
            set
            {

                Set(ref _selectedGame, value);
                OnPropertyChanged("IsNoGameSelected");
                OnPropertyChanged("IsGameSelected");
                OnPropertyChanged("IsRefreshButtonEnabled");
            }
        }

        private bool _refreshing;

        public bool Refreshing
        {
            get => _refreshing;
            set
            {
                Set(ref _refreshing, value);
                OnPropertyChanged("IsRefreshButtonEnabled");
            }
        }

        public bool IsNoGameSelected => SelectedGame == null ? true : SelectedGame.AbsolutePath.Equals(EMPTY_GAME);
        public bool IsGameSelected => SelectedGame == null ? false : !SelectedGame.AbsolutePath.Equals(EMPTY_GAME);

        public bool IsRefreshButtonEnabled => IsGameSelected && !Refreshing;

        //private bool isInstalling = false;
        //public bool IsInstalling
        //{
        //    get { return isInstalling; }
        //    set
        //    {
        //        Set(ref isInstalling, value);
        //    }
        //}

        public Session()
        {
            _selectedGame = new Game(EMPTY_GAME)
            {
                IsLoading = false
            };
        }

        public ObservableCollection<Game> Games { get; } = new ObservableCollection<Game>();

        public ObservableCollection<StoreAddon> StoreAddons { get; set; } = new ObservableCollection<StoreAddon>();

        public HashSet<string> KnownSubFolders { get; } = new HashSet<string>();

        public List<AddonData> AddonData { get; } = new List<AddonData>();

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
