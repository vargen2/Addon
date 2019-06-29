using AddonToolkit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
using static AddonToolkit.Model.Enums;
using Microsoft.Extensions.Logging;

namespace AddonToolkit.Parse
{
    public static class HtmlParser
    {
        public static List<Download> FromPageToDownloads(PROJECT_SITE projectSite, string page)
        {
            if (string.IsNullOrEmpty(page))
            {
                throw new ArgumentException("Can't be null or empty", nameof(page));
            }

            switch (projectSite)
            {
                case PROJECT_SITE.WOWACE:
                    return FromWowaceToDownloads(page);

                case PROJECT_SITE.CURSEFORGE:
                    return FromWowCurseForgeToDownloads(page);

                case PROJECT_SITE.ELVUI:
                    return FromElvUiToDownloads(page);
            }
            throw new ArgumentException("Invalid enum", nameof(projectSite));
        }

        public static List<Download> FromWowaceToDownloads(string htmlPage)
        {
            var downloads = new List<Download>();
            int index1 = htmlPage.IndexOf("<div class=\"listing-body\">");
            int index2 = htmlPage.IndexOf("</table>");
            string data = htmlPage.Substring(index1, index2 - index1);
            var strings = Regex.Split(data, "<tr class=\"project-file-list-item\">").Skip(1).ToList();

            foreach (var s in strings)
            {
                string temp = Parser.Parse(s, "<td class=\"project-file-release-type\">", "</td>");
                string release = Parser.Parse(temp, "title=\"", "\"></div>");

                string title = Parser.Parse(s, "data-name=\"", "\">");
                string fileSize = Parser.Parse(s, "<td class=\"project-file-size\">", "</td>").Trim();

                string a = Parser.Parse(s, "data-epoch=\"", "\"");
                var dateUploaded = DateTimeOffset.FromUnixTimeSeconds(long.Parse(a)).LocalDateTime;

                string gameVersion = Parser.Parse(s, "<span class=\"version-label\">", "</span>");

                string tempDL = Parser.Parse(s, "<td class=\"project-file-downloads\">", "</td>").Replace(",", "").Trim();

                long dls = long.Parse(tempDL);
                string downloadLink = Parser.Parse(s, " href=\"", "\"");

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
            var strings = Regex.Split(data, "<tr class=\"project-file-list-item\">").Skip(1).ToList();

            foreach (var s in strings)
            {
                string temp = Parser.Parse(s, "<td class=\"project-file-release-type\">", "</td>");
                string release = Parser.Parse(temp, "title=\"", "\"></div>");

                string title = Parser.Parse(s, "data-name=\"", "\">");
                string fileSize = Parser.Parse(s, "<td class=\"project-file-size\">", "</td>").Trim();

                string a = Parser.Parse(s, "data-epoch=\"", "\"");
                var dateUploaded = DateTimeOffset.FromUnixTimeSeconds(long.Parse(a)).LocalDateTime;

                string gameVersion = Parser.Parse(s, "<span class=\"version-label\">", "</span>");

                string tempDL = Parser.Parse(s, "<td class=\"project-file-downloads\">", "</td>").Replace(",", "").Trim();

                long dls = long.Parse(tempDL);
                string downloadLink = Parser.Parse(s, " href=\"", "\"");

                downloads.Add(new Download(release, title, fileSize, dateUploaded, gameVersion, dls, downloadLink));
            }
            return downloads;
        }

        public static List<Download> FromElvUiToDownloads(string htmlPage)
        {
            var downloads = new List<Download>();

            string downloadDiv = Parser.Parse(htmlPage, @"<div class=""tab-pane fade in active"" id=""download"">", "</div>");
            string parsedDownloadLink = Parser.Parse(downloadDiv, @"<a href=""/", @""" ");
            string downloadLink = "https://www.tukui.org/" + parsedDownloadLink;

            string versionDiv = Parser.Parse(htmlPage, @"<div class=""tab-pane fade"" id=""version"">", "</div>");
            string version = Parser.Parse(versionDiv, @"The current version of ElvUI is <b class=""Premium"">", @"</b>");
            string dateString = Parser.Parse(versionDiv, @"and was updated on <b class=""Premium"">", @"</b>");
            var date = DateTime.Parse(dateString);

            downloads.Add(new Download("Release", version, "3.6 MB", date, "", 0, downloadLink));

            return downloads;
        }

        public static string FromPageToChanges(string htmlPage)
        {
            try
            {
                string section = Parser.Parse(htmlPage, "<section class=\"project-content", "</section>");

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
                return Parser.Parse(htmlPage, @"<div class=""tab-pane fade "" id=""changelog"">", "</div>");
            }
            catch (Exception e)
            {
                Debug.WriteLine("[ERROR] FromElvUiPageToChanges, " + e.Message);
                return string.Empty;
            }
        }

        public static List<CurseAddon> FromCursePageToCurseAddons(string page, ILogger log = null)
        {
            var addons = new List<CurseAddon>();
            var strings = new List<string>();
            try
            {
                log?.LogInformation("page.Length, {page.Length}!", page.Length);
                strings = Regex.Split(page, "<div class=\"project-listing-row").Skip(1).ToList();
                log?.LogInformation("strings.count {strings.Count}", strings.Count);
            }
            catch (Exception e)
            {
                log?.LogError(e, nameof(FromCursePageToCurseAddons));
                return addons;
            }
            foreach (var s in strings)
            {
                try
                {
                    var url = Parser.Parse(s, "<a href=\"/wow/addons/", "\">").Trim();
                    var titleHtml = Parser.Parse(s, "<h2 class=\"list-item__title strong mg-b-05\">", "</h2>").Trim();
                    var title = System.Net.WebUtility.HtmlDecode(titleHtml);
                    var descriptionHtml = Parser.Parse(s, "<p title=\"", "\">").Trim();
                    var description = System.Net.WebUtility.HtmlDecode(descriptionHtml);

                    var downloadsString = Parser.Parse(s, "<span class=\"has--icon count--download\">", "</span>").Replace(",", "").Trim();
                    long downloads = long.Parse(downloadsString);

                    var updatedString = Parser.Parse(s, "<span class=\"has--icon date--updated\">", "</abbr></span>");
                    var updatedEpochString = Parser.Parse(updatedString, "data-epoch=\"", "\">");
                    var updated = long.Parse(updatedEpochString);

                    var createdString = Parser.Parse(s, "<span class=\"has--icon date--created\">", "</abbr></span>");
                    var createdEpochString = Parser.Parse(createdString, "data-epoch=\"", "\">");
                    var created = long.Parse(createdEpochString);

                    addons.Add(new CurseAddon()
                    {
                        AddonURL = url,
                        Title = title,
                        Description = description,
                        Downloads = downloads,
                        UpdatedEpoch = updated,
                        CreatedEpoch = created
                    });
                }
                catch (Exception e)
                {
                    log?.LogError(e, nameof(FromCursePageToCurseAddons));
                }
            }
            return addons;
        }
    }
}