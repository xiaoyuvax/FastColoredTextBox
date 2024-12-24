using FastColoredTextBoxNS.Feature;
using FastColoredTextBoxNS.Text;
using System.Drawing.Drawing2D;
using static System.Windows.Forms.AxHost;

namespace FastColoredTextBoxNS.Types
{

    /// <summary>
    /// Style of chars
    /// </summary>
    /// <remarks>This is base class for all text and design renderers</remarks>
    public abstract class Style : IDisposable
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Style() => IsExportable = true;

        /// <summary>
        /// Occurs when user click on StyleVisualMarker joined to this style
        /// </summary>
        public event EventHandler<VisualMarkerEventArgs> VisualMarkerClick;

        /// <summary>
        /// This style is exported to outer formats (HTML for example)
        /// </summary>
        public virtual bool IsExportable { get; set; }


        public static GraphicsPath GetRoundedRectangle(Rectangle rect, int d)
        {
            GraphicsPath gp = new();

            gp.AddArc(rect.X, rect.Y, d, d, 180, 90);
            gp.AddArc(rect.X + rect.Width - d, rect.Y, d, d, 270, 90);
            gp.AddArc(rect.X + rect.Width - d, rect.Y + rect.Height - d, d, d, 0, 90);
            gp.AddArc(rect.X, rect.Y + rect.Height - d, d, d, 90, 90);
            gp.AddLine(rect.X, rect.Y + rect.Height - d, rect.X, rect.Y + d / 2);

            return gp;
        }

        public static Size GetSizeOfRange(TextSelectionRange range)
        {
            //Original implementation: => new((range.End.iChar - range.Start.iChar) * range.tb.CharWidth, range.tb.CharHeight);
            int rangeWidth = 0;
            foreach (char c in range.Text) rangeWidth += range.tb.GetCharWidth(c);

            return new(rangeWidth, range.tb.CharHeight);
        }

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Renders given range of text
        /// </summary>
        /// <param name="gr">Graphics object</param>
        /// <param name="position">Position of the range in absolute control coordinates</param>
        /// <param name="range">Rendering range of text</param>
        public abstract void Draw(Graphics gr, Point position, TextSelectionRange range);

        /// <summary>
        /// Returns CSS for export to HTML
        /// </summary>
        /// <returns></returns>
        public virtual string GetCSS() => "";

        /// <summary>
        /// Returns RTF descriptor for export to RTF
        /// </summary>
        /// <returns></returns>
        public virtual RTFStyleDescriptor GetRTF() => new();

        /// <summary>
        /// Occurs when user click on StyleVisualMarker joined to this style
        /// </summary>
        public virtual void OnVisualMarkerClick(FastColoredTextBox tb, VisualMarkerEventArgs args) => VisualMarkerClick?.Invoke(tb, args);

        /// <summary>
        /// Shows VisualMarker
        /// Call this method in Draw method, when you need to show VisualMarker for your style
        /// </summary>
        protected virtual void AddVisualMarker(FastColoredTextBox tb, StyleVisualMarker marker) => tb.AddVisualMarker(marker);
    }
}