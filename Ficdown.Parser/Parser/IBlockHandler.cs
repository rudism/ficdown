namespace Ficdown.Parser.Parser
{
    using System.Collections.Generic;
    using Model.Parser;
    using Model.Story;

    internal interface IBlockHandler
    {
        List<FicdownException> Warnings { set; }
        IEnumerable<Block> ExtractBlocks(IEnumerable<string> lines);
        Story ParseBlocks(IEnumerable<Block> blocks);
    }
}
