using System;
using System.Collections.ObjectModel;
using Addon.Core.Helpers;
using Addon.Core.Models;
using Addon.Helpers;

namespace Addon.ViewModels
{
    public class BrowseViewModel : Observable
    {
        public ObservableCollection<StoreAddon> StoreAddons { get; set; }

        public BrowseViewModel()
        {
            StoreAddons = Singleton<Session>.Instance.StoreAddons;
        }
    }
}
