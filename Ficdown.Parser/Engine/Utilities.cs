using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Ficdown.Parser.Tests")]

namespace Ficdown.Parser.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal static class Utilities
    {
        public static string NormalizeString(string raw)
        {
            return Regex.Replace(Regex.Replace(raw.ToLower(), @"^\W+|\W+$", string.Empty), @"\W+", "-");
        }

        public static void ParseHref(string href, out string target)
        {
            IList<string> conditions, toggles;
            ParseHref(href, out target, out conditions, out toggles);
            if(conditions != null || toggles != null) throw new FormatException();
        }

        public static void ParseHref(string href, out IList<string> conditions)
        {
            string target;
            IList<string> toggles;
            ParseHref(href, out target, out conditions, out toggles);
            if(target != null || toggles != null) throw new FormatException();
        }

        public static void ParseHref(string href, out string target, out IList<string> conditions, out IList<string> toggles)
        {
            target = null;
            conditions = null;
            toggles = null;
            var match = RegexLib.Href.Match(href);
            if (match.Success)
            {
                var ttstr = match.Groups["target"].Value;
                var cstr = match.Groups["conditions"].Value;
                var tstr = match.Groups["toggles"].Value;
                if (!string.IsNullOrEmpty(ttstr))
                    target = ttstr.TrimStart('/');
                if (!string.IsNullOrEmpty(cstr))
                    conditions = new List<string>(cstr.TrimStart('?').Split('&').Select(c => c.Trim().ToLower()));
                if (!string.IsNullOrEmpty(tstr))
                    toggles = new List<string>(tstr.TrimStart('#').Split('+').Select(t => t.Trim().ToLower()));
            }
            else throw new FormatException(string.Format("Invalid href: {0}", href));
        }
    }
}
