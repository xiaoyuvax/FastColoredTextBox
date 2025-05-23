﻿using FastColoredTextBoxNS.Types;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tester {
	public partial class JokeSample : Form {
		public JokeSample() {
			InitializeComponent();
			fctb.DefaultStyle = new JokeStyle();
		}

		private void Timer1_Tick(object sender, EventArgs e) {
			fctb.Invalidate();
		}
	}

	/// <summary>
	/// This class is used as text renderer
	/// </summary>
	class JokeStyle : TextStyle {
		public JokeStyle() : base(null, null, FontStyle.Regular) {
		}

		public override void Draw(Graphics gr, Point position, TextSelectionRange range) {
			foreach (Place p in range) {
				int time = (int)(DateTime.Now.TimeOfDay.TotalMilliseconds / 2);
				int angle = (int)(time % 360L);
				int angle2 = (int)((time - (p.iChar - range.Start.iChar) * 20) % 360L) * 2;
				int x = position.X + (p.iChar - range.Start.iChar) * range.tb.CharWidth;
				TextSelectionRange r = range.tb.GetRange(p, new Place(p.iChar + 1, p.iLine));
				Point point = new(x, position.Y + (int)(5 + 5 * Math.Sin(Math.PI * angle2 / 180)));
				gr.ResetTransform();
				gr.TranslateTransform(point.X + range.tb.CharWidth / 2, point.Y + range.tb.CharHeight / 2);
				gr.RotateTransform(angle);
				gr.ScaleTransform(0.8f, 0.8f);
				gr.TranslateTransform(-range.tb.CharWidth / 2, -range.tb.CharHeight / 2);
				base.Draw(gr, new Point(0, 0), r);
			}
			gr.ResetTransform();
		}
	}
}
