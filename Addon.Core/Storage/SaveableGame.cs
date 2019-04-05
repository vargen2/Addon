using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Addon.Core.Models;

namespace Addon.Core.Storage
{
    public class SaveableGame
    {
        public string AbsolutePath { get; set; }
        public List<SaveableAddon> Addons { get; set; }


        public override string ToString()
        {
            return $"{nameof(AbsolutePath)}: {AbsolutePath}, {nameof(Addons)}: {Addons}";
        }

        public Game AsGame()
        {
            var game = new Game(AbsolutePath);
            foreach (var saveableAddon in Addons)
            {
                game.Addons.Add(saveableAddon.AsAddon(game));
            }
            return game;
        }
    }
}
