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
    public enum WordWrapMode
    {
        /// <summary>
        /// Word wrapping by control width
        /// </summary>
        WordWrapControlWidth,

        /// <summary>
        /// Word wrapping by preferred line width (PreferredLineWidth)
        /// </summary>
        WordWrapPreferredWidth,

        /// <summary>
        /// Char wrapping by control width
        /// </summary>
        CharWrapControlWidth,

        /// <summary>
        /// Char wrapping by preferred line width (PreferredLineWidth)
        /// </summary>
        CharWrapPreferredWidth,

        /// <summary>
        /// Custom wrap (by event WordWrapNeeded)
        /// </summary>
        Custom
    }

}