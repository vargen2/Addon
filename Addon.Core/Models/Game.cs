using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Addon.Core.Models
{
    public class Game
    {
        public string AbsolutePath { get; }
        public ObservableCollection<Addon> Addons { get; } = new ObservableCollection<Addon>();

        public Game(string absolutePath)
        {
            AbsolutePath = absolutePath ?? throw new NullReferenceException();
        }



        public override string ToString()
        {
            return base.ToString() + " " + AbsolutePath;
        }

    }
}
