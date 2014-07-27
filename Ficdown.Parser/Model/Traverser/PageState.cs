namespace Ficdown.Parser.Model.Traverser
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Story;

    internal class PageState
    {
        public Guid Id { get; set; }
        public BitArray PlayerState { get; set; }
        public BitArray ScenesSeen { get; set; }
        public BitArray ActionsToShow { get; set; }
        public Scene Scene { get; set; }

        public string Resolved { get; set; }
        public IDictionary<string, string> Links { get; set; }

        private string _uniqueHash;
        public string UniqueHash
        {
            get
            {
                if (_uniqueHash == null)
                {
                    var combined = new bool[PlayerState.Count + ScenesSeen.Count + ActionsToShow.Count];
                    PlayerState.CopyTo(combined, 0);
                    ScenesSeen.CopyTo(combined, PlayerState.Count);
                    ActionsToShow.CopyTo(combined, PlayerState.Count + ScenesSeen.Count);
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
