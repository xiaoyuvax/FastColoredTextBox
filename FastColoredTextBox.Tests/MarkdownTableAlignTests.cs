using System;
using System.Collections.Generic;
using System.Drawing;
using FastColoredTextBoxNS.Text;
using FastColoredTextBoxNS.Types;
using Xunit;

namespace FastColoredTextBoxNS.Tests
{
    /// <summary>
    /// Tests for SyntaxHighlighter.AlignMarkdownTables: it rewrites a table in the buffer, so the
    /// assertions are about the resulting source text, not about styles.
    /// </summary>
    public class MarkdownTableAlignTests
    {
        private static FastColoredTextBox Markdown(string doc)
        {
            var tb = new FastColoredTextBox { Language = Language.Markdown };
            tb.Text = doc;
            return tb;
        }

        private static int Align(FastColoredTextBox tb) =>
            tb.SyntaxHighlighter.AlignMarkdownTables(tb.Range);

        [Fact]
        public void RaggedTable_IsAligned()
        {
            StaRunner.Run(() =>
            {
                using var tb = Markdown("| a | b |\n| --- | --- |\n| ccc | d |");

                Assert.Equal(1, Align(tb));

                Assert.Equal("| a   | b   |", tb[0].Text);
                Assert.Equal("| --- | --- |", tb[1].Text);
                Assert.Equal("| ccc | d   |", tb[2].Text);
            });
        }

        [Fact]
        public void AlignmentMarkers_PlaceTheContent()
        {
            StaRunner.Run(() =>
            {
                using var tb = Markdown("| l | c | r |\n| :-- | :-: | --: |\n| x | x | x |");

                Assert.Equal(1, Align(tb));

                Assert.Equal("| l   |  c  |   r |", tb[0].Text);
                Assert.Equal("| :-- | :-: | --: |", tb[1].Text);
                Assert.Equal("| x   |  x  |   x |", tb[2].Text);
            });
        }

        [Fact]
        public void CjkCell_TakesTwoColumns()
        {
            StaRunner.Run(() =>
            {
                using var tb = Markdown("| 名称 | value |\n| --- | --- |\n| 长 | x |");

                Assert.Equal(1, Align(tb));

                //"名称" is four columns wide, so the first column is padded to four
                Assert.Equal("| 名称 | value |", tb[0].Text);
                Assert.Equal("| ---- | ----- |", tb[1].Text);
                Assert.Equal("| 长   | x     |", tb[2].Text);
            });
        }

        [Fact]
        public void MissingTrailingPipe_IsAdded()
        {
            StaRunner.Run(() =>
            {
                using var tb = Markdown("a | b\n--- | ---\nccc | d");

                Assert.Equal(1, Align(tb));

                Assert.Equal("| a   | b   |", tb[0].Text);
                Assert.Equal("| ccc | d   |", tb[2].Text);
            });
        }

        [Fact]
        public void EscapedAndCodeSpanPipes_StayInTheirCell()
        {
            StaRunner.Run(() =>
            {
                using var tb = Markdown("| a \\| b | `c|d` |\n| --- | --- |\n| e | f |");

                Assert.Equal(1, Align(tb));

                Assert.Equal("| a \\| b | `c|d` |", tb[0].Text);
                //the code span renders as "c|d": its backticks are hidden, so they take no column
                Assert.Equal("| e     | f   |", tb[2].Text);
            });
        }

        [Fact]
        public void ExtraCells_WidenTheTableInsteadOfBeingDropped()
        {
            StaRunner.Run(() =>
            {
                using var tb = Markdown("| a | b |\n| --- | --- |\n| 1 | 2 | 3 |");

                Assert.Equal(1, Align(tb));

                Assert.Equal("| a   | b   |     |", tb[0].Text);
                Assert.Equal("| 1   | 2   | 3   |", tb[2].Text);
            });
        }

        [Fact]
        public void SetextHeading_IsNotATable()
        {
            StaRunner.Run(() =>
            {
                using var tb = Markdown("a\n---\nb | c");

                Assert.Equal(0, Align(tb));

                Assert.Equal("a", tb[0].Text);
                Assert.Equal("---", tb[1].Text);
            });
        }

        [Fact]
        public void RunningTwice_ChangesNothing()
        {
            StaRunner.Run(() =>
            {
                using var tb = Markdown("| a | b |\n| :-: | --: |\n| ccc | d |");
                Assert.Equal(1, Align(tb));
                string once = string.Join("\n", tb.Lines);

                Assert.Equal(0, Align(tb));
                Assert.Equal(once, string.Join("\n", tb.Lines));
            });
        }

        [Fact]
        public void Table_IsOneUndoStep()
        {
            StaRunner.Run(() =>
            {
                using var tb = Markdown("| a | b |\n| --- | --- |\n| ccc | d |");
                string before = string.Join("\n", tb.Lines);

                Align(tb);
                Assert.NotEqual(before, string.Join("\n", tb.Lines));

                tb.Undo();
                Assert.Equal(before, string.Join("\n", tb.Lines));
            });
        }

        [Fact]
        public void Selection_IsKept()
        {
            StaRunner.Run(() =>
            {
                using var tb = Markdown("intro\n| a | b |\n| --- | --- |\n| ccc | d |\nend");
                tb.Selection.Start = new Place(0, 4);
                tb.Selection.End = new Place(0, 4);

                Align(tb);

                Assert.Equal(4, tb.Selection.Start.iLine);
                Assert.Equal(0, tb.Selection.Start.iChar);
                Assert.Equal("end", tb[4].Text);
            });
        }

