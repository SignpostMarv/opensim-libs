Public Class Form1
    Private mobjGraphics As Drawing.BufferedGraphics
    Private WithEvents mobjRenderTimer As New Timers.Timer
    Private mobjBackBuffer As Image
    Private mobjCBXImage As Image
    Private mobjFlareImage As Image
    Private mobjMousePosition As Point
    Private Scale As Single = 1

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        mobjBackBuffer.Dispose()
        mobjBackBuffer = Nothing
        mobjCBXImage.Dispose()
        mobjCBXImage = Nothing
        mobjRenderTimer.Stop()
        mobjRenderTimer = Nothing
        mobjGraphics.Dispose()
        mobjGraphics = Nothing
    End Sub

    Private Sub Form1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Keys.Escape
                Me.Close()

            Case Keys.Up
                Scale += 0.01
            Case Keys.Down
                Scale -= 0.01
        End Select
    End Sub

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Size = New Size(512, 512) + (Me.Size - Me.ClientSize)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint, True)

        Drawing.BufferedGraphicsManager.Current.MaximumBuffer = New Size(1, 1) + Me.ClientSize
        mobjGraphics = Drawing.BufferedGraphicsManager.Current.Allocate(Me.CreateGraphics, Me.ClientRectangle)

        mobjBackBuffer = New Image(512, 512)
        'Dim bmp As New Bitmap("CBX256x256.png")
        'Dim G As Graphics = Graphics.FromImage(bmp)
        'G.DrawRectangle(Pens.White, 0, 0, bmp.Width - 1, bmp.Height - 1)
        'G.Dispose()
        'G = Nothing
        mobjCBXImage = New Image(New Bitmap("CBX256x256.png"))

        mobjFlareImage = New Image(New Bitmap("flare1.png"))


        mobjRenderTimer.AutoReset = False
        mobjRenderTimer.Interval = 1
        mobjRenderTimer.Start()
    End Sub

    Private Sub Timer1_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles mobjRenderTimer.Elapsed
        If mobjRenderTimer Is Nothing Then Exit Sub
        If mobjGraphics Is Nothing Then Exit Sub


        Dim SW As New Stopwatch
        SW.Reset()
        SW.Start()
        mobjGraphics.Graphics.Clear(Color.Black)
        mobjBackBuffer.clear(Color.Gray.ToArgb)
        mobjBackBuffer.draw(mobjCBXImage, 0, 0, mobjCBXImage.Width, mobjCBXImage.Height)
        mobjBackBuffer.Modulate(mobjFlareImage, _
            mobjMousePosition.X - (mobjCBXImage.Width \ 2), mobjMousePosition.Y - (mobjCBXImage.Height \ 2), _
            mobjCBXImage.Width, mobjCBXImage.Height, Scale)

        SW.Stop()
        ' Dim MX As New Drawing2D.Matrix
        ' MX.RotateAt(Me.mobjMousePosition.X, New PointF(mobjMousePosition.X, mobjMousePosition.Y))
        ' mobjGraphics.Graphics.Transform = MX

        mobjGraphics.Graphics.DrawImage(mobjBackBuffer.getImage, 0, 0)
        Dim Msg As String = String.Empty
        'Msg = "Sprite Count: " & Sprites.Count.ToString & vbCrLf
        Msg &= "Time to render (Ticks): " & SW.ElapsedTicks.ToString & vbCrLf
        Msg &= "Time to render (Milli) :" & SW.ElapsedMilliseconds.ToString & vbCrLf
        'Msg &= "Pixels processed: " & ((CB.width * CB.height) * Sprites.Count).ToString & vbCrLf
        'Msg &= "Bytes processed: " & ((CB.width * CB.height * 4) * Sprites.Count).ToString & vbCrLf
        mobjGraphics.Graphics.DrawString(Msg, Me.Font, Brushes.Red, 0, 0)
        SW = Nothing

        mobjGraphics.Render()
        If Me.Created Then mobjRenderTimer.Start()
    End Sub

    Private Sub Form1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
        mobjMousepOsition = New Point(e.X, e.Y)
    End Sub
End Class
