using AddonManager.Core.Helpers;
using AddonManager.Core.Models;
using AddonManager.Core.Storage;
using AddonManager.Helpers;
using AddonToolkit.Model;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace AddonManager.Logic
{
    public static class Storage
    {
        private static readonly StorageFolder LOCAL_FOLDER = ApplicationData.Current.LocalFolder;
        // private static readonly StorageFolder APP_INSTALLED_FOLDER = Package.Current.InstalledLocation;

        public static async Task SaveSession()
        {
            try
            {
                var instance = Singleton<Session>.Instance.AsSaveableSession();
                await LOCAL_FOLDER.SaveAsync("session", instance);
                //var addonDataSet = new HashSet<AddonData>();
                //foreach (var game in instance.Games)
                //{
                //    var addonDataList = game.Addons
                //        .Where(saveableAddon =>
                //            !string.IsNullOrEmpty(saveableAddon.FolderName) &&
                //            !string.IsNullOrEmpty(saveableAddon.ProjectUrl))
                //        .Select(saveableAddon => saveableAddon.AsAddonData())
                //        .ToList();
                //    addonDataSet.UnionWith(addonDataList);
                //}
                //   await LOCAL_FOLDER.SaveAsync("addondata", addonDataSet);

                //    Debug.WriteLine("Saved Session to " + localFolder.Path);
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERROR when saveing session, " + e.Message);
            }
        }

        public static async Task LoadTask()
        {
            var session = Singleton<Session>.Instance;
            var saveableSession = await LOCAL_FOLDER.ReadAsync<SaveableSession>("session");
            if (saveableSession == null)
                return;

            //session.SelectedGame = saveableSession.SelectedGame.AsGame();
            session.Games.Clear();

            foreach (var saveableGame in saveableSession.Games)
            {
                var game = saveableGame.AsGame();

                foreach (var addon in game.Addons)
                {
                    if (addon.ProjectUrl.Contains("https://www.wowace") || addon.ProjectUrl.Contains("https://wow.curseforge"))
                    {
                        addon.ProjectUrl = string.Empty;
                        addon.Downloads = new List<Download>();
                    }
                }

                session.Games.Add(game);
                if (saveableSession.SelectedGame.AbsolutePath.Equals(game.AbsolutePath))
                {
                    session.SelectedGame = game;
                }
            }

            // Debug.WriteLine("Loaded from " + localFolder.Path);
        }

        //public static async Task<HashSet<string>> LoadKnownSubFolders()
        //{
        //    var assets = await APP_INSTALLED_FOLDER.GetFolderAsync("Assets");
        //    return await assets.ReadAsync<HashSet<string>>("knownsubfolders");
        //}

        public static async Task<HashSet<string>> LoadKnownSubFoldersFromUser()
        {
            var knownFolders = await LOCAL_FOLDER.ReadAsync<HashSet<string>>("knownsubfolders");
            return knownFolders;
        }

        public static async Task SaveKnownSubFolders()
        {
            try
            {
                var instance = Singleton<Session>.Instance.KnownSubFolders;
                await LOCAL_FOLDER.SaveAsync("knownsubfolders", instance);
                // Debug.WriteLine("Saved knownsubfolders");
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERROR when saveing knownsubfolders, " + e.Message);
            }
        }

        public static async Task<List<AddonData>> LoadAddonData()
        {
            using (var stream = await StreamHelper.GetPackagedFileStreamAsync("Assets/allvalidaddondata1-345.json"))
            {
                var fileContent = await stream.ReadTextAsync();
                var addonDatas = await Json.ToObjectAsync<List<AddonData>>(fileContent);
                addonDatas.Add(new AddonData()
                {
                    FolderName = "ElvUI",
                    Title = "ElvUI",
                    Description = "A user interface designed around user-friendliness with extra features that are not included in the standard UI.",
                    NrOfDownloads = 100000000,
                    UpdatedEpoch = 1557784800,
                    CreatedEpoch = 0,
                    ProjectName = "elvui",
                    //ProjectUrl = Version.ELVUI,
                    SubFolders = new HashSet<string>() { "ElvUI_Config", "ElvUI_OptionsUI" },
                    Files = 100,
                    Size = 100,
                    HasRetail = true
                });
                return addonDatas;
            }
            //var assets = await APP_INSTALLED_FOLDER.GetFolderAsync("Assets");
            //var addonDatas = await assets.ReadAsync<List<AddonData>>("allvalidaddondata1-340");
            //addonDatas.Add(new AddonData()
            //{
            //    FolderName = "ElvUI",
            //    Title = "ElvUI",
            //    Description = "A user interface designed around user-friendliness with extra features that are not included in the standard UI.",
            //    NrOfDownloads = 100000000,
            //    UpdatedEpoch = 1557784800,
            //    CreatedEpoch = 0,
            //    ProjectName = "elvui",
            //    ProjectUrl = Version.ELVUI,
            //    SubFolders = new HashSet<string>() { "ElvUI_Config" },
            //    Files = 100,
            //    Size = 100
            //});
            //return addonDatas;
        }
    }
}
