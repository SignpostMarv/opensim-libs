'Imports Rednettle.Warp3D
Imports Warp3DVB.Rednettle.Warp3D

Public Class ScreenDrawingMethods

    Private Enum DrawMethod
        Draw = 0
        Add = 1
        Subtract = 2
        Mix = 3
        Multiply = 4
        SubNegative = 5
        Invert = 6
        Modulate = 7
        Min = Draw
        Max = Modulate
    End Enum

    Private mobjW3D As warp_Scene
    Private CB As warp_Texture
    Private Buf As Drawing.BufferedGraphics
    Private UsingWhat As DrawMethod = DrawMethod.Add


    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        CB = Nothing
        mobjW3D = Nothing
        Buf.Dispose()
        Buf = Nothing
    End Sub

    Private Sub Form1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Keys.Escape
                Me.Close()

            Case Keys.Space
                Dim Val As Integer = UsingWhat
                Val += 1
                If Val > DrawMethod.Max Then Val = DrawMethod.Min
                UsingWhat = CType(Val, DrawMethod)
        End Select
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Show()
        Application.DoEvents()
        mobjW3D = New warp_Scene(512, 512)
        mobjW3D.environment.bgcolor = Color.Gray.ToArgb
        Me.Size = New Size(512, 512) + (Me.Size - Me.ClientSize)
        CB = New warp_Texture("CBX256x256.png")
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint, True)

        Drawing.BufferedGraphicsManager.Current.MaximumBuffer = New Size(1, 1) + Me.ClientSize
        Buf = Drawing.BufferedGraphicsManager.Current.Allocate(Me.CreateGraphics, Me.ClientRectangle)
    End Sub

    Private Sub BackgroundImageTest_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
        If mobjW3D Is Nothing Then Exit Sub
        With mobjW3D
            .renderPipeline.screen.clear(.environment.bgcolor)
            .renderPipeline.screen.draw(CB, (Me.ClientSize.Width \ 2) - (CB.width \ 2), (Me.ClientSize.Height \ 2) - (CB.height \ 2), CB.width, CB.height)
            Select Case UsingWhat
                Case DrawMethod.Add
                    .renderPipeline.screen.add(CB, e.X - (CB.width \ 2), e.Y - (CB.height \ 2), CB.width, CB.height)

                Case DrawMethod.Draw
                    .renderPipeline.screen.draw(CB, e.X - (CB.width \ 2), e.Y - (CB.height \ 2), CB.width, CB.height)

                Case DrawMethod.Subtract
                    .renderPipeline.screen.Subtract(CB, e.X - (CB.width \ 2), e.Y - (CB.height \ 2), CB.width, CB.height)

                Case DrawMethod.Mix
                    .renderPipeline.screen.Mix(CB, e.X - (CB.width \ 2), e.Y - (CB.height \ 2), CB.width, CB.height)

                Case DrawMethod.Multiply
                    .renderPipeline.screen.Multiply(CB, e.X - (CB.width \ 2), e.Y - (CB.height \ 2), CB.width, CB.height)

                Case DrawMethod.SubNegative
                    .renderPipeline.screen.SubNegative(CB, e.X - (CB.width \ 2), e.Y - (CB.height \ 2), CB.width, CB.height)

                Case DrawMethod.Invert
                    .renderPipeline.screen.Invert(CB, e.X - (CB.width \ 2), e.Y - (CB.height \ 2), CB.width, CB.height)

                Case DrawMethod.Modulate
                    .renderPipeline.screen.Modulate(CB, e.X - (CB.width \ 2), e.Y - (CB.height \ 2), CB.width, CB.height)
            End Select
           
            Buf.Graphics.DrawImage(.getImage, 0, 0)
            Buf.Graphics.DrawString("Using Method: " & UsingWhat.ToString, Me.Font, SystemBrushes.WindowText, 0, 0)
        End With
        Buf.Render()
    End Sub
End Class