using System;
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
                Assert.True(tb.SyntaxHighlighter.MdCodeBlockStyle is MarkdownCodeBlockStyle,
                    "code block style must be created for Language.Markdown");
                var fenceStyles = tb.GetStylesOfChar(new Place(1, 3)); // 'n' of "int x;"
                Assert.Contains(fenceStyles, s => s is MarkdownCodeBlockStyle);

                // heading style applied on the heading line ('T' of "# Title")
                var headingStyles = tb.GetStylesOfChar(new Place(2, 0));
                Assert.Contains(headingStyles,
                    s => s is TextStyle ts && ts.FontStyle.HasFlag(FontStyle.Bold));
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
    }
}
