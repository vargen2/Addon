using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Core;
using Windows.Web.Http;
using Addon.Core.Models;
using static System.String;

namespace Addon.Helpers
{
    public static class AppHelper
    {
        private const string KnownSubFoldersFile = @"Assets\knownsubfolders.txt";

        public sealed class Singleton
        {
            private static readonly Lazy<Singleton> lazy = new Lazy<Singleton>(() => new Singleton());

            public static Singleton Instance => lazy.Value;

            public HashSet<string> KnownSubFolders { get; } = AppHelper.LoadKnownSubFolders();

            private Singleton()
            {
            }
        }

        public class TocFile
        {
            public StorageFolder StorageFolder { get; }
            public string Version { get; }

            public string GameVersion { get; }

            public bool IsKnownSubFolder { get; }

            public TocFile(StorageFolder storageFolder, string version, string gameVersion, bool isKnownSubFolder)
            {
                StorageFolder = storageFolder;
                Version = version;
                GameVersion = gameVersion;
                IsKnownSubFolder = isKnownSubFolder;
            }
        }




        public static async void RefreshGameFolder(Game game)
        {
            //Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Wait, 10);
            game.IsLoading = true;
            var folder = await StorageFolder.GetFolderFromPathAsync(game.AbsolutePath);
            var storageFolderQueryResult = folder.CreateFolderQuery(CommonFolderQuery.DefaultQuery);
            var folders = await storageFolderQueryResult.GetFoldersAsync();

            var tasks = await Task.WhenAll(folders.Select(FolderToTocFile));

            tasks.Where(tf => tf != null && !tf.IsKnownSubFolder)
                .Select(tf => new Core.Models.Addon(game, tf.StorageFolder.Name, tf.StorageFolder.Path)
                {
                    Version = tf.Version,
                    GameVersion = tf.GameVersion
                })
                .ToList().ForEach(game.Addons.Add);
            game.IsLoading = false;
        }

        public static Game FolderToGame(StorageFolder folder)
        {
            return new Game(folder.Path);
        }

        public static async Task<TocFile> FolderToTocFile(StorageFolder folder)
        {
            var version = Empty;
            var gameVersion = Empty;
            var folderFromPathAsync = await StorageFolder.GetFolderFromPathAsync(folder.Path);
            var files = await folderFromPathAsync.GetFilesAsync(CommonFileQuery.DefaultQuery);
            var file = files.First(f => f.FileType.Equals(".toc"));

            var lines = await FileIO.ReadLinesAsync(file);
            foreach (var line in lines)
            {
                if (line.Contains("Interface:"))
                {
                    gameVersion = line.Substring(line.IndexOf("Interface:") + 10).Trim();
                }

                if (line.Contains("Version:"))
                {
                    version = line.Substring(line.IndexOf("Version:") + 8).Trim();
                }
            }

            return new TocFile(folder, version, gameVersion, Singleton.Instance.KnownSubFolders.Contains(folder.Name));
        }


        //    //public static void createKnownSubFolders() {
        //    //    var game = App.model.getSelectedGame();
        //    //    String saveString = game.getAddons().stream()
        //    //            .filter(addon -> addon.getExtraFolders() != null)
        //    //            .map(Addon::getExtraFolders)
        //    //            .flatMap(Collection::stream)
        //    //            .map(file -> file.getName() + Util.LINE)
        //    //            .collect(Collectors.joining());
        //    //    try {
        //    //        Files.writeString(KNOWN, saveString);
        //    //    } catch (IOException e) {
        //    //        e.printStackTrace();
        //    //    }
        //    //}

        //    //private static final List<Charset> CHARSETS = List.of(Charset.defaultCharset(), Charset.forName("ISO-8859-1"));

        public static HashSet<string> LoadKnownSubFolders()
        {
            return Task.Run(async () =>
              {
                  var packageFolder = Package.Current.InstalledLocation;
                  var sampleFile = await packageFolder.GetFileAsync(KnownSubFoldersFile);
                  var lines = await FileIO.ReadLinesAsync(sampleFile);
                  return new HashSet<string>(lines);
              }).Result;
        }

        public static async Task<string> DoCurlAsync()
        {
            var uri = new Uri("https://exonojnjnojjkmle.com");
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var task = httpClient.GetStringAsync(uri);
                    //task.Progress = (installResult, progress) => CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    //{
                    //    //StatusTextBlock.Text = "Progress: " + progress;
                    //});

                    return await task;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                }
            }

            return Empty;
            //using (var httpResponse = await httpClient.GetAsync(uri))
            //{
            //    return await httpResponse.Content.ReadAsStringAsync();
            //}
        }
    }
}
