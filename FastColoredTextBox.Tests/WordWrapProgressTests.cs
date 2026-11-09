using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Xunit;

namespace FastColoredTextBoxNS.Tests
{
    /// <summary>
    /// Word wrap must always advance. A continuation width smaller than one glyph (large
    /// <see cref="FastColoredTextBox.WordWrapIndent"/>, narrow row, wide CJK glyph) used to make
    /// <see cref="FastColoredTextBox.CalcCutOffsByMaxWidth"/> cut at the same offset forever,
    /// growing CutOffPositions until the heap (and the UI thread) died.
    /// </summary>
    public class WordWrapProgressTests
    {
        [Fact]
        public void Wrap_of_glyphs_wider_than_the_continuation_width_terminates()
        {
            Exception failure = null;
            List<int> cuts = null;
            var thread = new Thread(() =>
            {
                try
                {
                    using var form = new Form { Width = 320, Height = 240 };
                    var tb = new FastColoredTextBox
                    {
                        Dock = DockStyle.Fill,
                        AllowProportionalFont = true,
                        UseCJK = CJKMode.Hanzi,
                        Font = new Font("Microsoft YaHei UI", 9f),
                        WordWrap = true,
                        // 续行缩进远超可用宽度 → 续行宽度退化成 CharWidth，比任何汉字都窄
                        WordWrapIndent = 5000
                    };
                    form.Controls.Add(tb);
                    form.Show();
                    Application.DoEvents();   // 先完成布局，WordWrap 的宽度才是真实值

                    tb.Text = new string('中', 200);
                    Application.DoEvents();
                    cuts = tb.LineInfos[0].CutOffPositions.ToList();
                }
                catch (Exception ex)
                {
                    failure = ex;
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();

            Assert.True(thread.Join(TimeSpan.FromSeconds(15)), "word wrap did not finish: the wrap loop is not advancing");
            if (failure != null) throw failure;

            Assert.NotEmpty(cuts);
            for (int i = 1; i < cuts.Count; i++)
                Assert.True(cuts[i] > cuts[i - 1], $"cut positions must strictly increase: {string.Join(",", cuts)}");
        }
    }
}
