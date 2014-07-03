namespace Ficdown.Parser.Model.Story
{
    using System.Collections.Generic;

    public class Story
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string FirstScene { get; set; }
        public IDictionary<string, IList<Scene>> Scenes { get; set; }
        public IDictionary<string, Action> Actions { get; set; }
    }
}
