Public Class Form1

    Protected Friend WithEvents RenderTimer As Timers.Timer



    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        RenderTimer.Stop()
        RenderTimer = Nothing
    End Sub

    Private Sub CreateScene()
        'W3D.DisplayDefaultScene()
        W3D.CreateScene(Me.ClientSize.Width, Me.ClientSize.Height)
        W3D.AddBox("box", 5, 5, 5)
        W3D.AddMaterial("red")
        W3D.Scene.material("red").color = Color.Red.ToArgb
        W3D.SetObjectMaterial("box", "red")

        W3D.AddLight("light1", 0, 5, 20, Color.White.ToArgb, 100, 40)

        W3D.ShiftDefaultCamera(0, 0, -20)
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Control.CheckForIllegalCrossThreadCalls = False
        CreateScene()

        RenderTimer = New Timers.Timer(33)
        RenderTimer.AutoReset = False
        RenderTimer.Start()
    End Sub

    Private Sub RenderTimer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles RenderTimer.Elapsed

        W3D.Render()
        ' W3D.Refresh()
        If Me.Created AndAlso RenderTimer IsNot Nothing Then RenderTimer.Start()
    End Sub
End Class
