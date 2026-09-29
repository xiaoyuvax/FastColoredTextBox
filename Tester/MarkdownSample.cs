using System;
using System.Drawing;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using FastColoredTextBoxNS.Text;
using FastColoredTextBoxNS.Types;

namespace Tester
{
    public partial class MarkdownSample : Form
    {
        private static readonly Color DarkBack = Color.FromArgb(30, 30, 30);
        private bool dark;

        public MarkdownSample()
        {
            InitializeComponent();
        }

        private void MarkdownSample_Load(object sender, EventArgs e)
        {
            fctb.Font = new Font("Consolas", 12f);
            fctb.Language = Language.Markdown;
            //the markdown highlighter keeps fenced-code-block state, so re-highlight what is on
            //screen rather than only the edited line
            fctb.HighlightingRangeType = HighlightingRangeType.VisibleRange;
            fctb.ShowFoldingLines = true;
            fctb.Text = @"# Markdown syntax highlighting

## Headings keep their level by shade

The control is a fixed-width grid, so a heading cannot use a bigger font without
breaking the caret, the selection and word wrap. Levels are told apart by colour:

# H1
## H2
### H3
#### H4
##### H5
###### H6

## Emphasis

**bold**, __also bold__, *italic*, _also italic_, ~~struck out~~ and `inline code`.
Math stays plain: 2 * 3 * 4, and so do snake_case_names and #hashtags.
Escaped markers are literal: \*not italic\*

## Links and images

[Google](https://www.google.com), <https://github.com/xiaoyuvax/FastColoredTextBox>
and <someone@example.com>. Images: ![Alt text](image.png)

## Blockquotes

> A quote is a container: the tint and the bar are drawn by a plain Style, so
> **bold** and `code` nested inside it still render.

>> Nested quotes work too.

## Lists

- plain item
- item with **bold** and `code`
1) ordered with a parenthesis
2) another one
- [x] a finished task
- [ ] an open task

## Code blocks

A fence is protected: nothing below is parsed as markdown, and the info string
picks the language of the content. The built-in modes are csharp, vb, java, js,
json, php, html, xml, sql and lua.

```csharp
// the block is highlighted as C#
public class Hello
{
    static void Main() => Console.WriteLine(""Hello, World!"");
}
```

```lua
-- tilde fences work as well
local function f(x)
  return 2 * 3 * 4
end
```

```
A fence with no info string stays plain.
```

## Thematic breaks

These are rules, not lists:

- - -
***
___

## Known limits

Nested inline emphasis (**bold with *italic* inside**) shows the outer style; set
fctb.AllowSeveralTextStyleDrawing to let both render. Lazy blockquote
continuation, reference links, setext headings, tables and indented code blocks
are not highlighted. Pressing Enter after a list item indents the next line but
does not copy the ""- "" marker.

## An unterminated fence

Everything below this line is code, as in CommonMark. Type three more backticks
and it becomes Markdown again.

~~~
still code, and nothing after the fence is parsed as markdown";

        }

        private void btnTheme_Click(object sender, EventArgs e)
        {
            dark = !dark;
            fctb.BackColor = dark ? DarkBack : Color.White;
            fctb.ForeColor = dark ? Color.Gainsboro : Color.Black;
            fctb.LineNumberColor = dark ? Color.DimGray : Color.Teal;
            fctb.CurrentLineColor = dark ? Color.FromArgb(45, 45, 45) : Color.WhiteSmoke;
        }
    }
}
