using FastColoredTextBoxNS;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Tester {
	public partial class CustomWordWrapSample : Form {
		public CustomWordWrapSample() {
			InitializeComponent();
		}

		private readonly Regex regex = new(@"&&|&|\|\||\|");

		private void Fctb_WordWrapNeeded(object sender, WordWrapNeededEventArgs e) {
			//var max = (fctb.ClientSize.Width - fctb.LeftIndent)/fctb.CharWidth;
			//FastColoredTextBox.CalcCutOffs(e.CutOffPositions, max, max, e.ImeAllowed, true, e.Line);

			e.CutOffPositions.Clear();
			foreach (Match m in regex.Matches(e.Line.Text))
				e.CutOffPositions.Add(m.Index);
		}
	}
}
