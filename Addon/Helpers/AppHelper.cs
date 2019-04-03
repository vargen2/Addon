using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Addon.Core.Models;


namespace Addon.Helpers
{
    public static class AppHelper
    {
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

        private static readonly HashSet<string> knownSubFolders = new HashSet<string>();

        public static async Task<Game> FolderToGame(Windows.Storage.StorageFolder folder)
        {
            if (knownSubFolders.Count == 0)
            {
                var list = await LoadKnownSubFolders();
                knownSubFolders.UnionWith(list);
            }

            var game = new Game(folder.Path);
            var storageFolderQueryResult = folder.CreateFolderQuery(CommonFolderQuery.DefaultQuery);
            var folders = await storageFolderQueryResult.GetFoldersAsync();

            var tasks = await Task.WhenAll(folders.Select(FolderToTocFile));

            tasks.Where(tf => tf != null && !tf.IsKnownSubFolder)
                .Select(tf =>
                {
                    var addon = new Core.Models.Addon(game, tf.StorageFolder.Name, tf.StorageFolder.Path)
                    {
                        Version = tf.Version,
                        GameVersion = tf.GameVersion
                    };
                    return addon;
                })
                .ToList()
                .ForEach(game.Addons.Add);


            return game;
        }


        public static async Task<TocFile> FolderToTocFile(StorageFolder folder)
        {
            var version = String.Empty;
            var gameVersion = String.Empty;
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

            return new TocFile(folder, version, gameVersion, knownSubFolders.Contains(folder.Name));
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

        public static async Task<IList<string>> LoadKnownSubFolders()
        {
            var packageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            string file = @"Assets\knownsubfolders.txt";
            var sampleFile = await packageFolder.GetFileAsync(file);
            var lines = await FileIO.ReadLinesAsync(sampleFile);
            return lines;
        }
    }
}
