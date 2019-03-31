using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Addon.Core.Models
{
    public class Game
    {
        public string AbsolutePath { get; set; }

        private ObservableCollection<Addon> addons = new ObservableCollection<Addon>();

        public ObservableCollection<Addon> Addons
        {
            get { return addons; }
        }

        //public Game()
        //{
        //    AbsolutePath = "C:/Program Files/Wow";
        //    addons.Add(new Core.Models.Addon { Title = "BigWigs", Version = "10.4", Status = "Up to date", GameVersion = "80100" });
        //}

        public override string ToString()
        {
            return base.ToString() + " " + AbsolutePath;
        }

    }
}
