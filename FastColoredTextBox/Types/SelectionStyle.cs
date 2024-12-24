using System.Drawing.Drawing2D;

namespace FastColoredTextBoxNS.Types
{
    /// <summary>
    /// Renderer for selected area
    /// </summary>
    public sealed class SelectionStyle : Style
    {
        public SelectionStyle(Brush backgroundBrush, Brush foregroundBrush = null)
        {
            BackgroundBrush = backgroundBrush;
            ForegroundBrush = foregroundBrush;
        }

        public Brush BackgroundBrush { get; set; }
        public Brush ForegroundBrush { get; private set; }

        public override bool IsExportable
        {
            get => false;
            set { }
        }

        public override void Draw(Graphics gr, Point position, TextSelectionRange range)
        {
            //draw background
            if (BackgroundBrush != null)
            {
                var rangeSize = GetSizeOfRange(range);
                if (rangeSize.Width == 0) return;
                gr.SmoothingMode = SmoothingMode.None;
                var rect = new Rectangle(position.X, position.Y, rangeSize.Width, rangeSize.Height);
                
                gr.FillRectangle(BackgroundBrush, rect);
                //
                if (ForegroundBrush != null)
                {
                    //draw text
                    gr.SmoothingMode = SmoothingMode.AntiAlias;

                    var r = new TextSelectionRange(range.tb, range.Start.iChar, range.Start.iLine,
                                      Math.Min(range.tb[range.End.iLine].Count, range.End.iChar), range.End.iLine);
                    using var style = new TextStyle(ForegroundBrush, null, FontStyle.Regular);
                    style.Draw(gr, new Point(position.X, position.Y - 1), r);
                }
            }
        }
    }
}