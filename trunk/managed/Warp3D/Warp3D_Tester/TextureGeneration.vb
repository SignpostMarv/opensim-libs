'Imports Rednettle.Warp3D
Imports Warp3DVB.Rednettle.Warp3D

Public Class TextureGeneration
    Private mobjW3D As warp_Scene
    Private CB As warp_Texture
    Private WithEvents Timer1 As New Timers.Timer
    Private Buf As Drawing.BufferedGraphics
    Private TextureID As Integer = 1
    Private params(4) As Single
    Private TimeToGen As Long

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Timer1.Stop()
        Timer1 = Nothing
        CB = Nothing
        CB = Nothing
        mobjW3D = Nothing
        Buf.Dispose()
        Buf = Nothing
    End Sub

    Private Sub Form1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Dim Ammount As Single
        If e.Control Then Ammount = 1 Else Ammount = 0.1

        Select Case e.KeyCode
            Case Keys.Escape
                Me.Close()

            Case Keys.F1
                TextureID = 1
            Case Keys.F2
                TextureID = 2
            Case Keys.F3
                TextureID = 3
            Case Keys.F4
                TextureID = 4
            Case Keys.F5
                TextureID = 5
            Case Keys.F6
                TextureID = 6
            Case Keys.F7
                TextureID = 7

            Case Keys.Q
                params(0) -= Ammount

            Case Keys.W
                params(0) += Ammount

            Case Keys.A
                params(1) -= Ammount

            Case Keys.S
                params(1) += Ammount

            Case Keys.Z
                params(2) -= Ammount

            Case Keys.X
                params(2) += Ammount

            Case Keys.E
                params(3) -= Ammount

            Case Keys.R
                params(3) += Ammount

            Case Keys.D
                params(4) -= Ammount

            Case Keys.F
                params(4) += Ammount

            Case Keys.Space
                UpdateParams()
        End Select
    End Sub

    Private Sub UpdateParams()
        Dim SW As New Stopwatch
        SW.Reset()
        SW.Start()
        Select Case Me.TextureID
            Case 1
                CB = warp_TextureFactory.CHECKERBOARD(512, 512, CInt(params(0)), Color.White.ToArgb, Color.Black.ToArgb)

            Case 2
                CB = warp_TextureFactory.GRAIN(512, 512, params(0), params(1), CInt(params(2)), CInt(params(3)), CInt(params(4)))

            Case 3
                CB = warp_TextureFactory.MARBLE(512, 512, params(0))

            Case 4
                CB = warp_TextureFactory.PERLIN(512, 512, params(0), params(1), CInt(params(2)), CInt(params(3)))

            Case 5
                CB = warp_TextureFactory.SKY(512, 512, params(0))

            Case 6
                CB = warp_TextureFactory.WAVE(512, 512, params(0), params(1), CInt(params(2)), CInt(params(3)))

            Case 7
                CB = warp_TextureFactory.WOOD(512, 512, params(0))

        End Select
        SW.Stop()
        Me.TimeToGen = SW.ElapsedTicks
        SW = Nothing
    End Sub


    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        mobjW3D = New warp_Scene(512, 512)
        mobjW3D.defaultCamera = New warp_Camera
        mobjW3D.defaultCamera.pos = New warp_Vector(0, 0, -100)
        mobjW3D.environment.bgcolor = Color.Gray.ToArgb
        Me.Size = New Size(512, 512) + (Me.Size - Me.ClientSize)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint, True)

        Drawing.BufferedGraphicsManager.Current.MaximumBuffer = New Size(1, 1) + Me.ClientSize
        Buf = Drawing.BufferedGraphicsManager.Current.Allocate(Me.CreateGraphics, Me.ClientRectangle)
        CB = warp_TextureFactory.CHECKERBOARD(512, 512, 4, Color.White.ToArgb, Color.Black.ToArgb)
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
            .renderPipeline.screen.draw(CB, 0, 0, 512, 512)
            SW.Stop()
            Buf.Graphics.DrawImage(.getImage, 0, 0)
            Dim Msg As String = ""
            Msg &= "Time to render (Ticks): " & SW.ElapsedTicks.ToString & vbCrLf
            Msg &= "Time to render (Milli):" & SW.ElapsedMilliseconds.ToString & vbCrLf
            Msg &= "Last Time to Generate (Ticks):" & Me.TimeToGen.ToString & vbCrLf
            Msg &= "Last Time to Generate (Milli):" & (Me.TimeToGen \ TimeSpan.TicksPerMillisecond).ToString & vbCrLf
            For idx As Integer = 0 To GetParamCount()
                Msg &= "Param(" & idx.ToString & "): " & params(idx).ToString & vbCrLf
            Next
            Buf.Graphics.DrawString(Msg, Me.Font, Brushes.Red, 0, 0)
            SW = Nothing
        End With
        Buf.Render()

    End Sub

    Private Function GetParamCount() As Integer
        If TextureID = 1 Then Return 0
        If TextureID = 2 Then Return 4
        If TextureID = 3 Then Return 0
        If TextureID = 4 Then Return 3
        If TextureID = 5 Then Return 0
        If TextureID = 6 Then Return 3
        If TextureID = 7 Then Return 0
    End Function
End Class
