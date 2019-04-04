using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Core;
using Windows.Web.Http;
using Addon.Core.Models;
using static System.String;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Web;

namespace Addon.Helpers
{
    public static class AppHelper
    {
        private static Dictionary<string, List<string>> PROJECT_URLS = new Dictionary<string, List<string>>()
        {
            {"bigwigs", new List<string>{"big-wigs"}},
            {"dbm-core", new List<string>{"deadly-boss-mods"}},
            {"omnicc", new List<string>{"omni-cc"}},
            {"omen", new List<string>{"omen-threat-meter"}},
            {"littlewigs", new List<string>{"little-wigs"}},
            {"elvui_sle", new List<string>{"elvui-shadow-light"}},
            {"atlasloot", new List<string>{"atlasloot-enhanced"}},
            {"healbot", new List<string>{"heal-bot-continued"}},
            {"tradeskillmaster", new List<string>{"tradeskill-master"}}
        };

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


        public static async Task FindProjectUrlAndDownLoadVersionsFor(Core.Models.Addon addon)
        {
            if (String.IsNullOrEmpty(addon.ProjectUrl))
            {
                addon.ProjectUrl = await FindProjectUrlFor(addon);
            }
            addon.Downloads = await DownloadVersionsFor(addon);
        }

        public static async Task<string> FindProjectUrlFor(Core.Models.Addon addon)
        {
            List<String> urlNames = new List<string>() { addon.FolderName.Replace(" ", "-"), addon.FolderName };

            if (PROJECT_URLS.TryGetValue(addon.FolderName.ToLower(), out List<string> list))
            {
                urlNames.InsertRange(0, list);
            }

            foreach (var urlName in urlNames)
            {
                var uri = new Uri(@"https://www.curseforge.com/wow/addons/" + urlName);
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        var response = await httpClient.GetStringAsync(uri);
                        int index1 = response.IndexOf("<p class=\"infobox__cta\"");
                        int index2 = response.Substring(index1).IndexOf("</p>");
                        string data = response.Substring(index1, index2);
                        return Parse(data, "<a href=\"", "\">");
                    }
                    catch (Exception ex)
                    {
                        var error = WebSocketError.GetStatus(ex.HResult);
                        if (error == WebErrorStatus.Unknown)
                        {
                            Debug.WriteLine(uri + " " + ex.Message);
                        }
                        else
                        {
                            Debug.WriteLine(uri + " " + error);
                        }
                    }
                }
            }
            return Empty;
        }

        public static async Task<List<Download>> DownloadVersionsFor(Core.Models.Addon addon)
        {
            if (addon.ProjectUrl.Contains("https://wow.curseforge.com/projects/"))
            {

                //return new WowCurseForge(addon);

            }
            else if (addon.FolderName.Contains("https://www.wowace.com/projects/"))
            {
                var uri = new Uri(addon.ProjectUrl + "/files");
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        var response = await httpClient.GetStringAsync(uri);
                        
                    }
                    catch (Exception ex)
                    {
                        var error = WebSocketError.GetStatus(ex.HResult);
                        if (error == WebErrorStatus.Unknown)
                        {
                            Debug.WriteLine(uri + " " + ex.Message);
                        }
                        else
                        {
                            Debug.WriteLine(uri + " " + error);
                        }
                    }
                }

            }
            return new List<Download>();
        }

        public static String Parse(string input, string start, string end)
        {
            int startI = input.IndexOf(start) + start.Length;
            string mid = input.Substring(startI);
            return mid.Substring(0, mid.IndexOf(end)).Trim();
        }
    }
}
