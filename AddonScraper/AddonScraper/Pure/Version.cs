using AddonToolkit.Model;
using AddonToolkit.Parse;
using NLog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace AddonScraper.Pure
{
    public static class Version
    {
        private static readonly Logger logger = LogManager.GetLogger("AddonScraper");       

        public static async Task<string> FindProjectUrlFor(HttpClient httpClient, string projectName)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return string.Empty;
            }
            return await GetUrl(httpClient, projectName);
        }

        public static async Task<List<Download>> DownloadVersionsFor(HttpClient httpClient, string projectUrl)
        {
            if (string.IsNullOrEmpty(projectUrl))
            {
                return new List<Download>();
            }

            return await FromCurse(httpClient, projectUrl);
        }

        private static async Task<List<Download>> FromCurse(HttpClient httpClient, string projectUrl)
        {
            var uri = new Uri(projectUrl + "/files");

            try
            {
                var htmlPage = await httpClient.GetStringAsync(uri);
                var projectSite = Parser.FromProjectUrlToEnum(projectUrl);
                return HtmlParser.FromPageToDownloads(projectSite, htmlPage);
            }
            catch (Exception ex)
            {
                logger.Error(ex, nameof(FromCurse)+ " for "+projectUrl);
            }

            return new List<Download>();
        }

        //private static async Task<List<Download>> FromElvUI(Core.Models.Addon addon)
        //{
        //    var uri = new Uri(addon.ProjectUrl);
        //    //using (var httpClient = new HttpClient())
        //    //{
        //    try
        //    {
        //        var htmlPage = await Http.WebHttpClient.GetStringAsync(uri);
        //        return Parse.FromPageToDownloads(addon, htmlPage);
        //    }
        //    catch (Exception ex)
        //    {
        //        var error = WebSocketError.GetStatus(ex.HResult);
        //        if (error == Windows.Web.WebErrorStatus.Unknown)
        //        {
        //            Debug.WriteLine("[ERROR] DownloadVersionsFor " + uri + " " + ex.Message);
        //        }
        //        else
        //        {
        //            Debug.WriteLine("[ERROR] DownloadVersionsFor " + uri + " " + error);
        //        }
        //    }
        //    //}
        //    return new List<Download>();
        //}

       

        private static async Task<string> GetUrl(HttpClient httpClient, string projectName)
        {
            if (projectName.ToLower().Equals("elvui"))
            {
                return Consts.ELVUI;
            }

            var uri = new Uri(@"https://www.curseforge.com/wow/addons/" + projectName);
           
            try
            {
                var response = await httpClient.GetStringAsync(uri);
                int index1 = response.IndexOf("<p class=\"infobox__cta\"");
                int index2 = response.Substring(index1).IndexOf("</p>");
                string data = response.Substring(index1, index2);
                return Parser.Parse(data, "<a href=\"", "\">");
            }
            catch (Exception ex)
            {
               logger.Error(ex,nameof(GetUrl)+" for "+projectName);
            }
            
            return String.Empty;
        }

    }
}
