FastColoredTextBox
==================

Fast Colored TextBox is text editor component for .NET.
Allows you to create custom text editor with syntax highlighting.
It works well with small, medium, large and very-very large files.

It has such settings as foreground color, font style, background color which can be adjusted for arbitrarily selected text symbols. One can easily gain access to a text with the use of regular expressions. WordWrap, Find/Replace, Code folding and multilevel Undo/Redo are supported as well. 

[中文文档](README_CN.md)

## Update Logs   
29-09-2026 (V2.18.0.220)
- **TextBox migration**: the public properties a WinForms form needs but FCTB lacked are now implemented, so a `TextBox` can be replaced by a `FastColoredTextBox` without touching the call site: `MaxLength`, `CharacterCasing`, `CanUndo`, `Modified`, `HideSelection`, `PasswordChar`, `UseSystemPasswordChar`, `PlaceholderText`, `PreferredHeight`, `ScrollBars`, `ShortcutsEnabled`, `TextAlign`, `AutoCompleteMode`, `AutoCompleteSource`, `AutoCompleteCustomSource`
- `Lines` now returns `string[]` and has a setter, matching `TextBoxBase.Lines`
- Removed the `new bool RightToLeft` shadow that threw `NotImplementedException`; the inherited `Control.RightToLeft` is used again
- `ForceUpperCase` is now a shorthand for `CharacterCasing = Upper`

See [Migrating from TextBox](#migrating-from-textbox) below.

07-09-2026 (V2.17.0.206)
- Horizontal scroll text overlapping line number area fully fixed (CJK line re-evaluation, non-uniform char width support)

04-09-2025 (V2.17.0.205)
- Undo/Redo core logic refactored (position accuracy, cross-line support)
- Column selection mode IndexOutOfRangeException fixed
- Horizontal scroll text overlapping line numbers fixed
- Default padding 2px (matches WinForms TextBox)
- Localization support for Find/Replace/GoTo/Hotkeys forms (EN/ZH)
- Chinese labels shortened to match English width
- Label z-order fixed (no longer overlaps controls)
- Redo shortcut changed to Ctrl+Y (standard Windows convention)
- .NET 10.0 support added

25-12-22 (V2.17.0.203)
- A wordwrapping bug fixed
- .NET6.0 target removed for better code optimization (now targeting .NET7-9)
- Some other code optimization

25-04-02 (V2.17.0.2)
- CJKL mix wordwrapping almost perfect/中日韩拉丁字母混合换行基本完美
- Targeting .NET6.0-9.0
- Restructed the code and minor optimization
  
24-12-30 (V2.17.0.0)
- Reforked from Wiredwizard:-Daxanius:-PavelTorgashov
24-12-25 (V2.16.27.106)
- Improved CJK support with demo added,now it suits general CJK application comfortably.
- CN:添加了CJK支持，做了demo，现在支持一般CJK应用没问题。
24-12-24 (V2.16.27.103)
- Add UseCJK property, may need further improvements.

24-12-20 (V2.16.27.100)
- Start to support correct displaying Chinese based on the lately found fork by Daxanius, though not perfect, still needs some tweeks in later version
- CN:在能找到的最新的Daxanius版本上增加了汉字显示支持，虽不完美还需在以后版本调整
- Multitargeting net6.0-8.0
- Upgrade to C#12 sematics


## Migrating from TextBox

Every public instance property of `System.Windows.Forms.TextBox` exists on
`FastColoredTextBox` with the same type and the same public accessors, so dropping one
control in place of the other compiles. The members that had to be added, and how far
they go:

| Member | Behaviour |
| --- | --- |
| `MaxLength` | Limits typing, like `TextBoxBase`. Assigning `Text`/`SelectedText` is not truncated (same as `TextBox`). Negative values throw. |
| `CharacterCasing` | Applied to every char that enters the control, typed or inserted. `ForceUpperCase` is a shorthand for `CharacterCasing = Upper`. |
| `CanUndo` | `true` while there is something to undo. |
| `Modified` | Mirrors `IsChanged`; settable. |
| `HideSelection` | Hides the selection while the control is unfocused (default `true`). |
| `PasswordChar` / `UseSystemPasswordChar` | Masks the rendered glyphs. The text itself, the clipboard and the exporters are untouched. |
| `PlaceholderText` | Drawn while the control is empty. |
| `PreferredHeight` | Height of one text line. |
| `ScrollBars` | `None`/`Both` map onto `ShowScrollBars`. `Vertical`/`Horizontal` behave as `Both`: the scrollbars are driven by `ScrollableControl.AutoScroll` and cannot be shown one axis at a time. |
| `ShortcutsEnabled` | `false` suppresses the hotkeys of `HotkeysMapping`; typing is unaffected. |
| `TextAlign` | Offsets a single-line control's text. Ignored while `Multiline` or `WordWrap` is set. |
| `AutoCompleteMode` / `AutoCompleteSource` / `AutoCompleteCustomSource` | `Append` and `SuggestAppend` append the missing tail when a delimiter is typed (`SuggestAppend` leaves it selected). Single-line only, like `TextBox`. The inline ghost hint of `Suggest` is not drawn - use `AutocompleteMenu` for that. |

Still not carried over (used far less often, add them if you need them):
`DeselectAll`, `Paste(string)`, `GetCharFromPosition`, `GetCharIndexFromPosition`,
`GetPositionFromCharIndex`, `GetLineFromCharIndex`, `GetFirstCharIndexFromLine`,
`GetFirstCharIndexOfCurrentLine`, and the `*Changed` events for `ReadOnly`, `Multiline`,
`AcceptsTab`, `BorderStyle`, `HideSelection`, `Modified` and `TextAlign`.

Two signatures deliberately differ from `TextBox` because FCTB's own API depends on them:
`TextChanged` carries `TextChangedEventArgs` rather than `EventArgs`, and right-to-left
text is not rendered (`RightToLeft` is accepted but ignored).

## Known Issues
- Wordwrapped lines may exceed controlwidth in some cases, but can be solved by setting PaddingRight property.

![image](https://github.com/user-attachments/assets/45d80c00-62d4-4782-bc65-6c5cc13e9710)


More details http://www.codeproject.com/Articles/161871/Fast-Colored-TextBox-for-syntax-highlighting

Nuget package https://www.nuget.org/packages/VAX-FCTB/
