namespace Ficdown.Parser.Tests.Extensions
{
    using System;
    using System.Collections.Generic;

    public static class TestExtensions
    {
        public static IEnumerable<string> ToLines(this string content)
        {
            return content.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
        }
    }
}
