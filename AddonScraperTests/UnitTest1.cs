using AddonScraper;
using AddonToolkit.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace AddonScraperTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestScrape()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var addons = await AddonScraper.Program.Scrape(httpClient, 1, 1, 1);
                Assert.IsNotNull(addons);
                Assert.IsTrue(addons.Count == 20);
                foreach (var addon in addons)
                {
                    Console.WriteLine(addon.ToString());
                }
            }
        }

        [TestMethod]
        public async Task TestScrape10to12()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var addons = await AddonScraper.Program.Scrape(httpClient, 1, 10, 12);
                Assert.IsNotNull(addons);
                Assert.IsTrue(addons.Count == 60);
            }
        }

        [TestMethod]
        public async Task TestDownloadPage1()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var page = await AddonScraper.Program.DownloadPage(httpClient, 1, 1);
                Assert.IsNotNull(page);
                Assert.IsTrue(page.Length > 170000);
            }
        }

        [TestMethod]
        public async Task TestDownloadPage300()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var page = await AddonScraper.Program.DownloadPage(httpClient, 1, 300);
                Assert.IsNotNull(page);
                Assert.IsTrue(page.Length > 170000);
            }
        }

        [TestMethod]
        public async Task TestFindProjectUrl()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var projectUrl = await AddonScraper.Pure.Version.FindProjectUrlFor(httpClient, "weakauras-2");
                Assert.IsNotNull(projectUrl);
                Assert.IsTrue(projectUrl.Equals("https://www.wowace.com/projects/weakauras-2"));
            }
        }

        [TestMethod]
        public async Task TestDownloadVersion()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var downloads = await AddonScraper.Pure.Version.DownloadVersionsFor(httpClient, "https://www.wowace.com/projects/weakauras-2");
                Assert.IsNotNull(downloads);
                Assert.AreEqual(25, downloads.Count);
                Assert.IsTrue(downloads[0].DownloadLink.Contains("/projects/weakauras-2/files/"), "Fail: " + downloads[0].DownloadLink);
            }
        }

        [TestMethod]
        public async Task TestDownloadFile()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var addonData = new AddonData() { ProjectName = "weakauras-2" };
                addonData.ProjectUrl = "https://www.wowace.com/projects/weakauras-2";
                var downloads = await AddonScraper.Pure.Version.DownloadVersionsFor(httpClient, addonData.ProjectUrl);
                addonData.Downloads = downloads;
                Assert.IsNotNull(downloads);
                Assert.AreEqual(25, downloads.Count);
                Assert.IsTrue(downloads[0].DownloadLink.Contains("/projects/weakauras-2/files/"), "Fail: " + downloads[0].DownloadLink);

                var zipFile = await AddonScraper.Update.DLWithHttpProgress(httpClient, "https://www.wowace.com/projects/weakauras-2", downloads[0]);
                Assert.IsTrue(File.Exists(zipFile));

                //  var folders =  AddonScraper.Update.UpdateAddon2(addonData, zipFile);
            }
        }

        [TestMethod]
        public async Task TestProcess()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var addonData = new AddonData() { ProjectName = "weakauras-2" };
                addonData.ProjectUrl = "https://www.wowace.com/projects/weakauras-2";
                var downloads = await AddonScraper.Pure.Version.DownloadVersionsFor(httpClient, addonData.ProjectUrl);
                addonData.Downloads = downloads;
                Assert.IsNotNull(downloads);
                Assert.AreEqual(25, downloads.Count);
                Assert.IsTrue(downloads[0].DownloadLink.Contains("/projects/weakauras-2/files/"), "Fail: " + downloads[0].DownloadLink);

                var result = await Program.ProccessAddonData(httpClient, addonData);
                Assert.IsTrue(result.Item1);
            }
        }
    }
}