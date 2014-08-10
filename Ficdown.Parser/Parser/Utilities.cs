
namespace Ficdown.Parser.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Model.Parser;

    internal static class Utilities
    {
        public static string NormalizeString(string raw)
        {
            return Regex.Replace(Regex.Replace(raw.ToLower(), @"^\W+|\W+$", string.Empty), @"\W+", "-");
        }

        private static Href ParseHref(string href)
        {
            var match = RegexLib.Href.Match(href);
            if (match.Success)
            {
                var ttstr = match.Groups["target"].Value;
                var cstr = match.Groups["conditions"].Value;
                var tstr = match.Groups["toggles"].Value;
                return new Href
                {
                    Original = href,
                    Target = !string.IsNullOrEmpty(ttstr) ? ttstr.TrimStart('/') : null,
                    Conditions =
                        !string.IsNullOrEmpty(cstr)
                            ? new List<string>(cstr.TrimStart('?').Split('&').Select(c => c.Trim().ToLower()))
                                .ToDictionary(c => c.TrimStart('!'), c => !c.StartsWith("!"))
                            : null,
                    Toggles =
                        !string.IsNullOrEmpty(tstr)
                            ? new List<string>(tstr.TrimStart('#').Split('+').Select(t => t.Trim().ToLower())).ToArray()
                            : null
                };
            }
            throw new FormatException(string.Format("Invalid href: {0}", href));
        }

        public static Anchor ParseAnchor(string anchorText)
        {
            var match = RegexLib.Anchors.Match(anchorText);
            if (!match.Success) throw new FormatException(string.Format("Invalid anchor: {0}", anchorText));
            return MatchToAnchor(match);
        }

        public static IList<Anchor> ParseAnchors(string text)
        {
            var matches = RegexLib.Anchors.Matches(text);
            return matches.Cast<Match>().Select(MatchToAnchor).ToList();
        }

        private static Anchor MatchToAnchor(Match match)
        {
            var astr = match.Groups["anchor"].Value;
            var txstr = match.Groups["text"].Value;
            var ttstr = match.Groups["title"].Value;
            var hrefstr = match.Groups["href"].Value;
            if (hrefstr.StartsWith(@""""))
            {
                ttstr = hrefstr.Trim('"');
                hrefstr = string.Empty;
            }
            return new Anchor
            {
                Original = !string.IsNullOrEmpty(astr) ? astr : null,
                Text = !string.IsNullOrEmpty(txstr) ? txstr : null,
                Title = !string.IsNullOrEmpty(ttstr) ? ttstr : null,
                Href = ParseHref(hrefstr)
            };
        }

        public static IDictionary<bool, string> ParseConditionalText(string text)
        {
            var match = RegexLib.ConditionalText.Match(text);
            if (!match.Success) throw new FormatException(string.Format(@"Invalid conditional text: {0}", text));
            return new Dictionary<bool, string>
            {
                {true, match.Groups["true"].Value},
                {false, match.Groups["false"].Value}
            };
        }

        public static string ToHrefString(this IDictionary<string, bool> values, string separator)
        {
            return values != null
                ? string.Join(separator,
                    values.Where(v => !v.Key.StartsWith(">"))
                        .Select(v => string.Format("{0}{1}", v.Value ? null : "!", v.Key))
                        .ToArray())
                : null;
        }

        public static string ToHrefString(this IEnumerable<string> values, string separator)
        {
            return values != null ? string.Join(separator, values.ToArray()) : null;
        }

        public static bool ConditionsMet(IDictionary<string, bool> playerState, IDictionary<string, bool> conditions)
        {
            return
                conditions.All(
                    c =>
                        (!c.Value && (!playerState.ContainsKey(c.Key) || !playerState[c.Key])) ||
                        (c.Value && (playerState.ContainsKey(c.Key) && playerState[c.Key])));
        }
    }
}
