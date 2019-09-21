namespace Ficdown.Parser.Model.Player
{
    using System;
    using System.Collections.Generic;
    using Ficdown.Parser.Player;
    using Story;

    internal class PageState
    {
        public StateManager Manager { get; set; }

        public Guid Id { get; set; }
        public Scene Scene { get; set; }
        public State State { get; set; }
        public State AffectedState { get; set; }

        public IDictionary<string, int> StateMatrix { get; set; }

        public IDictionary<string, string> Links { get; set; }

        private string _uniqueHash;
        private string _compressedHash;

        public string UniqueHash
        {
            get { return _uniqueHash ?? (_uniqueHash = Manager.GetUniqueHash(State, Scene.Key)); }
        }

        public string CompressedHash
        {
            get { return _compressedHash ?? (_compressedHash = Manager.GetCompressedHash(this)); }
        }
    }
}
