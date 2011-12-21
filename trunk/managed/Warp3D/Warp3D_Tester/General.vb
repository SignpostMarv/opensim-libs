Public Module General
    Public Function LoadResource(ByVal ResourceName As String, _
  Optional ByVal PrePend As String = "Warp3D_Tester.") As IO.MemoryStream
        Dim ResourceStream As IO.Stream = Nothing
        Dim Asm As Reflection.Assembly
        For Each Asm In AppDomain.CurrentDomain.GetAssemblies
            ResourceStream = Asm.GetManifestResourceStream(PrePend & ResourceName)
            If ResourceStream Is Nothing = False Then Exit For
        Next
        If ResourceStream Is Nothing Then Return Nothing

        Dim byts(CInt(ResourceStream.Length - 1)) As Byte
        Dim Len As Integer = ResourceStream.Read(byts, 0, CInt(ResourceStream.Length))

        Dim MemStream As New IO.MemoryStream(byts, 0, Len)
        Return MemStream
    End Function

    Public Function GetFormCode(ByVal type As String) As String
        Dim Tmp As String = ""
        Tmp &= "        <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _" & vbCrLf
        Tmp &= "Partial Class " & type & "                                                   " & vbCrLf
        Tmp &= "    Inherits System.Windows.Forms.Form                                                                 " & vbCrLf
        Tmp &= "" & vbCrLf
        Tmp &= "'Form overrides dispose to clean up the component list.                                                            " & vbCrLf
        Tmp &= "<System.Diagnostics.DebuggerNonUserCode()> _                                                                                     " & vbCrLf
        Tmp &= "Protected Overrides Sub Dispose(ByVal disposing As Boolean)                                                                                " & vbCrLf
        Tmp &= "If Disposing AndAlso components IsNot Nothing Then                                                                                               " & vbCrLf
        Tmp &= "components.Dispose()" & vbCrLf
        Tmp &= "End If                                                                                                                                                           " & vbCrLf
        Tmp &= "MyBase.Dispose(Disposing)                                                                                                                                                  " & vbCrLf
        Tmp &= "End Sub                                                                                                                                                                                  " & vbCrLf
        Tmp &= "" & vbCrLf
        Tmp &= "'Required by the Windows Form Designer                                                                                                                                                                       " & vbCrLf
        Tmp &= "Private components As System.ComponentModel.IContainer                                                                                                                                                                 " & vbCrLf
        Tmp &= "" & vbCrLf
        Tmp &= "'NOTE: The following procedure is required by the Windows Form Designer" & vbCrLf
        Tmp &= "'It can be modified using the Windows Form Designer.                             " & vbCrLf
        Tmp &= "'Do not modify it using the code editor.                                                   " & vbCrLf
        Tmp &= "<System.Diagnostics.DebuggerStepThrough()> _                                                         " & vbCrLf
        Tmp &= "Private Sub InitializeComponent()                                                                              " & vbCrLf
        Tmp &= "components = New System.ComponentModel.Container" & vbCrLf
        Tmp &= "Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font" & vbCrLf
        Tmp &= "Me.Text = """ & type & """                                              " & vbCrLf
        Tmp &= "End Sub                                                                           " & vbCrLf
        Tmp &= "End Class" & vbCrLf
        Tmp &= "" & vbCrLf
        Return Tmp
    End Function
End Module

