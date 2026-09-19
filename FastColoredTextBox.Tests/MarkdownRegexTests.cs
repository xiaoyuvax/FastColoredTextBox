using System.Text.RegularExpressions;
using Xunit;

namespace FastColoredTextBoxNS.Tests
{
    /// <summary>
    /// Regression tests for issues #1-#4 and #25b: the Markdown regex set from
    /// SyntaxHighlighter.InitMarkdownRegex must behave correctly on multi-line
    /// documents and must not misfire on plain prose.
    /// The patterns below mirror the production ones; any change to the
    /// highlighter must keep these green.
    /// </summary>
    public class MarkdownRegexTests
    {
        private static string Normalize(string s) => s.Replace("\r\n", "\n");

        [Fact]
        public void FencedCodeBlock_MatchesAcrossLines()
        {
            // #1: pattern must span lines (RegexOptions.Singleline was missing).
            const string pattern = @"^```.*?^```";
            var opts = RegexOptions.Multiline | RegexOptions.Singleline;

            const string doc = "intro\n```cs\nint x = 1;\nint y = 2;\n```\nafter";
            var m = Regex.Match(doc, pattern, opts);
            Assert.True(m.Success, "fenced block not matched");
            Assert.Equal("```cs\nint x = 1;\nint y = 2;\n```", Normalize(m.Value));
        }

        [Fact]
        public void FencedCodeBlock_UnclosedBlockNotMatched()
        {
            const string pattern = @"^```.*?^```";
            var opts = RegexOptions.Multiline | RegexOptions.Singleline;

            Assert.False(Regex.IsMatch("```cs\nint x = 1;\n", pattern, opts),
                "unclosed block must not match");
        }

        [Fact]
        public void FencedCodeBlock_TwoAdjacentBlocksMatchedSeparately()
        {
            const string pattern = @"^```.*?^```";
            var opts = RegexOptions.Multiline | RegexOptions.Singleline;

            const string doc = "```a\nx\n```\ntext\n```b\ny\n```";
            var matches = Regex.Matches(doc, pattern, opts);
            Assert.Equal(2, matches.Count);
            Assert.Equal("```a\nx\n```", Normalize(matches[0].Value));
            Assert.Equal("```b\ny\n```", Normalize(matches[1].Value));
        }

        [Fact]
        public void Heading_DoesNotSwallowNextLine()
        {
            // #2: '\s+' ate the newline after '#', merging the following line.
            const string pattern = @"^#{1,6}[ \t]+.+$";
            var opts = RegexOptions.Multiline;

            const string doc = "#\nno heading\n## Real";
            var m = Regex.Match(doc, pattern, opts);
            Assert.True(m.Success, "'## Real' should match");
            Assert.Equal("## Real", m.Value);
            Assert.DoesNotContain('\n', m.Value);
        }

        [Fact]
        public void Heading_SingleHashAloneIsNotHeading()
        {
            const string pattern = @"^#{1,6}[ \t]+.+$";
            Assert.False(Regex.IsMatch("#\nno heading\n", pattern, RegexOptions.Multiline),
                "'#' alone on a line must not match");
        }

        [Fact]
        public void InlineCode_DoesNotSpanLines()
        {
            // #3: [^`] crossed newlines; two distant backticks swallowed everything between.
            const string pattern = @"`[^`\n]+`";

            const string doc = "a `x` b\n\ntext between\n\nmore `y` c";
            var matches = Regex.Matches(doc, pattern);
            Assert.Equal(2, matches.Count);
            Assert.All(matches, m => Assert.DoesNotContain('\n', m.Value));
        }

        [Fact]
        public void ImageNotDoubleStyledAsLink()
        {
            // #4: MdLinkRegex must not match inside ![alt](url).
            const string linkPattern = @"(?<!!)\[.+?\]\(.+?\)";
            const string doc = "![alt](img.png) and [site](https://x)";

            var matches = Regex.Matches(doc, linkPattern);
            Assert.Single(matches);
            Assert.Equal("[site](https://x)", matches[0].Value);
        }

        [Fact]
        public void Italic_MultiplicationSignsNotItalic()
        {
            // #25b: `2 * 3 * 4` must not be highlighted as italic.
            // CommonMark flanking rules: opening star must not be followed by space/digit,
            // closing star must not touch digits or be followed by space-star.
            const string pattern = @"(?<![\d*])\*(?![\s\d*]).+?(?<!\d)\*(?![\d*])";

            Assert.False(Regex.IsMatch("2 * 3 * 4", pattern), "math asterisks must not be italic");
            Assert.False(Regex.IsMatch("2*3*4", pattern), "tight math asterisks must not be italic");
            Assert.True(Regex.IsMatch("some *emphasis* here", pattern), "real emphasis must match");
            Assert.True(Regex.IsMatch("a*b*c", pattern), "tight emphasis must match");
            Assert.True(Regex.IsMatch("*start to end*", pattern), "closing star at end of string must match");
        }

        [Fact]
        public void Blockquote_MatchesCompactAndNested()
        {
            // companion of #25b: '^>\s+' missed '>text' and '>>'.
            const string pattern = @"^>[^\n]*$";

            var matches = Regex.Matches("> text\n>text\n>>nested\nplain", pattern, RegexOptions.Multiline);
            Assert.Equal(3, matches.Count);
        }
    }
}
