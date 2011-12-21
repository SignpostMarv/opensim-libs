' Created by: X
' http://www.createdbyx.com/
' ----------------------
' C# to VB.NET conversion of this code was done by hand and with the assistance of the 
' Convert C# to VB .NET application availible at http://www.kamalpatel.net/ConvertCSharp2VB.aspx
' ConvertCSharp2VB utility was developed by: Kamal Patel (http://www.KamalPatel.net) 
' ----------------------
' Thanks to Alan Simes ( http://alansimes.blogdns.net/ ) for creating the Warp3D library.
' http://alansimes.blogdns.net/cs/files/default.aspx
' ----------------------

Imports System

Namespace Rednettle.Warp3D
    '/ <summary>
    '/ Summary description for warp_Environment.
    '/ </summary>
    Public Class warp_Environment
        Public ambient As Integer = 0
        Public fogcolor As Integer = 0
        Public fogfact As Integer = 0
        Public bgcolor As Integer = &HFFFFFFFF

        Public background As warp_Texture = Nothing

        Public Sub setBackground(ByVal t As warp_Texture)
            background = t
        End Sub
    End Class
End Namespace