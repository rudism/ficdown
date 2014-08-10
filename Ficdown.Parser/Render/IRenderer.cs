namespace Ficdown.Parser.Render
{
    using System.Collections.Generic;
    using Model.Parser;
    using Model.Story;

    internal interface IRenderer
    {
        void Render(IEnumerable<ResolvedPage> pages, string outPath);
    }
}
