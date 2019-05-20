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
        public static async Task FolderToGame(Game game, StorageFolder folder)
        {
            var tocFile = await Task.Run(() => Toc.FolderToTocFile(folder));
            if (tocFile == null || tocFile.IsKnownSubFolder)
            {
                return;
            }
            var a = new Core.Models.Addon(game, tocFile.StorageFolder.Name, tocFile.StorageFolder.Path)
            {
                Version = tocFile.Version,
                GameVersion = tocFile.GameVersion,
                Title = tocFile.Title
            };
            game.Addons.Add(a);
            await Task.Delay(500);
            await Tasks.FindProjectUrlAndDownLoadVersionsFor(a);
        }

        public static async Task RefreshGameFolder(Game game)
        {
            var addonDatas = Singleton<Session>.Instance.AddonData;
            var gameFolder = await StorageFolder.GetFolderFromPathAsync(game.AbsolutePath);
            var storageFolderQueryResult = gameFolder.CreateFolderQuery(CommonFolderQuery.DefaultQuery);
            var folders = await storageFolderQueryResult.GetFoldersAsync();
            var filteredFolders = new List<StorageFolder>();
            var SubFoldersToBeRemoved = new HashSet<string>();

            foreach (var folder in folders)
            {
                var allMatchingFolders = addonDatas
                    .Where(ad => ad.FolderName.Equals(folder.Name, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                if (allMatchingFolders.Count == 1)
                {

                    filteredFolders.Add(folder);
                    SubFoldersToBeRemoved.UnionWith(allMatchingFolders[0].SubFolders);
                }
                else if (allMatchingFolders.Count > 1)
                {
                    filteredFolders.Add(folder);
                    SubFoldersToBeRemoved.UnionWith(allMatchingFolders[0].SubFolders);
                    Debug.WriteLine("RefreshGameFoder: Found " + allMatchingFolders.Count + " matches for " + folder.Name);
                }
            }

            var filteredAndPurgedFolders = filteredFolders.Where(f => !SubFoldersToBeRemoved.Contains(f.Name)).ToList();

            var tasks = filteredAndPurgedFolders.Select(f => FolderToGame(game, f));
            await Task.WhenAll(tasks);
        }

        //public static async Task<List<Core.Models.Addon>> RefreshGameFolder(Game game)
        //{
        //    Debug.WriteLine("install game start");
        //    //var addonDatas = Singleton<Session>.Instance.AddonData.Select(ad=>ad.FolderName).ToHashSet();
        //    var addonDatas = Singleton<Session>.Instance.AddonData;
        //    var gameFolder = await StorageFolder.GetFolderFromPathAsync(game.AbsolutePath);
        //    var storageFolderQueryResult = gameFolder.CreateFolderQuery(CommonFolderQuery.DefaultQuery);
        //    var folders = await storageFolderQueryResult.GetFoldersAsync();
        //    var filteredFolders = new List<StorageFolder>();
        //    Debug.WriteLine("install game  all folders");
        //    foreach (var folder in folders)
        //    {

        //        //if (addonDatas.Contains(folder.Name))
        //        //{
        //        //    filteredFolders.Add(folder);
        //        //}

        //        var allMatchingFolders = addonDatas
        //            .Where(ad => ad.FolderName.Equals(folder.Name, StringComparison.OrdinalIgnoreCase))
        //            .ToList();
        //        if (allMatchingFolders.Count == 1)
        //        {
        //            filteredFolders.Add(folder);
        //        }
        //        else if (allMatchingFolders.Count > 1)
        //        {
        //            filteredFolders.Add(folder);
        //            Debug.WriteLine("RefreshGameFoder: Found " + allMatchingFolders.Count + " matches for " + folder.Name);
        //        }


        //    }
        //    Debug.WriteLine("install game  all filtered");

        //    //var tasks = filteredFolders
        //    //    .Select(Toc.FolderToTocFile);
        //    //    //.Where(task => task != null).ToList();
        //    //Debug.WriteLine("install game  created toc tasks");
        //    //var proccessed = await Task.WhenAll(tasks);
        //    //Debug.WriteLine("install game  toc tasks done");
        //    //var addons = proccessed.Where(tf => tf != null && !tf.IsKnownSubFolder)
        //    //    .Select(tf => new Core.Models.Addon(game, tf.StorageFolder.Name, tf.StorageFolder.Path)
        //    //    {
        //    //        Version = tf.Version,
        //    //        GameVersion = tf.GameVersion,
        //    //        Title = tf.Title
        //    //    }).ToList();

        //    //Debug.WriteLine("install game  done");


        //    var tasks = filteredFolders.Select(folder => Toc.FolderToTocFile(folder)
        //     .ContinueWith(pTask =>
        //                 {
        //                     if (pTask.IsCompleted)
        //                     {
        //                         var tocFile = pTask.Result;
        //                         if (tocFile != null && !tocFile.IsKnownSubFolder)
        //                         {
        //                             var a = new Core.Models.Addon(game, tocFile.StorageFolder.Name, tocFile.StorageFolder.Path)
        //                             {
        //                                 Version = tocFile.Version,
        //                                 GameVersion = tocFile.GameVersion,
        //                                 Title = tocFile.Title
        //                             };
        //                             Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher
        //                      .RunAsync(CoreDispatcherPriority.Normal, () =>
        //                      {
        //                          game.Addons.Add(a);
        //                      });

        //                         }
        //                     }

        //                 }));
        //    Debug.WriteLine("install game  created toc tasks");
        //    await Task.WhenAll(tasks);
        //    Debug.WriteLine("install game  toc tasks done");
        //    //).Where(a => a != null).Select(at=>at.Result).ToList().ForEach(a =>
        //    //{
        //    //    Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher
        //    //    .RunAsync(CoreDispatcherPriority.Normal, () =>
        //    //     {
        //    //         game.Addons.Add(a);
        //    //     });
        //    //});

        //    //.Where(tf => tf != null && !tf.IsKnownSubFolder)
        //    //.Select(tf => new Core.Models.Addon(game, tf.StorageFolder.Name, tf.StorageFolder.Path)
        //    //{
        //    //    Version = tf.Version,
        //    //    GameVersion = tf.GameVersion,
        //    //    Title = tf.Title
        //    //});


        //    return null;
        //}

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
            if (addon.IsIgnored
                || addon.Status.Equals(Core.Models.Addon.UPDATING)
                || addon.Status.Equals(Core.Models.Addon.DOWNLOADING_VERSIONS))
            {
                return;
            }
            addon.Progress = 0;
            addon.Status = Core.Models.Addon.DOWNLOADING_VERSIONS;



            if (string.IsNullOrEmpty(addon.ProjectUrl))
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
