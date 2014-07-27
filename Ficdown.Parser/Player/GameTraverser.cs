namespace Ficdown.Parser.Player
{
    using System.Collections.Generic;
    using System.Linq;
    using Model.Story;
    using Model.Traverser;
    using Parser;

    internal class GameTraverser
    {
        private readonly StateManager _manager;
        private readonly Queue<PageState> _processingQueue;
        private readonly IDictionary<string, PageState> _processed;

        public GameTraverser(Story story)
        {
            _manager = new StateManager(story);
            _processingQueue = new Queue<PageState>();
            _processed = new Dictionary<string, PageState>();
        }

        public IEnumerable<PageState> Enumerate()
        {
            // generate comprehensive enumeration

            _processingQueue.Enqueue(_manager.InitialState);
            while (_processingQueue.Count > 0)
            {
                var state = _processingQueue.Dequeue();
                if (!_processed.ContainsKey(state.UniqueHash))
                {
                    _processed.Add(state.UniqueHash, state);
                    ProcessState(state);
                }
            }

            // compress redundancies


            return _processed.Values;
        }

        private void ProcessState(PageState currentState)
        {
            var states = new HashSet<string>();
            foreach (
                var anchor in
                    Utilities.ParseAnchors(currentState.Scene.Description)
                        .Where(a => a.Href.Target != null || a.Href.Toggles != null))
            {
                var newState = _manager.ResolveNewState(anchor, currentState);
                if (!currentState.Links.ContainsKey(anchor.Original))
                    currentState.Links.Add(anchor.Original, newState.UniqueHash);
                if (!states.Contains(newState.UniqueHash) && !_processed.ContainsKey(newState.UniqueHash))
                {
                    states.Add(newState.UniqueHash);
                    _processingQueue.Enqueue(newState);
                }
            }
        }
    }
}