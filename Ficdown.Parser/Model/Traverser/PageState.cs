namespace Ficdown.Parser.Model.Traverser
{
    using System;
    using System.Collections.Generic;
    using Ficdown.Parser.Player;
    using Story;

    internal class PageState
    {
        public Guid Id { get; set; }
        public Scene Scene { get; set; }
        public State State { get; set; }
        public State AffectedState { get; set; }

        public IDictionary<string, int> StateMatrix { get; set; } 

        public string Resolved { get; set; }
        public IDictionary<string, string> Links { get; set; }

        private string _uniqueHash;
        private string _compressedHash;

        public string UniqueHash
        {
            get { return _uniqueHash ?? (_uniqueHash = StateManager.GetUniqueHash(State, Scene.Key)); }
        }

        public string CompressedHash
        {
            get { return _compressedHash ?? (_compressedHash = StateManager.GetCompressedHash(this)); }
        }
    }
}