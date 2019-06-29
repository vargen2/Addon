using AddonToolkit.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace AddonScraper.FileIO
{
    internal static class Storage
    {

        internal static void SaveToFile(List<CurseAddon> addons, int from, int to)
        {
            using (StreamWriter file = File.CreateText(@".\out\curseaddons" + from + "-" + to + ".json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, addons);
            }

        }

        internal static void SaveToFile(List<AddonData> addonData, string fileTag, Formatting formatting, int from, int to)
        {
            using (StreamWriter file = File.CreateText(@".\out\" + fileTag + "addondata" + from + "-" + to + ".json"))
            {
                JsonSerializer serializer = new JsonSerializer
                {
                    Formatting = formatting
                };
                serializer.Serialize(file, addonData);
            }

        }

        internal static List<AddonData> LoadAddonData()
        {
            var files = Directory.GetFiles(@".\Data");
            var addons = new List<AddonData>();

            foreach (var file in files)
            {
                using (StreamReader streamReader = File.OpenText(file))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    var loaded = (AddonData[])serializer.Deserialize(streamReader, typeof(AddonData[]));
                    addons.AddRange(loaded);
                }
            }
            return addons;

        }


    }
}
