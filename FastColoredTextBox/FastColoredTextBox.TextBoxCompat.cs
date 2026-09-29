using System.ComponentModel;
using System.Text;
using FastColoredTextBoxNS.Types;

namespace FastColoredTextBoxNS
{
    /// <summary>
    /// Members of <see cref="System.Windows.Forms.TextBox"/> that FastColoredTextBox was missing.
    /// </summary>
    /// <remarks>
    /// Everything a WinForms form needs in order to swap a <see cref="System.Windows.Forms.TextBox"/>
    /// for a <see cref="FastColoredTextBox"/> without touching the call site lives here, so the
    /// compatibility surface stays greppable.
    /// </remarks>
    public partial class FastColoredTextBox
    {
        private int maxLength;
        private char passwordChar;
        private bool useSystemPasswordChar;

        #region TextBox members

        /// <summary>
        /// Maximum number of characters the user can enter. 0 means unlimited.
        /// </summary>
        /// <remarks>
        /// Like <see cref="System.Windows.Forms.TextBoxBase.MaxLength"/> this only limits typing;
        /// assigning <see cref="Text"/>/<see cref="SelectedText"/> or pasting is never truncated.
        /// </remarks>
        [DefaultValue(0)]
        [Description("Maximum number of characters the user can enter. 0 means unlimited.")]
        public int MaxLength
        {
            get => maxLength;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(MaxLength), "MaxLength cannot be negative.");
                if (maxLength == value)
                    return;
                maxLength = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Casing applied to every character that enters the control (typed or inserted).
        /// </summary>
        [DefaultValue(System.Windows.Forms.CharacterCasing.Normal)]
        [Description("Casing applied to every character that enters the control.")]
        public System.Windows.Forms.CharacterCasing CharacterCasing { get; set; } = System.Windows.Forms.CharacterCasing.Normal;

        /// <summary>
        /// True when there is something to undo.
        /// </summary>
        [Browsable(false)]
        public bool CanUndo => lines.Manager.UndoEnabled;

        /// <summary>
        /// Gets/sets whether the text has been modified since the control was created or the last
        /// time <see cref="Modified"/> was set to false. Mirrors <see cref="IsChanged"/>.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(false)]
        public bool Modified
        {
            get => IsChanged;
            set => IsChanged = value;
        }

        /// <summary>
        /// Hides the selection while the control does not have the keyboard focus.
        /// </summary>
        [DefaultValue(true)]
        [Description("Hides the selection while the control does not have the keyboard focus.")]
        public bool HideSelection { get; set; } = true;

