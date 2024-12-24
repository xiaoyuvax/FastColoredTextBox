using FastColoredTextBoxNS.Text;

namespace FastColoredTextBoxNS.Types
{
    /// <summary>
    /// Marker style
    /// Draws background color for text
    /// </summary>
    public sealed class MarkerStyle : Style
    {
        public MarkerStyle(Brush backgroundBrush)
        {
            BackgroundBrush = backgroundBrush;
            IsExportable = true;
        }

        public Brush BackgroundBrush { get; set; }

        public override void Draw(Graphics gr, Point position, TextSelectionRange range)
        {
            //draw background
            if (BackgroundBrush != null)
            {
                Rectangle rect = new(position.X, position.Y, (range.End.iChar - range.Start.iChar) * range.tb.CharWidth, range.tb.CharHeight);
                if (rect.Width == 0)
                    return;
                gr.FillRectangle(BackgroundBrush, rect);
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

            return result;
        }
    }
}