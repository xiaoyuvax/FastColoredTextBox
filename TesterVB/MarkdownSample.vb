Imports System.ComponentModel
Imports FastColoredTextBoxNS
Imports FastColoredTextBoxNS.Text
Imports FastColoredTextBoxNS.Types

Namespace TesterVB
	Public Class MarkdownSample
		Inherits Form

		Private ReadOnly components As IContainer = Nothing
		Private fctb As FastColoredTextBox
		Private label1 As Label
		Private btnTheme As Button
		Private dark As Boolean

		Protected Overrides Sub Dispose(disposing As Boolean)
			If disposing AndAlso Me.components IsNot Nothing Then
				Me.components.Dispose()
			End If
			MyBase.Dispose(disposing)
		End Sub

		Private Sub InitializeComponent()
			Me.label1 = New Label()
			Me.btnTheme = New Button()
			Me.fctb = New FastColoredTextBox()
			MyBase.SuspendLayout()
			Me.label1.Dock = DockStyle.Top
			Me.label1.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204)
			Me.label1.Location = New Point(0, 28)
			Me.label1.Name = "label1"
			Me.label1.Size = New Size(784, 35)
			Me.label1.TabIndex = 1
			Me.label1.Text = "Markdown syntax highlighting sample. Uses built-in Language.Markdown highlighter."
			Me.label1.TextAlign = ContentAlignment.MiddleCenter
			Me.btnTheme.Dock = DockStyle.Top
			Me.btnTheme.Location = New Point(0, 0)
			Me.btnTheme.Name = "btnTheme"
			Me.btnTheme.Size = New Size(784, 28)
			Me.btnTheme.TabIndex = 2
			Me.btnTheme.Text = "Toggle dark theme (markdown colours are derived from BackColor)"
			Me.btnTheme.UseVisualStyleBackColor = True
			AddHandler Me.btnTheme.Click, New EventHandler(AddressOf Me.btnTheme_Click)
			Me.fctb.AutoScrollMinSize = New Size(25, 15)
			Me.fctb.BackBrush = Nothing
			Me.fctb.Cursor = Cursors.IBeam
			Me.fctb.DescriptionFile = ""
			Me.fctb.DisabledColor = Color.FromArgb(100, 180, 180, 180)
			Me.fctb.Dock = DockStyle.Fill
			Me.fctb.Font = New Font("Consolas", 9.75F)
			Me.fctb.Location = New Point(0, 63)
			Me.fctb.Name = "fctb"
			Me.fctb.Paddings = New Padding(0)
			Me.fctb.SelectionColor = Color.FromArgb(50, 0, 0, 255)
			Me.fctb.Size = New Size(784, 498)
			Me.fctb.TabIndex = 0
			MyBase.AutoScaleDimensions = New SizeF(7.0F, 17.0F)
			MyBase.AutoScaleMode = AutoScaleMode.Font
			MyBase.ClientSize = New Size(784, 561)
			MyBase.Controls.Add(Me.fctb)
			MyBase.Controls.Add(Me.label1)
			MyBase.Controls.Add(Me.btnTheme)
			MyBase.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204)
			MyBase.Name = "MarkdownSample"
			Me.Text = "Markdown Syntax Highlighting Sample"
			AddHandler Me.Load, New EventHandler(AddressOf Me.MarkdownSample_Load)
			MyBase.ResumeLayout(False)
		End Sub

		Public Sub New()
			Me.InitializeComponent()
		End Sub

		Private Sub MarkdownSample_Load(sender As Object, e As EventArgs)
			fctb.Font = New Font("Consolas", 12.0F)
			fctb.Language = Language.Markdown
			'the markdown highlighter keeps fenced-code-block state, so re-highlight what is on
			'screen rather than only the edited line
			fctb.HighlightingRangeType = HighlightingRangeType.VisibleRange
			fctb.ShowFoldingLines = True
			fctb.Text = "# Markdown syntax highlighting" & vbCrLf &
				"" & vbCrLf &
				"## Headings keep their level by shade" & vbCrLf &
				"" & vbCrLf &
				"The control is a fixed-width grid, so a heading cannot use a bigger font without" & vbCrLf &
				"breaking the caret, the selection and word wrap. Levels are told apart by colour:" & vbCrLf &
				"" & vbCrLf &
				"# H1" & vbCrLf &
				"## H2" & vbCrLf &
				"### H3" & vbCrLf &
				"#### H4" & vbCrLf &
				"##### H5" & vbCrLf &
				"###### H6" & vbCrLf &
				"" & vbCrLf &
				"## Emphasis" & vbCrLf &
				"" & vbCrLf &
				"**bold**, __also bold__, *italic*, _also italic_, ~~struck out~~ and `inline code`." & vbCrLf &
				"Math stays plain: 2 * 3 * 4, and so do snake_case_names and #hashtags." & vbCrLf &
				"Escaped markers are literal: \*not italic\*" & vbCrLf &
				"" & vbCrLf &
				"## Links and images" & vbCrLf &
				"" & vbCrLf &
				"[Google](https://www.google.com), <https://github.com/xiaoyuvax/FastColoredTextBox>" & vbCrLf &
				"and <someone@example.com>. Images: ![Alt text](image.png)" & vbCrLf &
				"" & vbCrLf &
				"## Blockquotes" & vbCrLf &
				"" & vbCrLf &
				"> A quote is a container: the tint and the bar are drawn by a plain Style, so" & vbCrLf &
				"> **bold** and `code` nested inside it still render." & vbCrLf &
				"" & vbCrLf &
				">> Nested quotes work too." & vbCrLf &
				"" & vbCrLf &
				"## Lists" & vbCrLf &
				"" & vbCrLf &
				"- plain item" & vbCrLf &
				"- item with **bold** and `code`" & vbCrLf &
				"1) ordered with a parenthesis" & vbCrLf &
				"2) another one" & vbCrLf &
				"- [x] a finished task" & vbCrLf &
				"- [ ] an open task" & vbCrLf &
				"" & vbCrLf &
				"## Code blocks" & vbCrLf &
				"" & vbCrLf &
				"A fence is protected: nothing below is parsed as markdown, and the info string" & vbCrLf &
				"picks the language of the content. The built-in modes are csharp, vb, java, js," & vbCrLf &
				"json, php, html, xml, sql and lua." & vbCrLf &
				"" & vbCrLf &
				"```csharp" & vbCrLf &
				"// the block is highlighted as C#" & vbCrLf &
				"public class Hello" & vbCrLf &
				"{" & vbCrLf &
				"    static void Main() => Console.WriteLine(""Hello, World!"");" & vbCrLf &
				"}" & vbCrLf &
				"```" & vbCrLf &
				"" & vbCrLf &
				"```lua" & vbCrLf &
				"-- tilde fences work as well" & vbCrLf &
				"local function f(x)" & vbCrLf &
				"  return 2 * 3 * 4" & vbCrLf &
				"end" & vbCrLf &
				"```" & vbCrLf &
				"" & vbCrLf &
				"```" & vbCrLf &
				"A fence with no info string stays plain." & vbCrLf &
				"```" & vbCrLf &
				"" & vbCrLf &
				"## Thematic breaks" & vbCrLf &
				"" & vbCrLf &
				"These are rules, not lists:" & vbCrLf &
				"" & vbCrLf &
				"- - -" & vbCrLf &
				"***" & vbCrLf &
				"___" & vbCrLf &
				"" & vbCrLf &
				"## Known limits" & vbCrLf &
				"" & vbCrLf &
				"Nested inline emphasis (**bold with *italic* inside**) shows the outer style; set" & vbCrLf &
				"fctb.AllowSeveralTextStyleDrawing to let both render. Lazy blockquote" & vbCrLf &
				"continuation, reference links, setext headings, tables and indented code blocks" & vbCrLf &
				"are not highlighted. Pressing Enter after a list item indents the next line but" & vbCrLf &
				"does not copy the ""- "" marker." & vbCrLf &
				"" & vbCrLf &
				"## An unterminated fence" & vbCrLf &
				"" & vbCrLf &
				"Everything below this line is code, as in CommonMark. Type three more backticks" & vbCrLf &
				"and it becomes Markdown again." & vbCrLf &
				"" & vbCrLf &
				"~~~" & vbCrLf &
				"still code, and nothing after the fence is parsed as markdown"
		End Sub

		Private Sub btnTheme_Click(sender As Object, e As EventArgs)
			Me.dark = Not Me.dark
			If Me.dark Then
				fctb.BackColor = Color.FromArgb(30, 30, 30)
				fctb.ForeColor = Color.Gainsboro
				fctb.LineNumberColor = Color.DimGray
				fctb.CurrentLineColor = Color.FromArgb(45, 45, 45)
			Else
				fctb.BackColor = Color.White
				fctb.ForeColor = Color.Black
				fctb.LineNumberColor = Color.Teal
				fctb.CurrentLineColor = Color.WhiteSmoke
			End If
		End Sub
	End Class
End Namespace
