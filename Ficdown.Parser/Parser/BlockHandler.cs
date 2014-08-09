namespace Ficdown.Parser.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Model.Parser;
    using Model.Story;
    using Action = Model.Story.Action;

    internal class BlockHandler : IBlockHandler
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
            var storyAnchor = Utilities.ParseAnchor(storyBlock.Name);

            if (storyAnchor.Href.Target == null || storyAnchor.Href.Conditions != null ||
                storyAnchor.Href.Toggles != null)
                throw new FormatException(string.Format("Story href should only have target: {0}",
                    storyAnchor.Original));

            var story = new Story
            {
                Name = storyAnchor.Text,
                Description = string.Join("\n", storyBlock.Lines).Trim(),
                Scenes = new Dictionary<string, IList<Scene>>(),
                Actions = new Dictionary<string, Action>()
            };

            var sid = 1;
            var scenes = blocks.Where(b => b.Type == BlockType.Scene).Select(b => BlockToScene(b, sid++));
            foreach (var scene in scenes)
            {
                if (!story.Scenes.ContainsKey(scene.Key)) story.Scenes.Add(scene.Key, new List<Scene>());
                story.Scenes[scene.Key].Add(scene);
            }
            var aid = 1;
            story.Actions =
                blocks.Where(b => b.Type == BlockType.Action).Select(b => BlockToAction(b, aid++)).ToDictionary(a => a.Toggle, a => a);

            if (!story.Scenes.ContainsKey(storyAnchor.Href.Target))
                throw new FormatException(string.Format("Story targets non-existent scene: {0}", storyAnchor.Href.Target));
            story.FirstScene = storyAnchor.Href.Target;

            return story;
        }


        private Scene BlockToScene(Block block, int id)
        {
            var scene = new Scene
            {
                Id = id,
                Description = string.Join("\n", block.Lines).Trim()
            };

            try
            {
                var sceneName = Utilities.ParseAnchor(block.Name);
                scene.Name = sceneName.Title != null ? sceneName.Title.Trim() : sceneName.Text.Trim();
                scene.Key = Utilities.NormalizeString(sceneName.Text);
                if(sceneName.Href.Target != null || sceneName.Href.Toggles != null)
                    throw new FormatException(string.Format("Scene href should only have conditions: {0}", block.Name));
                scene.Conditions = sceneName.Href.Conditions;
            }
            catch(FormatException)
            {
                scene.Name = block.Name.Trim();
                scene.Key = Utilities.NormalizeString(block.Name);
            }

            return scene;
        }

        private Action BlockToAction(Block block, int id)
        {
            return new Action
            {
                Id = id,
                Toggle = Utilities.NormalizeString(block.Name),
                Description = string.Join("\n", block.Lines).Trim()
            };
        }
    }
}