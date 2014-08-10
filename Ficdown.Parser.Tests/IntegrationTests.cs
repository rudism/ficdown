namespace Ficdown.Parser.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using Render;
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
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "itest_output");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            foreach (var file in Directory.GetFiles(path))
            {
                File.Delete(file);
            }
            var rend = new HtmlRenderer();
            rend.Render(story, path);
        }
    }
}
