namespace Ficdown.Parser.Model.Story
{
    using System.Collections.Generic;

    public class Action
    {
        public string Description { get; set; }
        public IList<string> Conditions { get; set; }
        public IList<string> Toggles { get; set; }
    }
}
