using Addon.Core.Helpers;
using Addon.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Core;
using Windows.Web.Http;

namespace Addon.Logic
{
    internal static class Update
    {
        private static string GetDownLoadLink(Core.Models.Addon addon, Download download)
        {
            if (addon.ProjectUrl.Equals(Version.ELVUI))
            {
                return download.DownloadLink;
            }
            else
            {
                return addon.ProjectUrl.Remove(addon.ProjectUrl.IndexOf("/projects")) + download.DownloadLink;
            }
        }

        //internal static async Task<StorageFile> DLWithHttp(Core.Models.Addon addon, Download download)
        //{
        //    if (!NetworkInterface.GetIsNetworkAvailable())
        //    {
        //        return null;
        //    }
        //    string downloadLink = GetDownLoadLink(addon, download);

        //    //Debug.WriteLine(downloadLink);

        //    Uri source = new Uri(downloadLink);
        //    StorageFile destinationFile = await localFolder.CreateFileAsync(Util.RandomString(12) + ".zip", CreationCollisionOption.GenerateUniqueName);

        //    //using (var httpClient = new HttpClient())
        //    //{
        //    try
        //    {

        //        var htmlPage = await Http.NetHttpClient.GetByteArrayAsync(source);
        //        await FileIO.WriteBytesAsync(destinationFile, htmlPage);
        //        //Debug.WriteLine(htmlPage.Length);
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("[ERROR] DownloadFile. " + ex.Message);
        //    }
        //    //}
        //    return destinationFile;
        //}


        internal static async Task<StorageFile> DLWithHttpProgress(Core.Models.Addon addon, Download download)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }
            string downloadLink = GetDownLoadLink(addon, download);

            // Debug.WriteLine(downloadLink);

            Uri source = new Uri(downloadLink);

            StorageFile destinationFile = await localFolder.CreateFileAsync(Util.RandomString(12) + ".zip", CreationCollisionOption.GenerateUniqueName);


            try
            {

                if (addon.ProjectUrl.Equals(Version.ELVUI))
                {
                    var htmlPage = await Http.NetHttpClient.GetByteArrayAsync(source);
                    await FileIO.WriteBytesAsync(destinationFile, htmlPage);
                }
                else
                {
                    var result = Http.WebHttpClient.GetAsync(source);
                    var downloadProgessHandler = new DownloadProgressHandler() { Addon = addon };
                    result.Progress = downloadProgessHandler.DownloadProgressCallback;




                    //var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("filename.tmp", CreationCollisionOption.GenerateUniqueName);
                    using (var filestream = await destinationFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        var res = await result;
                        await res.Content.WriteToStreamAsync(filestream);
                        await filestream.FlushAsync();

                    }
                }
                // var htmlPage = await Http.NetHttpClient.GetByteArrayAsync(source);
                // await FileIO.WriteBytesAsync(destinationFile, htmlPage);
                //Debug.WriteLine(htmlPage.Length);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ERROR] DownloadFile. " + ex.Message);
            }

