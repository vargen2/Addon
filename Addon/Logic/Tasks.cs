using Addon.Core.Helpers;
using Addon.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Web;
using Windows.Web.Http;

namespace Addon.Logic
{
    public static class Tasks
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

            public HashSet<string> KnownSubFolders { get; } = Tasks.LoadKnownSubFolders();

            private Singleton()
            {
            }
        }

        public class TocFile
        {
            public StorageFolder StorageFolder { get; }
            public string Version { get; }

            public string GameVersion { get; }

            public string Title { get; }

            public bool IsKnownSubFolder { get; }

            public TocFile(StorageFolder storageFolder, string version, string gameVersion, string title, bool isKnownSubFolder)
            {
                StorageFolder = storageFolder;
                Version = version;
                GameVersion = gameVersion;
                Title = title;
                IsKnownSubFolder = isKnownSubFolder;
            }
        }




        public static async Task RefreshGameFolder(Game game)
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
                    GameVersion = tf.GameVersion,
                    Title = tf.Title
                })
                .ToList().ForEach(game.Addons.Add);
            game.IsLoading = false;
        }

        public static async Task<TocFile> FolderToTocFile(StorageFolder folder)
        {
            var version = String.Empty;
            var gameVersion = String.Empty;
            var title = String.Empty;
            //var folderFromPathAsync = await StorageFolder.GetFolderFromPathAsync(folder.Path);
            var files = await folder.GetFilesAsync(CommonFileQuery.DefaultQuery);
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

                if (line.Contains("Title:"))
                {
                    var temp = line.Substring(line.IndexOf("Title:") + 6).Trim();
                    var temp2 = Regex.Replace(temp, "\\|c[a-zA-Z_0-9]{8}", "");
                    title = Regex.Replace(temp2, "\\|r", "");
                }
            }

            return new TocFile(folder, version, gameVersion, title, Singleton.Instance.KnownSubFolders.Contains(folder.Name));
        }

        public static async Task RefreshTocFileFor(IList<Core.Models.Addon> addons)
        {
            foreach (var addon in addons)
            {
                var folder = await StorageFolder.GetFolderFromPathAsync(addon.AbsolutePath);
                var tocFile = await FolderToTocFile(folder);
                addon.Title = tocFile.Title;
                addon.Version = tocFile.Version;
                addon.GameVersion = tocFile.GameVersion;
            }
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

        public static async Task FindProjectUrlAndDownLoadVersionsFor(IList<Core.Models.Addon> addons)
        {
            foreach (var addon in addons)
            {
                await FindProjectUrlAndDownLoadVersionsFor(addon);
            }
        }

        public static async Task FindProjectUrlAndDownLoadVersionsFor(Core.Models.Addon addon)
        {
            if (String.IsNullOrEmpty(addon.ProjectUrl))
            {
                addon.ProjectUrl = await FindProjectUrlFor(addon);
            }

            addon.Downloads = await DownloadVersionsFor(addon);
            // Debug.WriteLine("[SUCCESS] FindProjectUrlAndDownLoadVersionsFor: " + addon.ProjectUrl);
            //foreach (var download in addon.Downloads)
            //{
            //    Debug.WriteLine(download);
            //}
        }

        public static async Task<string> FindProjectUrlFor(Core.Models.Addon addon)
        {
            List<String> urlNames = new List<string>() { addon.FolderName.Replace(" ", "-"),
                addon.FolderName,addon.Title.Replace(" ","-"),addon.Title.Replace(" ",""),addon.Title };

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
                        return Util.Parse(data, "<a href=\"", "\">");
                    }
                    catch (Exception ex)
                    {
                        var error = WebSocketError.GetStatus(ex.HResult);
                        if (error == WebErrorStatus.Unknown)
                        {
                            Debug.WriteLine("[ERROR] FindProjectUrlFor " + uri + " " + ex.Message);
                        }
                        else
                        {
                            Debug.WriteLine("[ERROR] FindProjectUrlFor " + uri + " " + error);
                        }
                    }
                }
            }
            return String.Empty;
        }

        public static async Task<List<Download>> DownloadVersionsFor(Core.Models.Addon addon)
        {
            if (string.IsNullOrEmpty(addon.ProjectUrl))
            {
                return new List<Download>();
            }

            var uri = new Uri(addon.ProjectUrl + "/files");
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var htmlPage = await httpClient.GetStringAsync(uri);
                    return Parse.FromPageToDownloads(addon, htmlPage);
                }
                catch (Exception ex)
                {
                    var error = WebSocketError.GetStatus(ex.HResult);
                    if (error == WebErrorStatus.Unknown)
                    {
                        Debug.WriteLine("[ERROR] DownloadVersionsFor " + uri + " " + ex.Message);
                    }
                    else
                    {
                        Debug.WriteLine("[ERROR] DownloadVersionsFor " + uri + " " + error);
                    }
                }
            }
            return new List<Download>();
        }

        public static async Task UpdateAddon(Core.Models.Addon addon)
        {
            var file = await Update.DownloadFile(addon);
            Debug.WriteLine("file downloaded: " + file.Path);
            await Update.UpdateAddon(addon, file);
            Debug.WriteLine("Update addon omcplete: " + addon.FolderName);
        }
    }
}
