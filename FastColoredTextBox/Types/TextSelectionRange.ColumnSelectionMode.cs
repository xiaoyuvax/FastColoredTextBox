using System.Text;
using FastColoredTextBoxNS.Text;

namespace FastColoredTextBoxNS.Types
{
    /// <summary>
    /// Column-selection-mode part of <see cref="TextSelectionRange"/> (extracted from TextSelectionRange.cs).
    /// </summary>
    public partial class TextSelectionRange
    {


        private TextSelectionRange GetIntersectionWith_ColumnSelectionMode(TextSelectionRange range)
        {
            if (range.Start.iLine != range.End.iLine)
                return new TextSelectionRange(tb, Start, Start);
            var rect = Bounds;
            if (range.Start.iLine < rect.iStartLine || range.Start.iLine > rect.iEndLine)
                return new TextSelectionRange(tb, Start, Start);

            return new TextSelectionRange(tb, rect.iStartChar, range.Start.iLine, rect.iEndChar, range.Start.iLine).GetIntersectionWith(range);
        }

        private bool GoRightThroughFolded_ColumnSelectionMode()
        {
            var boundes = Bounds;
            var endOfLines = true;
            for (int iLine = boundes.iStartLine; iLine <= boundes.iEndLine; iLine++)
                if (boundes.iEndChar < tb[iLine].Count)
                {
                    endOfLines = false;
                    break;
                }

            if (endOfLines)
                return false;

            var start = Start;
            var end = End;
            start.Offset(1, 0);
            end.Offset(1, 0);
            BeginUpdate();
            Start = start;
            End = end;
            EndUpdate();

            return true;
        }

        private IEnumerable<Place> GetEnumerator_ColumnSelectionMode()
        {
            var bounds = Bounds;
            if (bounds.iStartLine < 0) yield break;
            //
            for (int y = bounds.iStartLine; y <= bounds.iEndLine; y++)
            {
                for (int x = bounds.iStartChar; x < bounds.iEndChar; x++)
                {
                    if (x < tb[y].Count)
                        yield return new Place(x, y);
                }
            }
        }

        private string Text_ColumnSelectionMode
        {
            get
            {
                StringBuilder sb = new();
                var bounds = Bounds;
                if (bounds.iStartLine < 0) return "";
                //
                for (int y = bounds.iStartLine; y <= bounds.iEndLine; y++)
                {
                    for (int x = bounds.iStartChar; x < bounds.iEndChar; x++)
                    {
                        if (x < tb[y].Count)
                            sb.Append(tb[y][x].C);
                    }
                    if (bounds.iEndLine != bounds.iStartLine && y != bounds.iEndLine)
                        sb.Append('\n'); // one logical char per line break, matching Text and Length
                }

                return sb.ToString();
            }
        }

        private int Length_ColumnSelectionMode(bool withNewLines)
        {
            var bounds = Bounds;
            if (bounds.iStartLine < 0) return 0;
            int cnt = 0;
            //
            for (int y = bounds.iStartLine; y <= bounds.iEndLine; y++)
            {
                for (int x = bounds.iStartChar; x < bounds.iEndChar; x++)
                {
                    if (x < tb[y].Count)
                        cnt++;
                }
                if (withNewLines && bounds.iEndLine != bounds.iStartLine && y != bounds.iEndLine)
                    cnt++; // one logical char per line break, matching Text
            }

            return cnt;
        }

        internal void GoDown_ColumnSelectionMode()
        {
            var iLine = tb.FindNextVisibleLine(End.iLine);
            End = new Place(End.iChar, iLine);
        }

        internal void GoUp_ColumnSelectionMode()
        {
            var iLine = tb.FindPrevVisibleLine(End.iLine);
            End = new Place(End.iChar, iLine);
        }

        internal void GoRight_ColumnSelectionMode()
        {
            End = new Place(End.iChar + 1, End.iLine);
        }

        internal void GoLeft_ColumnSelectionMode()
        {
            if (End.iChar > 0)
                End = new Place(End.iChar - 1, End.iLine);
        }

    }
}
