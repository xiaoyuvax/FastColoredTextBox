using FastColoredTextBoxNS.Types;
using System.Text.RegularExpressions;

namespace FastColoredTextBoxNS.Text
{
    public class SyntaxDescriptor : IDisposable
    {
        public char leftBracket = '(';
        public char rightBracket = ')';
        public char leftBracket2 = '{';
        public char rightBracket2 = '}';
        public char leftBracket3 = '[';
        public char rightBracket3 = ']';
        public BracketsHighlightStrategy bracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;
        public readonly List<Style> styles = [];
        public readonly List<RuleDesc> rules = [];
        public readonly List<FoldingDesc> foldings = [];

        public void Dispose()
        {
            foreach (var style in styles)
                style.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    public class RuleDesc
    {
        private Regex regex;
        public string pattern;
        public RegexOptions options = RegexOptions.None;
        public Style style;

        public Regex Regex
        {
            get
            {
                regex ??= new Regex(pattern, SyntaxHighlighter.RegexCompiledOption | options);
                return regex;
            }
        }
    }

    public class FoldingDesc
    {
        public string startMarkerRegex;
        public string finishMarkerRegex;
        public RegexOptions options = RegexOptions.None;
    }
}