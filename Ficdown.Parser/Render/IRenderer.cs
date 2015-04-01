namespace Ficdown.Parser.Render
{
    using System.Security.Cryptography.X509Certificates;
    using Model.Parser;

    public interface IRenderer
    {
        string IndexTemplate { get; set; }
        string SceneTemplate { get; set; }
        string StylesTemplate { get; set; }
        string ImageDir { get; set; }
        void Render(ResolvedStory story, string outPath, bool debug);
    }
}
