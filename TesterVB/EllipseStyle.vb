Imports System.Drawing.Drawing2D
Imports FastColoredTextBoxNS.Types

Namespace TesterVB
	Friend Class EllipseStyle
		Inherits Style

		Public Overrides Sub Draw(gr As Graphics, position As Point, range As TextSelectionRange)
			Dim size As Size = Style.GetSizeOfRange(range)
			Dim rect As New Rectangle(position, size)
			rect.Inflate(2, 2)
			Dim path As GraphicsPath = Style.GetRoundedRectangle(rect, 7)
			gr.DrawPath(Pens.Red, path)
		End Sub
	End Class
End Namespace
