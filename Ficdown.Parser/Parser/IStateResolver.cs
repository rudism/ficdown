namespace Ficdown.Parser.Parser
{
    using System.Collections.Generic;

    public interface IStateResolver
    {
        string Resolve(string description, IDictionary<string, bool> playState, bool firstSeen);
    }
}
