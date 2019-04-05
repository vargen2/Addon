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
            Debug.WriteLine(localFolder.Path);
            var instance = Singleton<Session>.Instance.AsSaveableSession();
            await localFolder.SaveAsync("session", instance);
        }

        public static async Task LoadTask()
        {
            var saveableSession = await localFolder.ReadAsync<SaveableSession>("session");
            Singleton<Session>.Instance.SelectedGame = saveableSession.SelectedGame.AsGame();
            Singleton<Session>.Instance.Games.Clear();

            foreach (var saveableGame in saveableSession.Games)
            {
                Singleton<Session>.Instance.Games.Add(saveableGame.AsGame());
            }
            Debug.WriteLine("Load Done: "+localFolder.Path);
        }

    }
}
