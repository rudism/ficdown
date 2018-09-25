namespace Ficdown.Parser.Model.Parser
{
    using System;

    public class FicdownException : Exception
    {
        public string BlockName { get; private set; }
        public int? LineNumber { get; private set; }
        public int? ColNumber { get; private set; }

        public FicdownException(string blockName, string message, int? lineNumber = null, int? colNumber = null) : base(message)
        {
            BlockName = blockName;
            LineNumber = lineNumber;
            ColNumber = colNumber;
        }

        public FicdownException(string message) : base(message) { }

        public override string ToString()
        {
            return string.Format("Error L{0},{1}: {2}",
                LineNumber ?? 1,
                ColNumber ?? 1,
                !string.IsNullOrEmpty(BlockName)
                    ? string.Format("\"{0}\": {1}", BlockName, Message)
                    : Message);
        }
    }
}
