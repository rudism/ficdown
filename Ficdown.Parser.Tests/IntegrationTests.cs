namespace Ficdown.Parser.Tests
{
    using System.Linq;
    using System.Text;
    using Player;
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

            var player = new GameTraverser();
            player.ExportStaticStory(story, @"C:\Users\Rudis\Desktop\template.html", @"C:\Users\Rudis\Desktop\output");
        }
    }
}
