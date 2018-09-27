namespace Ficdown.Parser.Player
{
    using System.Collections.Generic;
    using Model.Parser;
    using Model.Player;
    using Model.Story;

    internal interface IGameTraverser
    {
        List<FicdownException> Warnings { set; }
        Story Story { get; set; }
        IEnumerable<PageState> Enumerate();
        IEnumerable<Scene> OrphanedScenes { get; }
        IEnumerable<Action> OrphanedActions { get; }
    }
}
