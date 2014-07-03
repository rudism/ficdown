namespace Ficdown.Parser.Tests
{
    using System;
    using Parser;
    using Xunit;

    public class UtilityTests
    {
        [Fact]
        public void FullAnchorMatches()
        {
            Console.WriteLine(RegexLib.Href.ToString());
            var anchorStr = @"[Link text](/target-scene)";
            var anchor = RegexLib.Anchors.Match(anchorStr);
            Assert.Equal(anchorStr, anchor.Groups["anchor"].Value);

            anchorStr = @"[Link text](?condition-state#toggle-state ""Title text"")";
            anchor = RegexLib.Anchors.Match(anchorStr);
            Assert.Equal(anchorStr, anchor.Groups["anchor"].Value);

            anchorStr = @"[Link text](""Title text"")";
            anchor = RegexLib.Anchors.Match(anchorStr);
            Assert.Equal(anchorStr, anchor.Groups["anchor"].Value);
        }

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

            anchor = RegexLib.Anchors.Match(@"[Link text](#toggle-1+!toggle-2)");
            Assert.True(anchor.Success);
            Assert.Equal("Link text", anchor.Groups["text"].Value);
            Assert.Equal("#toggle-1+!toggle-2", anchor.Groups["href"].Value);
        }

        [Fact]
        public void AnchorsWithTitlesMatch()
        {
            var anchor = RegexLib.Anchors.Match(@"[Link text](""Title text"")");
            Assert.True(anchor.Success);
            Assert.Equal("Link text", anchor.Groups["text"].Value);
            Assert.Equal("Title text", anchor.Groups["title"].Value);
        }

        [Fact]
        public void ComplexAnchorsMatch()
        {
            var anchor = RegexLib.Anchors.Match(@"[Link text](/target-scene?condition-state#toggle-state ""Title text"")");
            Assert.True(anchor.Success);
            Assert.Equal("Link text", anchor.Groups["text"].Value);
            Assert.Equal("/target-scene?condition-state#toggle-state", anchor.Groups["href"].Value);
            Assert.Equal("Title text", anchor.Groups["title"].Value);

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
            var anchors = Utilities.ParseAnchors("[Anchor](/target-scene)");
            Assert.Equal("target-scene", anchors[0].Href.Target);
            Assert.Null(anchors[0].Href.Conditions);
            Assert.Null(anchors[0].Href.Toggles);
        }

        [Fact]
        public void HrefsWithConditionsParse()
        {
            var anchors = Utilities.ParseAnchors("[Anchor](?condition-state)");
            Assert.Null(anchors[0].Href.Target);
            Assert.Equal(1, anchors[0].Href.Conditions.Count);
            Assert.True(anchors[0].Href.Conditions["condition-state"]);
            Assert.Null(anchors[0].Href.Toggles);

            anchors = Utilities.ParseAnchors("[Anchor](?condition-1&!condition-2)");
            Assert.Null(anchors[0].Href.Target);
            Assert.Equal(2, anchors[0].Href.Conditions.Count);
            Assert.True(anchors[0].Href.Conditions["condition-1"]);
            Assert.False(anchors[0].Href.Conditions["condition-2"]);
            Assert.Null(anchors[0].Href.Toggles);
        }

        [Fact]
        public void HrefsWithTogglesParse()
        {
            var anchors = Utilities.ParseAnchors("[Anchor](#toggle-state)");
            Assert.Null(anchors[0].Href.Target);
            Assert.Null(anchors[0].Href.Conditions);
            Assert.Equal(1, anchors[0].Href.Toggles.Count);
            Assert.True(anchors[0].Href.Toggles["toggle-state"]);

            anchors = Utilities.ParseAnchors("[Anchor](#toggle-1+!toggle-2)");
            Assert.Null(anchors[0].Href.Target);
            Assert.Null(anchors[0].Href.Conditions);
            Assert.Equal(2, anchors[0].Href.Toggles.Count);
            Assert.True(anchors[0].Href.Toggles["toggle-1"]);
            Assert.False(anchors[0].Href.Toggles["toggle-2"]);
        }

        [Fact]
        public void ComplexHrefsParse()
        {
            var anchors = Utilities.ParseAnchors("[Anchor](/target-scene?condition-state#toggle-state)");
            Assert.Equal("target-scene", anchors[0].Href.Target);
            Assert.Equal(1, anchors[0].Href.Conditions.Count);
            Assert.True(anchors[0].Href.Conditions["condition-state"]);
            Assert.Equal(1, anchors[0].Href.Toggles.Count);
            Assert.True(anchors[0].Href.Toggles["toggle-state"]);

            anchors = Utilities.ParseAnchors("[Anchor](/target-scene?condition-state)");
            Assert.Equal("target-scene", anchors[0].Href.Target);
            Assert.Equal(1, anchors[0].Href.Conditions.Count);
            Assert.True(anchors[0].Href.Conditions["condition-state"]);
            Assert.Null(anchors[0].Href.Toggles);

            anchors = Utilities.ParseAnchors("[Anchor](/target-scene#toggle-state)");
            Assert.Equal("target-scene", anchors[0].Href.Target);
            Assert.Null(anchors[0].Href.Conditions);
            Assert.Equal(1, anchors[0].Href.Toggles.Count);
            Assert.True(anchors[0].Href.Toggles["toggle-state"]);

            anchors = Utilities.ParseAnchors("[Anchor](?condition-one&!condition-two#toggle-one+!toggle-two)");
            Assert.Null(anchors[0].Href.Target);
            Assert.Equal(2, anchors[0].Href.Conditions.Count);
            Assert.True(anchors[0].Href.Conditions["condition-one"]);
            Assert.False(anchors[0].Href.Conditions["condition-two"]);
            Assert.Equal(2, anchors[0].Href.Toggles.Count);
            Assert.True(anchors[0].Href.Toggles["toggle-one"]);
            Assert.False(anchors[0].Href.Toggles["toggle-two"]);
        }
    }
}
