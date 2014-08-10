namespace Ficdown.Parser.Player
{
    using System.Collections.Generic;
    using Model.Player;
    using Model.Story;

    internal interface IGameTraverser
    {
        Story Story { get; set; }
        IEnumerable<PageState> Enumerate();
    }
}
