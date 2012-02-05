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
    '/ Summary description for warp_TextureProjector.
    '/ </summary>
    Public Class warp_TextureProjector
        Public Shared Sub projectFrontal(ByVal obj As warp_Object)
            obj.rebuild()
            Dim min As warp_Vector = obj.minimum()
            Dim max As warp_Vector = obj.maximum()
            Dim du As Single = 1 / (max.x - min.x)
            Dim dv As Single = 1 / (max.y - min.y)

            Dim i As Integer
            For i = 0 To obj.vertices - 1 Step i + 1
                obj.fastvertex(i).u = (obj.fastvertex(i).pos.x - min.x) * du
                obj.fastvertex(i).v = 1 - (obj.fastvertex(i).pos.y - min.y) * dv
            Next
        End Sub

        Public Shared Sub projectTop(ByVal obj As warp_Object)
            obj.rebuild()
            Dim min As warp_Vector = obj.minimum()
            Dim max As warp_Vector = obj.maximum()
            Dim du As Single = 1 / (max.x - min.x)
            Dim dv As Single = 1 / (max.z - min.z)
            Dim i As Integer
            For i = 0 To obj.vertices - 1 Step i + 1
                obj.fastvertex(i).u = (obj.fastvertex(i).pos.x - min.x) * du
                obj.fastvertex(i).v = (obj.fastvertex(i).pos.z - min.z) * dv
            Next
        End Sub

        Public Shared Sub projectCylindric(ByVal obj As warp_Object)
            obj.rebuild()
            Dim min As warp_Vector = obj.minimum()
            Dim max As warp_Vector = obj.maximum()
            Dim dz As Single = 1 / (max.z - min.z)
            Dim i As Integer
            For i = 0 To obj.vertices - 1 Step i + 1
                obj.fastvertex(i).pos.buildCylindric()
                obj.fastvertex(i).u = obj.fastvertex(i).pos.theta / (2 * 3.14159274F)
                obj.fastvertex(i).v = (obj.fastvertex(i).pos.z - min.z) * dz
            Next
        End Sub
    End Class
End Namespace
