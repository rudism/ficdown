namespace Ficdown.Parser.Parser
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Model.Parser;

    internal class Utilities
    {
        private List<FicdownException> _warnings { get; set; }

        public static Utilities GetInstance()
        {
            return new Utilities
            {
                _warnings = new List<FicdownException>(),
                _blockName = string.Empty
            };
        }

        public static Utilities GetInstance(List<FicdownException> warnings, string blockName, int lineNumber)
        {
            return new Utilities
            {
                _warnings = warnings,
                _blockName = blockName,
                _lineNumber = lineNumber
            };
        }

        public static Utilities GetInstance(List<FicdownException> warnings, string blockName)
        {
            return new Utilities
            {
                _warnings = warnings,
                _blockName = blockName
            };
        }

        protected string _blockName;
        protected int? _lineNumber;

        public string NormalizeString(string raw)
        {
            return Regex.Replace(Regex.Replace(raw.ToLower(), @"^\W+|\W+$", string.Empty), @"\W+", "-");
        }

        private Href ParseHref(string href, int lineNumber, int colNumber)
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
            _warnings.Add(new FicdownException(_blockName, string.Format("Invalid href: {0}", href), lineNumber, colNumber));
            return null;
        }

        public Anchor ParseAnchor(string anchorText, int lineNumber, int colNumber)
        {
            var match = RegexLib.Anchors.Match(anchorText);
            if (!match.Success)
            {
                _warnings.Add(new FicdownException(_blockName, string.Format("Invalid anchor: {0}", anchorText), lineNumber, colNumber));
                return null;
            }
            return MatchToAnchor(match, lineNumber, colNumber);
        }

        private void PosFromIndex(string text, int index, out int line, out int col)
        {
            line = 1;
            col = 1;
            for (int i = 0; i <= index - 1; i++)
            {
                col++;
                if (text[i] == '\n')
                {
                    line++;
                    col = 1;
                }
            }
        }

        public IList<Anchor> ParseAnchors(string text)
        {
            var matches = RegexLib.Anchors.Matches(text);
            return matches.Cast<Match>().Select(m =>
            {
                int line, col;
                PosFromIndex(text, m.Index, out line, out col);
                if(_lineNumber.HasValue) line += _lineNumber.Value;
                return MatchToAnchor(m, line, col);
            }).ToList();
        }

        private Anchor MatchToAnchor(Match match, int lineNumber, int colNumber)
        {
            var astr = match.Groups["anchor"].Value;
            var txstr = match.Groups["text"].Value;
            var ttstr = match.Groups["title"].Success
                ? match.Groups["title"].Value
                : null;
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
                Title = ttstr,
                Href = ParseHref(hrefstr, lineNumber, colNumber),
                LineNumber = lineNumber,
                ColNumber = colNumber
            };
        }

        public IDictionary<bool, string> ParseConditionalText(Anchor anchor)
        {
            var match = RegexLib.ConditionalText.Match(anchor.Text);
            if (!match.Success)
            {
                _warnings.Add(new FicdownException(_blockName, string.Format(@"Invalid conditional text: {0}", anchor.Text), anchor.LineNumber, anchor.ColNumber));
                return null;
            }

            return new Dictionary<bool, string>
            {
                {true, match.Groups["true"].Value},
                {false, match.Groups["false"].Value}
            };
        }

        public bool ConditionsMet(IDictionary<string, bool> playerState, IDictionary<string, bool> conditions)
        {
            return
                conditions.All(
                    c =>
                        (!c.Value && (!playerState.ContainsKey(c.Key) || !playerState[c.Key])) ||
                        (c.Value && (playerState.ContainsKey(c.Key) && playerState[c.Key])));
        }
    }
}
