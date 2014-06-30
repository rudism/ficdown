namespace Ficdown.Parser.Engine
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Model.Story;

    public class SceneLinker : ISceneLinker
    {
        public void ExpandScenes(Story story)
        {
            var newScenes = new Dictionary<string, IList<Scene>>();
            foreach(var key in story.Scenes.Keys)
            {
                newScenes.Add(key, new List<Scene>());
                foreach (var scene in story.Scenes[key])
                {
                    var anchors = RegexLib.Anchors.Matches(scene.Description);
                    foreach (Match anchor in anchors)
                    {
                        string target;
                        IList<string> conditions, toggles;
                        Utilities.ParseHref(anchor.Groups["href"].Value, out target, out conditions, out toggles);
                    }
                }
            }
        }
    }
}
