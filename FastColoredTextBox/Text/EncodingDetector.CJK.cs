namespace FastColoredTextBoxNS.Text
{
    public static partial class EncodingDetector
    {
        /// <summary>
        /// Determines whether the char belongs to a CJK (Chinese/Japanese/Korean) or similar
        /// full-width block whose glyphs are typically double-width, affecting wrapping and measuring.
        /// </summary>
        public static bool IsCJK(char code)
        {
            return code switch
            {
                var c when c >= 0x1100 && c <= 0x11FF => true,      // Hangul Jamo
                var c when c >= 0x2600 && c <= 0x26FF => true,      // Miscellaneous Symbols
                var c when c >= 0x2700 && c <= 0x27BF => true,      // Dingbats
                var c when c >= 0x2800 && c <= 0x28FF => true,      // Braille Patterns
                var c when c >= 0x2E80 && c <= 0x2EFF => true,      // CJK Radicals Supplement
                var c when c >= 0x2F00 && c <= 0x2FDF => true,      // Kangxi Radicals
                var c when c >= 0x2FF0 && c <= 0x2FFF => true,      // Ideographic Description Characters
                var c when c >= 0x3000 && c <= 0x303F => true,      // CJK Symbols and Punctuation
                var c when c >= 0x3040 && c <= 0x309F => true,      // Hiragana
                var c when c >= 0x30A0 && c <= 0x30FF => true,      // Katakana
                var c when c >= 0x3100 && c <= 0x312F => true,      // Bopomofo
                var c when c >= 0x3130 && c <= 0x318F => true,      // Hangul Compatibility Jamo
                var c when c >= 0x31A0 && c <= 0x31BF => true,      // Bopomofo Extended
                var c when c >= 0x31C0 && c <= 0x31EF => true,      // CJK Strokes
                var c when c >= 0x31F0 && c <= 0x31FF => true,      // Katakana Phonetic Extensions
                var c when c >= 0x3200 && c <= 0x32FF => true,      // Enclosed CJK Letters and Months
                var c when c >= 0x3300 && c <= 0x33FF => true,      // CJK Compatibility
                var c when c >= 0x3400 && c <= 0x4DB5 => true,      // CJK Unified Ideographs Extension A
                var c when c >= 0x4DC0 && c <= 0x4DFF => true,      // Hexagram Symbols
                var c when c >= 0x4E00 && c <= 0x9FA5 => true,      // CJK Unified Ideographs
                var c when c >= 0x9FA6 && c <= 0x9FBB => true,      // CJK Unified Ideographs
                var c when c >= 0xA000 && c <= 0xA48F => true,      // Yi Syllables
                var c when c >= 0xA490 && c <= 0xA4CF => true,      // Yi Radicals
                var c when c >= 0xAC00 && c <= 0xD7AF => true,      // Hangul Syllables
                var c when c >= 0xF900 && c <= 0xFA2D => true,      // CJK Compatibility Ideographs
                var c when c >= 0xFA30 && c <= 0xFA6A => true,      // CJK Compatibility Ideographs
                var c when c >= 0xFA70 && c <= 0xFAD9 => true,      // CJK Compatibility Ideographs
                var c when c >= 0xFE10 && c <= 0xFE1F => true,      // Vertical Forms
                var c when c >= 0xFE30 && c <= 0xFE4F => true,      // CJK Compatibility Forms
                var c when c >= 0xFF00 && c <= 0xFFEF => true,      // Fullwidth ASCII, Fullwidth English, Halfwidth Katakana, Halfwidth Hiragana, Halfwidth Hangul
                //var c when c >= 0x1D300 && c <= 0x1D35F => true,    // Tai Xuan Jing Symbols
                //var c when c >= 0x20000 && c <= 0x2A6D6 => true,    // CJK Unified Ideographs Extension B
                //var c when c >= 0x2F800 && c <= 0x2FA1D => true,    // CJK Compatibility Supplement
                _ => false,
            };
        }
    }
}
