using Addon.Core.Helpers;
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
    public static class Update
    {
        private static Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalCacheFolder;

        public static async Task<StorageFile> DownloadFile(Core.Models.Addon addon)
        {
            var download = addon.SuggestedDownload;
            var temp = addon.ProjectUrl.Remove(addon.ProjectUrl.IndexOf("/projects"));
            var downloadLink = temp + download.DownloadLink;
            Debug.WriteLine(downloadLink);
            try
            {
                Uri source = new Uri(downloadLink);

                StorageFile destinationFile = await localFolder.CreateFileAsync(Util.RandomString(12) + ".zip", CreationCollisionOption.GenerateUniqueName);

                BackgroundDownloader downloader = new BackgroundDownloader();
                DownloadOperation downloadOperation = downloader.CreateDownload(source, destinationFile);

                var aa = await downloadOperation.StartAsync();
                return destinationFile;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ERROR] DownloadFile. " + ex.Message);
            }

            return null;
        }

        public static async Task UpdateAddon(Core.Models.Addon addon, StorageFile file)
        {
            try
            {
                var extractFolderPath = localFolder.Path + @"\" + file.Name.Replace(".zip", "");
                ZipFile.ExtractToDirectory(file.Path, extractFolderPath);
                var extractFolder = await StorageFolder.GetFolderFromPathAsync(extractFolderPath);
                var folders = await extractFolder.GetFoldersAsync();
                var gameFolder = await StorageFolder.GetFolderFromPathAsync(addon.Game.AbsolutePath);
                foreach (var folder in folders)
                {
                    Debug.WriteLine("zipfolder: " + folder.Name);
                    try
                    {
                        var delete = await gameFolder.GetFolderAsync(folder.Name);
                        //Debug.WriteLine("deletfolder: " + delete.Name);
                        await delete.DeleteAsync();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("[ERROR] No folder found to delete. " + e.Message);
                    }

                }
                //Debug.WriteLine(gameFolder.Path);

                foreach (var folder in folders)
                {
                    await CopyFolderAsync(folder,gameFolder);
                }
                 Debug.WriteLine("copy ok");
               
                var foldersAsList = new List<StorageFolder>(folders);
                var subFoldersToDelete = foldersAsList.Select(f => f.Name).Where(name => !name.Equals(addon.FolderName)).ToList();
                var addons = addon.Game.Addons;
                foreach (var name in subFoldersToDelete)
                {
                    var subAddon = addons.FirstOrDefault(a => a.FolderName.Equals(name));
                    if (subAddon != null)
                    {
                        addons.Remove(subAddon);
                    }
                }
                addon.CurrentDownload = addon.SuggestedDownload;

                await file.DeleteAsync();
                await extractFolder.DeleteAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine("[ERROR] UpdateAddon. " + e.Message+", "+e.StackTrace);
            }

        }


        public static async Task CopyFolderAsync(StorageFolder source, StorageFolder destinationContainer, string desiredName = null)
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

    }
}
