using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Addon.Core.Helpers;
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

        private static List<string> TocFileTypeFilter = new List<string>() { ".toc" };

        private static Lazy<IList<string>> knownSubFolders = new Lazy<IList<string>>(LoadKnownSubFolders());

        public static IList<string> KnownFolders => knownSubFolders.Value;

        public static async Task<Game> FolderToGame(Windows.Storage.StorageFolder folder)
        {

            var game = new Game(folder.Path);
            var storageFolderQueryResult = folder.CreateFolderQuery(CommonFolderQuery.DefaultQuery);
            var folders = await storageFolderQueryResult.GetFoldersAsync();

            //foreach (var f in folders)
            //{
            //    var addon = new Core.Models.Addon(game, f.Name, f.Path);
            //    var add = await Task.Run(() => RefreshAddonFolder(addon));
            //    //var add = await new Task(Action);//RefreshAddonFolder(addon));
            //    if (add)
            //        game.Addons.Add(addon);


            //}

            //////var addons = folders.Select(FolderToTocFile)
                
            //////    .Where(tf => !tf.IsKnownSubFolder)
            //////    .Select(tf =>
            //////    {
            //////        var addon = new Core.Models.Addon(game, tf.StorageFolder.Name, tf.StorageFolder.Path);
            //////        addon.Version = tf.Version;
            //////        addon.GameVersion = tf.GameVersion;
            //////        return addon;
            //////    }).ToList();

            //////foreach (var addon in addons)
            //////{
            //////    game.Addons.Add(addon);
            //////}

            //foreach (var addon in game.Addons)
            //{
            //    RefreshAddonFolder(addon);
            //}


            //game.Addons.ToList().ForEach(RefreshAddonFolder);
            return game;
        }


        public static async Task<TocFile> FolderToTocFile(StorageFolder folder)
        {
            var version = GetVersion(folder).Result;
            var gameVersion = GetGameVersion(folder).Result;
            var isKnownSubFolder = knownSubFolders.Value.Any(k => k.Equals(folder.Name));
            return new TocFile(folder, version, gameVersion, isKnownSubFolder);
        }


        public static async Task<string> GetVersion(StorageFolder folder)
        {
            var folderFromPathAsync = await StorageFolder.GetFolderFromPathAsync(folder.Path);

            var files = await folderFromPathAsync.GetFilesAsync(CommonFileQuery.DefaultQuery);

            var file = files.First(f => f.FileType.Equals(".toc"));

            var lines = await FileIO.ReadLinesAsync(file);
            var interfaceLine = lines.FirstOrDefault(l => l.Contains("Interface:"));
            if (interfaceLine != null)
            {
                return interfaceLine.Substring(interfaceLine.IndexOf("Interface:") + 10).Trim();
            }
            return String.Empty;
        }

        public static async Task<string> GetGameVersion(StorageFolder folder)
        {
            var folderFromPathAsync = await StorageFolder.GetFolderFromPathAsync(folder.Path);
            var files = await folderFromPathAsync.GetFilesAsync(CommonFileQuery.DefaultQuery);
            var file = files.First(f => f.FileType.Equals(".toc"));
            var version = (await FileIO.ReadLinesAsync(file)).FirstOrDefault(l => l.Contains("Version:"));

            if (version != null)
            {
                return version.Substring(version.IndexOf("Version:") + 8).Trim();
            }
            return String.Empty;
        }

        //public static async Task<Boolean> RefreshAddonFolder(Core.Models.Addon addon)
        //{
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

        //    try
        //    {

        //        var folderFromPathAsync = await StorageFolder.GetFolderFromPathAsync(addon.AbsolutePath);

        //        var files = await folderFromPathAsync.GetFilesAsync(CommonFileQuery.DefaultQuery);

        //        var file = files.First(f => f.FileType.Equals(".toc"));

        //        var lines = await FileIO.ReadLinesAsync(file);
        //        var interfaceLine = lines.FirstOrDefault(l => l.Contains("Interface:"));
        //        if (interfaceLine != null)
        //        {
        //            addon.GameVersion = interfaceLine.Substring(interfaceLine.IndexOf("Interface:") + 10).Trim();
        //        }

        //        var version = (await FileIO.ReadLinesAsync(file)).FirstOrDefault(l => l.Contains("Version:"));


        //        if (version != null)
        //        {
        //            addon.Version = version.Substring(version.IndexOf("Version:") + 8).Trim();
        //        }

        //        //return knownSubFolders.stream().noneMatch(folder->folder.equalsIgnoreCase(addon.getFolderName()));
        //        return !knownSubFolders.Value.Any(k => k.Equals(addon.FolderName));

        //    }
        //    catch (Exception e)
        //    {
        //        Debug.WriteLine(addon.FolderName + " " + e);

        //    }

        //    return false;


        //    //boolean refresh() {
        //    //    var tocFile = new File(addon.getAbsolutePath()).listFiles((dir, name) -> name.toLowerCase().endsWith(".toc"));
        //    //    if (tocFile == null || tocFile[0] == null)
        //    //        return false;

        //    //    for (var charset : CHARSETS) {
        //    //        try {
        //    //            var tocString = Files.readString(tocFile[0].toPath(), charset);
        //    //            tocString.lines().filter(line -> line.contains("Interface:")).findAny().ifPresent(line -> addon.setGameVersion(line.substring(line.indexOf("Interface:") + 10).trim()));
        //    //            tocString.lines().filter(line -> line.contains("Version:")).findAny().ifPresent(line -> addon.setVersion(line.substring(line.indexOf("Version:") + 8).trim()));
        //    //            tocString.lines().filter(line -> line.contains("Title:")).findAny().ifPresent(line -> addon.setTitle(line.substring(line.indexOf("Title:") + 6).replaceAll("\\|c[a-zA-Z_0-9]{8}", "").replaceAll("\\|r", "").trim()));
        //    //            return knownSubFolders.stream().noneMatch(folder -> folder.equalsIgnoreCase(addon.getFolderName()));
        //    //        } catch (MalformedInputException e) {
        //    //            App.LOG.info(addon.getFolderName() + " " + e.getMessage());
        //    //        } catch (IOException e) {
        //    //            App.LOG.info(addon.getFolderName() + " " + e.getMessage());
        //    //            return false;
        //    //        }
        //    //    }
        //    //    return false;
        //    //}
        //}

        public static IList<string> LoadKnownSubFolders()
        {
            Debug.WriteLine("load known folders start");
            var packageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;

            string file = @"Assets\knownsubfolders.txt";
            var sampleFile = packageFolder.GetFileAsync(file);
            var lines = FileIO.ReadLinesAsync(sampleFile.GetResults());
            Debug.WriteLine("load known folders end");
            return lines.GetResults();
        }
    }


}
