namespace Ficdown.Parser.Player
{
    using System.IO;
    using MarkdownSharp;

    public class HtmlRenderer : IRenderer
    {
        public string Template { get; set; }
        public void Render(string text, string outFile)
        {
            File.WriteAllText(outFile, new Markdown().Transform(text));
        }
    }
}
