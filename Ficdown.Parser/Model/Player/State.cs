namespace Ficdown.Parser.Model.Player
{
    using System.Collections;

    internal class State
    {
        public BitArray PlayerState { get; set; }
        public BitArray ScenesSeen { get; set; }
        public BitArray ActionsToShow { get; set; }
    }
}
