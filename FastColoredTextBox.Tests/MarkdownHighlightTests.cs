using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FastColoredTextBoxNS.Text;
using FastColoredTextBoxNS.Types;
using Xunit;

namespace FastColoredTextBoxNS.Tests
{
    /// <summary>
    /// Behaviour tests for the Markdown highlighter. They run the real
    /// SyntaxHighlighter.MarkdownSyntaxHighlight over a real control and assert which style ended up
    /// on which character, so a change to the highlighter that breaks a rule fails here. Testing the
    /// regexes on their own only proves the regexes agree with the test file.
    /// </summary>
    public class MarkdownHighlightTests
    {
        #region harness

        private static FastColoredTextBox Highlight(string doc)
        {
            var tb = new FastColoredTextBox
            {
                Language = Language.Markdown,
                HighlightingRangeType = HighlightingRangeType.AllTextRange
            };
            tb.Text = doc;
            return tb;
        }

        private static string Col(FastColoredTextBox tb, int line, int col)
        {
            var c = tb[line][col];
            if (c.Styles == null)
                return "";
            var sb = new StringBuilder();
            for (int i = 0; i <= c.LastStyleIndex; i++)
                sb.Append(Name(tb, c.Styles[i])).Append('+');
            return sb.ToString().TrimEnd('+');
        }

        private static string Name(FastColoredTextBox tb, Style style)
        {
            if (style == null)
                return "NULL";
            foreach (var (s, name) in Named(tb))
                if (ReferenceEquals(s, style))
                    return name;
            return style.GetType().Name;
        }

        /// <summary>True when the char carries the given style instance</summary>
        private static bool HasStyle(FastColoredTextBox tb, int line, int col, Style style)
        {
            var c = tb[line][col];
            if (c.Styles == null || style == null)
                return false;
            for (int i = 0; i <= c.LastStyleIndex; i++)
                if (ReferenceEquals(c.Styles[i], style))
                    return true;
            return false;
        }

        /// <summary>
        /// Markdown styles only, for readable assertions. A few of them share a style object with
        /// another slot (MdHrStyle is MdStrikethroughStyle, MdAutoLinkStyle is MdLinkStyle), those
        /// are checked with HasStyle instead so the two cannot be confused.
        /// </summary>
        private static (Style, string)[] Named(FastColoredTextBox tb)
        {
            var h = tb.SyntaxHighlighter;
            return new[]
            {
                (h.MdH1Style, "H1"),
                (h.MdH2Style, "H2"),
                (h.MdH3Style, "H3"),
                (h.MdHeadingStyle, "H4"),
                (h.MdBoldStyle, "B"),
                (h.MdItalicStyle, "I"),
                (h.MdStrikethroughStyle, "S"),
                (h.MdInlineCodeStyle, "CODE"),
                (h.MdCodeBlockStyle, "CB"),
                (h.MdImageStyle, "IMG"),
                (h.MdBlockquoteStyle, "Q"),
                (h.MdListStyle, "LI"),
                (h.MdTaskListDoneStyle, "DONE"),
                (h.MdMarkerStyle, "HIDE"),
            };
        }

        /// <summary>Style names of every char of a line, space separated</summary>
        private static string Line(FastColoredTextBox tb, int line) =>
            string.Join(" ", Enumerable.Range(0, tb[line].Count).Select(x => Col(tb, line, x)));

        #endregion

        #region fenced code blocks

