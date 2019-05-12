namespace Ficdown.Parser.Render
{
    using System.Collections.Generic;
    using Model.Parser;

    public interface IRenderer
    {
        List<FicdownException> Warnings { set; }
        string IndexTemplate { get; set; }
        string SceneTemplate { get; set; }
        string StylesTemplate { get; set; }
        string ImageDir { get; set; }
        void Render(ResolvedStory story, string outPath, bool debug);
    }
}
