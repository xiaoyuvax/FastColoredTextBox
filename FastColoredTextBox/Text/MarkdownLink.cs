using System.Text.RegularExpressions;

namespace FastColoredTextBoxNS.Text
{
    /// <summary>
    /// What a clickable markdown region points at: a web address or a local path.
    /// </summary>
    public enum MarkdownLinkKind
    {
        Url,
        LocalFile,
    }

    /// <summary>
    /// A clickable region of one line: <see cref="Start"/> and <see cref="Length"/> are char offsets
    /// inside the text of <see cref="Line"/>, <see cref="Target"/> is where a click should go.
    /// </summary>
    /// <remarks>
    /// For <c>[label](url)</c> the region covers the label, not the url: the label is what the
    /// reader clicks. A hidden marker keeps its char index, so a label whose delimiters the
    /// highlighter hides is still hit at the offsets recorded here.
    /// </remarks>
    public readonly record struct MarkdownLink(
        int Line, int Start, int Length, MarkdownLinkKind Kind, string Target, bool FromSyntax);

    /// <summary>
    /// Finds the clickable regions of a markdown document: <c>[label](target)</c>,
    /// <c>![alt](src)</c>, <c>&lt;url&gt;</c>, bare http(s) addresses and local file paths.
    /// </summary>
    /// <remarks>
    /// Line oriented, and a fenced code block is skipped whole: its content is program text, so
    /// "http://x" inside a fence is not a link and "**x**" there is not emphasis. Local paths start
    /// at a drive letter or a UNC prefix, which keeps ordinary prose out of the link set.
    /// </remarks>
    public static class MarkdownLinkScanner
    {
        private static readonly Regex FenceLine = new(@"^ {0,3}(?:`{3,}|~{3,})", RegexOptions.Compiled);
        // ![alt](src) is matched before [label](target): the "!" belongs to the image
        private static readonly Regex InlineImage = new(@"!\[(?<text>(?:\\.|[^\]\\\n])*)\]\((?<url>[^)\n]*)\)", RegexOptions.Compiled);
        private static readonly Regex InlineLink = new(@"(?<!!)\[(?<text>(?:\\.|[^\]\\\n])+)\]\((?<url>[^)\n]*)\)", RegexOptions.Compiled);
        private static readonly Regex AutoLink = new(@"<(?<url>(?:[a-z][a-z0-9+.-]*://|mailto:)[^>\s]+)>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // A url runs until a char that cannot appear in one; CJK punctuation ends it too.
        // The start guard is not \b: .NET \w counts CJK, so "见https://x" would not match at all.
        private static readonly Regex UrlRegex = new(
            @"(?<![A-Za-z0-9_])https?://[^\s<>""'`{}|\\^（）()【】「」『』《》〈〉，。；：、！？…]+",
            RegexOptions.Compiled);

        // Path chars: no separator chars, no CJK (a Chinese word right after a path is prose, not
        // part of it) and no full-width punctuation.
        private const string PathChunk = @"[^\s<>|:*?""`(),;\r\n\u3000-\u303F\u3400-\u4DBF\u4E00-\u9FFF\uFF00-\uFFEF]+";
        private const string PathParen = @"\([^()\r\n]*\)(?=[\\/])";
        private const string PathTail = @"(?:" + PathChunk + "|" + PathParen + @")(?: ?(?:" + PathChunk + "|" + PathParen + "))*";
        private static readonly Regex PathRegex = new(
            @"(?<![A-Za-z0-9])(?:[A-Za-z]:[\\/]|\\\\)" + PathTail,
            RegexOptions.Compiled);

        // A path that ends in a real file name may carry CJK (J:/人物志/图.png). The ".ext" tail
        // bounds it, so a Chinese word glued to the path (J:/x.png已保存) still stops at the
        // extension instead of swallowing the sentence.
        private const string FileTailChunk = @"[^\s<>|:*?""`(),;\r\n]+";
        private const string FileTail = FileTailChunk +
            @"(?:" + PathParen + FileTailChunk + @"| (?:" + PathParen + @")?" + FileTailChunk + @")*" +
            @"\.[A-Za-z0-9]{1,8}(?![A-Za-z0-9])";
        private static readonly Regex FilePathRegex = new(
            @"(?<![A-Za-z0-9])(?:[A-Za-z]:[\\/]|\\\\)" + FileTail,
            RegexOptions.Compiled);

        // A path inside quotes or brackets: the fence already gives the boundary, so spaces and CJK
        // are allowed inside; the fence chars themselves are not part of the path.
        private const string PathFenceOpen = @"[""'\`（(【「『《〈{“‘<]";
        private const string PathFenceClose = @"[""'\`）)】」』》〉}”’>]";
        private static readonly Regex FencedPathRegex = new(
            PathFenceOpen + @"(?<p>(?:[A-Za-z]:[\\/]|\\\\)[^""'\`）)】」』》〉}”’<>\r\n]+?)" + PathFenceClose,
            RegexOptions.Compiled);

        /// <summary>
        /// Links of the whole document, in reading order. A fenced block is skipped, the fence
        /// line itself carries no link.
        /// </summary>
        public static List<MarkdownLink> Collect(string text)
        {
            var links = new List<MarkdownLink>();
            if (string.IsNullOrEmpty(text))
                return links;

            string s = text.IndexOf('\r') < 0 ? text : text.Replace("\r\n", "\n");
            int line = 0;
            bool inFence = false;
            foreach (string raw in s.Split('\n'))
            {
                if (FenceLine.IsMatch(raw))
                    inFence = !inFence;
                else if (!inFence)
                    links.AddRange(ScanLine(raw, line));
                line++;
            }
            return links;
        }

        /// <summary>
        /// Links of one line of prose. <paramref name="lineIndex"/> is only stamped into the
        /// results; the caller decides whether the line is inside a fence.
        /// </summary>
        public static List<MarkdownLink> ScanLine(string line, int lineIndex)
        {
            var links = new List<MarkdownLink>();
            if (string.IsNullOrEmpty(line))
                return links;

            void Add(int start, int length, MarkdownLinkKind kind, string target, bool fromSyntax)
            {
                if (length <= 0 || start < 0 || start + length > line.Length)
                    return;
                foreach (var l in links)
                    if (start < l.Start + l.Length && l.Start < start + length)
                        return;
                links.Add(new MarkdownLink(lineIndex, start, length, kind, target, fromSyntax));
            }

            foreach (Match m in InlineImage.Matches(line))
            {
                var text = m.Groups["text"];
                string url = m.Groups["url"].Value.Trim();
                Add(text.Index, text.Length, KindOf(url), url, true);
            }

            foreach (Match m in InlineLink.Matches(line))
            {
                var text = m.Groups["text"];
                string url = m.Groups["url"].Value.Trim();
                Add(text.Index, text.Length, KindOf(url), url, true);
            }

            foreach (Match m in AutoLink.Matches(line))
            {
                var url = m.Groups["url"];
                Add(url.Index, url.Length, KindOf(url.Value), url.Value, true);
            }

            foreach (Match m in UrlRegex.Matches(line))
            {
                string value = m.Value;
                int len = TrimUrlEnd(value);
                if (len == 0 || IsPlaceholder(value.Substring(0, len)))
                    continue;
                // "http://x/y…" is an address the UI cut for display: clicking it cannot work
                if (m.Index + len < line.Length && line[m.Index + len] == '\u2026')
                    continue;
                Add(m.Index, len, MarkdownLinkKind.Url, value.Substring(0, len), false);
            }

            foreach (Match m in FencedPathRegex.Matches(line))
            {
                var p = m.Groups["p"];
                int end = TrimEndSpaces(line, p.Index, p.Index + p.Length);
                Add(p.Index, end - p.Index, MarkdownLinkKind.LocalFile, line.Substring(p.Index, end - p.Index), false);
            }

            foreach (Match m in FilePathRegex.Matches(line))
            {
                int end = TrimPathEnd(line, m.Index, m.Index + m.Length);
                Add(m.Index, end - m.Index, MarkdownLinkKind.LocalFile, line.Substring(m.Index, end - m.Index), false);
            }

            foreach (Match m in PathRegex.Matches(line))
            {
                int end = TrimPathEnd(line, m.Index, m.Index + m.Length);
                Add(m.Index, end - m.Index, MarkdownLinkKind.LocalFile, line.Substring(m.Index, end - m.Index), false);
            }

            links.Sort((a, b) => a.Start.CompareTo(b.Start));
            return links;
        }

        private static MarkdownLinkKind KindOf(string target)
        {
            if (target.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || target.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                || target.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase))
                return MarkdownLinkKind.Url;
            return MarkdownLinkKind.LocalFile;
        }

