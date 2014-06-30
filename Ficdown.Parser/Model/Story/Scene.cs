namespace Ficdown.Parser.Model.Story
{
    using System.Collections.Generic;

    public class Scene
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<string> Conditions { get; set; }
    }
}