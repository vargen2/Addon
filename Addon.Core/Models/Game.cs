using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Addon.Core.Storage;

namespace Addon.Core.Models
{
    public class Game : INotifyPropertyChanged
    {
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

        public string AbsolutePath { get; }
        public ObservableCollection<Addon> Addons { get; } = new ObservableCollection<Addon>();

        public Game(string absolutePath)
        {
            AbsolutePath = absolutePath ?? throw new NullReferenceException();
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
                Addons = this.Addons.Select(a => a.AsSaveableAddon()).ToList()
            };
        }

    }
}
