using Addon.Core.Helpers;
using Addon.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace Addon.Logic
{


    internal static class Update
    {


        internal static StorageFolder localFolder = ApplicationData.Current.TemporaryFolder;

        internal static async Task<StorageFile> DownloadFile(Core.Models.Addon addon, Download download)
        {
            addon.Message = "Downloading...";
            var temp = addon.ProjectUrl.Remove(addon.ProjectUrl.IndexOf("/projects"));
            var downloadLink = temp + download.DownloadLink;
            Debug.WriteLine(downloadLink);
            try
            {
                Uri source = new Uri(downloadLink);

                StorageFile destinationFile = await localFolder.CreateFileAsync(Util.RandomString(12) + ".zip", CreationCollisionOption.GenerateUniqueName);

                BackgroundDownloader downloader = new BackgroundDownloader();
                DownloadOperation downloadOperation = downloader.CreateDownload(source, destinationFile);

                var fileSize = download.FileSize.Replace("M", "").Replace("K", "").Replace("B", "").Replace(" ", "").Replace(".", ",").Trim();
                long totalBytes = -1000000;
                try
                {
                    totalBytes = (long)(Double.Parse(fileSize) * 1000000);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("[ERROR] DownloadFile. Parsing FileSize " + ex.Message);
                }


                Progress<DownloadOperation> progressCallback = new Progress<DownloadOperation>(a => { ProgressCallback(a, addon, totalBytes); });


                await downloadOperation.StartAsync().AsTask(progressCallback);




                //  var aa = await downloadOperation.StartAsync();

                return destinationFile;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ERROR] DownloadFile. " + ex.Message);
            }

            return null;
        }

        private static void ProgressCallback(DownloadOperation obj, Core.Models.Addon addon, long totalBytes)
        {
            // Debug.WriteLine("HIT HIT "+obj.Progress.BytesReceived+",,,, total: "+obj.Progress.TotalBytesToReceive);
            //MessageModel DLItem = listViewCollection.First(p => p.GUID == obj.Guid);
            if (obj.Progress.TotalBytesToReceive > 0)
            {
                double br = obj.Progress.BytesReceived;
                var result = br / obj.Progress.TotalBytesToReceive * 100;
                addon.Progress = (int)result;
                // Debug.WriteLine("progress: "+addon.Progress);
            }
            else if (totalBytes > 0)
            {
                double br = obj.Progress.BytesReceived;
                var result = br / totalBytes * 100;
                addon.Progress = (int)result;
                //Debug.WriteLine("progress: "+addon.Progress);
            }


        }

        internal static async Task<Tuple<string, string>> UpdateAddon(Core.Models.Addon addon, Download download, StorageFile file)
        {
            var extractFolderPath = localFolder.Path + @"\" + file.Name.Replace(".zip", "");
            try
            {
                addon.Message = "Extracting...";
                ZipFile.ExtractToDirectory(file.Path, extractFolderPath);
                var extractFolder = await StorageFolder.GetFolderFromPathAsync(extractFolderPath);
                var folders = await extractFolder.GetFoldersAsync();
                var gameFolder = await StorageFolder.GetFolderFromPathAsync(addon.Game.AbsolutePath);
                addon.Message = "Removing old...";
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
                addon.Message = "Copy new...";
                foreach (var folder in folders)
                {
                    await CopyFolderAsync(folder, gameFolder);
                }
                Debug.WriteLine("copy ok");
                addon.Message = "Clean up...";
                var foldersAsList = new List<StorageFolder>(folders);
                var subFoldersToDelete = foldersAsList.Select(f => f.Name).Where(name => !name.Equals(addon.FolderName)).ToList();
                await AddSubFolders(addon, subFoldersToDelete);
                addon.CurrentDownload = download;
            }
            catch (Exception e)
            {
                Debug.WriteLine("[ERROR] UpdateAddon. " + e.Message + ", " + e.StackTrace);
            }
            return new Tuple<string, string>(file.Path, extractFolderPath);
        }


        internal static async Task CopyFolderAsync(StorageFolder source, StorageFolder destinationContainer, string desiredName = null)
        {
            StorageFolder destinationFolder = null;
            destinationFolder = await destinationContainer.CreateFolderAsync(
                desiredName ?? source.Name, CreationCollisionOption.ReplaceExisting);

            foreach (var file in await source.GetFilesAsync())
            {
                await file.CopyAsync(destinationFolder, file.Name, NameCollisionOption.ReplaceExisting);
            }
            foreach (var folder in await source.GetFoldersAsync())
            {
                await CopyFolderAsync(folder, destinationFolder);
            }
        }

        internal async static Task Cleanup(Tuple<string, string> trash)
        {
            var zipFile = await StorageFile.GetFileFromPathAsync(trash.Item1);
            await zipFile.DeleteAsync();

            var extractFolder = await StorageFolder.GetFolderFromPathAsync(trash.Item2);
            await extractFolder.DeleteAsync();



        }

        internal async static Task AddSubFolders(Core.Models.Addon addon, List<string> subFoldersToDelete)
        {
            var addons = addon.Game.Addons;
            foreach (var name in subFoldersToDelete)
            {
                var subAddon = addons.FirstOrDefault(a => a.FolderName.Equals(name));
                if (subAddon != null)
                {
                    addons.Remove(subAddon);
                }
            }
            if (subFoldersToDelete.Count > 0)
            {
                Singleton<Session>.Instance.KnownSubFolders.UnionWith(subFoldersToDelete);
            }
            await Task.CompletedTask;
        }

        private static void ProgressOnProgressChanged(object sender, string s)
        {
            // your logic here
        }
    }
}
