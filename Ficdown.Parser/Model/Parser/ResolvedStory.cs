namespace Ficdown.Parser.Model.Parser
{
    using System.Collections.Generic;

    public class ResolvedStory
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string FirstPage { get; set; }
        public IEnumerable<ResolvedPage> Pages { get; set; }
    }
}
