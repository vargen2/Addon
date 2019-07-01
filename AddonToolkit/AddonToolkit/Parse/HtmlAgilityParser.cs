using AddonToolkit.Model;
using AddonToolkit.Parse;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AddonToolkit.AddonToolkit.Parse
{
    public static class HtmlAgilityParser
    {
        public static List<Download> FromCurseForgeToDownloads(string htmlPage)
        {
            var downloads = new List<Download>();

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlPage);

            var table = htmlDocument.DocumentNode.SelectSingleNode("//table");

            var rows = table.SelectNodes("tbody/tr");
            // Console.WriteLine("rows: " + rows.Count);
            foreach (var row in rows)
            {
                var cells = row.SelectNodes("td");
                //    Console.WriteLine("cells: " + cells.Count);

                string release = cells[0].InnerText.Trim();

                string title = cells[1].InnerText.Trim();

                string fileSize = cells[2].InnerText.Trim();

                var date = cells[3].SelectSingleNode(".//abbr");
                var epoch = date.Attributes["data-epoch"].Value;
                var dateUploaded = Parser.SafeParseFromEpochString(epoch);

                string gameVersion = cells[4].InnerText.Trim();

                string dlsString = cells[5].InnerText.Replace(",", "").Trim();
                long dls = long.Parse(dlsString);

                var link = cells[6].SelectSingleNode(".//a");
                var downloadLink = link.Attributes["href"].Value;

                downloads.Add(new Download(release, title, fileSize, dateUploaded, gameVersion, dls, downloadLink));
                // Console.WriteLine(downloads.Last().ToStringManyLines());
            }

            return downloads;
        }
    }
}