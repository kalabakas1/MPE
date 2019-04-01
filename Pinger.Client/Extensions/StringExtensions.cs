using System.Text.RegularExpressions;

namespace Pinger.Client.Extensions
{
    internal static class StringExtensions
    {
        public static string RemoveSpecialCharacters(this string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }
    }
}
