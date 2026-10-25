using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using FastColoredTextBoxNS.Types;
using Xunit;

namespace FastColoredTextBoxNS.Tests
{
    /// <summary>
    /// A style run is drawn char by char, each at the origin accumulated from
    /// <see cref="FastColoredTextBox.GetCharWidth"/>. That layout must land the glyphs where a
    /// normal single-call text layout puts them: any per-char origin shift accumulates over the
    /// run and, with a background brush, the shifted glyphs get painted over by the next char.
    /// </summary>
    public class ProportionalFontRenderingTests
    {
        private static (int left, int right) InkColumns(Bitmap bmp)
        {
            int left = int.MaxValue, right = -1;
            for (int y = 0; y < bmp.Height; y++)
                for (int x = 0; x < bmp.Width; x++)
                {
                    var c = bmp.GetPixel(x, y);
                    if (c.R < 140 || c.G < 140 || c.B < 140)
                    {
                        if (x < left) left = x;
                        if (x > right) right = x;
                    }
                }
            return (left, right);
        }

        [Fact]
        public void StyledRun_IsLaidOutWhereNormalTextRenderingPutsIt()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox
                {
                    ShowScrollBars = false,
                    WordWrap = false,
                    ShowLineNumbers = false,
                    TextAreaBorder = TextAreaBorderType.None,
                    Paddings = new Padding(2),
                    LeftPadding = 12,
                    UseCJK = CJKMode.Hanzi,
                    TextRenderingHint = TextRenderingHint.AntiAliasGridFit,
                    AllowProportionalFont = true,
                    Font = new Font("Microsoft YaHei UI", 9F),
                };
                _ = tb.Handle;

                //narrow and wide glyphs mixed, like an inline-code span in a message
                const string text = "code-review.md Path.Combine";
                const int originX = 20;

                tb.Text = text;
                var styled = new Bitmap(originX + 400, tb.CharHeight + 40);
                using (var g = Graphics.FromImage(styled))
                {
                    g.Clear(Color.White);
                    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                    using var style = new TextStyle(Brushes.Black, Brushes.Gainsboro, FontStyle.Regular);
                    style.Draw(g, new Point(originX, 4), new TextSelectionRange(tb, 0, 0, text.Length, 0));
                }

                var plain = new Bitmap(styled.Width, styled.Height);
                using (var g = Graphics.FromImage(plain))
                {
                    g.Clear(Color.White);
                    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                    using var sf = (StringFormat)StringFormat.GenericTypographic.Clone();
                    using var f = new Font(tb.Font, FontStyle.Regular);
                    g.DrawString(text, f, Brushes.Black, originX, 4, sf);
                }

                var (styledLeft, styledRight) = InkColumns(styled);
                var (plainLeft, plainRight) = InkColumns(plain);

                Console.WriteLine($"styled=[{styledLeft},{styledRight}] plain=[{plainLeft},{plainRight}] dpi={tb.DeviceDpi} cell={(int)tb.CharWidth}");
                Assert.InRange(styledLeft, plainLeft - 2, plainLeft + 2);
                Assert.InRange(styledRight, plainRight - 2, plainRight + 2);
            });
        }
    }
}
