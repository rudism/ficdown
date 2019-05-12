namespace Ficdown.Parser.Player
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Model.Parser;
    using Model.Player;
    using Model.Story;
    using Parser;
    using Action = Model.Story.Action;

    internal class GameTraverser : IGameTraverser
    {
        private StateManager _manager;
        private Queue<StateQueueItem> _processingQueue;
        private IDictionary<string, PageState> _processed;
        private IDictionary<string, PageState> _compressed;
        private IDictionary<int, Action> _actionMatrix;
        private bool _wasRun = false;

        public List<FicdownException> Warnings { private get; set; }

        private Story _story;
        public Story Story
        {
            get { return _story; }
            set
            {
                _story = value;
                _actionMatrix = _story.Actions.ToDictionary(a => a.Value.Id, a => a.Value);
                _manager = new StateManager(_story, Warnings);
                _processingQueue = new Queue<StateQueueItem>();
                _processed = new Dictionary<string, PageState>();
                _compressed = new Dictionary<string, PageState>();
            }
        }

        public IEnumerable<Scene> OrphanedScenes
        {
            get
            {
                if(!_wasRun) throw new Exception("Call Enumerate() before getting orphans");
                return _story.Scenes.SelectMany(l => l.Value, (l, s) => s).Where(s => !s.Visited);
            }
        }

        public IEnumerable<Action> OrphanedActions
        {
            get
            {
                if(!_wasRun) throw new Exception("Call Enumerate() before getting orphans");
                return _actionMatrix.Values.Where(a => !a.Visited);
            }
        }

        public IEnumerable<PageState> Enumerate()
        {
            if(_wasRun) throw new Exception("Can't call Enumerate() more than once");
            _wasRun = true;

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

            // make sure every page gets affected data on every page that it links to
            foreach (var pageTuple in _processed)
            {
                foreach (var linkTuple in pageTuple.Value.Links)
                {
                    for (var i = 0; i < _processed[linkTuple.Value].AffectedState.PlayerState.Count; i++)
                    {
                        pageTuple.Value.AffectedState.PlayerState[i] |=
                            _processed[linkTuple.Value].AffectedState.PlayerState[i];
                    }
                    for (var i = 0; i < _processed[linkTuple.Value].AffectedState.ScenesSeen.Count; i++)
                    {
                        pageTuple.Value.AffectedState.ScenesSeen[i] |=
                            _processed[linkTuple.Value].AffectedState.ScenesSeen[i];
                    }
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

        private IEnumerable<Action> GetActionsForPage(PageState page)
        {
            var actions = new List<Action>();
            for (var i = 0; i < page.State.ActionsToShow.Count; i++)
            {
                if (page.State.ActionsToShow[i]) actions.Add(_actionMatrix[i + 1]);
            }
            return actions;
        }

        private void ProcessState(StateQueueItem currentState)
        {
            currentState.Page.Scene.Visited = true;

            var states = new HashSet<string>();

            var anchors = Utilities.GetInstance(Warnings, currentState.Page.Scene.Name, currentState.Page.Scene.LineNumber).ParseAnchors(currentState.Page.Scene.RawDescription).ToList();
            foreach (var action in GetActionsForPage(currentState.Page))
            {
                action.Visited = true;
                anchors.AddRange(Utilities.GetInstance(Warnings, action.Toggle, action.LineNumber).ParseAnchors(action.RawDescription));
            }
            var conditionals =
                anchors.SelectMany(
                    a => a.Href != null && a.Href.Conditions != null ? a.Href.Conditions.Select(c => c.Key) : new string[] {})
                    .Distinct()
                    .ToArray();
            var hasFirstSeen = RegexLib.BlockQuotes.IsMatch(currentState.Page.Scene.Description);

            foreach (var affected in currentState.AffectedStates)
            {
                // signal to previous scenes that this scene's used conditionals are important
                if(currentState.Page.Scene.Conditions != null)
                    foreach (var conditional in currentState.Page.Scene.Conditions)
                    {
                        var anchor = anchors.FirstOrDefault(a =>
                            a.Href.Conditions != null
                            && a.Href.Conditions.Keys.Contains(conditional.Key));
                        _manager.ToggleStateOn(affected, conditional.Key, currentState.Page.Scene.Name, anchor != null ? anchor.LineNumber : currentState.Page.Scene.LineNumber, anchor != null ? anchor.ColNumber : 1);
                    }
                foreach (var conditional in conditionals)
                {
                    var anchor = anchors.FirstOrDefault(a =>
                        a.Href.Conditions != null
                        && a.Href.Conditions.Keys.Contains(conditional));
                    _manager.ToggleStateOn(affected, conditional, currentState.Page.Scene.Name, anchor != null ? anchor.LineNumber : currentState.Page.Scene.LineNumber, anchor != null ? anchor.ColNumber : 1);
                }

                // signal to previous scenes if this scene has first-seen text
                if (hasFirstSeen) _manager.ToggleSeenSceneOn(affected, currentState.Page.Scene.Id);
            }

            foreach (var anchor in anchors.Where(a => a.Href != null && (a.Href.Target != null || a.Href.Toggles != null)))
            {
                // don't follow links that would be hidden
                if (anchor.Href.Conditions != null &&
                    string.IsNullOrEmpty(
                        Utilities.GetInstance(Warnings, currentState.Page.Scene.Name, currentState.Page.Scene.LineNumber).ParseConditionalText(anchor)[
                            Utilities.GetInstance(Warnings, currentState.Page.Scene.Name, currentState.Page.Scene.LineNumber).ConditionsMet(StateResolver.GetStateDictionary(currentState.Page),
                                anchor.Href.Conditions)])) continue;

                var newState = _manager.ResolveNewState(anchor, currentState.Page);
                if(newState.Scene != null)
                {
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
}