        [Fact]
        public void OnlyTablesInTheRange_AreFormatted()
        {
            StaRunner.Run(() =>
            {
                using var tb = Markdown(
                    "| a | b |\n| --- | --- |\n| ccc | d |\n\n| x | y |\n| --- | --- |\n| zzz | w |");
                var range = new TextSelectionRange(tb, 0, 4, tb[6].Count, 6);

                Assert.Equal(1, tb.SyntaxHighlighter.AlignMarkdownTables(range));

                Assert.Equal("| a | b |", tb[0].Text);
                Assert.Equal("| x   | y   |", tb[4].Text);
            });
        }

        [Fact]
        public void HiddenMarkers_DoNotCountTowardsColumnWidth()
        {
            StaRunner.Run(() =>
            {
                //"**a**" renders as "a" and "`x`" as "x": the column is padded to the rendered
                //width, otherwise the pipes would drift once the markers disappear
                using var tb = Markdown("| **a** | `x` |\n| --- | --- |\n| bbbb | yyy |");

                Assert.Equal(1, Align(tb));

                Assert.Equal("| **a**    | `x`   |", tb[0].Text);
                Assert.Equal("| ---- | --- |", tb[1].Text);
                Assert.Equal("| bbbb | yyy |", tb[2].Text);
                Assert.Equal(0, Align(tb));
            });
        }

        /// <summary>Indexes of the pipes the renderer still paints on a line</summary>
        private static List<int> VisiblePipes(FastColoredTextBox tb, int iLine)
        {
            var pipes = new List<int>();
            for (int i = 0; i < tb[iLine].Count; i++)
                if (tb[iLine][i].C == '|' && !tb.IsCharHidden(tb[iLine], i))
                    pipes.Add(i);
            return pipes;
        }

        [Fact]
        public void TableSeparators_AreAllHidden()
        {
            StaRunner.Run(() =>
            {
                using var tb = Markdown("| a | b |\n| --- | --- |\n| ccc | d |");

                tb.SyntaxHighlighter.MarkdownBodyHighlight(tb.Range);

                for (int iLine = 0; iLine < tb.LinesCount; iLine++)
                    Assert.Empty(VisiblePipes(tb, iLine));
            });
        }

        [Fact]
        public void EscapedAndCodeSpanPipes_StayVisible()
        {
            StaRunner.Run(() =>
            {
                using var tb = Markdown("| a \\| b | `c|d` |\n| --- | --- |\n| e | f |");

                tb.SyntaxHighlighter.MarkdownBodyHighlight(tb.Range);

                //the escaped pipe and the one inside the code span; every cell separator is hidden
                Assert.Equal(2, VisiblePipes(tb, 0).Count);
            });
        }

        [Fact]
        public void TableInsideFence_KeepsItsPipes()
        {
            StaRunner.Run(() =>
            {
                using var tb = Markdown("```\n| a | b |\n| --- | --- |\n```");

                tb.SyntaxHighlighter.MarkdownBodyHighlight(tb.Range);

                //a table inside a fence is code: its pipes are not table syntax
                Assert.Equal(3, VisiblePipes(tb, 1).Count);
            });
        }

        /// <summary>
        /// X offset of every visible '|' of a line, accumulated with the control's own char widths
        /// (a hidden markdown marker takes no room, so it does not shift the pipes)
        /// </summary>
        private static int[] PipeOffsets(FastColoredTextBox tb, int iLine)
        {
            var offsets = new List<int>();
            int x = 0;
            for (int i = 0; i < tb[iLine].Count; i++)
            {
                if (tb[iLine][i].C == '|')
                    offsets.Add(x);
                x += tb.GetCharWidth(tb[iLine], i);
            }
            return offsets.ToArray();
        }

        [Fact]
        public void ProportionalFont_PipesLineUpInPixels()
        {
            StaRunner.Run(() =>
            {
                //a proportional font has no character grid: padding in columns would leave the pipes
                //of a CJK-heavy column far away from those of a latin one, so the cell widths have to
                //come from the control's own metrics
                using var tb = new FastColoredTextBox
                {
                    Language = Language.Markdown,
                    AllowProportionalFont = true,
                    Font = new Font("Microsoft YaHei UI", 9F),
                };
                tb.Text = "| 名称 | **value** |\n| --- | --- |\n| 长 | `x` |\n| abcdef | 中文内容 |";

                Assert.Equal(1, Align(tb));
                //measure the rendered line: the inline markers are hidden, so they take no room
                tb.SyntaxHighlighter.MarkdownBodyHighlight(tb.Range, alignTables: false);

                int tolerance = 2 * tb.GetCharWidth(' ');
                int[] first = PipeOffsets(tb, 0);
                for (int iLine = 1; iLine < tb.LinesCount; iLine++)
                {
                    int[] pipes = PipeOffsets(tb, iLine);
                    Assert.Equal(first.Length, pipes.Length);
                    for (int c = 0; c < pipes.Length; c++)
                        Assert.InRange(Math.Abs(pipes[c] - first[c]), 0, tolerance);
                }
            });
        }
    }
}
