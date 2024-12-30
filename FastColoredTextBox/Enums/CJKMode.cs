//
//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE.
//
//  License: GNU Lesser General Public License (LGPLv3)
//
//  Github:https://github.com/xiaoyuvax
//
//  Copyright (C) Xiaoyuvax, 2024-?.
// -------------------------------------------------------------------------------

namespace FastColoredTextBoxNS
{
    public enum CJKMode
    {
        Disabled,
        Hanzi,  //Use universal chinese char(Hanzi) width for all CJK chars, it might not be perfect for some JK phonetic chars but offering almost equal performance as latin-only mode, when only Hanzi support is required.
        CJK     //CJK mix mode
    }
}