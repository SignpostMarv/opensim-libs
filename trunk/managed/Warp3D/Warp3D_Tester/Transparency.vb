'Imports Rednettle.Warp3D
Imports Warp3DVB.Rednettle.Warp3D

Public Class Transparency
   

    Private mobjW3D As warp_Scene
    Private CB As warp_Texture
    Private WithEvents Timer1 As New Timers.Timer
    Private Buf As Drawing.BufferedGraphics
    Private Ti As warp_Texture
    Private MP As Point

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Timer1.Stop()
        Timer1 = Nothing
        CB = Nothing
        Ti = Nothing
        mobjW3D = Nothing
        Buf.Dispose()
        Buf = Nothing
    End Sub

    Private Sub Form1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Keys.Escape
                Me.Close()

        End Select
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Show()
        Application.DoEvents()
        mobjW3D = New warp_Scene(512, 512)
        'mobjW3D.defaultCamera = New warp_Camera
        'mobjW3D.defaultCamera.pos = New warp_Vector(0, 0, -100)
        mobjW3D.environment.bgcolor = Color.Gray.ToArgb
        Me.Size = New Size(512, 512) + (Me.Size - Me.ClientSize)
        CB = New warp_Texture("CBX.png")
        Ti = New warp_Texture("flare1.png")
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint, True)

        Drawing.BufferedGraphicsManager.Current.MaximumBuffer = New Size(1, 1) + Me.ClientSize
        Buf = Drawing.BufferedGraphicsManager.Current.Allocate(Me.CreateGraphics, Me.ClientRectangle)


        Timer1.AutoReset = True
        Timer1.Interval = 1
        Timer1.Start()
    End Sub

    Private Sub Timer1_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles Timer1.Elapsed
        If Timer1 Is Nothing Then Exit Sub

        With mobjW3D
            .renderPipeline.screen.clear(.environment.bgcolor)
           
            Dim SW As New Stopwatch
            SW.Reset()
            SW.Start()
            .renderPipeline.screen.draw(CB, 0, 0, CB.width, CB.height)
            .renderPipeline.screen.add(Ti, MP.X, MP.Y, Ti.width, Ti.height)
            SW.Stop()
            Buf.Graphics.DrawImage(.getImage, 0, 0)
            Dim Msg As String = String.Empty
            ' Msg = "Sprite Count: " & Sprites.Count.ToString & vbCrLf
            Msg &= "Time to render (Ticks): " & SW.ElapsedTicks.ToString & vbCrLf
            Msg &= "Time to render (Milli) :" & SW.ElapsedMilliseconds.ToString & vbCrLf
            ' Msg &= "Pixels processed: " & ((CB.width * CB.height) * Sprites.Count).ToString & vbCrLf
            ' Msg &= "Bytes processed: " & ((CB.width * CB.height * 4) * Sprites.Count).ToString & vbCrLf
            Buf.Graphics.DrawString(Msg, Me.Font, Brushes.Red, 0, 0)
            SW = Nothing
        End With
        Buf.Render()
    End Sub

    Private Sub Transparency_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
        MP = New Point(e.X, e.Y)
    End Sub
End Class