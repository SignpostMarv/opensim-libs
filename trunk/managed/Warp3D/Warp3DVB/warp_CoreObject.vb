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
    '/ Summary description for warp_CoreObject.
    '/ </summary>
    '/ 

    Public Class warp_CoreObject
        Public matrix As New warp_Matrix ' = New warp_Matrix()
        Public normalmatrix As New warp_Matrix '= New warp_Matrix()

        Public Sub transform(ByVal m As warp_Matrix)
            matrix.transform(m)
            normalmatrix.transform(m)
        End Sub

        Public Sub shift(ByVal dx As Single, ByVal dy As Single, ByVal dz As Single)
            matrix.shift(dx, dy, dz)
        End Sub

        Public Sub shift(ByVal v As warp_Vector)
            matrix.shift(v.x, v.y, v.z)
        End Sub

        Public Sub scale(ByVal d As Single)
            matrix.scale(d)
        End Sub

        Public Sub scale(ByVal dx As Single, ByVal dy As Single, ByVal dz As Single)
            matrix.scale(dx, dy, dz)
        End Sub

        Public Sub scaleSelf(ByVal d As Single)
            matrix.scaleSelf(d)
        End Sub

        Public Sub scaleSelf(ByVal dx As Single, ByVal dy As Single, ByVal dz As Single)
            matrix.scaleSelf(dx, dy, dz)
        End Sub

        Public Sub rotate(ByVal d As warp_Vector)
            rotateSelf(d.x, d.y, d.z)
        End Sub

        Public Sub rotateSelf(ByVal d As warp_Vector)
            rotateSelf(d.x, d.y, d.z)
        End Sub

        Public Sub rotate(ByVal dx As Single, ByVal dy As Single, ByVal dz As Single)
            matrix.rotate(dx, dy, dz)
            normalmatrix.rotate(dx, dy, dz)
        End Sub

        Public Sub rotate(ByVal quat As warp_Quaternion, ByVal x As Single, ByVal y As Single, ByVal z As Single)
            matrix.rotate(quat, x, y, z)
            normalmatrix.rotate(quat, x, y, z)
        End Sub

        Public Sub rotate(ByVal m As warp_Matrix)
            matrix.rotate(m)
            normalmatrix.rotate(m)
        End Sub


        Public Sub rotateSelf(ByVal dx As Single, ByVal dy As Single, ByVal dz As Single)
            matrix.rotateSelf(dx, dy, dz)
            normalmatrix.rotateSelf(dx, dy, dz)
        End Sub

        Public Sub rotateSelf(ByVal quat As warp_Quaternion)
            matrix.rotateSelf(quat)
            normalmatrix.rotateSelf(quat)
        End Sub

        Public Sub rotateSelf(ByVal m As warp_Matrix)
            matrix.rotateSelf(m)
            normalmatrix.rotateSelf(m)
        End Sub

        Public Sub setPos(ByVal x As Single, ByVal y As Single, ByVal z As Single)
            matrix.m03 = x
            matrix.m13 = y
            matrix.m23 = z
        End Sub

        Public Sub setPos(ByVal v As warp_Vector)
            setPos(v.x, v.y, v.z)
        End Sub

        Public Function getPos() As warp_Vector
            Return New warp_Vector(matrix.m03, matrix.m13, matrix.m23)
        End Function
    End Class
End Namespace
