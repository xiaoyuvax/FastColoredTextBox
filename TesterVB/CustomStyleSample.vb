Imports System.ComponentModel
Imports System.Text.RegularExpressions

Namespace TesterVB
	Public Class CustomStyleSample
		Inherits Form

		Private ReadOnly ellipseStyle As New EllipseStyle()

		Private components As IContainer = Nothing

		Private label1 As Label

		Private WithEvents Fctb As FastColoredTextBox

		Public Sub New()
			Me.InitializeComponent()
		End Sub

		Private Sub Fctb_TextChanged(sender As Object, e As TextChangedEventArgs) Handles Fctb.TextChanged
			e.ChangedRange.ClearStyle(New Style() {Me.ellipseStyle})
			e.ChangedRange.SetStyle(Me.ellipseStyle, "\bBabylon\b", RegexOptions.IgnoreCase)
		End Sub

		Protected Overrides Sub Dispose(disposing As Boolean)
			If disposing AndAlso Me.components IsNot Nothing Then
				Me.components.Dispose()
			End If
			MyBase.Dispose(disposing)
		End Sub

		Private Sub InitializeComponent()
			Me.components = New System.ComponentModel.Container()
			Dim resources As New System.ComponentModel.ComponentResourceManager(GetType(CustomStyleSample))
			Me.label1 = New System.Windows.Forms.Label()
			Me.Fctb = New FastColoredTextBoxNS.FastColoredTextBox()
			CType(Me.Fctb, System.ComponentModel.ISupportInitialize).BeginInit()
			Me.SuspendLayout()
			'
			'label1
			'
			Me.label1.Dock = System.Windows.Forms.DockStyle.Top
			Me.label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
			Me.label1.Location = New System.Drawing.Point(0, 0)
			Me.label1.Name = "label1"
			Me.label1.Size = New System.Drawing.Size(600, 39)
			Me.label1.TabIndex = 1
			Me.label1.Text = "This example shows how to create own custom style."
			'
			'fctb
			'
			Me.Fctb.AllowDrop = True
			Me.Fctb.AutoIndent = False
			Me.Fctb.AutoScrollMinSize = New System.Drawing.Size(575, 145)
			Me.Fctb.BackBrush = Nothing
			Me.Fctb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
			Me.Fctb.Cursor = System.Windows.Forms.Cursors.IBeam
			Me.Fctb.DisabledColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(180, Byte), Integer), CType(CType(180, Byte), Integer), CType(CType(180, Byte), Integer))
			Me.Fctb.Dock = System.Windows.Forms.DockStyle.Fill
			Me.Fctb.Font = New System.Drawing.Font("Consolas", 9.75!)
			Me.Fctb.IsReplaceMode = False
			Me.Fctb.LeftBracket = Global.Microsoft.VisualBasic.ChrW(40)
			Me.Fctb.LeftPadding = 3
			Me.Fctb.Location = New System.Drawing.Point(0, 39)
			Me.Fctb.Name = "fctb"
			Me.Fctb.Paddings = New System.Windows.Forms.Padding(5)
			Me.Fctb.PreferredLineWidth = 80
			Me.Fctb.RightBracket = Global.Microsoft.VisualBasic.ChrW(41)
			Me.Fctb.SelectionColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(255, Byte), Integer))
			Me.Fctb.ShowLineNumbers = False
			Me.Fctb.Size = New System.Drawing.Size(600, 266)
			Me.Fctb.TabIndex = 2
			Me.Fctb.Text = resources.GetString("fctb.Text")
			Me.Fctb.WordWrap = True
			Me.Fctb.WordWrapMode = FastColoredTextBoxNS.WordWrapMode.WordWrapPreferredWidth
			'
			'CustomStyleSample
			'
			Me.ClientSize = New System.Drawing.Size(600, 305)
			Me.Controls.Add(Me.Fctb)
			Me.Controls.Add(Me.label1)
			Me.Name = "CustomStyleSample"
			Me.Text = "Custom style sample"
			CType(Me.Fctb, System.ComponentModel.ISupportInitialize).EndInit()
			Me.ResumeLayout(False)

		End Sub
	End Class
End Namespace
