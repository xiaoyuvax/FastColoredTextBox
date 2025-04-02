using FastColoredTextBoxNS.EventArg;
using FastColoredTextBoxNS.Types;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Tester {
	public partial class LazyLoadingSample : Form {
		public LazyLoadingSample() {
			InitializeComponent();
		}

		private void MiOpen_Click(object sender, EventArgs e) {
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				fctb.OpenBindingFile(ofd.FileName, Encoding.UTF8);
				fctb.IsChanged = false;
				fctb.ClearUndo();
				GC.Collect();
				GC.GetTotalMemory(true);
			}
		}

		private void Fctb_VisibleRangeChangedDelayed(object sender, EventArgs e) => HighlightVisibleRange();
		private void Fctb_TextChangedDelayed(object sender, TextChangedEventArgs e) => HighlightVisibleRange();

		const int margin = 2000;

		private void HighlightVisibleRange() {
			//expand visible range (+- margin)
			var startLine = Math.Max(0, fctb.VisibleRange.Start.iLine - margin);
			var endLine = Math.Min(fctb.LinesCount - 1, fctb.VisibleRange.End.iLine + margin);
			var range = new TextSelectionRange(fctb, 0, startLine, 0, endLine);
			//clear folding markers
			range.ClearFoldingMarkers();
			//set markers for folding
			range.SetFoldingMarkers(@"N\d\d00", @"N\d\d99");
			//
			range.ClearAllStyles();
			range.SetStyle(fctb.SyntaxHighlighter.BlueStyle, @"N\d+");
			range.SetStyle(fctb.SyntaxHighlighter.RedStyle, @"[+\-]?[\d\.]+\d+");
		}

		private void CloseFileToolStripMenuItem_Click(object sender, EventArgs e) => fctb.CloseBindingFile();
		private void LazyLoadingSample_FormClosing(object sender, FormClosingEventArgs e) => fctb.CloseBindingFile();

		private void MiSave_Click(object sender, EventArgs e) {
			if (sfd.ShowDialog() == DialogResult.OK) {
				fctb.SaveToFile(sfd.FileName, Encoding.UTF8);
			}
		}

		private void CreateTestFileToolStripMenuItem_Click(object sender, EventArgs e) {
			Random rnd = new();

			if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				using (StreamWriter sw = new(sfd.FileName, false, Encoding.Default)) {
					//create large test file
					for (int j = 0; j < 130; j++) {
						sw.WriteLine("\r\n--====" + j + "=====--\r\n");
						for (int i = 0; i < 10000; i++)
							sw.WriteLine(string.Format("N{0:0000} X{1} Y{2} Z{3}", i, rnd.Next(), rnd.Next(), rnd.Next()));
					}
				}
		}

		private void CollapseAllFoldingBlocksToolStripMenuItem_Click(object sender, EventArgs e) {
			fctb.CollapseAllFoldingBlocks();
			fctb.DoSelectionVisible();
		}

		private void ExpandAllCollapsedBlocksToolStripMenuItem_Click(object sender, EventArgs e) {
			fctb.ExpandAllFoldingBlocks();
			fctb.DoSelectionVisible();
		}

		private void RemoveEmptyLinesToolStripMenuItem_Click(object sender, EventArgs e) {
			var iLines = fctb.FindLines(@"^\s*$", RegexOptions.None);
			fctb.RemoveLines(iLines);
		}
		/*
        private void button1_Click(object sender, EventArgs e)
        {
            var replaces = new List<ReplaceMultipleTextCommand.ReplaceRange>();
            replaces.Add(new ReplaceMultipleTextCommand.ReplaceRange() { ReplacedRange = new Range(fctb, 0), ReplaceText = "0"});
            replaces.Add(new ReplaceMultipleTextCommand.ReplaceRange() { ReplacedRange = new Range(fctb, 1), ReplaceText = "1" });
            replaces.Add(new ReplaceMultipleTextCommand.ReplaceRange() { ReplacedRange = new Range(fctb, 2), ReplaceText = "2" });
            fctb.TextSource.Manager.ExecuteCommand(new ReplaceMultipleTextCommand(fctb.TextSource, replaces));

            Console.WriteLine(fctb[0].IsChanged);
            Console.WriteLine(fctb[1].IsChanged);
            Console.WriteLine(fctb[2].IsChanged);
        }*/
	}
}
