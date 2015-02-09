namespace Ficdown.Parser.Render
{
    using Model.Parser;

    public interface IRenderer
    {
        void Render(ResolvedStory story, string outPath, bool debug);
    }
}
