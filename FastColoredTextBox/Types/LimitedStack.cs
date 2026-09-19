namespace FastColoredTextBoxNS.Types
{
    /// <summary>
    /// Stack with a fixed maximum length.
    /// When full, pushing discards the OLDEST item (bottom of the stack) so the
    /// most recent items are always retained - the expected undo-history policy.
    /// Capacity is exactly <see cref="Max"/> items.
    /// </summary>
    public class LimitedStack<T> : Stack<T>
    {
        /// <summary>
        /// Max stack length
        /// </summary>
        public int Max { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="maxItemCount">Maximum length of stack</param>
        public LimitedStack(int maxItemCount) => Max = maxItemCount;

        /// <summary>
        /// Push item; when the stack is full the oldest item (bottom) is dropped.
        /// </summary>
        public new void Push(T item)
        {
            if (Count >= Max)
            {
                //drop the oldest (bottom) item; Stack<T> has no RemoveBottom, so rebuild
                var kept = new T[Count - 1];
                for (int i = kept.Length - 1; i >= 0; i--)
                    kept[i] = base.Pop();
                Clear();
                for (int i = 0; i < kept.Length; i++)
                    base.Push(kept[i]);
            }

            base.Push(item);
        }
    }
}
