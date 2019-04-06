using System;
using System.Collections.Generic;
using System.Linq;
using Addon.Core.Helpers;
using Addon.Core.Models;

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
            /*
             

            String subString = data.substring(matcher.start());
            String temp = Util.parse(subString, "<td class=\"project-file-release-type\">", "</td>");
            String release = Util.parse(temp, "title=\"", "\"></div>");
            String title = Util.parse(subString, "data-name=\"", "\">");
            String fileSize = Util.parse(subString, "<td class=\"project-file-size\">", "</td>").trim();
            String a = Util.parse(subString, "data-epoch=\"", "\"");
            LocalDateTime fileDateUploaded = LocalDateTime.ofEpochSecond(Integer.parseInt(a), 0, OffsetDateTime.now().getOffset());
            String gameVersion = Util.parse(subString, "<span class=\"version-label\">", "</span>");
            long dls = Long.valueOf(Util.parse(subString, "<td class=\"project-file-downloads\">", "</td>").replaceAll(",", "").trim());
            String downloadLink = Util.parse(subString, " href=\"", "\"");
                         */
                         
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
    }
}
