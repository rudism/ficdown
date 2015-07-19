namespace Ficdown.Parser.Parser
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public static class ParserExtensions
    {
        public static string ToHrefString(this IDictionary<string, bool> values, string separator)
        {
            return values != null
                ? string.Join(separator,
                    values.Where(v => !v.Key.StartsWith(">"))
                        .Select(v => string.Format("{0}{1}", v.Value ? null : "!", v.Key))
                        .ToArray())
                : null;
        }

        public static string ToHrefString(this IEnumerable<string> values, string separator)
        {
            return values != null ? string.Join(separator, values.ToArray()) : null;
        }
    }
}
