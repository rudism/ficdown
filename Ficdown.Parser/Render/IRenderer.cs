namespace Ficdown.Parser.Render
{
    using Model.Parser;

    internal interface IRenderer
    {
        void Render(ResolvedStory story, string outPath, bool debug);
    }
}
