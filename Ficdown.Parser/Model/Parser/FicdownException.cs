namespace Ficdown.Parser.Model.Parser
{
    using System;

    public class FicdownException : Exception
    {
        private string _blockName;
        private int? _lineNumber;
        private string _message;

        public FicdownException(string blockName, int? lineNumber, string message) : base(message)
        {
            _blockName = blockName;
            _lineNumber = lineNumber;
            _message = message;
        }

        public FicdownException(string message) : base(message)
        {
            _message = message;
        }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(_blockName)
                ? string.Format("Error in block \"{0}\" (Line {1}): {2}",
                    _blockName,
                    _lineNumber.HasValue
                        ? _lineNumber.ToString()
                        : "unknown", _message)
                : string.Format("Error: {0}", _message);
        }
    }
}
