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

		Protected Overrides Sub Dispose(disposing As Boolean)
			If disposing AndAlso Me.components IsNot Nothing Then
				Me.components.Dispose()
			End If
			MyBase.Dispose(disposing)
		End Sub

		Private Sub InitializeComponent()
			Me.label1 = New Label()
			Me.fctb = New FastColoredTextBox()
			MyBase.SuspendLayout()
			Me.label1.Dock = DockStyle.Top
			Me.label1.Font = New Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204)
			Me.label1.Location = New Point(0, 0)
			Me.label1.Name = "label1"
			Me.label1.Size = New Size(784, 35)
			Me.label1.TabIndex = 1
			Me.label1.Text = "Markdown syntax highlighting sample. Uses built-in Language.Markdown highlighter."
			Me.label1.TextAlign = ContentAlignment.MiddleCenter
			Me.fctb.AutoScrollMinSize = New Size(25, 15)
			Me.fctb.BackBrush = Nothing
			Me.fctb.Cursor = Cursors.IBeam
			Me.fctb.DescriptionFile = ""
			Me.fctb.DisabledColor = Color.FromArgb(100, 180, 180, 180)
			Me.fctb.Dock = DockStyle.Fill
			Me.fctb.Font = New Font("Consolas", 9.75F)
			Me.fctb.Location = New Point(0, 35)
			Me.fctb.Name = "fctb"
			Me.fctb.Paddings = New Padding(0)
			Me.fctb.SelectionColor = Color.FromArgb(50, 0, 0, 255)
			Me.fctb.Size = New Size(784, 526)
			Me.fctb.TabIndex = 0
			MyBase.AutoScaleDimensions = New SizeF(7.0F, 17.0F)
			MyBase.AutoScaleMode = AutoScaleMode.Font
			MyBase.ClientSize = New Size(784, 561)
			MyBase.Controls.Add(Me.fctb)
			MyBase.Controls.Add(Me.label1)
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
			fctb.Text = "# Markdown Syntax Highlighting Demo" & vbCrLf &
				"" & vbCrLf &
				"## Text Formatting" & vbCrLf &
				"" & vbCrLf &
				"This is **bold text** and this is *italic text*." & vbCrLf &
				"You can also use ~~strikethrough~~ text." & vbCrLf &
				"" & vbCrLf &
				"### Code" & vbCrLf &
				"" & vbCrLf &
				"Inline code: `var x = 42;`" & vbCrLf &
				"" & vbCrLf &
				"Fenced code block:" & vbCrLf &
				"" & vbCrLf &
				"```csharp" & vbCrLf &
				"public class Hello" & vbCrLf &
				"{" & vbCrLf &
				"    static void Main()" & vbCrLf &
				"    {" & vbCrLf &
				"        Console.WriteLine(""Hello, World!"");" & vbCrLf &
				"    }" & vbCrLf &
				"}" & vbCrLf &
				"```" & vbCrLf &
				"" & vbCrLf &
				"### Links and Images" & vbCrLf &
				"" & vbCrLf &
				"Visit [Google](https://www.google.com) for search." & vbCrLf &
				"" & vbCrLf &
				"![Alt text](image.png)" & vbCrLf &
				"" & vbCrLf &
				"### Blockquote" & vbCrLf &
				"" & vbCrLf &
				"> This is a blockquote." & vbCrLf &
				"> It can span multiple lines." & vbCrLf &
				"" & vbCrLf &
				"### Lists" & vbCrLf &
				"" & vbCrLf &
				"Unordered list:" & vbCrLf &
				"- Item 1" & vbCrLf &
				"- Item 2" & vbCrLf &
				"- Item 3" & vbCrLf &
				"" & vbCrLf &
				"Ordered list:" & vbCrLf &
				"1. First" & vbCrLf &
				"2. Second" & vbCrLf &
				"3. Third" & vbCrLf &
				"" & vbCrLf &
				"---" & vbCrLf &
				"" & vbCrLf &
				"## Heading Levels" & vbCrLf &
				"" & vbCrLf &
				"# H1 Heading" & vbCrLf &
				"## H2 Heading" & vbCrLf &
				"### H3 Heading" & vbCrLf &
				"#### H4 Heading" & vbCrLf &
				"##### H5 Heading" & vbCrLf &
				"###### H6 Heading" & vbCrLf &
				"" & vbCrLf &
				"This sample demonstrates built-in Markdown syntax highlighting with **Language.Markdown**."
		End Sub
	End Class
End Namespace
