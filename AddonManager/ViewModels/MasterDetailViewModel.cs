using AddonManager.Core.Helpers;
using AddonManager.Core.Models;
using AddonManager.Helpers;
using AddonManager.Logic;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AddonManager.ViewModels
{
    public class MasterDetailViewModel : Observable
    {
        private Core.Models.Addon oldSelected;

        public Session Session { get; }

        public AdvancedCollectionView Addons { get; set; }

        private Core.Models.Addon _selected;

        public Core.Models.Addon Selected
        {
            get { return _selected; }
            set
            {
                Set(ref _selected, value);
            }
        }
        
        public MasterDetailViewModel()
        {
            Session = Singleton<Session>.Instance;
            Addons = new AdvancedCollectionView(Session.SelectedGame.Addons);
            Addons.SortDescriptions.Add(new SortDescription("Status", SortDirection.Descending, StringComparer.OrdinalIgnoreCase));
            Addons.SortDescriptions.Add(new SortDescription("FolderName", SortDirection.Ascending, StringComparer.OrdinalIgnoreCase));
            PropertyChanged += MasterDetailViewModel_PropertyChanged;
        }

        private async void MasterDetailViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (e.PropertyName.Equals("Selected") && Selected != null)
            {
                if (string.IsNullOrEmpty(Selected.ChangeLog))
                {
                    //Debug.WriteLine("Download changes");
                    Selected.ChangeLog = await Changes.DownloadChangesFor(Selected);
                }
                oldSelected = Selected;
            }
            else if (e.PropertyName.Equals("Selected") && Selected == null)
            {
                // Debug.WriteLine("NULL selected");
                if (Session.SelectedGame != null && Session.SelectedGame.Addons.Count > 0 && oldSelected != null)
                {
                    if (oldSelected.Game == Session.SelectedGame)
                    {
                        Selected = oldSelected;
                    }

                }
            }
        }

        public async Task LoadDataAsync(MasterDetailsViewState viewState)
        {
            if (viewState == MasterDetailsViewState.Both && Session.SelectedGame != null && Session.SelectedGame.Addons.Count > 0)
            {
                Selected = Session.SelectedGame.Addons.First();
            }
            await Task.CompletedTask;
        }
    }
}
