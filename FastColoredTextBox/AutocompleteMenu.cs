﻿using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using FastColoredTextBoxNS.Input;
using FastColoredTextBoxNS.Types;
using Timer = System.Windows.Forms.Timer;

namespace FastColoredTextBoxNS
{
    [ToolboxItem(false)]
    public class AutocompleteListView : UserControl, IDisposable
    {
        internal ToolTip toolTip = new();

        internal List<AutocompleteItem> visibleItems;

        private readonly int hoveredItemIndex = -1;

        private readonly FastColoredTextBox tb;

        private readonly Timer timer = new();

        private int focussedItemIndex = 0;

        private int oldItemCount = 0;

        private IEnumerable<AutocompleteItem> sourceItems = new List<AutocompleteItem>();

        internal AutocompleteListView(FastColoredTextBox tb)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            base.Font = new Font(FontFamily.GenericSansSerif, 9);
            visibleItems = [];
            VerticalScroll.SmallChange = ItemHeight;
            MaximumSize = new Size(Size.Width, 180);
            toolTip.ShowAlways = false;
            AppearInterval = 500;
            timer.Tick += new EventHandler(Timer_Tick);
            SelectedColor = Color.Orange;
            HoveredColor = Color.Red;
            ToolTipDuration = 3000;
            toolTip.Popup += ToolTip_Popup;

            this.tb = tb;

            tb.KeyDown += new KeyEventHandler(Tb_KeyDown);
            tb.SelectionChanged += new EventHandler(Tb_SelectionChanged);
            tb.KeyPressed += new KeyPressEventHandler(Tb_KeyPressed);

            Form form = tb.FindForm();
            if (form != null)
            {
                form.LocationChanged += delegate { SafetyClose(); };
                form.ResizeBegin += delegate { SafetyClose(); };
                form.FormClosing += delegate { SafetyClose(); };
                form.LostFocus += delegate { SafetyClose(); };
            }

            tb.LostFocus += (o, e) =>
            {
                if (Menu != null && !Menu.IsDisposed)
                    if (!Menu.Focused)
                        SafetyClose();
            };

            tb.Scroll += delegate { SafetyClose(); };

            this.VisibleChanged += (o, e) =>
            {
                if (this.Visible)
                    DoSelectedVisible();
            };
        }

        public event EventHandler FocussedItemIndexChanged;
        public int Count
        {
            get { return visibleItems.Count; }
        }

        public AutocompleteItem FocussedItem
        {
            get
            {
                if (FocussedItemIndex >= 0 && focussedItemIndex < visibleItems.Count)
                    return visibleItems[focussedItemIndex];
                return null;
            }
            set
            {
                FocussedItemIndex = visibleItems.IndexOf(value);
            }
        }

