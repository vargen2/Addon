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
            //try
            //{
            int startI = input.IndexOf(start) + start.Length;
            string mid = input.Substring(startI);
            int length = mid.IndexOf(end);
            return input.Substring(startI, length).Trim();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("[ERROR] " + nameof(input) + " " + input + " " + e.Message);
            //    return string.Empty;
            //}
        }

        public static PROJECT_SITE FromProjectUrlToEnum(string projectUrl)
        {
            if (projectUrl.Contains("https://wow.curseforge.com/projects/"))
            {
                return PROJECT_SITE.CURSEFORGE;
            }
            else if (projectUrl.Equals(Consts.ELVUI))
            {
                return PROJECT_SITE.ELVUI;
            }
            throw new ArgumentException("Couldn't match projectUrl for " + projectUrl);
        }

        public static long FromStringDownloadToLong(string number)
        {
            long multiplier = GetMultiplier(number);
            string withOutLetter = RemoveEndLetter(number).Replace(".", ",");
            return (long)(double.Parse(withOutLetter) * multiplier);
        }

        private static long GetMultiplier(string number)
        {
            if (number.Contains("M"))
            {
                return 1000000;
            }
            if (number.Contains("K"))
            {
                return 1000;
            }
            return 1;
        }

        private static string RemoveEndLetter(string number)
        {
            if (number.Contains("M") || number.Contains("K"))
            {
                return number.Substring(0, number.Length - 1);
            }
            return number;
        }
    }
}