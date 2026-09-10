FastColoredTextBox
==================

Fast Colored TextBox 是一个适用于 .NET 的文本编辑器组件，支持语法高亮。
适用于小型、中型、超大型文件。

支持前景色、字体样式、背景色的任意调整，支持正则表达式访问文本。
支持自动换行、查找/替换、代码折叠和多级撤销/重做。

[English](README.md)

## 更新日志
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

## 已知问题
- 自动换行时部分行可能超出控件宽度（尤其滚动条可见时），可通过设置 PaddingRight 属性解决。

![image](https://github.com/user-attachments/assets/45d80c00-62d4-4782-bc65-6c5cc13e9710)

## 相关链接
- 详细介绍: http://www.codeproject.com/Articles/161871/Fast-Colored-TextBox-for-syntax-highlighting
- NuGet 包: https://www.nuget.org/packages/VAX-FCTB/
- GitHub: https://github.com/xiaoyuvax/FastColoredTextBox
