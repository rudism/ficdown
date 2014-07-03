namespace Ficdown.Parser.Parser
{
    using System.Collections.Generic;
    using Model.Parser;
    using Model.Story;

    public interface IBlockHandler
    {
        IEnumerable<Block> ExtractBlocks(IEnumerable<string> lines);
        Story ParseBlocks(IEnumerable<Block> blocks);
    }
}
