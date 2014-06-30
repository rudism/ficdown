namespace Ficdown.Parser.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Engine;
    using Model.Parser;
    using Model.Story;
    using ServiceStack.Text;
    using TestStories;
    using Xunit;

    public class BlockHandlerTests
    {
        [Fact]
        public void GoodTestStoryShouldParse()
        {
            var lines = Encoding.UTF8.GetString(Resources.the_robot_king).Split(new[] {'\r', '\n'});
            var handler = new BlockHandler();
            IEnumerable<Block> blocks = null;
            Story story = null;
            Assert.DoesNotThrow(() => blocks = handler.ExtractBlocks(lines));
            Assert.DoesNotThrow(() => story = handler.ParseBlocks(blocks));
            Assert.NotNull(story);

            Console.WriteLine(story.Dump());
        }
    }
}
