using System;
using FastColoredTextBoxNS;
using FastColoredTextBoxNS.Input;
using FastColoredTextBoxNS.Types;
using Xunit;

namespace FastColoredTextBoxNS.Tests
{
    /// <summary>
    /// Regression tests for the 2.17.0.208 undo/redo fixes:
    /// - LimitedStack drops the OLDEST item when full (#1)
    /// - RemoveLines keeps an empty line through the command stack so Undo
    ///   restores the removed content (#3)
    /// </summary>
    public class UndoRedo208Tests
    {
        [Fact]
        public void LimitedStack_Full_DropsOldestKeepsNewest()
        {
            var stack = new LimitedStack<int>(3);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            stack.Push(4); // full: 1 is dropped
            stack.Push(5); // full: 2 is dropped

            Assert.Equal(3, stack.Count);
            Assert.Equal(5, stack.Pop());
            Assert.Equal(4, stack.Pop());
            Assert.Equal(3, stack.Pop());
            Assert.Equal(0, stack.Count);
        }

        [Fact]
        public void LimitedStack_CapacityIsExactlyMax()
        {
            var stack = new LimitedStack<int>(5);
            for (int i = 0; i < 100; i++)
                stack.Push(i);

            Assert.Equal(5, stack.Count); // old off-by-one allowed 6
            Assert.Equal(99, stack.Pop()); // newest retained
            Assert.Equal(98, stack.Peek()); // second-newest now on top
        }

        [Fact]
        public void UndoHistory_Overflow_RetainsMostRecentEdits()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox();
                tb.Text = "";
                tb.ClearUndo();

                const int total = CommandManager.MaxHistoryLength + 5;
                for (int i = 0; i < total; i++)
                    tb.InsertText(i + "\n");
                var full = tb.Text;

                // undo everything retained: the OLDEST edits are gone from history,
                // the NEWEST (total-1 .. MaxHistoryLength) must be undoable
                for (int i = 0; i < CommandManager.MaxHistoryLength; i++)
                    tb.Undo();

                var afterUndo = tb.Text;
                // lines 0..4 (the oldest 5 inserts) must remain in the document
                Assert.StartsWith("0\n1\n2\n3\n4\n", afterUndo);
                // "0\n1\n2\n3\n4\n" is exactly 10 chars
                Assert.Equal(10, afterUndo.Length);

                // redo restores exactly what was undone
                for (int i = 0; i < CommandManager.MaxHistoryLength; i++)
                    tb.Redo();
                Assert.Equal(full, tb.Text);
            });
        }

        [Fact]
        public void RemoveLines_OnlyLine_UndoRestoresContent()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox();
                tb.Text = "only";
                Assert.Equal(1, tb.LinesCount);

                tb.RemoveLines(new System.Collections.Generic.List<int> { 0 });
                Assert.Equal(1, tb.LinesCount); // one empty line kept
                Assert.Equal("", tb.Text);

                tb.Undo();
                Assert.Equal("only\n", tb.Text); // content restored (#3); one trailing
                                                 // empty line remains from the compensation
                Assert.Equal(2, tb.LinesCount);
            });
        }

        [Fact]
        public void RemoveLines_MultipleLines_UndoRestoresAll()
        {
            StaRunner.Run(() =>
            {
                using var tb = new FastColoredTextBox();
                tb.Text = "a\nb\nc\nd\n";

                tb.RemoveLines(new System.Collections.Generic.List<int> { 1, 2 }); // remove b, c
                Assert.Equal("a\nd\n", tb.Text);

                tb.Undo();
                Assert.Equal("a\nb\nc\nd\n", tb.Text);

                tb.Redo();
                Assert.Equal("a\nd\n", tb.Text);
            });
        }
    }
}
