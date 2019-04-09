using Addon.Core.Helpers;
using Addon.Core.Models;
using Addon.Core.Storage;
using Addon.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Addon.Logic
{


    public static class Storage
    {

        // private static ConcurrentQueue<string> saveQueue = new ConcurrentQueue<string>();
        private static Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        //public static async Task SaveTask()
        //{
        //    Debug.WriteLine("nothing atm ");
        //    // saveQueue.Enqueue("asd");
        //}

        public static async Task SaveSession()
        {
            try
            {
                var instance = Singleton<Session>.Instance.AsSaveableSession();
                await localFolder.SaveAsync("session", instance);
                Debug.WriteLine("Saved Session to " + localFolder.Path);
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

            session.SelectedGame = saveableSession.SelectedGame.AsGame();
            session.Games.Clear();

            foreach (var saveableGame in saveableSession.Games)
            {
                session.Games.Add(saveableGame.AsGame());
            }
            //foreach (var game in session.Games)
            //{
            //    game.Addons.CollectionChanged += (a, b) => Debug.WriteLine("changed");
            //}

            Debug.WriteLine("Loaded from " + localFolder.Path);

            //var t = new Thread(() =>
            //{
            //    try
            //    {

            //        while (true)
            //        {
            //            Thread.Sleep(2000);
            //            if (saveQueue.TryDequeue(out var res))
            //            {
            //                SaveTask2();
            //            }
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Debug.WriteLine("[ABORTED] save thread. " + e.Message);

            //    }
            //});
            //t.Start();
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
                Debug.WriteLine("Saved knownsubfolders");
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERROR when saveing knownsubfolders, " + e.Message);

            }

        }

    }
}
