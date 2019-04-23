using Addon.Core.Helpers;
using Addon.Core.Models;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace Addon.Logic
{
    internal static class Update
    {
        internal static async Task<StorageFile> DLWithHttp(Core.Models.Addon addon, Download download)
        {
            var temp = addon.ProjectUrl.Remove(addon.ProjectUrl.IndexOf("/projects"));
            var downloadLink = temp + download.DownloadLink;
            //Debug.WriteLine(downloadLink);

            Uri source = new Uri(downloadLink);
            StorageFile destinationFile = await localFolder.CreateFileAsync(Util.RandomString(12) + ".zip", CreationCollisionOption.GenerateUniqueName);

            using (var httpClient = new HttpClient())
            {
                try
                {
                    var htmlPage = await httpClient.GetByteArrayAsync(source);
                    await FileIO.WriteBytesAsync(destinationFile, htmlPage);
                    //Debug.WriteLine(htmlPage.Length);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("[ERROR] DownloadFile. " + ex.Message);
                }
            }
            return destinationFile;
        }

        internal static BackgroundDownloader downloader = new BackgroundDownloader();
        internal static StorageFolder localFolder = ApplicationData.Current.TemporaryFolder;

        internal static async Task<StorageFile> DownloadFile(Core.Models.Addon addon, Download download)
        {
            addon.Message = "Downloading...";
            var temp = addon.ProjectUrl.Remove(addon.ProjectUrl.IndexOf("/projects"));
            var downloadLink = temp + download.DownloadLink;
           // Debug.WriteLine(downloadLink);
            try
            {
                Uri source = new Uri(downloadLink);

                StorageFile destinationFile = await localFolder.CreateFileAsync(Util.RandomString(12) + ".zip", CreationCollisionOption.GenerateUniqueName);

                //BackgroundDownloader downloader = new BackgroundDownloader();
                DownloadOperation downloadOperation = await Task.Run(() => { return downloader.CreateDownload(source, destinationFile); });
                //DownloadOperation downloadOperation = downloader.CreateDownload(source, destinationFile);

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




                // Debug.WriteLine("BEFORE-----------");
                await Task.Run(async () => { await downloadOperation.StartAsync().AsTask(progressCallback); });
                // t.Wait();
                // Debug.WriteLine("AFTER-----------");

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
                ////////////////////addon.Message = "Extracting...";
                ZipFile.ExtractToDirectory(file.Path, extractFolderPath);
                var extractFolder = await StorageFolder.GetFolderFromPathAsync(extractFolderPath);
                var folders = await extractFolder.GetFoldersAsync();
                var gameFolder = await StorageFolder.GetFolderFromPathAsync(addon.Game.AbsolutePath);
                ////////////////// addon.Message = "Removing old...";
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
                ////////////////////////////////   addon.Message = "Copy new...";
                //foreach (var folder in folders)
                //{
                await MoveContentFast(extractFolder, gameFolder);
                //await CopyFolderAsync(folder, gameFolder);
                //}
                //Debug.WriteLine("copy ok");
                /////////////////////////////////   addon.Message = "Clean up...";
                var foldersAsList = new List<StorageFolder>(folders);
                var subFoldersToDelete = foldersAsList.Select(f => f.Name).Where(name => !name.Equals(addon.FolderName)).ToList();
                await AddSubFolders(addon, subFoldersToDelete);
                //addon.CurrentDownload = download;
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
            destinationFolder = await destinationContainer.CreateFolderAsync(desiredName ?? source.Name, CreationCollisionOption.ReplaceExisting);

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

        //
        // https://stackoverflow.com/questions/54942686/fastest-way-to-move-folder-to-another-place-in-uwp 
        // 
        static async Task MoveContentFast(IStorageFolder source, IStorageFolder destination)
        {
            await Task.Run(() =>
            {
                MoveContextImpl(new DirectoryInfo(source.Path), destination);
            });
        }

        private static void MoveContextImpl(DirectoryInfo sourceFolderInfo, IStorageFolder destination)
        {
            var tasks = new List<Task>();

            var destinationAccess = destination as IStorageFolderHandleAccess;

            foreach (var item in sourceFolderInfo.EnumerateFileSystemInfos())
            {
                if ((item.Attributes & System.IO.FileAttributes.Directory) != 0)
                {
                    tasks.Add(destination.CreateFolderAsync(item.Name, CreationCollisionOption.ReplaceExisting).AsTask().ContinueWith((destinationSubFolder) =>
                    {
                        MoveContextImpl((DirectoryInfo)item, destinationSubFolder.Result);
                    }));
                }
                else
                {
                    if (destinationAccess == null)
                    {
                        // Slower, pre 14393 OS build path
                        tasks.Add(WindowsRuntimeStorageExtensions.OpenStreamForWriteAsync(destination, item.Name, CreationCollisionOption.ReplaceExisting).ContinueWith((openTask) =>
                        {
                            using (var stream = openTask.Result)
                            {
                                var sourceBytes = File.ReadAllBytes(item.FullName);
                                stream.Write(sourceBytes, 0, sourceBytes.Length);
                            }

                            File.Delete(item.FullName);
                        }));
                    }
                    else
                    {
                        int hr = destinationAccess.Create(item.Name, HANDLE_CREATION_OPTIONS.CREATE_ALWAYS, HANDLE_ACCESS_OPTIONS.WRITE, HANDLE_SHARING_OPTIONS.SHARE_NONE, HANDLE_OPTIONS.NONE, IntPtr.Zero, out SafeFileHandle file);
                        if (hr < 0)
                            Marshal.ThrowExceptionForHR(hr);

                        using (file)
                        {
                            using (var stream = new FileStream(file, FileAccess.Write))
                            {
                                var sourceBytes = File.ReadAllBytes(item.FullName);
                                stream.Write(sourceBytes, 0, sourceBytes.Length);
                            }
                        }

                        File.Delete(item.FullName);
                    }
                }
            }
            //await Task.WhenAll(tasks.ToArray());
            Task.WaitAll(tasks.ToArray());
        }

        [ComImport]
        [Guid("DF19938F-5462-48A0-BE65-D2A3271A08D6")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IStorageFolderHandleAccess
        {
            [PreserveSig]
            int Create(
                [MarshalAs(UnmanagedType.LPWStr)] string fileName,
                HANDLE_CREATION_OPTIONS creationOptions,
                HANDLE_ACCESS_OPTIONS accessOptions,
                HANDLE_SHARING_OPTIONS sharingOptions,
                HANDLE_OPTIONS options,
                IntPtr oplockBreakingHandler,
                out SafeFileHandle interopHandle); // using Microsoft.Win32.SafeHandles
        }

        internal enum HANDLE_CREATION_OPTIONS : uint
        {
            CREATE_NEW = 0x1,
            CREATE_ALWAYS = 0x2,
            OPEN_EXISTING = 0x3,
            OPEN_ALWAYS = 0x4,
            TRUNCATE_EXISTING = 0x5,
        }

        [Flags]
        internal enum HANDLE_ACCESS_OPTIONS : uint
        {
            NONE = 0,
            READ_ATTRIBUTES = 0x80,
            // 0x120089
            READ = SYNCHRONIZE | READ_CONTROL | READ_ATTRIBUTES | FILE_READ_EA | FILE_READ_DATA,
            // 0x120116
            WRITE = SYNCHRONIZE | READ_CONTROL | FILE_WRITE_ATTRIBUTES | FILE_WRITE_EA | FILE_APPEND_DATA | FILE_WRITE_DATA,
            DELETE = 0x10000,

            READ_CONTROL = 0x00020000,
            SYNCHRONIZE = 0x00100000,
            FILE_READ_DATA = 0x00000001,
            FILE_WRITE_DATA = 0x00000002,
            FILE_APPEND_DATA = 0x00000004,
            FILE_READ_EA = 0x00000008,
            FILE_WRITE_EA = 0x00000010,
            FILE_EXECUTE = 0x00000020,
            FILE_WRITE_ATTRIBUTES = 0x00000100,
        }

        [Flags]
        internal enum HANDLE_SHARING_OPTIONS : uint
        {
            SHARE_NONE = 0,
            SHARE_READ = 0x1,
            SHARE_WRITE = 0x2,
            SHARE_DELETE = 0x4
        }

        [Flags]
        internal enum HANDLE_OPTIONS : uint
        {
            NONE = 0,
            OPEN_REQUIRING_OPLOCK = 0x40000,
            DELETE_ON_CLOSE = 0x4000000,
            SEQUENTIAL_SCAN = 0x8000000,
            RANDOM_ACCESS = 0x10000000,
            NO_BUFFERING = 0x20000000,
            OVERLAPPED = 0x40000000,
            WRITE_THROUGH = 0x80000000
        }


    }
}
