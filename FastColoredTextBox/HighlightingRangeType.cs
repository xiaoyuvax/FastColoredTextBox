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


namespace FastColoredTextBoxNS
{
    /// <summary>
    /// Type of highlighting
    /// </summary>
    public enum HighlightingRangeType
    {
        /// <summary>
        /// Highlight only changed range of text. Highest performance.
        /// </summary>
        ChangedRange,

        /// <summary>
        /// Highlight visible range of text. Middle performance.
        /// </summary>
        VisibleRange,

        /// <summary>
        /// Highlight all (visible and invisible) text. Lowest performance.
        /// </summary>
        AllTextRange
    }

}