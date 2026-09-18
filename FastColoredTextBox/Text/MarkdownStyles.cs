using FastColoredTextBoxNS.Types;

namespace FastColoredTextBoxNS.Text
{
    /// <summary>
    /// Custom style for Markdown code blocks with border and background.
    /// </summary>
    public class MarkdownCodeBlockStyle : Style
    {
        public Brush ForeBrush { get; set; }
        public Brush BackgroundBrush { get; set; }
        public Pen BorderPen { get; set; }
        private readonly StringFormat stringFormat = new(StringFormatFlags.MeasureTrailingSpaces);

        public MarkdownCodeBlockStyle(Brush foreBrush, Brush backgroundBrush, Color borderColor)
        {
            ForeBrush = foreBrush;
            BackgroundBrush = backgroundBrush;
            BorderPen = new Pen(borderColor, 1f);
        }

        public override void Draw(Graphics gr, Point position, TextSelectionRange range)
        {
            var tb = range.tb;

            int width = 0;
            for (int i = range.Start.iChar; i < range.End.iChar; i++)
                width += tb.GetCharWidth(tb[range.Start.iLine][i].C);
            width = Math.Max(width, tb.Width - position.X - 10);

            //background
            if (BackgroundBrush != null)
                gr.FillRectangle(BackgroundBrush, position.X, position.Y, width, tb.CharHeight);

            //text: build the string once and draw it in a single call instead of per-char DrawString
            var line = tb[range.Start.iLine];
            if (range.End.iChar > range.Start.iChar)
            {
                var sb = new System.Text.StringBuilder(range.End.iChar - range.Start.iChar);
                for (int i = range.Start.iChar; i < range.End.iChar; i++)
                    sb.Append(line[i].C);

                var f = tb.Font;
                ForeBrush ??= new SolidBrush(tb.ForeColor);
                gr.DrawString(sb.ToString(), f, ForeBrush, position.X, position.Y, stringFormat);
            }

            //draw left border line
            int borderGap = 2;
            gr.DrawLine(BorderPen, borderGap, position.Y, borderGap, position.Y + tb.CharHeight);
        }

        public override string GetCSS()
        {
            string result = "";
            if (BackgroundBrush is SolidBrush bg)
                result += "background-color:" + ExportToHTML.GetColorAsString(bg.Color) + ";";
            if (ForeBrush is SolidBrush fg)
                result += "color:" + ExportToHTML.GetColorAsString(fg.Color) + ";";
            if (BorderPen != null)
                result += "border-left:2px solid " + ExportToHTML.GetColorAsString(BorderPen.Color) + ";";
            return result;
        }
    }

    /// <summary>
    /// Custom style for Markdown headings with different font sizes.
    /// Draws text with a scaled font, accepting overflow beyond the fixed grid.
    /// </summary>
    public class MarkdownHeadingStyle : Style
    {
        public Brush ForeBrush { get; set; }
        public Brush BackgroundBrush { get; set; }
        public FontStyle FontStyle { get; set; }
        public float FontSizeMultiplier { get; set; }
        private readonly StringFormat stringFormat = new(StringFormatFlags.MeasureTrailingSpaces);

        public MarkdownHeadingStyle(Brush foreBrush, Brush backgroundBrush, FontStyle fontStyle, float fontSizeMultiplier)
        {
            ForeBrush = foreBrush;
            BackgroundBrush = backgroundBrush;
            FontStyle = fontStyle;
            FontSizeMultiplier = fontSizeMultiplier;
        }

        public override void Draw(Graphics gr, Point position, TextSelectionRange range)
        {
            var tb = range.tb;
            float fontSize = tb.Font.Size * FontSizeMultiplier;

            int width = 0;
            for (int i = range.Start.iChar; i < range.End.iChar; i++)
                width += tb.GetCharWidth(tb[range.Start.iLine][i].C);

            //always cover the default style's text first
            using var bgBrush = new SolidBrush(tb.BackColor);
            gr.FillRectangle(bgBrush, position.X, position.Y, width, tb.CharHeight);

            //then draw our background if specified
            if (BackgroundBrush != null)
                gr.FillRectangle(BackgroundBrush, position.X, position.Y, width, tb.CharHeight);

            //draw text with scaled font
            using var f = new Font(tb.Font.FontFamily, fontSize, FontStyle, tb.Font.Unit);
            ForeBrush ??= new SolidBrush(tb.ForeColor);

            float y = position.Y + (tb.CharHeight - fontSize) / 2;
            float x = position.X;

            var line = tb[range.Start.iLine];
            if (range.End.iChar > range.Start.iChar)
            {
                var sb = new System.Text.StringBuilder(range.End.iChar - range.Start.iChar);
                for (int i = range.Start.iChar; i < range.End.iChar; i++)
                    sb.Append(line[i].C);
                gr.DrawString(sb.ToString(), f, ForeBrush, x, y, stringFormat);
            }
        }

        public override string GetCSS()
        {
            string result = "";
            if (BackgroundBrush is SolidBrush bg)
                result += "background-color:" + ExportToHTML.GetColorAsString(bg.Color) + ";";
            if (ForeBrush is SolidBrush fg)
                result += "color:" + ExportToHTML.GetColorAsString(fg.Color) + ";";
            if ((FontStyle & FontStyle.Bold) != 0) result += "font-weight:bold;";
            if ((FontStyle & FontStyle.Italic) != 0) result += "font-style:oblique;";
            return result;
        }
    }
}
