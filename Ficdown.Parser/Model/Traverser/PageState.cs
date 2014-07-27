namespace Ficdown.Parser.Model.Traverser
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Story;

    internal class PageState
    {
        public Guid Id { get; set; }
        public Scene Scene { get; set; }
        public State State { get; set; }
        public State AffectedState { get; set; }

        public string Resolved { get; set; }
        public IDictionary<string, string> Links { get; set; }

        private string _uniqueHash;
        public string UniqueHash
        {
            get
            {
                if (_uniqueHash == null)
                {
                    var combined =
                        new bool[State.PlayerState.Count + State.ScenesSeen.Count + State.ActionsToShow.Count];
                    State.PlayerState.CopyTo(combined, 0);
                    State.ScenesSeen.CopyTo(combined, State.PlayerState.Count);
                    State.ActionsToShow.CopyTo(combined, State.PlayerState.Count + State.ScenesSeen.Count);
                    var ba = new BitArray(combined);
                    var byteSize = (int) Math.Ceiling(combined.Length/8.0);
                    var encoded = new byte[byteSize];
                    ba.CopyTo(encoded, 0);
                    _uniqueHash = string.Format("{0}=={1}", Scene.Key, Convert.ToBase64String(encoded));
                }
                return _uniqueHash;
            }
        }
    }
}