        [Fact]
        public void FencedBlock_ContentIsNotHighlightedAsMarkdown()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("```cs\n# not a heading\n- not a list\n**not bold** [not](a link)\n```");
                for (int y = 0; y < 4; y++)
                    foreach (var tag in Line(tb, y).Split(' '))
                        Assert.DoesNotContain("H", tag);
                // the fence style is the only thing on the content
                Assert.Equal("CB", Col(tb, 1, 0));
                Assert.Equal("CB", Col(tb, 1, 2));
                Assert.Equal("CB", Col(tb, 2, 2));
                Assert.Equal("CB", Col(tb, 3, 10));
            });
        }

        [Fact]
        public void FencedBlock_TildeFenceWorks()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("~~~python\n# a comment\n~~~");
                Assert.Equal("CB", Col(tb, 0, 0));
                Assert.Equal("CB", Col(tb, 1, 0));
                Assert.Equal("CB", Col(tb, 2, 0));
            });
        }

        [Fact]
        public void FencedBlock_UnclosedRunsToEndOfDocument()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("text\n```\n# still code\nmore code");
                Assert.Equal("", Col(tb, 0, 0));
                Assert.Equal("CB", Col(tb, 1, 0));
                Assert.Equal("CB", Col(tb, 3, 0));
            });
        }

        [Fact]
        public void FencedBlock_InfoStringLanguageIsHighlighted()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("```csharp\n// note\nvar s = \"x\";\n```");
                var h = tb.SyntaxHighlighter;
                Assert.True(HasStyle(tb, 1, 3, h.CommentStyle), "// note must be a comment");
                Assert.True(HasStyle(tb, 2, 0, h.KeywordStyle), "var must be a keyword");
                Assert.True(HasStyle(tb, 2, 8, h.StringStyle), "\"x\" must be a string");
                //the block tint survives the nested styles
                Assert.True(HasStyle(tb, 1, 0, h.MdCodeBlockStyle));
            });
        }

        [Fact]
        public void FencedBlock_MarkdownFenceDoesNotRecurse()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("```markdown\n# not a heading\n```");
                Assert.Equal("CB", Col(tb, 1, 2));
            });
        }

        [Fact]
        public void FencedBlock_UnknownLanguageStaysPlain()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("```notalanguage\n# still code\n```");
                Assert.Equal("CB", Col(tb, 1, 0));
                Assert.Equal("CB", Col(tb, 1, 2));
            });
        }

        #endregion

        #region headings

        [Fact]
        public void Heading_LevelPicksTheStyle()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("# one\n## two\n### three\n#### four\n###### six");
                Assert.Equal("H1", Col(tb, 0, 2));
                Assert.Equal("H2", Col(tb, 1, 3));
                Assert.Equal("H3", Col(tb, 2, 4));
                Assert.Equal("H4", Col(tb, 3, 5));
                Assert.Equal("H4", Col(tb, 4, 7));
            });
        }

        [Fact]
        public void Heading_SkipsMarkersAndClosingHashes()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("## Title ##");
                Assert.Equal("HIDE", Col(tb, 0, 0));
                Assert.Equal("HIDE", Col(tb, 0, 2));
                Assert.Equal("H2", Col(tb, 0, 3));
                Assert.Equal("H2", Col(tb, 0, 7));
                Assert.Equal("HIDE", Col(tb, 0, 9));
            });
        }

        [Fact]
        public void Heading_HashWithoutSpaceIsNotAHeading()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("#hashtag\n#\n   ### indented");
                Assert.Equal("", Col(tb, 0, 1));
                Assert.Equal("", Col(tb, 1, 0));
                Assert.Equal("H3", Col(tb, 2, 7));
            });
        }

        #endregion

        #region inline

        [Fact]
        public void Bold_StarAndScore()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("**a**\n__b__");
                Assert.Equal("B", Col(tb, 0, 2));
                Assert.Equal("B", Col(tb, 1, 2));
            });
        }

        /// <summary>
        /// Markdown prose paints no text background: inline code is told apart by its colour, code
        /// blocks and quotes by their left bar. The old inline code was magenta on a grey chip.
        /// </summary>
        [Fact]
        public void Prose_HasNoTextBackground_AndCodeStaysReadableOnBothThemes()
        {
            StaRunner.Run(() =>
            {
                foreach (var (back, fore, darker) in new[]
                {
                    (Color.WhiteSmoke, Color.FromArgb(27, 38, 51), true),
                    (Color.FromArgb(0x1F, 0x1F, 0x1F), Color.FromArgb(0xDC, 0xDC, 0xDC), false),
                })
                {
                    using var tb = new FastColoredTextBox { BackColor = back, ForeColor = fore };
                    tb.Text = "a `code` b";
                    tb.SyntaxHighlighter.MarkdownBodyHighlight(tb.Range);

                    var h = tb.SyntaxHighlighter;
                    var code = (TextStyle)h.MdInlineCodeStyle;
                    Assert.Null(code.BackgroundBrush);
                    Assert.Null(((MarkdownBlockStyle)h.MdCodeBlockStyle).BackgroundBrush);
                    Assert.Null(((MarkdownBlockStyle)h.MdBlockquoteStyle).BackgroundBrush);

                    var codeFore = ((SolidBrush)code.ForeBrush).Color;
                    Assert.Equal(darker, codeFore.GetBrightness() < back.GetBrightness());
                }
            });
        }

        /// <summary>
        /// The block bar sits just left of the first glyph: without a tint it is the only mark of
        /// the block, so it has to read as the block's left edge (it used to hide in the gutter,
        /// which left a quote looking like ordinary prose). A bar at the text origin would cut
        /// through a CJK character, which fills its cell.
        /// </summary>
        [Fact]
        public void BlockBar_SitsJustLeftOfTheFirstGlyph()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox
                {
                    BackColor = Color.White,
                    ForeColor = Color.FromArgb(27, 38, 51),
                    LeftPadding = 20,
                    Paddings = new Padding(2),
                    Width = 300,
                    Height = 80,
                };
                tb.Text = "> quoted";
                tb.SyntaxHighlighter.MarkdownBodyHighlight(tb.Range);

                var style = (MarkdownBlockStyle)tb.SyntaxHighlighter.MdBlockquoteStyle;
                Color bar = ((SolidBrush)style.BarBrush).Color;
                using var bmp = new Bitmap(tb.Width, tb.Height);
                tb.DrawToBitmap(bmp, new Rectangle(0, 0, tb.Width, tb.Height));

                int y = tb.Paddings.Top + tb.CharHeight / 2;
                int textX = tb.LeftIndent + tb.Paddings.Left;
                int barX = textX - style.BarWidth - 2 + style.BarWidth / 2;
                Assert.Equal(bar.ToArgb(), bmp.GetPixel(barX, y).ToArgb());
                Assert.NotEqual(bar.ToArgb(), bmp.GetPixel(textX, y).ToArgb());
            });
        }

        [Fact]
        public void Italic_StarAndScore_ButNotSnakeCase()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("*a* _b_ snake_case\n2 * 3 * 4");
                Assert.True(HasStyle(tb, 0, 1, tb.SyntaxHighlighter.MdItalicStyle));
                Assert.True(HasStyle(tb, 0, 5, tb.SyntaxHighlighter.MdUnderscoreItalicStyle));
                Assert.False(HasStyle(tb, 0, 12, tb.SyntaxHighlighter.MdItalicStyle));
                Assert.False(HasStyle(tb, 1, 2, tb.SyntaxHighlighter.MdItalicStyle));
                Assert.False(HasStyle(tb, 1, 6, tb.SyntaxHighlighter.MdItalicStyle));
            });
        }

        [Fact]
        public void Italic_CommonMarkIntrawordExample()
        {
            StaRunner.Run(() =>
            {
                //spec example: foo*bar* and 5*6*78 are emphasis
                using var tb = Highlight("5*6*78");
                Assert.Equal("I+HIDE", Col(tb, 0, 1));
                Assert.Equal("I", Col(tb, 0, 2));
            });
        }

        [Fact]
        public void Inline_StrikethroughCodeLinkImageAutoLink()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("~~x~~ `y` [t](u) ![i](p) <https://h/p> <a@b.c>");
                var h = tb.SyntaxHighlighter;
                Assert.Equal("S", Col(tb, 0, 2));
                Assert.Equal("CODE", Col(tb, 0, 7));
                Assert.True(HasStyle(tb, 0, 11, h.MdLinkStyle));
                Assert.Equal("IMG", Col(tb, 0, 19));
                Assert.True(HasStyle(tb, 0, 25, h.MdAutoLinkStyle));
                Assert.True(HasStyle(tb, 0, 39, h.MdAutoLinkStyle));
            });
        }

        [Fact]
        public void Inline_EscapedMarkerIsLiteral()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight(@"\*not italic\*");
                Assert.Equal("", Col(tb, 0, 2));
            });
        }

        [Fact]
        public void Inline_CodeSpanDoesNotCrossLines()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("a `x` b\ntext between\nmore `y` c");
                Assert.Equal("CODE", Col(tb, 0, 3));
                Assert.Equal("", Col(tb, 1, 3));
                Assert.Equal("CODE", Col(tb, 2, 6));
            });
        }

        #endregion

        #region blocks

        [Fact]
        public void Blockquote_KeepsEmphasisVisible()
        {
            StaRunner.Run(() =>
            {
                //the container style is not a TextStyle, so the nested bold is the one that renders
                using var tb = Highlight("> quote **bold**");
                Assert.Equal("Q+HIDE", Col(tb, 0, 0));
                Assert.Equal("Q", Col(tb, 0, 2));
                Assert.Equal("Q+B", Col(tb, 0, 10));
            });
        }

        [Fact]
        public void Blockquote_NestedAndCompact()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight(">text\n>>nested\nplain");
                Assert.Equal("Q+HIDE", Col(tb, 0, 0));
                Assert.Equal("Q+HIDE", Col(tb, 1, 0));
                Assert.Equal("Q+HIDE", Col(tb, 1, 1));
                Assert.Equal("", Col(tb, 2, 0));
            });
        }

        [Fact]
        public void ThematicBreak_SpacedDotsAreNotAList()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("***\n- - -\n___\n--\n- item");
                var hr = tb.SyntaxHighlighter.MdHrStyle;
                Assert.True(HasStyle(tb, 0, 0, hr), "*** is a rule");
                Assert.True(HasStyle(tb, 1, 0, hr), "- - - is a rule");
                Assert.True(HasStyle(tb, 2, 0, hr), "___ is a rule");
                Assert.Equal("", Col(tb, 3, 0));
                Assert.Equal("LI", Col(tb, 4, 0));
            });
        }

        [Fact]
        public void List_BulletOnlyAndOrderedParenthesis()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("- a\n1. b\n2) c\ntext - not");
                Assert.Equal("LI", Col(tb, 0, 0));
                Assert.Equal("", Col(tb, 0, 2));
                Assert.Equal("LI", Col(tb, 1, 0));
                Assert.Equal("LI", Col(tb, 2, 0));
                Assert.Equal("", Col(tb, 3, 5));
            });
        }

        [Fact]
        public void List_TaskCheckbox()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("- [x] done\n- [ ] open");
                Assert.Equal("LI", Col(tb, 0, 0));
                Assert.Equal("DONE", Col(tb, 0, 2));
                Assert.Equal("DONE", Col(tb, 0, 4));
                Assert.Equal("LI", Col(tb, 1, 0));
                Assert.Equal("", Col(tb, 1, 2));
            });
        }

        #endregion

        #region rendered markers

        /// <summary>Text of a line without the chars hidden by the markdown highlighter</summary>
        private static string Visible(FastColoredTextBox tb, int line)
        {
            var sb = new StringBuilder();
            for (int x = 0; x < tb[line].Count; x++)
                if (!tb.IsCharHidden(tb[line], x))
                    sb.Append(tb[line][x].C);
            return sb.ToString();
        }

        [Fact]
        public void Heading_MarkersAreHidden()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("## 1. 迭代不依赖提交，能回撤但只能部分“按需”\n#### four ####");
                Assert.Equal("1. 迭代不依赖提交，能回撤但只能部分“按需”", Visible(tb, 0));
                Assert.Equal("four", Visible(tb, 1));
                //the text keeps its heading style
                Assert.Equal("H2", Col(tb, 0, 3));
            });
        }

        [Fact]
        public void Inline_MarkersAreHidden()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("**bold** *it* _it_ ~~gone~~ `code`");
                Assert.Equal("bold it it gone code", Visible(tb, 0));
            });
        }

        [Fact]
        public void LinkAndImage_KeepTheirLabelOnly()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("[text](https://h/p) and ![alt](img.png) and <https://h/p>");
                Assert.Equal("text and alt and https://h/p", Visible(tb, 0));
            });
        }

        [Fact]
        public void Blockquote_MarkersAreHidden()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("> quote\n>>nested");
                Assert.Equal("quote", Visible(tb, 0));
                Assert.Equal("nested", Visible(tb, 1));
            });
        }

        [Fact]
        public void StructuralMarkersStayVisible()
        {
            StaRunner.Run(() =>
            {
                //a bullet/checkbox/rule/fence is the rendered element itself, hiding it would delete it
                using var tb = Highlight("- item\n- [x] done\n---\n```\ncode\n```");
                Assert.Equal("- item", Visible(tb, 0));
                Assert.Equal("- [x] done", Visible(tb, 1));
                Assert.Equal("---", Visible(tb, 2));
                Assert.Equal("```", Visible(tb, 3));
                Assert.Equal("code", Visible(tb, 4));
            });
        }

        [Fact]
        public void FencedContentKeepsItsMarkers()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("```\n## not a heading\n```");
                Assert.Equal("## not a heading", Visible(tb, 1));
            });
        }

        [Fact]
        public void MarkersInsideCodeSpanStayLiteral()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("`a **b** [c](d)`");
                Assert.Equal("a **b** [c](d)", Visible(tb, 0));
            });
        }

        [Fact]
        public void EscapedMarkersAreNotHidden()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight(@"\*not italic\*");
                Assert.Equal(@"\*not italic\*", Visible(tb, 0));
            });
        }

        [Fact]
        public void HeadingWithInlineFormatting_HidesEveryMarker()
        {
            StaRunner.Run(() =>
            {
                //the heading branch used to return before the inline pass, so the bold/code/link
                //markers inside a heading stayed on screen
                using var tb = Highlight("## a **b** `c` [d](e)");
                Assert.Equal("a b c d", Visible(tb, 0));
                Assert.Equal("H2", Col(tb, 0, 3));
            });
        }

        [Fact]
        public void MarkersAreNotPainted()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("## abc");
                var style = tb.SyntaxHighlighter.MdH2Style;
                //drawing the whole line must produce exactly the bitmaps of its visible chars
                using var full = new Bitmap(240, tb.CharHeight + 20);
                using var visible = new Bitmap(full.Width, full.Height);
                using (var g = Graphics.FromImage(full))
                    style.Draw(g, new Point(4, 4), new TextSelectionRange(tb, 0, 0, tb[0].Count, 0));
                using (var g = Graphics.FromImage(visible))
                    style.Draw(g, new Point(4, 4), new TextSelectionRange(tb, 3, 0, tb[0].Count, 0));

                bool paintedMarkers = false;
                for (int y = 0; y < full.Height && !paintedMarkers; y++)
                    for (int x = 0; x < full.Width; x++)
                        if (full.GetPixel(x, y) != visible.GetPixel(x, y))
                        {
                            paintedMarkers = true;
                            break;
                        }

                Assert.False(paintedMarkers, "the hidden # markers must not be painted");
                Assert.Contains("abc", Visible(tb, 0));
            });
        }

        [Fact]
        public void HtmlExport_LeavesMarkersOut()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("## head\n**bold** [t](u)");
                string html = new ExportToHTML().GetHtml(tb.Range);

                Assert.DoesNotContain("##", html);
                Assert.DoesNotContain("**", html);
                Assert.DoesNotContain("](u)", html);
                Assert.Contains("head", html);
                Assert.Contains("bold", html);
                Assert.Contains(">t<", html);
            });
        }

        [Fact]
        public void RtfExport_LeavesMarkersOut()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("## head\n**bold** [t](u)");
                string rtf = new ExportToRTF().GetRtf(tb);

                Assert.DoesNotContain("##", rtf);
                Assert.DoesNotContain("**", rtf);
                Assert.DoesNotContain("](u)", rtf);
                Assert.Contains("head", rtf);
                Assert.Contains("bold", rtf);
            });
        }

        [Fact]
        public void SwitchingAwayFromMarkdownRevealsMarkers()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("## head\n**b**");
                Assert.Equal("head", Visible(tb, 0));
                Assert.Equal("b", Visible(tb, 1));
                tb.Language = Language.CSharp;
                Assert.Equal("## head", Visible(tb, 0));
                Assert.Equal("**b**", Visible(tb, 1));
            });
        }

        [Fact]
        public void HiddenMarkersTakeNoLayoutWidth()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("**ab**");
                Assert.True(tb.IsCharHidden(tb[0], 0));
                Assert.False(tb.IsCharHidden(tb[0], 2));
                var range = new TextSelectionRange(tb, 0, 0, 6, 0);
                Assert.Equal(tb.GetCharWidth('a') + tb.GetCharWidth('b'), Style.GetSizeOfRange(range).Width);
            });
        }

        #endregion

        #region auto indent

        [Fact]
        public void AutoIndent_AfterAListItemIndentsTheNextLine()
        {
            StaRunner.Run(() =>
            {
                using var list = new FastColoredTextBox { Language = Language.Markdown, AutoIndent = true };
                list.Text = "- item";
                PressEnter(list, 0);
                Assert.Equal(list.TabLength, list[1].StartSpacesCount);

                //a blockquote is not indented, its continuation carries no leading spaces
                using var quote = new FastColoredTextBox { Language = Language.Markdown, AutoIndent = true };
                quote.Text = "> quote";
                PressEnter(quote, 0);
                Assert.Equal(0, quote[1].StartSpacesCount);
            });
        }

        private static void PressEnter(FastColoredTextBox tb, int line)
        {
            tb.Selection.SetStartAndEnd(new Place(tb[line].Count, line));
            tb.InsertText("\n");
            tb.DoAutoIndentIfNeed();
        }

        #endregion

        #region styling hazards

        [Fact]
        public void AddingTheSameStyleTwiceKeepsLaterStylesVisible()
        {
            StaRunner.Run(() =>
            {
                //a null slot in Styles used to stop the renderer, hiding every style after it
                using var tb = Highlight("plain");
                var c = tb[0][0];
                c.AddStyle(tb.SyntaxHighlighter.MdBoldStyle);
                tb[0][0] = c;
                var c2 = tb[0][0];
                c2.AddStyle(tb.SyntaxHighlighter.MdBoldStyle);
                tb[0][0] = c2;
                Assert.Equal(0, tb[0][0].LastStyleIndex);
            });
        }

        [Fact]
        public void SwitchingAwayFromMarkdownDoesNotLeakBrushes()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("```\ncode\n```");
                var before = tb.SyntaxHighlighter.MdCodeBlockStyle;
                Assert.NotNull(before);

                tb.Language = Language.CSharp;
                tb.Language = Language.Markdown;
                Assert.NotNull(tb.SyntaxHighlighter.MdCodeBlockStyle);
                Assert.NotSame(before, tb.SyntaxHighlighter.MdCodeBlockStyle);
            });
        }

        /// <summary>
        /// Rebuilding the markdown styles disposes the brushes the old ones own. A char that still
        /// references an old style draws with a freed brush and DrawString throws
        /// ArgumentException, so no char may keep a style from a previous generation.
        /// </summary>
        [Fact]
        public void RebuiltStyles_LeaveNoStaleStyleOnChars()
        {
            StaRunner.Run(() =>
            {
                using var tb = Highlight("# head\n## two\n`x` **b** *i*\n> q\n- [x] t\n---\n```cs\nvar s = 1;\n```");
                var owned = OwnedMarkdownStyles(tb);
                Assert.All(owned, s => Assert.NotNull(s));

                tb.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);

                AssertNoStaleStyles(tb, owned);

                //the same hazard when the language is re-assigned
                using var other = Highlight("# head\n> q\n- i");
                var owned2 = OwnedMarkdownStyles(other);
                other.Language = Language.Markdown;
                AssertNoStaleStyles(other, owned2);
            });
        }

        /// <summary>Markdown styles the highlighter creates (and therefore disposes) itself</summary>
        private static Style[] OwnedMarkdownStyles(FastColoredTextBox tb)
        {
            var h = tb.SyntaxHighlighter;
            return new Style[]
            {
                h.MdH1Style, h.MdH2Style, h.MdH3Style, h.MdHeadingStyle,
                h.MdInlineCodeStyle, h.MdCodeBlockStyle, h.MdBlockquoteStyle, h.MdTaskListDoneStyle
            };
        }

        private static void AssertNoStaleStyles(FastColoredTextBox tb, IEnumerable<Style> stale)
        {
            var staleSet = new HashSet<Style>(stale);
            for (int y = 0; y < tb.LinesCount; y++)
                for (int x = 0; x < tb[y].Count; x++)
                {
                    var c = tb[y][x];
                    if (c.Styles == null)
                        continue;
                    for (int i = 0; i <= c.LastStyleIndex; i++)
                        Assert.False(staleSet.Contains(c.Styles[i]), $"stale style on {y}:{x}");
                }
        }

        #endregion
    }
}
