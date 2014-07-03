namespace Ficdown.Parser.Model.Player
{
    using System.Collections.Generic;

    internal class PlayerState : Dictionary<string, bool>
    {
        public PlayerState(IDictionary<string, bool> copyFrom) : base(copyFrom)
        {
        }

        public PlayerState Clone()
        {
            return new PlayerState(this);
        }

        public void VisitedScene(string sceneId)
        {
            var key = string.Format(">{0}", sceneId);
            if (!ContainsKey(key)) Add(key, true);
        }
    }
}
