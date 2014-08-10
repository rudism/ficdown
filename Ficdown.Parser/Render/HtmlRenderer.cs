namespace Ficdown.Parser.Render
{
    using System.IO;
    using System.Linq;
    using MarkdownSharp;
    using Model.Parser;
    using Parser;

    internal class HtmlRenderer : IRenderer
    {
        private readonly Markdown _md;

        public HtmlRenderer()
        {
            _md = new Markdown();
        }

        public void Render(ResolvedStory story, string outPath, bool debug = false)
        {
            var index = string.Format("# {0}\n\n{1}\n\n[Click here]({2}.html) to start!", story.Name, story.Description,
                story.FirstPage);

            File.WriteAllText(Path.Combine(outPath, "index.html"), _md.Transform(index));

            foreach (var page in story.Pages)
            {
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
                File.WriteAllText(Path.Combine(outPath, string.Format("{0}.html", page.Name)), _md.Transform(content));
            }
        }
    }
}
