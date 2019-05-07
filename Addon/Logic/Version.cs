using Addon.Core.Helpers;
using Addon.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace Addon.Logic
{
    internal static class Version
    {
        public const string ELVUI = "https://www.tukui.org/download.php?ui=elvui";

        public static Dictionary<string, List<string>> PROJECT_URLS = new Dictionary<string, List<string>>()
        {
            {"bigwigs", new List<string>{"big-wigs"}},
            {"dbm-core", new List<string>{"deadly-boss-mods"}},
            {"omnicc", new List<string>{"omni-cc"}},
            {"omen", new List<string>{"omen-threat-meter"}},
            {"littlewigs", new List<string>{"little-wigs"}},
            {"elvui_sle", new List<string>{"elvui-shadow-light"}},
            {"atlasloot", new List<string>{"atlasloot-enhanced"}},
            {"healbot", new List<string>{"heal-bot-continued"}},
            {"tradeskillmaster", new List<string>{"tradeskill-master"}},
            {"auc-advanced", new List<string>{"auctioneer"}},
            {"titan", new List<string>{"titan-panel"}},
            {"tidyplates_threatplates", new List<string>{"tidy-plates-threat-plates"}},
            {"maxdps", new List<string>{"maxdps-rotation-helper"}},
            {"easydelete", new List<string>{"easy-delete"}},
            {"raidframeicons", new List<string>{"enhanced-raid-frame-icons"}},
            {"allthethings", new List<string>{"all-the-things"}},
            {"dbm-dragonsoul", new List<string>{"deadly-boss-mods-cataclysm-mods"}},
            {"dbm-icecrown", new List<string>{"deadly-boss-mods-wotlk"}},
            {"dbm-pandaria", new List<string>{"deadly-boss-mods-mop"}},
            {"dbm-outlands", new List<string>{"dbm-bc"}}

        };

        internal static async Task<string> FindProjectUrlFor(Core.Models.Addon addon)
        {
            List<String> urlNames = new List<string>() { addon.FolderName.Replace(" ", "-"),
                addon.FolderName,addon.Title.Replace(" ","-"),addon.Title.Replace(" ",""),addon.Title };

            if (PROJECT_URLS.TryGetValue(addon.FolderName.ToLower(), out List<string> list))
            {
                urlNames.InsertRange(0, list);
            }

            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return string.Empty;
            }

            foreach (var urlName in urlNames)
            {
                var url = await GetUrl(urlName);
                if (!string.IsNullOrEmpty(url))
                {
                    return url;
                }
            }
            return String.Empty;
        }

        internal static async Task<List<Download>> DownloadVersionsFor(Core.Models.Addon addon)
        {
            if (string.IsNullOrEmpty(addon.ProjectUrl))
            {
                return new List<Download>();
            }

            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return new List<Download>();
            }

            if (addon.ProjectUrl.Equals(ELVUI))
            {
                return await FromElvUI(addon);
            }

            return await FromCurse(addon);
        }

        private static async Task<List<Download>> FromCurse(Core.Models.Addon addon)
        {
            var uri = new Uri(addon.ProjectUrl + "/files");
            //using (var httpClient = new HttpClient())
            //{
            try
            {
                var htmlPage = await Http.WebHttpClient.GetStringAsync(uri);
                return Parse.FromPageToDownloads(addon, htmlPage);
            }
            catch (Exception ex)
            {
                var error = WebSocketError.GetStatus(ex.HResult);
                if (error == Windows.Web.WebErrorStatus.Unknown)
                {
                    Debug.WriteLine("[ERROR] DownloadVersionsFor " + uri + " " + ex.Message);
                }
                else
                {
                    Debug.WriteLine("[ERROR] DownloadVersionsFor " + uri + " " + error);
                }
            }
            //}
            return new List<Download>();
        }

        private static async Task<List<Download>> FromElvUI(Core.Models.Addon addon)
        {
            var uri = new Uri(addon.ProjectUrl);
            //using (var httpClient = new HttpClient())
            //{
            try
            {
                var htmlPage = await Http.WebHttpClient.GetStringAsync(uri);
                return Parse.FromPageToDownloads(addon, htmlPage);
            }
            catch (Exception ex)
            {
                var error = WebSocketError.GetStatus(ex.HResult);
                if (error == Windows.Web.WebErrorStatus.Unknown)
                {
                    Debug.WriteLine("[ERROR] DownloadVersionsFor " + uri + " " + ex.Message);
                }
                else
                {
                    Debug.WriteLine("[ERROR] DownloadVersionsFor " + uri + " " + error);
                }
            }
            //}
            return new List<Download>();
        }

        internal static async Task<string> FindProjectUrlFor(string urlName)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return string.Empty;
            }
            return await GetUrl(urlName);
        }

        private static async Task<string> GetUrl(string urlName)
        {
            if (urlName.ToLower().Equals("elvui"))
            {
                return ELVUI;
            }

            var uri = new Uri(@"https://www.curseforge.com/wow/addons/" + urlName);
            //using (var httpClient = new HttpClient())
            //{
            try
            {
                var response = await Http.WebHttpClient.GetStringAsync(uri);
                int index1 = response.IndexOf("<p class=\"infobox__cta\"");
                int index2 = response.Substring(index1).IndexOf("</p>");
                string data = response.Substring(index1, index2);
                return Util.Parse(data, "<a href=\"", "\">");
            }
            catch (Exception ex)
            {
                var error = WebSocketError.GetStatus(ex.HResult);
                if (error == Windows.Web.WebErrorStatus.Unknown)
                {
                    Debug.WriteLine("[ERROR] FindProjectUrlFor " + uri + " " + ex.Message);
                }
                else
                {
                    Debug.WriteLine("[ERROR] FindProjectUrlFor " + uri + " " + error);
                }
            }
            //}
            return String.Empty;
        }

    }
}
