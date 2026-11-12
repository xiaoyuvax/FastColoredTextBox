using System;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using FastColoredTextBoxNS.Feature;
using FastColoredTextBoxNS.Types;
using Xunit;

namespace FastColoredTextBoxNS.Tests
{
    /// <summary>
    /// 折叠标记的绘制矩形只有 DefaultMarkerSize 见方，鼠标感知区域要比它大：
    /// 点在标记旁边/上下一点也要能命中（此前必须精确点进那个小方块）。
    /// </summary>
    public class FoldingMarkerHitTests
    {
        [Fact]
        public void ClickJustOutsideMarker_TogglesFolding()
        {
            StaRunner.Run(() =>
            {
                using var tb = NewBox();
                tb.Text = "head\nbody\nbody\nbody\ntail";
                tb[0].FoldingStartMarker = "chat-fold";
                tb[4].FoldingEndMarker = "chat-fold";

                Paint(tb);
                var marker = Marker<CollapseFoldingMarker>(tb);
                Assert.NotNull(marker);
                var r = marker.rectangle;

                // 绘制矩形上边缘外 1px、横向仍是标记中线：旧逻辑命不中，新感知区域要能命中
                Click(tb, new Point(r.Left + r.Width / 2, r.Top - 1));

                Assert.Equal(VisibleState.Hidden, tb.GetVisibleState(1));
            });
        }

        private static FastColoredTextBox NewBox() => new()
        {
            ShowScrollBars = false,
            WordWrap = false,
            ShowLineNumbers = false,
            Size = new Size(300, 200),
        };

        /// <summary>visibleMarkers 在 OnPaint 里重建：走真实绘制路径把标记表填出来。</summary>
        private static void Paint(FastColoredTextBox tb)
        {
            _ = tb.Handle;
            using var bmp = new Bitmap(tb.Width, tb.Height);
            using var g = Graphics.FromImage(bmp);
            var args = new PaintEventArgs(g, new Rectangle(0, 0, tb.Width, tb.Height));
            typeof(FastColoredTextBox)
                .GetMethod("OnPaint", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(tb, [args]);
        }

        private static T Marker<T>(FastColoredTextBox tb) where T : VisualMarker
        {
            var markers = (IEnumerable)typeof(FastColoredTextBox)
                .GetField("visibleMarkers", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(tb);
            foreach (VisualMarker m in markers)
                if (m is T match) return match;
            return null;
        }

        private static void Click(FastColoredTextBox tb, Point p)
        {
            var args = new MouseEventArgs(MouseButtons.Left, 1, p.X, p.Y, 0);
            typeof(FastColoredTextBox)
                .GetMethod("OnMouseDown", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(tb, [args]);
        }
    }
}
