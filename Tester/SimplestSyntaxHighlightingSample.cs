using FastColoredTextBoxNS;
using System.Drawing;
using System.Windows.Forms;

namespace Tester {
	public partial class SimplestSyntaxHighlightingSample : Form {
		//Create style for highlighting
		readonly TextStyle brownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Regular);

		public SimplestSyntaxHighlightingSample() => InitializeComponent();

		private void Fctb_TextChanged(object sender, TextChangedEventArgs e) {
			//clear previous highlighting
			e.ChangedRange.ClearStyle(brownStyle);
			//highlight tags
			e.ChangedRange.SetStyle(brownStyle, "<[^>]+>");
		}
	}
}
