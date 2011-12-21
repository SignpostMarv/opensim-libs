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
    '/ Summary description for warp_ObjectFactory.
    '/ </summary>
    Public Class warp_ObjectFactory
        Public Shared pi As Double = 3.1415926535
        Public Shared deg2rad As Double = pi / 180

        Public Shared Function SIMPLEPLANE(ByVal size As Single, ByVal doubleSided As Boolean) As warp_Object
            Dim NewObject As warp_Object = New warp_Object()

            NewObject.addVertex(New warp_Vertex(-size, 0.0F, size, 0, 0))
            NewObject.addVertex(New warp_Vertex(size, 0.0F, size, 4.0F, 0))
            NewObject.addVertex(New warp_Vertex(size, 0.0F, -size, 4.0F, 4.0F))
            NewObject.addVertex(New warp_Vertex(-size, 0.0F, -size, 0, 4.0F))

            NewObject.addTriangle(0, 3, 2)
            NewObject.addTriangle(0, 2, 1)

            If doubleSided Then
                NewObject.addTriangle(0, 2, 3)
                NewObject.addTriangle(0, 1, 2)
            End If

            Return NewObject
        End Function

        Public Shared Function CUBE(ByVal size As Single) As warp_Object
            Return BOX(size, size, size)
        End Function

        Public Shared Function BOX(ByVal size As warp_Vector) As warp_Object
            Return BOX(size.x, size.y, size.z)
        End Function

        Public Shared Function BOX(ByVal xsize As Single, ByVal ysize As Single, ByVal zsize As Single) As warp_Object
            Dim x As Single = Math.Abs(xsize / 2)
            Dim y As Single = Math.Abs(ysize / 2)
            Dim z As Single = Math.Abs(zsize / 2)

            Dim xx As Single, yy As Single, zz As Single

            Dim n As New warp_Object()
            Dim xflag() As Integer = New Integer(5) {}
            Dim yflag() As Integer = New Integer(5) {}
            Dim zflag() As Integer = New Integer(5) {}

            xflag(0) = 10
            yflag(0) = 3
            zflag(0) = 0
            xflag(1) = 10
            yflag(1) = 15
            zflag(1) = 3
            xflag(2) = 15
            yflag(2) = 3
            zflag(2) = 10
            xflag(3) = 10
            yflag(3) = 0
            zflag(3) = 12
            xflag(4) = 0
            yflag(4) = 3
            zflag(4) = 5
            xflag(5) = 5
            yflag(5) = 3
            zflag(5) = 15

            For side As Integer = 0 To 5
                For i As Integer = 0 To 3
                    If (xflag(side) And (1 << i)) > 0 Then xx = x Else xx = -x
                    If (yflag(side) And (1 << i)) > 0 Then yy = y Else yy = -y
                    If (zflag(side) And (1 << i)) > 0 Then zz = z Else zz = -z
                    n.addVertex(xx, yy, zz, i And 1, (i And 2) >> 1)
                Next
                Dim t As Integer = side << 2
                n.addTriangle(t, t + 2, t + 3)
                n.addTriangle(t, t + 3, t + 1)
            Next

            Return n
        End Function

        Public Shared Function CONE(ByVal height As Single, ByVal radius As Single, ByVal segments As Integer) As warp_Object
            Dim path() As warp_Vector = New warp_Vector(3) {}
            Dim h As Single = height / 2
            path(0) = New warp_Vector(0, h, 0)
            path(1) = New warp_Vector(radius, -h, 0)
            path(2) = New warp_Vector(radius, -h, 0)
            path(3) = New warp_Vector(0, -h, 0)

            Return ROTATIONOBJECT(path, segments)
        End Function

        Public Shared Function CYLINDER(ByVal height As Single, ByVal radius As Single, ByVal segments As Integer) As warp_Object
            Dim path() As warp_Vector = New warp_Vector(5) {}
            Dim h As Single = height / 2
            path(0) = New warp_Vector(0, h, 0)
            path(1) = New warp_Vector(radius, h, 0)
            path(2) = New warp_Vector(radius, h, 0)
            path(3) = New warp_Vector(radius, -h, 0)
            path(4) = New warp_Vector(radius, -h, 0)
            path(5) = New warp_Vector(0, -h, 0)

            Return ROTATIONOBJECT(path, segments)
        End Function

        Public Shared Function SPHERE(ByVal radius As Single, ByVal segments As Integer) As warp_object
            Dim path() As warp_Vector = New warp_Vector(segments) {}

            Dim x As Single, y As Single, angle As Single

            path(0) = New warp_Vector(0, radius, 0)
            path(segments - 1) = New warp_Vector(0, -radius, 0)

            For i As Integer = 1 To segments - 1
                angle = -((CSng(i) / CSng(segments - 2)) - 0.5F) * 3.14159274F
                x = CSng(Math.Cos(angle) * radius)
                y = CSng(Math.Sin(angle) * radius)
                path(i) = New warp_Vector(x, y, 0)
            Next

            Return ROTATIONOBJECT(path, segments)
        End Function

        Public Shared Function ROTATIONOBJECT(ByVal path As warp_Vector(), ByVal sides As Integer) As warp_object
            Dim steps As Integer = sides + 1
            Dim NewObject As New warp_Object()
            Dim alpha As Double = 2 * pi / (steps - 1)
            Dim qx As Single, qz As Single
            Dim nodes As Integer = path.GetLength(0) - 1
            Dim vertex As warp_Vertex = Nothing
            Dim u As Single, v As Single  ' Texture coordinates

            For j As Integer = 0 To steps - 1
                u = CSng((steps - j - 1) / (steps - 1))
                For i As Integer = 0 To nodes - 1
                    v = CSng(i / (nodes - 1))
                    qx = CSng(path(i).x * Math.Cos(j * alpha) + path(i).z * Math.Sin(j * alpha))
                    qz = CSng(path(i).z * Math.Cos(j * alpha) - path(i).x * Math.Sin(j * alpha))
                    vertex = New warp_Vertex(qx, path(i).y, qz)
                    vertex.u = u
                    vertex.v = v
                    NewObject.addVertex(vertex)
                Next
            Next

            For j As Integer = 0 To steps - 2
                For i As Integer = 0 To nodes - 2
                    NewObject.addTriangle(i + nodes * j, i + nodes * (j + 1), i + 1 + nodes * j)
                    NewObject.addTriangle(i + nodes * (j + 1), i + 1 + nodes * (j + 1), i + 1 + nodes * j)
                Next
            Next

            For i As Integer = 0 To nodes - 2
                NewObject.addTriangle(i + nodes * (steps - 1), i, i + 1 + nodes * (steps - 1))
                NewObject.addTriangle(i, i + 1, i + 1 + nodes * (steps - 1))
            Next

            Return NewObject
        End Function


        Public Shared Function TORUSKNOT(ByVal p As Single, ByVal q As Single, ByVal r_tube As Single, ByVal r_out As Single, ByVal r_in As Single, ByVal h As Single, ByVal segments As Integer, ByVal steps As Integer) As warp_Object
            Dim x As Single, y As Single, z As Single, r As Single, t As Single, theta As Single

            Dim path() As warp_Vector = New warp_Vector(segments) {}
            For i As Integer = 0 To segments '+ 1 - 1 Step i + 1
                t = 2 * 3.14159274F * i / segments
                r = r_out + r_in * warp_Math.cos(p * t)
                z = h * warp_Math.sin(p * t)
                theta = q * t
                x = r * warp_Math.cos(theta)
                y = r * warp_Math.sin(theta)
                path(i) = New warp_Vector(x, y, z)
            Next
            Return TUBE(path, r_tube, steps, True)
        End Function

        Public Shared Function SPIRAL(ByVal h As Single, ByVal r_out As Single, ByVal r_in As Single, _
        ByVal r_tube As Single, ByVal w As Single, ByVal f As Single, ByVal segments As Integer, ByVal steps As Integer) As warp_object

            Dim x, y, z, r, t, theta As Single

            Dim path() As warp_Vector = New warp_Vector(segments) {}

            For i As Integer = 0 To segments  '+ 1 - 1 Step i + 1
                t = CSng(i / segments)
                r = r_out + r_in * warp_Math.sin(2 * 3.14159274F * f * t)
                z = (h / 2) + h * t
                theta = 2 * 3.14159274F * w * t
                x = r * warp_Math.cos(theta)
                y = r * warp_Math.sin(theta)
                path(i) = New warp_Vector(x, y, z)
            Next
            Return TUBE(path, r_tube, steps, False)
        End Function

        Public Shared Function TUBE(ByVal path As warp_Vector(), ByVal r As Single, ByVal steps As Integer, ByVal closed As Boolean) As warp_object
            Dim circle() As warp_Vector = New warp_Vector(steps) {}
            Dim angle As Single
            For i As Integer = 0 To steps
                angle = 2 * 3.14159274F * CSng(i) / CSng(steps)
                circle(i) = New warp_Vector(r * warp_Math.cos(angle), r * warp_Math.sin(angle), 0.0F)
            Next

            Dim NewObject As New warp_Object()
            Dim segments As Integer = path.GetLength(0)
            Dim forward As warp_Vector, up As warp_Vector, right As warp_Vector
            Dim frenetmatrix As warp_Matrix
            Dim tempvertex As warp_Vertex
            Dim relx As Single, rely As Single
            Dim a As Integer, b As Integer, c As Integer, d As Integer

            For i As Integer = 0 To segments - 1

                ' Calculate frenet frame matrix

                If i <> (segments - 1) Then
                    forward = warp_Vector.sub(path(i + 1), path(i))
                Else
                    If Not closed Then
                        forward = warp_Vector.sub(path(i), path(i - 1))
                    Else
                        forward = warp_Vector.sub(path(1), path(0))
                    End If
                End If

                forward.normalize()
                up = New warp_Vector(0.0F, 0.0F, 1.0F)
                right = warp_Vector.getNormal(forward, up)
                up = warp_Vector.getNormal(forward, right)
                frenetmatrix = New warp_Matrix(right, up, forward)
                frenetmatrix.shift(path(i).x, path(i).y, path(i).z)

                ' Add nodes

                relx = CSng(i / (segments - 1))
                For k As Integer = 0 To steps
                    rely = CSng(k / steps)
                    tempvertex = New warp_Vertex(circle(k).transform(frenetmatrix))
                    tempvertex.u = relx
                    tempvertex.v = rely
                    NewObject.addVertex(tempvertex)
                Next
            Next

            For i As Integer = 0 To segments - 2
                For k As Integer = 0 To steps
                    a = i * steps + k
                    b = a + 1
                    c = a + steps
                    d = b + steps
                    NewObject.addTriangle(a, c, b)
                    NewObject.addTriangle(b, c, d)
                Next
                a = (i + 1) * steps - 1
                b = a + 1 - steps
                c = a + steps
                d = b + steps
                NewObject.addTriangle(a, c, b)
                NewObject.addTriangle(b, c, d)
            Next

            Return NewObject
        End Function

    End Class
End Namespace
