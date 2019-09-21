namespace Ficdown.Parser.Tests
{
    using System.IO;
    using Render;
    using Xunit;

    public class IntegrationTests
    {
        [Fact]
        public void CanParseValidStoryFile()
        {
            Logger.Initialize(true);
            var parser = new FicdownParser();
            var storyText = File.ReadAllText(Path.Combine(Template.BaseDir, "TestStories", "CloakOfDarkness.md"));
            var story = parser.ParseStory(storyText);
            var path = Path.Combine(Template.BaseDir, "itest_output");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            foreach (var file in Directory.GetFiles(path))
            {
                File.Delete(file);
            }
            var rend = new HtmlRenderer("en");
            rend.Render(story, path, true);
        }
    }
}
