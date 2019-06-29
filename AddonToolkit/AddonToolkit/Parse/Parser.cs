using AddonToolkit.Model;
using System;
using System.Diagnostics;
using static AddonToolkit.Model.Enums;

namespace AddonToolkit.Parse
{
    public static class Parser
    {
        public static DateTime SafeParseFromEpochString(string epoch)
        {
            if (long.TryParse(epoch, out long result))
            {
                return DateTimeOffset.FromUnixTimeSeconds(result).UtcDateTime;
            }
            return DateTime.UtcNow;
        }

        public static string Parse(string input, string start, string end)
        {
            try
            {
                int startI = input.IndexOf(start) + start.Length;
                string mid = input.Substring(startI);
                int length = mid.IndexOf(end);
                return input.Substring(startI, length).Trim();
            }
            catch (Exception e)
            {
                Debug.WriteLine("[ERROR] " + nameof(input) + " " + input + " " + e.Message);
                return string.Empty;
            }

        }

        public static PROJECT_SITE FromProjectUrlToEnum(string projectUrl)
        {
            if (projectUrl.Contains("https://wow.curseforge.com/projects/"))
            {
                return PROJECT_SITE.CURSEFORGE;
            }
            else if (projectUrl.Contains("https://www.wowace.com/projects/"))
            {
                return PROJECT_SITE.WOWACE;
            }
            else if (projectUrl.Equals(Consts.ELVUI))
            {
                return PROJECT_SITE.ELVUI;
            }
            throw new ArgumentException("Couldn't match projectUrl for " + projectUrl);
        }
    }
}
