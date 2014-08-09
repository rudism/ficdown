namespace Ficdown.Parser.Player
{
    using System.Collections.Generic;
    using Model.Story;
    using Model.Traverser;

    internal interface IGameTraverser
    {
        Story Story { get; set; }
        IEnumerable<PageState> Enumerate();
    }
}
