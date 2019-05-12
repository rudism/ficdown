using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Ficdown.Parser.Tests")]

namespace Ficdown.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Model.Parser;
    using Parser;
    using Player;

    public class FicdownParser
    {
        public List<FicdownException> Warnings { get; private set; }

        private IBlockHandler _blockHandler;
        internal IBlockHandler BlockHandler
        {
            get
            {
                return _blockHandler ??
                    (_blockHandler = new BlockHandler { Warnings = Warnings });
            }
            set
            {
                _blockHandler = value;
                _blockHandler.Warnings = Warnings;
            }
        }

        private IGameTraverser _gameTraverser;
        internal IGameTraverser GameTraverser
        {
            get { return _gameTraverser ??
                (_gameTraverser = new GameTraverser { Warnings = Warnings }); }
            set
            {
                _gameTraverser = value;
                _gameTraverser.Warnings = Warnings;
            }
        }

        private IStateResolver _stateResolver;
        internal IStateResolver StateResolver
        {
            get
            {
                return _stateResolver ??
                    (_stateResolver = new StateResolver { Warnings = Warnings });
            }
            set
            {
                _stateResolver = value;
                _stateResolver.Warnings = Warnings;
            }
        }

        public FicdownParser()
        {
            Warnings = new List<FicdownException>();
        }

        public ResolvedStory ParseStory(string storyText)
        {
            var lines = storyText.Split(new[] {"\n", "\r\n"}, StringSplitOptions.None);
            var blocks = BlockHandler.ExtractBlocks(lines);
            var story = BlockHandler.ParseBlocks(blocks);

            // dupe scene sanity check
            foreach(var key in story.Scenes.Keys)
            {
                foreach(var scene in story.Scenes[key])
                {
                    foreach(var otherScene in story.Scenes[key].Where(s => s != scene))
                    {
                        if((scene.Conditions == null && otherScene.Conditions == null)
                            || (scene.Conditions != null && otherScene.Conditions != null
                                && scene.Conditions.Count == otherScene.Conditions.Count
                                && !scene.Conditions.Except(otherScene.Conditions).Any()))
                            Warnings.Add(new FicdownException(scene.Name, string.Format("Scene defined again on line {0}", otherScene.LineNumber), scene.LineNumber));
                    }
                }
            }

            GameTraverser.Story = story;
            var resolved = StateResolver.Resolve(GameTraverser.Enumerate(), story);
            resolved.Orphans = GameTraverser.OrphanedScenes.Select(o => new Orphan
            {
                Type = "Scene",
                Name = o.Name,
                LineNumber = o.LineNumber
            }).Union(GameTraverser.OrphanedActions.Select(o => new Orphan
            {
                Type = "Action",
                Name = o.Toggle,
                LineNumber = o.LineNumber
            }));
            return resolved;
        }
    }
}
