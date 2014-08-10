namespace Ficdown.Parser.Parser
{
    using System.Collections.Generic;
    using Model.Parser;
    using Model.Player;
    using Model.Story;

    internal interface IStateResolver
    {
        IEnumerable<ResolvedPage> Resolve(IEnumerable<PageState> pages, Story story);
    }
}
