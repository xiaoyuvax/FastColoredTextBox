//
//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE.
//
//  License: GNU Lesser General Public License (LGPLv3)
//
//  Email: pavel_torgashov@ukr.net
//
//  Copyright (C) Pavel Torgashov, 2011-2016.

// -------------------------------------------------------------------------------


using FastColoredTextBoxNS.Types;

namespace FastColoredTextBoxNS
{
    /// <summary>
    /// TextChanged event argument
    /// </summary>
    public class TextChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TextChangedEventArgs(TextSelectionRange changedRange)
        {
            ChangedRange = changedRange;
        }

        /// <summary>
        /// This range contains changed area of text
        /// </summary>
        public TextSelectionRange ChangedRange { get; set; }
    }

}