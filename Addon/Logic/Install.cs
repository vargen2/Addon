using Addon.Core.Helpers;
using Addon.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace Addon.Logic
{
    internal static class Install
    {
        internal static async Task InstallAddon(StoreAddon storeAddon)
        {
            //TODO add functionality for storeaddon to progress
            storeAddon.Status = StoreAddon.INSTALLING;
            var game = Singleton<Session>.Instance.SelectedGame;
            var tempAddon = new Core.Models.Addon(game, storeAddon.Url, game.AbsolutePath + @"\" + storeAddon.Url) { };
            await Tasks.FindProjectUrlAndDownLoadVersionsFor(tempAddon);
            var download = tempAddon.SuggestedDownload;
            var file = await Update.DownloadFile(tempAddon, download);
            var trash = await InstallAddon(tempAddon, download, file);
            tempAddon.Message="";
            tempAddon.CurrentDownload = download;
            try
            {
                await Tasks.RefreshTocFileFor(new List<Core.Models.Addon>() { tempAddon });
                game.Addons.Add(tempAddon);
                await Update.AddSubFolders(tempAddon, trash.Item3);
                //await Tasks.Sort(game.Addons);
                storeAddon.Status = StoreAddon.INSTALLED;
            }
            catch (FileNotFoundException e)
            {
                var extractFolderPath = Update.localFolder.Path + @"\" + file.Name.Replace(".zip", "");
                var extractFolder = await StorageFolder.GetFolderFromPathAsync(extractFolderPath);
                var extractFolders = await extractFolder.GetFoldersAsync();

                var extractSet = new HashSet<string>(extractFolders.Select(f => f.Name.ToLower()).ToList());

                var folder = await StorageFolder.GetFolderFromPathAsync(game.AbsolutePath);
                var storageFolderQueryResult = folder.CreateFolderQuery(CommonFolderQuery.DefaultQuery);
                var gameFolders = await storageFolderQueryResult.GetFoldersAsync();

                var folders = gameFolders.Where(f => extractSet.Contains(f.Name.ToLower())).ToList();

                foreach (var item in folders)
                {
                    Debug.WriteLine(item.Name + " " + item.Path);
                }

                var tasks = await Task.WhenAll(folders.Select(Tasks.FolderToTocFile));

                var newAddons = tasks.Where(tf => tf != null && !tf.IsKnownSubFolder)
                     .Select(tf => new Core.Models.Addon(game, tf.StorageFolder.Name, tf.StorageFolder.Path)
                     {
                         Version = tf.Version,
                         GameVersion = tf.GameVersion,
                         Title = tf.Title
                     })
                     .ToList();

                foreach (var item in newAddons)
                {
                    game.Addons.Add(item);
                }
                var dlVersionTasks = newAddons.Select(Tasks.FindProjectUrlAndDownLoadVersionsFor).ToArray();
                await Task.WhenAll(dlVersionTasks);
                await Tasks.Sort(game);
                storeAddon.Status = StoreAddon.UNKNOWN;
            }
            await Update.Cleanup(new Tuple<string, string>(trash.Item1, trash.Item2));
        }



        private static async Task<Tuple<string, string, List<string>>> InstallAddon(Core.Models.Addon addon, Download download, StorageFile file)
        {
            var subFoldersToDelete = new List<string>();
            var extractFolderPath = Update.localFolder.Path + @"\" + file.Name.Replace(".zip", "");
            try
            {
                ZipFile.ExtractToDirectory(file.Path, extractFolderPath);
                var extractFolder = await StorageFolder.GetFolderFromPathAsync(extractFolderPath);
                var folders = await extractFolder.GetFoldersAsync();
                var gameFolder = await StorageFolder.GetFolderFromPathAsync(addon.Game.AbsolutePath);
                foreach (var folder in folders)
                {
                    try
                    {
                        var delete = await gameFolder.GetFolderAsync(folder.Name);
                        await delete.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("[ERROR] No folder found to delete. " + e.Message);
                    }

                }

                foreach (var folder in folders)
                {
                    await Update.CopyFolderAsync(folder, gameFolder);
                }
                Debug.WriteLine("install copy ok");

                var foldersAsList = new List<StorageFolder>(folders);
                subFoldersToDelete = foldersAsList.Select(f => f.Name).Where(name => !name.Equals(addon.FolderName)).ToList();

            }
            catch (Exception e)
            {
                Debug.WriteLine("[ERROR] InstallAddon. " + e.Message + ", " + e.StackTrace);
            }
            return new Tuple<string, string, List<string>>(file.Path, extractFolderPath, subFoldersToDelete);
        }


    }
}
