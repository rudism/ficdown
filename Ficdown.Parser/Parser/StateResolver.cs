namespace Ficdown.Parser.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Model.Parser;
    using Model.Traverser;

    internal class StateResolver : IStateResolver
    {
        private static readonly Random _random = new Random((int) DateTime.Now.Ticks);
        private readonly IDictionary<string, string> _pageNames;
        private readonly HashSet<string> _usedNames;

        public StateResolver()
        {
            _pageNames = new Dictionary<string, string>();
            _usedNames = new HashSet<string>();
        }

        public IEnumerable<ResolvedPage> Resolve(IEnumerable<PageState> pages)
        {
            return
                pages.Select(
                    page =>
                        new ResolvedPage
                        {
                            Name = GetPageNameForHash(page.CompressedHash),
                            Content = ResolveDescription(page)
                        }).ToList();
        }

        private string ResolveAnchor(Anchor anchor, IDictionary<string, bool> playerState, string targetHash)
        {
            var text = anchor.Text;
            if (anchor.Href.Conditions != null)
            {
                var satisfied = Utilities.ConditionsMet(playerState, anchor.Href.Conditions);
                var alts = Utilities.ParseConditionalText(text);
                var replace = alts[satisfied];
                text = RegexLib.EscapeChar.Replace(replace, string.Empty);
            }
            return !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(targetHash)
                ? string.Format("[{0}](/{1})", text, GetPageNameForHash(targetHash))
                : text;
        }

        private string ResolveDescription(PageState page)
        {
            var resolved = page.Scene.Description;

            var anchors = Utilities.ParseAnchors(resolved);

            resolved = RegexLib.EmptyListItem.Replace(anchors.Aggregate(resolved,
                (current, anchor) =>
                    current.Replace(anchor.Original,
                        ResolveAnchor(anchor, GetStateDictionary(page),
                            page.Links.ContainsKey(anchor.Original) ? page.Links[anchor.Original] : null))),
                string.Empty);

            var seen = page.State.ScenesSeen[page.Scene.Id - 1];
            resolved = !seen
                ? RegexLib.BlockQuoteToken.Replace(resolved, string.Empty)
                : RegexLib.BlockQuotes.Replace(resolved, string.Empty);
            return resolved;
        }

        private IDictionary<string, bool> GetStateDictionary(PageState page)
        {
            return page.StateMatrix.Where(matrix => page.State.PlayerState[matrix.Value])
                .ToDictionary(m => m.Key, m => true);
        }

        private string GetPageNameForHash(string hash)
        {
            if (!_pageNames.ContainsKey(hash))
            {
                string name;
                do
                {
                    name = RandomString(8);
                } while (_usedNames.Contains(name));
                _pageNames.Add(hash, name);
                _usedNames.Add(name);
            }
            return _pageNames[hash];
        }

        private string RandomString(int size)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26*_random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }
    }
}