namespace Ficdown.Parser.Engine
{
    using System.Text;
    using System.Text.RegularExpressions;

    internal static class RegexLib
    {
        public static Regex Anchors =
            new Regex(
                string.Format(@"(?<anchor>\[(?<text>{0})\]\([ ]*(?<href>{1})[ ]*\))", GetNestedBracketsPattern(),
                    GetNestedParensPattern()), RegexOptions.Singleline | RegexOptions.Compiled);

        public static Regex Href = new Regex(@"^(\?(?<conditions>[^#]+))?(#(?<toggles>.+))?$", RegexOptions.Compiled);

        private const int _nestDepth = 6;

        private static string RepeatString(string text, int count)
        {
            var sb = new StringBuilder(text.Length * count);
            for (int i = 0; i < count; i++)
                sb.Append(text);
            return sb.ToString();
        }

        private static string _nestedBracketsPattern;
        private static string GetNestedBracketsPattern()
        {
            if (_nestedBracketsPattern == null)
                _nestedBracketsPattern =
                    RepeatString(@"(?>[^\[\]]+|\[", _nestDepth) + RepeatString(@"\])*", _nestDepth);
            return _nestedBracketsPattern;
        }

        private static string _nestedParensPattern;
        private static string GetNestedParensPattern()
        {
            if (_nestedParensPattern == null)
                _nestedParensPattern =
                    RepeatString(@"(?>[^()\s]+|\(", _nestDepth) + RepeatString(@"\))*", _nestDepth);
            return _nestedParensPattern;
        }
    }
}
