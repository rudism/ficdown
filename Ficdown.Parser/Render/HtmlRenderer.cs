namespace Ficdown.Parser.Render
{
    using System.Collections.Generic;
    using System.IO;
    using MarkdownSharp;
    using Model.Parser;
    using Model.Story;
    using Parser;

    internal class HtmlRenderer : IRenderer
    {
        private readonly Markdown _md;

        public HtmlRenderer()
        {
            _md = new Markdown();
        }

        public void Render(IEnumerable<ResolvedPage> pages, string outPath)
        {
            foreach (var page in pages)
            {
                var content = page.Content;
                foreach (var anchor in Utilities.ParseAnchors(page.Content))
                {
                    var newAnchor = string.Format("[{0}]({1}.html)", anchor.Text, anchor.Href.Target);
                    content = content.Replace(anchor.Original, newAnchor);
                }
                File.WriteAllText(Path.Combine(outPath, string.Format("{0}.html", page.Name)), _md.Transform(content));
            }
        }
    }
}
