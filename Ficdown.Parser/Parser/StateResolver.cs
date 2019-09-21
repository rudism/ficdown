namespace Ficdown.Parser.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Model.Parser;
    using Model.Player;
    using Model.Story;

    internal class StateResolver : IStateResolver
    {
        private static Logger _logger = Logger.GetLogger<StateResolver>();
        private static readonly Random _random = new Random((int) DateTime.Now.Ticks);
        private readonly IDictionary<string, string> _pageNames;
        private readonly HashSet<string> _usedNames;
        private Story _story;

        public List<FicdownException> Warnings { private get; set; }

        public StateResolver()
        {
            _pageNames = new Dictionary<string, string>();
            _usedNames = new HashSet<string>();
        }

        public ResolvedStory Resolve(IEnumerable<PageState> pages, Story story)
        {
            _logger.Debug("Resolving story paths...");
            _story = story;
            return new ResolvedStory
            {
                Name = story.Name,
                Description = story.Description,
                FirstPage = GetPageNameForHash(pages.Single(p => p.Id.Equals(Guid.Empty)).CompressedHash),
                Pages = pages.Select(
                    page =>
                        new ResolvedPage
                        {
                            Name = GetPageNameForHash(page.CompressedHash),
                            Content = ResolveDescription(page),
                            ActiveToggles = GetStateDictionary(page).Where(t => t.Value).Select(t => t.Key)
                        }).ToList()
            };
        }

        private string ResolveAnchor(string blockName, int lineNumber, Anchor anchor, IDictionary<string, bool> playerState, string targetHash)
        {
            var text = anchor.Text;
            if (anchor.Href != null && anchor.Href.Conditions != null)
            {
                var satisfied = Utilities.GetInstance(Warnings, blockName, lineNumber).ConditionsMet(playerState, anchor.Href.Conditions);
                var alts = Utilities.GetInstance(Warnings, blockName, lineNumber).ParseConditionalText(anchor);
                if(alts != null)
                {
                    var replace = alts[satisfied];
                    text = RegexLib.EscapeChar.Replace(replace, string.Empty);
                }
            }
            return !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(targetHash)
                ? string.Format("[{0}](/{1})", text, GetPageNameForHash(targetHash))
                : text;
        }

        private string ResolveDescription(PageState page)
        {
            var resolved = new StringBuilder();
            if(!string.IsNullOrEmpty(page.Scene.Name))
                resolved.AppendFormat("## {0}\n\n", page.Scene.Name);

            var firstToggleCounter = 0;
            for (var i = 0; i < page.State.ActionsToShow.Count; i++)
            {
                if (page.State.ActionsToShow[i])
                {
                    var actionTuple = _story.Actions.Single(a => a.Value.Id == i + 1);
                    var actionAnchors = Utilities.GetInstance(Warnings, page.Scene.Name, page.Scene.LineNumber).ParseAnchors(actionTuple.Value.RawDescription);
                    var anchorDict = GetStateDictionary(page);
                    if (
                        actionAnchors.Any(
                            a => a.Href.Conditions != null && a.Href.Conditions.ContainsKey(actionTuple.Key)))
                    {
                        if (page.State.ActionFirstToggles[firstToggleCounter++])
                        {
                            anchorDict[actionTuple.Key] = false;
                        }
                    }
                    resolved.AppendFormat("{0}\n\n", actionAnchors.Aggregate(actionTuple.Value.Description,
                        (current, anchor) =>
                            current.Replace(anchor.Original,
                                ResolveAnchor(page.Scene.Name, page.Scene.LineNumber, anchor, anchorDict,
                                    page.Links.ContainsKey(anchor.Original) ? page.Links[anchor.Original] : null))));
                }
            }

            var anchors = Utilities.GetInstance(Warnings, page.Scene.Name, page.Scene.LineNumber).ParseAnchors(page.Scene.RawDescription);
            var stateDict = GetStateDictionary(page);
            var text =
                RegexLib.EmptyListItem.Replace(
                    anchors.Aggregate(page.Scene.Description,
                        (current, anchor) =>
                            current.Replace(anchor.Original,
                                ResolveAnchor(page.Scene.Name, page.Scene.LineNumber, anchor, stateDict,
                                    page.Links.ContainsKey(anchor.Original) ? page.Links[anchor.Original] : null))),
                    string.Empty);
            var seen = page.State.ScenesSeen[page.Scene.Id - 1];
            resolved.Append(!seen
                ? RegexLib.BlockQuoteToken.Replace(text, string.Empty)
                : RegexLib.BlockQuotes.Replace(text, string.Empty));
            return resolved.ToString();
        }

        public static IDictionary<string, bool> GetStateDictionary(PageState page)
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
