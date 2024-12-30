using FastColoredTextBoxNS;
using FastColoredTextBoxNS.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Tester {
	public partial class GifImageDrawingSample : Form {
		const string REGEX_SPEC_SYMBOLS_PATTERN = @"[\^\$\[\]\(\)\.\\\*\+\|\?\{\}]";
		readonly GifImageStyle style;

		public GifImageDrawingSample() {
			InitializeComponent();

			style = new GifImageStyle(fctb);
			style.ImagesByText.Add(@":bb", Properties.Resources.bye);
			style.ImagesByText.Add(@":D", Properties.Resources.lol);
			style.ImagesByText.Add(@"8)", Properties.Resources.rolleyes);
			style.ImagesByText.Add(@":@", Properties.Resources.unsure);
			style.ImagesByText.Add(@":)", Properties.Resources.smile_16x16);
			style.ImagesByText.Add(@":(", Properties.Resources.sad_16x16);

			style.StartAnimation();

			fctb.OnTextChanged();
		}

		private void Fctb_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e) {
			if (style == null) return;
			e.ChangedRange.ClearAllStyles();
			foreach (var key in style.ImagesByText.Keys) {
				string pattern = Regex.Replace(key, REGEX_SPEC_SYMBOLS_PATTERN, "\\$0");
				e.ChangedRange.SetStyle(style, pattern);
			}
		}
	}

	/// <summary>
	/// This class is used as text renderer for smiles
	/// </summary>
	class GifImageStyle : TextStyle {
		public Dictionary<string, Image> ImagesByText { get; private set; }

		readonly Timer timer;

		public GifImageStyle(FastColoredTextBox parent)
			: base(null, null, FontStyle.Regular) {
			ImagesByText = new Dictionary<string, Image>();

			//create timer
			timer = new System.Windows.Forms.Timer {
				Interval = 100
			};
			timer.Tick += (EventHandler)delegate {
				ImageAnimator.UpdateFrames();
				parent.Invalidate();
			};
			timer.Start();
		}

		public void StartAnimation() {
			foreach (var image in ImagesByText.Values)
				if (ImageAnimator.CanAnimate(image))
					ImageAnimator.Animate(image, new EventHandler(OnFrameChanged));
		}

		void OnFrameChanged(object sender, EventArgs args) {
		}

		public override void Draw(Graphics gr, Point position, TextSelectionRange range) {
			string text = range.Text;
			int iChar = range.Start.iChar;

			while (text != "") {
				bool replaced = false;
				foreach (var pair in ImagesByText) {
					if (text.StartsWith(pair.Key)) {
						float k = (float)(pair.Key.Length * range.tb.CharWidth) / pair.Value.Width;
						if (k > 1)
							k = 1f;
						//
						text = text[pair.Key.Length..];
						RectangleF rect = new(position.X + range.tb.CharWidth * pair.Key.Length / 2 - pair.Value.Width * k / 2, position.Y, pair.Value.Width * k, pair.Value.Height * k);
						gr.DrawImage(pair.Value, rect);
						position.Offset(range.tb.CharWidth * pair.Key.Length, 0);
						replaced = true;
						iChar += pair.Key.Length;
						break;
					}
				}
				if (!replaced && text.Length > 0) {
					TextSelectionRange r = new(range.tb, iChar, range.Start.iLine, iChar + 1, range.Start.iLine);
					base.Draw(gr, position, r);
					position.Offset(range.tb.CharWidth, 0);
					text = text[1..];
				}
			}
		}
	}
}
