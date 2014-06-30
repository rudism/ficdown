namespace Ficdown.Parser
{
    using System.IO;
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

        public Story ParseStory(string storyFilePath)
        {
            var lines = File.ReadAllLines(storyFilePath);
            var blocks = BlockHandler.ExtractBlocks(lines);
            return BlockHandler.ParseBlocks(blocks);
        }
    }
}
