namespace Ficdown.Parser.Tests
{
    using System.Collections.Generic;
    using Engine;
    using Xunit;

    public class UtilityTests
    {
        [Fact]
        public void AnchorWithTargetMatches()
        {
            var anchor = RegexLib.Anchors.Match(@"[Link text](/target-scene)");
            Assert.True(anchor.Success);
            Assert.Equal("Link text", anchor.Groups["text"].Value);
            Assert.Equal("/target-scene", anchor.Groups["href"].Value);
        }

        [Fact]
        public void AnchorsWithConditionsMatch()
        {
            var anchor = RegexLib.Anchors.Match(@"[Link text](?condition-state)");
            Assert.True(anchor.Success);
            Assert.Equal("Link text", anchor.Groups["text"].Value);
            Assert.Equal("?condition-state", anchor.Groups["href"].Value);

            anchor = RegexLib.Anchors.Match(@"[Link text](?!condition-state)");
            Assert.True(anchor.Success);
            Assert.Equal("Link text", anchor.Groups["text"].Value);
            Assert.Equal("?!condition-state", anchor.Groups["href"].Value);

            anchor = RegexLib.Anchors.Match(@"[Link text](?condition-1&!condition-2)");
            Assert.True(anchor.Success);
            Assert.Equal("Link text", anchor.Groups["text"].Value);
            Assert.Equal("?condition-1&!condition-2", anchor.Groups["href"].Value);
        }

        [Fact]
        public void AnchorsWithTogglesMatch()
        {
            var anchor = RegexLib.Anchors.Match(@"[Link text](#toggle-state)");
            Assert.True(anchor.Success);
            Assert.Equal("Link text", anchor.Groups["text"].Value);
            Assert.Equal("#toggle-state", anchor.Groups["href"].Value);

            anchor = RegexLib.Anchors.Match(@"[Link text](#toggle-1+toggle-2)");
            Assert.True(anchor.Success);
            Assert.Equal("Link text", anchor.Groups["text"].Value);
            Assert.Equal("#toggle-1+toggle-2", anchor.Groups["href"].Value);
        }

        [Fact]
        public void ComplexAnchorsMatch()
        {
            var anchor = RegexLib.Anchors.Match(@"[Link text](/target-scene?condition-state#toggle-state)");
            Assert.True(anchor.Success);
            Assert.Equal("Link text", anchor.Groups["text"].Value);
            Assert.Equal("/target-scene?condition-state#toggle-state", anchor.Groups["href"].Value);

            anchor = RegexLib.Anchors.Match(@"[Link text](/target-scene#toggle-state)");
            Assert.True(anchor.Success);
            Assert.Equal("Link text", anchor.Groups["text"].Value);
            Assert.Equal("/target-scene#toggle-state", anchor.Groups["href"].Value);

            anchor = RegexLib.Anchors.Match(@"[Link text](/target-scene?condition-state)");
            Assert.True(anchor.Success);
            Assert.Equal("Link text", anchor.Groups["text"].Value);
            Assert.Equal("/target-scene?condition-state", anchor.Groups["href"].Value);

            anchor = RegexLib.Anchors.Match(@"[Link text](?condition-state#toggle-state)");
            Assert.True(anchor.Success);
            Assert.Equal("Link text", anchor.Groups["text"].Value);
            Assert.Equal("?condition-state#toggle-state", anchor.Groups["href"].Value);
        }

        [Fact]
        public void HrefWithTargetParses()
        {
            string target;
            IList<string> conditions, toggles;
            Utilities.ParseHref("/target-scene", out target, out conditions, out toggles);
            Assert.Equal("target-scene", target);
            Assert.Null(conditions);
            Assert.Null(toggles);
        }

        [Fact]
        public void HrefsWithConditionsParse()
        {
            string target;
            IList<string> conditions, toggles;
            Utilities.ParseHref("?condition-state", out target, out conditions, out toggles);
            Assert.Null(target);
            Assert.Equal(1, conditions.Count);
            Assert.Contains("condition-state", conditions);
            Assert.Null(toggles);

            Utilities.ParseHref("?!condition-state", out target, out conditions, out toggles);
            Assert.Null(target);
            Assert.Equal(1, conditions.Count);
            Assert.Contains("!condition-state", conditions);
            Assert.Null(toggles);

            Utilities.ParseHref("?condition-1&!condition-2", out target, out conditions, out toggles);
            Assert.Null(target);
            Assert.Equal(2, conditions.Count);
            Assert.Contains("condition-1", conditions);
            Assert.Contains("!condition-2", conditions);
            Assert.Null(toggles);
        }

        [Fact]
        public void HrefsWithTogglesParse()
        {
            string target;
            IList<string> conditions, toggles;
            Utilities.ParseHref("#toggle-state", out target, out conditions, out toggles);
            Assert.Null(target);
            Assert.Null(conditions);
            Assert.Equal(1, toggles.Count);
            Assert.Contains("toggle-state", toggles);

            Utilities.ParseHref("#toggle-1+toggle-2", out target, out conditions, out toggles);
            Assert.Null(target);
            Assert.Null(conditions);
            Assert.Equal(2, toggles.Count);
            Assert.Contains("toggle-1", toggles);
            Assert.Contains("toggle-2", toggles);
        }

        [Fact]
        public void ComplexHrefsParse()
        {
            string target;
            IList<string> conditions, toggles;
            Utilities.ParseHref("/target-scene?condition-state#toggle-state", out target, out conditions, out toggles);
            Assert.Equal("target-scene", target);
            Assert.Equal(1, conditions.Count);
            Assert.Contains("condition-state", conditions);
            Assert.Equal(1, toggles.Count);
            Assert.Contains("toggle-state", toggles);

            Utilities.ParseHref("/target-scene?condition-state", out target, out conditions, out toggles);
            Assert.Equal("target-scene", target);
            Assert.Equal(1, conditions.Count);
            Assert.Contains("condition-state", conditions);
            Assert.Null(toggles);

            Utilities.ParseHref("/target-scene#toggle-state", out target, out conditions, out toggles);
            Assert.Equal("target-scene", target);
            Assert.Null(conditions);
            Assert.Equal(1, toggles.Count);
            Assert.Contains("toggle-state", toggles);

            Utilities.ParseHref("?!condition-one&condition-two#toggle-state", out target, out conditions, out toggles);
            Assert.Null(target);
            Assert.Equal(2, conditions.Count);
            Assert.Contains("!condition-one", conditions);
            Assert.Contains("condition-two", conditions);
            Assert.Equal(1, toggles.Count);
            Assert.Contains("toggle-state", toggles);
        }
    }
}
