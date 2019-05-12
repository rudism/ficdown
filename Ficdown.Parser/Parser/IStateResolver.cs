namespace Ficdown.Parser.Parser
{
    using System.Collections.Generic;
    using Model.Parser;
    using Model.Player;
    using Model.Story;

    internal interface IStateResolver
    {
        List<FicdownException> Warnings { set; }
        ResolvedStory Resolve(IEnumerable<PageState> pages, Story story);
    }
}
