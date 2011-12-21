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

namespace Rednettle.Warp3D
    Public Class warp_Triangle
        Public parent As warp_Object '  // the object which obtains this triangle
        Public visible As Boolean = True '  //visibility tag for clipping
        Public outOfFrustrum As Boolean = False '  //visibility tag for frustrum clipping

        Public p1 As warp_Vertex '  // first  vertex
        Public p2 As warp_Vertex '  // second vertex
        Public p3 As warp_Vertex '  // third  vertex

        Public n As warp_Vector '  // Normal vector of flat triangle
        Public n2 As warp_Vector ' // Projected Normal vector

        Private minx, maxx, miny, maxy As Integer ' // for clipping
        Private triangleCenter As New warp_Vector()
        Public dist As Single = 0

        Public id As Integer = 0

        Public Sub New(ByVal a As warp_Vertex, ByVal b As warp_Vertex, ByVal c As warp_Vertex)
            p1 = a
            p2 = b
            p3 = c
        End Sub

        Public Sub clipFrustrum(ByVal w As Integer, ByVal h As Integer)
            If parent.material Is Nothing Then
                visible = False
                Return
            End If
            outOfFrustrum = (p1.clipcode And p2.clipcode And p3.clipcode) <> 0
            If outOfFrustrum Then
                visible = False
                Return
            End If
            If n2.z > 0.5 Then
                visible = True
                Return
            End If


            triangleCenter.x = (p1.pos2.x + p2.pos2.x + p3.pos2.x)
            triangleCenter.y = (p1.pos2.y + p2.pos2.y + p3.pos2.y)
            triangleCenter.z = (p1.pos2.z + p2.pos2.z + p3.pos2.z)

            visible = warp_Vector.angle(triangleCenter, n2) > 0
        End Sub


        Public Sub project(ByVal normalProjection As warp_Matrix)
            n2 = n.transform(normalProjection)
            dist = getDist()
        End Sub

        Public Sub regenerateNormal()
            n = warp_Vector.getNormal(p1.pos, p2.pos, p3.pos)
        End Sub

        Public Function getWeightedNormal() As warp_Vector
            Return warp_Vector.vectorProduct(p1.pos, p2.pos, p3.pos)
        End Function

        Public Function getMedium() As warp_Vertex
            Dim cx As Single = (p1.pos.x + p2.pos.x + p3.pos.x) / 3
            Dim cy As Single = (p1.pos.y + p2.pos.y + p3.pos.y) / 3
            Dim cz As Single = (p1.pos.z + p2.pos.z + p3.pos.z) / 3
            Dim cu As Single = (p1.u + p2.u + p3.u) / 3
            Dim cv As Single = (p1.v + p2.v + p3.v) / 3
            Return New warp_Vertex(cx, cy, cz, cu, cv)
        End Function

        Public Function getCenter() As warp_Vector
            Dim cx As Single = (p1.pos.x + p2.pos.x + p3.pos.x) / 3
            Dim cy As Single = (p1.pos.y + p2.pos.y + p3.pos.y) / 3
            Dim cz As Single = (p1.pos.z + p2.pos.z + p3.pos.z) / 3
            Return New warp_Vector(cx, cy, cz)
        End Function

        Public Function getDist() As Single
            Return p1.z + p2.z + p3.z
        End Function

        Public Function degenerated() As Boolean
            Return p1.equals(p2) Or p2.equals(p3) Or p3.equals(p1)
        End Function

        Public Function getClone() As warp_Triangle
            Return New warp_Triangle(p1, p2, p3)
        End Function

    End Class
End Namespace
