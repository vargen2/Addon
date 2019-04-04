
using System;
using Addon.Core.Models;
using Addon.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var addon = new Addon.Core.Models.Addon(new Game(@"C:\Games\Wow\Interface\AddOns"), "AtlasLoot",
                    @"C:\Games\Wow\Interface\AddOns\AtlasLoot")
            { ProjectUrl = @"https://www.wowace.com/projects/atlasloot-enhanced" };
            var aa = AppHelper.DownloadVersionsFor(addon).Result;
            Assert.Equals(aa.Count, 0);
        }
    }
}
