using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using FastColoredTextBoxNS.Types;
using Xunit;

namespace FastColoredTextBoxNS.Tests
{
    /// <summary>
    /// TextBox migration tests: the members a WinForms form touches on a
    /// <see cref="TextBox"/> must exist on <see cref="FastColoredTextBox"/> with the same
    /// type and accessor, and must actually do something.
    /// </summary>
    public class TextBoxCompatTests
    {
        private static FastColoredTextBox NewBox()
        {
            var tb = new FastColoredTextBox { Multiline = false };
            tb.Text = "";
            tb.Selection.Start = new Place(0, 0);
            tb.Selection.End = new Place(0, 0);
            return tb;
        }

        private static void Type(FastColoredTextBox tb, string s)
        {
            foreach (char c in s)
                tb.ProcessKey(c, Keys.None);
        }

        #region API parity

        /// <summary>
        /// The regression guard for the migration itself: any public instance property of
        /// <see cref="TextBox"/> must exist on FastColoredTextBox with a compatible type and
        /// at least the same public accessors, otherwise migrated code stops compiling.
        /// </summary>
        [Fact]
        public void EveryPublicTextBoxProperty_IsPresentOnFastColoredTextBox()
        {
            var textBoxProps = typeof(TextBox)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .GroupBy(p => p.Name)
                .ToDictionary(g => g.Key, g => g.First());
            var fctbProps = typeof(FastColoredTextBox)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .GroupBy(p => p.Name)
                .ToDictionary(g => g.Key, g => g.First());

            var problems = new List<string>();
            foreach (var (name, expected) in textBoxProps)
            {
                if (!fctbProps.TryGetValue(name, out var actual))
                {
                    problems.Add($"missing property {name} : {expected.PropertyType.Name}");
                    continue;
                }

                if (expected.PropertyType != actual.PropertyType)
                    problems.Add($"{name} : {expected.PropertyType.Name} != {actual.PropertyType.Name}");
                if (expected.GetGetMethod(false) != null && actual.GetGetMethod(false) == null)
                    problems.Add($"{name} : public getter missing");
                if (expected.GetSetMethod(false) != null && actual.GetSetMethod(false) == null)
                    problems.Add($"{name} : public setter missing");
            }

            Assert.Empty(problems);
        }

        /// <summary>
        /// The ones that could not be added by inheritance only, listed explicitly so a
        /// regression names the offending member instead of just failing the sweep above.
        /// </summary>
        [Fact]
        public void TextBoxMembersAddedForMigration_AreAllUsable()
        {
            var expected = new[]
            {
                "AutoCompleteCustomSource", "AutoCompleteMode", "AutoCompleteSource", "CanUndo",
                "CharacterCasing", "HideSelection", "MaxLength", "Modified", "PasswordChar",
                "PlaceholderText", "PreferredHeight", "ScrollBars", "ShortcutsEnabled",
                "TextAlign", "UseSystemPasswordChar"
            };

            var fctbProps = typeof(FastColoredTextBox)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => p.Name)
                .ToHashSet();

            Assert.DoesNotContain(expected, name => !fctbProps.Contains(name));
        }
        #endregion

        #region MaxLength

        [Fact]
        public void MaxLength_BlocksTypingPastTheLimit()
        {
            StaRunner.Run(() =>
            {
                using var tb = NewBox();
                tb.MaxLength = 3;

                Type(tb, "abcd");

                Assert.Equal("abc", tb.Text);
            });
        }

        [Fact]
        public void MaxLength_DoesNotTruncateAssignedText()
        {
            StaRunner.Run(() =>
            {
                using var tb = NewBox();
                tb.MaxLength = 3;

                // TextBox does not truncate programmatic assignment either
                tb.Text = "abcdef";

                Assert.Equal("abcdef", tb.Text);
            });
        }

        [Fact]
        public void MaxLength_RejectsNegativeValues()
        {
            StaRunner.Run(() =>
            {
                using var tb = NewBox();
                Assert.Throws<ArgumentOutOfRangeException>(() => tb.MaxLength = -1);
            });
        }
        #endregion

        #region CharacterCasing

        [Theory]
        [InlineData(CharacterCasing.Upper, "ABC")]
        [InlineData(CharacterCasing.Lower, "abc")]
        [InlineData(CharacterCasing.Normal, "AbC")]
        public void CharacterCasing_AppliesToTypedText(CharacterCasing casing, string expected)
        {
            StaRunner.Run(() =>
            {
                using var tb = NewBox();
                tb.CharacterCasing = casing;

                Type(tb, "AbC");

                Assert.Equal(expected, tb.Text);
            });
        }

