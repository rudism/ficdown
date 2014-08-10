namespace Ficdown.Parser.Model.Parser
{
    using System.Collections.Generic;

    public class ResolvedPage
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public IEnumerable<string> ActiveToggles { get; set; }
    }
}
