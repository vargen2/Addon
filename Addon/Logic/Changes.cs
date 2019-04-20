using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Web;
using Windows.Web.Http;

namespace Addon.Logic
{
    internal static class Changes
    {

        internal static async Task<string> DownloadChangesFor(Core.Models.Addon addon)
        {
            if (string.IsNullOrEmpty(addon.ProjectUrl))
            {
                return string.Empty;
            }

            var changeUrl = addon.ProjectUrl.Substring(addon.ProjectUrl.IndexOf("projects/"));
            var aa = changeUrl.Replace("projects/", "https://www.curseforge.com/wow/addons/");
            var uri = new Uri(aa + "/changes");
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var htmlPage = await httpClient.GetStringAsync(uri);
                    return Parse.FromPageToChanges(htmlPage);
                    //return string.Join("\r\n",Parse.FromPageToChanges(htmlPage));
                }
                catch (Exception ex)
                {
                    var error = WebSocketError.GetStatus(ex.HResult);
                    if (error == WebErrorStatus.Unknown)
                    {
                        Debug.WriteLine("[ERROR] DownloadChangesFor " + uri + " " + ex.Message);
                    }
                    else
                    {
                        Debug.WriteLine("[ERROR] DownloadChangesFor " + uri + " " + error);
                    }
                }
            }
            return string.Empty;
        }
    }
}
