using FastColoredTextBoxNS.Types;

namespace FastColoredTextBoxNS.Text
{
    /// <summary>
    /// Paints a block-level Markdown region: a tinted background that reaches the right edge of
    /// the text area, plus a vertical bar at the start of each line of the block.
    /// Used for fenced code blocks and blockquotes.
    /// </summary>
    /// <remarks>
    /// Deliberately a plain <see cref="Style"/> and deliberately draws no text. The control renders
    /// only the first <see cref="TextStyle"/> of a char (see
    /// <see cref="FastColoredTextBox.AllowSeveralTextStyleDrawing"/>), so a container that carried its
    /// own text style would hide every bold/italic/inner-language style nested inside it. Keeping the
    /// container non-text lets the nested styles paint the glyphs on top of the background.
    /// </remarks>
    public class MarkdownBlockStyle : Style
    {
        public Brush BackgroundBrush { get; set; }
        public Brush BarBrush { get; set; }
        public int BarWidth { get; set; }

        public MarkdownBlockStyle(Brush backgroundBrush, Brush barBrush, int barWidth = 3)
        {
            BackgroundBrush = backgroundBrush;
            BarBrush = barBrush;
            BarWidth = barWidth;
        }

        public override void Draw(Graphics gr, Point position, TextSelectionRange range)
        {
            var tb = range.tb;
            int height = tb.CharHeight;
            //TextAreaRect already accounts for the left indent, padding and horizontal scrolling,
            //so the tint keeps its right edge put while the user scrolls sideways
            int right = Math.Max(tb.TextAreaRect.Right, position.X);

            if (BackgroundBrush != null)
                gr.FillRectangle(BackgroundBrush, position.X, position.Y, right - position.X, height);

            //the bar marks the start of a line, not of a word-wrapped continuation segment
            if (BarBrush != null && BarWidth > 0 && range.Start.iChar == 0)
                gr.FillRectangle(BarBrush, position.X, position.Y, BarWidth, height);
        }

        public override string GetCSS()
        {
            string result = "";
            if (BackgroundBrush is SolidBrush bg)
                result += "background-color:" + ExportToHTML.GetColorAsString(bg.Color) + ";";
            if (BarBrush is SolidBrush bar)
                result += "border-left:" + BarWidth + "px solid " + ExportToHTML.GetColorAsString(bar.Color) + ";";
            return result;
        }
    }
}
