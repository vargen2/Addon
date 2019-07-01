using AddonToolkit.AddonToolkit.Parse;
using AddonToolkit.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using static AddonToolkit.Model.Enums;

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
                case PROJECT_SITE.CURSEFORGE:
                    return HtmlAgilityParser.FromCurseForgeToDownloads(page);

                case PROJECT_SITE.ELVUI:
                    return FromElvUiToDownloads(page);
            }
            throw new ArgumentException("Invalid enum", nameof(projectSite));
        }

        //    public static List<Download> FromCurseForgeToDownloads(string htmlPage)
        //    {
        //        var downloads = new List<Download>();

        //        var strings = Regex.Split(htmlPage, @"<div class=""w-5 h-5 bg-green-500 flex items-center justify-center text-white mx-auto rounded-sm"">").Skip(1).ToList();

        //        foreach (var s in strings)
        //        {
        //            // Console.WriteLine(s.Substring(0, 49));
        //            string release = Parser.Parse(s, "<span class=\"text-white\">", "</span>");

        //            string temp = Parser.Parse(s, "<a data-action=\"file-link\"", "/a>");
        //            string title = Parser.Parse(temp, "\">", "<");

        //            string fileSize = Parser.Parse(s, @"</a>

        //</td>
        //<td>", "</td>").Trim();

        //            string a = Parser.Parse(s, "data-epoch=\"", "\"");
        //            var dateUploaded = DateTimeOffset.FromUnixTimeSeconds(long.Parse(a)).LocalDateTime;

        //            string gameVersion = Parser.Parse(s, "<div class=\"mr-2\">", "</div>");

        //            string tempDL = Parser.Parse(s, @"</div>
        //    </div>
        //</td>
        //<td>", "</td>").Replace(",", "").Trim();

        //            long dls = long.Parse(tempDL);
        //            string downloadLink = Parser.Parse(s, @"<div class=""mx-auto flex justify-center"">
        //            <a href=""", "\" ");
        //            var obj = new Download(release, title, fileSize, dateUploaded, gameVersion, dls, downloadLink);
        //            Console.WriteLine(obj.ToStringManyLines());
        //            //downloads.Add(new Download(release, title, fileSize, dateUploaded, gameVersion, dls, downloadLink));
        //            downloads.Add(obj);
        //        }
        //        return downloads;
        //    }

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

        public static List<CurseAddon> FromCursePageToCurseAddons(string page)
        {
            var addons = new List<CurseAddon>();
            var strings = Regex.Split(page, "<div class=\"project-listing-row").Skip(1).ToList();

            foreach (var s in strings)
            {
                var url = Parser.Parse(s, "<a href=\"/wow/addons/", "\"").Trim();
                var titleHtml = Parser.Parse(s, "<h3 class=\"text-primary-500 font-bold text-lg hover:no-underline\">", "</h3>").Trim();
                var title = System.Net.WebUtility.HtmlDecode(titleHtml);
                var descriptionHtml = Parser.Parse(s, "<p class=\"text-sm leading-snug\">", "</p>").Trim();
                var description = System.Net.WebUtility.HtmlDecode(descriptionHtml);

                var downloadsString = Parser.Parse(s, "<div class=\"flex my-1\">\r\n" +
                    "            <span class=\"mr-2 text-xs text-gray-500\">", " Downloads</span>").Trim();
                long downloads = Parser.FromStringDownloadToLong(downloadsString);

                var updatedString = Parser.Parse(s, "Updated <abbr", "</abbr></span>");
                var updatedEpochString = Parser.Parse(updatedString, "data-epoch=\"", "\"");
                var updated = long.Parse(updatedEpochString);

                var createdString = Parser.Parse(s, "Created <abbr", "</abbr></span>");
                var createdEpochString = Parser.Parse(createdString, "data-epoch=\"", "\"");
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

            return addons;
        }
    }
}