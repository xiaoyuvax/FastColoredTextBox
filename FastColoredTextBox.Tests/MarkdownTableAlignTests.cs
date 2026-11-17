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
                Assert.Equal("| e     | f     |", tb[2].Text);
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
    }
}
