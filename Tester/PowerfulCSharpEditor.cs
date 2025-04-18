﻿using FarsiLibrary.Win;
using FastColoredTextBoxNS;
using FastColoredTextBoxNS.EventArg;
using FastColoredTextBoxNS.Feature;
using FastColoredTextBoxNS.Text;
using FastColoredTextBoxNS.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Tester {
	public partial class PowerfulCSharpEditor : Form {
		readonly string[] keywords = { "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while", "add", "alias", "ascending", "descending", "dynamic", "from", "get", "global", "group", "into", "join", "let", "orderby", "partial", "remove", "select", "set", "value", "var", "where", "yield" };
		readonly string[] methods = { "Equals()", "GetHashCode()", "GetType()", "ToString()" };
		readonly string[] snippets = { "if(^)\n{\n;\n}", "if(^)\n{\n;\n}\nelse\n{\n;\n}", "for(^;;)\n{\n;\n}", "while(^)\n{\n;\n}", "do\n{\n^;\n}while();", "switch(^)\n{\ncase : break;\n}" };
		readonly string[] declarationSnippets = {
			   "public class ^\n{\n}", "private class ^\n{\n}", "internal class ^\n{\n}",
			   "public struct ^\n{\n;\n}", "private struct ^\n{\n;\n}", "internal struct ^\n{\n;\n}",
			   "public void ^()\n{\n;\n}", "private void ^()\n{\n;\n}", "internal void ^()\n{\n;\n}", "protected void ^()\n{\n;\n}",
			   "public ^{ get; set; }", "private ^{ get; set; }", "internal ^{ get; set; }", "protected ^{ get; set; }"
			   };
		readonly Style invisibleCharsStyle = new InvisibleCharsRenderer(Pens.Gray);
		readonly Color currentLineColor = Color.FromArgb(100, 210, 210, 255);
		readonly Color changedLineColor = Color.FromArgb(255, 230, 230, 255);


		public PowerfulCSharpEditor() {
			InitializeComponent();

			//init menu images
			ComponentResourceManager resources = new(typeof(PowerfulCSharpEditor));
			copyToolStripMenuItem.Image = (Image)resources.GetObject("copyToolStripButton.Image");
			cutToolStripMenuItem.Image = (Image)resources.GetObject("cutToolStripButton.Image");
			pasteToolStripMenuItem.Image = (Image)resources.GetObject("pasteToolStripButton.Image");
		}


		private void NewToolStripMenuItem_Click(object sender, EventArgs e) => CreateTab(null);
		private readonly Style sameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(50, Color.Gray)));

		private void CreateTab(string fileName) {
			try {
				var tb = new FastColoredTextBox {
					Font = new Font("Consolas", 9.75f),
					ContextMenuStrip = cmMain,
					Dock = DockStyle.Fill,
					BorderStyle = BorderStyle.Fixed3D,
					//tb.VirtualSpace = true;
					LeftPadding = 17,
					Language = Language.CSharp
				};
				tb.AddStyle(sameWordsStyle);//same words style
				var tab = new FATabStripItem(fileName != null ? Path.GetFileName(fileName) : "[new]", tb) {
					Tag = fileName
				};
				if (fileName != null)
					tb.OpenFile(fileName);
				tb.Tag = new TbInfo();
				tsFiles.AddTab(tab);
				tsFiles.SelectedItem = tab;
				tb.Focus();
				tb.DelayedTextChangedInterval = 1000;
				tb.DelayedEventsInterval = 500;
				tb.TextChangedDelayed += new EventHandler<TextChangedEventArgs>(Tb_TextChangedDelayed);
				tb.SelectionChangedDelayed += new EventHandler(Tb_SelectionChangedDelayed);
				tb.KeyDown += new KeyEventHandler(Tb_KeyDown);
				tb.MouseMove += new MouseEventHandler(Tb_MouseMove);
				tb.ChangedLineColor = changedLineColor;
				if (btHighlightCurrentLine.Checked)
					tb.CurrentLineColor = currentLineColor;
				tb.ShowFoldingLines = btShowFoldingLines.Checked;
				tb.HighlightingRangeType = HighlightingRangeType.VisibleRange;
				//create autocomplete popup menu
				AutocompleteMenu popupMenu = new(tb);
				popupMenu.Items.ImageList = ilAutocomplete;
				popupMenu.Opening += new EventHandler<CancelEventArgs>(PopupMenu_Opening);
				BuildAutocompleteMenu(popupMenu);
				(tb.Tag as TbInfo).popupMenu = popupMenu;
			} catch (Exception ex) {
				if (MessageBox.Show(ex.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Retry)
					CreateTab(fileName);
			}
		}

		private void PopupMenu_Opening(object sender, CancelEventArgs e) {
			//---block autocomplete menu for comments
			//get index of green style (used for comments)
         if (!CurrentTB.StyleManager.IsManaged(CurrentTB.SyntaxHighlighter.GreenStyle))
            return;

         if (CurrentTB.Selection.Start.iChar > 0)
			{
            //current char (before caret)
            var c = CurrentTB[CurrentTB.Selection.Start.iLine][CurrentTB.Selection.Start.iChar - 1];
            //if char contains green style then block popup menu
            if (c.HasStyle(CurrentTB.SyntaxHighlighter.GreenStyle))
               e.Cancel = true;
			}
      }

		private void BuildAutocompleteMenu(AutocompleteMenu popupMenu) {
			List<AutocompleteItem> items = new();

			foreach (var item in snippets)
				items.Add(new SnippetAutocompleteItem(item) { ImageIndex = 1 });
			foreach (var item in declarationSnippets)
				items.Add(new DeclarationSnippet(item) { ImageIndex = 0 });
			foreach (var item in methods)
				items.Add(new MethodAutocompleteItem(item) { ImageIndex = 2 });
			foreach (var item in keywords)
				items.Add(new AutocompleteItem(item));

			items.Add(new InsertSpaceSnippet());
			items.Add(new InsertSpaceSnippet(@"^(\w+)([=<>!:]+)(\w+)$"));
			items.Add(new InsertEnterSnippet());

			//set as autocomplete source
			popupMenu.Items.SetAutocompleteItems(items);
			popupMenu.SearchPattern = @"[\w\.:=!<>]";
		}

		void Tb_MouseMove(object sender, MouseEventArgs e) {
			var tb = sender as FastColoredTextBox;
			var place = tb.PointToPlace(e.Location);
			var r = new TextSelectionRange(tb, place, place);

			string text = r.GetFragment("[a-zA-Z]").Text;
			lbWordUnderMouse.Text = text;
		}

		void Tb_KeyDown(object sender, KeyEventArgs e) {
			if (e.Modifiers == Keys.Control && e.KeyCode == Keys.OemMinus) {
				NavigateBackward();
				e.Handled = true;
			}

			if (e.Modifiers == (Keys.Control | Keys.Shift) && e.KeyCode == Keys.OemMinus) {
				NavigateForward();
				e.Handled = true;
			}

			if (e.KeyData == (Keys.K | Keys.Control)) {
				//forced show (MinFragmentLength will be ignored)
				(CurrentTB.Tag as TbInfo).popupMenu.Show(true);
				e.Handled = true;
			}
		}

		void Tb_SelectionChangedDelayed(object sender, EventArgs e) {
			var tb = sender as FastColoredTextBox;
			//remember last visit time
			if (tb.Selection.IsEmpty && tb.Selection.Start.iLine < tb.LinesCount) {
				if (lastNavigatedDateTime != tb[tb.Selection.Start.iLine].LastVisit) {
					tb[tb.Selection.Start.iLine].LastVisit = DateTime.Now;
					lastNavigatedDateTime = tb[tb.Selection.Start.iLine].LastVisit;
				}
			}

			//highlight same words
			tb.VisibleRange.ClearStyle(sameWordsStyle);
			if (!tb.Selection.IsEmpty)
				return;//user selected diapason
					   //get fragment around caret
			var fragment = tb.Selection.GetFragment(@"\w");
			string text = fragment.Text;
			if (text.Length == 0)
				return;
			//highlight same words
			TextSelectionRange[] ranges = tb.VisibleRange.GetRanges("\\b" + text + "\\b").ToArray();

			if (ranges.Length > 1)
				foreach (var r in ranges)
					r.SetStyle(sameWordsStyle);
		}

		void Tb_TextChangedDelayed(object sender, TextChangedEventArgs e) {
			FastColoredTextBox tb = (sender as FastColoredTextBox);
			//rebuild object explorer
			string text = (sender as FastColoredTextBox).Text;
			ThreadPool.QueueUserWorkItem(
				(o) => ReBuildObjectExplorer(text)
			);

			//show invisible chars
			HighlightInvisibleChars(e.ChangedRange);
		}

		private void HighlightInvisibleChars(TextSelectionRange range) {
			range.ClearStyle(invisibleCharsStyle);
			if (btInvisibleChars.Checked)
				range.SetStyle(invisibleCharsStyle, @".$|.\r\n|\s");
		}

		List<ExplorerItem> explorerList = new();

		private void ReBuildObjectExplorer(string text) {
			try {
				List<ExplorerItem> list = new();
				int lastClassIndex = -1;
				//find classes, methods and properties
				Regex regex = new(@"^(?<range>[\w\s]+\b(class|struct|enum|interface)\s+[\w<>,\s]+)|^\s*(public|private|internal|protected)[^\n]+(\n?\s*{|;)?", RegexOptions.Multiline);
				foreach (Match r in regex.Matches(text))
					try {
						string s = r.Value;
						int i = s.IndexOfAny(new char[] { '=', '{', ';' });
						if (i >= 0)
							s = s[..i];
						s = s.Trim();

						var item = new ExplorerItem() { title = s, position = r.Index };
						if (Regex.IsMatch(item.title, @"\b(class|struct|enum|interface)\b")) {
							item.title = item.title[item.title.LastIndexOf(' ')..].Trim();
							item.type = ExplorerItemType.Class;
							list.Sort(lastClassIndex + 1, list.Count - (lastClassIndex + 1), new ExplorerItemComparer());
							lastClassIndex = list.Count;
						} else
							if (item.title.Contains(" event ")) {
							int ii = item.title.LastIndexOf(' ');
							item.title = item.title[ii..].Trim();
							item.type = ExplorerItemType.Event;
						} else
								if (item.title.Contains('(')) {
							var parts = item.title.Split('(');
							item.title = parts[0][parts[0].LastIndexOf(' ')..].Trim() + "(" + parts[1];
							item.type = ExplorerItemType.Method;
						} else
									if (item.title.EndsWith("]")) {
							var parts = item.title.Split('[');
							if (parts.Length < 2) continue;
							item.title = parts[0][parts[0].LastIndexOf(' ')..].Trim() + "[" + parts[1];
							item.type = ExplorerItemType.Method;
						} else {
							int ii = item.title.LastIndexOf(' ');
							item.title = item.title[ii..].Trim();
							item.type = ExplorerItemType.Property;
						}
						list.Add(item);
					} catch {; }

				list.Sort(lastClassIndex + 1, list.Count - (lastClassIndex + 1), new ExplorerItemComparer());

				BeginInvoke(
					new Action(() => {
						explorerList = list;
						dgvObjectExplorer.RowCount = explorerList.Count;
						dgvObjectExplorer.Invalidate();
					})
				);
			} catch {; }
		}

		enum ExplorerItemType {
			Class, Method, Property, Event
		}

		class ExplorerItem {
			public ExplorerItemType type;
			public string title;
			public int position;
		}

		class ExplorerItemComparer : IComparer<ExplorerItem> {
			public int Compare(ExplorerItem x, ExplorerItem y) {
				return x.title.CompareTo(y.title);
			}
		}

		private void TsFiles_TabStripItemClosing(TabStripItemClosingEventArgs e) {
			if ((e.Item.Controls[0] as FastColoredTextBox).IsChanged) {
				switch (MessageBox.Show("Do you want save " + e.Item.Title + " ?", "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information)) {
					case System.Windows.Forms.DialogResult.Yes:
						if (!Save(e.Item))
							e.Cancel = true;
						break;
					case DialogResult.Cancel:
						e.Cancel = true;
						break;
				}
			}
		}

		private bool Save(FATabStripItem tab) {
			var tb = (tab.Controls[0] as FastColoredTextBox);
			if (tab.Tag == null) {
				if (sfdMain.ShowDialog() != System.Windows.Forms.DialogResult.OK)
					return false;
				tab.Title = Path.GetFileName(sfdMain.FileName);
				tab.Tag = sfdMain.FileName;
			}

			try {
				File.WriteAllText(tab.Tag as string, tb.Text);
				tb.IsChanged = false;
			} catch (Exception ex) {
				if (MessageBox.Show(ex.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
					return Save(tab);
				else
					return false;
			}

			tb.Invalidate();

			return true;
		}

		private void SaveToolStripMenuItem_Click(object sender, EventArgs e) {
			if (tsFiles.SelectedItem != null)
				Save(tsFiles.SelectedItem);
		}

		private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e) {
			if (tsFiles.SelectedItem != null) {
				string oldFile = tsFiles.SelectedItem.Tag as string;
				tsFiles.SelectedItem.Tag = null;
				if (!Save(tsFiles.SelectedItem))
					if (oldFile != null) {
						tsFiles.SelectedItem.Tag = oldFile;
						tsFiles.SelectedItem.Title = Path.GetFileName(oldFile);
					}
			}
		}

		private void QuitToolStripMenuItem_Click(object sender, EventArgs e) => Close();

		private void OpenToolStripMenuItem_Click(object sender, EventArgs e) {
			if (ofdMain.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				CreateTab(ofdMain.FileName);
		}

		FastColoredTextBox CurrentTB {
			get {
				if (tsFiles.SelectedItem == null)
					return null;
				return (tsFiles.SelectedItem.Controls[0] as FastColoredTextBox);
			}

			set {
				tsFiles.SelectedItem = (value.Parent as FATabStripItem);
				value.Focus();
			}
		}

		private void CutToolStripMenuItem_Click(object sender, EventArgs e) => CurrentTB.Cut();
		private void CopyToolStripMenuItem_Click(object sender, EventArgs e) => CurrentTB.Copy();
		private void PasteToolStripMenuItem_Click(object sender, EventArgs e) => CurrentTB.Paste();
		private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e) => CurrentTB.Selection.SelectAll();

		private void UndoToolStripMenuItem_Click(object sender, EventArgs e) {
			if (CurrentTB.UndoEnabled)
				CurrentTB.Undo();
		}

		private void RedoToolStripMenuItem_Click(object sender, EventArgs e) {
			if (CurrentTB.RedoEnabled)
				CurrentTB.Redo();
		}

		private void TmUpdateInterface_Tick(object sender, EventArgs e) {
			try {
				if (CurrentTB != null && tsFiles.Items.Count > 0) {
					var tb = CurrentTB;
					undoStripButton.Enabled = undoToolStripMenuItem.Enabled = tb.UndoEnabled;
					redoStripButton.Enabled = redoToolStripMenuItem.Enabled = tb.RedoEnabled;
					saveToolStripButton.Enabled = saveToolStripMenuItem.Enabled = tb.IsChanged;
					saveAsToolStripMenuItem.Enabled = true;
					pasteToolStripButton.Enabled = pasteToolStripMenuItem.Enabled = true;
					cutToolStripButton.Enabled = cutToolStripMenuItem.Enabled =
					copyToolStripButton.Enabled = copyToolStripMenuItem.Enabled = !tb.Selection.IsEmpty;
					printToolStripButton.Enabled = true;
				} else {
					saveToolStripButton.Enabled = saveToolStripMenuItem.Enabled = false;
					saveAsToolStripMenuItem.Enabled = false;
					cutToolStripButton.Enabled = cutToolStripMenuItem.Enabled =
					copyToolStripButton.Enabled = copyToolStripMenuItem.Enabled = false;
					pasteToolStripButton.Enabled = pasteToolStripMenuItem.Enabled = false;
					printToolStripButton.Enabled = false;
					undoStripButton.Enabled = undoToolStripMenuItem.Enabled = false;
					redoStripButton.Enabled = redoToolStripMenuItem.Enabled = false;
					dgvObjectExplorer.RowCount = 0;
				}
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}

		private void PrintToolStripButton_Click(object sender, EventArgs e) {
			if (CurrentTB != null) {
				var settings = new PrintDialogSettings {
					Title = tsFiles.SelectedItem.Title,
					Header = "&b&w&b",
					Footer = "&b&p"
				};
				CurrentTB.Print(settings);
			}
		}

		bool tbFindChanged = false;

		private void TbFind_KeyPress(object sender, KeyPressEventArgs e) {
			if (e.KeyChar == '\r' && CurrentTB != null) {
				TextSelectionRange r = tbFindChanged ? CurrentTB.Range.Clone() : CurrentTB.Selection.Clone();
				tbFindChanged = false;
				r.End = new Place(CurrentTB[CurrentTB.LinesCount - 1].Count, CurrentTB.LinesCount - 1);
				var pattern = Regex.Escape(tbFind.Text);
				foreach (var found in r.GetRanges(pattern)) {
					found.Inverse();
					CurrentTB.Selection = found;
					CurrentTB.DoSelectionVisible();
					return;
				}
				MessageBox.Show("Not found.");
			} else
				tbFindChanged = true;
		}

		private void FindToolStripMenuItem_Click(object sender, EventArgs e) => CurrentTB.ShowFindDialog();
		private void ReplaceToolStripMenuItem_Click(object sender, EventArgs e) => CurrentTB.ShowReplaceDialog();

		private void PowerfulCSharpEditor_FormClosing(object sender, FormClosingEventArgs e) {
			List<FATabStripItem> list = new();
			foreach (FATabStripItem tab in tsFiles.Items)
				list.Add(tab);
			foreach (var tab in list) {
				TabStripItemClosingEventArgs args = new(tab);
				TsFiles_TabStripItemClosing(args);
				if (args.Cancel) {
					e.Cancel = true;
					return;
				}
				tsFiles.RemoveTab(tab);
			}
		}

		private void DgvObjectExplorer_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
			if (CurrentTB != null) {
				var item = explorerList[e.RowIndex];
				CurrentTB.GoEnd();
				CurrentTB.SelectionStart = item.position;
				CurrentTB.DoSelectionVisible();
				CurrentTB.Focus();
			}
		}

		private void DgvObjectExplorer_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
			try {
				ExplorerItem item = explorerList[e.RowIndex];
				if (e.ColumnIndex == 1)
					e.Value = item.title;
				else
					switch (item.type) {
						case ExplorerItemType.Class:
							e.Value = global::Tester.Properties.Resources.class_libraries;
							return;
						case ExplorerItemType.Method:
							e.Value = global::Tester.Properties.Resources.box;
							return;
						case ExplorerItemType.Event:
							e.Value = global::Tester.Properties.Resources.lightning;
							return;
						case ExplorerItemType.Property:
							e.Value = global::Tester.Properties.Resources.property;
							return;
					}
			} catch {; }
		}

		private void TsFiles_TabStripItemSelectionChanged(TabStripItemChangedEventArgs e) {
			if (CurrentTB != null) {
				CurrentTB.Focus();
				string text = CurrentTB.Text;
				ThreadPool.QueueUserWorkItem(
					(o) => ReBuildObjectExplorer(text)
				);
			}
		}

		private void BackStripButton_Click(object sender, EventArgs e) => NavigateBackward();
		private void ForwardStripButton_Click(object sender, EventArgs e) => NavigateForward();

		DateTime lastNavigatedDateTime = DateTime.Now;

		private bool NavigateBackward() {
			DateTime max = new();
			int iLine = -1;
			FastColoredTextBox tb = null;
			for (int iTab = 0; iTab < tsFiles.Items.Count; iTab++) {
				var t = (tsFiles.Items[iTab].Controls[0] as FastColoredTextBox);
				for (int i = 0; i < t.LinesCount; i++)
					if (t[i].LastVisit < lastNavigatedDateTime && t[i].LastVisit > max) {
						max = t[i].LastVisit;
						iLine = i;
						tb = t;
					}
			}
			if (iLine >= 0) {
				tsFiles.SelectedItem = (tb.Parent as FATabStripItem);
				tb.Navigate(iLine);
				lastNavigatedDateTime = tb[iLine].LastVisit;
				Console.WriteLine("Backward: " + lastNavigatedDateTime);
				tb.Focus();
				tb.Invalidate();
				return true;
			} else
				return false;
		}

		private bool NavigateForward() {
			DateTime min = DateTime.Now;
			int iLine = -1;
			FastColoredTextBox tb = null;
			for (int iTab = 0; iTab < tsFiles.Items.Count; iTab++) {
				var t = (tsFiles.Items[iTab].Controls[0] as FastColoredTextBox);
				for (int i = 0; i < t.LinesCount; i++)
					if (t[i].LastVisit > lastNavigatedDateTime && t[i].LastVisit < min) {
						min = t[i].LastVisit;
						iLine = i;
						tb = t;
					}
			}
			if (iLine >= 0) {
				tsFiles.SelectedItem = (tb.Parent as FATabStripItem);
				tb.Navigate(iLine);
				lastNavigatedDateTime = tb[iLine].LastVisit;
				Console.WriteLine("Forward: " + lastNavigatedDateTime);
				tb.Focus();
				tb.Invalidate();
				return true;
			} else
				return false;
		}

		/// <summary>
		/// This item appears when any part of snippet text is typed
		/// </summary>
		class DeclarationSnippet : SnippetAutocompleteItem {
			public DeclarationSnippet(string snippet)
				: base(snippet) {
			}

			public override CompareResult Compare(string fragmentText) {
				var pattern = Regex.Escape(fragmentText);
				if (Regex.IsMatch(Text, "\\b" + pattern, RegexOptions.IgnoreCase))
					return CompareResult.Visible;
				return CompareResult.Hidden;
			}
		}

		/// <summary>
		/// Divides numbers and words: "123AND456" -> "123 AND 456"
		/// Or "i=2" -> "i = 2"
		/// </summary>
		class InsertSpaceSnippet : AutocompleteItem {
			readonly string pattern;

			public InsertSpaceSnippet(string pattern)
				: base("") {
				this.pattern = pattern;
			}

			public InsertSpaceSnippet()
				: this(@"^(\d+)([a-zA-Z_]+)(\d*)$") {
			}

			public override CompareResult Compare(string fragmentText) {
				if (Regex.IsMatch(fragmentText, pattern)) {
					Text = InsertSpaces(fragmentText);
					if (Text != fragmentText)
						return CompareResult.Visible;
				}
				return CompareResult.Hidden;
			}

			public string InsertSpaces(string fragment) {
				var m = Regex.Match(fragment, pattern);
				if (m == null)
					return fragment;
				if (m.Groups[1].Value == "" && m.Groups[3].Value == "")
					return fragment;
				return (m.Groups[1].Value + " " + m.Groups[2].Value + " " + m.Groups[3].Value).Trim();
			}

			public override string ToolTipTitle {
				get {
					return Text;
				}
			}
		}

		/// <summary>
		/// Inerts line break after '}'
		/// </summary>
		class InsertEnterSnippet : AutocompleteItem {
			Place enterPlace = Place.Empty;

			public InsertEnterSnippet()
				: base("[Line break]") {
			}

			public override CompareResult Compare(string fragmentText) {
				var r = Parent.Fragment.Clone();
				while (r.Start.iChar > 0) {
					if (r.CharBeforeStart == '}') {
						enterPlace = r.Start;
						return CompareResult.Visible;
					}

					r.GoLeftThroughFolded();
				}

				return CompareResult.Hidden;
			}

			public override string GetTextForReplace() {
				//extend range
				TextSelectionRange r = Parent.Fragment;
				r.Start = enterPlace;
				r.End = r.End;
				//insert line break
				return Environment.NewLine + r.Text;
			}

			public override void OnSelected(AutocompleteMenu popupMenu, SelectedEventArgs e) {
				base.OnSelected(popupMenu, e);
				if (Parent.Fragment.tb.AutoIndent)
					Parent.Fragment.tb.DoAutoIndent();
			}

			public override string ToolTipTitle {
				get {
					return "Insert line break after '}'";
				}
			}
		}

		private void AutoIndentSelectedTextToolStripMenuItem_Click(object sender, EventArgs e) => CurrentTB.DoAutoIndent();

		private void BtInvisibleChars_Click(object sender, EventArgs e) {
			foreach (FATabStripItem tab in tsFiles.Items)
				HighlightInvisibleChars((tab.Controls[0] as FastColoredTextBox).Range);
			if (CurrentTB != null)
				CurrentTB.Invalidate();
		}

		private void BtHighlightCurrentLine_Click(object sender, EventArgs e) {
			foreach (FATabStripItem tab in tsFiles.Items) {
				if (btHighlightCurrentLine.Checked)
					(tab.Controls[0] as FastColoredTextBox).CurrentLineColor = currentLineColor;
				else
					(tab.Controls[0] as FastColoredTextBox).CurrentLineColor = Color.Transparent;
			}
			if (CurrentTB != null)
				CurrentTB.Invalidate();
		}

		private void CommentSelectedToolStripMenuItem_Click(object sender, EventArgs e) => CurrentTB.InsertLinePrefix("//");
		private void UncommentSelectedToolStripMenuItem_Click(object sender, EventArgs e) => CurrentTB.RemoveLinePrefix("//");

		private void CloneLinesToolStripMenuItem_Click(object sender, EventArgs e) {
			//expand selection
			CurrentTB.Selection.Expand();
			//get text of selected lines
			string text = Environment.NewLine + CurrentTB.Selection.Text;
			//move caret to end of selected lines
			CurrentTB.Selection.SetStartAndEnd(CurrentTB.Selection.End);
			//insert text
			CurrentTB.InsertText(text);
		}

		private void CloneLinesAndCommentToolStripMenuItem_Click(object sender, EventArgs e) {
			//start autoUndo block
			CurrentTB.BeginAutoUndo();
			//expand selection
			CurrentTB.Selection.Expand();
			//get text of selected lines
			string text = Environment.NewLine + CurrentTB.Selection.Text;
			//comment lines
			CurrentTB.InsertLinePrefix("//");
			//move caret to end of selected lines
			CurrentTB.Selection.SetStartAndEnd(CurrentTB.Selection.End);
			//insert text
			CurrentTB.InsertText(text);
			//end of autoUndo block
			CurrentTB.EndAutoUndo();
		}

		private void BookmarkPlusButton_Click(object sender, EventArgs e) {
			if (CurrentTB == null)
				return;
			CurrentTB.BookmarkLine(CurrentTB.Selection.Start.iLine);
		}

		private void BookmarkMinusButton_Click(object sender, EventArgs e) {
			if (CurrentTB == null)
				return;
			CurrentTB.UnbookmarkLine(CurrentTB.Selection.Start.iLine);
		}

		private void GotoButton_DropDownOpening(object sender, EventArgs e) {
			gotoButton.DropDownItems.Clear();
			foreach (Control tab in tsFiles.Items) {
				FastColoredTextBox tb = tab.Controls[0] as FastColoredTextBox;
				foreach (var bookmark in tb.Bookmarks) {
					var item = gotoButton.DropDownItems.Add(bookmark.Name + " [" + Path.GetFileNameWithoutExtension(tab.Tag as String) + "]");
					item.Tag = bookmark;
					item.Click += (o, a) => {
						var b = (Bookmark)(o as ToolStripItem).Tag;
						try {
							CurrentTB = b.TB;
						} catch (Exception ex) {
							MessageBox.Show(ex.Message);
							return;
						}
						b.DoVisible();
					};
				}
			}
		}

		private void BtShowFoldingLines_Click(object sender, EventArgs e) {
			foreach (FATabStripItem tab in tsFiles.Items)
				(tab.Controls[0] as FastColoredTextBox).ShowFoldingLines = btShowFoldingLines.Checked;
			if (CurrentTB != null)
				CurrentTB.Invalidate();
		}

		private void Zoom_click(object sender, EventArgs e) {
			if (CurrentTB != null)
				CurrentTB.Zoom = int.Parse((sender as ToolStripItem).Tag.ToString());
		}
	}

	public class InvisibleCharsRenderer : Style {
		readonly Pen pen;

		public InvisibleCharsRenderer(Pen pen) {
			this.pen = pen;
		}

		public override void Draw(Graphics gr, Point position, TextSelectionRange range) {
			var tb = range.tb;
			using Brush brush = new SolidBrush(pen.Color);
			foreach (var place in range) {
				switch (tb[place].C) {
					case ' ':
						var point = tb.PlaceToPoint(place);
						point.Offset(tb.CharWidth / 2, tb.CharHeight / 2);
						gr.DrawLine(pen, point.X, point.Y, point.X + 1, point.Y);
						break;
				}

				if (tb[place.iLine].Count - 1 == place.iChar) {
					var point = tb.PlaceToPoint(place);
					point.Offset(tb.CharWidth, 0);
					gr.DrawString("¶", tb.Font, brush, point);
				}
			}
		}
	}

	public class TbInfo {
		public AutocompleteMenu popupMenu;
	}
}
