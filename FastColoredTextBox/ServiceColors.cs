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
    [Serializable]
    public class ServiceColors
    {
        public Color CollapseMarkerForeColor { get; set; }
        public Color CollapseMarkerBackColor { get; set; }
        public Color CollapseMarkerBorderColor { get; set; }
        public Color ExpandMarkerForeColor { get; set; }
        public Color ExpandMarkerBackColor { get; set; }
        public Color ExpandMarkerBorderColor { get; set; }

        public ServiceColors()
        {
            CollapseMarkerForeColor = Color.Silver;
            CollapseMarkerBackColor = Color.White;
            CollapseMarkerBorderColor = Color.Silver;
            ExpandMarkerForeColor = Color.Red;
            ExpandMarkerBackColor = Color.White;
            ExpandMarkerBorderColor = Color.Silver;
        }
    }
}