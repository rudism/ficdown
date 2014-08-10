namespace Ficdown.Parser.Render
{
    using System.Collections.Generic;
    using Model.Parser;

    internal interface IRenderer
    {
        void Render(IEnumerable<ResolvedPage> pages, string outPath, bool debug);
    }
}
