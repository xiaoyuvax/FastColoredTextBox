using FastColoredTextBoxNS;
using FastColoredTextBoxNS.Types;
using System.Drawing;
using System.Windows.Forms;

namespace Tester {
	public partial class DynamicSyntaxHighlighting : Form {
		readonly Style KeywordsStyle = new TextStyle(Brushes.Green, null, FontStyle.Regular);
		readonly Style FunctionNameStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);

		public DynamicSyntaxHighlighting() {
			InitializeComponent();
		}

		private void Fctb_TextChangedDelayed(object sender, TextChangedEventArgs e) {
			//clear styles
			fctb.Range.ClearStyle(KeywordsStyle, FunctionNameStyle);
			//highlight keywords of LISP
			fctb.Range.SetStyle(KeywordsStyle, @"\b(and|eval|else|if|lambda|or|set|defun)\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
			//find function declarations, highlight all of their entry into the code
			foreach (TextSelectionRange found in fctb.GetRanges(@"\b(defun|DEFUN)\s+(?<range>\w+)\b"))
				fctb.Range.SetStyle(FunctionNameStyle, @"\b" + found.Text + @"\b");
		}
	}
}
