'Imports Rednettle.Warp3D
Imports Warp3DVB.Rednettle.Warp3D

Public Class TriangleTest
    Private mobjW3D As warp_Scene
    Private WithEvents Timer1 As New Timers.Timer
    Private Buf As Drawing.BufferedGraphics


    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Timer1.Stop()
        Timer1 = Nothing
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
        Dim Cam As New warp_Camera
        Cam.pos = New warp_Vector(0, 0, -50)

        mobjW3D.addCamera("Cam", Cam)
        mobjW3D.defaultCamera = Cam
        mobjW3D.environment.bgcolor = Color.Gray.ToArgb
        Me.Size = New Size(512, 512) + (Me.Size - Me.ClientSize)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint, True)

        Drawing.BufferedGraphicsManager.Current.MaximumBuffer = New Size(1, 1) + Me.ClientSize
        Buf = Drawing.BufferedGraphicsManager.Current.Allocate(Me.CreateGraphics, Me.ClientRectangle)

        Dim Mat As New warp_Material(Color.Red.ToArgb)
        mobjW3D.addMaterial("Red", Mat)

        Dim Obj As New warp_Object
        Dim Verts() As warp_Vertex = {New warp_Vertex(0, 0, 0, 0, 0), New warp_Vertex(1, 0, 0, 1, 0), _
                                      New warp_Vertex(1, 1, 0, 1, 1), New warp_Vertex(0, 1, 0, 0, 1)}

        Dim tri1 As New warp_Triangle(Verts(0), Verts(1), Verts(2))
        Dim tri2 As New warp_Triangle(Verts(1), Verts(2), Verts(3))
        tri1.regenerateNormal()
        tri2.regenerateNormal()
        Obj.addTriangle(tri1)
        Obj.addTriangle(tri2)
        Obj.setMaterial(mobjW3D.material("Red"))
        Obj.scaleSelf(25)
        mobjW3D.addObject("Tri", Obj)

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

            Dim M As New warp_Matrix


            ' render and measure performance
            Dim SW As New Stopwatch
            SW.Reset()
            SW.Start()

            ' render scene
            mobjW3D.render()

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
        '    rendering = False
        Timer1.Start()
    End Sub
End Class