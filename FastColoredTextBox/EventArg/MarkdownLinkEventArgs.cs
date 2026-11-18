using FastColoredTextBoxNS.Text;

namespace FastColoredTextBoxNS
{
    /// <summary>
    /// MarkdownLinkClicked event args: the clickable region that was clicked.
    /// </summary>
    public class MarkdownLinkEventArgs : EventArgs
    {
        public MarkdownLinkEventArgs(MarkdownLink link)
        {
            Link = link;
        }

        /// <summary>Line, char range and destination of the clicked region.</summary>
        public MarkdownLink Link { get; }

        /// <summary>Where the click should go: a url or a local path, see <see cref="MarkdownLink.Target"/>.</summary>
        public string Target => Link.Target;
    }
}
