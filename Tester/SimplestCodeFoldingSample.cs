using FastColoredTextBoxNS;
using System.Windows.Forms;

namespace Tester {
	public partial class SimplestCodeFoldingSample : Form {
		public SimplestCodeFoldingSample() => InitializeComponent();

		private void Fctb_TextChanged(object sender, TextChangedEventArgs e) {
			//clear folding markers
			e.ChangedRange.ClearFoldingMarkers();
			//set markers for folding
			e.ChangedRange.SetFoldingMarkers("{", "}");
		}
	}
}
