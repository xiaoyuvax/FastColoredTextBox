FastColoredTextBox
==================

Fast Colored TextBox is text editor component for .NET.
Allows you to create custom text editor with syntax highlighting.
It works well with small, medium, large and very-very large files.

It has such settings as foreground color, font style, background color which can be adjusted for arbitrarily selected text symbols. One can easily gain access to a text with the use of regular expressions. WordWrap, Find/Replace, Code folding and multilevel Undo/Redo are supported as well. 

[中文文档](README_CN.md)

## Update Logs   
17-11-2026 (V2.19.0.232)
- **Markdown markers, completed**: an inline construct inside a heading (`## **bold**`, `` ## `code` ``, `## [text](url)`) now hides its markers too. The heading branch returned before the inline pass, so a formatted heading still showed its `**`/backticks/`[]()`; the markers of the heading itself were already hidden
- **Exports**: `ExportToHTML`, `ExportToRTF` and therefore `Print` skip hidden markers instead of emitting every char of the line, so an exported or printed markdown document no longer carries the `#`'s and `**`'s that the control does not draw
- Rendering is covered by a bitmap regression test: drawing a line with hidden markers produces exactly the bitmap of its visible text

22-11-2026 (V2.19.0.235)
- **Markdown tables in a proportional font**: alignment is still done in the source text, but a control with `AllowProportionalFont` has no character grid, so padding by column count lined up nothing (a CJK cell is about four spaces wide in Microsoft YaHei UI, not two). `AlignMarkdownTables` now measures each cell with the control's own char metrics there and pads it in whole space widths, and the delimiter row's dash count comes from the pixels it has to fill, so the pipes of every row land within one space of the pipes above. A cell is measured as it renders, too: the inline markers the highlighter hides (`**bold**`, a code span, link syntax) take no room, so a column holding them no longer drifts once they disappear. Monospaced controls keep the column grid and their exact output. Tests: `MarkdownTableAlignTests.ProportionalFont_PipesLineUpInPixels`, `HiddenMarkers_DoNotCountTowardsColumnWidth`

17-11-2026 (V2.19.0.231)
- **Markdown tables**: `SyntaxHighlighter.AlignMarkdownTables(range)` rewrites the tables of a range so their columns line up, and returns how many tables it formatted. The control renders on a character grid, so the alignment is done in the source text by padding the cells: a column is as wide as its widest cell (at least 3, a CJK char counting as two columns), the `:--` / `:--:` / `--:` markers of the delimiter row decide left/center/right, and a missing leading or trailing pipe is added. Escaped (`\|`) and code-span pipes stay inside their cell, and a row with more cells widens the table instead of losing text. The whole table is replaced in one edit (one undo restores it), the selection is kept, and formatting twice changes nothing. Call it from a menu item, a shortcut or a save hook

17-11-2026 (V2.19.0.230)
- **Markdown rendering**: source markers of a formatted construct are no longer drawn. A heading renders as its title (the leading `#`s and the optional closing ones are hidden), `**bold**`, `*italic*`, `_emphasis_`, `~~strike~~` and `` `code` `` render as the styled text alone, and a link or image keeps only its label (`[text](url)`, `![alt](url)` and `<https://host>` lose their syntax)
- A hidden marker takes no width, so the caret, selection, click-to-caret mapping and word wrap treat `**bold**` as the four characters it renders as. List bullets, task checkboxes, thematic breaks and code fences stay visible (they are the rendered element), and markdown inside a code span or a fenced block is untouched; switching away from `Language.Markdown` reveals everything again

18-10-2026 (V2.19.0.223)
- **Word wrap**: the continuation indent is now deducted from the wrap width. With `WordWrapAutoIndent`, continuation lines are drawn shifted right by `LineInfo.wordWrapIndent` chars, but the wrap calculation still used the full available width, so every continuation line of an indented line stuck out past the right edge (visible as soon as a host indents a block). New overload `CalcCutOffsByMaxWidth(..., continuationIndentChars)`: the first segment keeps the full width, later ones reserve the indent
- A word-wrap recalculation on a near-zero-width control no longer freezes the UI or exhausts memory (`Array dimensions exceeded supported range`): cut-off calculation is skipped while the available width is below `CharWidth * 4` and recalculated once it recovers
- The Find/Replace dialogs can be themed by hosts through the new `FastColoredTextBox.FindReplaceFormShown` callback (fired after the dialog is shown)

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
