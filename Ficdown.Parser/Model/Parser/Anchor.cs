namespace Ficdown.Parser.Model.Parser
{
    internal class Anchor
    {
        public string Original { get; set; }
        public string Text { get; set; }
        public Href Href { get; set; }
        public string Title { get; set; }
    }
}
