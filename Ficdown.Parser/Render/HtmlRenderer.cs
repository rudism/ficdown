namespace Ficdown.Parser.Render
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using MarkdownSharp;
    using Model.Parser;
    using Parser;

    public class HtmlRenderer : IRenderer
    {
        protected readonly Markdown Markdown;

        private string _index;
        private string _scene;
        private string _styles;

        protected ResolvedStory Story { get; set; }

        public HtmlRenderer()
        {
            Markdown = new Markdown();
            _index = Template.Index;
            _scene = Template.Scene;
            _styles = Template.Styles;
        }

        public HtmlRenderer(string index, string scene, string styles)
        {
            _index = index;
            _scene = scene;
            _styles = styles;
        }

        public virtual void Render(ResolvedStory story, string outPath, bool debug = false)
        {
            Story = story;
            GenerateHtml(story, outPath, debug);
        }

        private string FillTemplate(string template, Dictionary<string, string> values)
        {
            return values.Aggregate(template,
                (current, pair) => current.Replace(string.Format("@{0}", pair.Key), pair.Value));
        }

        protected void GenerateHtml(ResolvedStory story, string outPath, bool debug)
        {
            var index = FillTemplate(_index, new Dictionary<string, string>
            {
                {"Title", story.Name},
                {"Description", Markdown.Transform(story.Description)},
                {"FirstScene", string.Format("{0}.html", story.FirstPage)}
            });

            File.WriteAllText(Path.Combine(outPath, "index.html"), index);

            foreach (var page in story.Pages)
            {
                File.WriteAllText(Path.Combine(outPath, "styles.css"), _styles);

                var content = page.Content;
                foreach (var anchor in Utilities.ParseAnchors(page.Content))
                {
                    var newAnchor = string.Format("[{0}]({1}.html)", anchor.Text, anchor.Href.Target);
                    content = content.Replace(anchor.Original, newAnchor);
                }
                if (debug)
                {
                    content += string.Format("\n\n### State Debug\n\n{0}",
                        string.Join("\n", page.ActiveToggles.Select(t => string.Format("- {0}", t)).ToArray()));
                }

                var scene = FillTemplate(_scene, new Dictionary<string, string>
                {
                    {"Title", story.Name},
                    {"Content", Markdown.Transform(content)}
                });

                File.WriteAllText(Path.Combine(outPath, string.Format("{0}.html", page.Name)), scene);
            }
        }
    }
}
