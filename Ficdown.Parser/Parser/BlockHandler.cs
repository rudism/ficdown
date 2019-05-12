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
        public List<FicdownException> Warnings { get; set; }

        public IEnumerable<Block> ExtractBlocks(IEnumerable<string> lines)
        {
            var blocks = new List<Block>();
            Block currentBlock = null;
            var lineNum = 1;
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
                        Lines = new List<Line>(),
                        LineNumber = lineNum++
                    };
                }
                else
                {
                    if (currentBlock != null) currentBlock.Lines.Add(new Line
                    {
                        Number = lineNum++,
                        Text = line
                    });
                }
            }
            if (currentBlock != null) blocks.Add(currentBlock);
            return blocks;
        }

        public Story ParseBlocks(IEnumerable<Block> blocks)
        {
            // get the story
            var storyBlocks = blocks.Where(b => b.Type == BlockType.Story);
            if(storyBlocks.Count() == 0) throw new FicdownException("No story block found");
            if(storyBlocks.Count() > 1) throw new FicdownException("More than one story block found");

            var storyBlock = storyBlocks.Single();

            var storyAnchor = Utilities.GetInstance(Warnings, storyBlock.Name, storyBlock.LineNumber).ParseAnchor(storyBlock.Name, storyBlock.LineNumber, 1);

            if(storyAnchor == null || storyAnchor.Href == null)
            {
                throw new FicdownException(storyBlock.Name, "Story name must be an anchor pointing to the first scene", storyBlock.LineNumber);
            }

            if (storyAnchor.Href.Target == null || storyAnchor.Href.Conditions != null ||
                storyAnchor.Href.Toggles != null)
                Warnings.Add(new FicdownException(storyBlock.Name, "Story href should only have a target", storyBlock.LineNumber));

            var story = new Story
            {
                Name = storyAnchor.Text,
                Description = string.Join("\n", storyBlock.Lines.Select(l => l.Text)).Trim(),
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
            try
            {
                story.Actions =
                    blocks.Where(b => b.Type == BlockType.Action).Select(b => BlockToAction(b, aid++)).ToDictionary(a => a.Toggle, a => a);
            }
            catch(ArgumentException)
            {
                var a = blocks.First(b => b.Type == BlockType.Action && blocks.Any(d => b != d && BlockToAction(b, 0).Toggle == BlockToAction(d, 0).Toggle));
                var actionA = BlockToAction(a, a.LineNumber);
                var dupe = blocks.First(b => b.Type == BlockType.Action && b != a && BlockToAction(b, 0).Toggle == actionA.Toggle);
                Warnings.Add(new FicdownException(actionA.Toggle, string.Format("Action is defined again on line {0}", dupe.LineNumber), actionA.LineNumber));
            }

            if (!story.Scenes.ContainsKey(storyAnchor.Href.Target))
                throw new FicdownException(storyBlock.Name, string.Format("Story links to undefined scene: {0}", storyAnchor.Href.Target), storyBlock.LineNumber);
            story.FirstScene = storyAnchor.Href.Target;

            return story;
        }


        private Scene BlockToScene(Block block, int id)
        {
            var scene = new Scene
            {
                Id = id,
                LineNumber = block.LineNumber,
                RawDescription = string.Join("\n", block.Lines.Select(l => l.Text)),
                Description = string.Join("\n", block.Lines.Select(l => l.Text)).Trim()
            };

            Anchor sceneName;
            if(RegexLib.Anchors.IsMatch(block.Name) && (sceneName = Utilities.GetInstance(Warnings, block.Name, block.LineNumber).ParseAnchor(block.Name, block.LineNumber, 1)).Href != null)
            {
                scene.Name = sceneName.Title != null ? sceneName.Title.Trim() : sceneName.Text.Trim();
                scene.Key = Utilities.GetInstance(Warnings, block.Name, block.LineNumber).NormalizeString(sceneName.Text);
                if(sceneName.Href.Target != null || sceneName.Href.Toggles != null)
                    Warnings.Add(new FicdownException(block.Name, "Scene href should only have conditions", block.LineNumber));
                scene.Conditions = sceneName.Href.Conditions;
            }
            else
            {
                scene.Name = block.Name.Trim();
                scene.Key = Utilities.GetInstance(Warnings, block.Name, block.LineNumber).NormalizeString(block.Name);
            }

            return scene;
        }

        private Action BlockToAction(Block block, int id)
        {
            return new Action
            {
                Id = id,
                Toggle = Utilities.GetInstance(Warnings, block.Name, block.LineNumber).NormalizeString(block.Name),
                RawDescription = string.Join("\n", block.Lines.Select(l => l.Text)),
                Description = string.Join("\n", block.Lines.Select(l => l.Text)).Trim(),
                LineNumber = block.LineNumber
            };
        }
    }
}