        /// <summary>
        /// Length of the url without the punctuation that follows it. Trailing <c>*</c> and
        /// <c>`</c> are markdown markers around the url, a trailing <c>]</c> is a bracket around
        /// it, and sentence punctuation is never part of the address.
        /// </summary>
        private static int TrimUrlEnd(string value)
        {
            int len = value.Length;
            bool bracketed = value.IndexOf('[') >= 0;   // an IPv6 literal ends with "]", it belongs
            while (len > 0)
            {
                char tail = value[len - 1];
                if (".,;:!?'\"*".IndexOf(tail) >= 0 || (tail == ']' && !bracketed))
                {
                    len--;
                    continue;
                }
                break;
            }
            return len;
        }

        /// <summary>Only a protocol or dots left (a <c>https://...</c> placeholder): no link.</summary>
        private static bool IsPlaceholder(string url)
        {
            int sep = url.IndexOf("://", StringComparison.Ordinal);
            if (sep < 0)
                return false;
            for (int i = sep + 3; i < url.Length; i++)
                if (url[i] != '.')
                    return false;
            return true;
        }

        private static int TrimEndSpaces(string s, int start, int end)
        {
            while (end > start && char.IsWhiteSpace(s[end - 1]))
                end--;
            return end;
        }

        /// <summary>
        /// Trailing words are not part of a path: while the last space-separated segment has no
        /// separator and no ".ext" tail, drop it (<c>C:\x\App status --short</c> -> <c>C:\x\App</c>).
        /// A space between two segments of the path stays (<c>C:\Program Files\App\x.exe</c>).
        /// </summary>
        private static int TrimPathEnd(string s, int start, int end)
        {
            end = TrimEndSpaces(s, start, end);
            while (end > start)
            {
                int sp = s.LastIndexOf(' ', end - 1, end - start);
                if (sp < start)
                    break;
                var tail = s.AsSpan(sp + 1, end - sp - 1);
                if (tail.IndexOfAny('\\', '/') >= 0 || LooksLikeFileTail(tail))
                    break;
                end = TrimEndSpaces(s, start, sp);
            }
            return end;
        }

        private static bool LooksLikeFileTail(ReadOnlySpan<char> tail)
        {
            int dot = tail.LastIndexOf('.');
            if (dot <= 0 || tail.Length - dot - 1 is < 1 or > 8)
                return false;
            foreach (char c in tail[(dot + 1)..])
                if (!char.IsAsciiLetterOrDigit(c))
                    return false;
            return true;
        }
    }
}
