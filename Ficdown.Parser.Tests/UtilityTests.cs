namespace Ficdown.Parser.Tests
{
    using System.Linq;
    using Parser;
    using Xunit;

    public class UtilityTests
    {
        [Fact]
        public void FullAnchorMatches()
        {
            var anchorStr = @"[Link text](/target-scene)";
            var anchor = Utilities.ParseAnchor(anchorStr);
            Assert.Equal(anchorStr, anchor.Original);

            anchorStr = @"[Link text](?condition-state#toggle-state ""Title text"")";
            anchor = Utilities.ParseAnchor(anchorStr);
            Assert.Equal(anchorStr, anchor.Original);

            anchorStr = @"[Link text](""Title text"")";
            anchor = Utilities.ParseAnchor(anchorStr);
            Assert.Equal(anchorStr, anchor.Original);
        }

        [Fact]
        public void AnchorWithTargetMatches()
        {
            var anchor = Utilities.ParseAnchor(@"[Link text](/target-scene)");
            Assert.Equal("Link text", anchor.Text);
            Assert.Equal("target-scene", anchor.Href.Target);
        }

        [Fact]
        public void AnchorsWithConditionsMatch()
        {
            var anchor = Utilities.ParseAnchor(@"[Link text](?condition-state)");
            Assert.Equal("Link text", anchor.Text);
            Assert.True(anchor.Href.Conditions["condition-state"]);

            anchor = Utilities.ParseAnchor(@"[Link text](?!condition-state)");
            Assert.Equal("Link text", anchor.Text);
            Assert.False(anchor.Href.Conditions["condition-state"]);

            anchor = Utilities.ParseAnchor(@"[Link text](?condition-1&!condition-2)");
            Assert.Equal("Link text", anchor.Text);
            Assert.True(anchor.Href.Conditions["condition-1"]);
            Assert.False(anchor.Href.Conditions["condition-2"]);
        }

        [Fact]
        public void AnchorsWithTogglesMatch()
        {
            var anchor = Utilities.ParseAnchor(@"[Link text](#toggle-state)");
            Assert.Equal("Link text", anchor.Text);
            Assert.Equal("#toggle-state", anchor.Href.Original);

            anchor = Utilities.ParseAnchor(@"[Link text](#toggle-1+toggle-2)");
            Assert.Equal("Link text", anchor.Text);
            Assert.Equal("#toggle-1+toggle-2", anchor.Href.Original);

            anchor = Utilities.ParseAnchor(@"[Link text](#toggle-1+toggle-2)");
            Assert.Equal("Link text", anchor.Text);
            Assert.Equal("#toggle-1+toggle-2", anchor.Href.Original);
        }

        [Fact]
        public void AnchorsWithTitlesMatch()
        {
            var anchor = Utilities.ParseAnchor(@"[Link text](""Title text"")");
            Assert.Equal("Link text", anchor.Text);
            Assert.Equal("Title text", anchor.Title);

            anchor = Utilities.ParseAnchor(@"[Talking to Kid](""Lobby"")");
            Assert.Equal("Talking to Kid", anchor.Text);
            Assert.Equal("Lobby", anchor.Title);
        }

        [Fact]
        public void ComplexAnchorsMatch()
        {
            var anchor = Utilities.ParseAnchor(@"[Link text](/target-scene?condition-state#toggle-state ""Title text"")");
            Assert.Equal("Link text", anchor.Text);
            Assert.Equal("/target-scene?condition-state#toggle-state", anchor.Href.Original);
            Assert.Equal("Title text", anchor.Title);

            anchor = Utilities.ParseAnchor(@"[Link text](/target-scene#toggle-state)");
            Assert.Equal("Link text", anchor.Text);
            Assert.Equal("/target-scene#toggle-state", anchor.Href.Original);

            anchor = Utilities.ParseAnchor(@"[Link text](/target-scene?condition-state)");
            Assert.Equal("Link text", anchor.Text);
            Assert.Equal("/target-scene?condition-state", anchor.Href.Original);

            anchor = Utilities.ParseAnchor(@"[Link text](?condition-state#toggle-state)");
            Assert.Equal("Link text", anchor.Text);
            Assert.Equal("?condition-state#toggle-state", anchor.Href.Original);
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
            Assert.Equal(1, anchors[0].Href.Toggles.Count());

            anchors = Utilities.ParseAnchors("[Anchor](#toggle-1+toggle-2)");
            Assert.Null(anchors[0].Href.Target);
            Assert.Null(anchors[0].Href.Conditions);
            Assert.Equal(2, anchors[0].Href.Toggles.Count());
        }

        [Fact]
        public void ComplexHrefsParse()
        {
            var anchors = Utilities.ParseAnchors("[Anchor](/target-scene?condition-state#toggle-state)");
            Assert.Equal("target-scene", anchors[0].Href.Target);
            Assert.Equal(1, anchors[0].Href.Conditions.Count);
            Assert.True(anchors[0].Href.Conditions["condition-state"]);
            Assert.Equal(1, anchors[0].Href.Toggles.Count());

            anchors = Utilities.ParseAnchors("[Anchor](/target-scene?condition-state)");
            Assert.Equal("target-scene", anchors[0].Href.Target);
            Assert.Equal(1, anchors[0].Href.Conditions.Count);
            Assert.True(anchors[0].Href.Conditions["condition-state"]);
            Assert.Null(anchors[0].Href.Toggles);

            anchors = Utilities.ParseAnchors("[Anchor](/target-scene#toggle-state)");
            Assert.Equal("target-scene", anchors[0].Href.Target);
            Assert.Null(anchors[0].Href.Conditions);
            Assert.Equal(1, anchors[0].Href.Toggles.Count());

            anchors = Utilities.ParseAnchors("[Anchor](?condition-one&!condition-two#toggle-one+toggle-two)");
            Assert.Null(anchors[0].Href.Target);
            Assert.Equal(2, anchors[0].Href.Conditions.Count);
            Assert.True(anchors[0].Href.Conditions["condition-one"]);
            Assert.False(anchors[0].Href.Conditions["condition-two"]);
            Assert.Equal(2, anchors[0].Href.Toggles.Count());
        }
    }
}
