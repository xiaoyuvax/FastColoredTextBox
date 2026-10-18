using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FastColoredTextBoxNS;
using FastColoredTextBoxNS.Input;
using FastColoredTextBoxNS.Text;
using FastColoredTextBoxNS.Types;
using Xunit;

namespace FastColoredTextBoxNS.Tests
{
    /// <summary>
    /// Control-level regression tests (STA required: FastColoredTextBox is a WinForms control).
    /// </summary>
    public class FastColoredTextBoxTests
    {
        #region Text / Length consistency (#25a)

        [Fact]
        public void Text_Length_CountsNewlineAsOneChar()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox();
                tb.Text = "ab\r\ncd\r\nef";

                Assert.Equal(8, tb.Text.Length);
                Assert.Equal(8, tb.TextLength);
                Assert.Equal(3, tb.LinesCount);
            });
        }

        [Fact]
        public void Text_EOLIsNormalizedToLF()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox();
                tb.Text = "ab\r\ncd";

                Assert.Equal("ab\ncd", tb.Text);
            });
        }

        [Fact]
        public void GetText_OfSelection_KeepsNewlineCountConsistentWithLength()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox();
                tb.Text = "ab\r\ncd\r\nef";
                tb.SelectAll();

                var text = tb.Selection.Text;
                Assert.Equal(tb.Text.Length, text.Length);
            });
        }
        #endregion

        #region Undo / Redo (#25d)

        [Fact]
        public void UndoRedo_RoundTrip()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox();
                tb.Text = "one\n";
                tb.ClearUndo();
                // assigning Text selects everything; deselect so InsertText doesn't replace it
                tb.Selection.Start = new Place(0, 0);
                tb.Selection.End = new Place(0, 0);
                tb.InsertText("zero\n");

                Assert.Equal("zero\none\n", tb.Text);

                tb.Undo();
                Assert.Equal("one\n", tb.Text);

                tb.Redo();
                Assert.Equal("zero\none\n", tb.Text);
            });
        }

        /// <summary>
        /// The undo history is capped at CommandManager.MaxHistoryLength (200) entries;
        /// with 1000 inserts the chain is deep. Regression for #25d: Undo/Redo must be
        /// iterative (no StackOverflow) and consistent for the capped part of the chain.
        /// </summary>
        [Fact]
        public void UndoRedo_ManyInserts_LongChainDoesNotOverflow()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox();
                tb.Text = "";
                tb.ClearUndo();

                const int inserts = 1000; // > MaxHistoryLength (200)
                for (int i = 0; i < inserts; i++)
                    tb.InsertText(i + "\n");

                var full = tb.Text;

                // undo everything the history retained, then keep calling: must not throw
                for (int i = 0; i < CommandManager.MaxHistoryLength; i++)
                    tb.Undo();
                var undone = tb.Text;
                Assert.NotEqual(full, undone);

                // redo restores exactly the undone part
                for (int i = 0; i < CommandManager.MaxHistoryLength; i++)
                    tb.Redo();
                Assert.Equal(full, tb.Text);

                // further redos are no-ops
                for (int i = 0; i < 10; i++)
                    tb.Redo();
                Assert.Equal(full, tb.Text);
            });
        }
        #endregion

        #region Markdown integration (smoke)

        [Fact]
        public void MarkdownLanguage_HighlightingProducesStyles()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox();
                tb.Language = Language.Markdown;
                tb.Text = "# Title\nbody\n```cs\nint x;\n```\nend";
                tb.SyntaxHighlighter.MarkdownSyntaxHighlight(tb.Range);

                // fenced code block style is created and applied inside the fence
                Assert.True(tb.SyntaxHighlighter.MdCodeBlockStyle is MarkdownBlockStyle,
                    "code block style must be created for Language.Markdown");
                var fenceStyles = tb.GetStylesOfChar(new Place(1, 3)); // 'n' of "int x;"
                Assert.Contains(fenceStyles, s => s is MarkdownBlockStyle);

                // heading style applied on the heading line ('T' of "# Title"), not on the '#'
                var headingStyles = tb.GetStylesOfChar(new Place(3, 0));
                Assert.Contains(headingStyles,
                    s => s is TextStyle ts && ts.FontStyle.HasFlag(FontStyle.Bold));
                Assert.Empty(tb.GetStylesOfChar(new Place(0, 0)));
            });
        }

        [Fact]
        public void LanguageSwitch_Repeatedly_DoesNotLeakOrThrow()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox();
                tb.Text = "# md\n```cs\nx\n```\n";
                for (int i = 0; i < 20; i++)
                    tb.Language = i % 2 == 0 ? Language.Markdown : Language.CSharp;

                Assert.Equal(Language.CSharp, tb.Language);
            });
        }
        #endregion

        #region TextStyle classic draw advance

        /// <summary>
        /// Regression: the classic (non-IME) branch of TextStyle.Draw advanced x by `dx`,
        /// which was only set in the IME branch and stayed 0 -> every char of a multi-char
        /// run was drawn at the same x (a folded line collapsed at the line start).
        /// </summary>
        [Fact]
        public void TextStyle_ClassicDraw_AdvancesPerChar()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox { Width = 400, Height = 100 };
                _ = tb.Handle;
                tb.ImeMode = System.Windows.Forms.ImeMode.NoControl; // force the classic (non-IME) draw branch
                tb.Font = new Font("Consolas", 9f);
                tb.Text = "ABCDEF";

                using var bmp = new Bitmap(400, 100);
                using (var gr = Graphics.FromImage(bmp))
                {
                    gr.Clear(Color.White);
                    var style = new TextStyle(Brushes.Black, null, FontStyle.Regular);
                    style.Draw(gr, new Point(10, 10), new TextSelectionRange(tb, 0, 0, 6, 0));
                }

                int minX = int.MaxValue, maxX = -1;
                for (int y = 0; y < bmp.Height; y++)
                    for (int x = 0; x < bmp.Width; x++)
                        if (bmp.GetPixel(x, y).R < 128)
                        {
                            if (x < minX) minX = x;
                            if (x > maxX) maxX = x;
                        }

                Assert.True(maxX - minX > tb.CharWidth * 3,
                    $"classic draw stacked chars at one x (span={maxX - minX}, charWidth={tb.CharWidth})");
            });
        }
        #endregion

        #region Word wrap on resize

        /// <summary>
        /// Regression: word wrap cut-offs depend on client width, but a size change did
        /// not recalculate them, so resizing left stale wrapping (lines overlapped in
        /// host layouts). OnSizeChanged must recompute the wrap.
        /// </summary>
        [Fact]
        public void WordWrap_IsRecalculatedWhenWidthChanges()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox { WordWrap = true, ShowLineNumbers = false };
                _ = tb.Handle; // force creation so Recalc actually computes the wrap

                tb.Text = new string('x', 400);
                tb.Width = 100;   // narrow: wraps into many lines
                int narrow = tb.TextHeight;
                tb.Width = 1000;  // wide: fewer wrapped lines
                int wide = tb.TextHeight;

                Assert.True(narrow > wide,
                    $"word wrap not recalculated on width change (narrow={narrow}, wide={wide})");
            });
        }

        /// <summary>
        /// Regression: with WordWrapAutoIndent the continuation lines are rendered shifted right by
        /// LineInfo.wordWrapIndent chars (line.StartSpacesCount + WordWrapIndent), but that indent was NOT
        /// deducted from the wrap width - so every continuation line stuck out of the right edge by that indent
        /// (visible as soon as a host indents a block, e.g. VeniceMan's prompt block with 4 leading spaces).
        /// First segment keeps the full width; every later segment must fit available - indent.
        /// </summary>
        [Fact]
        public void WordWrap_ContinuationIndent_IsDeductedFromWrapWidth()
        {
            StaRunner.Run(() =>
            {
                const int indentChars = 4;
                using var tb = new FastColoredTextBox
                {
                    WordWrap = true,
                    WordWrapAutoIndent = true,
                    ShowScrollBars = false,
                    ShowLineNumbers = false,
                    Font = new Font("Consolas", 10f),
                    Width = 220,
                    Height = 200,
                };
                _ = tb.Handle;   // CharWidth needs a created handle
                string words = string.Join(" ", Enumerable.Repeat("film-style photorealistic lighting detail", 12));
                tb.Text = new string(' ', indentChars) + words;

                var line = tb[0];
                int avail = tb.ClientSize.Width - tb.LeftIndent - tb.Paddings.Left - tb.Paddings.Right;
                int charWidth = tb.GetCharWidth('a');
                var cutOffs = new List<int>();
                tb.CalcCutOffsByMaxWidth(cutOffs, avail, false, line, indentChars);

                Assert.NotEmpty(cutOffs);
                int start = 0;
                for (int seg = 0; seg <= cutOffs.Count; seg++)
                {
                    int end = seg < cutOffs.Count ? cutOffs[seg] : line.Count;
                    int width = 0;
                    for (int i = start; i < end; i++) width += tb.GetCharWidth(line[i].C);
                    int limit = seg == 0 ? avail : avail - indentChars * charWidth;
                    Assert.True(width <= limit,
                        $"segment {seg} width {width} exceeds limit {limit} (wrap width must deduct the continuation indent)");
                    start = end;
                }
            });
        }
        #endregion
    }
}
