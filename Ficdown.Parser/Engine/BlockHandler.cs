namespace Ficdown.Parser.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Model.Parser;
    using Model.Story;
    using Action = Model.Story.Action;

    public class BlockHandler : IBlockHandler
    {
        public IEnumerable<Block> ExtractBlocks(IEnumerable<string> lines)
        {
            var blocks = new List<Block>();
            Block currentBlock = null;
            foreach (var line in lines)
            {
                var match = Regex.Match(line, @"^(?<level>#{1,3})\s+(?<name>[^#].*)$");
                if (match.Success)
                {
                    if (currentBlock != null) blocks.Add(currentBlock);
                    currentBlock = new Block()
                    {
                        Type = (BlockType) match.Groups["level"].Length,
                        Name = match.Groups["name"].Value,
                        Lines = new List<string>()
                    };
                }
                else
                {
                    if (currentBlock != null) currentBlock.Lines.Add(line);
                }
            }
            if (currentBlock != null) blocks.Add(currentBlock);
            return blocks;
        }

        public Story ParseBlocks(IEnumerable<Block> blocks)
        {
            // get the story
            var storyBlock = blocks.Single(b => b.Type == BlockType.Story);
            var storyName = RegexLib.Anchors.Match(storyBlock.Name);

            if (!storyName.Success)
                throw new FormatException("Story name must be a link to the first scene.");

            var story = new Story
            {
                Name = storyName.Groups["text"].Value,
                Description = string.Join("\n", storyBlock.Lines).Trim(),
                Scenes = new Dictionary<string, IList<Scene>>(),
                States = new Dictionary<string, IList<Action>>()
            };

            var scenes = blocks.Where(b => b.Type == BlockType.Scene).Select(b => BlockToScene(b));
            foreach (var scene in scenes)
            {
                var key = Utilities.NormalizeString(scene.Name);
                if (!story.Scenes.ContainsKey(key)) story.Scenes.Add(key, new List<Scene>());
                story.Scenes[key].Add(scene);
            }

            return story;
        }

        private Scene BlockToScene(Block block)
        {
            var scene = new Scene
            {
                Description = string.Join("\n", block.Lines).Trim()
            };

            var sceneName = RegexLib.Anchors.Match(block.Name);
            if (sceneName.Success)
            {
                scene.Name = sceneName.Groups["text"].Value;
            }
            else
            {
                scene.Name = block.Name;
            }

            return scene;
        }
    }
}