        /// <summary>
        /// Replaces every rendered char with this one. '\0' (the default) shows the real text.
        /// </summary>
        [DefaultValue('\0')]
        [Description("Replaces every rendered char with this one. '\\0' shows the real text.")]
        public char PasswordChar
        {
            get => passwordChar;
            set
            {
                if (passwordChar == value)
                    return;
                passwordChar = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Uses the system password char ('●') to mask the text.
        /// </summary>
        [DefaultValue(false)]
        [Description("Uses the system password char to mask the text.")]
        public bool UseSystemPasswordChar
        {
            get => useSystemPasswordChar;
            set
            {
                if (useSystemPasswordChar == value)
                    return;
                useSystemPasswordChar = value;
                PasswordChar = value ? '●' : '\0';
            }
        }

        /// <summary>
        /// Hint drawn while the control is empty.
        /// </summary>
        [DefaultValue("")]
        [Description("Hint drawn while the control is empty.")]
        public string PlaceholderText { get; set; } = "";

        /// <summary>
        /// Height of a single line of text, for AutoSize/anchor arithmetic.
        /// </summary>
        [Browsable(false)]
        public int PreferredHeight => Math.Max(CharHeight, Font.Height) + Paddings.Vertical;

        /// <summary>
        /// Shows/hides the scrollbars.
        /// </summary>
        /// <remarks>
        /// ponytail: <see cref="System.Windows.Forms.ScrollBars.Vertical"/> and
        /// <see cref="System.Windows.Forms.ScrollBars.Horizontal"/> behave as
        /// <see cref="System.Windows.Forms.ScrollBars.Both"/> - the scrollbars are driven by
        /// <see cref="ScrollableControl.AutoScroll"/> and cannot be shown one axis at a time.
        /// Override the per-axis Visible flags in <see cref="OnCharSizeChanged"/> if that is ever needed.
        /// </remarks>
        [DefaultValue(typeof(System.Windows.Forms.ScrollBars), "Both")]
        [Description("Shows/hides the scrollbars.")]
        public System.Windows.Forms.ScrollBars ScrollBars
        {
            get => ShowScrollBars ? System.Windows.Forms.ScrollBars.Both : System.Windows.Forms.ScrollBars.None;
            set => ShowScrollBars = value != System.Windows.Forms.ScrollBars.None;
        }

        /// <summary>
        /// Enables the hotkeys listed in <see cref="HotkeysMapping"/>.
        /// </summary>
        [DefaultValue(true)]
        [Description("Enables the hotkeys listed in HotkeysMapping.")]
        public bool ShortcutsEnabled { get; set; } = true;

        /// <summary>
        /// Horizontal alignment of the text. Only meaningful for a single-line control.
        /// </summary>
        [DefaultValue(HorizontalAlignment.Left)]
        [Description("Horizontal alignment of the text. Only meaningful for a single-line control.")]
        public HorizontalAlignment TextAlign { get; set; } = HorizontalAlignment.Left;

        /// <summary>
        /// Enables the built-in WinForms word completion.
        /// </summary>
        /// <remarks>
        /// <see cref="System.Windows.Forms.AutoCompleteMode.Append"/> and
        /// <see cref="System.Windows.Forms.AutoCompleteMode.SuggestAppend"/> complete the word before
        /// the caret when a delimiter is typed; SuggestAppend leaves the completed part selected so the
        /// next keystroke overwrites it. The inline ghost hint drawn by TextBox for
        /// <see cref="System.Windows.Forms.AutoCompleteMode.Suggest"/> is not reproduced - use
        /// <see cref="AutocompleteMenu"/> for that. Word completion, like in TextBox, is single-line only.
        /// </remarks>
        [DefaultValue(System.Windows.Forms.AutoCompleteMode.None)]
        [Description("Enables the built-in WinForms word completion.")]
        public System.Windows.Forms.AutoCompleteMode AutoCompleteMode { get; set; } = System.Windows.Forms.AutoCompleteMode.None;

        /// <summary>
        /// Where <see cref="AutoCompleteMode"/> takes its candidates from.
        /// </summary>
        [DefaultValue(System.Windows.Forms.AutoCompleteSource.ListItems)]
        [Description("Where AutoCompleteMode takes its candidates from.")]
        public System.Windows.Forms.AutoCompleteSource AutoCompleteSource { get; set; } = System.Windows.Forms.AutoCompleteSource.ListItems;

        /// <summary>
        /// Candidate list used when <see cref="AutoCompleteSource"/> is
        /// <see cref="System.Windows.Forms.AutoCompleteSource.CustomSource"/>.
        /// </summary>
        [Browsable(false)]
        public AutoCompleteStringCollection AutoCompleteCustomSource { get; set; }

        #endregion

        #region helpers

        /// <summary>
        /// Total length of the document, without allocating a selection range like
        /// <see cref="TextLength"/> does. Used per keystroke by <see cref="MaxLength"/>.
        /// </summary>
        private int TextLengthFast
        {
            get
            {
                if (lines.Count == 0)
                    return 0;

                //counts like TextLength does: every line but the last is followed by one newline char
                int result = lines.Count - 1;
                for (int i = 0; i < lines.Count; i++)
                    result += lines[i].Count;
                return result;
            }
        }

        /// <summary>
        /// Offset applied to the text origin for <see cref="TextAlign"/>. Zero unless the control is
        /// single-line, un-wrapped and not left-aligned.
        /// </summary>
        private int TextAlignOffset
        {
            get
            {
                if (TextAlign == HorizontalAlignment.Left || multiline || wordWrap || lines.Count != 1)
                    return 0;

                int available = ClientSize.Width - LeftIndent - Paddings.Left - Paddings.Right;
                int content = lines[0].Count * CharWidth;
                if (available <= 0 || content >= available)
                    return 0;

                return TextAlign == HorizontalAlignment.Center
                    ? (available - content) / 2
                    : available - content;
            }
        }

        /// <summary>
        /// Applies <see cref="CharacterCasing"/> to text entering the control.
        /// </summary>
        private string ApplyCasing(string text) => CharacterCasing switch
        {
            System.Windows.Forms.CharacterCasing.Upper => text.ToUpperInvariant(),
            System.Windows.Forms.CharacterCasing.Lower => text.ToLowerInvariant(),
            _ => text
        };

        private char ApplyCasing(char c) => CharacterCasing switch
        {
            System.Windows.Forms.CharacterCasing.Upper => char.ToUpperInvariant(c),
            System.Windows.Forms.CharacterCasing.Lower => char.ToLowerInvariant(c),
            _ => c
        };

        /// <summary>
        /// Completes the word before the caret when a delimiter has just been typed.
        /// </summary>
        private void AutoCompleteWord()
        {
            if (multiline || AutoCompleteMode is not (System.Windows.Forms.AutoCompleteMode.Append or System.Windows.Forms.AutoCompleteMode.SuggestAppend))
                return;

            // ponytail: linear scan of the candidate list on every delimiter key. The lists WinForms
            // autocomplete was designed for are small; index them if someone completes a dictionary.
            IEnumerable<string> candidates = AutoCompleteSource == System.Windows.Forms.AutoCompleteSource.CustomSource
                ? AutoCompleteCustomSource?.Cast<string>()
                : lines.GetLines();

            if (candidates == null)
                return;

            // the delimiter that has just been inserted sits right before the caret: step over it
            int iLine = Selection.Start.iLine;
            int wordEnd = Selection.Start.iChar - 1;
            var line = lines[iLine];
            while (wordEnd >= 0 && !IsWordChar(line[wordEnd].C))
                wordEnd--;
            if (wordEnd < 0)
                return; // nothing typed before the delimiter

            int wordStart = wordEnd;
            while (wordStart > 0 && IsWordChar(line[wordStart - 1].C))
                wordStart--;

            var word = new StringBuilder(wordEnd - wordStart + 1);
            for (int i = wordStart; i <= wordEnd; i++)
                word.Append(line[i].C);
            string typed = word.ToString();

            string match = null;
            foreach (string candidate in candidates)
                if (candidate != null && candidate.Length > typed.Length &&
                    candidate.StartsWith(typed, StringComparison.CurrentCultureIgnoreCase))
                {
                    match = candidate;
                    break;
                }

            if (match == null)
                return;

            //only the missing tail is appended, like TextBox SuggestAppend does
            string tail = match.Substring(typed.Length);
            int insertAt = wordEnd + 1;
            Selection.Start = new Place(insertAt, iLine);
            Selection.End = new Place(insertAt, iLine);
            InsertText(tail);
            if (AutoCompleteMode == System.Windows.Forms.AutoCompleteMode.SuggestAppend)
            {
                Selection.Start = new Place(insertAt, iLine);
                Selection.End = new Place(insertAt + tail.Length, iLine);
            }
            Invalidate();
        }

        private static bool IsWordChar(char c) => char.IsLetterOrDigit(c) || c == '_';

        #endregion
    }
}
