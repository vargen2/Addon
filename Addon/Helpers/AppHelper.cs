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
        private static List<string> TocFileTypeFilter = new List<string>() { ".toc" };

        private static Lazy<IList<string>> knownSubFolders = new Lazy<IList<string>>(LoadKnownSubFolders());

        public static IList<string> KnownFolders => knownSubFolders.Value;

        public static async Task<Game> FolderToGame(Windows.Storage.StorageFolder folder)
        {

            var game = new Game(folder.Path);
            var storageFolderQueryResult = folder.CreateFolderQuery(CommonFolderQuery.DefaultQuery);
            var folders = await storageFolderQueryResult.GetFoldersAsync();

            foreach (var f in folders)
            {
                game.Addons.Add(new Core.Models.Addon(game, f.Name, f.Path));
            }

            
                game.Addons.ToList().ForEach(RefreshAddonFolder);
           
            

            return game;
        }


        public static void RefreshAddonFolder(Core.Models.Addon addon)
        {
            //public static void createKnownSubFolders() {
            //    var game = App.model.getSelectedGame();
            //    String saveString = game.getAddons().stream()
            //            .filter(addon -> addon.getExtraFolders() != null)
            //            .map(Addon::getExtraFolders)
            //            .flatMap(Collection::stream)
            //            .map(file -> file.getName() + Util.LINE)
            //            .collect(Collectors.joining());
            //    try {
            //        Files.writeString(KNOWN, saveString);
            //    } catch (IOException e) {
            //        e.printStackTrace();
            //    }
            //}

            //private static final List<Charset> CHARSETS = List.of(Charset.defaultCharset(), Charset.forName("ISO-8859-1"));
            //private Addon addon;

            //TocRefresher(Addon addon) {
            //    this.addon = addon;
            //}
            Task.Run(async () =>
            {
                var folderFromPathAsync = await StorageFolder.GetFolderFromPathAsync(addon.AbsolutePath);
                var query = folderFromPathAsync.CreateFileQueryWithOptions(new QueryOptions(CommonFileQuery.DefaultQuery,
                    TocFileTypeFilter));
                IReadOnlyList<StorageFile> fileList = await query.GetFilesAsync();

                Debug.WriteLine(addon.FolderName+" "+fileList.Count);
                var lines = await FileIO.ReadLinesAsync(fileList.First());


            });

            //boolean refresh() {
            //    var tocFile = new File(addon.getAbsolutePath()).listFiles((dir, name) -> name.toLowerCase().endsWith(".toc"));
            //    if (tocFile == null || tocFile[0] == null)
            //        return false;

            //    for (var charset : CHARSETS) {
            //        try {
            //            var tocString = Files.readString(tocFile[0].toPath(), charset);
            //            tocString.lines().filter(line -> line.contains("Interface:")).findAny().ifPresent(line -> addon.setGameVersion(line.substring(line.indexOf("Interface:") + 10).trim()));
            //            tocString.lines().filter(line -> line.contains("Version:")).findAny().ifPresent(line -> addon.setVersion(line.substring(line.indexOf("Version:") + 8).trim()));
            //            tocString.lines().filter(line -> line.contains("Title:")).findAny().ifPresent(line -> addon.setTitle(line.substring(line.indexOf("Title:") + 6).replaceAll("\\|c[a-zA-Z_0-9]{8}", "").replaceAll("\\|r", "").trim()));
            //            return knownSubFolders.stream().noneMatch(folder -> folder.equalsIgnoreCase(addon.getFolderName()));
            //        } catch (MalformedInputException e) {
            //            App.LOG.info(addon.getFolderName() + " " + e.getMessage());
            //        } catch (IOException e) {
            //            App.LOG.info(addon.getFolderName() + " " + e.getMessage());
            //            return false;
            //        }
            //    }
            //    return false;
            //}
        }

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
