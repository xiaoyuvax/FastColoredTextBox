using FastColoredTextBoxNS.Types;

namespace FastColoredTextBoxNS
{
    public class SelectedEventArgs : EventArgs
    {
        public AutocompleteItem Item { get; internal set; }
        public FastColoredTextBox Tb { get; set; }
    }
}