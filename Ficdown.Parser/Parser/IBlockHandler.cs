namespace Ficdown.Parser.Parser
{
    using System.Collections.Generic;
    using Model.Parser;
    using Model.Story;

    internal interface IBlockHandler
    {
        IEnumerable<Block> ExtractBlocks(IEnumerable<string> lines);
        Story ParseBlocks(IEnumerable<Block> blocks);
    }
}
