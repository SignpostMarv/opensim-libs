Public Class MainForm

    Private Sub RunIt(ByVal Type As String)
        If chkBoth.Checked Then
            RunTest(Type, True)
            RunTest(Type, False)
        Else
            RunTest(Type, rdoVB.Checked = True)
        End If
    End Sub

    Private Sub RunTest(ByVal CodeFile As String, ByVal VBVersion As Boolean)

        Dim CD As New Microsoft.VisualBasic.VBCodeProvider
        Dim Lang As String = CodeDom.Compiler.CodeDomProvider.GetLanguageFromExtension(CD.FileExtension)
        Dim Compiler As CodeDom.Compiler.CodeDomProvider = CodeDom.Compiler.CodeDomProvider.CreateProvider(Lang)
        Dim ops As New CodeDom.Compiler.CompilerParameters
        ops.GenerateInMemory = True
        ops.GenerateExecutable = False
        ops.IncludeDebugInformation = True
        Dim Ret As CodeDom.Compiler.CompilerResults

        Dim Data, PrePend As String
        If IO.File.Exists(CodeFile & "." & CD.FileExtension) = False Then Exit Sub
        Data = My.Computer.FileSystem.ReadAllText(CodeFile & "." & CD.FileExtension)
        If VBVersion Then PrePend = "Imports Warp3DVB.Rednettle.Warp3D" Else PrePend = "Imports Rednettle.Warp3D"

        Dim Parts() As String = Nothing
        Parts = Data.Split(New String() {vbCrLf}, StringSplitOptions.None)
        Dim Lines As New Specialized.StringCollection
        Lines.AddRange(Parts)
        Data = PrePend & vbCrLf
        Data &= "imports system" & vbCrLf
        Data &= "imports system.windows.forms" & vbCrLf
        Data &= "imports system.drawing" & vbCrLf
        Data &= "imports system.collections" & vbCrLf
        Data &= "imports microsoft.visualbasic" & vbCrLf
        Data &= "imports system.data" & vbCrLf
        Data &= "imports system.deployment" & vbCrLf
        Data &= "imports system.diagnostics" & vbCrLf
        For index As Integer = 0 To Lines.Count - 1
            If Lines(index).Trim.ToLower.StartsWith("imports ") Then
                ' do nothing
            Else
                Data &= Lines(index) & vbCrLf
            End If
        Next

        Data &= getformcode(CodeFile)
        ops.ReferencedAssemblies.AddRange(New String() {"Warp3D.dll", "Warp3DVB.dll", "System.Windows.Forms.dll", _
                                                        "System.Drawing.dll", "System.Xml.dll", "System.dll", _
                                                        "System.Data.dll", "System.Deployment.dll"})

        Ret = Compiler.CompileAssemblyFromSource(ops, New String() {Data})

        Dim dt As Type = Nothing
        For Each t As Type In Ret.CompiledAssembly.GetTypes
            If t.Name = CodeFile Then
                dt = t
                Exit For

            End If
        Next

        If dt IsNot Nothing Then
            Dim Obj As Object
            Obj = Ret.CompiledAssembly.CreateInstance(dt.FullName)
            If TypeOf Obj Is Form Then
                Dim F As Form = Nothing
                F = CType(Obj, Form)
                If chkBoth.Checked Then
                    F.Show(Me)
                Else
                    F.ShowDialog(Me)
                    F.Close()
                End If
                F = Nothing
                Obj = Nothing
            End If
        End If
        Ret.TempFiles.Delete()
    End Sub

    Private Sub btnRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRun.Click
        If lbTests.SelectedItem Is Nothing Then Exit Sub

        Select Case CStr(lbTests.SelectedItem).Trim.ToLower
            Case "texture generation"
                RunIt("TextureGeneration")

            Case "sprite test"
                RunIt("SpriteTest")

            Case "animated sprite test"
                RunIt("AniSpriteTest")

            Case "triangle test"
                RunIt("TriangleTest")

            Case "background image test"
                RunIt("BackgroundImageTest")

            Case "screen drawing methods (warp_screen object)"
                RunIt("ScreenDrawingMethods")

            Case "transparency test"
                RunIt("Transparency")

        End Select
    End Sub

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    Private Sub lbTests_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbTests.DoubleClick
        btnRun_Click(sender, e)
    End Sub
End Class