using System;
using System.Diagnostics;
using System.Linq;

namespace Addon.Core.Helpers
{
    public static class Util
    {
        private static readonly Random Random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static string Parse2(string input, string start, string end)
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
                Debug.WriteLine(nameof(input)+input+" ERROR: "+e.Message);
                return string.Empty;
            }
            
        }
    }
}
