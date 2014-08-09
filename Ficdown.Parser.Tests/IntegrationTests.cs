namespace Ficdown.Parser.Tests
{
    using System.Text;
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
        }
    }
}
