using AddonManager.Core.Helpers;
using AddonManager.Core.Models;
using AddonManager.ViewModels;
using AddonToolkit.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Core;
using Windows.Web.Http;

namespace AddonManager.Logic
{
    internal static class Update
    {
        internal static StorageFolder localFolder = ApplicationData.Current.TemporaryFolder;

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

        internal static async Task<StorageFile> DLWithHttpProgress(Addon addon, Download download, IProgressable progressable = null)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }
            string downloadLink = GetDownLoadLink(addon, download);

            var source = new Uri(downloadLink);

            StorageFile destinationFile = await localFolder.CreateFileAsync(Util.RandomString(12) + ".zip", CreationCollisionOption.GenerateUniqueName);

            return await DownloadFile(destinationFile, source, addon, progressable);
        }

        private static async Task<StorageFile> DownloadFile(StorageFile destinationFile, Uri source, Core.Models.Addon addon, IProgressable progressable)
        {
            try
            {
                var result = Http.WebHttpClient.GetAsync(source);
                var downloadProgessHandler = new DownloadProgressHandler() { Progressable = progressable != null ? progressable : addon };
                result.Progress = downloadProgessHandler.DownloadProgressCallback;

                using (var filestream = await destinationFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var res = await result;
                    await res.Content.WriteToStreamAsync(filestream);
                    await filestream.FlushAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ERROR] DownloadFile. " + ex.Message + " " + ex.StackTrace);
            }
            return destinationFile;
        }

        internal static async Task<(string, List<string>)> UpdateAddonOld(Core.Models.Addon addon, Download download, StorageFile file, IProgressable progressable = null)
        {
            var extractFolderPath = localFolder.Path + @"\" + file.Name.Replace(".zip", "");
            var subFoldersToDelete = new List<string>();
            try
            {
                ZipFile.ExtractToDirectory(file.Path, extractFolderPath);
                // int directoryCount = Directory.GetDirectories(extractFolderPath, "*", SearchOption.AllDirectories).Length;
                float fileCount = Directory.GetFiles(extractFolderPath, "*", SearchOption.AllDirectories).Length;

                var extractFolder = await StorageFolder.GetFolderFromPathAsync(extractFolderPath);
                var folders = await extractFolder.GetFoldersAsync();
                var gameFolder = await StorageFolder.GetFolderFromPathAsync(addon.Game.AbsolutePath);

                //var tasks = folders.SelectMany(folder => CopyFolderAsync2(folder, gameFolder));
                //await Task.WhenAll(tasks);
                var foldersAsList = new List<StorageFolder>(folders);
                subFoldersToDelete = foldersAsList.Select(f => f.Name)
                    .Where(name => !name.Equals(addon.FolderName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                if (Singleton<SettingsViewModel>.Instance.IsDeleteOldFilesBeforeUpdate ?? false)
                {
                    var subFolders = new List<string>(subFoldersToDelete)
                    {
                        addon.FolderName
                    };
                    await RemoveFolders(addon.Game.AbsolutePath, subFolders);
                }

                var counter = new Counter(progressable != null ? progressable : addon, Math.Max(fileCount, 1));
                foreach (var folder in folders)
                {
                    await CopyFolderAsync(folder, gameFolder, counter);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[ERROR] UpdateAddon. " + e.Message + ", " + e.StackTrace);
            }
            return (extractFolderPath, subFoldersToDelete);
        }

        internal static async Task<List<string>> UpdateAddon2(Addon addon, StorageFile file, IProgressable progressable = null)
        {
            var subFoldersToDelete = new HashSet<string>();
            try
            {
                Debug.WriteLine("Start: " + addon.FolderName + " " + DateTime.Now.ToString("mm:ss"));
                int entries = 0;

                using (ZipArchive archive = ZipFile.OpenRead(file.Path))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName.Contains("/"))
                        {
                            var folderName = entry.FullName.Split("/").FirstOrDefault();
                            if (folderName != null && !folderName.Equals(addon.FolderName, StringComparison.OrdinalIgnoreCase))
                            {
                                subFoldersToDelete.Add(folderName);
                            }
                        }
                        entries++;
                    }
                }
                if (Singleton<SettingsViewModel>.Instance.IsDeleteOldFilesBeforeUpdate ?? false)
                {
                    var folders = new List<string>(subFoldersToDelete)
                    {
                        addon.FolderName
                    };
                    await RemoveFolders(addon.Game.AbsolutePath, folders);
                }

                var gameFolder = await StorageFolder.GetFolderFromPathAsync(addon.Game.AbsolutePath);

                var zipHelper = new ZipHelper() { Progressable = progressable != null ? progressable : addon, Entries = Math.Max(entries, 1) };
                zipHelper.PropertyChanged += UnzipProgress;
                await zipHelper.UnZipFileAsync(file, gameFolder);
                Debug.WriteLine("Copy done: " + addon.FolderName + " " + DateTime.Now.ToString("mm:ss"));
            }
            catch (Exception e)
            {
                Debug.WriteLine("[ERROR] UpdateAddon. " + e.Message + ", " + e.StackTrace);
            }
            return subFoldersToDelete.ToList();
        }

        private static void UnzipProgress(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var zipHelper = sender as ZipHelper;
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.
                RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    zipHelper.Progressable.Progress = zipHelper.Progress;
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

        //
        //bool filesAreEqual = new FileInfo(path1).Length == new FileInfo(path2).Length &&
        //File.ReadAllBytes(path1).SequenceEqual(File.ReadAllBytes(path2));
        //
        internal static async Task CopyFolderAsync(StorageFolder source, StorageFolder destinationContainer, Counter counter)
        {
            StorageFolder destinationFolder = null;
            destinationFolder = await destinationContainer.CreateFolderAsync(source.Name, CreationCollisionOption.OpenIfExists);
            var files = await source.GetFilesAsync();
            //var fileTasks = files.Select(file => file.CopyAsync(destinationFolder).AsTask());
            //await Task.WhenAll(fileTasks);
            var moveTasks = files.Select(file => file.MoveAsync(destinationFolder, file.Name, NameCollisionOption.ReplaceExisting).AsTask());
            await Task.WhenAll(moveTasks);
            //foreach (var file in files)
            //{
            //    try
            //    {
            //        await file.MoveAsync(destinationFolder, file.Name, NameCollisionOption.ReplaceExisting);
            //    }
            //    catch (Exception)
            //    {
            //        // Debug.WriteLine("[ERROR] Copy file. " + e.Message);
            //    }
            //}
            counter.Current += files.Count;
            await counter.Dispatch();
            // Debug.WriteLine($"Files: {current}/{fileCount}, progress={progressable.Progress}");

            var folders = await source.GetFoldersAsync();
            //var folderTasks = folders.Select(folder => CopyFolderAsync(folder, destinationFolder));
            //await Task.WhenAll(folderTasks);
            // Parallel.ForEach(folders,folder=>FolderCopy(folder,destinationFolder));

            foreach (var folder in folders)
            {
                try
                {
                    // System.IO.Directory.Move(folder.Path,destinationFolder.Path);
                    await CopyFolderAsync(folder, destinationFolder, counter);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("[ERROR] Copy folder. " + e.Message);
                }
            }
            // Parallel.ForEach(files, file => FileMove(file, destinationFolder));
        }

        //Dont use, seems unstable. Getting stack owerflow
        //////////////////internal static List<Task> CopyFolderAsync2(StorageFolder source, StorageFolder destinationContainer)
        //////////////////{
        //////////////////    var (destinationFolder, items) = Task.Run(async () =>
        //////////////////     {
        //////////////////         StorageFolder destination = await destinationContainer.CreateFolderAsync(source.Name, CreationCollisionOption.OpenIfExists);
        //////////////////         var itemz = await source.GetItemsAsync();
        //////////////////         return (destination, itemz);
        //////////////////     }).Result;
        //////////////////    var tasks = items.OfType<StorageFile>().Select(file => file.MoveAsync(destinationFolder, file.Name, NameCollisionOption.ReplaceExisting).AsTask()).ToList();

        //////////////////    var folderTasks = items.OfType<StorageFolder>().SelectMany(folder => CopyFolderAsync2(folder, destinationFolder));
        //////////////////    tasks.AddRange(folderTasks);

        //////////////////    return tasks;

        //////////////////    //var items = Task.Run(async () => await source.GetItemsAsync()).Result;
        //////////////////    //            var files = Task.Run(async ()=> await source.GetFilesAsync()).Result;
        //////////////////    //var fileTasks = files.Select(file => file.MoveAsync(destinationFolder, file.Name, NameCollisionOption.ReplaceExisting).AsTask()).ToList();
        //////////////////    //await Task.WhenAll(fileTasks);

        //////////////////    //foreach (var file in files)
        //////////////////    //{
        //////////////////    //    try
        //////////////////    //    {
        //////////////////    //      await  file.MoveAsync(destinationFolder, file.Name, NameCollisionOption.ReplaceExisting);

        //////////////////    //    }
        //////////////////    //    catch (Exception e)
        //////////////////    //    {
        //////////////////    //        Debug.WriteLine("[ERROR] Copy file. " + e.Message);
        //////////////////    //    }

        //////////////////    //}
        //////////////////    //var folders = Task.Run(async () => await source.GetFoldersAsync()).Result;
        //////////////////    //var folderTasks = folders.SelectMany(folder => CopyFolderAsync2(folder, destinationFolder));
        //////////////////    //fileTasks.AddRange(folderTasks);
        //////////////////    //return fileTasks;
        //////////////////    //await Task.WhenAll(folderTasks);
        //////////////////    // Parallel.ForEach(folders,folder=>FolderCopy(folder,destinationFolder));
        //////////////////    //foreach (var folder in folders)
        //////////////////    //{
        //////////////////    //    try
        //////////////////    //    {
        //////////////////    //      await  CopyFolderAsync(folder, destinationFolder);

        //////////////////    //    }
        //////////////////    //    catch (Exception e)
        //////////////////    //    {
        //////////////////    //        Debug.WriteLine("[ERROR] Copy folder. " + e.Message);
        //////////////////    //    }
        //////////////////    //}
        //////////////////    // Parallel.ForEach(files, file => FileMove(file, destinationFolder));
        //////////////////}

        internal static async Task Cleanup(string fileName, string folderPath)
        {
            try
            {
                var file = await localFolder.TryGetItemAsync(fileName);
                await file?.DeleteAsync();
            }
            catch (Exception)
            {
                Debug.WriteLine("[ERROR] Shouldn't reach here");
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

        internal static async Task Cleanup2(string fileName)
        {
            try
            {
                var file = await localFolder.TryGetItemAsync(fileName);
                await file?.DeleteAsync();
            }
            catch (Exception)
            {
                Debug.WriteLine("[ERROR] Shouldn't reach here");
            }
        }

        internal static async Task AddSubFolders(Core.Models.Addon addon, List<string> subFoldersToDelete)
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
                addon.AddSubFolders(subFoldersToDelete);
            }
            await Task.CompletedTask;
        }

        internal static async Task RemoveFilesFor(Core.Models.Addon addon)
        {
            var folders = new List<string>(addon.SubFolders)
            {
                addon.FolderName
            };
            await RemoveFolders(addon.Game.AbsolutePath, folders);

            //var gameFolder = await StorageFolder.GetFolderFromPathAsync(addon.Game.AbsolutePath);
            //var addonFolder = await gameFolder.GetFolderAsync(addon.FolderName);
            //await addonFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
            //foreach (var folderName in addon.SubFolders)
            //{
            //    var folder = await gameFolder.GetFolderAsync(folderName);
            //    await folder.DeleteAsync(StorageDeleteOption.PermanentDelete);
            //}
        }

        internal static async Task RemoveFolders(string gameFolderPath, IEnumerable<string> folders)
        {
            var gameFolder = await StorageFolder.GetFolderFromPathAsync(gameFolderPath);
            foreach (var folderName in folders)
            {
                var folder = await gameFolder.GetFolderAsync(folderName);
                await folder.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
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
            public Core.Models.IProgressable Progressable { get; set; }

            public void DownloadProgressCallback(IAsyncOperationWithProgress<HttpResponseMessage, HttpProgress> asyncInfo, HttpProgress progressInfo)
            {
                if (progressInfo.TotalBytesToReceive != null && progressInfo.TotalBytesToReceive > 0)
                {
                    var progress = (int)((progressInfo.BytesReceived / (double)progressInfo.TotalBytesToReceive) * 100d);
                    //Debug.WriteLine("received " + (int)progress);
                    Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.
                        RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            Progressable.Progress = progress;
                        });
                }
            }
        }

        internal class Counter
        {
            private readonly IProgressable progressable;
            public float Current { get; set; } = 0;
            private readonly float max;

            public Counter(IProgressable progressable, float max)
            {
                this.progressable = progressable ?? throw new ArgumentNullException(nameof(progressable));
                this.max = max;
            }

            public async Task Dispatch()
            {
                var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    progressable.Progress = (int)(Current / max * 100f);
                });
            }
        }
    }
}
