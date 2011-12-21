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
    '/ Summary description for warp_Edge.
    '/ </summary>
    '/ 
    Public Class warp_Edge
        Dim a As warp_Vertex, b As warp_Vertex

        Public Sub New()
        End Sub

        Public Sub New(ByVal v1 As warp_Vertex, ByVal v2 As warp_Vertex)
            a = v1
            b = v2
        End Sub

        Public Function start() As warp_Vertex
            Return a
        End Function

        Public Function [end]() As warp_Vertex
            Return b
        End Function
    End Class
End Namespace