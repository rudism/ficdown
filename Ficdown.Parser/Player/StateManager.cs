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

        public StateManager(Story story)
        {
            _story = story;
            var allScenes = _story.Scenes.SelectMany(s => s.Value);
            _sceneCount = allScenes.Max(s => s.Id);
            _actionCount = _story.Actions.Max(a => a.Value.Id);
            _stateMatrix = new Dictionary<string, int>();
            var state = 0;
            foreach (
                var toggle in
                    allScenes.SelectMany(
                        sc =>
                            Utilities.ParseAnchors(sc.Description)
                                .SelectMany(
                                    a =>
                                        a.Href.Toggles != null
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
                    Scene = _story.Scenes[_story.FirstScene].Single(s => s.Conditions == null),
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
                            Utilities.ParseAnchors(_story.Actions[toggle].Description)
                                .Any(a => a.Href.Conditions != null && a.Href.Conditions.ContainsKey(toggle)))
                            actionFirstToggles.Add(!current.State.PlayerState[_stateMatrix[toggle]]);
                    }
                    newState.State.PlayerState[_stateMatrix[toggle]] = true;
                }
            }
            newState.State.ActionFirstToggles = actionFirstToggles != null
                ? new BitArray(actionFirstToggles.ToArray())
                : null;
            newState.Scene = GetScene(target, newState.State.PlayerState);
            return newState;
        }

        public void ToggleStateOn(State state, string toggle)
        {
            state.PlayerState[_stateMatrix[toggle]] = true;
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

        private Scene GetScene(string target, BitArray playerState)
        {
            if (!_story.Scenes.ContainsKey(target))
                throw new FormatException(string.Format("Encountered link to non-existant scene: {0}", target));

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
                throw new FormatException(string.Format("Scene {0} reached with unmatched player state", target));
            return newScene;
        }

        private bool ConditionsMatch(Scene scene, BitArray playerState)
        {
            if (scene.Conditions == null) return true;
            return scene.Conditions.All(c => playerState[_stateMatrix[c.Key]] == c.Value);
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
