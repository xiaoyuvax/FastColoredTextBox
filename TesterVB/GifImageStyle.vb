Namespace TesterVB
    Friend Class GifImageStyle
        Inherits TextStyle

        Private ReadOnly timer As Timer

        Public Property ImagesByText() As Dictionary(Of String, Image)

        Public Sub New(parent As FastColoredTextBox)
            MyBase.New(Nothing, Nothing, FontStyle.Regular)
            Me.ImagesByText = New Dictionary(Of String, Image)()
            Me.timer = New Timer With {
                .Interval = 100
            }
            AddHandler Me.timer.Tick, Sub()
                                          ImageAnimator.UpdateFrames()
                                          parent.Invalidate()
                                      End Sub
            Me.timer.Start()
        End Sub

        Public Sub StartAnimation()
            For Each image As Image In Me.ImagesByText.Values
                If ImageAnimator.CanAnimate(image) Then
                    ImageAnimator.Animate(image, New EventHandler(AddressOf Me.OnFrameChanged))
                End If
            Next
        End Sub

        Private Sub OnFrameChanged(sender As Object, args As EventArgs)
        End Sub

        Public Overrides Sub Draw(gr As Graphics, position As Point, range As Range)
            Dim text As String = range.Text
            Dim iChar As Integer = range.Start.iChar
            While text <> ""
                Dim replaced As Boolean = False
                For Each pair As KeyValuePair(Of String, Image) In Me.ImagesByText
                    If text.StartsWith(pair.Key) Then
                        Dim i As Single = CSng(pair.Key.Length * range.tb.CharWidth) / CSng(pair.Value.Width)
                        If i > 1.0F Then
                            i = 1.0F
                        End If
                        text = text.Substring(pair.Key.Length)
                        Dim rect As New RectangleF(CSng(position.X + range.tb.CharWidth * pair.Key.Length / 2) - CSng(pair.Value.Width) * i / 2.0F, CSng(position.Y), CSng(pair.Value.Width) * i, CSng(pair.Value.Height) * i)
                        gr.DrawImage(pair.Value, rect)
                        position.Offset(range.tb.CharWidth * pair.Key.Length, 0)
                        replaced = True
                        iChar += pair.Key.Length
                        Exit For
                    End If
                Next
                If Not replaced AndAlso text.Length > 0 Then
                    Dim r As New Range(range.tb, iChar, range.Start.iLine, iChar + 1, range.Start.iLine)
                    MyBase.Draw(gr, position, r)
                    position.Offset(range.tb.CharWidth, 0)
                    text = text.Substring(1)
                End If
            End While
        End Sub
    End Class
End Namespace
