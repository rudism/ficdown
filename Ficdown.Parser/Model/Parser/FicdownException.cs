namespace Ficdown.Parser.Model.Parser
{
    using System;

    public class FicdownException : Exception
    {
        public string BlockName { get; private set; }
        public int? LineNumber { get; private set; }

        public FicdownException(string blockName, int? lineNumber, string message) : base(message)
        {
            BlockName = blockName;
            LineNumber = lineNumber;
        }

        public FicdownException(string message) : base(message) { }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(BlockName)
                ? string.Format("Error in block \"{0}\" (Line {1}): {2}",
                    BlockName,
                    LineNumber.HasValue
                        ? LineNumber.ToString()
                        : "unknown", Message)
                : string.Format("Error: {0}", Message);
        }
    }
}
