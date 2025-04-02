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
    /// ToolTipNeeded event args
    /// </summary>
    public class ToolTipNeededEventArgs : EventArgs
    {
        public ToolTipNeededEventArgs(Place place, string hoveredWord)
        {
            HoveredWord = hoveredWord;
            Place = place;
        }

        public Place Place { get; private set; }
        public string HoveredWord { get; private set; }
        public string ToolTipTitle { get; set; }
        public string ToolTipText { get; set; }
        public ToolTipIcon ToolTipIcon { get; set; }
    }

}