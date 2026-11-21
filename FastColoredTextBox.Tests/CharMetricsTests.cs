using System;
using System.Drawing;
using System.Windows.Forms;
using FastColoredTextBoxNS.Types;
using Xunit;

namespace FastColoredTextBoxNS.Tests
{
    /// <summary>
    /// CharWidth / CharCnWidth / CharHeight are device-pixel values derived from the font on the
    /// control's own device context. They must be recomputed whenever the font changes (the host
    /// scales fonts per monitor), and the cached values must stay shared across instances.
    /// </summary>
    public class CharMetricsTests
    {
        [Fact]
        public void Metrics_follow_font_size_and_are_cached()
        {
            StaRunner.Run(() =>
            {
                using var box = new FastColoredTextBox { AllowProportionalFont = true, UseCJK = CJKMode.Hanzi };
                box.Font = new Font("Microsoft YaHei UI", 12F);
                int bigWidth = box.CharWidth, bigCjk = box.CharCnWidth, bigHeight = box.CharHeight;

                box.Font = new Font("Microsoft YaHei UI", 6F);
                Assert.True(box.CharWidth < bigWidth, $"CharWidth {box.CharWidth} 未随字号变小");
                Assert.True(box.CharCnWidth < bigCjk, $"CharCnWidth {box.CharCnWidth} 未随字号变小");
                Assert.True(box.CharHeight < bigHeight, $"CharHeight {box.CharHeight} 未随字号变小");

                box.Font = new Font("Microsoft YaHei UI", 12F);
                Assert.Equal(bigWidth, box.CharWidth);
                Assert.Equal(bigCjk, box.CharCnWidth);
                Assert.Equal(bigHeight, box.CharHeight);
            });
        }

        [Fact]
        public void CharCnWidth_matches_cjk_advance_of_the_same_context()
        {
            StaRunner.Run(() =>
            {
                using var box = new FastColoredTextBox { AllowProportionalFont = true, UseCJK = CJKMode.Hanzi };
                box.Font = new Font("Microsoft YaHei UI", 12F);

                // 探针字必须量与 CharCnWidth 同一个来源：换探针字（中 → 国）时这条会挡住不一致
                int expected = (int)Math.Round(FastColoredTextBox.GetCharSize(box.Font, '国').Width) - 1;
                Assert.Equal(expected, box.CharCnWidth);

                // Hanzi 模式下所有 CJK（含假名）都按同一个全角宽排
                Assert.Equal(box.CharCnWidth, box.GetCharWidth('国'));
                Assert.Equal(box.CharCnWidth, box.GetCharWidth('あ'));
                // 非 CJK 仍走 CharWidth
                Assert.Equal(box.CharWidth, box.GetCharWidth('M'));
            });
        }

        /// <summary>
        /// GDI 的 TextRenderer 默认把 '&amp;' 当助记符前缀：不画 &amp;，改为给后一个字符加下划线。
        /// 逐字测宽因此只得到约 1px，而绘制走 GDI+ 会画完整字形，于是 &amp; 会压到下一个字符上
        /// （现场：ChatFlow 思维链里的 <c>2&gt;&amp;1</c>、<c>&amp; $env:…</c> 叠在一起）。
        /// </summary>
        [Fact]
        public void Ampersand_advance_is_its_glyph_width()
        {
            StaRunner.Run(() =>
            {
                using var box = new FastColoredTextBox { AllowProportionalFont = true };
                box.Font = new Font("Microsoft YaHei UI", 12F);

                int Measure(string s) => TextRenderer.MeasureText(s, box.Font,
                    new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPrefix).Width;
                int expected = Measure("<&>") - Measure("<>");

                Assert.InRange(box.GetCharWidth('&'), expected - 2, expected + 1);
            });
        }

        [Fact]
        public void Proportional_cjk_width_follows_mode()
        {
            StaRunner.Run(() =>
            {
                using var box = new FastColoredTextBox { AllowProportionalFont = true, UseCJK = CJKMode.Hanzi };
                box.Font = new Font("Microsoft YaHei UI", 12F);

                // Hanzi：比例字体下所有 CJK 一律全角宽（启用静态宽的等价前提，也是换行/绘制共用的口径）
                foreach (char c in "的一是在不了有和人这中大为上个国我あアイウ　，。、가")
                    Assert.Equal(box.CharCnWidth, box.GetCharWidth(c));

                // CJK：逐字实测，结果等于该字在本上下文的步进（至少韩文/符号与全角宽不同时才会体现差异）
                box.UseCJK = CJKMode.CJK;
                foreach (char c in "国あ가★…")
                {
                    int measured = (int)Math.Round(FastColoredTextBox.GetCharSize(box.Font, c).Width) - 1;
                    Assert.Equal(measured, box.GetCharWidth(c));
                }
            });
        }
    }
}
