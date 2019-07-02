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
    }
}