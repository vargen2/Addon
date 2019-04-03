using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Addon.Core.Models;


namespace Addon.Helpers
{
    public class AppHelper
    {
        public async static Task<Game> FolderToGame(Windows.Storage.StorageFolder folder)
        {
            var game = new Game(folder.Path);
            var storageFolderQueryResult = folder.CreateFolderQuery(CommonFolderQuery.DefaultQuery);
            var folders = await storageFolderQueryResult.GetFoldersAsync();

            foreach (var f in folders)
            {
                game.Addons.Add(new Core.Models.Addon(game, f.Name, f.Path));
            }

            return game;
        }

    }
}
