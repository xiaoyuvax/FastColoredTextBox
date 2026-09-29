FastColoredTextBox
==================

Fast Colored TextBox 是一个适用于 .NET 的文本编辑器组件，支持语法高亮。
适用于小型、中型、超大型文件。

支持前景色、字体样式、背景色的任意调整，支持正则表达式访问文本。
支持自动换行、查找/替换、代码折叠和多级撤销/重做。

[English](README.md)

## 更新日志
2026-09-29 (V2.18.0.220)
- **从 TextBox 迁移**：`System.Windows.Forms.TextBox` 的所有 public 实例属性现在在
  `FastColoredTextBox` 上都存在，且类型与读写器一致，可直接替换控件、调用代码无需改动：
  `MaxLength`、`CharacterCasing`、`CanUndo`、`Modified`、`HideSelection`、`PasswordChar`、
  `UseSystemPasswordChar`、`PlaceholderText`、`PreferredHeight`、`ScrollBars`、
  `ShortcutsEnabled`、`TextAlign`、`AutoCompleteMode`、`AutoCompleteSource`、
  `AutoCompleteCustomSource`
- `Lines` 改为返回 `string[]` 并支持写入，与 `TextBoxBase.Lines` 一致
- 移除会抛 `NotImplementedException` 的 `new bool RightToLeft` 影子属性，恢复使用继承的
  `Control.RightToLeft`
- `ForceUpperCase` 现在是 `CharacterCasing = Upper` 的简写

详见下方[从 TextBox 迁移](#从-textbox-迁移)。

2026-09-07 (V2.17.0.206)
- 水平滚动时文字覆盖行号区域问题完全修复（CJK行首字符重新评估、非等宽字符宽度支持）

2025-09-04 (V2.17.0.205)
- 撤销/重做核心逻辑重构（位置准确性、跨行支持）
- 列选择模式 IndexOutOfRangeException 修复
- 水平滚动文字覆盖行号修复
- 默认 Padding 2px（与 WinForms TextBox 一致）
- 查找/替换/转到/快捷键窗体本地化支持（中英文）
- 中文标签缩短以匹配英文宽度
- 标签z顺序修复（不再覆盖控件）
- 重做快捷键改为 Ctrl+Y（标准Windows惯例）
- 支持 .NET 10.0

2025-12-22 (V2.17.0.203)
- 修复换行错误
- 移除 .NET6.0 目标以优化代码（现支持 .NET7-9）
- 其他代码优化

2025-04-02 (V2.17.0.2)
- 中日韩拉丁字母混合换行基本完美
- 支持 .NET6.0-9.0
- 代码重构与优化

2024-12-30 (V2.17.0.0)
- 从 Wiredwizard:-Daxanius:-PavelTorgashov 重新 Fork

2024-12-25 (V2.16.27.106)
- 添加CJK支持与示例，现已支持一般CJK应用

2024-12-24 (V2.16.27.103)
- 添加 UseCJK 属性

2024-12-20 (V2.16.27.100)
- 基于 Daxanius 的 Fork 增加汉字显示支持（尚不完美，后续版本调整）
- 支持 .NET6.0-8.0
- 升级至 C#12 语法

## 从 TextBox 迁移

`System.Windows.Forms.TextBox` 的每个 public 实例属性在 `FastColoredTextBox` 上都有同名、
同类型、同可见性的成员，直接换控件即可通过编译。其中新增成员的实现范围如下：

| 成员 | 行为 |
| --- | --- |
| `MaxLength` | 与 `TextBoxBase` 一致，只限制键盘输入；给 `Text`/`SelectedText` 赋值不会被截断。负值抛异常。 |
| `CharacterCasing` | 对所有进入控件的字符生效（键入与插入）。`ForceUpperCase` 等价于 `CharacterCasing = Upper`。 |
| `CanUndo` | 撤销栈非空时为 `true`。 |
| `Modified` | 与 `IsChanged` 同步，可读写。 |
| `HideSelection` | 失去焦点时隐藏选区（默认 `true`）。 |
| `PasswordChar` / `UseSystemPasswordChar` | 只遮蔽渲染出来的字形；文本本身、剪贴板和导出结果不变。 |
| `PlaceholderText` | 控件为空时绘制。 |
| `PreferredHeight` | 单行文本的高度。 |
| `ScrollBars` | `None`/`Both` 映射到 `ShowScrollBars`；`Vertical`/`Horizontal` 按 `Both` 处理——滚动条由 `ScrollableControl.AutoScroll` 驱动，无法只显示单轴。 |
| `ShortcutsEnabled` | 设为 `false` 时屏蔽 `HotkeysMapping` 的快捷键，正常键入不受影响。 |
| `TextAlign` | 调整单行控件的文本位置；`Multiline` 或 `WordWrap` 时无效。 |
| `AutoCompleteMode` / `AutoCompleteSource` / `AutoCompleteCustomSource` | `Append` 与 `SuggestAppend` 在键入分隔符时补全缺失部分（`SuggestAppend` 会选中补上的部分）。与 `TextBox` 一样仅对单行生效；`Suggest` 的灰色内联提示未实现，请用 `AutocompleteMenu`。 |

尚未补齐（很少用到，需要时再加）：`DeselectAll`、`Paste(string)`、
`GetCharFromPosition`、`GetCharIndexFromPosition`、`GetPositionFromCharIndex`、
`GetLineFromCharIndex`、`GetFirstCharIndexFromLine`、`GetFirstCharIndexOfCurrentLine`，
以及 `ReadOnly`、`Multiline`、`AcceptsTab`、`BorderStyle`、`HideSelection`、`Modified`、
`TextAlign` 的 `*Changed` 事件。

有两处签名与 `TextBox` 故意不同，因为 FCTB 自身 API 依赖它们：`TextChanged` 事件参数是
`TextChangedEventArgs` 而不是 `EventArgs`；从右到左文本不做渲染（`RightToLeft` 可读写但
不生效）。

## 已知问题
- 自动换行时部分行可能超出控件宽度（尤其滚动条可见时），可通过设置 PaddingRight 属性解决。

![image](https://github.com/user-attachments/assets/45d80c00-62d4-4782-bc65-6c5cc13e9710)

## 相关链接
- 详细介绍: http://www.codeproject.com/Articles/161871/Fast-Colored-TextBox-for-syntax-highlighting
- NuGet 包: https://www.nuget.org/packages/VAX-FCTB/
- GitHub: https://github.com/xiaoyuvax/FastColoredTextBox
