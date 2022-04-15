using FastColoredTextBoxNS;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Tester {
	public partial class CustomStyleSample : Form {
		//create my custom style
		readonly EllipseStyle ellipseStyle = new EllipseStyle();

		public CustomStyleSample() {
			InitializeComponent();
		}

		private void Fctb_TextChanged(object sender, TextChangedEventArgs e) {
			//clear old styles of chars
			e.ChangedRange.ClearStyle(ellipseStyle);
			//append style for word 'Babylon'
			e.ChangedRange.SetStyle(ellipseStyle, @"\bBabylon\b", RegexOptions.IgnoreCase);
		}
	}

	/// <summary>
	/// This style will drawing ellipse around of the word
	/// </summary>
	class EllipseStyle : Style {
		public override void Draw(Graphics gr, Point position, Range range) {
			//get size of rectangle
			Size size = GetSizeOfRange(range);
			//create rectangle
			Rectangle rect = new Rectangle(position, size);
			//inflate it
			rect.Inflate(2, 2);
			//get rounded rectangle
			var path = GetRoundedRectangle(rect, 7);
			//draw rounded rectangle
			gr.DrawPath(Pens.Red, path);
		}
	}
}
