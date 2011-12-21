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
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.rdoCS = New System.Windows.Forms.RadioButton
        Me.rdoVB = New System.Windows.Forms.RadioButton
        Me.chkBoth = New System.Windows.Forms.CheckBox
        Me.lbTests = New System.Windows.Forms.ListBox
        Me.btnExit = New System.Windows.Forms.Button
        Me.btnRun = New System.Windows.Forms.Button
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.rdoCS)
        Me.GroupBox1.Controls.Add(Me.rdoVB)
        Me.GroupBox1.Controls.Add(Me.chkBoth)
        Me.GroupBox1.Location = New System.Drawing.Point(13, 210)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(153, 74)
        Me.GroupBox1.TabIndex = 2
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Use Library"
        '
        'rdoCS
        '
        Me.rdoCS.AutoSize = True
        Me.rdoCS.Location = New System.Drawing.Point(103, 20)
        Me.rdoCS.Name = "rdoCS"
        Me.rdoCS.Size = New System.Drawing.Size(39, 17)
        Me.rdoCS.TabIndex = 2
        Me.rdoCS.Text = "C#"
        Me.rdoCS.UseVisualStyleBackColor = True
        '
        'rdoVB
        '
        Me.rdoVB.AutoSize = True
        Me.rdoVB.Checked = True
        Me.rdoVB.Location = New System.Drawing.Point(7, 20)
        Me.rdoVB.Name = "rdoVB"
        Me.rdoVB.Size = New System.Drawing.Size(64, 17)
        Me.rdoVB.TabIndex = 1
        Me.rdoVB.TabStop = True
        Me.rdoVB.Text = "VB.NET"
        Me.rdoVB.UseVisualStyleBackColor = True
        '
        'chkBoth
        '
        Me.chkBoth.AutoSize = True
        Me.chkBoth.Location = New System.Drawing.Point(7, 43)
        Me.chkBoth.Name = "chkBoth"
        Me.chkBoth.Size = New System.Drawing.Size(71, 17)
        Me.chkBoth.TabIndex = 0
        Me.chkBoth.Text = "Run Both"
        Me.chkBoth.UseVisualStyleBackColor = True
        '
        'lbTests
        '
        Me.lbTests.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbTests.FormattingEnabled = True
        Me.lbTests.IntegralHeight = False
        Me.lbTests.Items.AddRange(New Object() {"Texture Generation", "Sprite Test", "Animated Sprite Test", "Triangle Test", "Background Image Test", "Screen Drawing methods (warp_Screen object)", "Transparency Test"})
        Me.lbTests.Location = New System.Drawing.Point(12, 12)
        Me.lbTests.Name = "lbTests"
        Me.lbTests.Size = New System.Drawing.Size(299, 192)
        Me.lbTests.TabIndex = 4
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(235, 261)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(75, 23)
        Me.btnExit.TabIndex = 5
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = True
        '
        'btnRun
        '
        Me.btnRun.Location = New System.Drawing.Point(234, 211)
        Me.btnRun.Name = "btnRun"
        Me.btnRun.Size = New System.Drawing.Size(75, 23)
        Me.btnRun.TabIndex = 6
        Me.btnRun.Text = "Run"
        Me.btnRun.UseVisualStyleBackColor = True
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(323, 296)
        Me.Controls.Add(Me.btnRun)
        Me.Controls.Add(Me.btnExit)
        Me.Controls.Add(Me.lbTests)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "MainForm"
        Me.Text = "MainForm"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents rdoCS As System.Windows.Forms.RadioButton
    Friend WithEvents rdoVB As System.Windows.Forms.RadioButton
    Friend WithEvents chkBoth As System.Windows.Forms.CheckBox
    Friend WithEvents lbTests As System.Windows.Forms.ListBox
    Friend WithEvents btnExit As System.Windows.Forms.Button
    Friend WithEvents btnRun As System.Windows.Forms.Button
End Class
