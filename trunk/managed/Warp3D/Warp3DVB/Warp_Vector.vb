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
    Public Class warp_Vector
        Public x As Single = 0  'Cartesian (default)
        Public y As Single = 0  'Cartesian (default)
        Public z As Single = 0  'Cartesian (default),Cylindric
        Public r As Single = 0  'Cylindric
        Public theta As Single = 0  'Cylindric

        Public Sub New()
        End Sub

        Public Sub New(ByVal xpos As Single, ByVal ypos As Single, ByVal zpos As Single)
            x = xpos
            y = ypos
            z = zpos
        End Sub

        Public Function normalize() As warp_Vector
            Dim dist As Single = length()
            If dist = 0 Then Return Me
            Dim invdist As Single = 1 / dist
            x *= invdist
            y *= invdist
            z *= invdist
            Return Me
        End Function

        Public Function reverse() As warp_Vector
            x = -x
            y = -y
            z = -z
            Return Me
        End Function

        Public Function length() As Single
            Return CType(Math.Sqrt(x * x + y * y + z * z), Single)
        End Function

        Public Function transform(ByVal m As warp_Matrix) As warp_Vector
            Dim Newx As Single = x * m.m00 + y * m.m01 + z * m.m02 + m.m03
            Dim Newy As Single = x * m.m10 + y * m.m11 + z * m.m12 + m.m13
            Dim Newz As Single = x * m.m20 + y * m.m21 + z * m.m22 + m.m23

            Return New warp_Vector(Newx, Newy, Newz)
        End Function

        Public Sub buildCylindric()
            r = CType(Math.Sqrt(x * x + y * y), Single)
            theta = CType(Math.Atan2(x, y), Single)
        End Sub

        Public Sub buildCartesian()
            x = r * warp_Math.cos(theta)
            y = r * warp_Math.sin(theta)
        End Sub

        Public Shared Function getNormal(ByVal a As warp_Vector, ByVal b As warp_Vector) As warp_Vector
            Return vectorProduct(a, b).normalize()
        End Function

        Public Shared Function getNormal(ByVal a As warp_Vector, ByVal b As warp_Vector, ByVal c As warp_Vector) As warp_Vector
            Return vectorProduct(a, b, c).normalize()
        End Function

        Public Shared Function vectorProduct(ByVal a As warp_Vector, ByVal b As warp_Vector) As warp_Vector
            Return New warp_Vector(a.y * b.z - b.y * a.z, a.z * b.x - b.z * a.x, a.x * b.y - b.x * a.y)
        End Function

        Public Shared Function vectorProduct(ByVal a As warp_Vector, ByVal b As warp_Vector, ByVal c As warp_Vector) As warp_Vector
            Return vectorProduct(warp_Vector.sub(b, a), warp_Vector.sub(c, a))
        End Function

        Public Shared Function angle(ByVal a As warp_Vector, ByVal b As warp_Vector) As Single
            a.normalize()
            b.normalize()
            Return (a.x * b.x + a.y * b.y + a.z * b.z)
        End Function

        Public Shared Function add(ByVal a As warp_Vector, ByVal b As warp_Vector) As warp_Vector
            Return New warp_Vector(a.x + b.x, a.y + b.y, a.z + b.z)
        End Function

        Public Shared Function [sub](ByVal a As warp_Vector, ByVal b As warp_Vector) As warp_Vector
            Return New warp_Vector(a.x - b.x, a.y - b.y, a.z - b.z)
        End Function

        Public Shared Function scale(ByVal f As Single, ByVal a As warp_Vector) As warp_Vector
            Return New warp_Vector(f * a.x, f * a.y, f * a.z)
        End Function

        Public Shared Function len(ByVal a As warp_Vector) As Single
            Return CType(Math.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z), Single)
        End Function
        ' 		
        ' 		Public static  void New random(single fact)
        ' 			' returns a random vector
        ' 		{
        ' 			Return New warp_Vector(fact*warp_Math.random(),fact*warp_Math.random(),fact*warp_Math.random())
        ' 		}
        '  
        ' 		Public String toString()
        ' 		{
        ' 			Return New String ("<vector x="+x+" y="+y+" z="+z+">\r\n")
        ' 		}
        ' */

        Public Function getClone() As warp_Vector
            Return New warp_Vector(x, y, z)
        End Function
    End Class
End Namespace
