namespace Ficdown.Parser.Parser
{
    using System.Collections.Generic;
    using Model.Parser;
    using Model.Traverser;

    internal interface IStateResolver
    {
        IEnumerable<ResolvedPage> Resolve(IEnumerable<PageState> pages);
    }
}
