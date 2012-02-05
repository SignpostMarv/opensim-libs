'Imports Rednettle.Warp3D
Imports Warp3DVB.Rednettle.Warp3D

Public Class SpriteTest
    Private Structure SpriteItem
        Public Position As warp_Vector
        Public Direction As warp_Vector
    End Structure

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
                Dim Item As SpriteItem
                Item = New SpriteItem
                Item.Position = New warp_Vector(0, 0, 0)
                'Item.Position = New warp_Vector(Rnd() * 512, Rnd() * 512, 0)
                Item.Direction = New warp_Vector(Rnd() * 1, Rnd() * 1, 0)
                Sprites.Add(Item)

            Case Keys.Down
                If Sprites.Count = 0 Then Exit Select
                Sprites.RemoveAt(Sprites.Count - 1)

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
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint, True)

        Drawing.BufferedGraphicsManager.Current.MaximumBuffer = New Size(1, 1) + Me.ClientSize
        Buf = Drawing.BufferedGraphicsManager.Current.Allocate(Me.CreateGraphics, Me.ClientRectangle)

        Randomize(Now.Ticks)
        For idx As Integer = 0 To 24
            Dim Item As SpriteItem
            Item = New SpriteItem
            Item.Position = New warp_Vector(0, 0, 0)
            'Item.Position = New warp_Vector(Rnd() * 512, Rnd() * 512, 0)
            Item.Direction = New warp_Vector(Rnd() * 1, Rnd() * 1, 0)
            Sprites.Add(Item)
        Next


        Timer1.AutoReset = True
        Timer1.Interval = 1
        Timer1.Start()
    End Sub

    Private Sub Timer1_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles Timer1.Elapsed
        If Timer1 Is Nothing Then Exit Sub

        With mobjW3D
            .renderPipeline.screen.clear(.environment.bgcolor)
            Dim Item As SpriteItem

            Dim SW As New Stopwatch
            SW.Reset()
            SW.Start()
            For idx As Integer = 0 To Sprites.Count - 1
                If Timer1 Is Nothing Then Exit Sub
                Item = Sprites(idx)
                .renderPipeline.screen.draw(CB, CInt(Item.Position.x), CInt(Item.Position.y), CB.width, CB.height)
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
    End Sub
End Class