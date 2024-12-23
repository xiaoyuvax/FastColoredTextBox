namespace FastColoredTextBoxNS
{
    /// <summary>
    /// Calling MeasureText on each char is extremly slow
    /// Thankfully the size of the char does not change for the same font
    /// However we would still need to indentify the font somehow
    /// </summary>
    internal static class CharSizeCache
    {
        private static readonly Dictionary<string, SizeF> cache = [];

        internal static SizeF GetCharSize(Font font, char c)
        {
            var key = GetKey(font, c);
            if (!cache.TryGetValue(key, out SizeF value))
            {
                Size sz2 = TextRenderer.MeasureText("<" + c.ToString() + ">", font);
                Size sz3 = TextRenderer.MeasureText("<>", font);
                value = new SizeF(sz2.Width - sz3.Width + 1, /*sz2.Height*/font.Height);
                cache[key] = value;
            }
            return value;
        }

        /// <summary>
        /// Font is disposable, so we need to indentify it without keeping manged resources
        /// </summary>
        /// <param name="font"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private static string GetKey(Font font, char c)
        {
            return font.FontFamily.Name
                    + ":" + font.Size
                    + ":" + font.Style
                    + ":" + font.Unit
                    + ":" + font.GdiCharSet
                    + ":" + font.GdiVerticalFont
                    + ":" + c;
        }
    }
}