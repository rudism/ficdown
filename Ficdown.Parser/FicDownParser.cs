using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Ficdown.Parser.Tests")]

namespace Ficdown.Parser
{
    using System;
    using System.Collections.Generic;
    using Model.Parser;
    using Parser;
    using Player;

    public class FicdownParser
    {
        private IBlockHandler _blockHandler;
        internal IBlockHandler BlockHandler
        {
            get { return _blockHandler ?? (_blockHandler = new BlockHandler()); }
            set { _blockHandler = value; }
        }

        private IGameTraverser _gameTraverser;
        internal IGameTraverser GameTraverser
        {
            get { return _gameTraverser ?? (_gameTraverser = new GameTraverser()); }
            set { _gameTraverser = value; }
        }

        private IStateResolver _stateResolver;
        internal IStateResolver StateResolver
        {
            get { return _stateResolver ?? (_stateResolver = new StateResolver()); }
            set { _stateResolver = value; }
        }

        public ResolvedStory ParseStory(string storyText)
        {
            var lines = storyText.Split(new[] {"\n", "\r\n"}, StringSplitOptions.None);
            var blocks = BlockHandler.ExtractBlocks(lines);
            var story = BlockHandler.ParseBlocks(blocks);
            GameTraverser.Story = story;
            return StateResolver.Resolve(GameTraverser.Enumerate(), story);
        }
    }
}
