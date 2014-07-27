namespace Ficdown.Parser.Model.Parser
{
    using System.Collections.Generic;

    internal class Href
    {
        public string Original { get; set; }
        public string Target { get; set; }
        public IDictionary<string, bool> Conditions { get; set; }
        public IEnumerable<string> Toggles { get; set; }
    }
}
