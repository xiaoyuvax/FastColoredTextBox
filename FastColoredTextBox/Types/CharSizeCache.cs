namespace FastColoredTextBoxNS.Types
{
    /// <summary>
    /// Calling MeasureText on each char is extremly slow
    /// Thankfully the size of the char does not change for the same font
    /// However we would still need to indentify the font somehow
    ///
    /// 度量是「某个字体在某个 DPI 下」的属性：同一字体在 96 / 144 / 240 DPI 下的像素宽高不同，
    /// 所以键里必须带 DPI，否则跨屏或跨 DPI 上下文会读到另一个量纲的值。
    /// 缓存保持静态：所有实例共享，占用量只跟「字体 × DPI × 字符」的组合数有关。
    /// </summary>
    internal static class CharSizeCache
    {
        // 结构化键：每字符一次测量要查一次缓存，字符串拼接（每个字符一次分配 + 一次哈希）
        // 曾占掉大半开销；record struct 直接参与哈希，无分配。
        private readonly record struct CharKey(
            string Family,
            float Size,
            FontStyle Style,
            GraphicsUnit Unit,
            byte CharSet,
            bool Vertical,
            char Char,
            int Dpi);

        private static readonly Dictionary<CharKey, SizeF> cache = [];

        internal static void Clear()
        {
            lock (cache) cache.Clear();
        }

        /// <summary>
        /// 用 <paramref name="dc"/>（控件所在显示器的设备上下文）实测字符的步进宽度与行高，
        /// 结果按「字体 + 字符 + 测量时的 DPI」缓存。
        /// </summary>
        internal static SizeF GetCharSize(IDeviceContext dc, Font font, char c, int dpi)
        {
            var key = new CharKey(font.FontFamily.Name, font.Size, font.Style, font.Unit,
                                  font.GdiCharSet, font.GdiVerticalFont, c, dpi);
            // 非线程安全 Dictionary：并发（如并行测试、后台测量）同时写入会抛
            // "Operations that change non-concurrent collections..." 并损坏内部状态。
            lock (cache)
            {
                if (cache.TryGetValue(key, out SizeF value)) return value;
            }

            // 测量放在锁外：一次 MeasureText 可能几百微秒，别让别的线程排队等
            Size sz2 = TextRenderer.MeasureText(dc, $"<{c}>", font);
            Size sz3 = TextRenderer.MeasureText(dc, "<>", font);
            // 行高也取自同一 DC：Font.Height 用的是主屏 DPI，副屏上会偏小/偏大
            var measured = new SizeF(sz2.Width - sz3.Width + 1, sz2.Height);
            lock (cache) cache[key] = measured;
            return measured;
        }

        /// <summary>没有设备上下文时（兼容旧调用）：用屏幕 DC，并按它的 DPI 记键。</summary>
        internal static SizeF GetCharSize(Font font, char c)
        {
            using var g = Graphics.FromHwnd(IntPtr.Zero);
            return GetCharSize(g, font, c, (int)Math.Round(g.DpiX));
        }
    }
}
