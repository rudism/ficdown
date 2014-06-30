namespace Ficdown.Parser.Engine
{
    using System.Text.RegularExpressions;

    internal static class Utilities
    {
        public static string NormalizeString(string raw)
        {
            return Regex.Replace(Regex.Replace(raw.ToLower(), @"^\W+|\W+$", string.Empty), @"\W+", "-");
        }
    }
}
