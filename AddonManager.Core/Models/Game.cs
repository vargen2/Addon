using AddonManager.Core.Storage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using static AddonToolkit.Model.Enums;

namespace AddonManager.Core.Models
{
    public class Game : INotifyPropertyChanged
    {
        private string displayName = "W";

        public string DisplayName
        {
            get => displayName;
            set
            {
                displayName = value;
                NotifyPropertyChanged();
            }
        }

        private bool isLoading = false;

        public bool IsLoading
        {
            get => isLoading;
            set
            {
                //if (value == isLoading)
                //    return;
                isLoading = value;
                NotifyPropertyChanged();
            }
        }

        public GAME_TYPE GameType { get; }

        public string AbsolutePath { get; }
        public ObservableCollection<Addon> Addons { get; } = new ObservableCollection<Addon>();

        public Game(string absolutePath, GAME_TYPE gameType)
        {
            AbsolutePath = absolutePath ?? throw new NullReferenceException();
            GameType = gameType;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return base.ToString() + " " + AbsolutePath;
        }

        public SaveableGame AsSaveableGame()
        {
            return new SaveableGame()
            {
                AbsolutePath = this.AbsolutePath,
                DisplayName = this.DisplayName,
                GameType = this.GameType,
                Addons = this.Addons.Select(a => a.AsSaveableAddon()).ToList()
            };
        }
    }
}