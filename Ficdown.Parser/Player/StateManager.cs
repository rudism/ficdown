namespace Ficdown.Parser.Player
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Model.Parser;
    using Model.Story;
    using Model.Traverser;
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
                        ActionsToShow = new BitArray(_actionCount)
                    },
                    AffectedState = new State
                    {
                        PlayerState = new BitArray(_stateMatrix.Keys.Count),
                        ScenesSeen = new BitArray(_sceneCount),
                        ActionsToShow = new BitArray(_actionCount)
                    },
                    Scene = _story.Scenes[_story.FirstScene].Single(s => s.Conditions == null)
                };
            }
        }

        public PageState ResolveNewState(Anchor anchor, PageState current)
        {
            var target = anchor.Href.Target ?? current.Scene.Key;

            var newState = ClonePage(current);
            newState.State.ScenesSeen[current.Scene.Id - 1] = true;
            if (anchor.Href.Toggles != null)
            {
                foreach (var toggle in anchor.Href.Toggles)
                {
                    if (_story.Actions.ContainsKey(toggle))
                    {
                        newState.State.ActionsToShow[_story.Actions[toggle].Id - 1] = true;
                    }
                    newState.State.PlayerState[_stateMatrix[toggle]] = true;
                }
            }
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
                }
            };
        }
    }
}
