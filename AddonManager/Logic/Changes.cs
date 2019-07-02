using AddonToolkit.Parse;
using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace AddonManager.Logic
{
    internal static class Changes
    {
        internal static async Task<string> DownloadChangesFor(Core.Models.Addon addon)
        {
            if (string.IsNullOrEmpty(addon.ProjectUrl))
            {
                Debug.WriteLine("No project url found");
                return string.Empty;
            }
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                Debug.WriteLine("No Internet connection available");
                return string.Empty;
            }

            var uri = GetChangeLogUri(addon);

            try
            {
                var htmlPage = await Http.WebHttpClient.GetStringAsync(uri);
                if (string.IsNullOrEmpty(htmlPage))
                {
                    return string.Empty;
                }
                Debug.WriteLine("changes: " + htmlPage);
                return ParsedPage(addon, htmlPage);
            }
            catch (Exception ex)
            {
                var error = WebSocketError.GetStatus(ex.HResult);
                Debug.WriteLine("[ERROR] DownloadChangesFor " + uri + " " + error);
                return string.Empty;
            }
        }

        private static string ParsedPage(Core.Models.Addon addon, string htmlPage)
        {
            return (addon.ProjectUrl.Equals(Version.ELVUI)) ?
                                    Parse.FromElvUiPageToChanges(htmlPage) :
                                    HtmlAgilityParser.FromCursePageToChanges(htmlPage);
        }

        private static Uri GetChangeLogUri(Core.Models.Addon addon)
        {
            if (addon.ProjectUrl.Equals(Version.ELVUI))
            {
                return new Uri(addon.ProjectUrl);
            }

            //var changeUrl = addon.ProjectUrl.Substring(addon.ProjectUrl.IndexOf("projects/"));
            //var temp = changeUrl.Replace("projects/", "https://www.curseforge.com/wow/addons/");
            return new Uri("https://www.curseforge.com/wow/addons/" + addon.ProjectUrl + "/files");
        }
    }
}
