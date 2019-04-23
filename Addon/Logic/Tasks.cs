using Addon.Core.Helpers;
using Addon.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace Addon.Logic
{
    public static class Tasks
    {
        public static async Task Sort(ObservableCollection<Core.Models.Addon> addons)
        {
            if (addons.Count == 0)
                return;

            var count = addons.Where(a => a.IsUpdateable).Count();
            if (count == 0)
                return;

            for (int i = 0; i < count; i++)
            {
                var addon = addons.LastOrDefault(a => a.IsUpdateable);
                if (addon == null)
                {
                    return;
                }
                var moveFrom = addons.IndexOf(addon);
                addons.Move(moveFrom, 0);
            }
            await Task.CompletedTask;
        }

        public static async Task Sort(Game game)
        {
            await Sort(game.Addons);
        }


        public static async Task RefreshGameFolder(Game game)
        {
            game.IsLoading = true;
            var folder = await StorageFolder.GetFolderFromPathAsync(game.AbsolutePath);
            var storageFolderQueryResult = folder.CreateFolderQuery(CommonFolderQuery.DefaultQuery);
            var folders = await storageFolderQueryResult.GetFoldersAsync();

            var tasks = await Task.WhenAll(folders.Select(Toc.FolderToTocFile));

            tasks.Where(tf => tf != null && !tf.IsKnownSubFolder)
                .Select(tf => new Core.Models.Addon(game, tf.StorageFolder.Name, tf.StorageFolder.Path)
                {
                    Version = tf.Version,
                    GameVersion = tf.GameVersion,
                    Title = tf.Title
                })
                .ToList().ForEach(game.Addons.Add);
            game.IsLoading = false;
        }



        public static async Task RefreshTocFileFor(IList<Core.Models.Addon> addons)
        {
            foreach (var addon in addons)
            {
                var folder = await StorageFolder.GetFolderFromPathAsync(addon.AbsolutePath);
                var tocFile = await Toc.FolderToTocFile(folder);
                addon.Title = tocFile.Title;
                addon.Version = tocFile.Version;
                addon.GameVersion = tocFile.GameVersion;
            }
        }






        public static async Task FindProjectUrlAndDownLoadVersionsFor(ObservableCollection<Core.Models.Addon> addons)
        {
            var tasks = addons.Select(FindProjectUrlAndDownLoadVersionsFor).ToArray();
            await Task.WhenAll(tasks);
            await Tasks.Sort(addons);
        }

        public static async Task FindProjectUrlAndDownLoadVersionsFor(Core.Models.Addon addon)
        {
            addon.Progress = 0;
            addon.Status = Core.Models.Addon.DOWNLOADING_VERSIONS;
            if (String.IsNullOrEmpty(addon.ProjectUrl))
            {
                addon.ProjectUrl = await Task.Run(() => Version.FindProjectUrlFor(addon));
            }
            addon.Downloads = await Task.Run(() => Version.DownloadVersionsFor(addon));
        }





        public static async Task UpdateAddon(Core.Models.Addon addon)
        {
            await UpdateAddon(addon, addon.SuggestedDownload);
        }

        public static async Task UpdateAddon(Core.Models.Addon addon, Download download)
        {
            //testa göra Task run på alla
            //ända inget i addon i metoderna
            addon.Message = "Downloading...";
            addon.Progress = 0;
            addon.Status = Core.Models.Addon.UPDATING;
            //var file = await Update.DownloadFile(addon, download);
            var file = await Task.Run(() => Update.DLWithHttp(addon, download));
            addon.Message = "Extract/Copy...";
            addon.Progress = 0;
            //Debug.WriteLine("file downloaded: " + file.Path);
            var trash = await Task.Run(() => Update.UpdateAddon(addon, download, file));
            addon.CurrentDownload = download;
            //Debug.WriteLine("Update addon complete: " + addon.FolderName);
            // await Sort(addon.Game);
            addon.Message = "";
            await Task.Run(() => Update.Cleanup(trash));
            //addon.Message = "";
           // Debug.WriteLine("Cleanup complete: " + addon.FolderName);
        }


        
    }
}