        public int FocussedItemIndex
        {
            get { return focussedItemIndex; }
            set
            {
                if (focussedItemIndex != value)
                {
                    focussedItemIndex = value;
                    FocussedItemIndexChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public Color HoveredColor { get; set; }

        public ImageList ImageList { get; set; }

        public Color SelectedColor { get; set; }

        internal bool AllowTabKey { get; set; }

        internal bool AlwaysShowTooltip
        {
            get { return toolTip.ShowAlways; }
            set { toolTip.ShowAlways = value; }
        }

        internal int AppearInterval
        { get { return timer.Interval; } set { timer.Interval = value; } }

        internal Size MaxToolTipSize { get; set; }

        internal int ToolTipDuration { get; set; }

        private int ItemHeight
        {
            get { return Font.Height + 2; }
        }

        private AutocompleteMenu Menu
        { get { return Parent as AutocompleteMenu; } }
        public void SelectNext(int shift)
        {
            FocussedItemIndex = Math.Max(0, Math.Min(FocussedItemIndex + shift, visibleItems.Count - 1));
            DoSelectedVisible();
            //
            Invalidate();
        }

        public void SetAutocompleteItems(ICollection<string> items)
        {
            List<AutocompleteItem> list = new(items.Count);
            foreach (var item in items)
                list.Add(new AutocompleteItem(item));
            SetAutocompleteItems(list);
        }

        public void SetAutocompleteItems(IEnumerable<AutocompleteItem> items)
        {
            sourceItems = items;
        }

        internal void DoAutocomplete()
        {
            DoAutocomplete(false);
        }

        internal void DoAutocomplete(bool forced)
        {
            if (!Menu.Enabled)
            {
                Menu.Close();
                return;
            }

            visibleItems.Clear();
            FocussedItemIndex = 0;
            VerticalScroll.Value = 0;
            //some magic for update scrolls
            AutoScrollMinSize -= new Size(1, 0);
            AutoScrollMinSize += new Size(1, 0);
            //get fragment around caret
            TextSelectionRange fragment = tb.Selection.GetFragment(Menu.SearchPattern);
            string text = fragment.Text;
            //calc screen point for popup menu
            Point point = tb.PlaceToPoint(fragment.End);
            point.Offset(2, tb.CharHeight);
            //
            if (forced || (text.Length >= Menu.MinFragmentLength
                && tb.Selection.IsEmpty /*pops up only if selected range is empty*/
                && (tb.Selection.Start > fragment.Start || text.Length == 0/*pops up only if caret is after first letter*/)))
            {
                Menu.Fragment = fragment;
                bool foundSelected = false;
                //build popup menu
                foreach (var item in sourceItems)
                {
                    item.Parent = Menu;
                    CompareResult res = item.Compare(text);
                    if (res != CompareResult.Hidden)
                        visibleItems.Add(item);
                    if (res == CompareResult.VisibleAndSelected && !foundSelected)
                    {
                        foundSelected = true;
                        FocussedItemIndex = visibleItems.Count - 1;
                    }
                }
            }

            //show popup menu
            if (Count > 0)
            {
                if (!Menu.Visible)
                {
                    CancelEventArgs args = new();
                    Menu.OnOpening(args);
                    if (!args.Cancel)
                        Menu.Show(tb, point);
                }

                DoSelectedVisible();
                Invalidate();
            }
            else
                Menu.Close();
        }

        internal virtual void OnSelecting()
        {
            if (FocussedItemIndex < 0 || FocussedItemIndex >= visibleItems.Count)
                return;
            tb.TextSource.Manager.BeginAutoUndoCommands();
            try
            {
                AutocompleteItem item = FocussedItem;
                SelectingEventArgs args = new()
                {
                    Item = item,
                    SelectedIndex = FocussedItemIndex
                };

                Menu.OnSelecting(args);

                if (args.Cancel)
                {
                    FocussedItemIndex = args.SelectedIndex;
                    Invalidate();
                    return;
                }

                if (!args.Handled)
                {
                    var fragment = Menu.Fragment;
                    DoAutocomplete(item, fragment);
                }

                Menu.Close();
                //
                SelectedEventArgs args2 = new()
                {
                    Item = item,
                    Tb = Menu.Fragment.tb
                };
                item.OnSelected(Menu, args2);
                Menu.OnSelected(args2);
            }
            finally
            {
                tb.TextSource.Manager.EndAutoUndoCommands();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (toolTip != null)
            {
                toolTip.Popup -= ToolTip_Popup;
                toolTip.Dispose();
            }
            if (tb != null)
            {
                tb.KeyDown -= Tb_KeyDown;
                tb.KeyPressed -= Tb_KeyPressed;
                tb.SelectionChanged -= Tb_SelectionChanged;
            }

            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= Timer_Tick;
                timer.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                FocussedItemIndex = PointToItemIndex(e.Location);
                DoSelectedVisible();
                Invalidate();
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            FocussedItemIndex = PointToItemIndex(e.Location);
            Invalidate();
            OnSelecting();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            AdjustScroll();

            var itemHeight = ItemHeight;
            int startI = VerticalScroll.Value / itemHeight - 1;
            int finishI = (VerticalScroll.Value + ClientSize.Height) / itemHeight + 1;
            startI = Math.Max(startI, 0);
            finishI = Math.Min(finishI, visibleItems.Count);
            int leftPadding = 18;
            for (int i = startI; i < finishI; i++)
            {
                int y = i * itemHeight - VerticalScroll.Value;

                var item = visibleItems[i];

                if (item.BackColor != Color.Transparent)
                    using (var brush = new SolidBrush(item.BackColor))
                        e.Graphics.FillRectangle(brush, 1, y, ClientSize.Width - 1 - 1, itemHeight - 1);

                if (ImageList != null && visibleItems[i].ImageIndex >= 0)
                    e.Graphics.DrawImage(ImageList.Images[item.ImageIndex], 1, y);

                if (i == FocussedItemIndex)
                    using (var selectedBrush = new LinearGradientBrush(new Point(0, y - 3), new Point(0, y + itemHeight), Color.Transparent, SelectedColor))
                    using (var pen = new Pen(SelectedColor))
                    {
                        e.Graphics.FillRectangle(selectedBrush, leftPadding, y, ClientSize.Width - 1 - leftPadding, itemHeight - 1);
                        e.Graphics.DrawRectangle(pen, leftPadding, y, ClientSize.Width - 1 - leftPadding, itemHeight - 1);
                    }

                if (i == hoveredItemIndex)
                    using (var pen = new Pen(HoveredColor))
                        e.Graphics.DrawRectangle(pen, leftPadding, y, ClientSize.Width - 1 - leftPadding, itemHeight - 1);

                using (var brush = new SolidBrush(item.ForeColor != Color.Transparent ? item.ForeColor : ForeColor))
                    e.Graphics.DrawString(item.ToString(), Font, brush, leftPadding, y);
            }
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            Invalidate();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            ProcessKey(keyData, Keys.None);

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private static void DoAutocomplete(AutocompleteItem item, TextSelectionRange fragment)
        {
            string newText = item.GetTextForReplace();

            //replace text of fragment
            var tb = fragment.tb;

            tb.BeginAutoUndo();
            tb.TextSource.Manager.ExecuteCommand(new SelectCommand(tb.TextSource));
            if (tb.Selection.ColumnSelectionMode)
            {
                var start = tb.Selection.Start;
                var end = tb.Selection.End;
                start.iChar = fragment.Start.iChar;
                end.iChar = fragment.End.iChar;
                tb.Selection.Start = start;
                tb.Selection.End = end;
            }
            else
            {
                tb.Selection.Start = fragment.Start;
                tb.Selection.End = fragment.End;
            }
            tb.InsertText(newText);
            tb.TextSource.Manager.ExecuteCommand(new SelectCommand(tb.TextSource));
            tb.EndAutoUndo();
            tb.Focus();
        }

        private static void ResetTimer(Timer timer)
        {
            timer.Stop();
            timer.Start();
        }

        private void AdjustScroll()
        {
            if (oldItemCount == visibleItems.Count)
                return;

            int needHeight = ItemHeight * visibleItems.Count + 1;
            Height = Math.Min(needHeight, MaximumSize.Height);
            Menu.CalcSize();

            AutoScrollMinSize = new Size(0, needHeight);
            oldItemCount = visibleItems.Count;
        }

        private void DoSelectedVisible()
        {
            if (FocussedItem != null)
                SetToolTip(FocussedItem);

            var y = FocussedItemIndex * ItemHeight - VerticalScroll.Value;
            if (y < 0)
                VerticalScroll.Value = FocussedItemIndex * ItemHeight;
            if (y > ClientSize.Height - ItemHeight)
                VerticalScroll.Value = Math.Min(VerticalScroll.Maximum, FocussedItemIndex * ItemHeight - ClientSize.Height + ItemHeight);
            //some magic for update scrolls
            AutoScrollMinSize -= new Size(1, 0);
            AutoScrollMinSize += new Size(1, 0);
        }

        private int PointToItemIndex(Point p)
        {
            return (p.Y + VerticalScroll.Value) / ItemHeight;
        }

        private bool ProcessKey(Keys keyData, Keys keyModifiers)
        {
            if (keyModifiers == Keys.None)
                switch (keyData)
                {
                    case Keys.Down:
                        SelectNext(+1);
                        return true;

                    case Keys.PageDown:
                        SelectNext(+10);
                        return true;

                    case Keys.Up:
                        SelectNext(-1);
                        return true;

                    case Keys.PageUp:
                        SelectNext(-10);
                        return true;

                    case Keys.Enter:
                        OnSelecting();
                        return true;

                    case Keys.Tab:
                        if (!AllowTabKey)
                            break;
                        OnSelecting();
                        return true;

                    case Keys.Escape:
                        Menu.Close();
                        return true;
                }

            return false;
        }

        private void SafetyClose()
        {
            if (Menu != null && !Menu.IsDisposed)
                Menu.Close();
        }

        private void SetToolTip(AutocompleteItem autocompleteItem)
        {
            var title = autocompleteItem.ToolTipTitle;
            var text = autocompleteItem.ToolTipText;

            if (string.IsNullOrEmpty(title))
            {
                toolTip.ToolTipTitle = null;
                toolTip.SetToolTip(this, null);
                return;
            }

            if (this.Parent != null)
            {
                IWin32Window window = this.Parent ?? this;
                Point location;

                if ((this.PointToScreen(this.Location).X + MaxToolTipSize.Width + 105) < Screen.FromControl(this.Parent).WorkingArea.Right)
                    location = new Point(Right + 5, 0);
                else
                    location = new Point(Left - 105 - MaximumSize.Width, 0);

                if (string.IsNullOrEmpty(text))
                {
                    toolTip.ToolTipTitle = null;
                    toolTip.Show(title, window, location.X, location.Y, ToolTipDuration);
                }
                else
                {
                    toolTip.ToolTipTitle = title;
                    toolTip.Show(text, window, location.X, location.Y, ToolTipDuration);
                }
            }
        }

        private void Tb_KeyDown(object sender, KeyEventArgs e)
        {
            var tb = sender as FastColoredTextBox;

            if (Menu.Visible)
                if (ProcessKey(e.KeyCode, e.Modifiers))
                    e.Handled = true;

            if (!Menu.Visible)
            {
                if (tb.HotkeysMapping.ContainsKey(e.KeyData) && tb.HotkeysMapping[e.KeyData] == FCTBAction.AutocompleteMenu)
                {
                    DoAutocomplete();
                    e.Handled = true;
                }
                else
                {
                    if (e.KeyCode == Keys.Escape && timer.Enabled)
                        timer.Stop();
                }
            }
        }

        private void Tb_KeyPressed(object sender, KeyPressEventArgs e)
        {
            bool backspaceORdel = e.KeyChar == '\b' || e.KeyChar == 0xff;

            /*
            if (backspaceORdel)
                prevSelection = tb.Selection.Start;*/

            if (Menu.Visible && !backspaceORdel)
                DoAutocomplete(false);
            else
                ResetTimer(timer);
        }

        private void Tb_SelectionChanged(object sender, EventArgs e)
        {
            /*
            FastColoredTextBox tb = sender as FastColoredTextBox;

            if (Math.Abs(prevSelection.iChar - tb.Selection.Start.iChar) > 1 ||
                        prevSelection.iLine != tb.Selection.Start.iLine)
                Menu.Close();
            prevSelection = tb.Selection.Start;*/
            if (Menu.Visible)
            {
                bool needClose = false;

                if (!tb.Selection.IsEmpty)
                    needClose = true;
                else
                    if (!Menu.Fragment.Contains(tb.Selection.Start))
                {
                    if (tb.Selection.Start.iLine == Menu.Fragment.End.iLine && tb.Selection.Start.iChar == Menu.Fragment.End.iChar + 1)
                    {
                        //user press key at end of fragment
                        char c = tb.Selection.CharBeforeStart;
                        if (!Regex.IsMatch(c.ToString(), Menu.SearchPattern))//check char
                            needClose = true;
                    }
                    else
                        needClose = true;
                }

                if (needClose)
                    Menu.Close();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            DoAutocomplete(false);
        }

        private void ToolTip_Popup(object sender, PopupEventArgs e)
        {
            if (MaxToolTipSize.Height > 0 && MaxToolTipSize.Width > 0)
                e.ToolTipSize = MaxToolTipSize;
        }
    }

    /// <summary>
    /// Popup menu for autocomplete
    /// </summary>
    [Browsable(false)]
    public class AutocompleteMenu : ToolStripDropDown, IDisposable
    {
        public ToolStripControlHost host;
        private readonly AutocompleteListView listView;
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

        /// <summary>
        /// Occurs when popup menu is opening
        /// </summary>
        public new event EventHandler<CancelEventArgs> Opening;

        /// <summary>
        /// It fires after item inserting
        /// </summary>
        public event EventHandler<SelectedEventArgs> Selected;

        /// <summary>
        /// User selects item
        /// </summary>
        public event EventHandler<SelectingEventArgs> Selecting;

        /// <summary>
        /// Allow TAB for select menu item
        /// </summary>
        public bool AllowTabKey
        { get { return listView.AllowTabKey; } set { listView.AllowTabKey = value; } }

        /// <summary>
        /// Tooltip will perm show and duration will be ignored
        /// </summary>
        public bool AlwaysShowTooltip
        { get { return listView.AlwaysShowTooltip; } set { listView.AlwaysShowTooltip = value; } }

        /// <summary>
        /// Interval of menu appear (ms)
        /// </summary>
        public int AppearInterval
        { get { return listView.AppearInterval; } set { listView.AppearInterval = value; } }

        public new Font Font
        {
            get { return listView.Font; }
            set { listView.Font = value; }
        }

        public TextSelectionRange Fragment { get; internal set; }

        /// <summary>
        /// Border color of hovered item
        /// </summary>
        [DefaultValue(typeof(Color), "Red")]
        public Color HoveredColor
        {
            get { return listView.HoveredColor; }
            set { listView.HoveredColor = value; }
        }

        /// <summary>
        /// Image list of menu
        /// </summary>
        public new ImageList ImageList
        {
            get { return Items.ImageList; }
            set { Items.ImageList = value; }
        }

        public new AutocompleteListView Items
        {
            get { return listView; }
        }

        /// <summary>
        /// Sets the max tooltip window size
        /// </summary>
        public Size MaxTooltipSize
        { get { return listView.MaxToolTipSize; } set { listView.MaxToolTipSize = value; } }

        /// <summary>
        /// Minimum fragment length for popup
        /// </summary>
        public int MinFragmentLength { get; set; }

        /// <summary>
        /// Minimal size of menu
        /// </summary>
        public new Size MinimumSize
        {
            get { return Items.MinimumSize; }
            set { Items.MinimumSize = value; }
        }

        /// <summary>
        /// Regex pattern for serach fragment around caret
        /// </summary>
        public string SearchPattern { get; set; }
        /// <summary>
        /// Back color of selected item
        /// </summary>
        [DefaultValue(typeof(Color), "Orange")]
        public Color SelectedColor
        {
            get { return listView.SelectedColor; }
            set { listView.SelectedColor = value; }
        }
        /// <summary>
        /// Tooltip
        /// </summary>
        public ToolTip ToolTip
        {
            get { return Items.toolTip; }
            set { Items.toolTip = value; }
        }

        /// <summary>
        /// Tooltip duration (ms)
        /// </summary>
        public int ToolTipDuration
        {
            get { return Items.ToolTipDuration; }
            set { Items.ToolTipDuration = value; }
        }

        public new void Close()
        {
            listView.toolTip.Hide(listView);
            base.Close();
        }

        public void OnSelected(SelectedEventArgs args)
        {
            Selected?.Invoke(this, args);
        }

        public virtual void OnSelecting()
        {
            listView.OnSelecting();
        }

        public void SelectNext(int shift)
        {
            listView.SelectNext(shift);
        }

        /// <summary>
        /// Shows popup menu immediately
        /// </summary>
        /// <param name="forced">If True - MinFragmentLength will be ignored</param>
        public void Show(bool forced)
        {
            Items.DoAutocomplete(forced);
        }

        internal void CalcSize()
        {
            host.Size = listView.Size;
            Size = new Size(listView.Size.Width + 4, listView.Size.Height + 4);
        }

        internal new void OnOpening(CancelEventArgs args)
        {
            Opening?.Invoke(this, args);
        }
        internal void OnSelecting(SelectingEventArgs args)
        {
            Selecting?.Invoke(this, args);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (listView != null && !listView.IsDisposed)
                listView.Dispose();
        }
    }
    public class SelectedEventArgs : EventArgs
    {
        public AutocompleteItem Item { get; internal set; }
        public FastColoredTextBox Tb { get; set; }
    }

    public class SelectingEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
        public bool Handled { get; set; }
        public AutocompleteItem Item { get; internal set; }
        public int SelectedIndex { get; set; }
    }
}