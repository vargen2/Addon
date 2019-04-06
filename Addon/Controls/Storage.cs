using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Addon.Core.Helpers;
using Addon.Core.Models;
using Addon.Core.Storage;
using Addon.Helpers;

namespace Addon.Controls
{
    public static class Storage
    {
        //private ConcurrentQueue<Task> saveQueue = new ConcurrentQueue<Task>();
        private static Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        public static async Task SaveTask()
        {
            try
            {
                Debug.WriteLine(localFolder.Path);
                var instance = Singleton<Session>.Instance.AsSaveableSession();
                await localFolder.SaveAsync("session", instance);
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERROR when saveing, "+e.Message);
                
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
            Debug.WriteLine("Load Done: " + localFolder.Path);
        }

    }
}
