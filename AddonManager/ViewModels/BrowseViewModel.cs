using AddonManager.Core.Helpers;
using AddonManager.Core.Models;
using Microsoft.Toolkit.Uwp.UI;

namespace AddonManager.ViewModels
{
    public class BrowseViewModel : Observable
    {
        public Session Session { get; }
        public AdvancedCollectionView StoreAddons { get; set; }
        public SortDirection TitleSortDirection { get; set; }
        public SortDirection DownloadSortDirection { get; set; }
        public SortDirection StatusSortDirection { get; set; }
        public SortDirection UpdatedSortDirection { get; set; }
        public SortDirection CreatedSortDirection { get; set; }

        public BrowseViewModel()
        {
            Session = Singleton<Session>.Instance;
            StoreAddons = new AdvancedCollectionView(Session.StoreAddons);
        }
    }
}
