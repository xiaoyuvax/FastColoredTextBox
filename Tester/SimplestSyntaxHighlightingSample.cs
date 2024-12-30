using FastColoredTextBoxNS;
using FastColoredTextBoxNS.Types;

namespace Tester
{
    public partial class SimplestSyntaxHighlightingSample : Form
    {
        //Create style for highlighting
        private readonly TextStyle brownStyle = new(Brushes.Brown, null, FontStyle.Regular);

        public SimplestSyntaxHighlightingSample()
        {
            InitializeComponent();
            cboCJKMode.Items.AddRange(Enum.GetNames<CJKMode>());
            cboCJKMode.Text = CJKMode.CJK.ToString();
        }

        private void Fctb_TextChanged(object sender, TextChangedEventArgs e)
        {
            //clear previous highlighting
            e.ChangedRange.ClearStyle(brownStyle);
            //highlight tags
            e.ChangedRange.SetStyle(brownStyle, "<[^>]+>");
        }

        private void cboCJKMode_SelectedIndexChanged(object sender, EventArgs e)
        {            
            fctb.UseCJK = Enum.TryParse(cboCJKMode.Text, out CJKMode cjk) ? cjk : CJKMode.Disabled;
        }
    }
}