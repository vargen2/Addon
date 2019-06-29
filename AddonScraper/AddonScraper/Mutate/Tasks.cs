using AddonToolkit.Model;
using NLog;
using System.Net.Http;
using System.Threading.Tasks;
using AddonScraper.Pure;
using System.Collections.Generic;

namespace AddonScraper.Mutate
{
    public static class Tasks
    {
      
        //public static async Task<(string,List<Download>)> FindProjectUrlAndDownLoadVersionsFor(HttpClient httpClient, AddonData addon)
        //{
        //    if (string.IsNullOrEmpty(addon.ProjectUrl))
        //    {
        //        addon.ProjectUrl = await Task.Run(() => Version.FindProjectUrlFor(httpClient, addon.ProjectName));
        //    }
        //    addon.Downloads = await Task.Run(() => Version.DownloadVersionsFor(httpClient, addon.ProjectUrl));
        
        //}



        //public static async Task UpdateAddon(AddonData addon, Download download)
        //{


        //    var file = await Task.Run(() => Update.DLWithHttpProgress(addon, download));

        //    if (file == null)
        //    {

        //        return;
        //    }



        //    //if (addon.ProjectUrl.Equals(Version.ELVUI))
        //    //{
        //    //    var trash = await Task.Run(() => Update.UpdateAddonOld(addon, download, file));
        //    //    addon.CurrentDownload = download;
        //    //    await Update.AddSubFolders(addon, trash.Item2);

        //    //    addon.Message = string.Empty;
        //    //    await Task.Run(() => Update.Cleanup(file.Name, trash.Item1));
        //    //}
        //    //else
        //    //{


        //        var subFolders = await Task.Run(() => Update.UpdateAddon2(addon, file));
        //       // addon.CurrentDownload = download;
        //        await Update.AddSubFolders(addon, subFolders);

        //        //addon.Message = string.Empty;
        //        await Task.Run(() => Update.Cleanup2(file.Name));
        //    //}
        //}

        ////internal static async Task Remove(AddonData addon)
        ////{
        ////    addon.Game.Addons.Remove(addon);
        ////    await Task.Run(() => Update.RemoveFilesFor(addon));
        ////    //Debug.WriteLine("Remove done for " + addon.FolderName);
        ////}

    }
}
