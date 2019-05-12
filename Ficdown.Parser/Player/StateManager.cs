namespace Ficdown.Parser.Player
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Model.Parser;
    using Model.Player;
    using Model.Story;
    using Parser;

    internal class StateManager
    {
        private readonly Story _story;
        private readonly Dictionary<string, int> _stateMatrix;
        private readonly int _sceneCount;
        private readonly int _actionCount;

        private List<FicdownException> _warnings { get; set; }

        public StateManager(Story story, List<FicdownException> warnings)
        {
            _warnings = warnings;
            _story = story;
            var allScenes = _story.Scenes.SelectMany(s => s.Value);
            _sceneCount = allScenes.Max(s => s.Id);
            _actionCount = _story.Actions.Count > 0 ? _story.Actions.Max(a => a.Value.Id) : 0;
            _stateMatrix = new Dictionary<string, int>();
            var state = 0;
            foreach (
                var toggle in
                    allScenes.SelectMany(
                        sc =>
                            Utilities.GetInstance(_warnings, sc.Name, sc.LineNumber).ParseAnchors(sc.RawDescription)
                                .SelectMany(
                                    a =>
                                        a.Href != null && a.Href.Toggles != null
                                            ? a.Href.Toggles.Where(t => !_stateMatrix.ContainsKey(t))
                                            : new string[] {})))
            {
                _stateMatrix.Add(toggle, state++);
            }
        }

        public PageState InitialState
        {
            get
            {
                var scene = _story.Scenes[_story.FirstScene].Where(s => s.Conditions == null);
                if(scene.Count()  == 0)
                    throw new FicdownException(_story.Name, string.Format("Story links to undefined scene: {0}", _story.FirstScene));
                if(scene.Count() > 1)
                    _warnings.Add(new FicdownException(_story.Name, string.Format("Story links to scene that is defined more than once: {0}", _story.FirstScene)));

                return new PageState
                {
                    Id = Guid.Empty,
                    Links = new Dictionary<string, string>(),
                    State = new State
                    {
                        PlayerState = new BitArray(_stateMatrix.Keys.Count),
                        ScenesSeen = new BitArray(_sceneCount),
                        ActionsToShow = new BitArray(_actionCount),
                        ActionFirstToggles = null
                    },
                    AffectedState = new State
                    {
                        PlayerState = new BitArray(_stateMatrix.Keys.Count),
                        ScenesSeen = new BitArray(_sceneCount),
                        ActionsToShow = new BitArray(_actionCount),
                        ActionFirstToggles = null
                    },
                    Scene = scene.First(),
                    StateMatrix = _stateMatrix
                };
            }
        }

        public PageState ResolveNewState(Anchor anchor, PageState current)
        {
            var target = anchor.Href.Target ?? current.Scene.Key;

            var newState = ClonePage(current);
            newState.State.ScenesSeen[current.Scene.Id - 1] = true;
            List<bool> actionFirstToggles = null;
            if (anchor.Href.Toggles != null)
            {
                foreach (var toggle in anchor.Href.Toggles)
                {
                    if (_story.Actions.ContainsKey(toggle))
                    {
                        if(actionFirstToggles == null) actionFirstToggles = new List<bool>();
                        newState.State.ActionsToShow[_story.Actions[toggle].Id - 1] = true;
                        if (
                            Utilities.GetInstance(_warnings, _story.Actions[toggle].Toggle, _story.Actions[toggle].LineNumber).ParseAnchors(_story.Actions[toggle].RawDescription)
                                .Any(a => a.Href.Conditions != null && a.Href.Conditions.ContainsKey(toggle)))
                            actionFirstToggles.Add(!current.State.PlayerState[_stateMatrix[toggle]]);
                    }
                    newState.State.PlayerState[_stateMatrix[toggle]] = true;
                }
            }
            newState.State.ActionFirstToggles = actionFirstToggles != null
                ? new BitArray(actionFirstToggles.ToArray())
                : null;
            newState.Scene = GetScene(current.Scene.Name, anchor, target, newState.State.PlayerState);
            return newState;
        }

        public void ToggleStateOn(State state, string toggle, string blockName, int lineNumber, int colNumber)
        {
            if(_stateMatrix.ContainsKey(toggle))
                state.PlayerState[_stateMatrix[toggle]] = true;
            else
                _warnings.Add(new FicdownException(blockName, string.Format("Conditional for undefined state: {0}", toggle), lineNumber, colNumber));
        }

        public void ToggleSeenSceneOn(State state, int sceneId)
        {
            state.ScenesSeen[sceneId - 1] = true;
        }

        public static string GetUniqueHash(State state, string sceneKey)
        {
            var combined =
                new bool[
                    state.PlayerState.Count + state.ScenesSeen.Count + state.ActionsToShow.Count +
                    (state.ActionFirstToggles != null ? state.ActionFirstToggles.Count : 0)];
            state.PlayerState.CopyTo(combined, 0);
            state.ScenesSeen.CopyTo(combined, state.PlayerState.Count);
            state.ActionsToShow.CopyTo(combined, state.PlayerState.Count + state.ScenesSeen.Count);
            if (state.ActionFirstToggles != null)
                state.ActionFirstToggles.CopyTo(combined,
                    state.PlayerState.Count + state.ScenesSeen.Count + state.ActionsToShow.Count);
            var ba = new BitArray(combined);
            var byteSize = (int)Math.Ceiling(combined.Length / 8.0);
            var encoded = new byte[byteSize];
            ba.CopyTo(encoded, 0);
            return string.Format("{0}=={1}", sceneKey, Convert.ToBase64String(encoded));
        }

        public static string GetCompressedHash(PageState page)
        {
            var compressed = new State
            {
                PlayerState = page.State.PlayerState.And(page.AffectedState.PlayerState),
                ScenesSeen = page.State.ScenesSeen.And(page.AffectedState.ScenesSeen),
                ActionsToShow = page.State.ActionsToShow,
                ActionFirstToggles = page.State.ActionFirstToggles
            };
            return GetUniqueHash(compressed, page.Scene.Key);
        }

        private Scene GetScene(string blockName, Anchor anchor, string target, BitArray playerState)
        {
            if (_story.Scenes.ContainsKey(target))
            {
                Scene newScene = null;
                foreach (var scene in _story.Scenes[target])
                {
                    if (ConditionsMatch(scene, playerState) &&
                        (newScene == null || newScene.Conditions == null ||
                         scene.Conditions.Count > newScene.Conditions.Count))
                    {
                        newScene = scene;
                    }
                }
                if (newScene == null)
                    _warnings.Add(new FicdownException(blockName, string.Format("Link to scene that is undefined for conditionals: {0}", target), anchor.LineNumber, anchor.ColNumber));
                return newScene;
            }
            _warnings.Add(new FicdownException(blockName, string.Format("Link to undefined scene: {0}", target), anchor.LineNumber, anchor.ColNumber));
            return null;
        }

        private bool ConditionsMatch(Scene scene, BitArray playerState)
        {
            if (scene.Conditions == null) return true;
            scene.Conditions.ToList().ForEach(c =>
            {
                if(!_stateMatrix.ContainsKey(c.Key))
                    _warnings.Add(new FicdownException(scene.Name, string.Format("Conditional for undefined state: {0}", c.Key), scene.LineNumber));
            });
            return scene.Conditions.Where(c => _stateMatrix.ContainsKey(c.Key)).All(c => playerState[_stateMatrix[c.Key]] == c.Value);
        }

        private PageState ClonePage(PageState page)
        {
            return new PageState
            {
                Id = Guid.NewGuid(),
                Links = new Dictionary<string, string>(),
                State = new State
                {
                    PlayerState = page.State.PlayerState.Clone() as BitArray,
                    ScenesSeen = page.State.ScenesSeen.Clone() as BitArray,
                    ActionsToShow = new BitArray(_actionCount)
                },
                AffectedState = new State
                {
                    PlayerState = new BitArray(_stateMatrix.Keys.Count),
                    ScenesSeen = new BitArray(_sceneCount),
                    ActionsToShow = new BitArray(_actionCount)
                },
                StateMatrix = _stateMatrix
            };
        }
    }
}
