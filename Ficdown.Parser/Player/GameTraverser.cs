namespace Ficdown.Parser.Player
{
    using System.Collections.Generic;
    using System.Linq;
    using Model.Story;
    using Model.Traverser;
    using Parser;

    internal class GameTraverser : IGameTraverser
    {
        private StateManager _manager;
        private Queue<StateQueueItem> _processingQueue;
        private IDictionary<string, PageState> _processed;
        private IDictionary<string, PageState> _compressed;

        private Story _story;
        public Story Story
        {
            get { return _story; }
            set
            {
                _story = value;
                _manager = new StateManager(_story);
                _processingQueue = new Queue<StateQueueItem>();
                _processed = new Dictionary<string, PageState>();
                _compressed = new Dictionary<string, PageState>();
            }
        }

        public IEnumerable<PageState> Enumerate()
        {
            // generate comprehensive enumeration

            var initial = _manager.InitialState;
            _processingQueue.Enqueue(new StateQueueItem
            {
                Page = initial,
                AffectedStates = new List<State> {initial.AffectedState}
            });
            while (_processingQueue.Count > 0)
            {
                var state = _processingQueue.Dequeue();
                if (!_processed.ContainsKey(state.Page.UniqueHash))
                {
                    _processed.Add(state.Page.UniqueHash, state.Page);
                    ProcessState(state);
                }
            }

            // compress redundancies
            foreach (var row in _processed)
            {
                if (!_compressed.ContainsKey(row.Value.CompressedHash))
                {
                    var scene = row.Value;
                    var links = scene.Links.Keys.ToArray();
                    foreach (var link in links)
                    {
                        scene.Links[link] = _processed[scene.Links[link]].CompressedHash;
                    }
                    _compressed.Add(row.Value.CompressedHash, row.Value);
                }
            }

            return _compressed.Values;
        }

        private void ProcessState(StateQueueItem currentState)
        {
            var states = new HashSet<string>();

            var anchors = Utilities.ParseAnchors(currentState.Page.Scene.Description);
            var conditionals =
                anchors.SelectMany(
                    a => a.Href.Conditions != null ? a.Href.Conditions.Select(c => c.Key) : new string[] {})
                    .Distinct()
                    .ToArray();
            var hasFirstSeen = RegexLib.BlockQuotes.IsMatch(currentState.Page.Scene.Description);

            foreach (var affected in currentState.AffectedStates)
            {
                // signal to previous scenes that this scene's used conditionals are important
                foreach (var conditional in conditionals) _manager.ToggleStateOn(affected, conditional);

                // signal to previous scenes if this scene has first-seen text
                if (hasFirstSeen) _manager.ToggleSeenSceneOn(affected, currentState.Page.Scene.Id);
            }

            foreach (var anchor in anchors.Where(a => a.Href.Target != null || a.Href.Toggles != null))
            {
                var newState = _manager.ResolveNewState(anchor, currentState.Page);
                if (!currentState.Page.Links.ContainsKey(anchor.Original))
                    currentState.Page.Links.Add(anchor.Original, newState.UniqueHash);

                if (!states.Contains(newState.UniqueHash) && !_processed.ContainsKey(newState.UniqueHash))
                {
                    states.Add(newState.UniqueHash);
                    var newAffected = new List<State>(currentState.AffectedStates);
                    newAffected.Add(newState.AffectedState);
                    _processingQueue.Enqueue(new StateQueueItem {Page = newState, AffectedStates = newAffected});
                }
            }
        }
    }
}