using AddonManager.Core.Models;
using System.Collections.Generic;
using static AddonToolkit.Model.Enums;

namespace AddonManager.Core.Storage
{
    public class SaveableGame
    {
        public GAME_TYPE GameType { get; set; }
        public string AbsolutePath { get; set; }
        public string DisplayName { get; set; }
        public List<SaveableAddon> Addons { get; set; }

        public override string ToString()
        {
            return $"{nameof(AbsolutePath)}: {AbsolutePath}, {nameof(DisplayName)}: {DisplayName}, {nameof(Addons)}: {Addons}";
        }

        public Game AsGame()
        {
            var game = new Game(AbsolutePath, GameType)
            {
                DisplayName = string.IsNullOrEmpty(this.DisplayName) ? "W" : this.DisplayName
            };
            foreach (var saveableAddon in Addons)
            {
                game.Addons.Add(saveableAddon.AsAddon(game));
            }
            return game;
        }
    }
}