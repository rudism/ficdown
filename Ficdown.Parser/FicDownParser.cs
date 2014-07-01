namespace Ficdown.Parser
{
    using System;
    using Engine;
    using Model.Story;

    public class FicdownParser
    {
        private IBlockHandler _blockHandler;
        public IBlockHandler BlockHandler
        {
            get { return _blockHandler ?? (_blockHandler = new BlockHandler()); }
            set { _blockHandler = value; }
        }

        private ISceneLinker _sceneLinker;

        public ISceneLinker SceneLinker
        {
            get { return _sceneLinker ?? (_sceneLinker = new SceneLinker()); }
            set { _sceneLinker = value; }
        }

        public Story ParseStory(string storyText)
        {
            var lines = storyText.Split(new[] {"\n", "\r\n"}, StringSplitOptions.None);
            var blocks = BlockHandler.ExtractBlocks(lines);
            var story = BlockHandler.ParseBlocks(blocks);
            SceneLinker.ExpandScenes(story);
            return story;
        }
    }
}
