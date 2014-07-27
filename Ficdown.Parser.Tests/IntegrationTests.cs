namespace Ficdown.Parser.Tests
{
    using System;
    using System.Linq;
    using System.Text;
    using Player;
    using ServiceStack.Text;
    using TestStories;
    using Xunit;

    public class IntegrationTests
    {
        [Fact]
        public void CanParseValidStoryFile()
        {
            var parser = new FicdownParser();
            var storyText = Encoding.UTF8.GetString(Resources.TheRobotKing);
            var story = parser.ParseStory(storyText);
            Assert.NotNull(story);
            Assert.Equal("The Robot King", story.Name);
            Assert.Equal("Robot Cave", story.Scenes[story.FirstScene].First().Name);

            var traverser = new GameTraverser(story);
            var test = traverser.Enumerate();
            Console.WriteLine(test.Take(10).Dump());
        }
    }
}
