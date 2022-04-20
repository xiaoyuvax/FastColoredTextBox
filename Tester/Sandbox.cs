using FastColoredTextBoxNS;
using FastColoredTextBoxNS.Types;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tester {
	public partial class Sandbox : Form {
		public Sandbox() => InitializeComponent();
		private void Button1_Click(object sender, EventArgs e) => fctb.SelectedText = "new line value";
		private readonly ColorStyle colorStyle = new(Brushes.Black, Brushes.White, FontStyle.Regular);
		private void Fctb_TextChanged(object sender, TextChangedEventArgs e) => e.ChangedRange.SetStyle(colorStyle, @"Color\.\w+");
	}

	class ColorStyle : TextStyle {
		public ColorStyle(Brush foreBrush, Brush backgroundBrush, FontStyle fontStyle) : base(foreBrush, backgroundBrush, fontStyle) { }

		public override void Draw(Graphics gr, Point position, TextSelectionRange range) {
			//get color name
			var parts = range.Text.Split('.');
			var colorName = parts[^1];
			var color = Color.FromName(colorName);
			(BackgroundBrush as SolidBrush).Color = color;
			base.Draw(gr, position, range);
		}
	}

	class MyFCTB : FastColoredTextBox {
		protected override void OnMouseMove(MouseEventArgs e) {
			//to prevent drag&drop inside FCTB
			typeof(FastColoredTextBox).GetField("mouseIsDragDrop", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(this, false);
			base.OnMouseMove(e);
		}
	}
}