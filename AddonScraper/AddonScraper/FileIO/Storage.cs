using AddonToolkit.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AddonScraper.FileIO
{
    internal static class Storage
    {
        internal static void SaveToFile(DirectoryInfo directory, List<CurseAddon> addons, int from, int to)
        {
            using (StreamWriter file = File.CreateText(directory.FullName + "/curseaddons" + from + "-" + to + ".json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, addons);
            }
        }

        internal static void SaveToFile(DirectoryInfo directory, List<AddonData> addonData, string fileTag, Formatting formatting, int from, int to)
        {
            using (StreamWriter file = File.CreateText(directory.FullName + "/" + fileTag + "addondata" + from + "-" + to + ".json"))
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
            if (!Directory.Exists(@".\out"))
            {
                return new List<AddonData>();
            }

            var outDir = new DirectoryInfo(@".\out");
            var mostRecent = outDir.GetDirectories().OrderByDescending(d => d.LastWriteTimeUtc).FirstOrDefault();

            if (mostRecent == null)
            {
                return new List<AddonData>();
            }

            var file = mostRecent.GetFiles("allvalidaddondata*").FirstOrDefault();
            var addons = new List<AddonData>();

            using (StreamReader streamReader = File.OpenText(file.FullName))
            {
                JsonSerializer serializer = new JsonSerializer();
                var loaded = (AddonData[])serializer.Deserialize(streamReader, typeof(AddonData[]));
                addons.AddRange(loaded);
            }

            return addons;
        }
    }
}