        [Fact]
        public void ForceUpperCase_IsShorthandForCharacterCasingUpper()
        {
            StaRunner.Run(() =>
            {
                using var tb = NewBox();
                tb.ForceUpperCase = true;

                Assert.Equal(CharacterCasing.Upper, tb.CharacterCasing);
                Type(tb, "ab");
                Assert.Equal("AB", tb.Text);

                tb.ForceUpperCase = false;
                Assert.Equal(CharacterCasing.Normal, tb.CharacterCasing);
            });
        }
        #endregion

        #region CanUndo / Modified / HideSelection

        [Fact]
        public void CanUndo_FollowsTheUndoHistory()
        {
            StaRunner.Run(() =>
            {
                using var tb = NewBox();
                tb.ClearUndo();
                Assert.False(tb.CanUndo);

                Type(tb, "x");

                Assert.True(tb.CanUndo);
                tb.ClearUndo();
                Assert.False(tb.CanUndo);
            });
        }

        [Fact]
        public void Modified_MirrorsIsChangedAndIsSettable()
        {
            StaRunner.Run(() =>
            {
                using var tb = NewBox();
                tb.Modified = false;

                Type(tb, "x");

                Assert.True(tb.Modified);
                tb.Modified = false;
                Assert.False(tb.Modified);
                Assert.False(tb.IsChanged);
            });
        }

        [Fact]
        public void HideSelection_DefaultsToTrue()
        {
            StaRunner.Run(() =>
            {
                using var tb = NewBox();
                Assert.True(tb.HideSelection);
            });
        }
        #endregion

        #region PasswordChar / PlaceholderText

        [Fact]
        public void UseSystemPasswordChar_TogglesTheMaskChar()
        {
            StaRunner.Run(() =>
            {
                using var tb = NewBox();
                Assert.Equal('\0', tb.PasswordChar);

                tb.UseSystemPasswordChar = true;
                Assert.Equal('●', tb.PasswordChar);

                tb.UseSystemPasswordChar = false;
                Assert.Equal('\0', tb.PasswordChar);
            });
        }

        [Fact]
        public void PasswordChar_KeepsTheUnderlyingTextIntact()
        {
            StaRunner.Run(() =>
            {
                using var tb = NewBox();
                tb.PasswordChar = '*';
                tb.Text = "secret";

                // masking is a rendering concern only, exactly like TextBox
                Assert.Equal("secret", tb.Text);
                Assert.Equal("secret", tb.SelectedText.Length == 0 ? tb.Text : tb.Text);
            });
        }

        [Fact]
        public void PlaceholderText_DefaultsToEmpty()
        {
            StaRunner.Run(() =>
            {
                using var tb = NewBox();
                Assert.Equal("", tb.PlaceholderText);
                tb.PlaceholderText = "type here";
                Assert.Equal("type here", tb.PlaceholderText);
            });
        }

        [Fact]
        public void PasswordChar_MasksEveryGlyph()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox { Width = 400, Height = 100 };
                _ = tb.Handle;
                tb.ImeMode = ImeMode.NoControl; // force the classic (non-IME) draw branch
                tb.Font = new Font("Consolas", 9f);

                // "aaa" masked with '*' must render exactly like the text "***"
                using var masked = Render(tb, "aaa", '*');
                using var stars = Render(tb, "***", '\0');
                using var plain = Render(tb, "aaa", '\0');

