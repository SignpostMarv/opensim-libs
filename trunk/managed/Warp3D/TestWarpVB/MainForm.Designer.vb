<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.label1 = New System.Windows.Forms.Label
        Me.button_Sphere = New System.Windows.Forms.Button
        Me.button_Body = New System.Windows.Forms.Button
        Me.button_Physics = New System.Windows.Forms.Button
        Me.button_Studio = New System.Windows.Forms.Button
        Me.button_Bounce = New System.Windows.Forms.Button
        Me.button_Stones = New System.Windows.Forms.Button
        Me.button_Flare1 = New System.Windows.Forms.Button
        Me.Load_Scene = New System.Windows.Forms.Button
        Me.warp3D1 = New Warp3DVB.Warp3D.Warp3D
        Me.SuspendLayout()
        '
        'label1
        '
        Me.label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(9, 446)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(355, 13)
        Me.label1.TabIndex = 22
        Me.label1.Text = "Use the mouse to rotate the scene. Hold down control to zoom in and out."
        '
        'button_Sphere
        '
        Me.button_Sphere.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.button_Sphere.Enabled = False
        Me.button_Sphere.Location = New System.Drawing.Point(488, 41)
        Me.button_Sphere.Name = "button_Sphere"
        Me.button_Sphere.Size = New System.Drawing.Size(42, 23)
        Me.button_Sphere.TabIndex = 21
        Me.button_Sphere.Text = "sphere"
        '
        'button_Body
        '
        Me.button_Body.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.button_Body.Enabled = False
        Me.button_Body.Location = New System.Drawing.Point(488, 12)
        Me.button_Body.Name = "button_Body"
        Me.button_Body.Size = New System.Drawing.Size(42, 23)
        Me.button_Body.TabIndex = 20
        Me.button_Body.Text = "cube"
        '
        'button_Physics
        '
        Me.button_Physics.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.button_Physics.Location = New System.Drawing.Point(414, 420)
        Me.button_Physics.Name = "button_Physics"
        Me.button_Physics.Size = New System.Drawing.Size(75, 23)
        Me.button_Physics.TabIndex = 19
        Me.button_Physics.Text = "Physics"
        '
        'button_Studio
        '
        Me.button_Studio.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.button_Studio.Location = New System.Drawing.Point(333, 420)
        Me.button_Studio.Name = "button_Studio"
        Me.button_Studio.Size = New System.Drawing.Size(75, 23)
        Me.button_Studio.TabIndex = 18
        Me.button_Studio.Text = "3Ds"
        '
        'button_Bounce
        '
        Me.button_Bounce.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.button_Bounce.Location = New System.Drawing.Point(252, 420)
        Me.button_Bounce.Name = "button_Bounce"
        Me.button_Bounce.Size = New System.Drawing.Size(75, 23)
        Me.button_Bounce.TabIndex = 17
        Me.button_Bounce.Text = "Objects"
        '
        'button_Stones
        '
        Me.button_Stones.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.button_Stones.Location = New System.Drawing.Point(171, 420)
        Me.button_Stones.Name = "button_Stones"
        Me.button_Stones.Size = New System.Drawing.Size(75, 23)
        Me.button_Stones.TabIndex = 16
        Me.button_Stones.Text = "Stones"
        '
        'button_Flare1
        '
        Me.button_Flare1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.button_Flare1.Location = New System.Drawing.Point(90, 420)
        Me.button_Flare1.Name = "button_Flare1"
        Me.button_Flare1.Size = New System.Drawing.Size(75, 23)
        Me.button_Flare1.TabIndex = 15
        Me.button_Flare1.Text = "Flare"
        '
        'Load_Scene
        '
        Me.Load_Scene.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Load_Scene.Location = New System.Drawing.Point(9, 420)
        Me.Load_Scene.Name = "Load_Scene"
        Me.Load_Scene.Size = New System.Drawing.Size(75, 23)
        Me.Load_Scene.TabIndex = 14
        Me.Load_Scene.Text = "Chair"
        '
        'warp3D1
        '
        Me.warp3D1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.warp3D1.Location = New System.Drawing.Point(12, 12)
        Me.warp3D1.Name = "warp3D1"
        Me.warp3D1.Size = New System.Drawing.Size(470, 402)
        Me.warp3D1.TabIndex = 13
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(542, 468)
        Me.Controls.Add(Me.label1)
        Me.Controls.Add(Me.button_Sphere)
        Me.Controls.Add(Me.button_Body)
        Me.Controls.Add(Me.button_Physics)
        Me.Controls.Add(Me.button_Studio)
        Me.Controls.Add(Me.button_Bounce)
        Me.Controls.Add(Me.button_Stones)
        Me.Controls.Add(Me.button_Flare1)
        Me.Controls.Add(Me.Load_Scene)
        Me.Controls.Add(Me.warp3D1)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents label1 As System.Windows.Forms.Label
    Private WithEvents button_Sphere As System.Windows.Forms.Button
    Private WithEvents button_Body As System.Windows.Forms.Button
    Private WithEvents button_Physics As System.Windows.Forms.Button
    Private WithEvents button_Studio As System.Windows.Forms.Button
    Private WithEvents button_Bounce As System.Windows.Forms.Button
    Private WithEvents button_Stones As System.Windows.Forms.Button
    Private WithEvents button_Flare1 As System.Windows.Forms.Button
    Private WithEvents Load_Scene As System.Windows.Forms.Button
    Private WithEvents warp3D1 As Warp3DVB.Warp3D.Warp3D

End Class
