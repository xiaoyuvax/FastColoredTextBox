Imports System.ComponentModel
Imports FastColoredTextBoxNS.Text

Namespace TesterVB
	Public Class TooltipSample
		Inherits Form

		Private components As IContainer = Nothing

		Private label1 As Label

		Private WithEvents Fctb As FastColoredTextBox

		Public Sub New()
			Me.InitializeComponent()
		End Sub


		Protected Overrides Sub Dispose(disposing As Boolean)
			If disposing AndAlso Me.components IsNot Nothing Then
				Me.components.Dispose()
			End If
			MyBase.Dispose(disposing)
		End Sub

		Private Sub InitializeComponent()
			Me.components = New System.ComponentModel.Container()
			Dim resources As New System.ComponentModel.ComponentResourceManager(GetType(TooltipSample))
			Me.label1 = New System.Windows.Forms.Label()
			Me.Fctb = New FastColoredTextBoxNS.FastColoredTextBox()
			Me.SuspendLayout()
			'
			'label1
			'
			Me.label1.Dock = System.Windows.Forms.DockStyle.Top
			Me.label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
			Me.label1.Location = New System.Drawing.Point(0, 0)
			Me.label1.Name = "label1"
			Me.label1.Size = New System.Drawing.Size(355, 30)
			Me.label1.TabIndex = 4
			Me.label1.Text = "This example shows tooltips for words under mouse."
			'
			'fctb
			'
			Me.Fctb.AllowDrop = True
			Me.Fctb.AutoScrollMinSize = New System.Drawing.Size(0, 255)
			Me.Fctb.BackBrush = Nothing
			Me.Fctb.Cursor = System.Windows.Forms.Cursors.IBeam
			Me.Fctb.DisabledColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(180, Byte), Integer), CType(CType(180, Byte), Integer), CType(CType(180, Byte), Integer))
			Me.Fctb.Dock = System.Windows.Forms.DockStyle.Fill
			Me.Fctb.Font = New System.Drawing.Font("Consolas", 9.75!)
			Me.Fctb.IsReplaceMode = False
			Me.Fctb.Language = Language.CSharp
			Me.Fctb.LeftBracket = Global.Microsoft.VisualBasic.ChrW(40)
			Me.Fctb.Location = New System.Drawing.Point(0, 30)
			Me.Fctb.Name = "fctb"
			Me.Fctb.Paddings = New System.Windows.Forms.Padding(0)
			Me.Fctb.ReadOnly = True
			Me.Fctb.RightBracket = Global.Microsoft.VisualBasic.ChrW(41)
			Me.Fctb.SelectionColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(255, Byte), Integer))
			Me.Fctb.Size = New System.Drawing.Size(355, 282)
			Me.Fctb.TabIndex = 5
			Me.Fctb.Text = resources.GetString("fctb.Text")
			Me.Fctb.WordWrap = True
			'
			'TooltipSample
			'
			Me.ClientSize = New System.Drawing.Size(355, 312)
			Me.Controls.Add(Me.Fctb)
			Me.Controls.Add(Me.label1)
			Me.Name = "TooltipSample"
			Me.Text = "TooltipSample"
			Me.ResumeLayout(False)

		End Sub

		Private Sub Fctb_ToolTipNeeded(sender As System.Object, e As FastColoredTextBoxNS.ToolTipNeededEventArgs) Handles Fctb.ToolTipNeeded
			If Not String.IsNullOrEmpty(e.HoveredWord) Then
				e.ToolTipTitle = e.HoveredWord
				e.ToolTipText = "This is tooltip for '" + e.HoveredWord & "'"
			End If

		End Sub
	End Class
End Namespace
