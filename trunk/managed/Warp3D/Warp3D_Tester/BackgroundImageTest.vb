'Imports Rednettle.Warp3D
Imports Warp3DVB.Rednettle.Warp3D

Public Class BackgroundImageTest
  
    Private mobjW3D As warp_Scene
    Private CB As warp_Texture


    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        CB = Nothing
        mobjW3D = Nothing
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
         mobjW3D.environment.bgcolor = Color.Gray.ToArgb
        Me.Size = New Size(512, 512) + (Me.Size - Me.ClientSize)
        CB = New warp_Texture("CBX32x32.png")
      
        Me.BackgroundImage = mobjW3D.getImage
    End Sub

    Private Sub BackgroundImageTest_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
        If mobjW3D Is Nothing Then Exit Sub
        With mobjW3D
            .renderPipeline.screen.clear(.environment.bgcolor)
            .renderPipeline.screen.draw(CB, e.X, e.Y, CB.width, CB.height)
         End With
        Me.Invalidate()
    End Sub
End Class