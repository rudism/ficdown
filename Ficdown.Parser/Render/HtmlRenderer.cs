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
        private readonly string _language;

        protected readonly Markdown Markdown;

        public List<FicdownException> Warnings { private get; set; }

        public string IndexTemplate { get; set; }
        public string SceneTemplate { get; set; }
        public string StylesTemplate { get; set; }
        public string ImageDir { get; set; }

        protected ResolvedStory Story { get; set; }

        public HtmlRenderer(string language)
        {
            _language = language;
            Markdown = new Markdown();
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
            var index = FillTemplate(IndexTemplate ?? Template.Index, new Dictionary<string, string>
            {
                {"Language", _language},
                {"Title", story.Name},
                {"Description", Markdown.Transform(story.Description)},
                {"FirstScene", string.Format("{0}.html", story.FirstPage)}
            });

            File.WriteAllText(Path.Combine(outPath, "index.html"), index);

            foreach (var page in story.Pages)
            {
                File.WriteAllText(Path.Combine(outPath, "styles.css"), StylesTemplate ?? Template.Styles);

                var content = page.Content;
                foreach (var anchor in Utilities.GetInstance(Warnings, page.Name).ParseAnchors(page.Content))
                {
                    var newAnchor = string.Format("[{0}]({1}.html)", anchor.Text, anchor.Href.Target);
                    content = content.Replace(anchor.Original, newAnchor);
                }
                if (debug)
                {
                    content += string.Format("\n\n### State Debug\n\n{0}",
                        string.Join("\n", page.ActiveToggles.Select(t => string.Format("- {0}", t)).ToArray()));
                }

                var scene = FillTemplate(SceneTemplate ?? Template.Scene, new Dictionary<string, string>
                {
                    {"Language", _language},
                    {"Title", story.Name},
                    {"Content", Markdown.Transform(content)}
                });

                File.WriteAllText(Path.Combine(outPath, string.Format("{0}.html", page.Name)), scene);
            }

            if (!string.IsNullOrWhiteSpace(ImageDir))
            {
                var dirname = ImageDir.Substring(ImageDir.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                Directory.CreateDirectory(Path.Combine(outPath, dirname));
                CopyFilesRecursively(ImageDir, Path.Combine(outPath, dirname));
            }
        }

        private static void CopyFilesRecursively(string sourcePath, string destinationPath)
        {
            foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));

            foreach (var newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(sourcePath, destinationPath));
        }
    }
}
