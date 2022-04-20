Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Threading
Imports FarsiLibrary.Win
Imports FastColoredTextBoxNS.Text
Imports FastColoredTextBoxNS.Types

'-----------------------------------------------------------------------------------------------------
' Note: this sample is deprecated. See project Tester for newest code samples.
' For example: Bookmarks now is built-in feature. But this sample implements bookmarks manually.
'-----------------------------------------------------------------------------------------------------

Namespace TesterVB
	Partial Public Class PowerfulCSharpEditor
		Inherits Form

		Private Enum ExplorerItemType
			[Class]
			Method
			[Property]
			[Event]
		End Enum

		Private Class ExplorerItem
			Public type As PowerfulCSharpEditor.ExplorerItemType

			Public title As String

			Public position As Integer
		End Class

		Private Class ExplorerItemComparer
			Implements IComparer(Of PowerfulCSharpEditor.ExplorerItem)

			Public Function Compare(x As PowerfulCSharpEditor.ExplorerItem, y As PowerfulCSharpEditor.ExplorerItem) As Integer Implements IComparer(Of PowerfulCSharpEditor.ExplorerItem).Compare
				Return x.title.CompareTo(y.title)
			End Function
		End Class

		Private Class DeclarationSnippet
			Inherits SnippetAutocompleteItem

			Public Sub New(snippet As String)
				MyBase.New(snippet)
			End Sub

			Public Overrides Function Compare(fragmentText As String) As CompareResult
				Dim pattern As String = Regex.Escape(fragmentText)
				Dim result As CompareResult
				If Regex.IsMatch(Me.Text, "\b" + pattern, RegexOptions.IgnoreCase) Then
					result = CompareResult.Visible
				Else
					result = CompareResult.Hidden
				End If
				Return result
			End Function
		End Class

		Private Class InsertSpaceSnippet
			Inherits AutocompleteItem

			Private ReadOnly pattern As String

			Public Overrides Property ToolTipTitle() As String
				Get
					Return Me.Text
				End Get
				Set(value As String)

				End Set
			End Property

			Public Sub New(pattern As String)
				MyBase.New("")
				Me.pattern = pattern
			End Sub

			Public Sub New()
				Me.New("^(\d+)([a-zA-Z_]+)(\d*)$")
			End Sub

			Public Overrides Function Compare(fragmentText As String) As CompareResult
				Dim result As CompareResult
				If Regex.IsMatch(fragmentText, Me.pattern) Then
					Me.Text = Me.InsertSpaces(fragmentText)
					If Me.Text <> fragmentText Then
						result = CompareResult.Visible
						Return result
					End If
				End If
				result = CompareResult.Hidden
				Return result
			End Function

			Public Function InsertSpaces(fragment As String) As String
				Dim i As Match = Regex.Match(fragment, Me.pattern)
				Dim result As String
				If i Is Nothing Then
					result = fragment
				Else
					If i.Groups(1).Value = "" AndAlso i.Groups(3).Value = "" Then
						result = fragment
					Else
						result = String.Concat(New String() {i.Groups(1).Value, " ", i.Groups(2).Value, " ", i.Groups(3).Value}).Trim()
					End If
				End If
				Return result
			End Function
		End Class

		Private Class InsertEnterSnippet
			Inherits AutocompleteItem

			Private enterPlace As Place = Place.Empty

			Public Overrides Property ToolTipTitle() As String
				Get
					Return "Insert line break after '}'"
				End Get
				Set(value As String)

				End Set
			End Property

			Public Sub New()
				MyBase.New("[Line break]")
			End Sub

			Public Overrides Function Compare(fragmentText As String) As CompareResult
				Dim r As TextSelectionRange = MyBase.Parent.Fragment.Clone()
				Dim result As CompareResult
				While r.Start.iChar > 0
					If r.CharBeforeStart = "}" Then
						Me.enterPlace = r.Start
						result = CompareResult.Visible
						Return result
					End If
					r.GoLeftThroughFolded()
				End While
				result = CompareResult.Hidden
				Return result
			End Function

			Public Overrides Function GetTextForReplace() As String
				Dim r As TextSelectionRange = MyBase.Parent.Fragment
				Dim [end] As Place = r.[End]
				r.Start = Me.enterPlace
				r.[End] = r.[End]
				Return Environment.NewLine + r.Text
			End Function

			Public Overrides Sub OnSelected(popupMenu As AutocompleteMenu, e As SelectedEventArgs)
				MyBase.OnSelected(popupMenu, e)
				If MyBase.Parent.Fragment.tb.AutoIndent Then
					MyBase.Parent.Fragment.tb.DoAutoIndent()
				End If
			End Sub
		End Class

		Public Class BookmarkInfo
			Public iBookmark As Integer
			Public tb As FastColoredTextBox
		End Class

		Private components As IContainer = Nothing

		Private WithEvents MsMain As MenuStrip

		Private WithEvents FileToolStripMenuItem As ToolStripMenuItem

		Private WithEvents OpenToolStripMenuItem As ToolStripMenuItem

		Private WithEvents SaveToolStripMenuItem As ToolStripMenuItem

		Private WithEvents SaveAsToolStripMenuItem As ToolStripMenuItem

		Private WithEvents ToolStripMenuItem1 As ToolStripSeparator

		Private WithEvents QuitToolStripMenuItem As ToolStripMenuItem

		Private WithEvents SsMain As StatusStrip

		Private WithEvents TsMain As ToolStrip

		Private WithEvents TsFiles As FATabStrip

		Private WithEvents NewToolStripMenuItem As ToolStripMenuItem

		Private WithEvents Splitter1 As Splitter

		Private WithEvents SfdMain As SaveFileDialog

		Private WithEvents OfdMain As OpenFileDialog

		Private WithEvents CmMain As ContextMenuStrip

		Private WithEvents CutToolStripMenuItem As ToolStripMenuItem

		Private WithEvents CopyToolStripMenuItem As ToolStripMenuItem

		Private WithEvents PasteToolStripMenuItem As ToolStripMenuItem

		Private WithEvents SelectAllToolStripMenuItem As ToolStripMenuItem

		Private WithEvents ToolStripMenuItem2 As ToolStripSeparator

		Private WithEvents UndoToolStripMenuItem As ToolStripMenuItem

		Private WithEvents RedoToolStripMenuItem As ToolStripMenuItem

		Private WithEvents TmUpdateInterface As System.Windows.Forms.Timer

		Private WithEvents NewToolStripButton As ToolStripButton

		Private WithEvents OpenToolStripButton As ToolStripButton

		Private WithEvents SaveToolStripButton As ToolStripButton

		Private WithEvents PrintToolStripButton As ToolStripButton

		Private WithEvents ToolStripSeparator As ToolStripSeparator

		Private WithEvents CutToolStripButton As ToolStripButton

		Private WithEvents CopyToolStripButton As ToolStripButton

		Private WithEvents PasteToolStripButton As ToolStripButton

		Private WithEvents ToolStripSeparator1 As ToolStripSeparator

		Private WithEvents UndoStripButton As ToolStripButton

		Private WithEvents RedoStripButton As ToolStripButton

		Private WithEvents ToolStripSeparator2 As ToolStripSeparator

		Private WithEvents TbFind As ToolStripTextBox

		Private WithEvents ToolStripLabel1 As ToolStripLabel

		Private WithEvents ToolStripMenuItem3 As ToolStripSeparator

		Private WithEvents FindToolStripMenuItem As ToolStripMenuItem

		Private WithEvents ReplaceToolStripMenuItem As ToolStripMenuItem

		Private WithEvents DgvObjectExplorer As DataGridView

		Private WithEvents BackStripButton As ToolStripButton

		Private WithEvents ForwardStripButton As ToolStripButton

		Private WithEvents ToolStripSeparator3 As ToolStripSeparator

		Private WithEvents ToolStripSeparator4 As ToolStripSeparator

		Private WithEvents ToolStripSeparator5 As ToolStripSeparator

		Private WithEvents ClImage As DataGridViewImageColumn

		Private WithEvents ClName As DataGridViewTextBoxColumn

		Private WithEvents LbWordUnderMouse As ToolStripStatusLabel

		Private WithEvents IlAutocomplete As ImageList

		Private WithEvents ToolStripMenuItem4 As ToolStripSeparator

		Private WithEvents AutoIndentSelectedTextToolStripMenuItem As ToolStripMenuItem

		Private WithEvents BtInvisibleChars As ToolStripButton

		Private WithEvents BtHighlightCurrentLine As ToolStripButton

		Private WithEvents CommentSelectedToolStripMenuItem As ToolStripMenuItem

		Private WithEvents UncommentSelectedToolStripMenuItem As ToolStripMenuItem

		Private WithEvents CloneLinesToolStripMenuItem As ToolStripMenuItem

		Private WithEvents CloneLinesAndCommentToolStripMenuItem As ToolStripMenuItem

		Private WithEvents ToolStripSeparator6 As ToolStripSeparator

		Private WithEvents BookmarkPlusButton As ToolStripButton

		Private WithEvents BookmarkMinusButton As ToolStripButton

		Private WithEvents GotoButton As ToolStripDropDownButton

		Private WithEvents BtShowFoldingLines As ToolStripButton

		Private ReadOnly keywords As String() = New String() {"abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while", "add", "alias", "ascending", "descending", "dynamic", "from", "get", "global", "group", "into", "join", "let", "orderby", "partial", "remove", "select", "set", "value", "var", "where", "yield"}

		Private ReadOnly methods As String() = New String() {"Equals()", "GetHashCode()", "GetType()", "ToString()"}

		Private ReadOnly snippets As String() = New String() {"if(^)" & vbLf & "{" & vbLf & ";" & vbLf & "}", "if(^)" & vbLf & "{" & vbLf & ";" & vbLf & "}" & vbLf & "else" & vbLf & "{" & vbLf & ";" & vbLf & "}", "for(^;;)" & vbLf & "{" & vbLf & ";" & vbLf & "}", "while(^)" & vbLf & "{" & vbLf & ";" & vbLf & "}", "do" & vbLf & "{" & vbLf & "^;" & vbLf & "}while();", "switch(^)" & vbLf & "{" & vbLf & "case : break;" & vbLf & "}"}

		Private ReadOnly declarationSnippets As String() = New String() {"public class ^" & vbLf & "{" & vbLf & "}", "private class ^" & vbLf & "{" & vbLf & "}", "internal class ^" & vbLf & "{" & vbLf & "}", "public struct ^" & vbLf & "{" & vbLf & ";" & vbLf & "}", "private struct ^" & vbLf & "{" & vbLf & ";" & vbLf & "}", "internal struct ^" & vbLf & "{" & vbLf & ";" & vbLf & "}", "public void ^()" & vbLf & "{" & vbLf & ";" & vbLf & "}", "private void ^()" & vbLf & "{" & vbLf & ";" & vbLf & "}", "internal void ^()" & vbLf & "{" & vbLf & ";" & vbLf & "}", "protected void ^()" & vbLf & "{" & vbLf & ";" & vbLf & "}", "public ^{ get; set; }", "private ^{ get; set; }", "internal ^{ get; set; }", "protected ^{ get; set; }"}

		Private ReadOnly invisibleCharsStyle As Style = New InvisibleCharsRenderer(Pens.Gray)

		Private ReadOnly currentLineColor As Color = Color.FromArgb(100, 210, 210, 255)

		Private ReadOnly changedLineColor As Color = Color.FromArgb(255, 230, 230, 255)

		Private explorerList As New List(Of PowerfulCSharpEditor.ExplorerItem)()

		Private tbFindChanged As Boolean = False

		Private lastNavigatedDateTime As DateTime = DateTime.Now

		Private Property CurrentTB() As FastColoredTextBox
			Get
				Dim result As FastColoredTextBox
				If Me.TsFiles.SelectedItem Is Nothing Then
					result = Nothing
				Else
					result = TryCast(Me.TsFiles.SelectedItem.Controls(0), FastColoredTextBox)
				End If
				Return result
			End Get
			Set(value As FastColoredTextBox)
				Me.TsFiles.SelectedItem = TryCast(value.Parent, FATabStripItem)
				value.Focus()
			End Set
		End Property

		Protected Overrides Sub Dispose(disposing As Boolean)
			If disposing AndAlso Me.components IsNot Nothing Then
				Me.components.Dispose()
			End If
			MyBase.Dispose(disposing)
		End Sub

		Private Sub InitializeComponent()
			Me.components = New System.ComponentModel.Container()
			Dim resources As New System.ComponentModel.ComponentResourceManager(GetType(PowerfulCSharpEditor))
			Me.MsMain = New System.Windows.Forms.MenuStrip()
			Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.NewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.OpenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.SaveAsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
			Me.QuitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.SsMain = New System.Windows.Forms.StatusStrip()
			Me.LbWordUnderMouse = New System.Windows.Forms.ToolStripStatusLabel()
			Me.TsMain = New System.Windows.Forms.ToolStrip()
			Me.NewToolStripButton = New System.Windows.Forms.ToolStripButton()
			Me.OpenToolStripButton = New System.Windows.Forms.ToolStripButton()
			Me.SaveToolStripButton = New System.Windows.Forms.ToolStripButton()
			Me.PrintToolStripButton = New System.Windows.Forms.ToolStripButton()
			Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
			Me.CutToolStripButton = New System.Windows.Forms.ToolStripButton()
			Me.CopyToolStripButton = New System.Windows.Forms.ToolStripButton()
			Me.PasteToolStripButton = New System.Windows.Forms.ToolStripButton()
			Me.BtInvisibleChars = New System.Windows.Forms.ToolStripButton()
			Me.BtHighlightCurrentLine = New System.Windows.Forms.ToolStripButton()
			Me.BtShowFoldingLines = New System.Windows.Forms.ToolStripButton()
			Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
			Me.UndoStripButton = New System.Windows.Forms.ToolStripButton()
			Me.RedoStripButton = New System.Windows.Forms.ToolStripButton()
			Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
			Me.BackStripButton = New System.Windows.Forms.ToolStripButton()
			Me.ForwardStripButton = New System.Windows.Forms.ToolStripButton()
			Me.TbFind = New System.Windows.Forms.ToolStripTextBox()
			Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
			Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
			Me.BookmarkPlusButton = New System.Windows.Forms.ToolStripButton()
			Me.BookmarkMinusButton = New System.Windows.Forms.ToolStripButton()
			Me.GotoButton = New System.Windows.Forms.ToolStripDropDownButton()
			Me.ToolStripSeparator = New System.Windows.Forms.ToolStripSeparator()
			Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
			Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
			Me.TsFiles = New FarsiLibrary.Win.FATabStrip()
			Me.Splitter1 = New System.Windows.Forms.Splitter()
			Me.SfdMain = New System.Windows.Forms.SaveFileDialog()
			Me.OfdMain = New System.Windows.Forms.OpenFileDialog()
			Me.CmMain = New System.Windows.Forms.ContextMenuStrip(Me.components)
			Me.CutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.CopyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.PasteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.SelectAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
			Me.UndoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.RedoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
			Me.FindToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.ReplaceToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripSeparator()
			Me.AutoIndentSelectedTextToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.CommentSelectedToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.UncommentSelectedToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.CloneLinesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.CloneLinesAndCommentToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.TmUpdateInterface = New System.Windows.Forms.Timer(Me.components)
			Me.DgvObjectExplorer = New System.Windows.Forms.DataGridView()
			Me.ClImage = New System.Windows.Forms.DataGridViewImageColumn()
			Me.ClName = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.IlAutocomplete = New System.Windows.Forms.ImageList(Me.components)
			Me.MsMain.SuspendLayout()
			Me.SsMain.SuspendLayout()
			Me.TsMain.SuspendLayout()
			CType(Me.TsFiles, System.ComponentModel.ISupportInitialize).BeginInit()
			Me.CmMain.SuspendLayout()
			CType(Me.DgvObjectExplorer, System.ComponentModel.ISupportInitialize).BeginInit()
			Me.SuspendLayout()
			'
			'msMain
			'
			Me.MsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem})
			Me.MsMain.Location = New System.Drawing.Point(0, 0)
			Me.MsMain.Name = "msMain"
			Me.MsMain.Size = New System.Drawing.Size(728, 24)
			Me.MsMain.TabIndex = 0
			Me.MsMain.Text = "menuStrip1"
			'
			'fileToolStripMenuItem
			'
			Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewToolStripMenuItem, Me.OpenToolStripMenuItem, Me.SaveToolStripMenuItem, Me.SaveAsToolStripMenuItem, Me.ToolStripMenuItem1, Me.QuitToolStripMenuItem})
			Me.FileToolStripMenuItem.Name = "fileToolStripMenuItem"
			Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
			Me.FileToolStripMenuItem.Text = "File"
			'
			'newToolStripMenuItem
			'
			Me.NewToolStripMenuItem.Name = "newToolStripMenuItem"
			Me.NewToolStripMenuItem.Size = New System.Drawing.Size(124, 22)
			Me.NewToolStripMenuItem.Text = "New"
			'
			'openToolStripMenuItem
			'
			Me.OpenToolStripMenuItem.Name = "openToolStripMenuItem"
			Me.OpenToolStripMenuItem.Size = New System.Drawing.Size(124, 22)
			Me.OpenToolStripMenuItem.Text = "Open"
			'
			'saveToolStripMenuItem
			'
			Me.SaveToolStripMenuItem.Name = "saveToolStripMenuItem"
			Me.SaveToolStripMenuItem.Size = New System.Drawing.Size(124, 22)
			Me.SaveToolStripMenuItem.Text = "Save"
			'
			'saveAsToolStripMenuItem
			'
			Me.SaveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem"
			Me.SaveAsToolStripMenuItem.Size = New System.Drawing.Size(124, 22)
			Me.SaveAsToolStripMenuItem.Text = "Save as ..."
			'
			'toolStripMenuItem1
			'
			Me.ToolStripMenuItem1.Name = "toolStripMenuItem1"
			Me.ToolStripMenuItem1.Size = New System.Drawing.Size(121, 6)
			'
			'quitToolStripMenuItem
			'
			Me.QuitToolStripMenuItem.Name = "quitToolStripMenuItem"
			Me.QuitToolStripMenuItem.Size = New System.Drawing.Size(124, 22)
			Me.QuitToolStripMenuItem.Text = "Quit"
			'
			'ssMain
			'
			Me.SsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LbWordUnderMouse})
			Me.SsMain.Location = New System.Drawing.Point(0, 374)
			Me.SsMain.Name = "ssMain"
			Me.SsMain.Size = New System.Drawing.Size(728, 22)
			Me.SsMain.TabIndex = 2
			Me.SsMain.Text = "statusStrip1"
			'
			'lbWordUnderMouse
			'
			Me.LbWordUnderMouse.AutoSize = False
			Me.LbWordUnderMouse.ForeColor = System.Drawing.Color.Gray
			Me.LbWordUnderMouse.Name = "lbWordUnderMouse"
			Me.LbWordUnderMouse.Size = New System.Drawing.Size(200, 17)
			Me.LbWordUnderMouse.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
			'
			'tsMain
			'
			Me.TsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewToolStripButton, Me.OpenToolStripButton, Me.SaveToolStripButton, Me.PrintToolStripButton, Me.ToolStripSeparator3, Me.CutToolStripButton, Me.CopyToolStripButton, Me.PasteToolStripButton, Me.BtInvisibleChars, Me.BtHighlightCurrentLine, Me.BtShowFoldingLines, Me.ToolStripSeparator4, Me.UndoStripButton, Me.RedoStripButton, Me.ToolStripSeparator5, Me.BackStripButton, Me.ForwardStripButton, Me.TbFind, Me.ToolStripLabel1, Me.ToolStripSeparator6, Me.BookmarkPlusButton, Me.BookmarkMinusButton, Me.GotoButton})
			Me.TsMain.Location = New System.Drawing.Point(0, 24)
			Me.TsMain.Name = "tsMain"
			Me.TsMain.Size = New System.Drawing.Size(728, 25)
			Me.TsMain.TabIndex = 3
			Me.TsMain.Text = "toolStrip1"
			'
			'newToolStripButton
			'
			Me.NewToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.NewToolStripButton.Image = CType(resources.GetObject("newToolStripButton.Image"), System.Drawing.Image)
			Me.NewToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.NewToolStripButton.Name = "newToolStripButton"
			Me.NewToolStripButton.Size = New System.Drawing.Size(23, 22)
			Me.NewToolStripButton.Text = "&New"
			'
			'openToolStripButton
			'
			Me.OpenToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.OpenToolStripButton.Image = CType(resources.GetObject("openToolStripButton.Image"), System.Drawing.Image)
			Me.OpenToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.OpenToolStripButton.Name = "openToolStripButton"
			Me.OpenToolStripButton.Size = New System.Drawing.Size(23, 22)
			Me.OpenToolStripButton.Text = "&Open"
			'
			'saveToolStripButton
			'
			Me.SaveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.SaveToolStripButton.Image = CType(resources.GetObject("saveToolStripButton.Image"), System.Drawing.Image)
			Me.SaveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.SaveToolStripButton.Name = "saveToolStripButton"
			Me.SaveToolStripButton.Size = New System.Drawing.Size(23, 22)
			Me.SaveToolStripButton.Text = "&Save"
			'
			'printToolStripButton
			'
			Me.PrintToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.PrintToolStripButton.Image = CType(resources.GetObject("printToolStripButton.Image"), System.Drawing.Image)
			Me.PrintToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.PrintToolStripButton.Name = "printToolStripButton"
			Me.PrintToolStripButton.Size = New System.Drawing.Size(23, 22)
			Me.PrintToolStripButton.Text = "&Print"
			'
			'toolStripSeparator3
			'
			Me.ToolStripSeparator3.Name = "toolStripSeparator3"
			Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 25)
			'
			'cutToolStripButton
			'
			Me.CutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.CutToolStripButton.Image = CType(resources.GetObject("cutToolStripButton.Image"), System.Drawing.Image)
			Me.CutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.CutToolStripButton.Name = "cutToolStripButton"
			Me.CutToolStripButton.Size = New System.Drawing.Size(23, 22)
			Me.CutToolStripButton.Text = "C&ut"
			'
			'copyToolStripButton
			'
			Me.CopyToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.CopyToolStripButton.Image = CType(resources.GetObject("copyToolStripButton.Image"), System.Drawing.Image)
			Me.CopyToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.CopyToolStripButton.Name = "copyToolStripButton"
			Me.CopyToolStripButton.Size = New System.Drawing.Size(23, 22)
			Me.CopyToolStripButton.Text = "&Copy"
			'
			'pasteToolStripButton
			'
			Me.PasteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.PasteToolStripButton.Image = CType(resources.GetObject("pasteToolStripButton.Image"), System.Drawing.Image)
			Me.PasteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.PasteToolStripButton.Name = "pasteToolStripButton"
			Me.PasteToolStripButton.Size = New System.Drawing.Size(23, 22)
			Me.PasteToolStripButton.Text = "&Paste"
			'
			'btInvisibleChars
			'
			Me.BtInvisibleChars.CheckOnClick = True
			Me.BtInvisibleChars.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
			Me.BtInvisibleChars.Image = CType(resources.GetObject("btInvisibleChars.Image"), System.Drawing.Image)
			Me.BtInvisibleChars.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.BtInvisibleChars.Name = "btInvisibleChars"
			Me.BtInvisibleChars.Size = New System.Drawing.Size(23, 22)
			Me.BtInvisibleChars.Text = "¶"
			Me.BtInvisibleChars.ToolTipText = "Show invisible chars"
			'
			'btHighlightCurrentLine
			'
			Me.BtHighlightCurrentLine.CheckOnClick = True
			Me.BtHighlightCurrentLine.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.BtHighlightCurrentLine.Image = Global.TesterVB.My.Resources.Resources.edit_padding_top
			Me.BtHighlightCurrentLine.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.BtHighlightCurrentLine.Name = "btHighlightCurrentLine"
			Me.BtHighlightCurrentLine.Size = New System.Drawing.Size(23, 22)
			Me.BtHighlightCurrentLine.Text = "Highlight current line"
			Me.BtHighlightCurrentLine.ToolTipText = "Highlight current line"
			'
			'btShowFoldingLines
			'
			Me.BtShowFoldingLines.Checked = True
			Me.BtShowFoldingLines.CheckOnClick = True
			Me.BtShowFoldingLines.CheckState = System.Windows.Forms.CheckState.Checked
			Me.BtShowFoldingLines.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.BtShowFoldingLines.Image = CType(resources.GetObject("btShowFoldingLines.Image"), System.Drawing.Image)
			Me.BtShowFoldingLines.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.BtShowFoldingLines.Name = "btShowFoldingLines"
			Me.BtShowFoldingLines.Size = New System.Drawing.Size(23, 22)
			Me.BtShowFoldingLines.Text = "Show folding lines"
			'
			'toolStripSeparator4
			'
			Me.ToolStripSeparator4.Name = "toolStripSeparator4"
			Me.ToolStripSeparator4.Size = New System.Drawing.Size(6, 25)
			'
			'undoStripButton
			'
			Me.UndoStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.UndoStripButton.Image = Global.TesterVB.My.Resources.Resources.undo_16x16
			Me.UndoStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.UndoStripButton.Name = "undoStripButton"
			Me.UndoStripButton.Size = New System.Drawing.Size(23, 22)
			Me.UndoStripButton.Text = "Undo (Ctrl+Z)"
			'
			'redoStripButton
			'
			Me.RedoStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.RedoStripButton.Image = Global.TesterVB.My.Resources.Resources.redo_16x16
			Me.RedoStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.RedoStripButton.Name = "redoStripButton"
			Me.RedoStripButton.Size = New System.Drawing.Size(23, 22)
			Me.RedoStripButton.Text = "Redo (Ctrl+R)"
			'
			'toolStripSeparator5
			'
			Me.ToolStripSeparator5.Name = "toolStripSeparator5"
			Me.ToolStripSeparator5.Size = New System.Drawing.Size(6, 25)
			'
			'backStripButton
			'
			Me.BackStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.BackStripButton.Image = Global.TesterVB.My.Resources.Resources.backward0_16x16
			Me.BackStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.BackStripButton.Name = "backStripButton"
			Me.BackStripButton.Size = New System.Drawing.Size(23, 22)
			Me.BackStripButton.Text = "Navigate Backward (Ctrl+ -)"
			'
			'forwardStripButton
			'
			Me.ForwardStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.ForwardStripButton.Image = Global.TesterVB.My.Resources.Resources.forward_16x16
			Me.ForwardStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.ForwardStripButton.Name = "forwardStripButton"
			Me.ForwardStripButton.Size = New System.Drawing.Size(23, 22)
			Me.ForwardStripButton.Text = "Navigate Forward (Ctrl+Shift+ -)"
			'
			'tbFind
			'
			Me.TbFind.AcceptsReturn = True
			Me.TbFind.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
			Me.TbFind.Name = "tbFind"
			Me.TbFind.Size = New System.Drawing.Size(100, 25)
			'
			'toolStripLabel1
			'
			Me.ToolStripLabel1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
			Me.ToolStripLabel1.Name = "toolStripLabel1"
			Me.ToolStripLabel1.Size = New System.Drawing.Size(36, 22)
			Me.ToolStripLabel1.Text = "Find: "
			'
			'toolStripSeparator6
			'
			Me.ToolStripSeparator6.Name = "toolStripSeparator6"
			Me.ToolStripSeparator6.Size = New System.Drawing.Size(6, 25)
			'
			'bookmarkPlusButton
			'
			Me.BookmarkPlusButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.BookmarkPlusButton.Image = Global.TesterVB.My.Resources.Resources.layer__plus
			Me.BookmarkPlusButton.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.BookmarkPlusButton.Name = "bookmarkPlusButton"
			Me.BookmarkPlusButton.Size = New System.Drawing.Size(23, 22)
			Me.BookmarkPlusButton.Text = "Add bookmark"
			'
			'bookmarkMinusButton
			'
			Me.BookmarkMinusButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.BookmarkMinusButton.Image = Global.TesterVB.My.Resources.Resources.layer__minus
			Me.BookmarkMinusButton.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.BookmarkMinusButton.Name = "bookmarkMinusButton"
			Me.BookmarkMinusButton.Size = New System.Drawing.Size(23, 22)
			Me.BookmarkMinusButton.Text = "Remove bookmark"
			'
			'gotoButton
			'
			Me.GotoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
			Me.GotoButton.Image = CType(resources.GetObject("gotoButton.Image"), System.Drawing.Image)
			Me.GotoButton.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.GotoButton.Name = "gotoButton"
			Me.GotoButton.Size = New System.Drawing.Size(55, 22)
			Me.GotoButton.Text = "Goto..."
			'
			'toolStripSeparator
			'
			Me.ToolStripSeparator.Name = "toolStripSeparator"
			Me.ToolStripSeparator.Size = New System.Drawing.Size(6, 25)
			'
			'toolStripSeparator1
			'
			Me.ToolStripSeparator1.Name = "toolStripSeparator1"
			Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
			'
			'toolStripSeparator2
			'
			Me.ToolStripSeparator2.Name = "toolStripSeparator2"
			Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
			'
			'tsFiles
			'
			Me.TsFiles.Dock = System.Windows.Forms.DockStyle.Fill
			Me.TsFiles.Font = New System.Drawing.Font("Tahoma", 8.25!)
			Me.TsFiles.Location = New System.Drawing.Point(172, 49)
			Me.TsFiles.Name = "tsFiles"
			Me.TsFiles.Size = New System.Drawing.Size(556, 325)
			Me.TsFiles.TabIndex = 0
			Me.TsFiles.Text = "faTabStrip1"
			'
			'splitter1
			'
			Me.Splitter1.Location = New System.Drawing.Point(172, 49)
			Me.Splitter1.Name = "splitter1"
			Me.Splitter1.Size = New System.Drawing.Size(3, 325)
			Me.Splitter1.TabIndex = 5
			Me.Splitter1.TabStop = False
			'
			'sfdMain
			'
			Me.SfdMain.DefaultExt = "cs"
			Me.SfdMain.Filter = "C# file(*.cs)|*.cs"
			'
			'ofdMain
			'
			Me.OfdMain.DefaultExt = "cs"
			Me.OfdMain.Filter = "C# file(*.cs)|*.cs"
			'
			'cmMain
			'
			Me.CmMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CutToolStripMenuItem, Me.CopyToolStripMenuItem, Me.PasteToolStripMenuItem, Me.SelectAllToolStripMenuItem, Me.ToolStripMenuItem2, Me.UndoToolStripMenuItem, Me.RedoToolStripMenuItem, Me.ToolStripMenuItem3, Me.FindToolStripMenuItem, Me.ReplaceToolStripMenuItem, Me.ToolStripMenuItem4, Me.AutoIndentSelectedTextToolStripMenuItem, Me.CommentSelectedToolStripMenuItem, Me.UncommentSelectedToolStripMenuItem, Me.CloneLinesToolStripMenuItem, Me.CloneLinesAndCommentToolStripMenuItem})
			Me.CmMain.Name = "cmMain"
			Me.CmMain.Size = New System.Drawing.Size(219, 308)
			'
			'cutToolStripMenuItem
			'
			Me.CutToolStripMenuItem.Name = "cutToolStripMenuItem"
			Me.CutToolStripMenuItem.Size = New System.Drawing.Size(218, 22)
			Me.CutToolStripMenuItem.Text = "Cut"
			'
			'copyToolStripMenuItem
			'
			Me.CopyToolStripMenuItem.Name = "copyToolStripMenuItem"
			Me.CopyToolStripMenuItem.Size = New System.Drawing.Size(218, 22)
			Me.CopyToolStripMenuItem.Text = "Copy"
			'
			'pasteToolStripMenuItem
			'
			Me.PasteToolStripMenuItem.Name = "pasteToolStripMenuItem"
			Me.PasteToolStripMenuItem.Size = New System.Drawing.Size(218, 22)
			Me.PasteToolStripMenuItem.Text = "Paste"
			'
			'selectAllToolStripMenuItem
			'
			Me.SelectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem"
			Me.SelectAllToolStripMenuItem.Size = New System.Drawing.Size(218, 22)
			Me.SelectAllToolStripMenuItem.Text = "Select all"
			'
			'toolStripMenuItem2
			'
			Me.ToolStripMenuItem2.Name = "toolStripMenuItem2"
			Me.ToolStripMenuItem2.Size = New System.Drawing.Size(215, 6)
			'
			'undoToolStripMenuItem
			'
			Me.UndoToolStripMenuItem.Image = Global.TesterVB.My.Resources.Resources.undo_16x16
			Me.UndoToolStripMenuItem.Name = "undoToolStripMenuItem"
			Me.UndoToolStripMenuItem.Size = New System.Drawing.Size(218, 22)
			Me.UndoToolStripMenuItem.Text = "Undo"
			'
			'redoToolStripMenuItem
			'
			Me.RedoToolStripMenuItem.Image = Global.TesterVB.My.Resources.Resources.redo_16x16
			Me.RedoToolStripMenuItem.Name = "redoToolStripMenuItem"
			Me.RedoToolStripMenuItem.Size = New System.Drawing.Size(218, 22)
			Me.RedoToolStripMenuItem.Text = "Redo"
			'
			'toolStripMenuItem3
			'
			Me.ToolStripMenuItem3.Name = "toolStripMenuItem3"
			Me.ToolStripMenuItem3.Size = New System.Drawing.Size(215, 6)
			'
			'findToolStripMenuItem
			'
			Me.FindToolStripMenuItem.Name = "findToolStripMenuItem"
			Me.FindToolStripMenuItem.Size = New System.Drawing.Size(218, 22)
			Me.FindToolStripMenuItem.Text = "Find"
			'
			'replaceToolStripMenuItem
			'
			Me.ReplaceToolStripMenuItem.Name = "replaceToolStripMenuItem"
			Me.ReplaceToolStripMenuItem.Size = New System.Drawing.Size(218, 22)
			Me.ReplaceToolStripMenuItem.Text = "Replace"
			'
			'toolStripMenuItem4
			'
			Me.ToolStripMenuItem4.Name = "toolStripMenuItem4"
			Me.ToolStripMenuItem4.Size = New System.Drawing.Size(215, 6)
			'
			'autoIndentSelectedTextToolStripMenuItem
			'
			Me.AutoIndentSelectedTextToolStripMenuItem.Name = "autoIndentSelectedTextToolStripMenuItem"
			Me.AutoIndentSelectedTextToolStripMenuItem.Size = New System.Drawing.Size(218, 22)
			Me.AutoIndentSelectedTextToolStripMenuItem.Text = "AutoIndent selected text"
			'
			'commentSelectedToolStripMenuItem
			'
			Me.CommentSelectedToolStripMenuItem.Name = "commentSelectedToolStripMenuItem"
			Me.CommentSelectedToolStripMenuItem.Size = New System.Drawing.Size(218, 22)
			Me.CommentSelectedToolStripMenuItem.Text = "Comment selected"
			'
			'uncommentSelectedToolStripMenuItem
			'
			Me.UncommentSelectedToolStripMenuItem.Name = "uncommentSelectedToolStripMenuItem"
			Me.UncommentSelectedToolStripMenuItem.Size = New System.Drawing.Size(218, 22)
			Me.UncommentSelectedToolStripMenuItem.Text = "Uncomment selected"
			'
			'cloneLinesToolStripMenuItem
			'
			Me.CloneLinesToolStripMenuItem.Name = "cloneLinesToolStripMenuItem"
			Me.CloneLinesToolStripMenuItem.Size = New System.Drawing.Size(218, 22)
			Me.CloneLinesToolStripMenuItem.Text = "Clone line(s)"
			'
			'cloneLinesAndCommentToolStripMenuItem
			'
			Me.CloneLinesAndCommentToolStripMenuItem.Name = "cloneLinesAndCommentToolStripMenuItem"
			Me.CloneLinesAndCommentToolStripMenuItem.Size = New System.Drawing.Size(218, 22)
			Me.CloneLinesAndCommentToolStripMenuItem.Text = "Clone line(s) and comment"
			'
			'tmUpdateInterface
			'
			Me.TmUpdateInterface.Enabled = True
			Me.TmUpdateInterface.Interval = 400
			'
			'dgvObjectExplorer
			'
			Me.DgvObjectExplorer.AllowUserToAddRows = False
			Me.DgvObjectExplorer.AllowUserToDeleteRows = False
			Me.DgvObjectExplorer.AllowUserToResizeColumns = False
			Me.DgvObjectExplorer.AllowUserToResizeRows = False
			Me.DgvObjectExplorer.BackgroundColor = System.Drawing.Color.White
			Me.DgvObjectExplorer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
			Me.DgvObjectExplorer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
			Me.DgvObjectExplorer.ColumnHeadersVisible = False
			Me.DgvObjectExplorer.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ClImage, Me.ClName})
			Me.DgvObjectExplorer.Cursor = System.Windows.Forms.Cursors.Hand
			Me.DgvObjectExplorer.Dock = System.Windows.Forms.DockStyle.Left
			Me.DgvObjectExplorer.GridColor = System.Drawing.Color.White
			Me.DgvObjectExplorer.Location = New System.Drawing.Point(0, 49)
			Me.DgvObjectExplorer.MultiSelect = False
			Me.DgvObjectExplorer.Name = "dgvObjectExplorer"
			Me.DgvObjectExplorer.ReadOnly = True
			Me.DgvObjectExplorer.RowHeadersVisible = False
			Me.DgvObjectExplorer.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Black
			Me.DgvObjectExplorer.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.White
			Me.DgvObjectExplorer.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Green
			Me.DgvObjectExplorer.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
			Me.DgvObjectExplorer.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
			Me.DgvObjectExplorer.Size = New System.Drawing.Size(172, 325)
			Me.DgvObjectExplorer.TabIndex = 6
			Me.DgvObjectExplorer.VirtualMode = True
			'
			'clImage
			'
			Me.ClImage.HeaderText = "Column2"
			Me.ClImage.MinimumWidth = 32
			Me.ClImage.Name = "clImage"
			Me.ClImage.ReadOnly = True
			Me.ClImage.Width = 32
			'
			'clName
			'
			Me.ClName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
			Me.ClName.HeaderText = "Column1"
			Me.ClName.Name = "clName"
			Me.ClName.ReadOnly = True
			'
			'ilAutocomplete
			'
			Me.IlAutocomplete.ImageStream = CType(resources.GetObject("ilAutocomplete.ImageStream"), System.Windows.Forms.ImageListStreamer)
			Me.IlAutocomplete.TransparentColor = System.Drawing.Color.Transparent
			Me.IlAutocomplete.Images.SetKeyName(0, "script_16x16.png")
			Me.IlAutocomplete.Images.SetKeyName(1, "app_16x16.png")
			Me.IlAutocomplete.Images.SetKeyName(2, "1302166543_virtualbox.png")
			'
			'PowerfulCSharpEditor
			'
			Me.ClientSize = New System.Drawing.Size(728, 396)
			Me.Controls.Add(Me.Splitter1)
			Me.Controls.Add(Me.TsFiles)
			Me.Controls.Add(Me.DgvObjectExplorer)
			Me.Controls.Add(Me.TsMain)
			Me.Controls.Add(Me.MsMain)
			Me.Controls.Add(Me.SsMain)
			Me.MainMenuStrip = Me.MsMain
			Me.Name = "PowerfulCSharpEditor"
			Me.Text = "PowerfulCSharpEditor"
			Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
			Me.MsMain.ResumeLayout(False)
			Me.MsMain.PerformLayout()
			Me.SsMain.ResumeLayout(False)
			Me.SsMain.PerformLayout()
			Me.TsMain.ResumeLayout(False)
			Me.TsMain.PerformLayout()
			CType(Me.TsFiles, System.ComponentModel.ISupportInitialize).EndInit()
			Me.CmMain.ResumeLayout(False)
			CType(Me.DgvObjectExplorer, System.ComponentModel.ISupportInitialize).EndInit()
			Me.ResumeLayout(False)
			Me.PerformLayout()

		End Sub

		Public Sub New()
			Me.InitializeComponent()
			Dim resources As New ComponentResourceManager(GetType(PowerfulCSharpEditor))
			Me.CopyToolStripMenuItem.Image = CType(resources.GetObject("copyToolStripButton.Image"), Image)
			Me.CutToolStripMenuItem.Image = CType(resources.GetObject("cutToolStripButton.Image"), Image)
			Me.PasteToolStripMenuItem.Image = CType(resources.GetObject("pasteToolStripButton.Image"), Image)
		End Sub

		Private Sub NewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewToolStripButton.Click
			Me.CreateTab(Nothing)
		End Sub

		Private Sub CreateTab(fileName As String)
			Try
				Dim tb As New FastColoredTextBox With {
					.Font = New Font("Consolas", 9.75F),
					.ContextMenuStrip = Me.CmMain,
					.Dock = DockStyle.Fill,
					.BorderStyle = BorderStyle.Fixed3D,
					.LeftPadding = 17,
					.Language = Language.CSharp
				}
				tb.AddStyle(New MarkerStyle(New SolidBrush(Color.FromArgb(50, Color.Gray))))
				Dim tab As New FATabStripItem(If(fileName IsNot Nothing, Path.GetFileName(fileName), "[new]"), tb) With {
					.Tag = fileName
				}
				If fileName <> Nothing Then
					tb.Text = File.ReadAllText(fileName)
				End If
				tb.ClearUndo()
				tb.Tag = New TbInfo()
				tb.IsChanged = False
				Me.TsFiles.AddTab(tab)
				Me.TsFiles.SelectedItem = tab
				tb.Focus()
				tb.DelayedTextChangedInterval = 1000
				tb.DelayedEventsInterval = 1000
				AddHandler tb.TextChangedDelayed, New EventHandler(Of TextChangedEventArgs)(AddressOf Me.Tb_TextChangedDelayed)
				AddHandler tb.SelectionChangedDelayed, New EventHandler(AddressOf Me.Tb_SelectionChangedDelayed)
				AddHandler tb.KeyDown, New KeyEventHandler(AddressOf Me.Tb_KeyDown)
				AddHandler tb.MouseMove, New MouseEventHandler(AddressOf Me.Tb_MouseMove)
				AddHandler tb.LineRemoved, New EventHandler(Of LineRemovedEventArgs)(AddressOf Me.Tb_LineRemoved)
				AddHandler tb.PaintLine, New EventHandler(Of PaintLineEventArgs)(AddressOf Me.Tb_PaintLine)
				tb.ChangedLineColor = Me.changedLineColor
				If Me.BtHighlightCurrentLine.Checked Then
					tb.CurrentLineColor = Me.currentLineColor
				End If
				tb.ShowFoldingLines = Me.BtShowFoldingLines.Checked
				tb.HighlightingRangeType = HighlightingRangeType.VisibleRange
				Dim popupMenu As New AutocompleteMenu(tb)
				popupMenu.Items.ImageList = Me.IlAutocomplete
				AddHandler popupMenu.Opening, New EventHandler(Of CancelEventArgs)(AddressOf Me.PopupMenu_Opening)
				Me.BuildAutocompleteMenu(popupMenu)
				TryCast(tb.Tag, TbInfo).popupMenu = popupMenu
			Catch ex As Exception
				If MessageBox.Show(ex.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Hand) = DialogResult.Retry Then
					Me.CreateTab(fileName)
				End If
			End Try
		End Sub

		Private Sub PopupMenu_Opening(sender As Object, e As CancelEventArgs)
			Dim iGreenStyle As Integer = Me.CurrentTB.GetStyleIndex(Me.CurrentTB.SyntaxHighlighter.GreenStyle)
			If iGreenStyle >= 0 AndAlso Me.CurrentTB.Selection.Start.iChar > 0 Then
				Dim c As StyledChar = Me.CurrentTB(Me.CurrentTB.Selection.Start.iLine)(Me.CurrentTB.Selection.Start.iChar - 1)
				Dim greenStyleIndex As StyleIndex = TextSelectionRange.ToStyleIndex(iGreenStyle)
				If CUShort(c.style And greenStyleIndex) <> 0 Then
					e.Cancel = True
				End If
			End If
		End Sub

		Private Sub Tb_PaintLine(sender As Object, e As PaintLineEventArgs)
			Dim info As TbInfo = TryCast(TryCast(sender, FastColoredTextBox).Tag, TbInfo)
			If info.bookmarksLineId.Contains(TryCast(sender, FastColoredTextBox)(e.LineIndex).UniqueId) Then
				e.Graphics.FillEllipse(New LinearGradientBrush(New Rectangle(0, e.LineRect.Top, 15, 15), Color.White, Color.PowderBlue, 45.0F), 0, e.LineRect.Top, 15, 15)
				e.Graphics.DrawEllipse(Pens.PowderBlue, 0, e.LineRect.Top, 15, 15)
			End If
		End Sub

		Private Sub Tb_LineRemoved(sender As Object, e As LineRemovedEventArgs)
			Dim info As TbInfo = TryCast(TryCast(sender, FastColoredTextBox).Tag, TbInfo)
			For Each id As Integer In e.RemovedLineUniqueIds
				If info.bookmarksLineId.Contains(id) Then
					info.bookmarksLineId.Remove(id)
					info.bookmarks.Remove(id)
				End If
			Next
		End Sub

		Private Sub BuildAutocompleteMenu(popupMenu As AutocompleteMenu)
			Dim items As New List(Of AutocompleteItem)()
			Dim array As String() = Me.snippets
			For i As Integer = 0 To array.Length - 1
				Dim item As String = array(i)
				items.Add(New SnippetAutocompleteItem(item) With {.ImageIndex = 1})
			Next
			array = Me.declarationSnippets
			For i As Integer = 0 To array.Length - 1
				Dim item As String = array(i)
				items.Add(New PowerfulCSharpEditor.DeclarationSnippet(item) With {.ImageIndex = 0})
			Next
			array = Me.methods
			For i As Integer = 0 To array.Length - 1
				Dim item As String = array(i)
				items.Add(New MethodAutocompleteItem(item) With {.ImageIndex = 2})
			Next
			array = Me.keywords
			For i As Integer = 0 To array.Length - 1
				Dim item As String = array(i)
				items.Add(New AutocompleteItem(item))
			Next
			items.Add(New PowerfulCSharpEditor.InsertSpaceSnippet())
			items.Add(New PowerfulCSharpEditor.InsertSpaceSnippet("^(\w+)([=<>!:]+)(\w+)$"))
			items.Add(New PowerfulCSharpEditor.InsertEnterSnippet())
			popupMenu.Items.SetAutocompleteItems(items)
			popupMenu.SearchPattern = "[\w\.:=!<>]"
		End Sub

		Private Sub Tb_MouseMove(sender As Object, e As MouseEventArgs)
			Dim tb As FastColoredTextBox = TryCast(sender, FastColoredTextBox)
			Dim place As Place = tb.PointToPlace(e.Location)
			Dim r As New TextSelectionRange(tb, place, place)
			Dim text As String = r.GetFragment("[a-zA-Z]").Text
			Me.LbWordUnderMouse.Text = text
		End Sub

		Private Sub Tb_KeyDown(sender As Object, e As KeyEventArgs)
			If e.Modifiers = Keys.Control AndAlso e.KeyCode = Keys.OemMinus Then
				Me.NavigateBackward()
				e.Handled = True
			End If
			If e.Modifiers = Keys.Shift Or Keys.Control AndAlso e.KeyCode = Keys.OemMinus Then
				Me.NavigateForward()
				e.Handled = True
			End If
			If e.KeyData = CType(131147, Keys) Then
				TryCast(Me.CurrentTB.Tag, TbInfo).popupMenu.Show(True)
				e.Handled = True
			End If
		End Sub

		Private Sub Tb_SelectionChangedDelayed(sender As Object, e As EventArgs)
			Dim tb As FastColoredTextBox = TryCast(sender, FastColoredTextBox)
			If tb.Selection.IsEmpty AndAlso tb.Selection.Start.iLine < tb.LinesCount Then
				If Me.lastNavigatedDateTime <> tb(tb.Selection.Start.iLine).LastVisit Then
					tb(tb.Selection.Start.iLine).LastVisit = DateTime.Now
					Me.lastNavigatedDateTime = tb(tb.Selection.Start.iLine).LastVisit
				End If
			End If
			tb.VisibleRange.ClearStyle(New Style() {tb.Styles(0)})
			If tb.Selection.IsEmpty Then
				Dim fragment As TextSelectionRange = tb.Selection.GetFragment("\w")
				Dim text As String = fragment.Text
				If text.Length <> 0 Then
					Dim ranges As TextSelectionRange() = tb.VisibleRange.GetRanges("\b" + text + "\b").ToArray()
					If ranges.Length > 1 Then
						Dim array As TextSelectionRange() = ranges
						For i As Integer = 0 To array.Length - 1
							Dim r As TextSelectionRange = array(i)
							r.SetStyle(tb.Styles(0))
						Next
					End If
				End If
			End If
		End Sub

		Private Sub Tb_TextChangedDelayed(sender As Object, e As TextChangedEventArgs)
			Dim tb As FastColoredTextBox = TryCast(sender, FastColoredTextBox)
			Dim text As String = TryCast(sender, FastColoredTextBox).Text
			ThreadPool.QueueUserWorkItem(Sub(o As Object)
											 Me.ReBuildObjectExplorer(text)
										 End Sub)
			Me.HighlightInvisibleChars(e.ChangedRange)
		End Sub

		Private Sub HighlightInvisibleChars(range As TextSelectionRange)
			range.ClearStyle(New Style() {Me.invisibleCharsStyle})
			If Me.BtInvisibleChars.Checked Then
				range.SetStyle(Me.invisibleCharsStyle, ".$|.\r\n|\s")
			End If
		End Sub

		Private Sub ReBuildObjectExplorer(text As String)
			Try
				Dim list As New List(Of PowerfulCSharpEditor.ExplorerItem)()
				Dim lastClassIndex As Integer = -1
				Dim regex As New Regex("^(?<range>[\w\s]+\b(class|struct|enum|interface)\s+[\w<>,\s]+)|^\s*(public|private|internal|protected)[^\n]+(\n?\s*{|;)?", RegexOptions.Multiline)
				For Each r As Match In regex.Matches(text)
					Try
						Dim s As String = r.Value
						Dim i As Integer = s.IndexOfAny(New Char() {"=", "{", ";"})
						If i >= 0 Then
							s = s.Substring(0, i)
						End If
						s = s.Trim()
						Dim item As New PowerfulCSharpEditor.ExplorerItem() With {.title = s, .position = r.Index}
						If Regex.IsMatch(item.title, "\b(class|struct|enum|interface)\b") Then
							item.title = item.title.Substring(item.title.LastIndexOf(" ")).Trim()
							item.type = PowerfulCSharpEditor.ExplorerItemType.[Class]
							list.Sort(lastClassIndex + 1, list.Count - (lastClassIndex + 1), New PowerfulCSharpEditor.ExplorerItemComparer())
							lastClassIndex = list.Count
						Else
							If item.title.Contains(" event ") Then
								Dim ii As Integer = item.title.LastIndexOf(" ")
								item.title = item.title.Substring(ii).Trim()
								item.type = PowerfulCSharpEditor.ExplorerItemType.[Event]
							Else
								If item.title.Contains("("c) Then
									Dim parts As String() = item.title.Split(New Char() {"("})
									item.title = parts(0).Substring(parts(0).LastIndexOf(" ")).Trim() + "(" + parts(1)
									item.type = PowerfulCSharpEditor.ExplorerItemType.Method
								Else
									If item.title.EndsWith("]") Then
										Dim parts As String() = item.title.Split(New Char() {"["})
										If parts.Length < 2 Then
											Continue For
										End If
										item.title = parts(0).Substring(parts(0).LastIndexOf(" ")).Trim() + "[" + parts(1)
										item.type = PowerfulCSharpEditor.ExplorerItemType.Method
									Else
										Dim ii As Integer = item.title.LastIndexOf(" ")
										item.title = item.title.Substring(ii).Trim()
										item.type = PowerfulCSharpEditor.ExplorerItemType.[Property]
									End If
								End If
							End If
						End If
						list.Add(item)
					Catch ex_2BF As Exception
						Console.WriteLine(ex_2BF)
					End Try
				Next
				list.Sort(lastClassIndex + 1, list.Count - (lastClassIndex + 1), New PowerfulCSharpEditor.ExplorerItemComparer())
				MyBase.BeginInvoke(Sub()
									   Me.explorerList = list
									   Me.DgvObjectExplorer.RowCount = Me.explorerList.Count
									   Me.DgvObjectExplorer.Invalidate()
								   End Sub)
			Catch ex_332 As Exception
				Console.WriteLine(ex_332)
			End Try
		End Sub

		Private Sub TsFiles_TabStripItemClosing(e As TabStripItemClosingEventArgs) Handles TsFiles.TabStripItemClosing
			If TryCast(e.Item.Controls(0), FastColoredTextBox).IsChanged Then
				Dim dialogResult As DialogResult = MessageBox.Show("Do you want save " + e.Item.Title + " ?", "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Asterisk)
				If dialogResult <> DialogResult.Cancel Then
					If dialogResult = DialogResult.Yes Then
						If Not Me.Save(e.Item) Then
							e.Cancel = True
						End If
					End If
				Else
					e.Cancel = True
				End If
			End If
		End Sub

		Private Function Save(tab As FATabStripItem) As Boolean
			Dim tb As FastColoredTextBox = TryCast(tab.Controls(0), FastColoredTextBox)
			Dim result As Boolean
			If tab.Tag Is Nothing Then
				If Me.SfdMain.ShowDialog() <> DialogResult.OK Then
					result = False
					Return result
				End If
				tab.Title = Path.GetFileName(Me.SfdMain.FileName)
				tab.Tag = Me.SfdMain.FileName
			End If
			Try
				File.WriteAllText(TryCast(tab.Tag, String), tb.Text)
				tb.IsChanged = False
			Catch ex As Exception
				If MessageBox.Show(ex.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Hand) = DialogResult.Retry Then
					result = Me.Save(tab)
					Return result
				End If
				result = False
				Return result
			End Try
			tb.Invalidate()
			result = True
			Return result
		End Function

		Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripButton.Click
			If Me.TsFiles.SelectedItem IsNot Nothing Then
				Me.Save(Me.TsFiles.SelectedItem)
			End If
		End Sub

		Private Sub SaveAsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveAsToolStripMenuItem.Click
			If Me.TsFiles.SelectedItem IsNot Nothing Then
				Dim oldFile As String = TryCast(Me.TsFiles.SelectedItem.Tag, String)
				Me.TsFiles.SelectedItem.Tag = Nothing
				If Not Me.Save(Me.TsFiles.SelectedItem) AndAlso oldFile <> Nothing Then
					Me.TsFiles.SelectedItem.Tag = oldFile
					Me.TsFiles.SelectedItem.Title = Path.GetFileName(oldFile)
				End If
			End If
		End Sub

		Private Sub QuitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles QuitToolStripMenuItem.Click
			MyBase.Close()
		End Sub

		Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripButton.Click
			If Me.OfdMain.ShowDialog() = DialogResult.OK Then
				Me.CreateTab(Me.OfdMain.FileName)
			End If
		End Sub

		Private Sub CutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CutToolStripButton.Click
			Me.CurrentTB.Cut()
		End Sub

		Private Sub CopyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyToolStripButton.Click
			Me.CurrentTB.Copy()
		End Sub

		Private Sub PasteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PasteToolStripButton.Click
			Me.CurrentTB.Paste()
		End Sub

		Private Sub SelectAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectAllToolStripMenuItem.Click
			Me.CurrentTB.Selection.SelectAll()
		End Sub

		Private Sub UndoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UndoStripButton.Click
			If Me.CurrentTB.UndoEnabled Then
				Me.CurrentTB.Undo()
			End If
		End Sub

		Private Sub RedoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RedoStripButton.Click
			If Me.CurrentTB.RedoEnabled Then
				Me.CurrentTB.Redo()
			End If
		End Sub

		Private Sub TmUpdateInterface_Tick(sender As Object, e As EventArgs) Handles TmUpdateInterface.Tick
			Try
				If Me.CurrentTB IsNot Nothing AndAlso Me.TsFiles.Items.Count > 0 Then
					Dim tb As FastColoredTextBox = Me.CurrentTB
					Me.UndoStripButton.Enabled = Me.UndoToolStripMenuItem.Enabled = tb.UndoEnabled
					Me.RedoStripButton.Enabled = Me.RedoToolStripMenuItem.Enabled = tb.RedoEnabled
					Me.SaveToolStripButton.Enabled = Me.SaveToolStripMenuItem.Enabled = tb.IsChanged
					Me.SaveAsToolStripMenuItem.Enabled = True
					Me.PasteToolStripButton.Enabled = Me.PasteToolStripMenuItem.Enabled = True
					Me.CutToolStripButton.Enabled = Me.CutToolStripMenuItem.Enabled = Me.CopyToolStripButton.Enabled = Me.CopyToolStripMenuItem.Enabled = Not tb.Selection.IsEmpty
					Me.PrintToolStripButton.Enabled = True
				Else
					Me.SaveToolStripButton.Enabled = Me.SaveToolStripMenuItem.Enabled = False
					Me.SaveAsToolStripMenuItem.Enabled = False
					Me.CutToolStripButton.Enabled = Me.CutToolStripMenuItem.Enabled = Me.CopyToolStripButton.Enabled = Me.CopyToolStripMenuItem.Enabled = False
					Me.PasteToolStripButton.Enabled = Me.PasteToolStripMenuItem.Enabled = False
					Me.PrintToolStripButton.Enabled = False
					Me.UndoStripButton.Enabled = Me.UndoToolStripMenuItem.Enabled = False
					Me.RedoStripButton.Enabled = Me.RedoToolStripMenuItem.Enabled = False
					Me.DgvObjectExplorer.RowCount = 0
				End If
			Catch ex As Exception
				Console.WriteLine(ex.Message)
			End Try
		End Sub

		Private Sub PrintToolStripButton_Click(sender As Object, e As EventArgs) Handles PrintToolStripButton.Click
			If Me.CurrentTB IsNot Nothing Then
				Dim settings As New PrintDialogSettings With {
					.Title = Me.TsFiles.SelectedItem.Title,
					.Header = "&b&w&b",
					.Footer = "&b&p"
				}
				Me.CurrentTB.Print(settings)
			End If
		End Sub

		Private Sub TbFind_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TbFind.KeyPress
			If e.KeyChar = vbCr AndAlso Me.CurrentTB IsNot Nothing Then
				Dim r As TextSelectionRange = If(Me.tbFindChanged, Me.CurrentTB.Range.Clone(), Me.CurrentTB.Selection.Clone())
				Me.tbFindChanged = False
				r.[End] = New Place(Me.CurrentTB(Me.CurrentTB.LinesCount - 1).Count, Me.CurrentTB.LinesCount - 1)
				Dim pattern As String = Regex.Escape(Me.TbFind.Text)
				Using enumerator As IEnumerator(Of TextSelectionRange) = r.GetRanges(pattern).GetEnumerator()
					If enumerator.MoveNext() Then
						Dim found As TextSelectionRange = enumerator.Current
						found.Inverse()
						Me.CurrentTB.Selection = found
						Me.CurrentTB.DoSelectionVisible()
						Return
					End If
				End Using
				MessageBox.Show("Not found.")
			Else
				Me.tbFindChanged = True
			End If
		End Sub

		Private Sub FindToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FindToolStripMenuItem.Click
			Me.CurrentTB.ShowFindDialog()
		End Sub

		Private Sub ReplaceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReplaceToolStripMenuItem.Click
			Me.CurrentTB.ShowReplaceDialog()
		End Sub

		Private Sub PowerfulCSharpEditor_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
			Dim list As New List(Of FATabStripItem)()
			For Each tab As FATabStripItem In Me.TsFiles.Items
				list.Add(tab)
			Next
			For Each tab As FATabStripItem In list
				Dim args As New TabStripItemClosingEventArgs(tab)
				Me.TsFiles_TabStripItemClosing(args)
				If args.Cancel Then
					e.Cancel = True
					Exit For
				End If
				Me.TsFiles.RemoveTab(tab)
			Next
		End Sub

		Private Sub DgvObjectExplorer_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles DgvObjectExplorer.CellMouseClick
			If Me.CurrentTB IsNot Nothing Then
				Dim item As PowerfulCSharpEditor.ExplorerItem = Me.explorerList(e.RowIndex)
				Me.CurrentTB.GoEnd()
				Me.CurrentTB.SelectionStart = item.position
				Me.CurrentTB.DoSelectionVisible()
				Me.CurrentTB.Focus()
			End If
		End Sub

		Private Sub DgvObjectExplorer_CellValueNeeded(sender As Object, e As DataGridViewCellValueEventArgs) Handles DgvObjectExplorer.CellValueNeeded
			Try
				Dim item As PowerfulCSharpEditor.ExplorerItem = Me.explorerList(e.RowIndex)
				If e.ColumnIndex = 1 Then
					e.Value = item.title
				Else
					Select Case item.type
						Case PowerfulCSharpEditor.ExplorerItemType.[Class]
							e.Value = My.Resources.class_libraries
						Case PowerfulCSharpEditor.ExplorerItemType.Method
							e.Value = My.Resources.box
						Case PowerfulCSharpEditor.ExplorerItemType.[Property]
							e.Value = My.Resources._property
						Case PowerfulCSharpEditor.ExplorerItemType.[Event]
							e.Value = My.Resources.lightning
					End Select
				End If
			Catch ex_8D As Exception
			End Try
		End Sub

		Private Sub TsFiles_TabStripItemSelectionChanged(e As TabStripItemChangedEventArgs) Handles TsFiles.TabStripItemSelectionChanged
			If Me.CurrentTB IsNot Nothing Then
				Me.CurrentTB.Focus()
				Dim text As String = Me.CurrentTB.Text
				ThreadPool.QueueUserWorkItem(Sub(o As Object)
												 Me.ReBuildObjectExplorer(text)
											 End Sub)
			End If
		End Sub

		Private Sub BackStripButton_Click(sender As Object, e As EventArgs) Handles BackStripButton.Click
			Me.NavigateBackward()
		End Sub

		Private Sub ForwardStripButton_Click(sender As Object, e As EventArgs) Handles ForwardStripButton.Click
			Me.NavigateForward()
		End Sub

		Private Function NavigateBackward() As Boolean
			Dim max As DateTime = Nothing
			Dim iLine As Integer = -1
			Dim tb As FastColoredTextBox = Nothing
			For iTab As Integer = 0 To Me.TsFiles.Items.Count - 1
				Dim t As FastColoredTextBox = TryCast(Me.TsFiles.Items(iTab).Controls(0), FastColoredTextBox)
				For i As Integer = 0 To t.LinesCount - 1
					If t(i).LastVisit < Me.lastNavigatedDateTime AndAlso t(i).LastVisit > max Then
						max = t(i).LastVisit
						iLine = i
						tb = t
					End If
				Next
			Next
			Dim result As Boolean
			If iLine >= 0 Then
				Me.TsFiles.SelectedItem = TryCast(tb.Parent, FATabStripItem)
				tb.Navigate(iLine)
				Me.lastNavigatedDateTime = tb(iLine).LastVisit
				Console.WriteLine("Backward: " + Me.lastNavigatedDateTime)
				tb.Focus()
				tb.Invalidate()
				result = True
			Else
				result = False
			End If
			Return result
		End Function

		Private Function NavigateForward() As Boolean
			Dim min As DateTime = DateTime.Now
			Dim iLine As Integer = -1
			Dim tb As FastColoredTextBox = Nothing
			For iTab As Integer = 0 To Me.TsFiles.Items.Count - 1
				Dim t As FastColoredTextBox = TryCast(Me.TsFiles.Items(iTab).Controls(0), FastColoredTextBox)
				For i As Integer = 0 To t.LinesCount - 1
					If t(i).LastVisit > Me.lastNavigatedDateTime AndAlso t(i).LastVisit < min Then
						min = t(i).LastVisit
						iLine = i
						tb = t
					End If
				Next
			Next
			Dim result As Boolean
			If iLine >= 0 Then
				Me.TsFiles.SelectedItem = TryCast(tb.Parent, FATabStripItem)
				tb.Navigate(iLine)
				Me.lastNavigatedDateTime = tb(iLine).LastVisit
				Console.WriteLine("Forward: " + Me.lastNavigatedDateTime)
				tb.Focus()
				tb.Invalidate()
				result = True
			Else
				result = False
			End If
			Return result
		End Function

		Private Sub AutoIndentSelectedTextToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AutoIndentSelectedTextToolStripMenuItem.Click
			Me.CurrentTB.DoAutoIndent()
		End Sub

		Private Sub BtInvisibleChars_Click(sender As Object, e As EventArgs) Handles BtInvisibleChars.Click
			For Each tab As FATabStripItem In Me.TsFiles.Items
				Me.HighlightInvisibleChars(TryCast(tab.Controls(0), FastColoredTextBox).Range)
			Next
			If Me.CurrentTB IsNot Nothing Then
				Me.CurrentTB.Invalidate()
			End If
		End Sub

		Private Sub BtHighlightCurrentLine_Click(sender As Object, e As EventArgs) Handles BtHighlightCurrentLine.Click
			For Each tab As FATabStripItem In Me.TsFiles.Items
				If Me.BtHighlightCurrentLine.Checked Then
					TryCast(tab.Controls(0), FastColoredTextBox).CurrentLineColor = Me.currentLineColor
				Else
					TryCast(tab.Controls(0), FastColoredTextBox).CurrentLineColor = Color.Transparent
				End If
			Next
			If Me.CurrentTB IsNot Nothing Then
				Me.CurrentTB.Invalidate()
			End If
		End Sub

		Private Sub CommentSelectedToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CommentSelectedToolStripMenuItem.Click
			Me.CurrentTB.InsertLinePrefix("//")
		End Sub

		Private Sub UncommentSelectedToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UncommentSelectedToolStripMenuItem.Click
			Me.CurrentTB.RemoveLinePrefix("//")
		End Sub

		Private Sub CloneLinesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CloneLinesToolStripMenuItem.Click
			Me.CurrentTB.Selection.Expand()
			Dim text As String = Environment.NewLine + Me.CurrentTB.Selection.Text
			Me.CurrentTB.Selection.Start = Me.CurrentTB.Selection.[End]
			Me.CurrentTB.InsertText(text)
		End Sub

		Private Sub CloneLinesAndCommentToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CloneLinesAndCommentToolStripMenuItem.Click
			Me.CurrentTB.BeginAutoUndo()
			Me.CurrentTB.Selection.Expand()
			Dim text As String = Environment.NewLine + Me.CurrentTB.Selection.Text
			Me.CurrentTB.InsertLinePrefix("//")
			Me.CurrentTB.Selection.Start = Me.CurrentTB.Selection.[End]
			Me.CurrentTB.InsertText(text)
			Me.CurrentTB.EndAutoUndo()
		End Sub

		Private Sub BookmarkPlusButton_Click(sender As Object, e As EventArgs) Handles BookmarkPlusButton.Click
			If Me.CurrentTB IsNot Nothing Then
				Dim info As TbInfo = TryCast(Me.CurrentTB.Tag, TbInfo)
				Dim id As Integer = Me.CurrentTB(Me.CurrentTB.Selection.Start.iLine).UniqueId
				If Not info.bookmarksLineId.Contains(id) Then
					info.bookmarks.Add(id)
					info.bookmarksLineId.Add(id)
					Me.CurrentTB.Invalidate()
				End If
			End If
		End Sub

		Private Sub BookmarkMinusButton_Click(sender As Object, e As EventArgs) Handles BookmarkMinusButton.Click
			If Me.CurrentTB IsNot Nothing Then
				Dim info As TbInfo = TryCast(Me.CurrentTB.Tag, TbInfo)
				Dim id As Integer = Me.CurrentTB(Me.CurrentTB.Selection.Start.iLine).UniqueId
				If info.bookmarksLineId.Contains(id) Then
					info.bookmarks.Remove(id)
					info.bookmarksLineId.Remove(id)
					Me.CurrentTB.Invalidate()
				End If
			End If
		End Sub

		Private Sub GotoButton_DropDownOpening(sender As Object, e As EventArgs) Handles GotoButton.DropDownOpening
			Me.GotoButton.DropDownItems.Clear()
			For Each tab As Control In Me.TsFiles.Items
				Dim tb As FastColoredTextBox = TryCast(tab.Controls(0), FastColoredTextBox)
				Dim info As TbInfo = TryCast(tb.Tag, TbInfo)
				For i As Integer = 0 To info.bookmarks.Count - 1
					Dim item As ToolStripItem = Me.GotoButton.DropDownItems.Add(String.Concat(New Object() {"Bookmark ", Me.GotoButton.DropDownItems.Count, " [", Path.GetFileNameWithoutExtension(TryCast(tab.Tag, String)), "]"}))
					item.Tag = New PowerfulCSharpEditor.BookmarkInfo() With {.tb = tb, .iBookmark = i}
					AddHandler item.Click, Sub(o As Object, a As EventArgs)
											   Me.[GoTo](CType(TryCast(o, ToolStripItem).Tag, PowerfulCSharpEditor.BookmarkInfo))
										   End Sub
				Next
			Next
		End Sub

		Private Sub [GoTo](bookmark As PowerfulCSharpEditor.BookmarkInfo)
			Dim info As TbInfo = TryCast(bookmark.tb.Tag, TbInfo)
			Try
				Me.CurrentTB = bookmark.tb
			Catch ex As Exception
				MessageBox.Show(ex.Message)
				Return
			End Try
			If bookmark.iBookmark >= 0 AndAlso bookmark.iBookmark < info.bookmarks.Count Then
				Dim id As Integer = info.bookmarks(bookmark.iBookmark)
				For i As Integer = 0 To Me.CurrentTB.LinesCount - 1
					If Me.CurrentTB(i).UniqueId = id Then
						Me.CurrentTB.Selection.Start = New Place(0, i)
						Me.CurrentTB.DoSelectionVisible()
						Me.CurrentTB.Invalidate()
						Exit For
					End If
				Next
			End If
		End Sub

		Private Sub BtShowFoldingLines_Click(sender As Object, e As EventArgs) Handles BtShowFoldingLines.Click
			For Each tab As FATabStripItem In Me.TsFiles.Items
				TryCast(tab.Controls(0), FastColoredTextBox).ShowFoldingLines = Me.BtShowFoldingLines.Checked
			Next
			If Me.CurrentTB IsNot Nothing Then
				Me.CurrentTB.Invalidate()
			End If
		End Sub
	End Class
End Namespace
