using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Addon.Core.Helpers;
using Addon.Core.Models;
using static System.String;

namespace Addon.Helpers
{
    public static class Parse
    {

        public static List<Download> FromWowaceProjectsFilesToDownloads(string htmlPage)
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
    }
}
