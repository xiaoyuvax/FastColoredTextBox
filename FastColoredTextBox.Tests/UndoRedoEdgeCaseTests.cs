using System;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using FastColoredTextBoxNS.Types;
using Xunit;

namespace FastColoredTextBoxNS.Tests
{
    /// <summary>
    /// Undo/Redo edge cases: empty document, read-only mode, column selection mode.
    /// </summary>
    public class UndoRedoEdgeCaseTests
    {
        #region Empty document

        [Fact]
        public void Undo_InsertIntoEmptyDocument_RestoresEmptyDocument()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox();
                tb.Text = "";
                tb.ClearUndo();
                // Text setter on an empty document leaves a single empty line
                Assert.True(tb.LinesCount <= 1, $"LinesCount={tb.LinesCount}");

                tb.Selection.Start = Place.Empty;
                tb.InsertText("hello");

                Assert.Equal("hello", tb.Text);

                tb.Undo();
                // undo restores an empty single-line document (not zero lines)
                Assert.Equal("", tb.Text);
                Assert.Equal(1, tb.LinesCount);

                tb.Redo();
                Assert.Equal("hello", tb.Text);
            });
        }

        [Fact]
        public void Undo_InsertNewlineOnlyIntoEmptyDocument()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox();
                tb.Text = "";
                tb.ClearUndo();

                tb.Selection.Start = Place.Empty;
                tb.InsertText("\n");

                tb.Undo();
                Assert.Equal("", tb.Text);

                tb.Redo();
                Assert.Equal("\n", tb.Text);
            });
        }
        #endregion

        #region Read-only mode

        [Fact]
        public void ReadOnly_KeyboardInput_IsRejected()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox();
                tb.Text = "abc";
                tb.ClearUndo();
                tb.ReadOnly = true;

                tb.Selection.Start = new Place(3, 0);
                // keyboard path: ProcessKey checks ReadOnly and rejects the char
                bool handled = tb.ProcessKey('X', Keys.None);

                Assert.False(handled);
                Assert.Equal("abc", tb.Text);
                Assert.False(tb.UndoEnabled); // nothing recorded in history
            });
        }

        [Fact]
        public void ReadOnly_ProgrammaticInsertText_InsertsByDesign()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox();
                tb.Text = "abc";
                tb.ClearUndo();
                tb.ReadOnly = true;

                tb.Selection.Start = new Place(3, 0);
                tb.InsertText("X");

                // documented FCTB behavior: the ReadOnly flag only gates keyboard/UI
                // input (ProcessKey), programmatic InsertText still inserts and
                // records undo history
                Assert.Equal("abcX", tb.Text);
                Assert.True(tb.UndoEnabled);
            });
        }

        [Fact]
        public void ReadOnly_ViaProcessKeyboard_UndoIsBlockedButBecomesPossibleAfterUnlock()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox();
                tb.Text = "abcdef";
                tb.ClearUndo();
                // Text setter selects all; collapse the caret before inserting,
                // otherwise InsertText("def") would replace the selected "def"
                tb.Selection.Start = new Place(3, 0);
                tb.Selection.End = new Place(3, 0);
                tb.InsertText("def");
                Assert.Equal("abcdefdef", tb.Text);
                Assert.True(tb.UndoEnabled);

                tb.ReadOnly = true;
                // FCTBAction.Undo path checks ReadOnly and skips the operation
                tb.ProcessKey(Keys.Control | Keys.Z); // must not throw and must not undo

                Assert.Equal("abcdefdef", tb.Text); // undo was blocked
                Assert.True(tb.UndoEnabled); // history untouched

                // after unlocking, the same keystroke undoes
                tb.ReadOnly = false;
                tb.ProcessKey(Keys.Control | Keys.Z);
                Assert.Equal("abcdef", tb.Text);
                Assert.False(tb.UndoEnabled); // undo really executed
            });
        }
        #endregion

        #region Column selection mode

        [Fact]
        public void ColumnSelection_InsertText_UndoRestoresAllLines()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox();
                tb.Text = "aaa\nbbb\nccc\n";
                tb.ClearUndo();

                // select column 1..2 on the three content lines
                tb.Selection.ColumnSelectionMode = true;
                tb.Selection.Start = new Place(1, 0);
                tb.Selection.End = new Place(2, 2);

                tb.InsertText("X");

                var afterInsert = tb.Text;
                Assert.Contains("aXa", afterInsert);
                Assert.Contains("bXb", afterInsert);
                Assert.Contains("cXc", afterInsert);

                tb.Undo();
                Assert.Equal("aaa\nbbb\nccc\n", tb.Text);

                tb.Redo();
                Assert.Equal(afterInsert, tb.Text);
            });
        }

        [Fact]
        public void ColumnSelection_DeleteSelection_UndoRestores()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox();
                tb.Text = "aaa\nbbb\nccc\n";
                tb.ClearUndo();

                tb.Selection.ColumnSelectionMode = true;
                tb.Selection.Start = new Place(1, 0);
                tb.Selection.End = new Place(2, 2);

                tb.ClearSelected(); // delete the column block

                var afterDelete = tb.Text;
                Assert.Contains("aa", afterDelete);

                tb.Undo();
                Assert.Equal("aaa\nbbb\nccc\n", tb.Text);
            });
        }
        #endregion
    }
}
