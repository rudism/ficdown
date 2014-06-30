namespace Ficdown.Parser.Engine
{
    using Model.Story;

    public interface ISceneLinker
    {
        void ExpandScenes(Story story);
    }
}
