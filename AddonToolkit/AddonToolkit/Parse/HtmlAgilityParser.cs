using AddonToolkit.Model;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace AddonToolkit.Parse
{
    public static class HtmlAgilityParser
    {
        public static List<CurseAddon> FromCursePageToCurseAddons(string page)
        {
            var addons = new List<CurseAddon>();

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(page);

            var divs = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class,'project-listing-row')]");

            foreach (var addonDiv in divs)
            {
                var link = addonDiv.SelectSingleNode(".//a");
                string url = link.Attributes["href"].Value.Replace("/wow/addons/", "").Trim();

                string title = System.Net.WebUtility.HtmlDecode(addonDiv.SelectSingleNode(".//h3[contains(@class,'text-primary-500') and contains(@class,'font-bold') and contains(@class,'text-lg')]").InnerText.Trim());

                string description = System.Net.WebUtility.HtmlDecode(addonDiv.SelectSingleNode(".//p[contains(@class,'text-sm') and contains(@class,'leading-snug')]").InnerText.Trim());

                var downloadString = addonDiv.SelectSingleNode(".//span[contains(text(),'Downloads')]").InnerText.Replace("Downloads", "").Trim();
                long downloads = Parser.FromStringDownloadToLong(downloadString);

                var updatedString = addonDiv.SelectSingleNode(".//span[contains(text(),'Updated')]/abbr")
                    .Attributes["data-epoch"].Value;
                long updated = long.Parse(updatedString);

                var createdString = addonDiv.SelectSingleNode(".//span[contains(text(),'Created')]/abbr")
                    .Attributes["data-epoch"].Value;
                long created = long.Parse(createdString);

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

        public static List<Download> FromCurseForgeToDownloads(string htmlPage)
        {
            var downloads = new List<Download>();

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlPage);

            var table = htmlDocument.DocumentNode.SelectSingleNode("//table");

            var rows = table.SelectNodes("tbody/tr");

            foreach (var row in rows)
            {
                var cells = row.SelectNodes("td");

                string release = Parser.RELEASE_TYPES[cells[0].InnerText.Trim()];

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
            }
            return downloads;
        }

        public static string FromCursePageToChanges(string htmlPage)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlPage);

            var changeDiv = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class,'user-content')]");

            var allLinks = changeDiv.SelectNodes(".//a");
            if (allLinks != null)
            {
                foreach (var link in allLinks)
                {
                    link.Remove();
                }
            }

            return changeDiv.InnerHtml;
        }
    }
}