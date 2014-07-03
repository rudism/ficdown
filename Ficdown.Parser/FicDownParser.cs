namespace Ficdown.Parser
{
    using System;
    using Model.Story;
    using Parser;

    public class FicdownParser
    {
        private IBlockHandler _blockHandler;
        public IBlockHandler BlockHandler
        {
            get { return _blockHandler ?? (_blockHandler = new BlockHandler()); }
            set { _blockHandler = value; }
        }

        private IStateResolver _stateResolver;

        public IStateResolver StateResolver
        {
            get { return _stateResolver ?? (_stateResolver = new StateResolver()); }
            set { _stateResolver = value; }
        }

        public Story ParseStory(string storyText)
        {
            var lines = storyText.Split(new[] {"\n", "\r\n"}, StringSplitOptions.None);
            var blocks = BlockHandler.ExtractBlocks(lines);
            var story = BlockHandler.ParseBlocks(blocks);
            return story;
        }
    }
}
