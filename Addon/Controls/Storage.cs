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
            var instance = Singleton<Session>.Instance;
            // TODO dra en copy till json friendly ny SaveableSession typ
            await localFolder.SaveAsync("session", instance);

            //StorageFile saveFile = await localFolder.CreateFileAsync("session.json", CreationCollisionOption.ReplaceExisting);
            //string sessionAsJson = await Json.StringifyAsync(Singleton<Session>.Instance);
            //await FileIO.WriteTextAsync(saveFile, sessionAsJson);
        }


    }
}
