using System;
using System.Windows.Forms;

namespace Tester {
	public partial class MainForm : Form {
		public MainForm() => InitializeComponent();

		private void Button1_Click(object sender, EventArgs e) => new PowerfulSample().Show();
		private void Button2_Click(object sender, EventArgs e) => new MarkerToolSample().Show();
		private void Button3_Click(object sender, EventArgs e) => new CustomStyleSample().Show();

		private void Button4_Click(object sender, EventArgs e) {
			Cursor = Cursors.WaitCursor;
			new VisibleRangeChangedDelayedSample().Show();
			Cursor = Cursors.Default;
		}

		private void Button5_Click(object sender, EventArgs e) => new SimplestSyntaxHighlightingSample().Show();
		private void Button6_Click(object sender, EventArgs e) => new JokeSample().Show();
		private void Button7_Click(object sender, EventArgs e) => new SimplestCodeFoldingSample().Show();
		private void Button8_Click(object sender, EventArgs e) => new AutocompleteSample().Show();
		private void Button9_Click(object sender, EventArgs e) => new DynamicSyntaxHighlighting().Show();
		private void Button10_Click(object sender, EventArgs e) => new SyntaxHighlightingByXmlDescription().Show();
		private void Button11_Click(object sender, EventArgs e) => new IMEsample().Show();
		private void Button12_Click(object sender, EventArgs e) => new PowerfulCSharpEditor().Show();
		private void Button13_Click(object sender, EventArgs e) => new GifImageDrawingSample().Show();
		private void Button14_Click(object sender, EventArgs e) => new AutocompleteSample2().Show();
		private void Button15_Click(object sender, EventArgs e) => new AutoIndentSample().Show();
		private void Button16_Click(object sender, EventArgs e) => new BookmarksSample().Show();
		private void Button17_Click(object sender, EventArgs e) => new LoggerSample().Show();
		private void Button18_Click(object sender, EventArgs e) => new TooltipSample().Show();
		private void Button19_Click(object sender, EventArgs e) => new SplitSample().Show();
		private void Button20_Click(object sender, EventArgs e) => new LazyLoadingSample().Show();
		private void Button21_Click(object sender, EventArgs e) => new ConsoleSample().Show();
		private void Button22_Click(object sender, EventArgs e) => new CustomFoldingSample().Show();
		private void Button23_Click(object sender, EventArgs e) => new BilingualHighlighterSample().Show();
		private void Button24_Click(object sender, EventArgs e) => new HyperlinkSample().Show();
		private void Button25_Click(object sender, EventArgs e) => new CustomTextSourceSample().Show();
		private void Button26_Click(object sender, EventArgs e) => new HintSample().Show();
		private void Button27_Click(object sender, EventArgs e) => new ReadOnlyBlocksSample().Show();
		private void Button28_Click(object sender, EventArgs e) => new PredefinedStylesSample().Show();
		private void Button29_Click(object sender, EventArgs e) => new MacrosSample().Show();
		private void Button30_Click(object sender, EventArgs e) => new OpenTypeFontSample().Show();
		private void MainForm_DoubleClick(object sender, EventArgs e) => new Sandbox().ShowDialog();
		private void Button31_Click(object sender, EventArgs e) => new RulerSample().Show();
		private void Button32_Click(object sender, EventArgs e) => new AutocompleteSample3().Show();
		private void Button33_Click(object sender, EventArgs e) => new AutocompleteSample4().Show();
		private void Button34_Click(object sender, EventArgs e) => new DocumentMapSample().Show();
		private void Button35_Click(object sender, EventArgs e) => new DiffMergeSample().Show();
		private void Button36_Click(object sender, EventArgs e) => new CustomScrollBarsSample().Show();
		private void Button37_Click(object sender, EventArgs e) => new CustomWordWrapSample().Show();
		private void Button38_Click(object sender, EventArgs e) => new AutoIndentCharsSample().Show();
		private void Button39_Click(object sender, EventArgs e) => new CustomTextSourceSample2().Show();
	}
}
