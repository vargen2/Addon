using Addon.Core.Helpers;
using Addon.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace Addon.Logic
{
    internal static class Install
    {
        internal static async Task InstallAddon(StoreAddon storeAddon)
        {
            var game = Singleton<Session>.Instance.SelectedGame;
            if (game.AbsolutePath.Equals(Session.EMPTY_GAME))
            {
                return;
            }

            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return;
            }

            storeAddon.Status = StoreAddon.INSTALLING;

            var tempAddon = new Core.Models.Addon(game, storeAddon.AddonData.FolderName, game.AbsolutePath + @"\" + storeAddon.AddonData.FolderName) { };
            await Tasks.FindProjectUrlAndDownLoadVersionsFor(tempAddon);
            var download = tempAddon.SuggestedDownload;
            storeAddon.Message = "Downloading...";
            var file = await Task.Run(() => Update.DLWithHttpProgress(tempAddon, download, storeAddon));
            storeAddon.Progress = 0;
            storeAddon.Message = "Extracting...";

            if (tempAddon.ProjectUrl.Equals(Version.ELVUI))
            {
                // TODO move to method
                var trash = await Task.Run(() => Update.UpdateAddonOld(tempAddon, download, file));
                tempAddon.CurrentDownload = download;
                await Tasks.RefreshTocFileFor(new List<Core.Models.Addon>() { tempAddon });
                game.Addons.Add(tempAddon);
                await Update.AddSubFolders(tempAddon, trash.Item2);
                storeAddon.Status = StoreAddon.INSTALLED;
                tempAddon.Message = string.Empty;
                await Task.Run(() => Update.Cleanup(file.Name, trash.Item1));
            }
            else
            {
                // TODO move to method
                var trash = await Task.Run(() => Update.UpdateAddon2(tempAddon, file, storeAddon));
                storeAddon.Progress = 0;
                tempAddon.Message = string.Empty;
                storeAddon.Message = "Configuring...";
                tempAddon.CurrentDownload = download;
            
                try
                {
                    await Tasks.RefreshTocFileFor(new List<Core.Models.Addon>() { tempAddon });
                    game.Addons.Add(tempAddon);
                    await Update.AddSubFolders(tempAddon, trash);

                    storeAddon.Status = StoreAddon.INSTALLED;
                    await Task.Run(() => Update.Cleanup2(file.Name));
                }
                catch (FileNotFoundException)
                {
                    var newAddons = await Task.Run(() => AnotherInstall(file, game));
                    foreach (var item in newAddons)
                    {
                        game.Addons.Add(item);
                    }
                    var dlVersionTasks = newAddons.Select(Tasks.FindProjectUrlAndDownLoadVersionsFor).ToArray();
                    await Task.WhenAll(dlVersionTasks);

                    storeAddon.Status = StoreAddon.UNKNOWN;
                }
            }
            storeAddon.Message = string.Empty;


        }

        private static async Task<List<Core.Models.Addon>> AnotherInstall(StorageFile file, Game game)
        {

            var extractFolderPath = Update.localFolder.Path + @"\" + file.Name.Replace(".zip", "");
            ZipFile.ExtractToDirectory(file.Path, extractFolderPath);
            var extractFolder = await StorageFolder.GetFolderFromPathAsync(extractFolderPath);
            var extractFolders = await extractFolder.GetFoldersAsync();

            var extractSet = new HashSet<string>(extractFolders.Select(f => f.Name.ToLower()).ToList());

            var folder = await StorageFolder.GetFolderFromPathAsync(game.AbsolutePath);
            var storageFolderQueryResult = folder.CreateFolderQuery(CommonFolderQuery.DefaultQuery);
            var gameFolders = await storageFolderQueryResult.GetFoldersAsync();

            var folders = gameFolders.Where(f => extractSet.Contains(f.Name.ToLower())).ToList();

            foreach (var item in folders)
            {
                Debug.WriteLine("AnotherInstall: " + item.Name + " " + item.Path);
            }

            var tasks = await Task.WhenAll(folders.Select(Toc.FolderToTocFile));

            var newAddons = tasks.Where(tf => tf != null && !tf.IsKnownSubFolder)
                 .Select(tf => new Core.Models.Addon(game, tf.StorageFolder.Name, tf.StorageFolder.Path)
                 {
                     Version = tf.Version,
                     GameVersion = tf.GameVersion,
                     Title = tf.Title
                 })
                 .ToList();
            await Update.Cleanup(file.Name, extractFolderPath);
            return newAddons;
        }
    }
}
