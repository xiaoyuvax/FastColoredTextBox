using FastColoredTextBoxNS.Text;

namespace FastColoredTextBoxNS.Types
{
    /// <summary>
    /// Rectangular bounds of a selection (line/char coordinates), used by column selection mode.
    /// </summary>
    public struct RangeRect
    {
        public RangeRect(int iStartLine, int iStartChar, int iEndLine, int iEndChar)
        {
            this.iStartLine = iStartLine;
            this.iStartChar = iStartChar;
            this.iEndLine = iEndLine;
            this.iEndChar = iEndChar;
        }

        public int iStartLine;
        public int iStartChar;
        public int iEndLine;
        public int iEndChar;
    }
}
