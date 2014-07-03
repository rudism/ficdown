namespace Ficdown.Parser.Parser
{
    using System.Collections.Generic;

    public class StateResolver : IStateResolver
    {
        public string Resolve(string description, IDictionary<string, bool> playerState, bool firstSeen)
        {
            foreach (var anchor in Utilities.ParseAnchors(description))
            {
                if (anchor.Href.Conditions != null)
                {
                    var satisfied = Utilities.ConditionsMet(playerState, anchor.Href.Conditions);
                    var alts = Utilities.ParseConditionalText(anchor.Text);
                    var replace = alts[satisfied];
                    replace = RegexLib.EscapeChar.Replace(replace, string.Empty);
                    if (!replace.Equals(string.Empty) || (anchor.Href.Toggles == null && anchor.Href.Target == null))
                    {
                        description = description.Replace(anchor.Original, replace);
                        description = RegexLib.EmptyListItem.Replace(description, string.Empty);
                    }
                    else
                    {
                        var newAnchor = string.Format(@"[{0}]({1}{2})", replace,
                            (anchor.Href.Target != null ? string.Format(@"/{0}", anchor.Href.Target) : null),
                            anchor.Href.Toggles.ToHrefString("+"));
                        description = description.Replace(anchor.Original, newAnchor);
                    }
                }
            }
            return firstSeen
                ? RegexLib.BlockQuoteToken.Replace(description, string.Empty)
                : RegexLib.BlockQuotes.Replace(description, string.Empty);
        }
    }
}
