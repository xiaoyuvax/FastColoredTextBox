﻿using FastColoredTextBoxNS.Input;
using FastColoredTextBoxNS.Types;
using System.Collections;
using System.Text;

namespace FastColoredTextBoxNS.Text
{
    /// <summary>
    /// This class contains the source text (chars and styles).
    /// It stores a text lines, the manager of commands, undo/redo stack, styles.
    /// </summary>
    public class TextSource : IList<Line>, IDisposable
    {
        readonly protected List<Line> lines = [];
        protected LinesAccessor linesAccessor;
        private int lastLineUniqueId;
        public CommandManager Manager { get; set; }
        private FastColoredTextBox currentTB;

        /// <summary>
        /// Styles
        /// </summary>
        public readonly StyleManager Styles;

        /// <summary>
        /// Occurs when line was inserted/added
        /// </summary>
        public event EventHandler<LineInsertedEventArgs> LineInserted;

        /// <summary>
        /// Occurs when line was removed
        /// </summary>
        public event EventHandler<LineRemovedEventArgs> LineRemoved;

        /// <summary>
        /// Occurs when text was changed
        /// </summary>
        public event EventHandler<TextChangedEventArgs> TextChanged;

        /// <summary>
        /// Occurs when recalc is needed
        /// </summary>
        public event EventHandler<TextChangedEventArgs> RecalcNeeded;

        /// <summary>
        /// Occurs when recalc wordwrap is needed
        /// </summary>
        public event EventHandler<TextChangedEventArgs> RecalcWordWrap;

        /// <summary>
        /// Occurs before text changing
        /// </summary>
        public event EventHandler<TextChangingEventArgs> TextChanging;

        /// <summary>
        /// Occurs after CurrentTB was changed
        /// </summary>
        public event EventHandler CurrentTBChanged;

        /// <summary>
        /// Current focused FastColoredTextBox
        /// </summary>
        public FastColoredTextBox CurrentTB
        {
            get { return currentTB; }
            set
            {
                if (currentTB == value)
                    return;
                currentTB = value;
                OnCurrentTBChanged();
            }
        }

        public virtual void ClearIsChanged()
        {
            foreach (var line in lines)
                line.IsChanged = false;
        }

        public virtual Line CreateLine() => new(GenerateUniqueLineId());

        private void OnCurrentTBChanged() => CurrentTBChanged?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Default text style
        /// This style is using when no one other TextStyle is not defined in Char.style
        /// </summary>
        public TextStyle DefaultStyle { get; set; }

        /// <summary>
        /// Gets the default read only style.
        /// </summary>
        /// <value>The default read only style.</value>
        public ReadOnlyStyle DefaultReadOnlyStyle { get; } = new ReadOnlyStyle();

        /// <summary>
        /// Gets the default blinking style.
        /// </summary>
        /// <value>The default blinking style.</value>
        public BlinkingStyle DefaultBlinkingStyle { get; } = new BlinkingStyle();

        public TextSource(FastColoredTextBox currentTB)
        {
            CurrentTB = currentTB;
            linesAccessor = new LinesAccessor(this);
            Manager = new CommandManager(this);
            Styles = new StyleManager();

            InitDefaultStyle();
        }

        public virtual void InitDefaultStyle() => DefaultStyle = new TextStyle(null, null, FontStyle.Regular);

        public virtual Line this[int i]
        {
            get
            {
                return lines[i];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual bool IsLineLoaded(int iLine) => lines[iLine] != null;

        /// <summary>
        /// Text lines
        /// </summary>
        public virtual IList<string> GetLines() => linesAccessor;

        public IEnumerator<Line> GetEnumerator() => lines.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => lines as IEnumerator;

        public virtual int BinarySearch(Line item, IComparer<Line> comparer) => lines.BinarySearch(item, comparer);

        public virtual int GenerateUniqueLineId() => lastLineUniqueId++;

        public virtual void InsertLine(int index, Line line)
        {
            lines.Insert(index, line);
            OnLineInserted(index);
        }

        public virtual void OnLineInserted(int index) => OnLineInserted(index, 1);

        public virtual void OnLineInserted(int index, int count) => LineInserted?.Invoke(this, new LineInsertedEventArgs(index, count));

        public virtual void RemoveLine(int index) => RemoveLine(index, 1);

        public virtual bool IsNeedBuildRemovedLineIds => LineRemoved != null;

        public virtual void RemoveLine(int index, int count)
        {
            List<int> removedLineIds = [];
            //
            if (count > 0)
                if (IsNeedBuildRemovedLineIds)
                    for (int i = 0; i < count; i++)
                        removedLineIds.Add(this[index + i].UniqueId);
            //
            lines.RemoveRange(index, count);

            OnLineRemoved(index, count, removedLineIds);
        }

        public virtual void OnLineRemoved(int index, int count, List<int> removedLineIds)
        {
            if (count > 0)
                LineRemoved?.Invoke(this, new LineRemovedEventArgs(index, count, removedLineIds));
        }

        public virtual void OnTextChanged(int fromLine, int toLine) => TextChanged?.Invoke(this, new TextChangedEventArgs(Math.Min(fromLine, toLine), Math.Max(fromLine, toLine)));

        public class TextChangedEventArgs : EventArgs
        {
            public int iFromLine;
            public int iToLine;

            public TextChangedEventArgs(int iFromLine, int iToLine)
            {
                this.iFromLine = iFromLine;
                this.iToLine = iToLine;
            }
        }

        public virtual int IndexOf(Line item) => lines.IndexOf(item);

        public virtual void Insert(int index, Line item) => InsertLine(index, item);

        public virtual void RemoveAt(int index) => RemoveLine(index);

        public virtual void Add(Line item) => InsertLine(Count, item);

        public virtual void Clear() => RemoveLine(0, Count);

        public virtual bool Contains(Line item) => lines.Contains(item);

        public virtual void CopyTo(Line[] array, int arrayIndex) => lines.CopyTo(array, arrayIndex);

        /// <summary>
        /// Lines count
        /// </summary>
        public virtual int Count => lines.Count;

        public virtual bool IsReadOnly => false;

        public virtual bool Remove(Line item)
        {
            int i = IndexOf(item);
            if (i >= 0)
            {
                RemoveLine(i);
                return true;
            }
            else
                return false;
        }

        public virtual void NeedRecalc(TextChangedEventArgs args) => RecalcNeeded?.Invoke(this, args);

        public virtual void OnRecalcWordWrap(TextChangedEventArgs args) => RecalcWordWrap?.Invoke(this, args);

        public virtual void OnTextChanging()
        {
            string temp = null;
            OnTextChanging(ref temp);
        }

        public virtual void OnTextChanging(ref string text)
        {
            if (TextChanging != null)
            {
                var args = new TextChangingEventArgs() { InsertingText = text };
                TextChanging(this, args);
                text = args.InsertingText;
                if (args.Cancel)
                    text = string.Empty;
            };
        }

        public virtual int GetLineLength(int i) => lines[i].Count;

        public virtual bool LineHasFoldingStartMarker(int iLine) => !string.IsNullOrEmpty(lines[iLine].FoldingStartMarker);

        public virtual bool LineHasFoldingEndMarker(int iLine) => !string.IsNullOrEmpty(lines[iLine].FoldingEndMarker);

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public virtual void SaveToFile(string fileName, Encoding enc)
        {
            using StreamWriter sw = new(fileName, false, enc);
            for (int i = 0; i < Count - 1; i++)
                sw.WriteLine(lines[i].Text);

            sw.Write(lines[Count - 1].Text);
        }
    }
}