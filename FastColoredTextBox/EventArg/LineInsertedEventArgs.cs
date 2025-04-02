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
    public class LineInsertedEventArgs : EventArgs
    {
        public LineInsertedEventArgs(int index, int count)
        {
            Index = index;
            Count = count;
        }

        /// <summary>
        /// Inserted line index
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Count of inserted lines
        /// </summary>
        public int Count { get; private set; }
    }

}