                Assert.True(ImageEquals(masked, stars), "PasswordChar did not mask the glyphs");
                Assert.False(ImageEquals(masked, plain), "masked and unmasked text rendered identically");
            });
        }

        /// <summary>
        /// Smoke test for the paint path: the new placeholder/align/mask code must not throw.
        /// </summary>
        [Fact]
        public void OnPaint_WithTheNewTextBoxMembers_DoesNotThrow()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox { Width = 300, Height = 80 };
                _ = tb.Handle;
                tb.Multiline = false;
                tb.TextAlign = HorizontalAlignment.Center;
                tb.PasswordChar = '*';
                tb.HideSelection = false;
                tb.ScrollBars = ScrollBars.None;
                tb.PlaceholderText = "type here";
                tb.Text = "";

                tb.Text = "hello world";
                tb.SelectAll();
                tb.Refresh();
                tb.CreateControl();
                tb.DrawToBitmap(new Bitmap(300, 80), new Rectangle(0, 0, 300, 80));
            });
        }

        private static Bitmap Render(FastColoredTextBox tb, string text, char password)
        {
            tb.PasswordChar = password;
            tb.Text = text;

            var bmp = new Bitmap(400, 100);
            using (var gr = Graphics.FromImage(bmp))
            {
                gr.Clear(Color.White);
                new TextStyle(Brushes.Black, null, FontStyle.Regular)
                    .Draw(gr, new Point(10, 10), new TextSelectionRange(tb, 0, 0, text.Length, 0));
            }
            return bmp;
        }

        private static bool ImageEquals(Bitmap a, Bitmap b)
        {
            for (int y = 0; y < a.Height; y++)
                for (int x = 0; x < a.Width; x++)
                    if (a.GetPixel(x, y) != b.GetPixel(x, y))
                        return false;
            return true;
        }
        #endregion

        #region ScrollBars / ShortcutsEnabled / TextAlign / PreferredHeight

        [Fact]
        public void ScrollBars_MapsOntoShowScrollBars()
        {
            StaRunner.Run(() =>
            {
                using var tb = NewBox();

                tb.ScrollBars = ScrollBars.None;
                Assert.False(tb.ShowScrollBars);
                Assert.Equal(ScrollBars.None, tb.ScrollBars);

                tb.ScrollBars = ScrollBars.Both;
                Assert.True(tb.ShowScrollBars);
                Assert.Equal(ScrollBars.Both, tb.ScrollBars);
            });
        }

        [Fact]
        public void ShortcutsEnabled_False_SuppressesHotkeys()
        {
            StaRunner.Run(() =>
            {
                using var tb = NewBox();
                Type(tb, "hello");
                Assert.Equal(5, tb.TextLength);

                tb.ShortcutsEnabled = false;
                tb.ProcessKey(Keys.A | Keys.Control);
                Assert.Equal(0, tb.SelectionLength);

                tb.ShortcutsEnabled = true;
                tb.ProcessKey(Keys.A | Keys.Control);
                Assert.Equal(5, tb.SelectionLength);
            });
        }

        [Fact]
        public void TextAlign_ShiftsTheTextOriginOfASingleLineControl()
        {
            StaRunner.Run(() =>
            {
                using var tb = NewBox();
                tb.Text = "abc";
                _ = tb.Handle;

                tb.TextAlign = HorizontalAlignment.Left;
                int left = tb.PlaceToPoint(new Place(0, 0)).X;

                tb.TextAlign = HorizontalAlignment.Center;
                int center = tb.PlaceToPoint(new Place(0, 0)).X;

                tb.TextAlign = HorizontalAlignment.Right;
                int right = tb.PlaceToPoint(new Place(0, 0)).X;

                Assert.True(center > left, $"Center={center} Left={left}");
                Assert.True(right > center, $"Right={right} Center={center}");
            });
        }

        [Fact]
        public void PreferredHeight_IsAtLeastOneLine()
        {
            StaRunner.Run(() =>
            {
                using var tb = NewBox();
                _ = tb.Handle;
                Assert.True(tb.PreferredHeight > 0);
            });
        }
        #endregion

        #region AutoComplete

        [Fact]
        public void AutoComplete_SuggestAppend_AppendsAndSelectsTheTail()
        {
            StaRunner.Run(() =>
            {
                using var tb = NewBox();
                tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
                tb.AutoCompleteCustomSource = new AutoCompleteStringCollection { "Hello", "Help" };
                tb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                Type(tb, "hel ");

                Assert.Equal("hello ", tb.Text);
                Assert.Equal("lo", tb.SelectedText); // only the appended part stays selected
            });
        }

        [Fact]
        public void AutoComplete_Append_LeavesTheCaretAfterTheCompletion()
        {
            StaRunner.Run(() =>
            {
                using var tb = NewBox();
                tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
                tb.AutoCompleteCustomSource = new AutoCompleteStringCollection { "Hello" };
                tb.AutoCompleteMode = AutoCompleteMode.Append;

                Type(tb, "hel ");

                Assert.Equal("hello ", tb.Text);
                Assert.Equal(0, tb.SelectionLength);
            });
        }

        [Fact]
        public void AutoComplete_None_IsTheDefaultAndDoesNothing()
        {
            StaRunner.Run(() =>
            {
                using var tb = NewBox();
                Assert.Equal(AutoCompleteMode.None, tb.AutoCompleteMode);
                Assert.Equal(AutoCompleteSource.ListItems, tb.AutoCompleteSource);

                tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
                tb.AutoCompleteCustomSource = new AutoCompleteStringCollection { "Hello" };
                Type(tb, "hel ");

                Assert.Equal("hel ", tb.Text);
            });
        }
        #endregion

        #region Lines / RightToLeft

        [Fact]
        public void Lines_IsAStringArrayLikeTextBox()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox();
                tb.Text = "one\ntwo\nthree";

                string[] lines = tb.Lines;
                Assert.Equal(["one", "two", "three"], lines);
                Assert.Equal(3, lines.Length);

                tb.Lines = ["a", "b"];
                Assert.Equal("a\nb", tb.Text);
            });
        }

        [Fact]
        public void RightToLeft_IsTheInheritedEnumAndDoesNotThrow()
        {
            StaRunner.Run(() =>
            {
                using var tb = NewBox();
                // used to be shadowed by a bool property that threw NotImplementedException
                System.Windows.Forms.RightToLeft rtl = tb.RightToLeft;
                Assert.Equal(System.Windows.Forms.RightToLeft.No, rtl);
            });
        }
        #endregion
    }
}
