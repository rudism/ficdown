namespace Ficdown.Parser.Model.Traverser
{
    using System.Collections.Generic;

    internal class StateQueueItem
    {
        public PageState Page { get; set; }
        public IList<State> AffectedStates { get; set; }
    }
}
