using Addon.Core.Helpers;
using Addon.Core.Models;
using Addon.Helpers;
using Microsoft.Toolkit.Uwp.UI;

namespace Addon.ViewModels
{
    public class BrowseViewModel : Observable
    {
        public Session Session { get; }
        public AdvancedCollectionView StoreAddons { get;set; }
        public SortDirection TitleSortDirection { get; set; }
        public SortDirection DownloadSortDirection { get; set; }
        public SortDirection StatusSortDirection { get; set; }
        
        public BrowseViewModel()
        {
            Session = Singleton<Session>.Instance;
            StoreAddons = new AdvancedCollectionView(Session.StoreAddons);            
        }
    }
}
