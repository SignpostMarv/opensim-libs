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
Imports System.Collections

namespace Rednettle.Warp3D
    Public Class warp_Vertex
        Public parent As warp_Object

        Public pos As New warp_Vector() '   //(x,y,z) Coordinate of vertex
        Public pos2 As warp_Vector '  //Transformed vertex coordinate
        Public n As New warp_Vector() '   //Normal Vector at vertex
        Public n2 As warp_Vector '  //Transformed normal vector (camera space)

        Public x As Integer '  //Projected x coordinate
        Public y As Integer '  //Projected y coordinate
        Public z As Integer '  //Projected z coordinate for z-Buffer

        Public u As Single = 0 ' // Texture x-coordinate (relative)
        Public v As Single = 0 ' // Texture y-coordinate (relative)

        Public nx As Integer = 0 ' // Normal x-coordinate for envmapping
        Public ny As Integer = 0 ' // Normal y-coordinate for envmapping
        Public tx As Integer = 0 ' // Texture x-coordinate (absolute)
        Public ty As Integer = 0 ' // Texture y-coordinate (absolute)

        Public sw As Single = 1.0F


        Public visible As Boolean = True '  //visibility tag for clipping
        Public clipcode As Integer = 0
        Public id As Integer ' // Vertex index

        Private fact As Single
        'private Vector neighbor=new Vector(); //Neighbor triangles of vertex
        Private neighbor As New ArrayList()



        Public Sub New()
            pos = New warp_Vector(0.0F, 0.0F, 0.0F)
        End Sub

        Public Sub New(ByVal xpos As Single, ByVal ypos As Single, ByVal zpos As Single)
            pos = New warp_Vector(xpos, ypos, zpos)
        End Sub

        Public Sub New(ByVal xpos As Single, ByVal ypos As Single, ByVal zpos As Single, ByVal u As Single, ByVal v As Single)
            pos = New warp_Vector(xpos, ypos, zpos)
            Me.u = u
            Me.v = v
        End Sub

        Public Sub New(ByVal ppos As warp_Vector)
            pos = ppos.getClone()
        End Sub

        Public Sub New(ByVal ppos As warp_Vector, ByVal u As Single, ByVal v As Single)
            pos = ppos.getClone()
            Me.u = u
            Me.v = v
        End Sub


        Public Sub project(ByVal vertexProjection As warp_Matrix, ByVal normalProjection As warp_Matrix, ByVal camera As warp_Camera)
            ' Projects this vertex into camera space
            pos2 = pos.transform(vertexProjection)
            n2 = n.transform(normalProjection)

            fact = camera.screenscale / camera.fovfact / CSng(IIf(pos2.z > 0.1, pos2.z, 0.1F))
            x = CInt((pos2.x * fact + (camera.screenwidth >> 1)))
            y = CInt((-pos2.y * fact + (camera.screenheight >> 1)))
            z = CInt((65536.0F * pos2.z))
            sw = -(pos2.z)
            nx = CInt((n2.x * 127 + 127))
            ny = CInt((n2.y * 127 + 127))
            If parent.material Is Nothing Then Return
            If parent.material.texture Is Nothing Then Return
            tx = CInt((parent.material.texture.width * u))
            ty = CInt((parent.material.texture.height * v))
        End Sub

        Public Sub setUV(ByVal u As Single, ByVal v As Single)
            Me.u = u
            Me.v = v
        End Sub


        Public Sub clipFrustrum(ByVal w As Integer, ByVal h As Integer)
            ' View plane clipping
            clipcode = 0
            If x < 0 Then
                clipcode = clipcode Or 1
            End If
            If x >= w Then
                clipcode = clipcode Or 2
            End If
            If y < 0 Then
                clipcode = clipcode Or 4
            End If
            If y >= h Then
                clipcode = clipcode Or 8
            End If
            If pos2.z < 0 Then
                clipcode = clipcode Or 16
            End If
            visible = (clipcode = 0)
        End Sub

        Public Sub registerNeighbor(ByVal triangle As warp_Triangle)
            If Not neighbor.Contains(triangle) Then
                neighbor.Add(triangle)
            End If
        End Sub

        Public Sub resetNeighbors()
            neighbor.Clear()
        End Sub

        Public Sub regenerateNormal()
            Dim nx As Single = 0
            Dim ny As Single = 0
            Dim nz As Single = 0
            Dim Enumerator As IEnumerator = neighbor.GetEnumerator()
           
            Dim tri As warp_Triangle
            Dim wn As warp_Vector
            While Enumerator.MoveNext()
                tri = CType(Enumerator.Current, warp_Triangle)
                wn = tri.getWeightedNormal()
                nx += wn.x
                ny += wn.y
                nz += wn.z
            End While

            n = New warp_Vector(nx, ny, nz).normalize()
        End Sub

        '/*
        '		public void regenerateNormal()
        '		{
        '			float nx=0;
        '			float ny=0;
        '			float nz=0;
        '			Enumeration enum=neighbor.elements();
        '			warp_Triangle tri;
        '			warp_Vector wn;
        '			while (enum.hasMoreElements())
        '			{	
        '				tri=(warp_Triangle)enum.nextElement();
        '				wn=tri.getWeightedNormal();
        '				nx+=wn.x;
        '				ny+=wn.y;
        '				nz+=wn.z;
        '			}

        '			n=new warp_Vector(nx,ny,nz).normalize();
        '		}
        '*/

        Public Sub scaleTextureCoordinates(ByVal fx As Single, ByVal fy As Single)
            u *= fx
            v *= fy
        End Sub

        Public Sub moveTextureCoordinates(ByVal fx As Single, ByVal fy As Single)
            u += fx
            v += fy
        End Sub

        Public Function getClone() As warp_Vertex
            Dim NewVertex As warp_Vertex = New warp_Vertex()
            NewVertex.pos = pos.getClone()
            NewVertex.n = n.getClone()
            NewVertex.u = u
            NewVertex.v = v

            Return NewVertex
        End Function


        Public Function equals(ByVal v As warp_Vertex) As Boolean
            Return ((pos.x = v.pos.x) And (pos.y = v.pos.y) And (pos.z = v.pos.z))
        End Function

        Public Function equals(ByVal v As warp_Vertex, ByVal tolerance As Single) As Boolean
            Return Math.Abs(warp_Vector.sub(pos, v.pos).length()) < tolerance
        End Function
    End Class
End Namespace