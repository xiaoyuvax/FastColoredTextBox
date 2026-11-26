using FastColoredTextBoxNS.Types;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace FastColoredTextBoxNS.Text
{
    public class SyntaxHighlighter : IDisposable
    {
        //styles
        protected static readonly Platform platformType = PlatformType.GetOperationSystemPlatform();

        public readonly Style BlueBoldStyle = new TextStyle(Brushes.DeepSkyBlue, null, FontStyle.Bold);
        public readonly Style BlueStyle = new TextStyle(Brushes.DeepSkyBlue, null, FontStyle.Regular);
        public readonly Style BoldStyle = new TextStyle(null, null, FontStyle.Bold | FontStyle.Underline);
        public readonly Style BrownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Italic);
        public readonly Style GrayStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);
        public readonly Style GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        public readonly Style MagentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
        public readonly Style MaroonStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);
        public readonly Style RedStyle = new TextStyle(Brushes.Red, null, FontStyle.Regular);
        public readonly Style BlackStyle = new TextStyle(Brushes.Black, null, FontStyle.Regular);

        //
        //ConcurrentDictionary: the highlighter can be shared by multiple FCTB instances on different threads (e.g. tabbed editors)
        protected readonly System.Collections.Concurrent.ConcurrentDictionary<string, SyntaxDescriptor> descByXMLfileNames =
            new(System.StringComparer.Ordinal);

        protected readonly List<Style> resilientStyles = new(5);

        protected Regex CSharpAttributeRegex,
                      CSharpClassNameRegex;

        protected Regex CSharpCommentRegex1,
                      CSharpCommentRegex2,
                      CSharpCommentRegex3;

        protected Regex CSharpKeywordRegex;
        protected Regex CSharpNumberRegex;
        protected Regex CSharpStringRegex;

        protected Regex HTMLAttrRegex,
                      HTMLAttrValRegex,
                      HTMLCommentRegex1,
                      HTMLCommentRegex2;

        protected Regex HTMLEndTagRegex;

        protected Regex HTMLEntityRegex,
                      HTMLTagContentRegex;

        protected Regex HTMLTagNameRegex;
        protected Regex HTMLTagRegex;

        protected Regex XMLAttrRegex,
                      XMLAttrValRegex,
                      XMLCommentRegex1,
                      XMLCommentRegex2;

        protected Regex XMLEndTagRegex;

        protected Regex XMLEntityRegex,
                      XMLTagContentRegex;

        protected Regex XMLTagNameRegex;
        protected Regex XMLTagRegex;
        protected Regex XMLCDataRegex;
        protected Regex XMLFoldingRegex;

        protected Regex JScriptCommentRegex1,
                      JScriptCommentRegex2,
                      JScriptCommentRegex3;

        protected Regex JScriptKeywordRegex;
        protected Regex JScriptNumberRegex;
        protected Regex JScriptStringRegex;

        protected Regex JSONKeywordRegex;
        protected Regex JSONNumberRegex;
        protected Regex JSONStringRegex;

        protected Regex LuaCommentRegex1,
                      LuaCommentRegex2,
                      LuaCommentRegex3;

        protected Regex LuaKeywordRegex;
        protected Regex LuaNumberRegex;
        protected Regex LuaStringRegex;
        protected Regex LuaFunctionsRegex;

        protected Regex PHPCommentRegex1,
                      PHPCommentRegex2,
                      PHPCommentRegex3;

        protected Regex PHPKeywordRegex1,
                      PHPKeywordRegex2,
                      PHPKeywordRegex3;

        protected Regex PHPNumberRegex;
        protected Regex PHPStringRegex;
        protected Regex PHPVarRegex;

        protected Regex MdHeadingRegex,
                      MdHrRegex,
                      MdQuoteRegex,
                      MdListRegex,
                      MdFenceOpenRegex,
                      MdFenceCloseRegex,
                      MdBoldRegex,
                      MdItalicRegex,
                      MdUnderscoreItalicRegex,
                      MdStrikethroughRegex,
                      MdInlineCodeRegex,
                      MdLinkRegex,
                      MdImageRegex,
                      MdAutoLinkRegex,
                      MdTableSeparatorRegex;

        //GDI objects created by InitStyleSchema for the markdown styles
        private readonly List<Brush> mdBrushes = new(8);

        protected Regex SQLCommentRegex1,
                      SQLCommentRegex2,
                      SQLCommentRegex3,
                      SQLCommentRegex4;

        protected Regex SQLFunctionsRegex;
        protected Regex SQLKeywordsRegex;
        protected Regex SQLNumberRegex;
        protected Regex SQLStatementsRegex;
        protected Regex SQLStringRegex;
        protected Regex SQLTypesRegex;
        protected Regex SQLVarRegex;
        protected Regex VBClassNameRegex;
        protected Regex VBCommentRegex;
        protected Regex VBKeywordRegex;
        protected Regex VBNumberRegex;
        protected Regex VBStringRegex;

        protected FastColoredTextBox currentTb;

        public static RegexOptions RegexCompiledOption => RegexOptions.Compiled;

        public SyntaxHighlighter(FastColoredTextBox currentTb)
        {
            this.currentTb = currentTb;
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (SyntaxDescriptor desc in descByXMLfileNames.Values)
                desc.Dispose();
            DisposeMarkdownStyles();
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members

        /// <summary>
        /// Highlights syntax for given language
        /// </summary>
        public virtual void HighlightSyntax(Language language, TextSelectionRange range)
        {
            switch (language)
            {
                case Language.CSharp:
                    CSharpSyntaxHighlight(range);
                    break;

                case Language.VB:
                    VBSyntaxHighlight(range);
                    break;

                case Language.HTML:
                    HTMLSyntaxHighlight(range);
                    break;

                case Language.XML:
                    XMLSyntaxHighlight(range);
                    break;

                case Language.SQL:
                    SQLSyntaxHighlight(range);
                    break;

                case Language.PHP:
                    PHPSyntaxHighlight(range);
                    break;

                case Language.JS:
                    JScriptSyntaxHighlight(range);
                    break;

                case Language.Lua:
                    LuaSyntaxHighlight(range);
                    break;

                case Language.JSON:
                    JSONSyntaxHighlight(range);
                    break;

                case Language.Markdown:
                    MarkdownSyntaxHighlight(range);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Highlights syntax for given XML description file
        /// </summary>
        public virtual void HighlightSyntax(string XMLdescriptionFile, TextSelectionRange range)
        {
            if (!descByXMLfileNames.TryGetValue(XMLdescriptionFile, out SyntaxDescriptor desc))
            {
                var doc = new XmlDocument();
                string file = XMLdescriptionFile;
                if (!File.Exists(file))
                    file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(file));

                doc.LoadXml(File.ReadAllText(file));
                desc = ParseXmlDescription(doc);
                descByXMLfileNames[XMLdescriptionFile] = desc;
            }

            HighlightSyntax(desc, range);
        }

        public virtual void AutoIndentNeeded(object sender, AutoIndentEventArgs args)
        {
            var tb = sender as FastColoredTextBox;
            Language language = tb.Language;
            switch (language)
            {
                case Language.CSharp:
                    CSharpAutoIndentNeeded(sender, args);
                    break;

                case Language.VB:
                    VBAutoIndentNeeded(sender, args);
                    break;

                case Language.HTML:
                    HTMLAutoIndentNeeded(sender, args);
                    break;

                case Language.XML:
                    XMLAutoIndentNeeded(sender, args);
                    break;

                case Language.SQL:
                    SQLAutoIndentNeeded(sender, args);
                    break;

                case Language.PHP:
                    PHPAutoIndentNeeded(sender, args);
                    break;

                case Language.JS:
                    CSharpAutoIndentNeeded(sender, args);
                    break; //JS like C#
                case Language.Lua:
                    LuaAutoIndentNeeded(sender, args);
                    break;

                case Language.Markdown:
                    MarkdownAutoIndentNeeded(sender, args);
                    break;

                default:
                    break;
            }
        }

        // Cached, precompiled auto-indent regexes (created on first use, reused afterwards).
        // Regex.IsMatch(string, string) re-parses the pattern on every call.
        private static Regex aiPHPBlockOpenClose, aiPHPBlockStart, aiPHPBlockEnd, aiPHPUnclosedOp, aiPHPClosedOp,
                             aiCSharpBlockOpenClose, aiCSharpBlockStart, aiCSharpBlockEnd,
                             aiCSharpLabel, aiCSharpDefault, aiCSharpCaseDefault,
                             aiCSharpUnclosedOp, aiCSharpClosedOp,
                             aiVBBlockEnd, aiVBDeclStart, aiVBThen, aiVBBlockStart, aiVBElse,
                             aiLuaBlockEnd, aiLuaThen, aiLuaBlockStart, aiLuaElse;

        private static Regex AiRegex(ref Regex slot, string pattern, RegexOptions options = RegexOptions.None)
        {
            slot ??= new Regex(pattern, RegexOptions.Compiled | options);
            return slot;
        }

        protected void PHPAutoIndentNeeded(object sender, AutoIndentEventArgs args)
        {
            /*
            FastColoredTextBox tb = sender as FastColoredTextBox;
            tb.CalcAutoIndentShiftByCodeFolding(sender, args);*/
            //block {}
            if (AiRegex(ref aiPHPBlockOpenClose, @"^[^""']*\{.*\}[^""']*$").IsMatch(args.LineText))
                return;
            //start of block {}
            if (AiRegex(ref aiPHPBlockStart, @"^[^""']*\{").IsMatch(args.LineText))
            {
                args.ShiftNextLines = args.TabLength;
                return;
            }
            //end of block {}
            if (AiRegex(ref aiPHPBlockEnd, @"}[^""']*$").IsMatch(args.LineText))
            {
                args.Shift = -args.TabLength;
                args.ShiftNextLines = -args.TabLength;
                return;
            }
            //is unclosed operator in previous line ?
            if (AiRegex(ref aiPHPUnclosedOp, @"^\s*(if|for|foreach|while|[\}\s]*else)\b[^{]*$").IsMatch(args.PrevLineText))
                if (!AiRegex(ref aiPHPClosedOp, @"(;\s*$)|(;\s*//)").IsMatch(args.PrevLineText)) //operator is unclosed
                {
                    args.Shift = args.TabLength;
                    return;
                }
        }

        protected void SQLAutoIndentNeeded(object sender, AutoIndentEventArgs args)
        {
            var tb = sender as FastColoredTextBox;
            tb.CalcAutoIndentShiftByCodeFolding(sender, args);
        }

        protected void HTMLAutoIndentNeeded(object sender, AutoIndentEventArgs args)
        {
            var tb = sender as FastColoredTextBox;
            tb.CalcAutoIndentShiftByCodeFolding(sender, args);
        }

        protected void XMLAutoIndentNeeded(object sender, AutoIndentEventArgs args)
        {
            var tb = sender as FastColoredTextBox;
            tb.CalcAutoIndentShiftByCodeFolding(sender, args);
        }

        protected void VBAutoIndentNeeded(object sender, AutoIndentEventArgs args)
        {
            //end of block
            if (AiRegex(ref aiVBBlockEnd, @"^\s*(End|EndIf|Next|Loop)\b", RegexOptions.IgnoreCase).IsMatch(args.LineText))
            {
                args.Shift = -args.TabLength;
                args.ShiftNextLines = -args.TabLength;
                return;
            }
            //start of declaration
            if (AiRegex(ref aiVBDeclStart, @"\b(Class|Property|Enum|Structure|Sub|Function|Namespace|Interface|Get)\b|(Set\s*\()", RegexOptions.IgnoreCase).IsMatch(args.LineText))
            {
                args.ShiftNextLines = args.TabLength;
                return;
            }
            // then ...
            if (AiRegex(ref aiVBThen, @"\b(Then)\s*\S+", RegexOptions.IgnoreCase).IsMatch(args.LineText))
                return;
            //start of operator block
            if (AiRegex(ref aiVBBlockStart, @"^\s*(If|While|For|Do|Try|With|Using|Select)\b", RegexOptions.IgnoreCase).IsMatch(args.LineText))
            {
                args.ShiftNextLines = args.TabLength;
                return;
            }

            //Statements else, elseif, case etc
            if (AiRegex(ref aiVBElse, @"^\s*(Else|ElseIf|Case|Catch|Finally)\b", RegexOptions.IgnoreCase).IsMatch(args.LineText))
            {
                args.Shift = -args.TabLength;
                return;
            }

            //Char _
            if (args.PrevLineText.TrimEnd().EndsWith("_"))
            {
                args.Shift = args.TabLength;
                return;
            }
        }

        protected void CSharpAutoIndentNeeded(object sender, AutoIndentEventArgs args)
        {
            //block {}
            if (AiRegex(ref aiCSharpBlockOpenClose, @"^[^""']*\{.*\}[^""']*$").IsMatch(args.LineText))
                return;
            //start of block {}
            if (AiRegex(ref aiCSharpBlockStart, @"^[^""']*\{").IsMatch(args.LineText))
            {
                args.ShiftNextLines = args.TabLength;
                return;
            }
            //end of block {}
            if (AiRegex(ref aiCSharpBlockEnd, @"}[^""']*$").IsMatch(args.LineText))
            {
                args.Shift = -args.TabLength;
                args.ShiftNextLines = -args.TabLength;
                return;
            }
            //label
            if (AiRegex(ref aiCSharpLabel, @"^\s*\w+\s*:\s*($|//)").IsMatch(args.LineText) &&
                !AiRegex(ref aiCSharpDefault, @"^\s*default\s*:").IsMatch(args.LineText))
            {
                args.Shift = -args.TabLength;
                return;
            }
            //some statements: case, default
            if (AiRegex(ref aiCSharpCaseDefault, @"^\s*(case|default)\b.*:\s*($|//)").IsMatch(args.LineText))
            {
                args.Shift = -args.TabLength / 2;
                return;
            }
            //is unclosed operator in previous line ?
            if (AiRegex(ref aiCSharpUnclosedOp, @"^\s*(if|for|foreach|while|[\}\s]*else)\b[^{]*$").IsMatch(args.PrevLineText))
                if (!AiRegex(ref aiCSharpClosedOp, @"(;\s*$)|(;\s*//)").IsMatch(args.PrevLineText)) //operator is unclosed
                {
                    args.Shift = args.TabLength;
                    return;
                }
        }

        /// <summary>
        /// Uses the given <paramref name="doc"/> to parse a XML description and adds it as syntax descriptor.
        /// The syntax descriptor is used for highlighting when
        /// <list type="bullet">
        ///     <item>Language property of FCTB is set to <see cref="Language.Custom"/></item>
        ///     <item>DescriptionFile property of FCTB has the same value as the method parameter <paramref name="descriptionFileName"/></item>
        /// </list>
        /// </summary>
        /// <param name="descriptionFileName">Name of the description file</param>
        /// <param name="doc">XmlDocument to parse</param>
        public virtual void AddXmlDescription(string descriptionFileName, XmlDocument doc)
        {
            SyntaxDescriptor desc = ParseXmlDescription(doc);
            descByXMLfileNames[descriptionFileName] = desc;
        }

        /// <summary>
        /// Adds the given <paramref name="style"/> as resilient style. A resilient style is additionally available when highlighting is
        /// based on a syntax descriptor that has been derived from a XML description file. In the run of the highlighting routine
        /// the styles used by the FCTB are always dropped and replaced with the (initial) ones from the syntax descriptor. Resilient styles are
        /// added afterwards and can be used anyway.
        /// </summary>
        /// <param name="style">Style to add</param>
        public virtual void AddResilientStyle(Style style)
        {
            if (resilientStyles.Contains(style)) return;
            resilientStyles.Add(style);
        }

        public static SyntaxDescriptor ParseXmlDescription(XmlDocument doc)
        {
            var desc = new SyntaxDescriptor();
            XmlNode brackets = doc.SelectSingleNode("doc/brackets");
            if (brackets != null)
            {
                if (brackets.Attributes["left"] == null || brackets.Attributes["right"] == null ||
                    brackets.Attributes["left"].Value == "" || brackets.Attributes["right"].Value == "")
                {
                    desc.leftBracket = '\x0';
                    desc.rightBracket = '\x0';
                }
                else
                {
                    desc.leftBracket = brackets.Attributes["left"].Value[0];
                    desc.rightBracket = brackets.Attributes["right"].Value[0];
                }

                if (brackets.Attributes["left2"] == null || brackets.Attributes["right2"] == null ||
                    brackets.Attributes["left2"].Value == "" || brackets.Attributes["right2"].Value == "")
                {
                    desc.leftBracket2 = '\x0';
                    desc.rightBracket2 = '\x0';
                }
                else
                {
                    desc.leftBracket2 = brackets.Attributes["left2"].Value[0];
                    desc.rightBracket2 = brackets.Attributes["right2"].Value[0];
                }

                if (brackets.Attributes["left3"] == null || brackets.Attributes["right3"] == null ||
                    brackets.Attributes["left3"].Value == "" || brackets.Attributes["right3"].Value == "")
                {
                    desc.leftBracket3 = '\x0';
                    desc.rightBracket3 = '\x0';
                }
                else
                {
                    desc.leftBracket3 = brackets.Attributes["left3"].Value[0];
                    desc.rightBracket3 = brackets.Attributes["right3"].Value[0];
                }

                if (brackets.Attributes["strategy"] == null || brackets.Attributes["strategy"].Value == "")
                    desc.bracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;
                else
                    desc.bracketsHighlightStrategy = (BracketsHighlightStrategy)Enum.Parse(typeof(BracketsHighlightStrategy), brackets.Attributes["strategy"].Value);
            }

            var styleByName = new Dictionary<string, Style>();

            foreach (XmlNode style in doc.SelectNodes("doc/style"))
            {
                Style s = ParseStyle(style);
                styleByName[style.Attributes["name"].Value] = s;
                desc.styles.Add(s);
            }
            foreach (XmlNode rule in doc.SelectNodes("doc/rule"))
                desc.rules.Add(ParseRule(rule, styleByName));
            foreach (XmlNode folding in doc.SelectNodes("doc/folding"))
                desc.foldings.Add(ParseFolding(folding));

            return desc;
        }

        protected static FoldingDesc ParseFolding(XmlNode foldingNode)
        {
            var folding = new FoldingDesc
            {
                //regex
                startMarkerRegex = foldingNode.Attributes["start"].Value,
                finishMarkerRegex = foldingNode.Attributes["finish"].Value
            };
            //options
            XmlAttribute optionsA = foldingNode.Attributes["options"];
            if (optionsA != null)
                folding.options = (RegexOptions)Enum.Parse(typeof(RegexOptions), optionsA.Value);

            return folding;
        }

        protected static RuleDesc ParseRule(XmlNode ruleNode, Dictionary<string, Style> styles)
        {
            var rule = new RuleDesc
            {
                pattern = ruleNode.InnerText
            };
            //
            XmlAttribute styleA = ruleNode.Attributes["style"];
            XmlAttribute optionsA = ruleNode.Attributes["options"];
            //Style
            if (styleA == null)
                throw new Exception("Rule must contain style name.");
            if (!styles.ContainsKey(styleA.Value))
                throw new Exception("Style '" + styleA.Value + "' is not found.");
            rule.style = styles[styleA.Value];
            //options
            if (optionsA != null)
                rule.options = (RegexOptions)Enum.Parse(typeof(RegexOptions), optionsA.Value);

            return rule;
        }

        protected static Style ParseStyle(XmlNode styleNode)
        {
            XmlAttribute colorA = styleNode.Attributes["color"];
            XmlAttribute backColorA = styleNode.Attributes["backColor"];
            XmlAttribute fontStyleA = styleNode.Attributes["fontStyle"];
            //colors
            SolidBrush foreBrush = null;
            if (colorA != null)
                foreBrush = new SolidBrush(ParseColor(colorA.Value));
            SolidBrush backBrush = null;
            if (backColorA != null)
                backBrush = new SolidBrush(ParseColor(backColorA.Value));
            //fontStyle
            FontStyle fontStyle = FontStyle.Regular;
            if (fontStyleA != null)
                fontStyle = (FontStyle)Enum.Parse(typeof(FontStyle), fontStyleA.Value);

            return new TextStyle(foreBrush, backBrush, fontStyle);
        }

        protected static Color ParseColor(string s)
        {
            if (s.StartsWith("#"))
            {
                if (s.Length <= 7)
                    return Color.FromArgb(255,
                                          Color.FromArgb(int.Parse(s[1..], NumberStyles.AllowHexSpecifier)));
                else
                    return Color.FromArgb(int.Parse(s[1..], NumberStyles.AllowHexSpecifier));
            }
            else
                return Color.FromName(s);
        }

        public void HighlightSyntax(SyntaxDescriptor desc, TextSelectionRange range)
        {
            //set style order
            range.tb.ClearStylesBuffer();
            foreach (var t in desc.styles)
                range.tb.StyleManager.AddStyle(t);

            // add resilient styles
            int l = desc.styles.Count;
            foreach (var t in resilientStyles)
                range.tb.StyleManager.AddStyle(t);

            //brackets
            char[] oldBrackets = RememberBrackets(range.tb);
            range.tb.LeftBracket = desc.leftBracket;
            range.tb.RightBracket = desc.rightBracket;
            range.tb.LeftBracket2 = desc.leftBracket2;
            range.tb.RightBracket2 = desc.rightBracket2;
            range.tb.LeftBracket3 = desc.leftBracket3;
            range.tb.RightBracket3 = desc.rightBracket3;
            //clear styles of range
            range.ClearStyle(desc.styles.ToArray());
            //highlight syntax
            foreach (RuleDesc rule in desc.rules)
                range.SetStyle(rule.style, rule.Regex);
            //clear folding
            range.ClearFoldingMarkers();
            //folding markers
            foreach (FoldingDesc folding in desc.foldings)
                range.SetFoldingMarkers(folding.startMarkerRegex, folding.finishMarkerRegex, folding.options);

            //
            RestoreBrackets(range.tb, oldBrackets);
        }

        protected static void RestoreBrackets(FastColoredTextBox tb, char[] oldBrackets)
        {
            tb.LeftBracket = oldBrackets[0];
            tb.RightBracket = oldBrackets[1];
            tb.LeftBracket2 = oldBrackets[2];
            tb.RightBracket2 = oldBrackets[3];
            tb.LeftBracket3 = oldBrackets[4];
            tb.RightBracket3 = oldBrackets[5];
        }

        protected static char[] RememberBrackets(FastColoredTextBox tb) => [tb.LeftBracket, tb.RightBracket, tb.LeftBracket2, tb.RightBracket2, tb.LeftBracket3, tb.RightBracket3];

        protected void InitCShaprRegex()
        {
            //CSharpStringRegex = new Regex( @"""""|@""""|''|@"".*?""|(?<!@)(?<range>"".*?[^\\]"")|'.*?[^\\]'", RegexCompiledOption);

            CSharpStringRegex =
                new Regex(
                    @"
                            # Character definitions:
                            '
                            (?> # disable backtracking
                              (?:
                                \\[^\r\n]|    # escaped meta char
                                [^'\r\n]      # any character except '
                              )*
                            )
                            '?
                            |
                            # Normal string & verbatim strings definitions:
                            (?<verbatimIdentifier>@)?         # this group matches if it is an verbatim string
                            ""
                            (?> # disable backtracking
                              (?:
                                # match and consume an escaped character including escaped double quote ("") char
                                (?(verbatimIdentifier)        # if it is a verbatim string ...
                                  """"|                         #   then: only match an escaped double quote ("") char
                                  \\.                         #   else: match an escaped sequence
                                )
                                | # OR

                                # match any char except double quote char ("")
                                [^""]
                              )*
                            )
                            ""
                        ",
                    RegexOptions.ExplicitCapture | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace |
                    RegexCompiledOption
                    ); //thanks to rittergig for this regex

            CSharpCommentRegex1 = new Regex(@"//.*$", RegexOptions.Multiline | RegexCompiledOption);
            CSharpCommentRegex2 = new Regex(@"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline | RegexCompiledOption);
            CSharpCommentRegex3 = new Regex(@"(/\*.*?\*/)|(.*\*/)",
                                            RegexOptions.Singleline | RegexOptions.RightToLeft | RegexCompiledOption);
            CSharpNumberRegex = new Regex(@"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b",
                                          RegexCompiledOption);
            CSharpAttributeRegex = new Regex(@"^\s*(?<range>\[.+?\])\s*$", RegexOptions.Multiline | RegexCompiledOption);
            CSharpClassNameRegex = new Regex(@"\b(class|struct|enum|interface)\s+(?<range>\w+?)\b", RegexCompiledOption);
            CSharpKeywordRegex =
                new Regex(
                    @"\b(abstract|add|alias|as|ascending|async|await|base|bool|break|by|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|descending|do|double|dynamic|else|enum|equals|event|explicit|extern|false|finally|fixed|float|for|foreach|from|get|global|goto|group|if|implicit|in|int|interface|internal|into|is|join|let|lock|long|nameof|namespace|new|null|object|on|operator|orderby|out|override|params|partial|private|protected|public|readonly|ref|remove|return|sbyte|sealed|select|set|short|sizeof|stackalloc|static|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|using|value|var|virtual|void|volatile|when|where|while|yield)\b|#region\b|#endregion\b",
                    RegexCompiledOption);
        }

        public void InitStyleSchema(Language lang)
        {
            switch (lang)
            {
                case Language.CSharp:
                    StringStyle = BrownStyle;
                    CommentStyle = GreenStyle;
                    NumberStyle = MagentaStyle;
                    AttributeStyle = GreenStyle;
                    ClassNameStyle = BoldStyle;
                    KeywordStyle = BlueStyle;
                    CommentTagStyle = GrayStyle;
                    break;

                case Language.VB:
                    StringStyle = BrownStyle;
                    CommentStyle = GreenStyle;
                    NumberStyle = MagentaStyle;
                    ClassNameStyle = BoldStyle;
                    KeywordStyle = BlueStyle;
                    break;

                case Language.HTML:
                    CommentStyle = GreenStyle;
                    TagBracketStyle = BlueStyle;
                    TagNameStyle = MaroonStyle;
                    AttributeStyle = RedStyle;
                    AttributeValueStyle = BlueStyle;
                    HtmlEntityStyle = RedStyle;
                    break;

                case Language.XML:
                    CommentStyle = GreenStyle;
                    XmlTagBracketStyle = BlueStyle;
                    XmlTagNameStyle = MaroonStyle;
                    XmlAttributeStyle = RedStyle;
                    XmlAttributeValueStyle = BlueStyle;
                    XmlEntityStyle = RedStyle;
                    XmlCDataStyle = BlackStyle;
                    break;

                case Language.JS:
                    StringStyle = BrownStyle;
                    CommentStyle = GreenStyle;
                    NumberStyle = MagentaStyle;
                    KeywordStyle = BlueStyle;
                    break;

                case Language.Lua:
                    StringStyle = BrownStyle;
                    CommentStyle = GreenStyle;
                    NumberStyle = MagentaStyle;
                    KeywordStyle = BlueBoldStyle;
                    FunctionsStyle = MaroonStyle;
                    break;

                case Language.PHP:
                    StringStyle = RedStyle;
                    CommentStyle = GreenStyle;
                    NumberStyle = RedStyle;
                    VariableStyle = MaroonStyle;
                    KeywordStyle = MagentaStyle;
                    KeywordStyle2 = BlueStyle;
                    KeywordStyle3 = GrayStyle;
                    break;

                case Language.SQL:
                    StringStyle = RedStyle;
                    CommentStyle = GreenStyle;
                    NumberStyle = MagentaStyle;
                    KeywordStyle = BlueBoldStyle;
                    StatementsStyle = BlueBoldStyle;
                    FunctionsStyle = MaroonStyle;
                    VariableStyle = MaroonStyle;
                    TypesStyle = BrownStyle;
                    break;

                case Language.JSON:
                    StringStyle = BrownStyle;
                    NumberStyle = MagentaStyle;
                    KeywordStyle = BlueStyle;
                    break;

                case Language.Markdown:
                    //dispose previously created brushes/styles before recreating them,
                    //InitStyleSchema is called every time the Language property changes
                    DisposeMarkdownStyles();
                    //The palette is the page's own ink plus a single accent hue, so prose reads calmly
                    //instead of taking a different colour per construct. Everything is derived from
                    //the control's colours, so light and dark hosts share the same scheme.
                    Color page = currentTb?.BackColor ?? SystemColors.Window;
                    bool darkPage = page.GetBrightness() < 0.5f;
                    Color ink = currentTb?.ForeColor ?? SystemColors.WindowText;
                    Color accent = darkPage ? Color.FromArgb(0x93, 0xB8, 0xE8) : Color.FromArgb(0x2B, 0x5C, 0x8A);
                    Color dim = MdMix(ink, page, 0.45f);
                    Color faint = MdMix(ink, page, 0.62f);
                    Color green = ((TextStyle)GreenStyle).ForeBrush is SolidBrush g ? g.Color : Color.Green;
                    //heading levels are told apart by the strength of the accent: the control is a
                    //fixed-width grid, so a scaled font would break caret, selection and wrap geometry
                    MdH1Style = new TextStyle(MdBrush(Shade(accent, 0.15f, darkPage)), null, FontStyle.Bold);
                    MdH2Style = new TextStyle(MdBrush(accent), null, FontStyle.Bold);
                    MdH3Style = new TextStyle(MdBrush(MdMix(accent, page, 0.25f)), null, FontStyle.Bold);
                    MdHeadingStyle = new TextStyle(MdBrush(MdMix(accent, page, 0.40f)), null, FontStyle.Bold);
                    //emphasis is the body colour in another weight: no extra hue for bold / italic
                    MdBoldStyle = new TextStyle(null, null, FontStyle.Bold);
                    MdItalicStyle = new TextStyle(null, null, FontStyle.Italic);
                    MdUnderscoreItalicStyle = MdItalicStyle;
                    MdStrikethroughStyle = new TextStyle(MdBrush(dim), null, FontStyle.Strikeout);
                    //nothing paints a text background any more: code is marked by the accent colour
                    //(a magenta glyph on a grey chip was unreadable), blocks by their left bar only
                    MdInlineCodeStyle = new TextStyle(MdBrush(accent), null, FontStyle.Regular);
                    //block bars are the only mark of a block now (no tint), so their width follows the
                    //screen DPI: 3 device pixels would be a hairline on a 250% display
                    int bar = Math.Max(3, (int)Math.Round(3 * (currentTb?.DeviceDpi ?? 96) / 96.0));
                    MdCodeBlockStyle = new MarkdownBlockStyle(null, MdBrush(dim), bar);
                    MdBlockquoteStyle = new MarkdownBlockStyle(null, MdBrush(dim), Math.Max(2, bar * 2 / 3));
                    MdLinkStyle = new TextStyle(MdBrush(accent), null, FontStyle.Underline);
                    MdAutoLinkStyle = MdLinkStyle;
                    MdImageStyle = new TextStyle(MdBrush(dim), null, FontStyle.Italic);
                    MdTaskListDoneStyle = new TextStyle(MdBrush(Shade(green, 0.15f, darkPage)), null, FontStyle.Bold);
                    MdHrStyle = new TextStyle(MdBrush(faint), null, FontStyle.Regular);
                    MdListStyle = new TextStyle(MdBrush(ink), null, FontStyle.Regular);
                    break;
            }
        }

        /// <summary>
        /// Moves a colour away from the page background: darker on a light page, lighter on a dark
        /// one. A negative amount moves it back towards the background (less prominent).
        /// </summary>
        private static Color Shade(Color color, float amount, bool darkPage)
        {
            float a = Math.Abs(amount);
            bool lighten = amount > 0 ? darkPage : !darkPage;
            return lighten ? ControlPaint.Light(color, a) : ControlPaint.Dark(color, a);
        }

        /// <summary>
        /// Blends <paramref name="color"/> towards <paramref name="towards"/> by 0..1, used to fade
        /// a colour into the page (secondary text, softer heading levels).
        /// </summary>
        private static Color MdMix(Color color, Color towards, float amount)
        {
            float t = Math.Clamp(amount, 0f, 1f);
            return Color.FromArgb(
                (int)Math.Round(color.R + (towards.R - color.R) * t),
                (int)Math.Round(color.G + (towards.G - color.G) * t),
                (int)Math.Round(color.B + (towards.B - color.B) * t));
        }

        /// <summary>
        /// Solid brush owned by the markdown styles, disposed by <see cref="DisposeMarkdownStyles"/>
        /// </summary>
        private Brush MdBrush(Color color)
        {
            var brush = new SolidBrush(color);
            mdBrushes.Add(brush);
            return brush;
        }

        /// <summary>
        /// Disposes the GDI brushes created by InitStyleSchema for Markdown styles
        /// </summary>
        protected virtual void DisposeMarkdownStyles()
        {
            foreach (var brush in mdBrushes)
                brush.Dispose();
            mdBrushes.Clear();
            //
            MdCodeBlockStyle = MdBlockquoteStyle = null;
            MdH1Style = MdH2Style = MdH3Style = MdHeadingStyle = null;
            MdInlineCodeStyle = MdTaskListDoneStyle = null;
        }

        /// <summary>
        /// Highlights C# code
        /// </summary>
        /// <param name="range"></param>
        public virtual void CSharpSyntaxHighlight(TextSelectionRange range)
        {
            range.tb.CommentPrefix = "//";
            range.tb.LeftBracket = '(';
            range.tb.RightBracket = ')';
            range.tb.LeftBracket2 = '{';
            range.tb.RightBracket2 = '}';
            range.tb.LeftBracket3 = '[';
            range.tb.RightBracket3 = ']';
            range.tb.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;

            range.tb.AutoIndentCharsPatterns
                = @"
^\s*[\w\.]+(\s\w+)?\s*(?<range>=)\s*(?<range>[^;=]+);
^\s*(case|default)\s*[^:]*(?<range>:)\s*(?<range>[^;]+);
";
            //clear style of changed range
            range.ClearStyle(StringStyle, CommentStyle, NumberStyle, AttributeStyle, ClassNameStyle, KeywordStyle);
            //
            if (CSharpStringRegex == null)
                InitCShaprRegex();
            //string highlighting
            range.SetStyle(StringStyle, CSharpStringRegex);
            //comment highlighting
            range.SetStyle(CommentStyle, CSharpCommentRegex1);
            range.SetStyle(CommentStyle, CSharpCommentRegex2);
            range.SetStyle(CommentStyle, CSharpCommentRegex3);
            //number highlighting
            range.SetStyle(NumberStyle, CSharpNumberRegex);
            //attribute highlighting
            range.SetStyle(AttributeStyle, CSharpAttributeRegex);
            //class name highlighting
            range.SetStyle(ClassNameStyle, CSharpClassNameRegex);
            //keyword highlighting
            range.SetStyle(KeywordStyle, CSharpKeywordRegex);

            //find document comments
            foreach (TextSelectionRange r in range.GetRanges(@"^\s*///.*$", RegexOptions.Multiline))
            {
                //remove C# highlighting from this fragment
                r.ClearAllStyles();
                //do XML highlighting
                if (HTMLTagRegex == null)
                    InitHTMLRegex();
                //
                r.SetStyle(CommentStyle);
                //tags
                foreach (TextSelectionRange rr in r.GetRanges(HTMLTagContentRegex))
                {
                    rr.ClearAllStyles();
                    rr.SetStyle(CommentTagStyle);
                }
                //prefix '///'
                foreach (TextSelectionRange rr in r.GetRanges(@"^\s*///", RegexOptions.Multiline))
                {
                    rr.ClearAllStyles();
                    rr.SetStyle(CommentTagStyle);
                }
            }

            //clear folding markers
            range.ClearFoldingMarkers();
            //set folding markers
            range.SetFoldingMarkers("{", "}"); //allow to collapse brackets block
            range.SetFoldingMarkers(@"#region\b", @"#endregion\b"); //allow to collapse #region blocks
            range.SetFoldingMarkers(@"/\*", @"\*/"); //allow to collapse comment block
        }

        protected void InitVBRegex()
        {
            VBStringRegex = new Regex(@"""""|"".*?[^\\]""", RegexCompiledOption);
            VBCommentRegex = new Regex(@"'.*$", RegexOptions.Multiline | RegexCompiledOption);
            VBNumberRegex = new Regex(@"\b\d+[\.]?\d*([eE]\-?\d+)?\b", RegexCompiledOption);
            VBClassNameRegex = new Regex(@"\b(Class|Structure|Enum|Interface)[ ]+(?<range>\w+?)\b",
                                         RegexOptions.IgnoreCase | RegexCompiledOption);
            VBKeywordRegex =
                new Regex(
                    @"\b(AddHandler|AddressOf|Alias|And|AndAlso|As|Boolean|ByRef|Byte|ByVal|Call|Case|Catch|CBool|CByte|CChar|CDate|CDbl|CDec|Char|CInt|Class|CLng|CObj|Const|Continue|CSByte|CShort|CSng|CStr|CType|CUInt|CULng|CUShort|Date|Decimal|Declare|Default|Delegate|Dim|DirectCast|Do|Double|Each|Else|ElseIf|End|EndIf|Enum|Erase|Error|Event|Exit|False|Finally|For|Friend|Function|Get|GetType|GetXMLNamespace|Global|GoSub|GoTo|Handles|If|Implements|Imports|In|Inherits|Integer|Interface|Is|IsNot|Let|Lib|Like|Long|Loop|Me|Mod|Module|MustInherit|MustOverride|MyBase|MyClass|Namespace|Narrowing|New|Next|Not|Nothing|NotInheritable|NotOverridable|Object|Of|On|Operator|Option|Optional|Or|OrElse|Overloads|Overridable|Overrides|ParamArray|Partial|Private|Property|Protected|Public|RaiseEvent|ReadOnly|ReDim|REM|RemoveHandler|Resume|Return|SByte|Select|Set|Shadows|Shared|Short|Single|Static|Step|Stop|String|Structure|Sub|SyncLock|Then|Throw|To|True|Try|TryCast|TypeOf|UInteger|ULong|UShort|Using|Variant|Wend|When|While|Widening|With|WithEvents|WriteOnly|Xor|Region)\b|(#Const|#Else|#ElseIf|#End|#If|#Region)\b",
                    RegexOptions.IgnoreCase | RegexCompiledOption);
        }

        /// <summary>
        /// Highlights VB code
        /// </summary>
        /// <param name="range"></param>
        public virtual void VBSyntaxHighlight(TextSelectionRange range)
        {
            range.tb.CommentPrefix = "'";
            range.tb.LeftBracket = '(';
            range.tb.RightBracket = ')';
            range.tb.LeftBracket2 = '\x0';
            range.tb.RightBracket2 = '\x0';
            range.tb.LeftBracket3 = '\x0';
            range.tb.RightBracket3 = '\x0';

            range.tb.AutoIndentCharsPatterns
                = @"
^\s*[\w\.\(\)]+\s*(?<range>=)\s*(?<range>.+)
";
            //clear style of changed range
            range.ClearStyle(StringStyle, CommentStyle, NumberStyle, ClassNameStyle, KeywordStyle);
            //
            if (VBStringRegex == null)
                InitVBRegex();
            //string highlighting
            range.SetStyle(StringStyle, VBStringRegex);
            //comment highlighting
            range.SetStyle(CommentStyle, VBCommentRegex);
            //number highlighting
            range.SetStyle(NumberStyle, VBNumberRegex);
            //class name highlighting
            range.SetStyle(ClassNameStyle, VBClassNameRegex);
            //keyword highlighting
            range.SetStyle(KeywordStyle, VBKeywordRegex);

            //clear folding markers
            range.ClearFoldingMarkers();
            //set folding markers
            range.SetFoldingMarkers(@"#Region\b", @"#End\s+Region\b", RegexOptions.IgnoreCase);
            range.SetFoldingMarkers(@"\b(Class|Property|Enum|Structure|Interface)[ \t]+\S+",
                                    @"\bEnd (Class|Property|Enum|Structure|Interface)\b", RegexOptions.IgnoreCase);
            range.SetFoldingMarkers(@"^\s*(?<range>While)[ \t]+\S+", @"^\s*(?<range>End While)\b",
                                    RegexOptions.Multiline | RegexOptions.IgnoreCase);
            range.SetFoldingMarkers(@"\b(Sub|Function)[ \t]+[^\s']+", @"\bEnd (Sub|Function)\b", RegexOptions.IgnoreCase);
            //this declared separately because Sub and Function can be unclosed
            range.SetFoldingMarkers(@"(\r|\n|^)[ \t]*(?<range>Get|Set)[ \t]*(\r|\n|$)", @"\bEnd (Get|Set)\b",
                                    RegexOptions.IgnoreCase);
            range.SetFoldingMarkers(@"^\s*(?<range>For|For\s+Each)\b", @"^\s*(?<range>Next)\b",
                                    RegexOptions.Multiline | RegexOptions.IgnoreCase);
            range.SetFoldingMarkers(@"^\s*(?<range>Do)\b", @"^\s*(?<range>Loop)\b",
                                    RegexOptions.Multiline | RegexOptions.IgnoreCase);
        }

        protected void InitHTMLRegex()
        {
            HTMLCommentRegex1 = new Regex(@"(<!--.*?-->)|(<!--.*)", RegexOptions.Singleline | RegexCompiledOption);
            HTMLCommentRegex2 = new Regex(@"(<!--.*?-->)|(.*-->)",
                                          RegexOptions.Singleline | RegexOptions.RightToLeft | RegexCompiledOption);
            HTMLTagRegex = new Regex(@"<|/>|</|>", RegexCompiledOption);
            HTMLTagNameRegex = new Regex(@"<(?<range>[!\w:]+)", RegexCompiledOption);
            HTMLEndTagRegex = new Regex(@"</(?<range>[\w:]+)>", RegexCompiledOption);
            HTMLTagContentRegex = new Regex(@"<[^>]+>", RegexCompiledOption);
            HTMLAttrRegex =
                new Regex(
                    @"(?<range>[\w\d\-]{1,20}?)='[^']*'|(?<range>[\w\d\-]{1,20})=""[^""]*""|(?<range>[\w\d\-]{1,20})=[\w\d\-]{1,20}",
                    RegexCompiledOption);
            HTMLAttrValRegex =
                new Regex(
                    @"[\w\d\-]{1,20}?=(?<range>'[^']*')|[\w\d\-]{1,20}=(?<range>""[^""]*"")|[\w\d\-]{1,20}=(?<range>[\w\d\-]{1,20})",
                    RegexCompiledOption);
            HTMLEntityRegex = new Regex(@"\&(amp|gt|lt|nbsp|quot|apos|copy|reg|#[0-9]{1,8}|#x[0-9a-f]{1,8});",
                                        RegexCompiledOption | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Highlights HTML code
        /// </summary>
        /// <param name="range"></param>
        public virtual void HTMLSyntaxHighlight(TextSelectionRange range)
        {
            range.tb.CommentPrefix = null;
            range.tb.LeftBracket = '<';
            range.tb.RightBracket = '>';
            range.tb.LeftBracket2 = '(';
            range.tb.RightBracket2 = ')';
            range.tb.LeftBracket3 = '\x0';
            range.tb.RightBracket3 = '\x0';
            range.tb.AutoIndentCharsPatterns = @"";
            //clear style of changed range
            range.ClearStyle(CommentStyle, TagBracketStyle, TagNameStyle, AttributeStyle, AttributeValueStyle,
                             HtmlEntityStyle);
            //
            if (HTMLTagRegex == null)
                InitHTMLRegex();
            //comment highlighting
            range.SetStyle(CommentStyle, HTMLCommentRegex1);
            range.SetStyle(CommentStyle, HTMLCommentRegex2);
            //tag brackets highlighting
            range.SetStyle(TagBracketStyle, HTMLTagRegex);
            //tag name
            range.SetStyle(TagNameStyle, HTMLTagNameRegex);
            //end of tag
            range.SetStyle(TagNameStyle, HTMLEndTagRegex);
            //attributes
            range.SetStyle(AttributeStyle, HTMLAttrRegex);
            //attribute values
            range.SetStyle(AttributeValueStyle, HTMLAttrValRegex);
            //html entity
            range.SetStyle(HtmlEntityStyle, HTMLEntityRegex);

            //clear folding markers
            range.ClearFoldingMarkers();
            //set folding markers
            range.SetFoldingMarkers("<head", "</head>", RegexOptions.IgnoreCase);
            range.SetFoldingMarkers("<body", "</body>", RegexOptions.IgnoreCase);
            range.SetFoldingMarkers("<table", "</table>", RegexOptions.IgnoreCase);
            range.SetFoldingMarkers("<form", "</form>", RegexOptions.IgnoreCase);
            range.SetFoldingMarkers("<div", "</div>", RegexOptions.IgnoreCase);
            range.SetFoldingMarkers("<script", "</script>", RegexOptions.IgnoreCase);
            range.SetFoldingMarkers("<tr", "</tr>", RegexOptions.IgnoreCase);
        }

        protected void InitXMLRegex()
        {
            XMLCommentRegex1 = new Regex(@"(<!--.*?-->)|(<!--.*)", RegexOptions.Singleline | RegexCompiledOption);
            XMLCommentRegex2 = new Regex(@"(<!--.*?-->)|(.*-->)", RegexOptions.Singleline | RegexOptions.RightToLeft | RegexCompiledOption);

            XMLTagRegex = new Regex(@"<\?|</|>|<|/>|\?>", RegexCompiledOption);
            XMLTagNameRegex = new Regex(@"<[?](?<range1>[x][m][l]{1})|<(?<range>[!\w:\-\.]+)", RegexCompiledOption);
            XMLEndTagRegex = new Regex(@"</(?<range>[\w:\-\.]+)>", RegexCompiledOption);

            XMLTagContentRegex = new Regex(@"<[^>]+>", RegexCompiledOption);
            XMLAttrRegex =
                new Regex(
                    @"(?<range>[\w\d\-\:]+)[ ]*=[ ]*'[^']*'|(?<range>[\w\d\-\:]+)[ ]*=[ ]*""[^""]*""|(?<range>[\w\d\-\:]+)[ ]*=[ ]*[\w\d\-\:]+",
                    RegexCompiledOption);
            XMLAttrValRegex =
                new Regex(
                    @"[\w\d\-]+?=(?<range>'[^']*')|[\w\d\-]+[ ]*=[ ]*(?<range>""[^""]*"")|[\w\d\-]+[ ]*=[ ]*(?<range>[\w\d\-]+)",
                    RegexCompiledOption);
            XMLEntityRegex = new Regex(@"\&(amp|gt|lt|nbsp|quot|apos|copy|reg|#[0-9]{1,8}|#x[0-9a-f]{1,8});",
                                        RegexCompiledOption | RegexOptions.IgnoreCase);
            XMLCDataRegex = new Regex(@"<!\s*\[CDATA\s*\[(?<text>(?>[^]]+|](?!]>))*)]]>", RegexCompiledOption | RegexOptions.IgnoreCase); // http://stackoverflow.com/questions/21681861/i-need-a-regex-that-matches-cdata-elements-in-html
            XMLFoldingRegex = new Regex(@"<(?<range>/?[\w:\-\.]+)\s[^>]*?[^/]>|<(?<range>/?[\w:\-\.]+)\s*>", RegexOptions.Singleline | RegexCompiledOption);
        }

        /// <summary>
        /// Highlights XML code
        /// </summary>
        /// <param name="range"></param>
        public virtual void XMLSyntaxHighlight(TextSelectionRange range)
        {
            range.tb.CommentPrefix = null;
            range.tb.LeftBracket = '<';
            range.tb.RightBracket = '>';
            range.tb.LeftBracket2 = '(';
            range.tb.RightBracket2 = ')';
            range.tb.LeftBracket3 = '\x0';
            range.tb.RightBracket3 = '\x0';
            range.tb.AutoIndentCharsPatterns = @"";
            //clear style of changed range
            range.ClearStyle(CommentStyle, XmlTagBracketStyle, XmlTagNameStyle, XmlAttributeStyle, XmlAttributeValueStyle,
                             XmlEntityStyle, XmlCDataStyle);

            //
            if (XMLTagRegex == null)
            {
                InitXMLRegex();
            }

            //xml CData
            range.SetStyle(XmlCDataStyle, XMLCDataRegex);

            //comment highlighting
            range.SetStyle(CommentStyle, XMLCommentRegex1);
            range.SetStyle(CommentStyle, XMLCommentRegex2);

            //tag brackets highlighting
            range.SetStyle(XmlTagBracketStyle, XMLTagRegex);

            //tag name
            range.SetStyle(XmlTagNameStyle, XMLTagNameRegex);

            //end of tag
            range.SetStyle(XmlTagNameStyle, XMLEndTagRegex);

            //attributes
            range.SetStyle(XmlAttributeStyle, XMLAttrRegex);

            //attribute values
            range.SetStyle(XmlAttributeValueStyle, XMLAttrValRegex);

            //xml entity
            range.SetStyle(XmlEntityStyle, XMLEntityRegex);

            //clear folding markers
            range.ClearFoldingMarkers();

            //set folding markers
            XmlFolding(range);
        }

        private void XmlFolding(TextSelectionRange range)
        {
            var stack = new Stack<XmlFoldingTag>();
            var id = 0;
            var fctb = range.tb;
            //extract opening and closing tags (exclude open-close tags: <TAG/>)
            foreach (var r in range.GetRanges(XMLFoldingRegex))
            {
                var tagName = r.Text;
                var iLine = r.Start.iLine;
                //if it is opening tag...
                if (tagName[0] != '/')
                {
                    // ...push into stack
                    var tag = new XmlFoldingTag { Name = tagName, id = id++, startLine = r.Start.iLine };
                    stack.Push(tag);
                    // if this line has no markers - set marker
                    if (string.IsNullOrEmpty(fctb[iLine].FoldingStartMarker))
                        fctb[iLine].FoldingStartMarker = tag.Marker;
                }
                else
                {
                    //if it is closing tag - pop from stack
                    if (stack.Count > 0)
                    {
                        var tag = stack.Pop();
                        //compare line number
                        if (iLine == tag.startLine)
                        {
                            //remove marker, because same line can not be folding
                            if (fctb[iLine].FoldingStartMarker == tag.Marker) //was it our marker?
                                fctb[iLine].FoldingStartMarker = null;
                        }
                        else
                        {
                            //set end folding marker
                            if (string.IsNullOrEmpty(fctb[iLine].FoldingEndMarker))
                                fctb[iLine].FoldingEndMarker = tag.Marker;
                        }
                    }
                }
            }
        }

        private class XmlFoldingTag
        {
            public string Name;
            public int id;
            public int startLine;
            public string Marker { get { return Name + id; } }
        }

        protected void InitSQLRegex()
        {
            SQLStringRegex = new Regex(@"""""|''|"".*?[^\\]""|'.*?[^\\]'", RegexCompiledOption);
            SQLNumberRegex = new Regex(@"\b\d+[\.]?\d*([eE]\-?\d+)?\b", RegexCompiledOption);
            SQLCommentRegex1 = new Regex(@"--.*$", RegexOptions.Multiline | RegexCompiledOption);
            SQLCommentRegex2 = new Regex(@"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline | RegexCompiledOption);
            SQLCommentRegex3 = new Regex(@"(/\*.*?\*/)|(.*\*/)", RegexOptions.Singleline | RegexOptions.RightToLeft | RegexCompiledOption);
            SQLCommentRegex4 = new Regex(@"#.*$", RegexOptions.Multiline | RegexCompiledOption);
            SQLVarRegex = new Regex(@"@[a-zA-Z_\d]*\b", RegexCompiledOption);
            SQLStatementsRegex = new Regex(@"\b(ALTER APPLICATION ROLE|ALTER ASSEMBLY|ALTER ASYMMETRIC KEY|ALTER AUTHORIZATION|ALTER BROKER PRIORITY|ALTER CERTIFICATE|ALTER CREDENTIAL|ALTER CRYPTOGRAPHIC PROVIDER|ALTER DATABASE|ALTER DATABASE AUDIT SPECIFICATION|ALTER DATABASE ENCRYPTION KEY|ALTER ENDPOINT|ALTER EVENT SESSION|ALTER FULLTEXT CATALOG|ALTER FULLTEXT INDEX|ALTER FULLTEXT STOPLIST|ALTER FUNCTION|ALTER INDEX|ALTER LOGIN|ALTER MASTER KEY|ALTER MESSAGE TYPE|ALTER PARTITION FUNCTION|ALTER PARTITION SCHEME|ALTER PROCEDURE|ALTER QUEUE|ALTER REMOTE SERVICE BINDING|ALTER RESOURCE GOVERNOR|ALTER RESOURCE POOL|ALTER ROLE|ALTER ROUTE|ALTER SCHEMA|ALTER SERVER AUDIT|ALTER SERVER AUDIT SPECIFICATION|ALTER SERVICE|ALTER SERVICE MASTER KEY|ALTER SYMMETRIC KEY|ALTER TABLE|ALTER TRIGGER|ALTER USER|ALTER VIEW|ALTER WORKLOAD GROUP|ALTER XML SCHEMA COLLECTION|BULK INSERT|CREATE AGGREGATE|CREATE APPLICATION ROLE|CREATE ASSEMBLY|CREATE ASYMMETRIC KEY|CREATE BROKER PRIORITY|CREATE CERTIFICATE|CREATE CONTRACT|CREATE CREDENTIAL|CREATE CRYPTOGRAPHIC PROVIDER|CREATE DATABASE|CREATE DATABASE AUDIT SPECIFICATION|CREATE DATABASE ENCRYPTION KEY|CREATE DEFAULT|CREATE ENDPOINT|CREATE EVENT NOTIFICATION|CREATE EVENT SESSION|CREATE FULLTEXT CATALOG|CREATE FULLTEXT INDEX|CREATE FULLTEXT STOPLIST|CREATE FUNCTION|CREATE INDEX|CREATE LOGIN|CREATE MASTER KEY|CREATE MESSAGE TYPE|CREATE PARTITION FUNCTION|CREATE PARTITION SCHEME|CREATE PROCEDURE|CREATE QUEUE|CREATE REMOTE SERVICE BINDING|CREATE RESOURCE POOL|CREATE ROLE|CREATE ROUTE|CREATE RULE|CREATE SCHEMA|CREATE SERVER AUDIT|CREATE SERVER AUDIT SPECIFICATION|CREATE SERVICE|CREATE SPATIAL INDEX|CREATE STATISTICS|CREATE SYMMETRIC KEY|CREATE SYNONYM|CREATE TABLE|CREATE TRIGGER|CREATE TYPE|CREATE USER|CREATE VIEW|CREATE WORKLOAD GROUP|CREATE XML INDEX|CREATE XML SCHEMA COLLECTION|DELETE|DISABLE TRIGGER|DROP AGGREGATE|DROP APPLICATION ROLE|DROP ASSEMBLY|DROP ASYMMETRIC KEY|DROP BROKER PRIORITY|DROP CERTIFICATE|DROP CONTRACT|DROP CREDENTIAL|DROP CRYPTOGRAPHIC PROVIDER|DROP DATABASE|DROP DATABASE AUDIT SPECIFICATION|DROP DATABASE ENCRYPTION KEY|DROP DEFAULT|DROP ENDPOINT|DROP EVENT NOTIFICATION|DROP EVENT SESSION|DROP FULLTEXT CATALOG|DROP FULLTEXT INDEX|DROP FULLTEXT STOPLIST|DROP FUNCTION|DROP INDEX|DROP LOGIN|DROP MASTER KEY|DROP MESSAGE TYPE|DROP PARTITION FUNCTION|DROP PARTITION SCHEME|DROP PROCEDURE|DROP QUEUE|DROP REMOTE SERVICE BINDING|DROP RESOURCE POOL|DROP ROLE|DROP ROUTE|DROP RULE|DROP SCHEMA|DROP SERVER AUDIT|DROP SERVER AUDIT SPECIFICATION|DROP SERVICE|DROP SIGNATURE|DROP STATISTICS|DROP SYMMETRIC KEY|DROP SYNONYM|DROP TABLE|DROP TRIGGER|DROP TYPE|DROP USER|DROP VIEW|DROP WORKLOAD GROUP|DROP XML SCHEMA COLLECTION|ENABLE TRIGGER|EXEC|EXECUTE|REPLACE|FROM|INSERT|MERGE|OPTION|OUTPUT|SELECT|TOP|TRUNCATE TABLE|UPDATE|UPDATE STATISTICS|WHERE|WITH|INTO|IN|SET)\b", RegexOptions.IgnoreCase | RegexCompiledOption);
            SQLKeywordsRegex = new Regex(@"\b(ADD|ALL|AND|ANY|AS|ASC|AUTHORIZATION|BACKUP|BEGIN|BETWEEN|BREAK|BROWSE|BY|CASCADE|CHECK|CHECKPOINT|CLOSE|CLUSTERED|COLLATE|COLUMN|COMMIT|COMPUTE|CONSTRAINT|CONTAINS|CONTINUE|CROSS|CURRENT|CURRENT_DATE|CURRENT_TIME|CURSOR|DATABASE|DBCC|DEALLOCATE|DECLARE|DEFAULT|DENY|DESC|DISK|DISTINCT|DISTRIBUTED|DOUBLE|DUMP|ELSE|END|ERRLVL|ESCAPE|EXCEPT|EXISTS|EXIT|EXTERNAL|FETCH|FILE|FILLFACTOR|FOR|FOREIGN|FREETEXT|FULL|FUNCTION|GOTO|GRANT|GROUP|HAVING|HOLDLOCK|IDENTITY|IDENTITY_INSERT|IDENTITYCOL|IF|INDEX|INNER|INTERSECT|IS|JOIN|KEY|KILL|LIKE|LINENO|LOAD|NATIONAL|NOCHECK|NONCLUSTERED|NOT|NULL|OF|OFF|OFFSETS|ON|OPEN|OR|ORDER|OUTER|OVER|PERCENT|PIVOT|PLAN|PRECISION|PRIMARY|PRINT|PROC|PROCEDURE|PUBLIC|RAISERROR|READ|READTEXT|RECONFIGURE|REFERENCES|REPLICATION|RESTORE|RESTRICT|RETURN|REVERT|REVOKE|ROLLBACK|ROWCOUNT|ROWGUIDCOL|RULE|SAVE|SCHEMA|SECURITYAUDIT|SHUTDOWN|SOME|STATISTICS|TABLE|TABLESAMPLE|TEXTSIZE|THEN|TO|TRAN|TRANSACTION|TRIGGER|TSEQUAL|UNION|UNIQUE|UNPIVOT|UPDATETEXT|USE|USER|VALUES|VARYING|VIEW|WAITFOR|WHEN|WHILE|WRITETEXT)\b", RegexOptions.IgnoreCase | RegexCompiledOption);
            SQLFunctionsRegex = new Regex(@"(@@CONNECTIONS|@@CPU_BUSY|@@CURSOR_ROWS|@@DATEFIRST|@@DATEFIRST|@@DBTS|@@ERROR|@@FETCH_STATUS|@@IDENTITY|@@IDLE|@@IO_BUSY|@@LANGID|@@LANGUAGE|@@LOCK_TIMEOUT|@@MAX_CONNECTIONS|@@MAX_PRECISION|@@NESTLEVEL|@@OPTIONS|@@PACKET_ERRORS|@@PROCID|@@REMSERVER|@@ROWCOUNT|@@SERVERNAME|@@SERVICENAME|@@SPID|@@TEXTSIZE|@@TRANCOUNT|@@VERSION)\b|\b(ABS|ACOS|APP_NAME|ASCII|ASIN|ASSEMBLYPROPERTY|AsymKey_ID|ASYMKEY_ID|asymkeyproperty|ASYMKEYPROPERTY|ATAN|ATN2|AVG|CASE|CAST|CEILING|Cert_ID|Cert_ID|CertProperty|CHAR|CHARINDEX|CHECKSUM_AGG|COALESCE|COL_LENGTH|COL_NAME|COLLATIONPROPERTY|COLLATIONPROPERTY|COLUMNPROPERTY|COLUMNS_UPDATED|COLUMNS_UPDATED|CONTAINSTABLE|CONVERT|COS|COT|COUNT|COUNT_BIG|CRYPT_GEN_RANDOM|CURRENT_TIMESTAMP|CURRENT_TIMESTAMP|CURRENT_USER|CURRENT_USER|CURSOR_STATUS|DATABASE_PRINCIPAL_ID|DATABASE_PRINCIPAL_ID|DATABASEPROPERTY|DATABASEPROPERTYEX|DATALENGTH|DATALENGTH|DATEADD|DATEDIFF|DATENAME|DATEPART|DAY|DB_ID|DB_NAME|DECRYPTBYASYMKEY|DECRYPTBYCERT|DECRYPTBYKEY|DECRYPTBYKEYAUTOASYMKEY|DECRYPTBYKEYAUTOCERT|DECRYPTBYPASSPHRASE|DEGREES|DENSE_RANK|DIFFERENCE|ENCRYPTBYASYMKEY|ENCRYPTBYCERT|ENCRYPTBYKEY|ENCRYPTBYPASSPHRASE|ERROR_LINE|ERROR_MESSAGE|ERROR_NUMBER|ERROR_PROCEDURE|ERROR_SEVERITY|ERROR_STATE|EVENTDATA|EXP|FILE_ID|FILE_IDEX|FILE_NAME|FILEGROUP_ID|FILEGROUP_NAME|FILEGROUPPROPERTY|FILEPROPERTY|FLOOR|fn_helpcollations|fn_listextendedproperty|fn_servershareddrives|fn_virtualfilestats|fn_virtualfilestats|FORMATMESSAGE|FREETEXTTABLE|FULLTEXTCATALOGPROPERTY|FULLTEXTSERVICEPROPERTY|GETANSINULL|GETDATE|GETUTCDATE|GROUPING|HAS_PERMS_BY_NAME|HOST_ID|HOST_NAME|IDENT_CURRENT|IDENT_CURRENT|IDENT_INCR|IDENT_INCR|IDENT_SEED|IDENTITY\(|INDEX_COL|INDEXKEY_PROPERTY|INDEXPROPERTY|IS_MEMBER|IS_OBJECTSIGNED|IS_SRVROLEMEMBER|ISDATE|ISDATE|ISNULL|ISNUMERIC|Key_GUID|Key_GUID|Key_ID|Key_ID|KEY_NAME|KEY_NAME|LEFT|LEN|LOG|LOG10|LOWER|LTRIM|MAX|MIN|MONTH|NCHAR|NEWID|NTILE|NULLIF|OBJECT_DEFINITION|OBJECT_ID|OBJECT_NAME|OBJECT_SCHEMA_NAME|OBJECTPROPERTY|OBJECTPROPERTYEX|OPENDATASOURCE|OPENQUERY|OPENROWSET|OPENXML|ORIGINAL_LOGIN|ORIGINAL_LOGIN|PARSENAME|PATINDEX|PATINDEX|PERMISSIONS|PI|POWER|PUBLISHINGSERVERNAME|PWDCOMPARE|PWDENCRYPT|QUOTENAME|RADIANS|RAND|RANK|REPLICATE|REVERSE|RIGHT|ROUND|ROW_NUMBER|ROWCOUNT_BIG|RTRIM|SCHEMA_ID|SCHEMA_ID|SCHEMA_NAME|SCHEMA_NAME|SCOPE_IDENTITY|SERVERPROPERTY|SESSION_USER|SESSION_USER|SESSIONPROPERTY|SETUSER|SIGN|SignByAsymKey|SignByCert|SIN|SOUNDEX|SPACE|SQL_VARIANT_PROPERTY|SQRT|SQUARE|STATS_DATE|STDEV|STDEVP|STR|STUFF|SUBSTRING|SUM|SUSER_ID|SUSER_NAME|SUSER_SID|SUSER_SNAME|SWITCHOFFSET|SYMKEYPROPERTY|symkeyproperty|sys\.dm_db_index_physical_stats|sys\.fn_builtin_permissions|sys\.fn_my_permissions|SYSDATETIME|SYSDATETIMEOFFSET|SYSTEM_USER|SYSTEM_USER|SYSUTCDATETIME|TAN|TERTIARY_WEIGHTS|TEXTPTR|TODATETIMEOFFSET|TRIGGER_NESTLEVEL|TYPE_ID|TYPE_NAME|TYPEPROPERTY|UNICODE|UPDATE\(|UPPER|USER_ID|USER_NAME|USER_NAME|VAR|VARP|VerifySignedByAsymKey|VerifySignedByCert|XACT_STATE|YEAR)\b", RegexOptions.IgnoreCase | RegexCompiledOption);
            SQLTypesRegex =
                new Regex(
                    @"\b(BIGINT|NUMERIC|BIT|SMALLINT|DECIMAL|SMALLMONEY|INT|TINYINT|MONEY|FLOAT|REAL|DATE|DATETIMEOFFSET|DATETIME2|SMALLDATETIME|DATETIME|TIME|CHAR|VARCHAR|TEXT|NCHAR|NVARCHAR|NTEXT|BINARY|VARBINARY|IMAGE|TIMESTAMP|HIERARCHYID|TABLE|UNIQUEIDENTIFIER|SQL_VARIANT|XML)\b",
                    RegexOptions.IgnoreCase | RegexCompiledOption);
        }

        /// <summary>
        /// Highlights SQL code
        /// </summary>
        /// <param name="range"></param>
        public virtual void SQLSyntaxHighlight(TextSelectionRange range)
        {
            range.tb.CommentPrefix = "--";
            range.tb.LeftBracket = '(';
            range.tb.RightBracket = ')';
            range.tb.LeftBracket2 = '\x0';
            range.tb.RightBracket2 = '\x0';
            range.tb.LeftBracket3 = '\x0';
            range.tb.RightBracket3 = '\x0';

            range.tb.AutoIndentCharsPatterns = @"";
            //clear style of changed range
            range.ClearStyle(CommentStyle, StringStyle, NumberStyle, VariableStyle, StatementsStyle, KeywordStyle,
                             FunctionsStyle, TypesStyle);
            //
            if (SQLStringRegex == null)
                InitSQLRegex();
            //comment highlighting
            range.SetStyle(CommentStyle, SQLCommentRegex1);
            range.SetStyle(CommentStyle, SQLCommentRegex2);
            range.SetStyle(CommentStyle, SQLCommentRegex3);
            range.SetStyle(CommentStyle, SQLCommentRegex4);
            //string highlighting
            range.SetStyle(StringStyle, SQLStringRegex);
            //number highlighting
            range.SetStyle(NumberStyle, SQLNumberRegex);
            //types highlighting
            range.SetStyle(TypesStyle, SQLTypesRegex);
            //var highlighting
            range.SetStyle(VariableStyle, SQLVarRegex);
            //statements
            range.SetStyle(StatementsStyle, SQLStatementsRegex);
            //keywords
            range.SetStyle(KeywordStyle, SQLKeywordsRegex);
            //functions
            range.SetStyle(FunctionsStyle, SQLFunctionsRegex);

            //clear folding markers
            range.ClearFoldingMarkers();
            //set folding markers
            range.SetFoldingMarkers(@"\bBEGIN\b", @"\bEND\b", RegexOptions.IgnoreCase);
            //allow to collapse BEGIN..END blocks
            range.SetFoldingMarkers(@"/\*", @"\*/"); //allow to collapse comment block
        }

        protected void InitPHPRegex()
        {
            PHPStringRegex = new Regex(@"""""|''|"".*?[^\\]""|'.*?[^\\]'", RegexCompiledOption);
            PHPNumberRegex = new Regex(@"\b\d+[\.]?\d*\b", RegexCompiledOption);
            PHPCommentRegex1 = new Regex(@"(//|#).*$", RegexOptions.Multiline | RegexCompiledOption);
            PHPCommentRegex2 = new Regex(@"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline | RegexCompiledOption);
            PHPCommentRegex3 = new Regex(@"(/\*.*?\*/)|(.*\*/)",
                                         RegexOptions.Singleline | RegexOptions.RightToLeft | RegexCompiledOption);
            PHPVarRegex = new Regex(@"\$[a-zA-Z_\d]*\b", RegexCompiledOption);
            PHPKeywordRegex1 =
                new Regex(
                    @"\b(die|echo|empty|exit|eval|include|include_once|isset|list|require|require_once|return|print|unset)\b",
                    RegexCompiledOption);
            PHPKeywordRegex2 =
                new Regex(
                    @"\b(abstract|and|array|as|break|case|catch|cfunction|class|clone|const|continue|declare|default|do|else|elseif|enddeclare|endfor|endforeach|endif|endswitch|endwhile|extends|final|for|foreach|function|global|goto|if|implements|instanceof|interface|namespace|new|or|private|protected|public|static|switch|throw|try|use|var|while|xor)\b",
                    RegexCompiledOption);
            PHPKeywordRegex3 = new Regex(@"__CLASS__|__DIR__|__FILE__|__LINE__|__FUNCTION__|__METHOD__|__NAMESPACE__",
                                         RegexCompiledOption);
        }

        /// <summary>
        /// Highlights PHP code
        /// </summary>
        /// <param name="range"></param>
        public virtual void PHPSyntaxHighlight(TextSelectionRange range)
        {
            range.tb.CommentPrefix = "//";
            range.tb.LeftBracket = '(';
            range.tb.RightBracket = ')';
            range.tb.LeftBracket2 = '{';
            range.tb.RightBracket2 = '}';
            range.tb.LeftBracket3 = '\x0';
            range.tb.RightBracket3 = '\x0';
            range.tb.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;
            //clear style of changed range
            range.ClearStyle(StringStyle, CommentStyle, NumberStyle, VariableStyle, KeywordStyle, KeywordStyle2,
                             KeywordStyle3);

            range.tb.AutoIndentCharsPatterns
                = @"
^\s*\$[\w\.\[\]\'\""]+\s*(?<range>=)\s*(?<range>[^;]+);
";

            //
            if (PHPStringRegex == null)
                InitPHPRegex();
            //string highlighting
            range.SetStyle(StringStyle, PHPStringRegex);
            //comment highlighting
            range.SetStyle(CommentStyle, PHPCommentRegex1);
            range.SetStyle(CommentStyle, PHPCommentRegex2);
            range.SetStyle(CommentStyle, PHPCommentRegex3);
            //number highlighting
            range.SetStyle(NumberStyle, PHPNumberRegex);
            //var highlighting
            range.SetStyle(VariableStyle, PHPVarRegex);
            //keyword highlighting
            range.SetStyle(KeywordStyle, PHPKeywordRegex1);
            range.SetStyle(KeywordStyle2, PHPKeywordRegex2);
            range.SetStyle(KeywordStyle3, PHPKeywordRegex3);

            //clear folding markers
            range.ClearFoldingMarkers();
            //set folding markers
            range.SetFoldingMarkers("{", "}"); //allow to collapse brackets block
            range.SetFoldingMarkers(@"/\*", @"\*/"); //allow to collapse comment block
        }

        protected void InitJScriptRegex()
        {
            JScriptStringRegex = new Regex(@"""""|''|"".*?[^\\]""|'.*?[^\\]'", RegexCompiledOption);
            JScriptCommentRegex1 = new Regex(@"//.*$", RegexOptions.Multiline | RegexCompiledOption);
            JScriptCommentRegex2 = new Regex(@"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline | RegexCompiledOption);
            JScriptCommentRegex3 = new Regex(@"(/\*.*?\*/)|(.*\*/)",
                                             RegexOptions.Singleline | RegexOptions.RightToLeft | RegexCompiledOption);
            JScriptNumberRegex = new Regex(@"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b",
                                           RegexCompiledOption);
            JScriptKeywordRegex =
                new Regex(
                    @"\b(true|false|break|case|catch|const|continue|default|delete|do|else|export|for|function|if|in|instanceof|new|null|return|switch|this|throw|try|var|void|while|with|typeof)\b",
                    RegexCompiledOption);
        }

        /// <summary>
        /// Highlights JavaScript code
        /// </summary>
        /// <param name="range"></param>
        public virtual void JScriptSyntaxHighlight(TextSelectionRange range)
        {
            range.tb.CommentPrefix = "//";
            range.tb.LeftBracket = '(';
            range.tb.RightBracket = ')';
            range.tb.LeftBracket2 = '{';
            range.tb.RightBracket2 = '}';
            range.tb.LeftBracket3 = '\x0';
            range.tb.RightBracket3 = '\x0';
            range.tb.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;

            range.tb.AutoIndentCharsPatterns
                = @"
^\s*[\w\.]+(\s\w+)?\s*(?<range>=)\s*(?<range>[^;]+);
";

            //clear style of changed range
            range.ClearStyle(StringStyle, CommentStyle, NumberStyle, KeywordStyle);
            //
            if (JScriptStringRegex == null)
                InitJScriptRegex();
            //string highlighting
            range.SetStyle(StringStyle, JScriptStringRegex);
            //comment highlighting
            range.SetStyle(CommentStyle, JScriptCommentRegex1);
            range.SetStyle(CommentStyle, JScriptCommentRegex2);
            range.SetStyle(CommentStyle, JScriptCommentRegex3);
            //number highlighting
            range.SetStyle(NumberStyle, JScriptNumberRegex);
            //keyword highlighting
            range.SetStyle(KeywordStyle, JScriptKeywordRegex);
            //clear folding markers
            range.ClearFoldingMarkers();
            //set folding markers
            range.SetFoldingMarkers("{", "}"); //allow to collapse brackets block
            range.SetFoldingMarkers(@"/\*", @"\*/"); //allow to collapse comment block
        }

        protected void InitLuaRegex()
        {
            LuaStringRegex = new Regex(@"""""|''|"".*?[^\\]""|'.*?[^\\]'", RegexCompiledOption);
            LuaCommentRegex1 = new Regex(@"--.*$", RegexOptions.Multiline | RegexCompiledOption);
            LuaCommentRegex2 = new Regex(@"(--\[\[.*?\]\])|(--\[\[.*)", RegexOptions.Singleline | RegexCompiledOption);
            LuaCommentRegex3 = new Regex(@"(--\[\[.*?\]\])|(.*\]\])",
                                             RegexOptions.Singleline | RegexOptions.RightToLeft | RegexCompiledOption);
            LuaNumberRegex = new Regex(@"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b",
                                           RegexCompiledOption);
            LuaKeywordRegex =
                new Regex(
                    @"\b(and|break|do|else|elseif|end|false|for|function|if|in|local|nil|not|or|repeat|return|then|true|until|while)\b",
                    RegexCompiledOption);

            LuaFunctionsRegex =
                new Regex(
                    @"\b(assert|collectgarbage|dofile|error|getfenv|getmetatable|ipairs|load|loadfile|loadstring|module|next|pairs|pcall|print|rawequal|rawget|rawset|require|select|setfenv|setmetatable|tonumber|tostring|type|unpack|xpcall)\b",
                    RegexCompiledOption);
        }

        /// <summary>
        /// Highlights Lua code
        /// </summary>
        /// <param name="range"></param>
        public virtual void LuaSyntaxHighlight(TextSelectionRange range)
        {
            range.tb.CommentPrefix = "--";
            range.tb.LeftBracket = '(';
            range.tb.RightBracket = ')';
            range.tb.LeftBracket2 = '{';
            range.tb.RightBracket2 = '}';
            range.tb.LeftBracket3 = '\x0';
            range.tb.RightBracket3 = '\x0';
            range.tb.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;

            range.tb.AutoIndentCharsPatterns
                = @"
^\s*[\w\.]+(\s\w+)?\s*(?<range>=)\s*(?<range>.+)
";

            //clear style of changed range
            range.ClearStyle(StringStyle, CommentStyle, NumberStyle, KeywordStyle, FunctionsStyle);
            //
            if (LuaStringRegex == null)
                InitLuaRegex();
            //string highlighting
            range.SetStyle(StringStyle, LuaStringRegex);
            //comment highlighting
            range.SetStyle(CommentStyle, LuaCommentRegex1);
            range.SetStyle(CommentStyle, LuaCommentRegex2);
            range.SetStyle(CommentStyle, LuaCommentRegex3);
            //number highlighting
            range.SetStyle(NumberStyle, LuaNumberRegex);
            //keyword highlighting
            range.SetStyle(KeywordStyle, LuaKeywordRegex);
            //functions highlighting
            range.SetStyle(FunctionsStyle, LuaFunctionsRegex);
            //clear folding markers
            range.ClearFoldingMarkers();
            //set folding markers
            range.SetFoldingMarkers("{", "}"); //allow to collapse brackets block
            range.SetFoldingMarkers(@"--\[\[", @"\]\]"); //allow to collapse comment block
        }

        protected void LuaAutoIndentNeeded(object sender, AutoIndentEventArgs args)
        {
            //end of block
            if (AiRegex(ref aiLuaBlockEnd, @"^\s*(end|until)\b").IsMatch(args.LineText))
            {
                args.Shift = -args.TabLength;
                args.ShiftNextLines = -args.TabLength;
                return;
            }
            // then ...
            if (AiRegex(ref aiLuaThen, @"\b(then)\s*\S+").IsMatch(args.LineText))
                return;
            //start of operator block
            if (AiRegex(ref aiLuaBlockStart, @"^\s*(function|do|for|while|repeat|if)\b").IsMatch(args.LineText))
            {
                args.ShiftNextLines = args.TabLength;
                return;
            }

            //Statements else, elseif, case etc
            if (AiRegex(ref aiLuaElse, @"^\s*(else|elseif)\b", RegexOptions.IgnoreCase).IsMatch(args.LineText))
            {
                args.Shift = -args.TabLength;
                return;
            }
        }

        protected void InitJSONRegex()
        {
            JSONStringRegex = new Regex(@"""([^\\""]|\\"")*""", RegexCompiledOption);
            JSONNumberRegex = new Regex(@"\b(\d+[\.]?\d*|true|false|null)\b", RegexCompiledOption);
            JSONKeywordRegex = new Regex(@"(?<range>""([^\\""]|\\"")*"")\s*:", RegexCompiledOption);
        }

        /// <summary>
        /// Highlights JSON code
        /// </summary>
        /// <param name="range"></param>
        public virtual void JSONSyntaxHighlight(TextSelectionRange range)
        {
            range.tb.LeftBracket = '[';
            range.tb.RightBracket = ']';
            range.tb.LeftBracket2 = '{';
            range.tb.RightBracket2 = '}';
            range.tb.LeftBracket3 = '\x0';
            range.tb.RightBracket3 = '\x0';
            range.tb.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;

            range.tb.AutoIndentCharsPatterns
                = @"
^\s*[\w\.]+(\s\w+)?\s*(?<range>=)\s*(?<range>[^;]+);
";

            //clear style of changed range
            range.ClearStyle(StringStyle, NumberStyle, KeywordStyle);
            //
            if (JSONStringRegex == null)
                InitJSONRegex();
            //keyword highlighting
            range.SetStyle(KeywordStyle, JSONKeywordRegex);
            //string highlighting
            range.SetStyle(StringStyle, JSONStringRegex);
            //number highlighting
            range.SetStyle(NumberStyle, JSONNumberRegex);
            //clear folding markers
            range.ClearFoldingMarkers();
            //set folding markers
            range.SetFoldingMarkers("{", "}"); //allow to collapse brackets block
            range.SetFoldingMarkers(@"\[", @"\]"); //allow to collapse comment block
        }

        protected void InitMarkdownRegex()
        {
            //Line constructs are matched at the position where the line content starts, so none of
            //them is anchored with '^' (the caller checks Match.Index). Only the fence constructs
            //are anchored: they are about the whole line.

            //#{1,6} followed by whitespace: '#tag' and a lone '#' are not headings
            MdHeadingRegex = new Regex(@"#{1,6}(?=[ \t])[^\n]*", RegexCompiledOption);
            //thematic break: 3+ of the same char, spaces allowed between them ("- - -", "***")
            //but the line must hold nothing else
            MdHrRegex = new Regex(@"(?<c>[-*_])(?:[ \t]*\k<c>){2,}[ \t]*$", RegexCompiledOption);
            MdQuoteRegex = new Regex(@"(?: {0,3}>[ \t]?)+", RegexCompiledOption);
            //list bullet / task checkbox; the checkbox is optional and its own group so it can be
            //styled separately
            MdListRegex = new Regex(@"(?<bullet>[-*+]|\d{1,9}[.)])[ \t]+(?<task>\[[ xX]\][ \t]+)?", RegexCompiledOption);
            //fenced code block: 3+ backticks or tildes, indented by at most 3 spaces. A backtick
            //info string may not contain a backtick, a closing fence carries nothing else.
            MdFenceOpenRegex = new Regex(@"^[ ]{0,3}(?<fence>`{3,}|~{3,})[ \t]*(?<lang>[^`\n]*)$", RegexCompiledOption);
            MdFenceCloseRegex = new Regex(@"^[ ]{0,3}(?<fence>`{3,}|~{3,})[ \t]*$", RegexCompiledOption);

            //Inline constructs. The control renders only the first TextStyle of a char, so these are
            //applied in CommonMark precedence order: the first one applied is the one that shows.
            //Strong, both **x** and __x__; the (?<!\\) guards keep a backslash-escaped run literal.
            MdBoldRegex = new Regex(@"(\*\*|__)(?<!\\)(?=\S)[^\n]+?(?<!\\)\1", RegexCompiledOption);
            //Emphasis with *. The opener may not be part of ** and may not be followed by
            //whitespace, which is what keeps "2 * 3 * 4" plain, while the CommonMark examples
            //"*em*", "a*b*c" and "2*3*4" still emphasize.
            MdItalicRegex = new Regex(@"(?<![\*\\])\*(?!\s)[^*\n]+?(?<!\s)\*(?!\*)", RegexCompiledOption);
            //Emphasis with _. Intraword underscores are not emphasis, so snake_case stays plain.
            MdUnderscoreItalicRegex = new Regex(@"(?<![\w\\])_(?!\s)[^_\n]+?(?<!\s)_(?!\w)", RegexCompiledOption);
            MdStrikethroughRegex = new Regex(@"~~(?=\S)[^~\n]+?(?<!\\)~~", RegexCompiledOption);
            MdInlineCodeRegex = new Regex(@"`[^`\n]+`", RegexCompiledOption);
            //(?<!!): an image ![alt](url) must not also be styled as a link
            MdLinkRegex = new Regex(@"(?<!!)\[(?<text>[^\]\n]+)\]\((?<url>[^)\n]*)\)", RegexCompiledOption);
            MdImageRegex = new Regex(@"!\[(?<text>[^\]\n]*)\]\((?<url>[^)\n]*)\)", RegexCompiledOption);
            //<https://host/path> and <user@host>
            MdAutoLinkRegex = new Regex(@"<(?<url>(?:[a-z][a-z0-9+.-]*://|mailto:)[^>\s]+|[^ >@\[\]\n]+@[^ >@\[\]\n]+)>",
                                        RegexCompiledOption | RegexOptions.IgnoreCase);
            //table delimiter row: cells of hyphens, optionally with a leading/trailing colon
            MdTableSeparatorRegex = new Regex(@"^[ \t]*\|?[ \t]*:?-+:?[ \t]*(?:\|[ \t]*:?-+:?[ \t]*)*\|?[ \t]*$",
                                              RegexCompiledOption);
        }

        /// <summary>
        /// Highlights Markdown text
        /// </summary>
        /// <remarks>
        /// Line oriented by design: markdown is a container format, so the highlighter walks the
        /// lines of the range keeping the fenced-code-block state, and inline constructs are only
        /// matched inside lines that are not code. That is what keeps "# not a heading", "**not
        /// bold**" and "[not a link](x)" inside a fence from being highlighted as markdown.
        /// </remarks>
        /// <param name="range"></param>
        public virtual void MarkdownSyntaxHighlight(TextSelectionRange range)
        {
            var tb = range.tb;
            range.Normalize();

            if (MdHeadingRegex == null)
                InitMarkdownRegex();

            ClearMarkdownStyles(range);

            //code blocks seen inside the range, highlighted with their own language afterwards
            var codeBlocks = new List<(int startLine, int endLine, Language language)>();
            StyleMarkdownLines(tb, range, codeBlocks);

            //highlight the content of every fence with the language named in its info string
            foreach (var (startLine, endLine, language) in codeBlocks)
            {
                if (language == Language.Custom || language == Language.Markdown)
                    continue;
                //the nested highlighters paint with the shared style slots, which only exist once
                //InitStyleSchema ran for that language; every built-in schema sets StringStyle or
                //CommentStyle, so this does not touch a host that set its styles up itself
                if (StringStyle == null && CommentStyle == null)
                    InitStyleSchema(language);
                HighlightSyntax(language, new TextSelectionRange(tb, 0, startLine, tb[endLine].Count, endLine));
            }

            //control-wide state is set last: the nested highlighters above overwrite it
            tb.CommentPrefix = null;
            tb.LeftBracket = '(';
            tb.RightBracket = ')';
            tb.LeftBracket2 = '[';
            tb.RightBracket2 = ']';
            tb.LeftBracket3 = '\x0';
            tb.RightBracket3 = '\x0';
            tb.AutoIndentCharsPatterns = @"";

            //clear folding markers
            range.ClearFoldingMarkers();
            //set folding markers for fenced code blocks
            range.SetFoldingMarkers(@"^(?:`{3,}|~{3,})", RegexOptions.Multiline);
        }

        /// <summary>
        /// Styles a document as markdown prose on the character grid: the constructs get their
        /// colours, their source markers are hidden, and bare urls / local paths become link coloured.
        /// </summary>
        /// <remarks>
        /// This is the reusable half of <see cref="MarkdownSyntaxHighlight"/> for a host that shows a
        /// markdown body inside a document of its own (a chat transcript, a notes pane): it never
        /// clears folding markers and never rewrites the bracket / auto-indent state, so the host
        /// keeps its own styles and folding. Fenced blocks are painted as code but not highlighted
        /// with their language (the host does that), and the cell padding of tables - the only way a
        /// character grid can line columns up - is an edit, so it only runs when asked for.
        /// </remarks>
        /// <param name="range">Range to style</param>
        /// <param name="alignTables">Pad markdown tables with spaces so their columns line up, and hide their cell separators</param>
        public virtual void MarkdownBodyHighlight(TextSelectionRange range, bool alignTables = true)
        {
            if (range == null)
                return;
            var tb = range.tb;
            if (MdHeadingRegex == null)
                InitMarkdownRegex();
            if (MdH1Style == null)
                InitStyleSchema(Language.Markdown);

            //padding the cells rewrites the buffer, so the old range now describes text that is gone
            if (alignTables && AlignMarkdownTables(range) > 0)
                range = tb.Range;
            range.Normalize();

            ClearMarkdownStyles(range);
            StyleMarkdownLines(tb, range, new List<(int, int, Language)>());
            StyleBareLinks(tb, range);
            //the padding is what aligns the columns; the cell separators are source syntax, so
            //they are hidden like the other markers and the rows read as plain aligned columns
            if (alignTables)
                HideTableSeparators(tb, range);
        }

        private void ClearMarkdownStyles(TextSelectionRange range)
            => range.ClearStyle(MdH1Style, MdH2Style, MdH3Style, MdHeadingStyle,
                                MdBoldStyle, MdItalicStyle, MdUnderscoreItalicStyle, MdStrikethroughStyle,
                                MdInlineCodeStyle, MdCodeBlockStyle, MdLinkStyle, MdAutoLinkStyle,
                                MdImageStyle, MdBlockquoteStyle, MdHrStyle, MdListStyle, MdTaskListDoneStyle,
                                MdMarkerStyle);

        /// <summary>
        /// The line pass shared by the document and the body entry; code blocks are reported to the
        /// caller, which alone knows whether it wants to highlight their language.
        /// </summary>
        private void StyleMarkdownLines(FastColoredTextBox tb, TextSelectionRange range,
                                        List<(int startLine, int endLine, Language language)> codeBlocks)
        {
            int fromLine = Math.Max(0, range.Start.iLine);
            int toLine = Math.Min(range.End.iLine, tb.LinesCount - 1);

            var fence = new FenceState();
            RewindFenceState(tb, fromLine, fence);

            for (int iLine = fromLine; iLine <= toLine; iLine++)
            {
                Line line = tb[iLine];
                string text = line.Text;
                bool isCode = FenceStep(text, fence, iLine, codeBlocks);

                int from = iLine == fromLine ? range.Start.iChar : 0;
                int to = iLine == toLine ? Math.Min(range.End.iChar, line.Count) : line.Count;
                if (to <= from)
                    continue;

                if (isCode)
                {
                    new TextSelectionRange(tb, from, iLine, to, iLine).SetStyle(MdCodeBlockStyle);
                    continue;
                }

                HighlightMarkdownLine(tb, text, from, to, iLine);
            }
            //an unterminated fence runs to the end of the document, as in CommonMark
            if (fence.Inside)
                codeBlocks.Add((Math.Max(fence.StartLine, fromLine), toLine, fence.Language));
        }

        /// <summary>
        /// Gives bare urls and local paths the link style. The syntax forms (<c>[label](url)</c>,
        /// <c>&lt;url&gt;</c>) already carry it from the inline pass, and text inside an inline code
        /// span is literal, so it keeps the code style there.
        /// </summary>
        private void StyleBareLinks(FastColoredTextBox tb, TextSelectionRange range)
        {
            if (MdLinkStyle == null)
                return;
            int fromLine = Math.Max(0, range.Start.iLine);
            int toLine = Math.Min(range.End.iLine, tb.LinesCount - 1);

            foreach (var link in MarkdownLinkScanner.Collect(tb.Text))
            {
                if (link.FromSyntax || link.Line < fromLine || link.Line > toLine)
                    continue;
                Line line = tb[link.Line];
                if (link.Start + link.Length > line.Count)
                    continue;
                var span = new TextSelectionRange(tb, link.Start, link.Line, link.Start + link.Length, link.Line);
                if (ContainsStyle(span, MdInlineCodeStyle))
                    continue;
                span.SetStyle(MdLinkStyle);
            }
        }

        private static bool ContainsStyle(TextSelectionRange range, Style style)
        {
            if (style == null)
                return false;
            for (int iLine = range.Start.iLine; iLine <= range.End.iLine; iLine++)
            {
                Line line = range.tb[iLine];
                int from = iLine == range.Start.iLine ? range.Start.iChar : 0;
                int to = iLine == range.End.iLine ? range.End.iChar : line.Count;
                for (int i = from; i < to && i < line.Count; i++)
                    if (line[i].HasStyle(style))
                        return true;
            }
            return false;
        }

        /// <summary>
        /// Applies every markdown style that fits a single line of prose.
        /// </summary>
        /// <remarks>
        /// A construct that is rendered as formatted text also gets its source markers hidden
        /// (<see cref="MdMarkerStyle"/>), so a heading reads as its title and <c>**bold**</c> as
        /// bold. Markers that are themselves the rendered element (list bullets, task checkboxes,
        /// rules, fences) stay visible: hiding them would remove the element instead of the syntax.
        /// </remarks>
        /// <param name="tb"></param>
        /// <param name="text">Text of the whole line</param>
        /// <param name="from">First char of the line inside the highlighted range</param>
        /// <param name="to">Char after the last one</param>
        /// <param name="iLine">Line index</param>
        private void HighlightMarkdownLine(FastColoredTextBox tb, string text, int from, int to, int iLine)
        {
            int lineTo = to - from;
            int content = SkipIndent(text, 0);
            TextSelectionRange Line(int start, int end) => new(tb, from + start, iLine, from + Math.Min(end, lineTo), iLine);
            void Hide(int start, int end)
            {
                if (end > start)
                    Line(start, end).SetStyle(MdMarkerStyle);
            }

            //blockquote: the bar covers the marker too, the content is parsed behind it. The style
            //starts at column 0 so the bar is drawn on every quoted line, not only on unindented ones
            var quote = MdQuoteRegex.Match(text, content);
            if (quote.Success && quote.Index == content)
            {
                Line(0, lineTo).SetStyle(MdBlockquoteStyle);
                Hide(content, quote.Index + quote.Length);
                content = SkipIndent(text, quote.Index + quote.Length);
            }
            if (content >= lineTo)
                return;

            //heading: the text carries the heading style, the #'s and the space around them are hidden
            var heading = MdHeadingRegex.Match(text, content);
            if (heading.Success && heading.Index == content)
            {
                int headingEnd = content + heading.Length;
                int end = headingEnd;
                while (end > content && (text[end - 1] == '#' || text[end - 1] == ' ' || text[end - 1] == '\t'))
                    end--;
                int level = 0;
                while (level < heading.Length && text[content + level] == '#')
                    level++;
                int start = content + level;
                while (start < end && (text[start] == ' ' || text[start] == '\t'))
                    start++;
                var style = level switch
                {
                    1 => MdH1Style,
                    2 => MdH2Style,
                    3 => MdH3Style,
                    _ => MdHeadingStyle
                };
                if (end > start)
                    Line(start, end).SetStyle(style);
                Hide(content, start);
                Hide(end, headingEnd);
                //an inline construct inside the heading still gets its markers hidden, so
                //"## **title**" reads as the title; the inline pass below stays inside the heading
                content = start;
                lineTo = Math.Min(lineTo, end);
            }
            else
            {
                //thematic break: checked before the list so that "- - -" is a rule, not a bullet
                var rule = MdHrRegex.Match(text, content);
                if (rule.Success && rule.Index == content && rule.Index + rule.Length == text.Length)
                {
                    Line(content, text.Length).SetStyle(MdHrStyle);
                    return;
                }

                //list: the bullet gets the list colour, the task checkbox its own
                var list = MdListRegex.Match(text, content);
                if (list.Success && list.Index == content)
                {
                    var bullet = list.Groups["bullet"];
                    Line(bullet.Index, bullet.Index + bullet.Length).SetStyle(MdListStyle);
                    content = SkipIndent(text, bullet.Index + bullet.Length);
                    var task = list.Groups["task"];
                    if (task.Success)
                    {
                        int end = task.Index + task.Value.TrimEnd(' ', '\t').Length;
                        //only a ticked box is worth pointing at, an open one keeps the plain text
                        if (task.Value[1] != ' ')
                            Line(task.Index, end).SetStyle(MdTaskListDoneStyle);
                        content = SkipIndent(text, end);
                    }
                }
            }
            if (content >= lineTo)
                return;

            //inline constructs, outermost first: the control shows the first TextStyle of a char
            var inline = Line(content, lineTo);
            inline.SetStyle(MdInlineCodeStyle, MdInlineCodeRegex);
            inline.SetStyle(MdImageStyle, MdImageRegex);
            inline.SetStyle(MdLinkStyle, MdLinkRegex);
            inline.SetStyle(MdAutoLinkStyle, MdAutoLinkRegex);
            inline.SetStyle(MdBoldStyle, MdBoldRegex);
            inline.SetStyle(MdItalicStyle, MdItalicRegex);
            inline.SetStyle(MdUnderscoreItalicStyle, MdUnderscoreItalicRegex);
            inline.SetStyle(MdStrikethroughStyle, MdStrikethroughRegex);

            //hide the delimiters of what was just formatted: the styled text is what should show
            string tail = text.Substring(content, lineTo - content);
            HideInlineMarkers(tail, (start, end) => Hide(content + start, content + end));
        }

        /// <summary>
        /// Reports the chars of a prose fragment that the inline pass hides: the delimiters of an
        /// inline code span, bold / italic / strikethrough, the angle brackets of an autolink, and
        /// the syntax around the label of a link or image (the label itself stays visible).
        /// </summary>
        /// <remarks>
        /// Shared with the table formatter, which pads a column to the width the cell renders at:
        /// counting the source markers would misplace every pipe once they disappear. A construct
        /// matched inside a code span is literal text there, so its markers stay.
        /// </remarks>
        private void HideInlineMarkers(string text, Action<int, int> hide)
        {
            var codeSpans = new List<Match>();
            foreach (Match m in MdInlineCodeRegex.Matches(text))
                codeSpans.Add(m);
            bool InCode(Match m)
            {
                foreach (var span in codeSpans)
                    if (m.Index >= span.Index && m.Index + m.Length <= span.Index + span.Length)
                        return true;
                return false;
            }
            //the two delimiters of an inline construct (lead = opener length, trail = closer)
            void HideDelimiters(Match m, int lead, int trail)
            {
                hide(m.Index, m.Index + lead);
                hide(m.Index + m.Length - trail, m.Index + m.Length);
            }
            //everything of an inline construct except its label group (links and images)
            void HideSyntax(Match m, Group label)
            {
                hide(m.Index, label.Index);
                hide(label.Index + label.Length, m.Index + m.Length);
            }

            foreach (var m in codeSpans)
                HideDelimiters(m, 1, 1);
            foreach (Match m in MdImageRegex.Matches(text))
                if (!InCode(m))
                    HideSyntax(m, m.Groups["text"]);
            foreach (Match m in MdLinkRegex.Matches(text))
                if (!InCode(m))
                    HideSyntax(m, m.Groups["text"]);
            foreach (Match m in MdAutoLinkRegex.Matches(text))
                if (!InCode(m))
                    HideDelimiters(m, 1, 1);
            foreach (Match m in MdBoldRegex.Matches(text))
                if (!InCode(m))
                    HideDelimiters(m, 2, 2);
            foreach (Match m in MdItalicRegex.Matches(text))
                if (!InCode(m))
                    HideDelimiters(m, 1, 1);
            foreach (Match m in MdUnderscoreItalicRegex.Matches(text))
                if (!InCode(m))
                    HideDelimiters(m, 1, 1);
            foreach (Match m in MdStrikethroughRegex.Matches(text))
                if (!InCode(m))
                    HideDelimiters(m, 2, 2);
        }

        /// <summary>
        /// Returns the position after the block indent of a line: at most 3 spaces, a deeper indent
        /// is an indented code block and must not match any block construct
        /// </summary>
        private static int SkipIndent(string text, int i)
        {
            int skipped = 0;
            while (skipped < 3 && i < text.Length && text[i] == ' ')
            {
                i++;
                skipped++;
            }
            return i;
        }

        /// <summary>
        /// Runs the fenced-code-block state machine over one line.
        /// Returns true when the line belongs to a code block.
        /// </summary>
        private bool FenceStep(string text, FenceState fence, int iLine, List<(int, int, Language)> codeBlocks)
        {
            if (fence.Inside)
            {
                var close = MdFenceCloseRegex.Match(text);
                if (!close.Success || close.Groups["fence"].Value[0] != fence.Char
                                  || close.Groups["fence"].Value.Length < fence.Length)
                    return true;

                //an unterminated block above the highlighted range has an unknown fence, so only
                //blocks that opened inside the range can be reported for nested highlighting
                if (fence.Char != '\0')
                    codeBlocks.Add((fence.StartLine, iLine, fence.Language));
                fence.Inside = false;
                fence.StartLine = -1;
                return true;
            }

            var open = MdFenceOpenRegex.Match(text);
            if (!open.Success)
                return false;

            fence.Char = open.Groups["fence"].Value[0];
            fence.Length = open.Groups["fence"].Value.Length;
            fence.StartLine = iLine;
            fence.Inside = true;
            fence.Language = LanguageDetector.StringToLanguage(open.Groups["lang"].Value);
            return true;
        }

        /// <summary>
        /// Restores the fenced-code-block state for a highlight pass that starts in the middle of the
        /// document: the nearest fence line above decides, an opening one meaning we are inside a
        /// block and a closing one meaning we are not.
        /// </summary>
        private void RewindFenceState(FastColoredTextBox tb, int fromLine, FenceState fence)
        {
            //ponytail: 2000 lines of lookback. A fence that opens further up than that is shown as
            //markdown until the caret moves back into it; raise the limit if that ever bites
            int limit = Math.Max(0, fromLine - 2000);
            //"```" with nothing after it is both an opener and a closer, so the next fence line up
            //is needed to tell which one it was
            bool bareFenceSeen = false;

            for (int i = fromLine - 1; i >= limit; i--)
            {
                Line line = tb[i];
                if (line.Count == 0)
                    continue;
                //a fence line starts with at most 3 spaces and then a backtick or a tilde
                int j = 0;
                while (j < 3 && j < line.Count && line[j].C == ' ')
                    j++;
                if (j >= line.Count || (line[j].C != '`' && line[j].C != '~'))
                    continue;

                var open = MdFenceOpenRegex.Match(line.Text);
                if (!open.Success)
                    continue;

                string info = open.Groups["lang"].Value;
                if (info.Length == 0)
                {
                    bareFenceSeen = true;
                    continue;
                }

                //a fence with an info string is always an opener, so the bare fence below it (if
                //any) was its closer and we are outside a block
                if (bareFenceSeen)
                    return;

                fence.Char = open.Groups["fence"].Value[0];
                fence.Length = open.Groups["fence"].Value.Length;
                fence.Language = LanguageDetector.StringToLanguage(info);
                fence.Inside = true;
                //the opener is above the range: only the part inside the range gets re-highlighted
                fence.StartLine = fromLine;
                return;
            }

            if (bareFenceSeen)
            {
                //a bare fence with nothing above it opened a block that was never closed
                fence.Inside = true;
                fence.StartLine = fromLine;
            }
        }

        /// <summary>
        /// Fenced-code-block state kept while walking the lines of a highlight pass
        /// </summary>
        private sealed class FenceState
        {
            /// <summary>Fence character of the opening fence, '\0' when it is above the range</summary>
            public char Char;
            public int Length;
            public Language Language = Language.Custom;
            public bool Inside;
            public int StartLine = -1;
        }

        /// <summary>
        /// Indents the line after a list item so nested content keeps its place.
        /// </summary>
        /// <remarks>
        /// AutoIndentEventArgs can only add or remove leading whitespace, so this indents instead of
        /// copying the "- " marker text, and a blockquote gets nothing (its continuation is not
        /// indented). Copying the marker needs a key handler (tb.ProcessKey), not an auto-indent hook.
        /// </remarks>
        protected void MarkdownAutoIndentNeeded(object sender, AutoIndentEventArgs args)
        {
            if (args.Shift != 0 || args.LineText == null)
                return;

            int start = SkipIndent(args.LineText, 0);
            var list = MdListRegex.Match(args.LineText, start);
            if (!list.Success || list.Index != start)
                return;

            //a thematic break looks like a bullet but is not a list
            var rule = MdHrRegex.Match(args.LineText, start);
            if (rule.Success && rule.Index == start && rule.Index + rule.Length == args.LineText.Length)
                return;

            args.ShiftNextLines = args.TabLength;
        }

        #region Table alignment

        /// <summary>
        /// Rewrites the markdown tables of the range so their columns line up, and returns how many
        /// tables were reformatted.
        /// </summary>
        /// <remarks>
        /// Alignment has to happen in the text: the control renders on a character grid, so a table
        /// is lined up by padding its cells with spaces. A table whose header is in the range is
        /// taken whole and replaced in one edit (one undo restores it); formatting an already
        /// formatted table changes nothing. Each column is aligned the way its delimiter cell says
        /// (<c>:--</c> left, <c>:--:</c> center, <c>--:</c> right), and a CJK char counts as two
        /// columns wide so Chinese and latin cells line up. A cell is measured as it renders, so
        /// the inline markers the highlighter hides (<c>**bold**</c>, a code span, link syntax)
        /// take no room and the padding stays right once they disappear. A control with
        /// <see cref="FastColoredTextBox.AllowProportionalFont"/> has no character grid, so there the
        /// cells are measured with the control's own char metrics and padded in space widths: the
        /// columns line up as closely as whole spaces allow.
        /// </remarks>
        /// <param name="range">Range holding the tables to format</param>
        /// <returns>Number of tables formatted</returns>
        public virtual int AlignMarkdownTables(TextSelectionRange range)
        {
            if (range == null)
                return 0;
            var tb = range.tb;
            if (MdTableSeparatorRegex == null)
                InitMarkdownRegex();

            range.Normalize();
            int fromLine = Math.Max(0, range.Start.iLine);
            int toLine = Math.Min(range.End.iLine, tb.LinesCount - 1);
            int formatted = 0;

            //bottom-up: replacing a table changes the line count below it, the lines above stay put
            for (int iLine = toLine; iLine >= fromLine; iLine--)
            {
                if (!IsTableStart(tb, iLine))
                    continue;
                int lastLine = iLine + 1;
                while (lastLine + 1 < tb.LinesCount && IsTableRow(tb[lastLine + 1].Text))
                    lastLine++;
                if (FormatTable(tb, iLine, lastLine))
                    formatted++;
            }

            return formatted;
        }

        /// <summary>
        /// True when the line is a table header, i.e. the line below it is a delimiter row.
        /// </summary>
        /// <remarks>
        /// The delimiter row has to carry a pipe of its own: "a" over "---" is a setext heading,
        /// not a one-column table.
        /// </remarks>
        private bool IsTableStart(FastColoredTextBox tb, int iLine)
        {
            if (iLine + 1 >= tb.LinesCount)
                return false;
            string separator = tb[iLine + 1].Text;
            return tb[iLine].Text.IndexOf('|') >= 0
                   && separator.IndexOf('|') >= 0
                   && MdTableSeparatorRegex.IsMatch(separator);
        }

        /// <summary>True when the line can be a row of a table that is already open</summary>
        private static bool IsTableRow(string text) => text.Trim().Length > 0 && text.IndexOf('|') >= 0;

        /// <summary>
        /// Hides every cell separator of the tables in the range, leading pipe included: with the
        /// padding in place the rows read as plain aligned columns, and the text stays a valid
        /// markdown table (so a later pass can align it again).
        /// </summary>
        private void HideTableSeparators(FastColoredTextBox tb, TextSelectionRange range)
        {
            int fromLine = Math.Max(0, range.Start.iLine);
            int toLine = Math.Min(range.End.iLine, tb.LinesCount - 1);

            for (int iLine = fromLine; iLine <= toLine; iLine++)
            {
                if (!IsTableStart(tb, iLine))
                    continue;
                //a table inside a fence is code: its pipes are not table syntax
                if (ContainsStyle(new TextSelectionRange(tb, 0, iLine, tb[iLine].Count, iLine), MdCodeBlockStyle))
                    continue;
                int lastLine = iLine + 1;
                while (lastLine + 1 < tb.LinesCount && IsTableRow(tb[lastLine + 1].Text))
                    lastLine++;
                for (int r = iLine; r <= lastLine; r++)
                    HideRowSeparators(tb, r);
                iLine = lastLine;
            }
        }

        /// <summary>Hides every pipe of the row; an escaped or code-span pipe is cell content.</summary>
        private void HideRowSeparators(FastColoredTextBox tb, int iLine)
        {
            Line line = tb[iLine];
            bool inCode = false;
            for (int i = 0; i < line.Count; i++)
            {
                char c = line[i].C;
                if (c == '\\')
                {
                    i++;
                    continue;
                }
                if (c == '`')
                    inCode = !inCode;
                if (c != '|' || inCode)
                    continue;
                new TextSelectionRange(tb, i, iLine, i + 1, iLine).SetStyle(MdMarkerStyle);
            }
        }

        /// <summary>
        /// Rewrites one table. Returns false when it already was aligned, so a command that opens a
        /// document of aligned tables does not push undo entries that change nothing.
        /// </summary>
        private bool FormatTable(FastColoredTextBox tb, int firstLine, int lastLine)
        {
            int rowCount = lastLine - firstLine + 1;
            var rows = new List<string[]>(rowCount);
            for (int i = 0; i < rowCount; i++)
                rows.Add(SplitTableRow(tb[firstLine + i].Text));

            //a short row is padded with empty cells and a long one widens the table: dropping the
            //extra cells (as GFM rendering does) would delete text the user typed
            int columns = 1;
            foreach (var row in rows)
                columns = Math.Max(columns, row.Length);

            //the delimiter needs room for at least "---"
            var widths = new int[columns];
            var cellWidths = new int[rowCount][];
            var alignment = new int[columns];
            for (int c = 0; c < rows[1].Length && c < columns; c++)
            {
                string cell = rows[1][c];
                bool left = cell.StartsWith(":");
                bool right = cell.EndsWith(":");
                alignment[c] = left && right ? 2 : right ? 3 : left ? 1 : 0;
            }
            for (int r = 0; r < rowCount; r++)
            {
                cellWidths[r] = new int[columns];
                for (int c = 0; c < rows[r].Length && c < columns; c++)
                    cellWidths[r][c] = CellWidth(tb, rows[r][c]);
            }
            for (int c = 0; c < columns; c++)
            {
                widths[c] = MinColumnWidth(tb, alignment[c]);
                for (int r = 0; r < rowCount; r++)
                    widths[c] = Math.Max(widths[c], cellWidths[r][c]);
            }

            var sb = new StringBuilder();
            for (int r = 0; r < rowCount; r++)
            {
                if (r > 0)
                    sb.Append('\n');
                sb.Append('|');
                for (int c = 0; c < columns; c++)
                {
                    string cell = c < rows[r].Length ? rows[r][c] : "";
                    sb.Append(r == 1 ? SeparatorCell(tb, widths[c], alignment[c])
                                     : ContentCell(cell, widths[c], alignment[c], cellWidths[r][c]));
                    sb.Append('|');
                }
            }

            var block = new TextSelectionRange(tb, 0, firstLine, tb[lastLine].Count, lastLine);
            string text = sb.ToString();
            if (block.Text == text)
                return false;

            ReplaceRange(block, text);
            return true;
        }

        /// <summary>
        /// Splits a table row into its cells: pipes in a code span or escaped with a backslash are
        /// cell content, the pipe at each end is optional.
        /// </summary>
        private static string[] SplitTableRow(string text)
        {
            int from = 0;
            int to = text.Length;
            while (from < to && text[from] == ' ')
                from++;
            while (to > from && text[to - 1] == ' ')
                to--;
            if (from < to && text[from] == '|')
                from++;
            if (to > from && text[to - 1] == '|' && !IsEscaped(text, to - 1))
                to--;

            var cells = new List<string>();
            var sb = new StringBuilder();
            bool inCode = false;
            for (int i = from; i < to; i++)
            {
                char c = text[i];
                if (c == '\\' && i + 1 < to)
                {
                    sb.Append(c).Append(text[++i]);
                    continue;
                }
                if (c == '`')
                    inCode = !inCode;
                if (c == '|' && !inCode)
                {
                    cells.Add(sb.ToString().Trim());
                    sb.Clear();
                    continue;
                }
                sb.Append(c);
            }
            cells.Add(sb.ToString().Trim());
            return cells.ToArray();
        }

        /// <summary>True when the char at <paramref name="i"/> is preceded by an odd run of backslashes</summary>
        private static bool IsEscaped(string text, int i)
        {
            int backslashes = 0;
            for (int j = i - 1; j >= 0 && text[j] == '\\'; j--)
                backslashes++;
            return backslashes % 2 == 1;
        }

        /// <summary>Width of a cell in columns; a CJK char takes two, an escape pair renders as one</summary>
        private static int DisplayWidth(string text)
        {
            int width = 0;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '\\' && i + 1 < text.Length)
                {
                    i++;
                    width++;
                    continue;
                }
                width += EncodingDetector.IsCJK(c) ? 2 : 1;
            }
            return width;
        }

        /// <summary>
        /// Narrowest column that still holds the delimiter row: "---" plus the colons of its
        /// alignment. In a proportional font that is wider than three spaces, so the column grows.
        /// </summary>
        private static int MinColumnWidth(FastColoredTextBox tb, int alignment)
        {
            if (!tb.AllowProportionalFont)
                return 3;
            int colon = alignment is 1 or 2 or 3 ? tb.GetCharWidth(':') : 0;
            int dash = Math.Max(1, tb.GetCharWidth('-'));
            int room = 3 * dash + (alignment is 1 or 2 ? colon : 0) + (alignment is 2 or 3 ? colon : 0);
            return Math.Max(3, (int)Math.Ceiling(room / (double)PaddingWidth(tb)));
        }

        /// <summary>
        /// Width of a cell in padding units: character cells for a monospaced font, spaces for a
        /// proportional one. Measured on what the cell renders as: the inline markers the
        /// highlighter hides take no room, so counting them would leave every pipe of the column
        /// one marker width off.
        /// </summary>
        private int CellWidth(FastColoredTextBox tb, string text)
        {
            string visible = VisibleCellText(text);
            if (!tb.AllowProportionalFont)
                return DisplayWidth(visible);
            int unit = PaddingWidth(tb);
            return (int)Math.Round(MeasuredWidth(tb, visible) / (double)unit);
        }

        /// <summary>The text a cell renders as: the inline markers the highlighter hides are dropped</summary>
        private string VisibleCellText(string text)
        {
            if (text.Length == 0)
                return text;
            var hidden = new bool[text.Length];
            HideInlineMarkers(text, (start, end) =>
            {
                for (int i = Math.Max(0, start); i < Math.Min(end, hidden.Length); i++)
                    hidden[i] = true;
            });
            var sb = new StringBuilder(text.Length);
            for (int i = 0; i < text.Length; i++)
                if (!hidden[i])
                    sb.Append(text[i]);
            return sb.ToString();
        }

        /// <summary>Pixel width of a cell as the control draws it: an escape pair renders as one char</summary>
        private static int MeasuredWidth(FastColoredTextBox tb, string text)
        {
            int width = 0;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '\\' && i + 1 < text.Length)
                    c = text[++i];
                width += tb.GetCharWidth(c);
            }
            return width;
        }

        /// <summary>Pixel width of one padding space</summary>
        private static int PaddingWidth(FastColoredTextBox tb) => Math.Max(1, tb.GetCharWidth(' '));

        /// <summary>Cell of a content row, padded to the column width and placed by its alignment</summary>
        private static string ContentCell(string text, int width, int alignment, int cellWidth)
        {
            int pad = Math.Max(0, width - cellWidth);
            int left = alignment == 2 ? pad / 2 : alignment == 3 ? pad : 0;
            return " " + new string(' ', left) + text + new string(' ', pad - left) + " ";
        }

        /// <summary>
        /// Delimiter cell (<c>---</c>, <c>:--</c>, <c>:--:</c>, <c>--:</c>) padded like a content cell
        /// </summary>
        private static string SeparatorCell(FastColoredTextBox tb, int width, int alignment)
        {
            if (!tb.AllowProportionalFont)
                return alignment switch
                {
                    1 => " :" + new string('-', width - 1) + " ",
                    2 => " :" + new string('-', width - 2) + ": ",
                    3 => " " + new string('-', width - 1) + ": ",
                    _ => " " + new string('-', width) + " ",
                };

            //proportional: a dash is not a space wide, so the count comes from the pixels the column
            //has to fill, otherwise the delimiter row drifts away from the pipes above it
            bool left = alignment is 1 or 2;
            bool right = alignment is 2 or 3;
            int colon = tb.GetCharWidth(':');
            int dash = Math.Max(1, tb.GetCharWidth('-'));
            int room = width * PaddingWidth(tb) - (left ? colon : 0) - (right ? colon : 0);
            int dashes = Math.Max(3, (int)Math.Round(room / (double)dash));
            return " " + (left ? ":" : "") + new string('-', dashes) + (right ? ":" : "") + " ";
        }

        /// <summary>
        /// Replaces a range and puts the selection back where it was, shifted by the length change.
        /// </summary>
        private static void ReplaceRange(TextSelectionRange range, string text)
        {
            var tb = range.tb;
            int start = tb.PlaceToPosition(tb.Selection.Start);
            int end = tb.PlaceToPosition(tb.Selection.End);
            int at = tb.PlaceToPosition(range.Start);
            int delta = text.Length - range.Length;

            tb.Selection.BeginUpdate();
            tb.Selection = range;
            tb.InsertText(text, false);
            tb.Selection.Start = tb.PositionToPlace(start >= at ? start + delta : start);
            tb.Selection.End = tb.PositionToPlace(end >= at ? end + delta : end);
            tb.Selection.EndUpdate();
        }

        #endregion

        #region Styles

        /// <summary>
        /// String style
        /// </summary>
        public Style StringStyle { get; set; }

        /// <summary>
        /// Comment style
        /// </summary>
        public Style CommentStyle { get; set; }

        /// <summary>
        /// Number style
        /// </summary>
        public Style NumberStyle { get; set; }

        /// <summary>
        /// C# attribute style
        /// </summary>
        public Style AttributeStyle { get; set; }

        /// <summary>
        /// Class name style
        /// </summary>
        public Style ClassNameStyle { get; set; }

        /// <summary>
        /// Keyword style
        /// </summary>
        public Style KeywordStyle { get; set; }

        /// <summary>
        /// Style of tags in comments of C#
        /// </summary>
        public Style CommentTagStyle { get; set; }

        /// <summary>
        /// HTML attribute value style
        /// </summary>
        public Style AttributeValueStyle { get; set; }

        /// <summary>
        /// HTML tag brackets style
        /// </summary>
        public Style TagBracketStyle { get; set; }

        /// <summary>
        /// HTML tag name style
        /// </summary>
        public Style TagNameStyle { get; set; }

        /// <summary>
        /// HTML Entity style
        /// </summary>
        public Style HtmlEntityStyle { get; set; }

        /// <summary>
        /// XML attribute style
        /// </summary>
        public Style XmlAttributeStyle { get; set; }

        /// <summary>
        /// XML attribute value style
        /// </summary>
        public Style XmlAttributeValueStyle { get; set; }

        /// <summary>
        /// XML tag brackets style
        /// </summary>
        public Style XmlTagBracketStyle { get; set; }

        /// <summary>
        /// XML tag name style
        /// </summary>
        public Style XmlTagNameStyle { get; set; }

        /// <summary>
        /// XML Entity style
        /// </summary>
        public Style XmlEntityStyle { get; set; }

        /// <summary>
        /// XML CData style
        /// </summary>
        public Style XmlCDataStyle { get; set; }

        /// <summary>
        /// Variable style
        /// </summary>
        public Style VariableStyle { get; set; }

        /// <summary>
        /// Specific PHP keyword style
        /// </summary>
        public Style KeywordStyle2 { get; set; }

        /// <summary>
        /// Specific PHP keyword style
        /// </summary>
        public Style KeywordStyle3 { get; set; }

        /// <summary>
        /// SQL Statements style
        /// </summary>
        public Style StatementsStyle { get; set; }

        /// <summary>
        /// SQL Functions style
        /// </summary>
        public Style FunctionsStyle { get; set; }

        /// <summary>
        /// SQL Types style
        /// </summary>
        public Style TypesStyle { get; set; }

        /// <summary>
        /// Markdown H1 style
        /// </summary>
        public Style MdH1Style { get; set; }

        /// <summary>
        /// Markdown H2 style
        /// </summary>
        public Style MdH2Style { get; set; }

        /// <summary>
        /// Markdown H3 style
        /// </summary>
        public Style MdH3Style { get; set; }

        /// <summary>
        /// Markdown H4-H6 style
        /// </summary>
        public Style MdHeadingStyle { get; set; }

        /// <summary>
        /// Markdown bold style
        /// </summary>
        public Style MdBoldStyle { get; set; }

        /// <summary>
        /// Markdown italic style (*text*)
        /// </summary>
        public Style MdItalicStyle { get; set; }

        /// <summary>
        /// Markdown italic style (_text_)
        /// </summary>
        public Style MdUnderscoreItalicStyle { get; set; }

        /// <summary>
        /// Markdown strikethrough style
        /// </summary>
        public Style MdStrikethroughStyle { get; set; }

        /// <summary>
        /// Markdown inline code style
        /// </summary>
        public Style MdInlineCodeStyle { get; set; }

        /// <summary>
        /// Markdown fenced code block style, a <see cref="MarkdownBlockStyle"/>
        /// </summary>
        public Style MdCodeBlockStyle { get; set; }

        /// <summary>
        /// Markdown link style
        /// </summary>
        public Style MdLinkStyle { get; set; }

        /// <summary>
        /// Markdown autolink style (&lt;https://host&gt;)
        /// </summary>
        public Style MdAutoLinkStyle { get; set; }

        /// <summary>
        /// Markdown image style
        /// </summary>
        public Style MdImageStyle { get; set; }

        /// <summary>
        /// Markdown blockquote style, a <see cref="MarkdownBlockStyle"/>
        /// </summary>
        public Style MdBlockquoteStyle { get; set; }

        /// <summary>
        /// Markdown horizontal rule style
        /// </summary>
        public Style MdHrStyle { get; set; }

        /// <summary>
        /// Markdown list bullet style
        /// </summary>
        public Style MdListStyle { get; set; }

        /// <summary>
        /// Markdown task list checkbox style, applied to the [x] of a finished item
        /// </summary>
        public Style MdTaskListDoneStyle { get; set; }

        /// <summary>
        /// Applied to the source markers of a rendered construct (<c>#</c>, <c>**</c>,
        /// <c>[text](url)</c>, <c>&gt;</c>): a char carrying it is not painted and takes no width,
        /// a <see cref="MarkdownMarkerStyle"/>. Kept in one instance so a restyled document never
        /// piles up generations of it on the chars.
        /// </summary>
        public Style MdMarkerStyle { get; } = new MarkdownMarkerStyle();

        #endregion Styles
    }
}
