Imports System.ComponentModel

Namespace TesterVB

	Public Class MainForm
		Inherits Form

		Private ReadOnly components As IContainer = Nothing
		Private WithEvents Button1 As Button
		Private WithEvents Label1 As Label
		Private WithEvents Label2 As Label
		Private WithEvents Button2 As Button
		Private WithEvents Label3 As Label
		Private WithEvents Button3 As Button
		Private WithEvents Label4 As Label
		Private WithEvents Button4 As Button
		Private WithEvents Label5 As Label
		Private WithEvents Button5 As Button
		Private WithEvents Label6 As Label
		Private WithEvents Button6 As Button
		Private WithEvents Label7 As Label
		Private WithEvents Button7 As Button
		Private WithEvents Label8 As Label
		Private WithEvents Button8 As Button
		Private WithEvents Label9 As Label
		Private WithEvents Button9 As Button
		Private WithEvents Label10 As Label
		Private WithEvents Button10 As Button
		Private WithEvents Label11 As Label
		Private WithEvents Button11 As Button
		Private WithEvents Label12 As Label
		Private WithEvents Button12 As Button
		Private WithEvents Label13 As Label
		Private WithEvents Button13 As Button
		Private WithEvents Label14 As Label
		Private WithEvents Button14 As Button
		Private WithEvents Label15 As Label
		Private WithEvents Button15 As Button
		Private WithEvents Button16 As Button
		Private WithEvents Label16 As Label
		Private WithEvents Label17 As Label
		Private WithEvents Button17 As Button
		Private WithEvents Label18 As Label
		Private WithEvents Button18 As Button
		Private WithEvents Label19 As Label
		Private WithEvents Button19 As Button
		Private WithEvents Label20 As Label
		Private WithEvents Button20 As Button
		Private WithEvents Label21 As Label
		Private WithEvents Button21 As Button
		Private WithEvents Label22 As Label
		Private WithEvents Button22 As Button

		Protected Overrides Sub Dispose(disposing As Boolean)
			If disposing AndAlso Me.components IsNot Nothing Then
				Me.components.Dispose()
			End If
			MyBase.Dispose(disposing)
		End Sub

		Private Sub InitializeComponent()
			Me.Button1 = New System.Windows.Forms.Button()
			Me.Label1 = New System.Windows.Forms.Label()
			Me.Label2 = New System.Windows.Forms.Label()
			Me.Button2 = New System.Windows.Forms.Button()
			Me.Label3 = New System.Windows.Forms.Label()
			Me.Button3 = New System.Windows.Forms.Button()
			Me.Label4 = New System.Windows.Forms.Label()
			Me.Button4 = New System.Windows.Forms.Button()
			Me.Label5 = New System.Windows.Forms.Label()
			Me.Button5 = New System.Windows.Forms.Button()
			Me.Label6 = New System.Windows.Forms.Label()
			Me.Button6 = New System.Windows.Forms.Button()
			Me.Label7 = New System.Windows.Forms.Label()
			Me.Button7 = New System.Windows.Forms.Button()
			Me.Label8 = New System.Windows.Forms.Label()
			Me.Button8 = New System.Windows.Forms.Button()
			Me.Label9 = New System.Windows.Forms.Label()
			Me.Button9 = New System.Windows.Forms.Button()
			Me.Label10 = New System.Windows.Forms.Label()
			Me.Button10 = New System.Windows.Forms.Button()
			Me.Label11 = New System.Windows.Forms.Label()
			Me.Button11 = New System.Windows.Forms.Button()
			Me.Label12 = New System.Windows.Forms.Label()
			Me.Button12 = New System.Windows.Forms.Button()
			Me.Label13 = New System.Windows.Forms.Label()
			Me.Button13 = New System.Windows.Forms.Button()
			Me.Label14 = New System.Windows.Forms.Label()
			Me.Button14 = New System.Windows.Forms.Button()
			Me.Label15 = New System.Windows.Forms.Label()
			Me.Button15 = New System.Windows.Forms.Button()
			Me.Button16 = New System.Windows.Forms.Button()
			Me.Label16 = New System.Windows.Forms.Label()
			Me.Label17 = New System.Windows.Forms.Label()
			Me.Button17 = New System.Windows.Forms.Button()
			Me.Label18 = New System.Windows.Forms.Label()
			Me.Button18 = New System.Windows.Forms.Button()
			Me.Label19 = New System.Windows.Forms.Label()
			Me.Button19 = New System.Windows.Forms.Button()
			Me.Label20 = New System.Windows.Forms.Label()
			Me.Button20 = New System.Windows.Forms.Button()
			Me.Label21 = New System.Windows.Forms.Label()
			Me.Button21 = New System.Windows.Forms.Button()
			Me.Label22 = New System.Windows.Forms.Label()
			Me.Button22 = New System.Windows.Forms.Button()
			Me.Label23 = New System.Windows.Forms.Label()
			Me.SuspendLayout()
			'
			'button1
			'
			Me.Button1.Location = New System.Drawing.Point(222, 3)
			Me.Button1.Name = "button1"
			Me.Button1.Size = New System.Drawing.Size(75, 23)
			Me.Button1.TabIndex = 0
			Me.Button1.Text = "Show"
			Me.Button1.UseVisualStyleBackColor = True
			'
			'label1
			'
			Me.Label1.Location = New System.Drawing.Point(8, 4)
			Me.Label1.Name = "label1"
			Me.Label1.Size = New System.Drawing.Size(208, 34)
			Me.Label1.TabIndex = 1
			Me.Label1.Text = "Powerful sample. It shows syntax highlighting and many features."
			Me.Label1.TextAlign = System.Drawing.ContentAlignment.TopRight
			'
			'label2
			'
			Me.Label2.Location = New System.Drawing.Point(8, 82)
			Me.Label2.Name = "label2"
			Me.Label2.Size = New System.Drawing.Size(208, 30)
			Me.Label2.TabIndex = 3
			Me.Label2.Text = "Marker sample. It shows how to make marker tool."
			Me.Label2.TextAlign = System.Drawing.ContentAlignment.TopRight
			'
			'button2
			'
			Me.Button2.Location = New System.Drawing.Point(222, 82)
			Me.Button2.Name = "button2"
			Me.Button2.Size = New System.Drawing.Size(75, 23)
			Me.Button2.TabIndex = 2
			Me.Button2.Text = "Show"
			Me.Button2.UseVisualStyleBackColor = True
			'
			'label3
			'
			Me.Label3.Location = New System.Drawing.Point(8, 122)
			Me.Label3.Name = "label3"
			Me.Label3.Size = New System.Drawing.Size(208, 30)
			Me.Label3.TabIndex = 5
			Me.Label3.Text = "Custom style sample. This example shows how to create own custom style."
			Me.Label3.TextAlign = System.Drawing.ContentAlignment.TopRight
			'
			'button3
			'
			Me.Button3.Location = New System.Drawing.Point(222, 122)
			Me.Button3.Name = "button3"
			Me.Button3.Size = New System.Drawing.Size(75, 23)
			Me.Button3.TabIndex = 4
			Me.Button3.Text = "Show"
			Me.Button3.UseVisualStyleBackColor = True
			'
			'label4
			'
			Me.Label4.Location = New System.Drawing.Point(8, 159)
			Me.Label4.Name = "label4"
			Me.Label4.Size = New System.Drawing.Size(208, 56)
			Me.Label4.TabIndex = 7
			Me.Label4.Text = "VisibleRangeChangedDelayed usage sample. This example shows how to highlight synt" &
		"ax for extremally large text by VisibleRangeChangedDelayed event."
			Me.Label4.TextAlign = System.Drawing.ContentAlignment.TopRight
			'
			'button4
			'
			Me.Button4.Location = New System.Drawing.Point(222, 173)
			Me.Button4.Name = "button4"
			Me.Button4.Size = New System.Drawing.Size(75, 23)
			Me.Button4.TabIndex = 6
			Me.Button4.Text = "Show"
			Me.Button4.UseVisualStyleBackColor = True
			'
			'label5
			'
			Me.Label5.Location = New System.Drawing.Point(11, 35)
			Me.Label5.Name = "label5"
			Me.Label5.Size = New System.Drawing.Size(205, 44)
			Me.Label5.TabIndex = 9
			Me.Label5.Text = "Simplest custom syntax highlighting sample. It shows how to make custom syntax hi" &
		"ghlighting."
			Me.Label5.TextAlign = System.Drawing.ContentAlignment.TopRight
			'
			'button5
			'
			Me.Button5.Location = New System.Drawing.Point(222, 40)
			Me.Button5.Name = "button5"
			Me.Button5.Size = New System.Drawing.Size(75, 23)
			Me.Button5.TabIndex = 8
			Me.Button5.Text = "Show"
			Me.Button5.UseVisualStyleBackColor = True
			'
			'label6
			'
			Me.Label6.Location = New System.Drawing.Point(8, 429)
			Me.Label6.Name = "label6"
			Me.Label6.Size = New System.Drawing.Size(208, 26)
			Me.Label6.TabIndex = 11
			Me.Label6.Text = "Joke sample :)"
			Me.Label6.TextAlign = System.Drawing.ContentAlignment.TopRight
			'
			'button6
			'
			Me.Button6.Location = New System.Drawing.Point(222, 424)
			Me.Button6.Name = "button6"
			Me.Button6.Size = New System.Drawing.Size(75, 23)
			Me.Button6.TabIndex = 10
			Me.Button6.Text = "Show"
			Me.Button6.UseVisualStyleBackColor = True
			'
			'label7
			'
			Me.Label7.Location = New System.Drawing.Point(315, 4)
			Me.Label7.Name = "label7"
			Me.Label7.Size = New System.Drawing.Size(208, 41)
			Me.Label7.TabIndex = 13
			Me.Label7.Text = "Simplest code folding sample. This example shows how to make simplest code foldin" &
		"g."
			Me.Label7.TextAlign = System.Drawing.ContentAlignment.TopRight
			'
			'button7
			'
			Me.Button7.Location = New System.Drawing.Point(529, 4)
			Me.Button7.Name = "button7"
			Me.Button7.Size = New System.Drawing.Size(75, 23)
			Me.Button7.TabIndex = 12
			Me.Button7.Text = "Show"
			Me.Button7.UseVisualStyleBackColor = True
			'
			'label8
			'
			Me.Label8.Location = New System.Drawing.Point(315, 87)
			Me.Label8.Name = "label8"
			Me.Label8.Size = New System.Drawing.Size(208, 41)
			Me.Label8.TabIndex = 15
			Me.Label8.Text = "Autocomplete sample. This example shows simplest way to create autocomplete funct" &
		"ionality."
			Me.Label8.TextAlign = System.Drawing.ContentAlignment.TopRight
			'
			'button8
			'
			Me.Button8.Location = New System.Drawing.Point(529, 98)
			Me.Button8.Name = "button8"
			Me.Button8.Size = New System.Drawing.Size(75, 23)
			Me.Button8.TabIndex = 14
			Me.Button8.Text = "Show"
			Me.Button8.UseVisualStyleBackColor = True
			'
			'label9
			'
			Me.Label9.Location = New System.Drawing.Point(315, 220)
			Me.Label9.Name = "label9"
			Me.Label9.Size = New System.Drawing.Size(208, 59)
			Me.Label9.TabIndex = 17
			Me.Label9.Text = "Dynamic syntax highlighting. This example finds the functions declared in the pro" &
		"gram and dynamically highlights all of their entry into the code of LISP."
			Me.Label9.TextAlign = System.Drawing.ContentAlignment.TopRight
			'
			'button9
			'
			Me.Button9.Location = New System.Drawing.Point(529, 234)
			Me.Button9.Name = "button9"
			Me.Button9.Size = New System.Drawing.Size(75, 23)
			Me.Button9.TabIndex = 16
			Me.Button9.Text = "Show"
			Me.Button9.UseVisualStyleBackColor = True
			'
			'label10
			'
			Me.Label10.Location = New System.Drawing.Point(315, 285)
			Me.Label10.Name = "label10"
			Me.Label10.Size = New System.Drawing.Size(208, 45)
			Me.Label10.TabIndex = 19
			Me.Label10.Text = "Syntax highlighting by XML description file. This example shows how to use XML fi" &
		"le for description of syntax highlighting."
			Me.Label10.TextAlign = System.Drawing.ContentAlignment.TopRight
			'
			'button10
			'
			Me.Button10.Location = New System.Drawing.Point(529, 292)
			Me.Button10.Name = "button10"
			Me.Button10.Size = New System.Drawing.Size(75, 23)
			Me.Button10.TabIndex = 18
			Me.Button10.Text = "Show"
			Me.Button10.UseVisualStyleBackColor = True
			'
			'label11
			'
			Me.Label11.Location = New System.Drawing.Point(315, 339)
			Me.Label11.Name = "label11"
			Me.Label11.Size = New System.Drawing.Size(208, 37)
			Me.Label11.TabIndex = 21
			Me.Label11.Text = "This example supports IME entering mode and rendering of wide characters."
			Me.Label11.TextAlign = System.Drawing.ContentAlignment.TopRight
			'
			'button11
			'
			Me.Button11.Location = New System.Drawing.Point(529, 339)
			Me.Button11.Name = "button11"
			Me.Button11.Size = New System.Drawing.Size(75, 23)
			Me.Button11.TabIndex = 20
			Me.Button11.Text = "Show"
			Me.Button11.UseVisualStyleBackColor = True
			'
			'label12
			'
			Me.Label12.Location = New System.Drawing.Point(8, 228)
			Me.Label12.Name = "label12"
			Me.Label12.Size = New System.Drawing.Size(208, 26)
			Me.Label12.TabIndex = 23
			Me.Label12.Text = "Powerfull C# source file editor"
			Me.Label12.TextAlign = System.Drawing.ContentAlignment.TopRight
			'
			'button12
			'
			Me.Button12.Location = New System.Drawing.Point(222, 223)
			Me.Button12.Name = "button12"
			Me.Button12.Size = New System.Drawing.Size(75, 23)
			Me.Button12.TabIndex = 22
			Me.Button12.Text = "Show"
			Me.Button12.UseVisualStyleBackColor = True
			'
			'label13
			'
			Me.Label13.Location = New System.Drawing.Point(8, 265)
			Me.Label13.Name = "label13"
			Me.Label13.Size = New System.Drawing.Size(208, 26)
			Me.Label13.TabIndex = 25
			Me.Label13.Text = "Example of image drawing"
			Me.Label13.TextAlign = System.Drawing.ContentAlignment.TopRight
			'
			'button13
			'
			Me.Button13.Location = New System.Drawing.Point(222, 265)
			Me.Button13.Name = "button13"
			Me.Button13.Size = New System.Drawing.Size(75, 23)
			Me.Button13.TabIndex = 24
			Me.Button13.Text = "Show"
			Me.Button13.UseVisualStyleBackColor = True
			'
			'label14
			'
			Me.Label14.Location = New System.Drawing.Point(315, 135)
			Me.Label14.Name = "label14"
			Me.Label14.Size = New System.Drawing.Size(208, 41)
			Me.Label14.TabIndex = 27
			Me.Label14.Text = "Autocomplete sample 2." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "This example demonstrates more flexible variant of Autoco" &
		"mpleteMenu using."
			Me.Label14.TextAlign = System.Drawing.ContentAlignment.TopRight
			'
			'button14
			'
			Me.Button14.Location = New System.Drawing.Point(529, 141)
			Me.Button14.Name = "button14"
			Me.Button14.Size = New System.Drawing.Size(75, 23)
			Me.Button14.TabIndex = 26
			Me.Button14.Text = "Show"
			Me.Button14.UseVisualStyleBackColor = True
			'
			'label15
			'
			Me.Label15.Location = New System.Drawing.Point(315, 387)
			Me.Label15.Name = "label15"
			Me.Label15.Size = New System.Drawing.Size(208, 23)
			Me.Label15.TabIndex = 29
			Me.Label15.Text = "AutoIndent sample"
			Me.Label15.TextAlign = System.Drawing.ContentAlignment.TopRight
			'
			'button15
			'
			Me.Button15.Location = New System.Drawing.Point(529, 382)
			Me.Button15.Name = "button15"
			Me.Button15.Size = New System.Drawing.Size(75, 23)
			Me.Button15.TabIndex = 28
			Me.Button15.Text = "Show"
			Me.Button15.UseVisualStyleBackColor = True
			'
			'button16
			'
			Me.Button16.Location = New System.Drawing.Point(529, 425)
			Me.Button16.Name = "button16"
			Me.Button16.Size = New System.Drawing.Size(75, 23)
			Me.Button16.TabIndex = 30
			Me.Button16.Text = "Show"
			Me.Button16.UseVisualStyleBackColor = True
			'
			'label16
			'
			Me.Label16.Location = New System.Drawing.Point(315, 430)
			Me.Label16.Name = "label16"
			Me.Label16.Size = New System.Drawing.Size(208, 23)
			Me.Label16.TabIndex = 31
			Me.Label16.Text = "Bookmarks sample"
			Me.Label16.TextAlign = System.Drawing.ContentAlignment.TopRight
			'
			'label17
			'
			Me.Label17.Location = New System.Drawing.Point(8, 306)
			Me.Label17.Name = "label17"
			Me.Label17.Size = New System.Drawing.Size(208, 26)
			Me.Label17.TabIndex = 33
			Me.Label17.Text = "Logger sample. It shows how to add text with predefined style."
			Me.Label17.TextAlign = System.Drawing.ContentAlignment.TopRight
			'
			'button17
			'
			Me.Button17.Location = New System.Drawing.Point(222, 306)
			Me.Button17.Name = "button17"
			Me.Button17.Size = New System.Drawing.Size(75, 23)
			Me.Button17.TabIndex = 32
			Me.Button17.Text = "Show"
			Me.Button17.UseVisualStyleBackColor = True
			'
			'label18
			'
			Me.Label18.Location = New System.Drawing.Point(315, 185)
			Me.Label18.Name = "label18"
			Me.Label18.Size = New System.Drawing.Size(208, 26)
			Me.Label18.TabIndex = 35
			Me.Label18.Text = "Tooltip sample. It shows tooltips for words under mouse."
			Me.Label18.TextAlign = System.Drawing.ContentAlignment.TopRight
			'
			'button18
			'
			Me.Button18.Location = New System.Drawing.Point(529, 185)
			Me.Button18.Name = "button18"
			Me.Button18.Size = New System.Drawing.Size(75, 23)
			Me.Button18.TabIndex = 34
			Me.Button18.Text = "Show"
			Me.Button18.UseVisualStyleBackColor = True
			'
			'label19
			'
			Me.Label19.Location = New System.Drawing.Point(8, 345)
			Me.Label19.Name = "label19"
			Me.Label19.Size = New System.Drawing.Size(208, 26)
			Me.Label19.TabIndex = 37
			Me.Label19.Text = "Split sample. This example shows how to make split-screen mode."
			Me.Label19.TextAlign = System.Drawing.ContentAlignment.TopRight
			'
			'button19
			'
			Me.Button19.Location = New System.Drawing.Point(222, 345)
			Me.Button19.Name = "button19"
			Me.Button19.Size = New System.Drawing.Size(75, 23)
			Me.Button19.TabIndex = 36
			Me.Button19.Text = "Show"
			Me.Button19.UseVisualStyleBackColor = True
			'
			'label20
			'
			Me.Label20.Location = New System.Drawing.Point(8, 386)
			Me.Label20.Name = "label20"
			Me.Label20.Size = New System.Drawing.Size(208, 26)
			Me.Label20.TabIndex = 39
			Me.Label20.Text = "Lazy loading sample."
			Me.Label20.TextAlign = System.Drawing.ContentAlignment.TopRight
			'
			'button20
			'
			Me.Button20.Location = New System.Drawing.Point(222, 386)
			Me.Button20.Name = "button20"
			Me.Button20.Size = New System.Drawing.Size(75, 23)
			Me.Button20.TabIndex = 38
			Me.Button20.Text = "Show"
			Me.Button20.UseVisualStyleBackColor = True
			'
			'label21
			'
			Me.Label21.Location = New System.Drawing.Point(315, 471)
			Me.Label21.Name = "label21"
			Me.Label21.Size = New System.Drawing.Size(208, 23)
			Me.Label21.TabIndex = 41
			Me.Label21.Text = "Console sample"
			Me.Label21.TextAlign = System.Drawing.ContentAlignment.TopRight
			'
			'button21
			'
			Me.Button21.Location = New System.Drawing.Point(529, 466)
			Me.Button21.Name = "button21"
			Me.Button21.Size = New System.Drawing.Size(75, 23)
			Me.Button21.TabIndex = 40
			Me.Button21.Text = "Show"
			Me.Button21.UseVisualStyleBackColor = True
			'
			'label22
			'
			Me.Label22.Location = New System.Drawing.Point(315, 53)
			Me.Label22.Name = "label22"
			Me.Label22.Size = New System.Drawing.Size(208, 26)
			Me.Label22.TabIndex = 43
			Me.Label22.Text = "Custom code folding sample."
			Me.Label22.TextAlign = System.Drawing.ContentAlignment.TopRight
			'
			'button22
			'
			Me.Button22.Location = New System.Drawing.Point(529, 48)
			Me.Button22.Name = "button22"
			Me.Button22.Size = New System.Drawing.Size(75, 23)
			Me.Button22.TabIndex = 42
			Me.Button22.Text = "Show"
			Me.Button22.UseVisualStyleBackColor = True
			'
			'Label23
			'
			Me.Label23.Location = New System.Drawing.Point(12, 503)
			Me.Label23.Name = "Label23"
			Me.Label23.Size = New System.Drawing.Size(550, 35)
			Me.Label23.TabIndex = 44
			Me.Label23.Text = "Note: In this project some examples are deprecated and/or not implemented. Newer " &
		"and full code samples see in project Tester (C#)."
			'
			'MainForm
			'
			Me.ClientSize = New System.Drawing.Size(608, 537)
			Me.Controls.Add(Me.Label23)
			Me.Controls.Add(Me.Label22)
			Me.Controls.Add(Me.Button22)
			Me.Controls.Add(Me.Label21)
			Me.Controls.Add(Me.Button21)
			Me.Controls.Add(Me.Label20)
			Me.Controls.Add(Me.Button20)
			Me.Controls.Add(Me.Label19)
			Me.Controls.Add(Me.Button19)
			Me.Controls.Add(Me.Label18)
			Me.Controls.Add(Me.Button18)
			Me.Controls.Add(Me.Label17)
			Me.Controls.Add(Me.Button17)
			Me.Controls.Add(Me.Label16)
			Me.Controls.Add(Me.Button16)
			Me.Controls.Add(Me.Label15)
			Me.Controls.Add(Me.Button15)
			Me.Controls.Add(Me.Label14)
			Me.Controls.Add(Me.Button14)
			Me.Controls.Add(Me.Label13)
			Me.Controls.Add(Me.Button13)
			Me.Controls.Add(Me.Label12)
			Me.Controls.Add(Me.Button12)
			Me.Controls.Add(Me.Label11)
			Me.Controls.Add(Me.Button11)
			Me.Controls.Add(Me.Label10)
			Me.Controls.Add(Me.Button10)
			Me.Controls.Add(Me.Label9)
			Me.Controls.Add(Me.Button9)
			Me.Controls.Add(Me.Label8)
			Me.Controls.Add(Me.Button8)
			Me.Controls.Add(Me.Label7)
			Me.Controls.Add(Me.Button7)
			Me.Controls.Add(Me.Label6)
			Me.Controls.Add(Me.Button6)
			Me.Controls.Add(Me.Label5)
			Me.Controls.Add(Me.Button5)
			Me.Controls.Add(Me.Label4)
			Me.Controls.Add(Me.Button4)
			Me.Controls.Add(Me.Label3)
			Me.Controls.Add(Me.Button3)
			Me.Controls.Add(Me.Label2)
			Me.Controls.Add(Me.Button2)
			Me.Controls.Add(Me.Label1)
			Me.Controls.Add(Me.Button1)
			Me.Name = "MainForm"
			Me.Text = "MainForm"
			Me.ResumeLayout(False)

		End Sub

		Public Sub New()
			Me.InitializeComponent()
		End Sub

		Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
			Dim x = New PowerfulSample
			x.Show()
		End Sub

		Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
			Dim x = New MarkerToolSample
			x.Show()
		End Sub

		Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
			Dim x = New CustomStyleSample
			x.Show()
		End Sub

		Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
			Me.Cursor = Cursors.WaitCursor
			Dim x = New VisibleRangeChangedDelayedSample
			x.Show()
			Me.Cursor = Cursors.[Default]
		End Sub

		Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
			Dim x = New SimplestSyntaxHighlightingSample
			x.Show()
		End Sub

		Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
			Dim x = New JokeSample
			x.Show()
		End Sub

		Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
			Dim x = New SimplestCodeFoldingSample
			x.Show()
		End Sub

		Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
			Dim x = New AutocompleteSample
			x.Show()
		End Sub

		Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
			Dim x = New DynamicSyntaxHighlighting
			x.Show()
		End Sub

		Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
			Dim x = New SyntaxHighlightingByXmlDescription
			x.Show()
		End Sub

		Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
			Dim x = New IMEsample
			x.Show()
		End Sub

		Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
			Dim x = New PowerfulCSharpEditor
			x.Show()
		End Sub

		Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
			Dim x = New GifImageDrawingSample
			x.Show()
		End Sub

		Private Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click
			Dim x = New AutocompleteSample2
			x.Show()
		End Sub

		Private Sub Button15_Click(sender As Object, e As EventArgs) Handles Button15.Click
			Dim x = New AutoIndentSample
			x.Show()
		End Sub

		Private Sub Button16_Click(sender As Object, e As EventArgs) Handles Button16.Click
			Dim x = New BookmarksSample
			x.Show()
		End Sub

		Private Sub Button17_Click(sender As Object, e As EventArgs) Handles Button17.Click
			Dim x = New LoggerSample
			x.Show()
		End Sub

		Private Sub Button18_Click(sender As Object, e As EventArgs) Handles Button18.Click
			Dim x = New TooltipSample
			x.Show()
		End Sub

		Private Sub Button19_Click(sender As Object, e As EventArgs) Handles Button19.Click
			Dim x = New SplitSample
			x.Show()
		End Sub

		Private Sub Button20_Click(sender As Object, e As EventArgs) Handles Button20.Click
			Dim x = New LazyLoadingSample
			x.Show()
		End Sub

		Private Sub Button21_Click(sender As Object, e As EventArgs) Handles Button21.Click
			Dim x = New ConsoleSample
			x.Show()
		End Sub

		Private Sub Button22_Click(sender As Object, e As EventArgs) Handles Button22.Click
			Dim x = New CustomFoldingSample
			x.Show()
		End Sub
		Friend WithEvents Label23 As System.Windows.Forms.Label
	End Class
End Namespace
