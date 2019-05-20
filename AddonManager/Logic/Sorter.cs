using AddonManager.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AddonManager.Logic
{
    [Obsolete("Not used anymore, since masterdetailpage uses AdvancedViewCollection")]
    internal static class Sorter
    {

        public static async Task Sort(ObservableCollection<Core.Models.Addon> addons)
        {
            var shouldSort = await Task.Run(() => ShouldSort(addons));
            if (shouldSort)
            {
                SortObservableCollection(addons);
            }
            await Task.CompletedTask;
        }

        public static async Task Sort(Game game)
        {
            await Sort(game.Addons);
        }

        private static bool ShouldSort(ObservableCollection<Core.Models.Addon> addons)
        {
            if (addons.Count == 0)
                return false;

            var count = addons.Where(a => a.IsUpdateable).Count();
            if (count == 0)
                return false;

            var tempList = new ObservableCollection<Core.Models.Addon>(addons);
            SortObservableCollection(tempList);
            return !tempList.SequenceEqual(addons);
        }

        private static void SortObservableCollection(ObservableCollection<Core.Models.Addon> addons)
        {
            var count = addons.Where(a => a.IsUpdateable).Count();
            for (int i = 0; i < count; i++)
            {
                var addon = addons.LastOrDefault(a => a.IsUpdateable);
                if (addon == null)
                {
                    return;
                }
                var moveFrom = addons.IndexOf(addon);
                addons.Move(moveFrom, 0);
            }

        }

    }
}
