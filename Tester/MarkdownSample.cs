using FastColoredTextBoxNS;
using FastColoredTextBoxNS.Text;
using FastColoredTextBoxNS.Types;

namespace Tester
{
    public partial class MarkdownSample : Form
    {
        public MarkdownSample()
        {
            InitializeComponent();
        }


        private void MarkdownSample_Load(object sender, EventArgs e)
        {
            fctb.Font = new Font("Consolas", 12f);
            fctb.Language = Language.Markdown;
            fctb.Text = @"# Markdown Syntax Highlighting Demo

## Text Formatting

This is **bold text** and this is *italic text*.
You can also use ~~strikethrough~~ text.

### Code

Inline code: `var x = 42;`

Fenced code block:

```csharp
public class Hello
{
    static void Main()
    {
        Console.WriteLine(""Hello, World!"");
    }
}
```

### Links and Images

Visit [Google](https://www.google.com) for search.

![Alt text](image.png)

### Blockquote

> This is a blockquote.
> It can span multiple lines.

### Lists

Unordered list:
- Item 1
- Item 2
- Item 3

Ordered list:
1. First
2. Second
3. Third

---

## Heading Levels

# H1 Heading
## H2 Heading
### H3 Heading
#### H4 Heading
##### H5 Heading
###### H6 Heading

This sample demonstrates **Markdown** rendering with *different font sizes* for headings using **Language.Markdown**.";
        }
    }
}
