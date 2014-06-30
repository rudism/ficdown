namespace Ficdown.Parser.Tests
{
    using System;
    using System.Text;
    using ServiceStack.Text;
    using TestStories;
    using Xunit;

    public class IntegrationTests
    {
        [Fact]
        public void CanParseValidStoryFile()
        {
            var parser = new FicdownParser();
            var storyText = Encoding.UTF8.GetString(Resources.the_robot_king);
            var story = parser.ParseStory(storyText);
            Assert.NotNull(story);
            Console.WriteLine(story.Dump());
        }
    }
}
