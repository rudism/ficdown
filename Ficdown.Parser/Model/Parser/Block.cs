namespace Ficdown.Parser.Model.Parser
{
    using System.Collections.Generic;

    public class Block
    {
        public BlockType Type { get; set; }
        public string Name { get; set; }
        public IList<string> Lines { get; set; }
    }
}
