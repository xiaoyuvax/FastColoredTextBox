using FastColoredTextBoxNS.Input;
using FastColoredTextBoxNS.Types;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using Timer = System.Windows.Forms.Timer;

namespace FastColoredTextBoxNS
{
    /// <summary>
    /// Popup menu for autocomplete
    /// </summary>
    [Browsable(false)]
    public class AutocompleteMenu : ToolStripDropDown, IDisposable
    {
        private readonly AutocompleteListView listView;
        public ToolStripControlHost host;
        public TextSelectionRange Fragment { get; internal set; }

        /// <summary>
        /// Regex pattern for serach fragment around caret
        /// </summary>
        public string SearchPattern { get; set; }

        /// <summary>
        /// Minimum fragment length for popup
        /// </summary>
        public int MinFragmentLength { get; set; }

        /// <summary>
        /// User selects item
        /// </summary>
        public event EventHandler<SelectingEventArgs> Selecting;

        /// <summary>
        /// It fires after item inserting
        /// </summary>
        public event EventHandler<SelectedEventArgs> Selected;

        /// <summary>
        /// Occurs when popup menu is opening
        /// </summary>
        public new event EventHandler<CancelEventArgs> Opening;

        /// <summary>
        /// Allow TAB for select menu item
        /// </summary>
        public bool AllowTabKey { get => listView.AllowTabKey; set => listView.AllowTabKey = value; }

        /// <summary>
        /// Interval of menu appear (ms)
        /// </summary>
        public int AppearInterval { get => listView.AppearInterval; set => listView.AppearInterval = value; }

        /// <summary>
        /// Sets the max tooltip window size
        /// </summary>
        public Size MaxTooltipSize { get => listView.MaxToolTipSize; set => listView.MaxToolTipSize = value; }

        /// <summary>
        /// Tooltip will perm show and duration will be ignored
        /// </summary>
        public bool AlwaysShowTooltip { get => listView.AlwaysShowTooltip; set => listView.AlwaysShowTooltip = value; }

        /// <summary>
        /// Back color of selected item
        /// </summary>
        [DefaultValue(typeof(Color), "Orange")]
        public Color SelectedColor
        {
            get => listView.SelectedColor;
            set => listView.SelectedColor = value;
        }

        /// <summary>
        /// Border color of hovered item
        /// </summary>
        [DefaultValue(typeof(Color), "Red")]
        public Color HoveredColor
        {
            get => listView.HoveredColor;
            set => listView.HoveredColor = value;
        }

        public AutocompleteMenu(FastColoredTextBox tb)
        {
            // create a new popup and add the list view to it
            AutoClose = false;
            AutoSize = false;
            Margin = Padding.Empty;
            Padding = Padding.Empty;
            BackColor = Color.White;
            listView = new AutocompleteListView(tb);
            host = new ToolStripControlHost(listView)
            {
                Margin = new Padding(2, 2, 2, 2),
                Padding = Padding.Empty,
                AutoSize = false,
                AutoToolTip = false
            };
            CalcSize();
            base.Items.Add(host);
            listView.Parent = this;
            SearchPattern = @"[\w\.]";
            MinFragmentLength = 2;
        }

        public new Font Font
        {
            get => listView.Font;
            set => listView.Font = value;
        }

        new internal void OnOpening(CancelEventArgs args)
        {
            Opening?.Invoke(this, args);
        }

        public new void Close()
        {
            listView.toolTip.Hide(listView);
            base.Close();
        }

        internal void CalcSize()
        {
            host.Size = listView.Size;
            Size = new System.Drawing.Size(listView.Size.Width + 4, listView.Size.Height + 4);
        }

        public virtual void OnSelecting()
        {
            listView.OnSelecting();
        }

        public void SelectNext(int shift)
        {
            listView.SelectNext(shift);
        }

        internal void OnSelecting(SelectingEventArgs args)
        {
            Selecting?.Invoke(this, args);
        }

        public void OnSelected(SelectedEventArgs args)
        {
            Selected?.Invoke(this, args);
        }

        public new AutocompleteListView Items
        {
            get { return listView; }
        }

        /// <summary>
        /// Shows popup menu immediately
        /// </summary>
        /// <param name="forced">If True - MinFragmentLength will be ignored</param>
        public void Show(bool forced)
        {
            Items.DoAutocomplete(forced);
        }

        /// <summary>
        /// Minimal size of menu
        /// </summary>
        public new Size MinimumSize
        {
            get => Items.MinimumSize;
            set => Items.MinimumSize = value;
        }

        /// <summary>
        /// Image list of menu
        /// </summary>
        public new ImageList ImageList
        {
            get => Items.ImageList;
            set => Items.ImageList = value;
        }

        /// <summary>
        /// Tooltip duration (ms)
        /// </summary>
        public int ToolTipDuration
        {
            get => Items.ToolTipDuration;
            set => Items.ToolTipDuration = value;
        }

        /// <summary>
        /// Tooltip
        /// </summary>
        public ToolTip ToolTip
        {
            get => Items.toolTip;
            set => Items.toolTip = value;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (listView != null && !listView.IsDisposed)
                listView.Dispose();
        }
    }
}