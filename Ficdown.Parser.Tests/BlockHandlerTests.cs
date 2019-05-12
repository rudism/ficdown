namespace Ficdown.Parser.Tests
{
    using Xunit;
    using Parser;
    using Model.Parser;
    using Extensions;
    using System.Collections.Generic;

    public class BlockHandlerTests
    {
        private BlockHandler NewBlockHandler
        {
            get { return new BlockHandler { Warnings = new List<FicdownException>() }; }
        }

        [Fact]
        public void NoStoryBlockThrowsException()
        {
            var bh = NewBlockHandler;
            Assert.Throws<FicdownException>(() => bh.ParseBlocks(bh.ExtractBlocks(@"
## this file has no story
just a lonely scene".ToLines())));
        }

        [Fact]
        public void StoryWithNoAnchorThrowsException()
        {
            var bh = NewBlockHandler;
            Assert.Throws<FicdownException>(() => bh.ParseBlocks(bh.ExtractBlocks(@"
# my story
doesn't link to a scene
## a scene
nothing links here".ToLines())));
        }

        [Fact]
        public void StoriesWithFancyAnchorsThrowExceptions()
        {
            var bh = NewBlockHandler;
            bh.ParseBlocks(bh.ExtractBlocks(@"
# [my story](/a-scene?conditional)
story with a conditional
## a scene
this is a scene".ToLines()));
            Assert.NotEmpty(bh.Warnings);

            bh = NewBlockHandler;
            bh.ParseBlocks(bh.ExtractBlocks(@"
# [my story](/a-scene#toggle)
story with a toggle
## a scene
this is a scene".ToLines()));
            Assert.NotEmpty(bh.Warnings);

            bh = NewBlockHandler;
            Assert.Throws<FicdownException>(() => bh.ParseBlocks(bh.ExtractBlocks(@"
# [my story](/a-scene#?conditional#toggle)
story with a conditional and a toggle
## a scene
this is a scene".ToLines())));
        }

        [Fact]
        public void StoryLinkingToNonExistentSceneThrowsException()
        {
            var bh = NewBlockHandler;
            Assert.Throws<FicdownException>(() => bh.ParseBlocks(bh.ExtractBlocks(@"
# [a story](/non-existent)
this story links to a first scene that doesn't exist
## a scene
this scene is so cold and lonely".ToLines())));
        }

        [Fact]
        public void StoryWithALegitAnchorParses()
        {
            var bh = NewBlockHandler;
            bh.ParseBlocks(bh.ExtractBlocks(@"
# [my story](/a-scene)
story with a simple link
## a scene
this is a scene".ToLines()));
        }

        [Fact]
        public void StoryWithDuplicateActionsThrowsException()
        {
            var bh = NewBlockHandler;
            bh.ParseBlocks(bh.ExtractBlocks(@"
# [a story](/a-scene)
this story is action-happy
## a scene
this is the first scene
### an action
this is an action
## another scene
this is another scene
### an action
oops, this is the same action!".ToLines()));
            Assert.NotEmpty(bh.Warnings);
        }

        [Fact]
        public void StoryWithScenesAndActionsParses()
        {
            var bh = NewBlockHandler;
            var story = bh.ParseBlocks(bh.ExtractBlocks(@"
# [my story](/a-scene)
story with a simple link
## a scene
this is a scene
## [a scene](?something)
this is the same scene with a conditional
## another scene
this is another scene
### action1
this is an action
### action 2
this is another action
## yet another scene
yup here's some more
### another action
the last action (hero?)".ToLines()));

            Assert.Equal(3, story.Scenes.Count);
            Assert.Equal(2, story.Scenes["a-scene"].Count);
            Assert.Equal(3, story.Actions.Count);
        }
    }
}
