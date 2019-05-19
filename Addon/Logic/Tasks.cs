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
        public static async Task<List<Core.Models.Addon>> RefreshGameFolder(Game game)
        {
            
            var addonDatas = Singleton<Session>.Instance.AddonData.Select(ad=>ad.FolderName).ToHashSet();
            var gameFolder = await StorageFolder.GetFolderFromPathAsync(game.AbsolutePath);
            var storageFolderQueryResult = gameFolder.CreateFolderQuery(CommonFolderQuery.DefaultQuery);
            var folders = await storageFolderQueryResult.GetFoldersAsync();
            var filteredFolders = new List<StorageFolder>();
            foreach (var folder in folders)
            {

                if (addonDatas.Contains(folder.Name))
                {
                    filteredFolders.Add(folder);
                }

                //var allMatchingFolders = addonDatas
                //    .Where(ad => ad.FolderName.Equals(folder.Name, StringComparison.OrdinalIgnoreCase))
                //    .ToList();
                //if (allMatchingFolders.Count == 1)
                //{
                //    filteredFolders.Add(folder);
                //}
                //else if (allMatchingFolders.Count > 1)
                //{
                //    filteredFolders.Add(folder);
                //    Debug.WriteLine("RefreshGameFoder: Found " + allMatchingFolders.Count + " matches for " + folder.Name);
                //}


            }


            var tasks = filteredFolders
                .Select(Toc.FolderToTocFile)
                .Where(task => task != null);

            var proccessed = await Task.WhenAll(tasks);

            var addons = proccessed.Where(tf => tf != null && !tf.IsKnownSubFolder)
                .Select(tf => new Core.Models.Addon(game, tf.StorageFolder.Name, tf.StorageFolder.Path)
                {
                    Version = tf.Version,
                    GameVersion = tf.GameVersion,
                    Title = tf.Title
                }).ToList();

           

           
            return addons;
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
            var addonList = new List<Core.Models.Addon>(addons);
            var tasks = new List<Task>();
            // Start tasks with a small delay to not get a UI spike
            foreach (var addon in addonList)
            {
                tasks.Add(FindProjectUrlAndDownLoadVersionsFor(addon));
                await Task.Delay(25);
            }
            await Task.WhenAll(tasks);


            //await Sorter.Sort(addons);
        }

        public static async Task FindProjectUrlAndDownLoadVersionsFor(Core.Models.Addon addon)
        {
            if (addon.IsIgnored)
            {
                return;
            }
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
            if (addon.IsIgnored)
            {
                return;
            }
            addon.Message = "Downloading...";
            addon.Progress = 0;
            addon.Status = Core.Models.Addon.UPDATING;

            var file = await Task.Run(() => Update.DLWithHttpProgress(addon, download));

            if (file == null)
            {
                addon.Status = Core.Models.Addon.UNKNOWN;
                addon.Message = string.Empty;
                addon.Progress = 0;
                return;
            }

            addon.Message = "Extracting...";
            addon.Progress = 0;

            if (addon.ProjectUrl.Equals(Version.ELVUI))
            {
                var trash = await Task.Run(() => Update.UpdateAddonOld(addon, download, file));
                addon.CurrentDownload = download;
                await Update.AddSubFolders(addon, trash.Item2);

                addon.Message = string.Empty;
                await Task.Run(() => Update.Cleanup(file.Name, trash.Item1));
            }
            else
            {


                var subFolders = await Task.Run(() => Update.UpdateAddon2(addon, file));
                addon.CurrentDownload = download;
                await Update.AddSubFolders(addon, subFolders);

                addon.Message = string.Empty;
                await Task.Run(() => Update.Cleanup2(file.Name));
            }
        }

        internal static async Task Remove(Core.Models.Addon addon)
        {
            addon.Game.Addons.Remove(addon);
            await Task.Run(() => Update.RemoveFilesFor(addon));
            Debug.WriteLine("Remove done for " + addon.FolderName);
        }

    }
}