            return destinationFile;
        }

        //internal static BackgroundDownloader downloader = new BackgroundDownloader();
        internal static StorageFolder localFolder = ApplicationData.Current.TemporaryFolder;

        //internal static async Task<StorageFile> DownloadFile(Core.Models.Addon addon, Download download)
        //{
        //    addon.Message = "Downloading...";
        //    var temp = addon.ProjectUrl.Remove(addon.ProjectUrl.IndexOf("/projects"));
        //    var downloadLink = temp + download.DownloadLink;
        //    // Debug.WriteLine(downloadLink);
        //    try
        //    {
        //        Uri source = new Uri(downloadLink);

        //        StorageFile destinationFile = await localFolder.CreateFileAsync(Util.RandomString(12) + ".zip", CreationCollisionOption.GenerateUniqueName);

        //        //BackgroundDownloader downloader = new BackgroundDownloader();
        //        DownloadOperation downloadOperation = await Task.Run(() => { return downloader.CreateDownload(source, destinationFile); });
        //        //DownloadOperation downloadOperation = downloader.CreateDownload(source, destinationFile);

        //        var fileSize = download.FileSize.Replace("M", "").Replace("K", "").Replace("B", "").Replace(" ", "").Replace(".", ",").Trim();
        //        long totalBytes = -1000000;
        //        try
        //        {
        //            totalBytes = (long)(Double.Parse(fileSize) * 1000000);
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine("[ERROR] DownloadFile. Parsing FileSize " + ex.Message);
        //        }


        //        Progress<DownloadOperation> progressCallback = new Progress<DownloadOperation>(a => { ProgressCallback(a, addon, totalBytes); });




        //        // Debug.WriteLine("BEFORE-----------");
        //        await Task.Run(async () => { await downloadOperation.StartAsync().AsTask(progressCallback); });
        //        // t.Wait();
        //        // Debug.WriteLine("AFTER-----------");

        //        //  var aa = await downloadOperation.StartAsync();

        //        return destinationFile;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("[ERROR] DownloadFile. " + ex.Message);
        //    }

        //    return null;
        //}

        //private static void ProgressCallback(DownloadOperation obj, Core.Models.Addon addon, long totalBytes)
        //{
        //    // Debug.WriteLine("HIT HIT "+obj.Progress.BytesReceived+",,,, total: "+obj.Progress.TotalBytesToReceive);
        //    //MessageModel DLItem = listViewCollection.First(p => p.GUID == obj.Guid);
        //    if (obj.Progress.TotalBytesToReceive > 0)
        //    {
        //        double br = obj.Progress.BytesReceived;
        //        var result = br / obj.Progress.TotalBytesToReceive * 100;
        //        addon.Progress = (int)result;
        //        // Debug.WriteLine("progress: "+addon.Progress);
        //    }
        //    else if (totalBytes > 0)
        //    {
        //        double br = obj.Progress.BytesReceived;
        //        var result = br / totalBytes * 100;
        //        addon.Progress = (int)result;
        //        //Debug.WriteLine("progress: "+addon.Progress);
        //    }


        //}

        //internal static async Task<(string, string, List<string>)> UpdateAddon(Core.Models.Addon addon, Download download, StorageFile file)
        //{
        //    var extractFolderPath = localFolder.Path + @"\" + file.Name.Replace(".zip", "");
        //    var subFoldersToDelete = new List<string>();
        //    try
        //    {
        //        Debug.WriteLine("Start: " + DateTime.Now.ToString("mm:ss"));
        //        ZipFile.ExtractToDirectory(file.Path, extractFolderPath);

        //        using (ZipArchive archive = ZipFile.OpenRead(file.Path))
        //        {
        //            foreach (ZipArchiveEntry entry in archive.Entries)
        //            {
        //                if (string.IsNullOrEmpty(entry.Name))
        //                {
        //                    Debug.WriteLine(entry.FullName);
        //                }

        //            }
        //        }


        //        Debug.WriteLine("extracted done: " + DateTime.Now.ToString("mm:ss"));
        //        var extractFolder = await StorageFolder.GetFolderFromPathAsync(extractFolderPath);
        //        var folders = await extractFolder.GetFoldersAsync();
        //        var gameFolder = await StorageFolder.GetFolderFromPathAsync(addon.Game.AbsolutePath);
        //        Debug.WriteLine("getting folders done: " + DateTime.Now.ToString("mm:ss"));
        //        //foreach (var folder in folders)
        //        //{
        //        //    try
        //        //    {
        //        //        var delete = await gameFolder.GetFolderAsync(folder.Name);
        //        //        await delete.DeleteAsync(StorageDeleteOption.PermanentDelete);
        //        //    }
        //        //    catch (Exception e)
        //        //    {
        //        //        Debug.WriteLine("[ERROR] No folder found to delete. " + e.Message);
        //        //    }
        //        //}



        //        Debug.WriteLine("Deleting old folders done: " + DateTime.Now.ToString("mm:ss"));
        //        //await MoveContentFast(extractFolder, gameFolder);
        //        ////////var aaa = extractFolder.CreateItemQueryWithOptions(new QueryOptions() { FolderDepth = FolderDepth.Deep });
        //        ////////var itemsInFolder = await aaa.GetItemsAsync();
        //        ////////foreach (IStorageItem item in itemsInFolder)
        //        ////////{
        //        ////////    if (item.IsOfType(StorageItemTypes.Folder))
        //        ////////        Debug.WriteLine("Folder: " + item.Path);
        //        ////////    else
        //        ////////        Debug.WriteLine("File: " + item.Path);
        //        ////////}
        //        ///

        //        //var copyFolderTasks = folders.Select(folder => CopyFolderAsync(folder, gameFolder));
        //        //await Task.WhenAll(copyFolderTasks);

        //        //List<Task> tasks = new List<Task>();
        //        //foreach (var folder in folders)
        //        //{
        //        //    tasks.AddRange(CopyFolderAsync2(folder, gameFolder));
        //        //    //await CopyFolderAsync(folder, gameFolder);
        //        //}
        //        //var tasks=await Task.Run(()=>folders.SelectMany(folder => CopyFolderAsync2(folder, gameFolder)));


        //        ////var tasks = folders.SelectMany(folder => CopyFolderAsync2(folder, gameFolder));
        //        ////Debug.WriteLine("Generated tasks, " + tasks.Count() + " Count " + DateTime.Now.ToString("mm:ss"));
        //        ////await Task.WhenAll(tasks);

        //        //var copiedZip = await file.CopyAsync(gameFolder);
        //        await UnZipFileAsync(file, gameFolder);


        //        Debug.WriteLine("copy done: " + DateTime.Now.ToString("mm:ss"));
        //        var foldersAsList = new List<StorageFolder>(folders);
        //        subFoldersToDelete = foldersAsList.Select(f => f.Name).Where(name => !name.Equals(addon.FolderName)).ToList();
        //        //await AddSubFolders(addon, subFoldersToDelete);

        //    }
        //    catch (Exception e)
        //    {
        //        Debug.WriteLine("[ERROR] UpdateAddon. " + e.Message + ", " + e.StackTrace);
        //    }
        //    return (file.Path, extractFolderPath, subFoldersToDelete);
        //}



        internal static async Task<(string, List<string>)> UpdateAddon2(Core.Models.Addon addon, Download download, StorageFile file)
        {

            var subFoldersToDelete = new HashSet<string>();
            try
            {
                Debug.WriteLine("Start: " + addon.FolderName + " " + DateTime.Now.ToString("mm:ss"));
                int entries = 0;
                var addonFoldersInGameToDelete = new HashSet<string>();
                using (ZipArchive archive = ZipFile.OpenRead(file.Path))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName.Contains("/"))
                        {
                            var folderName = entry.FullName.Split("/").FirstOrDefault();
                            if (folderName != null && !folderName.Equals(addon.FolderName))
                            {
                                subFoldersToDelete.Add(folderName);

                            }
                            if (folderName != null)
                            {
                                addonFoldersInGameToDelete.Add(folderName);

                            }
                        }

                        entries++;




                    }
                }
                var gameFolder = await StorageFolder.GetFolderFromPathAsync(addon.Game.AbsolutePath);
                if (addon.ProjectUrl.Equals(Version.ELVUI))
                {

                    foreach (var folder in addonFoldersInGameToDelete)
                    {
                        try
                        {
                            var delete = await gameFolder.GetFolderAsync(folder);
                            await delete.DeleteAsync(StorageDeleteOption.PermanentDelete);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("[ERROR] No folder found to delete. " + e.Message);
                        }
                    }
                }

                //var gameFolder = await StorageFolder.GetFolderFromPathAsync(addon.Game.AbsolutePath);
                var zipHelper = new ZipHelper() { Addon = addon, Entries = Math.Max(entries, 1) };
                zipHelper.PropertyChanged += UnzipProgress;
                await zipHelper.UnZipFileAsync(file, gameFolder);
                Debug.WriteLine("Copy done: " + addon.FolderName + " " + DateTime.Now.ToString("mm:ss"));
            }
            catch (Exception e)
            {
                Debug.WriteLine("[ERROR] UpdateAddon. " + e.Message + ", " + e.StackTrace);
            }
            return (file.Path, subFoldersToDelete.ToList());
        }

        private static void UnzipProgress(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var zipHelper = sender as ZipHelper;
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.
                RunAsync(CoreDispatcherPriority.Normal, () =>
                {

                    zipHelper.Addon.Progress = zipHelper.Progress;
                });

        }

        //internal static async Task DeleteFilesFrom(StorageFolder folder)
        //{
        //    var aaa = folder.CreateItemQueryWithOptions(new QueryOptions() { FolderDepth = FolderDepth.Deep });
        //    var itemsInFolder = await aaa.GetItemsAsync();

        //    var tasks = itemsInFolder.OfType<StorageFile>().Select(file => file.DeleteAsync(StorageDeleteOption.PermanentDelete).AsTask());

        //    await Task.WhenAll(tasks);
        //}
        // private static Action<StorageFile, StorageFolder> FileMove =  (file, destination) => { file.MoveAsync(destination, file.Name, NameCollisionOption.ReplaceExisting).AsTask().RunSynchronously(); };

        internal static async Task CopyFolderAsync(StorageFolder source, StorageFolder destinationContainer)
        {
            Debug.WriteLine("Folder: " + source.Name);
            StorageFolder destinationFolder = null;
            destinationFolder = await destinationContainer.CreateFolderAsync(source.Name, CreationCollisionOption.OpenIfExists);
            var files = await source.GetFilesAsync();
            //var fileTasks = files.Select(file => file.CopyAsync(destinationFolder).AsTask());
            //await Task.WhenAll(fileTasks);

            foreach (var file in files)
            {
                try
                {
                    await file.MoveAsync(destinationFolder, file.Name, NameCollisionOption.ReplaceExisting);

                }
                catch (Exception e)
                {
                    Debug.WriteLine("[ERROR] Copy file. " + e.Message);
                }

            }
            var folders = await source.GetFoldersAsync();
            //var folderTasks = folders.Select(folder => CopyFolderAsync(folder, destinationFolder));
            //await Task.WhenAll(folderTasks);
            // Parallel.ForEach(folders,folder=>FolderCopy(folder,destinationFolder));
            foreach (var folder in folders)
            {
                try
                {
                    await CopyFolderAsync(folder, destinationFolder);

                }
                catch (Exception e)
                {
                    Debug.WriteLine("[ERROR] Copy folder. " + e.Message);
                }
            }
            // Parallel.ForEach(files, file => FileMove(file, destinationFolder));
        }


        internal static List<Task> CopyFolderAsync2(StorageFolder source, StorageFolder destinationContainer)
        {

            var (destinationFolder, items) = Task.Run(async () =>
             {
                 StorageFolder destination = await destinationContainer.CreateFolderAsync(source.Name, CreationCollisionOption.OpenIfExists);
                 var itemz = await source.GetItemsAsync();
                 return (destination, itemz);
             }).Result;
            var tasks = items.OfType<StorageFile>().Select(file => file.MoveAsync(destinationFolder, file.Name, NameCollisionOption.ReplaceExisting).AsTask()).ToList();

            var folderTasks = items.OfType<StorageFolder>().SelectMany(folder => CopyFolderAsync2(folder, destinationFolder));
            tasks.AddRange(folderTasks);

            return tasks;

            //var items = Task.Run(async () => await source.GetItemsAsync()).Result;
            //            var files = Task.Run(async ()=> await source.GetFilesAsync()).Result;
            //var fileTasks = files.Select(file => file.MoveAsync(destinationFolder, file.Name, NameCollisionOption.ReplaceExisting).AsTask()).ToList();
            //await Task.WhenAll(fileTasks);

            //foreach (var file in files)
            //{
            //    try
            //    {
            //      await  file.MoveAsync(destinationFolder, file.Name, NameCollisionOption.ReplaceExisting);

            //    }
            //    catch (Exception e)
            //    {
            //        Debug.WriteLine("[ERROR] Copy file. " + e.Message);
            //    }

            //}
            //var folders = Task.Run(async () => await source.GetFoldersAsync()).Result;
            //var folderTasks = folders.SelectMany(folder => CopyFolderAsync2(folder, destinationFolder));
            //fileTasks.AddRange(folderTasks);
            //return fileTasks;
            //await Task.WhenAll(folderTasks);
            // Parallel.ForEach(folders,folder=>FolderCopy(folder,destinationFolder));
            //foreach (var folder in folders)
            //{
            //    try
            //    {
            //      await  CopyFolderAsync(folder, destinationFolder);

            //    }
            //    catch (Exception e)
            //    {
            //        Debug.WriteLine("[ERROR] Copy folder. " + e.Message);
            //    }
            //}
            // Parallel.ForEach(files, file => FileMove(file, destinationFolder));
        }

        internal async static Task Cleanup(string filePath, string folderPath)
        {
            try
            {
                var zipFile = await StorageFile.GetFileFromPathAsync(filePath);
                await zipFile.DeleteAsync();

            }
            catch (Exception)
            {

            }
            try
            {
                var extractFolder = await StorageFolder.GetFolderFromPathAsync(folderPath);
                await extractFolder.DeleteAsync();

            }
            catch (Exception)
            {

            }



        }



        internal async static Task Cleanup2(string filePath)
        {
            try
            {
                var zipFile = await StorageFile.GetFileFromPathAsync(filePath);
                await zipFile.DeleteAsync();

            }
            catch (Exception)
            {

            }
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
        //static async Task MoveContentFast(IStorageFolder source, IStorageFolder destination)
        //{
        //    await Task.Run(() =>
        //    {
        //        MoveContextImpl(new DirectoryInfo(source.Path), destination);
        //    });
        //}

        //private static void MoveContextImpl(DirectoryInfo sourceFolderInfo, IStorageFolder destination)
        //{
        //    var tasks = new List<Task>();

        //    var destinationAccess = destination as IStorageFolderHandleAccess;

        //    foreach (var item in sourceFolderInfo.EnumerateFileSystemInfos())
        //    {
        //        if ((item.Attributes & System.IO.FileAttributes.Directory) != 0)
        //        {
        //            tasks.Add(destination.CreateFolderAsync(item.Name, CreationCollisionOption.ReplaceExisting).AsTask().ContinueWith((destinationSubFolder) =>
        //            {
        //                MoveContextImpl((DirectoryInfo)item, destinationSubFolder.Result);
        //            }));
        //        }
        //        else
        //        {
        //            if (destinationAccess == null)
        //            {
        //                // Slower, pre 14393 OS build path
        //                tasks.Add(WindowsRuntimeStorageExtensions.OpenStreamForWriteAsync(destination, item.Name, CreationCollisionOption.ReplaceExisting).ContinueWith((openTask) =>
        //                {
        //                    using (var stream = openTask.Result)
        //                    {
        //                        var sourceBytes = File.ReadAllBytes(item.FullName);
        //                        stream.Write(sourceBytes, 0, sourceBytes.Length);
        //                    }

        //                    File.Delete(item.FullName);
        //                }));
        //            }
        //            else
        //            {
        //                int hr = destinationAccess.Create(item.Name, HANDLE_CREATION_OPTIONS.CREATE_ALWAYS, HANDLE_ACCESS_OPTIONS.WRITE, HANDLE_SHARING_OPTIONS.SHARE_NONE, HANDLE_OPTIONS.NONE, IntPtr.Zero, out SafeFileHandle file);
        //                if (hr < 0)
        //                    Marshal.ThrowExceptionForHR(hr);

        //                using (file)
        //                {
        //                    using (var stream = new FileStream(file, FileAccess.Write))
        //                    {
        //                        var sourceBytes = File.ReadAllBytes(item.FullName);
        //                        stream.Write(sourceBytes, 0, sourceBytes.Length);
        //                    }
        //                }

        //                File.Delete(item.FullName);
        //            }
        //        }
        //    }
        //    //await Task.WhenAll(tasks.ToArray());
        //    Task.WaitAll(tasks.ToArray());
        //}

        //[ComImport]
        //[Guid("DF19938F-5462-48A0-BE65-D2A3271A08D6")]
        //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        //internal interface IStorageFolderHandleAccess
        //{
        //    [PreserveSig]
        //    int Create(
        //        [MarshalAs(UnmanagedType.LPWStr)] string fileName,
        //        HANDLE_CREATION_OPTIONS creationOptions,
        //        HANDLE_ACCESS_OPTIONS accessOptions,
        //        HANDLE_SHARING_OPTIONS sharingOptions,
        //        HANDLE_OPTIONS options,
        //        IntPtr oplockBreakingHandler,
        //        out SafeFileHandle interopHandle); // using Microsoft.Win32.SafeHandles
        //}

        //internal enum HANDLE_CREATION_OPTIONS : uint
        //{
        //    CREATE_NEW = 0x1,
        //    CREATE_ALWAYS = 0x2,
        //    OPEN_EXISTING = 0x3,
        //    OPEN_ALWAYS = 0x4,
        //    TRUNCATE_EXISTING = 0x5,
        //}

        //[Flags]
        //internal enum HANDLE_ACCESS_OPTIONS : uint
        //{
        //    NONE = 0,
        //    READ_ATTRIBUTES = 0x80,
        //    // 0x120089
        //    READ = SYNCHRONIZE | READ_CONTROL | READ_ATTRIBUTES | FILE_READ_EA | FILE_READ_DATA,
        //    // 0x120116
        //    WRITE = SYNCHRONIZE | READ_CONTROL | FILE_WRITE_ATTRIBUTES | FILE_WRITE_EA | FILE_APPEND_DATA | FILE_WRITE_DATA,
        //    DELETE = 0x10000,

        //    READ_CONTROL = 0x00020000,
        //    SYNCHRONIZE = 0x00100000,
        //    FILE_READ_DATA = 0x00000001,
        //    FILE_WRITE_DATA = 0x00000002,
        //    FILE_APPEND_DATA = 0x00000004,
        //    FILE_READ_EA = 0x00000008,
        //    FILE_WRITE_EA = 0x00000010,
        //    FILE_EXECUTE = 0x00000020,
        //    FILE_WRITE_ATTRIBUTES = 0x00000100,
        //}

        //[Flags]
        //internal enum HANDLE_SHARING_OPTIONS : uint
        //{
        //    SHARE_NONE = 0,
        //    SHARE_READ = 0x1,
        //    SHARE_WRITE = 0x2,
        //    SHARE_DELETE = 0x4
        //}

        //[Flags]
        //internal enum HANDLE_OPTIONS : uint
        //{
        //    NONE = 0,
        //    OPEN_REQUIRING_OPLOCK = 0x40000,
        //    DELETE_ON_CLOSE = 0x4000000,
        //    SEQUENTIAL_SCAN = 0x8000000,
        //    RANDOM_ACCESS = 0x10000000,
        //    NO_BUFFERING = 0x20000000,
        //    OVERLAPPED = 0x40000000,
        //    WRITE_THROUGH = 0x80000000
        //}


        //////private static async void extract(StorageFolder SF, string zipFileName)
        //////{
        //////    // StorageFolder SF = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("SampleBook");
        //////    Stream stream = await SF.OpenStreamForReadAsync(zipFileName);

        //////    using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
        //////    {
        //////        try
        //////        {
        //////            foreach (ZipArchiveEntry entry in archive.Entries)
        //////            {
        //////                StorageFile newFile = await CreateFile(SF, entry.FullName);
        //////                Stream newFileStream = await newFile.OpenStreamForWriteAsync();
        //////                Stream fileData = entry.Open();
        //////                byte[] data = new byte[entry.Length];
        //////                await fileData.ReadAsync(data, 0, data.Length);
        //////                newFileStream.Write(data, 0, data.Length);
        //////                await newFileStream.FlushAsync();
        //////            }
        //////        }
        //////        catch (Exception)
        //////        {
        //////        }

        //////    }
        //////}

        //////private static async Task<StorageFile> CreateFile(StorageFolder dataFolder, string fileName)
        //////{
        //////    return await dataFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting).AsTask();
        //////}

        private class DownloadProgressHandler
        {
            public Core.Models.Addon Addon { get; set; }

            public void DownloadProgressCallback(IAsyncOperationWithProgress<HttpResponseMessage, HttpProgress> asyncInfo, HttpProgress progressInfo)
            {
                if (progressInfo.TotalBytesToReceive != null && progressInfo.TotalBytesToReceive > 0)
                {
                    var progress = (int)(((double)progressInfo.BytesReceived / (double)progressInfo.TotalBytesToReceive) * 100d);
                    //Debug.WriteLine("received " + (int)progress);
                    Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.
                        RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            Addon.Progress = progress;
                        });
                }


            }
        }


    }





}
