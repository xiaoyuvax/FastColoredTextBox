using FastColoredTextBoxNS.Input;
using FastColoredTextBoxNS.Types;
using System.Text.RegularExpressions;


namespace FastColoredTextBoxNS
{
    public partial class ReplaceForm : Form
    {
        private readonly FastColoredTextBox tb;
        bool firstSearch = true;
        Place startPlace;

        public ReplaceForm(FastColoredTextBox tb)
        {
            InitializeComponent();
            this.tb = tb;
            ApplyLocalization();
        }

        private void ApplyLocalization()
        {
            Text = Localization.GetString("ReplaceForm_Title");
            label1.Text = Localization.GetString("ReplaceForm_FindLabel");
            label2.Text = Localization.GetString("ReplaceForm_ReplaceLabel");
            cbMatchCase.Text = Localization.GetString("ReplaceForm_MatchCase");
            cbWholeWord.Text = Localization.GetString("ReplaceForm_MatchWholeWord");
            cbRegex.Text = Localization.GetString("ReplaceForm_Regex");
            btFindNext.Text = Localization.GetString("ReplaceForm_FindNext");
            btReplace.Text = Localization.GetString("ReplaceForm_Replace");
            btReplaceAll.Text = Localization.GetString("ReplaceForm_ReplaceAll");
            btClose.Text = Localization.GetString("ReplaceForm_Close");
        }

        public bool Find(string pattern)
        {
            RegexOptions opt = cbMatchCase.Checked ? RegexOptions.None : RegexOptions.IgnoreCase;
            if (!cbRegex.Checked)
                pattern = Regex.Escape(pattern);
            if (cbWholeWord.Checked)
                pattern = "\\b" + pattern + "\\b";
            //
            TextSelectionRange range = tb.Selection.Clone();
            range.Normalize();
            //
            if (firstSearch)
            {
                startPlace = range.Start;
                firstSearch = false;
            }
            //
            range.Start = range.End;
            if (range.Start >= startPlace)
                tb.Selection.SetStartAndEnd(new Place(0, 0));
            else
                range.End = startPlace;
            //
            foreach (var r in range.GetRangesByLines(pattern, opt))
            {
                tb.Selection.Start = r.Start;
                tb.Selection.End = r.End;
                tb.DoSelectionVisible();
                tb.Invalidate();
                return true;
            }
            if (range.Start >= startPlace && startPlace > Place.Empty)
            {
                tb.Selection.Start = new Place(0, 0);
                return Find(pattern);
            }
            return false;
        }

        public List<TextSelectionRange> FindAll(string pattern)
        {
            var opt = cbMatchCase.Checked ? RegexOptions.None : RegexOptions.IgnoreCase;
            if (!cbRegex.Checked)
                pattern = Regex.Escape(pattern);
            if (cbWholeWord.Checked)
                pattern = "\\b" + pattern + "\\b";
            //
            var range = tb.Selection.IsEmpty ? tb.Range.Clone() : tb.Selection.Clone();
            //
            var list = new List<TextSelectionRange>();
            foreach (var r in range.GetRangesByLines(pattern, opt))
                list.Add(r);

            return list;
        }

        protected override void OnActivated(EventArgs e)
        {
            tbFind.Focus();
            ResetSerach();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void BtClose_Click(object sender, EventArgs e) => Close();

        private void BtFindNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Find(tbFind.Text))
                    MessageBox.Show(Localization.GetString("ReplaceForm_NotFound"));
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void BtReplace_Click(object sender, EventArgs e)
        {
            try
            {
                if (tb.SelectionLength != 0)
                    if (!tb.Selection.ReadOnly)
                        tb.InsertText(tbReplace.Text);
                BtFindNext_Click(sender, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtReplaceAll_Click(object sender, EventArgs e)
        {
            try
            {
                tb.Selection.BeginUpdate();

                //search
                var ranges = FindAll(tbFind.Text);
                //check readonly
                var ro = false;
                foreach (var r in ranges)
                    if (r.ReadOnly)
                    {
                        ro = true;
                        break;
                    }
                //replace
                if (!ro)
                    if (ranges.Count > 0)
                    {
                        tb.TextSource.Manager.ExecuteCommand(new ReplaceTextCommand(tb.TextSource, ranges, tbReplace.Text));
                        tb.Selection.SetStartAndEnd(new Place(0, 0));
                    }
                //
                tb.Invalidate();
                MessageBox.Show(Localization.GetString("ReplaceForm_ReplacedCount", ranges.Count));
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            tb.Selection.EndUpdate();
        }

        private void CbMatchCase_CheckedChanged(object sender, EventArgs e) => ResetSerach();

        private void ReplaceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
            tb.Focus();
        }

        void ResetSerach() => firstSearch = true;

        private void TbFind_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '\r':
                    BtFindNext_Click(sender, null);
                    break;

                case '\x1b':
                    Hide();
                    break;
            }
        }
    }
}