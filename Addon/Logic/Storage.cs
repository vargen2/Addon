using Addon.Core.Helpers;
using Addon.Core.Models;
using Addon.Core.Storage;
using Addon.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Addon.Logic
{
    public static class Storage
    {
        private static Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        public static async Task SaveSession()
        {
            try
            {
                var instance = Singleton<Session>.Instance.AsSaveableSession();
                await localFolder.SaveAsync("session", instance);
                var addonDataSet = new HashSet<AddonData>();
                foreach (var game in instance.Games)
                {
                    var addonDataList = game.Addons
                        .Where(saveableAddon =>
                            !string.IsNullOrEmpty(saveableAddon.FolderName) &&
                            !string.IsNullOrEmpty(saveableAddon.ProjectUrl))
                        .Select(saveableAddon => saveableAddon.AsAddonData())
                        .ToList();
                    addonDataSet.UnionWith(addonDataList);
                }
                await localFolder.SaveAsync("addondata", addonDataSet);

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
            var saveableSession = await localFolder.ReadAsync<SaveableSession>("session");
            if (saveableSession == null)
                return;

            //session.SelectedGame = saveableSession.SelectedGame.AsGame();
            session.Games.Clear();

            foreach (var saveableGame in saveableSession.Games)
            {
                var game = saveableGame.AsGame();
                session.Games.Add(game);
                if (saveableSession.SelectedGame.AbsolutePath.Equals(game.AbsolutePath))
                {
                    session.SelectedGame = game;
                }
            }



            // Debug.WriteLine("Loaded from " + localFolder.Path);
        }

        public static async Task<HashSet<string>> LoadKnownSubFolders()
        {
            var assets = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
            return await assets.ReadAsync<HashSet<string>>("knownsubfolders");
        }


        public static async Task<HashSet<string>> LoadKnownSubFoldersFromUser()
        {
            var knownFolders = await localFolder.ReadAsync<HashSet<string>>("knownsubfolders");
            return knownFolders;
        }

        public static async Task SaveKnownSubFolders()
        {
            try
            {
                var instance = Singleton<Session>.Instance.KnownSubFolders;
                await localFolder.SaveAsync("knownsubfolders", instance);
                // Debug.WriteLine("Saved knownsubfolders");
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERROR when saveing knownsubfolders, " + e.Message);
            }
        }

        public static async Task<IList<StoreAddon>> LoadStoreAddons()
        {
            var packageFolder = Package.Current.InstalledLocation;
            var sampleFile = await packageFolder.GetFileAsync(@"Assets\curseaddons.txt");
            var text = await FileIO.ReadTextAsync(sampleFile);
            // TODO fix time
            IList<CurseAddon> curseAddons = await Json.ToObjectAsync<List<CurseAddon>>(@text);
            var storeAddons = curseAddons
                .Select(ca => new StoreAddon(ca.addonURL, ca.title, ca.description, ca.downloads, DateTime.Now, DateTime.Now))
                .ToList();

            storeAddons.Insert(0, new StoreAddon("elvui", "ElvUI", "A user interface designed around user-friendliness with extra features that are not included in the standard UI.", 0, DateTime.Now, DateTime.Now));
            return storeAddons;
        }

        public static async Task<List<AddonData>> LoadAddonData()
        {
            var assets = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
            return await assets.ReadAsync<List<AddonData>>("addondata");
        }
    }
}
