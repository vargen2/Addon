using Addon.Core.Helpers;
using Addon.Core.Models;
using Addon.Core.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Addon.Logic
{
    public static class Parse
    {

        public static List<Download> FromPageToDownloads(Core.Models.Addon addon, string page)
        {
            if (addon.ProjectUrl.Contains("https://wow.curseforge.com/projects/"))
            {
                return FromWowCurseForgeToDownloads(page);
            }
            else if (addon.ProjectUrl.Contains("https://www.wowace.com/projects/"))
            {
                return FromWowaceToDownloads(page);
            }
            else if (addon.ProjectUrl.Equals(Version.ELVUI))
            {
                return FromElvUiToDownloads(page);
            }
            return new List<Download>();
        }

        public static List<Download> FromWowaceToDownloads(string htmlPage)
        {
            var downloads = new List<Download>();
            int index1 = htmlPage.IndexOf("<div class=\"listing-body\">");
            int index2 = htmlPage.IndexOf("</table>");
            string data = htmlPage.Substring(index1, index2 - index1);
            var strings = data.Split("<tr class=\"project-file-list-item\">").Skip(1).ToList();

            foreach (var s in strings)
            {
                string temp = Util.Parse2(s, "<td class=\"project-file-release-type\">", "</td>");
                string release = Util.Parse2(temp, "title=\"", "\"></div>");

                string title = Util.Parse2(s, "data-name=\"", "\">");
                string fileSize = Util.Parse2(s, "<td class=\"project-file-size\">", "</td>").Trim();

                string a = Util.Parse2(s, "data-epoch=\"", "\"");
                var dateUploaded = DateTimeOffset.FromUnixTimeSeconds(long.Parse(a)).LocalDateTime;

                string gameVersion = Util.Parse2(s, "<span class=\"version-label\">", "</span>");

                string tempDL = Util.Parse2(s, "<td class=\"project-file-downloads\">", "</td>").Replace(",", "").Trim();

                long dls = long.Parse(tempDL);
                string downloadLink = Util.Parse2(s, " href=\"", "\"");

                downloads.Add(new Download(release, title, fileSize, dateUploaded, gameVersion, dls, downloadLink));
            }
            return downloads;
        }

        public static List<Download> FromWowCurseForgeToDownloads(string htmlPage)
        {
            var downloads = new List<Download>();
            int index1 = htmlPage.IndexOf("<div class=\"listing-body\">");
            int index2 = htmlPage.IndexOf("</table>");
            string data = htmlPage.Substring(index1, index2 - index1);
            var strings = data.Split("<tr class=\"project-file-list-item\">").Skip(1).ToList();

            foreach (var s in strings)
            {
                string temp = Util.Parse2(s, "<td class=\"project-file-release-type\">", "</td>");
                string release = Util.Parse2(temp, "title=\"", "\"></div>");

                string title = Util.Parse2(s, "data-name=\"", "\">");
                string fileSize = Util.Parse2(s, "<td class=\"project-file-size\">", "</td>").Trim();

                string a = Util.Parse2(s, "data-epoch=\"", "\"");
                var dateUploaded = DateTimeOffset.FromUnixTimeSeconds(long.Parse(a)).LocalDateTime;

                string gameVersion = Util.Parse2(s, "<span class=\"version-label\">", "</span>");

                string tempDL = Util.Parse2(s, "<td class=\"project-file-downloads\">", "</td>").Replace(",", "").Trim();

                long dls = long.Parse(tempDL);
                string downloadLink = Util.Parse2(s, " href=\"", "\"");

                downloads.Add(new Download(release, title, fileSize, dateUploaded, gameVersion, dls, downloadLink));
            }
            return downloads;
        }

        public static List<Download> FromElvUiToDownloads(string htmlPage)
        {
            var downloads = new List<Download>();

            string downloadDiv = Util.Parse2(htmlPage, @"<div class=""tab-pane fade in active"" id=""download"">", "</div>");
            string parsedDownloadLink = Util.Parse2(downloadDiv, @"<a href=""/", @""" ");
            string downloadLink = "https://www.tukui.org/" + parsedDownloadLink;

            string versionDiv = Util.Parse2(htmlPage, @"<div class=""tab-pane fade"" id=""version"">", "</div>");
            string version = Util.Parse2(versionDiv, @"The current version of ElvUI is <b class=""Premium"">", @"</b>");
            string dateString = Util.Parse2(versionDiv, @"and was updated on <b class=""Premium"">", @"</b>");
            var date = DateTime.Parse(dateString);

            downloads.Add(new Download("Release", version, "3.6 MB", date, "", 0, downloadLink));

            return downloads;
        }

        public static string FromPageToChanges(string htmlPage)
        {
            try
            {
                string section = Util.Parse2(htmlPage, "<section class=\"project-content", "</section>");

                section = Regex.Replace(section, "href=\".*\"", "href=\"#\"");

                return section.Substring(section.IndexOf(">") + 1);
            }
            catch (Exception e)
            {
                Debug.WriteLine("[ERROR] FromPageToChanges, " + e.Message);
                return string.Empty;
            }
        }

        public static string FromElvUiPageToChanges(string htmlPage)
        {
            try
            {
                return Util.Parse2(htmlPage, @"<div class=""tab-pane fade "" id=""changelog"">", "</div>");
            }
            catch (Exception e)
            {
                Debug.WriteLine("[ERROR] FromElvUiPageToChanges, " + e.Message);
                return string.Empty;
            }
        }

        //
        // Can return null
        //
        //internal static string GetFromAddonDataFor(Core.Models.Addon addon)
        //{
        //    return Singleton<Session>.Instance.AddonData
        //       .Where(addonData => addonData.FolderName.Equals(addon.FolderName, StringComparison.OrdinalIgnoreCase))
        //       .Where(addonData => addonData.ProjectUrl.Contains("projects/"))
        //       .Select(addonData => addonData.ProjectUrl.Substring(addonData.ProjectUrl.IndexOf("projects/") + 9))
        //       .FirstOrDefault();
        //}

        internal static DateTime SafeParseFromEpochString(string epoch)
        {
            if (long.TryParse(epoch, out long result))
            {
                return DateTimeOffset.FromUnixTimeSeconds(result).UtcDateTime;
            }
            return DateTime.UtcNow;
        }

        public static List<StoreAddon> LoadStoreAddons(List<AddonData> addonDatas)
        {
            // var assets = await APP_INSTALLED_FOLDER.GetFolderAsync("Assets");
            // var curseAddons = await assets.ReadAsync<HashSet<CurseAddon>>("curseaddons");

            var storeAddons = addonDatas
                .Select(ca => new StoreAddon(ca))
                .ToList();

            storeAddons.Sort((x, y) =>
            {
                if (x == null || y == null)
                {
                    return 0;
                }
                // highest first
                return y.AddonData.NrOfDownloads.CompareTo(x.AddonData.NrOfDownloads);
            });
            //var elvuiAddonData= new AddonData() { FolderName= "ElvUI",Title="ElvUI",
            //    Description= "A user interface designed around user-friendliness with extra features that are not included in the standard UI.",
            //    NrOfDownloads= 100000000,
            //    UpdatedEpoch= 1557784800,
            //    CreatedEpoch=0,
            //    ProjectName="elvui",
            //    ProjectUrl= Version.ELVUI,
            //    SubFolders=new HashSet<string>(){"ElvUI_Config" },
            //    Files=100,
            //    Size=100
            //};
            //storeAddons.Insert(0, new StoreAddon(elvuiAddonData));
            return storeAddons;
        }
    }
}
