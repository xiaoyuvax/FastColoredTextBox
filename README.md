FastColoredTextBox
==================

Fast Colored TextBox is text editor component for .NET.
Allows you to create custom text editor with syntax highlighting.
It works well with small, medium, large and very-very large files.

It has such settings as foreground color, font style, background color which can be adjusted for arbitrarily selected text symbols. One can easily gain access to a text with the use of regular expressions. WordWrap, Find/Replace, Code folding and multilevel Undo/Redo are supported as well. 

## Update Logs   
24-12-24 (V2.16.27.103)
- Add UseCJK property, may need further improvements.

24-12-20 (V2.16.27.100)
- Start to support correct displaying Chinese based on the lately found fork by Daxanius, though not perfect, still needs some tweets in later version
- CN:在能找到的最新的Daxanius版本上增加了汉字显示支持，虽不完美还需在以后版本调整
- Multitargeting net6.0-8.0-windows7.0
- Upgrade to C#12 sematics


## Known Issues
- Selection position may be incorrect if any CJK characters mixed with latin letters in line.This requires a systematic refactoration of relevant codes.

![Fast Colored TextBox](http://www.codeproject.com/KB/edit/FastColoredTextBox_/fastcoloredtextbox2.png)

![image](https://github.com/user-attachments/assets/57038914-88ca-4dc3-9b44-07b0bf9ff2da)


More details http://www.codeproject.com/Articles/161871/Fast-Colored-TextBox-for-syntax-highlighting

Nuget package https://www.nuget.org/packages/VAX-FCTB/
