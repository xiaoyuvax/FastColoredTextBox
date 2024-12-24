using FastColoredTextBoxNS.Text;

namespace FastColoredTextBoxNS.Types
{
    /// <summary>
    /// Style for chars rendering
    /// This renderer can draws chars, with defined fore and back colors
    /// </summary>
    public class TextStyle : Style
    {
        //public readonly Font Font;
        public StringFormat stringFormat;

        public TextStyle(Brush foreBrush, Brush backgroundBrush, FontStyle fontStyle)
        {
            ForeBrush = foreBrush;
            BackgroundBrush = backgroundBrush;
            FontStyle = fontStyle;
            stringFormat = new StringFormat(StringFormatFlags.MeasureTrailingSpaces);
        }

        public Brush BackgroundBrush { get; set; }
        public FontStyle FontStyle { get; set; }
        public Brush ForeBrush { get; set; }

        public override void Draw(Graphics gr, Point position, TextSelectionRange range)
        {
            //draw background
            if (BackgroundBrush != null)
                gr.FillRectangle(BackgroundBrush, position.X, position.Y, (range.End.iChar - range.Start.iChar) * range.tb.CharWidth, range.tb.CharHeight);
            //draw chars
            using var f = new Font(range.tb.Font, FontStyle);
            Line line = range.tb[range.Start.iLine];

            float y = position.Y + range.tb.LineInterval / 2;
            float x = position.X - range.tb.CharWidth / 3;

            ForeBrush ??= new SolidBrush(range.tb.ForeColor);

            float dx;
            if (range.tb.ImeAllowed)
            {
                //IME mode
                for (int i = range.Start.iChar; i < range.End.iChar; i++)
                {
                    var c = line[i].c;
                    dx = range.tb.GetCharWidth(c);

                    var gs = gr.Save();
                    float k = dx > range.tb.CharWidth + 1 ? range.tb.CharWidth / dx : 1;
                    gr.TranslateTransform(x, y + (1 - k) * range.tb.CharHeight / 2);
                    gr.ScaleTransform(k, (float)Math.Sqrt(k));
                    gr.DrawString(c.ToString(), f, ForeBrush, 0, 0, stringFormat);
                    gr.Restore(gs);
                    x += dx;
                }
            }
            else
            {
                //classic mode
                for (int i = range.Start.iChar; i < range.End.iChar; i++)
                {
                    var c = line[i].c;
                    dx = range.tb.GetCharWidth(c);
                    //draw char
                    gr.DrawString(c.ToString(), f, ForeBrush, x, y, stringFormat);
                    x += dx;
                }
            }
        }



        public override string GetCSS()
        {
            string result = "";

            if (BackgroundBrush is SolidBrush)
            {
                var s = ExportToHTML.GetColorAsString((BackgroundBrush as SolidBrush).Color);
                if (s != "")
                    result += "background-color:" + s + ";";
            }
            if (ForeBrush is SolidBrush)
            {
                var s = ExportToHTML.GetColorAsString((ForeBrush as SolidBrush).Color);
                if (s != "")
                    result += "color:" + s + ";";
            }
            if ((FontStyle & FontStyle.Bold) != 0)
                result += "font-weight:bold;";
            if ((FontStyle & FontStyle.Italic) != 0)
                result += "font-style:oblique;";
            if ((FontStyle & FontStyle.Strikeout) != 0)
                result += "text-decoration:line-through;";
            if ((FontStyle & FontStyle.Underline) != 0)
                result += "text-decoration:underline;";

            return result;
        }

        public override RTFStyleDescriptor GetRTF()
        {
            var result = new RTFStyleDescriptor();

            if (BackgroundBrush is SolidBrush)
                result.BackColor = (BackgroundBrush as SolidBrush).Color;

            if (ForeBrush is SolidBrush)
                result.ForeColor = (ForeBrush as SolidBrush).Color;

            if ((FontStyle & FontStyle.Bold) != 0)
                result.AdditionalTags += @"\b";
            if ((FontStyle & FontStyle.Italic) != 0)
                result.AdditionalTags += @"\i";
            if ((FontStyle & FontStyle.Strikeout) != 0)
                result.AdditionalTags += @"\strike";
            if ((FontStyle & FontStyle.Underline) != 0)
                result.AdditionalTags += @"\ul";

            return result;
        }
    }
}