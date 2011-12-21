'Imports Rednettle.Warp3D
Imports Warp3DVB.Rednettle.Warp3D

Public Class AniSpriteTest
    Private Class SpriteItem
        Public Position As warp_Vector
        Public Direction As warp_Vector
        Public LastTime As Long
        Public Interval As Long
    End Class

    Private mobjW3D As warp_Scene
    Private CB As warp_Texture
    Private WithEvents Timer1 As New Timers.Timer
    Private Sprites As New Generic.List(Of SpriteItem)
    Private Buf As Drawing.BufferedGraphics


    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Timer1.Stop()
        Timer1 = Nothing
        CB = Nothing
        mobjW3D = Nothing
        Buf.Dispose()
        Buf = Nothing
        Sprites.Clear()
        Sprites = Nothing
    End Sub

    Private Sub Form1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Keys.Escape
                Me.Close()

            Case Keys.Up
                Dim Count As Integer = 1
                If e.Control Then Count += 9

                For idx As Integer = 1 To Count
                    addsprite()
                Next

            Case Keys.Down
                Dim Count As Integer = 1
                If e.Control Then Count += 9

                For idx As Integer = 1 To Count
                    If Sprites.Count = 0 Then Exit For
                    Sprites.RemoveAt(Sprites.Count - 1)
                Next

            Case Keys.D1, Keys.NumPad1
                Dim count As Integer = 1000
                If e.Control Then SetSpriteCount(count + Sprites.Count) Else SetSpriteCount(count)

            Case Keys.D2, Keys.NumPad2
                SetSpriteCount(2000)

            Case Keys.D3, Keys.NumPad3
                SetSpriteCount(3000)

            Case Keys.D4, Keys.NumPad4
                SetSpriteCount(4000)

            Case Keys.D4, Keys.NumPad4
                SetSpriteCount(5000)

            Case Keys.D5, Keys.NumPad5
                SetSpriteCount(5000)

            Case Keys.D6, Keys.NumPad6
                SetSpriteCount(6000)

            Case Keys.D7, Keys.NumPad7
                SetSpriteCount(7000)

            Case Keys.D8, Keys.NumPad8
                SetSpriteCount(8000)

            Case Keys.D9, Keys.NumPad9
                SetSpriteCount(9000)

            Case Keys.D0, Keys.NumPad0
                SetSpriteCount(10000)
        End Select
    End Sub

    Private Sub SetSpriteCount(ByVal Count As Integer)
        If Sprites.Count > Count Then
            Sprites.RemoveRange(Count, Sprites.Count - Count)
        Else
            While Sprites.Count <= Count
                AddSprite()
            End While
        End If
    End Sub

    Private Sub AddSprite()
        Dim Item As SpriteItem
        Item = New SpriteItem
        Dim X, Y As Single
        Randomize(Now.Ticks)
        If CBool(CLng(Rnd()) And 1) Then X = 10 Else X = -10
        If CBool(CLng(Rnd()) And 1) Then Y = 10 Else Y = -10

        Item.Direction = New warp_Vector(Rnd() * X, Rnd() * Y, 0)
        Item.Position = New warp_Vector(Rnd() * mobjW3D.renderPipeline.screen.width, Rnd() * mobjW3D.renderPipeline.screen.height, 0)
        Item.Interval = CLng(Rnd() * TimeSpan.TicksPerSecond)
        Sprites.Add(Item)
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Show()
        Application.DoEvents()
        mobjW3D = New warp_Scene(512, 512)
        'mobjW3D.defaultCamera = New warp_Camera
        'mobjW3D.defaultCamera.pos = New warp_Vector(0, 0, -100)
        mobjW3D.environment.bgcolor = Color.Gray.ToArgb
        Me.Size = New Size(512, 512) + (Me.Size - Me.ClientSize)
        CB = New warp_Texture("CBX32x32.png")
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint, True)

        Drawing.BufferedGraphicsManager.Current.MaximumBuffer = New Size(1, 1) + Me.ClientSize
        Buf = Drawing.BufferedGraphicsManager.Current.Allocate(Me.CreateGraphics, Me.ClientRectangle)

        Randomize(Now.Ticks)
        For idx As Integer = 0 To 999
            AddSprite()
        Next


        ' Timer1.AutoReset = True
        Timer1.Interval = 1
        Timer1.Start()
    End Sub

    Private Sub Timer1_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles Timer1.Elapsed
        If Timer1 Is Nothing Then Exit Sub
        '     Static rendering As Boolean = False
        '      If rendering Then Exit Sub
        '       rendering = True
        '        Timer1.Stop()
        With mobjW3D
            .renderPipeline.screen.clear(.environment.bgcolor)
          
            ' update positions
            For Each tmp As SpriteItem In Sprites
                tmp.Position = warp_Vector.add(tmp.Position, tmp.Direction)
                tmp.Position.x = warp_Math.crop(tmp.Position.x, 0, Me.ClientSize.Width - 1)
                tmp.Position.y = warp_Math.crop(tmp.Position.y, 0, Me.ClientSize.Height - 1)
                If Now.Ticks > tmp.LastTime + tmp.Interval Then
                    Dim X, Y As Single
                    Randomize(Now.Ticks)
                    If CBool(CLng(Rnd()) And 1) Then X = 1 Else X = -1
                    If CBool(CLng(Rnd()) And 1) Then Y = 1 Else Y = -1
                    tmp.Direction = New warp_Vector(Rnd() * X, Rnd() * Y, 0)
                    tmp.LastTime = Now.Ticks
                End If
            Next

            ' render and measure performance
            Dim SW As New Stopwatch
            SW.Reset()
            SW.Start()
            For Each item As SpriteItem In Sprites
                If Timer1 Is Nothing Then Exit Sub
                .renderPipeline.screen.draw(CB, CInt(item.Position.x), CInt(item.Position.y), CB.width, CB.height)
            Next
            SW.Stop()
            Buf.Graphics.DrawImage(.getImage, 0, 0)
            Dim Msg As String
            Msg = "Sprite Count: " & Sprites.Count.ToString & vbCrLf
            Msg &= "Time to render (Ticks): " & SW.ElapsedTicks.ToString & vbCrLf
            Msg &= "Time to render (Milli) :" & SW.ElapsedMilliseconds.ToString & vbCrLf
            Msg &= "Pixels processed: " & ((CB.width * CB.height) * Sprites.Count).ToString & vbCrLf
            Msg &= "Bytes processed: " & ((CB.width * CB.height * 4) * Sprites.Count).ToString & vbCrLf
            Buf.Graphics.DrawString(Msg, Me.Font, Brushes.Red, 0, 0)
            SW = Nothing
        End With
        Buf.Render()
        '    rendering = False
        Timer1.Start()
    End Sub
End Class