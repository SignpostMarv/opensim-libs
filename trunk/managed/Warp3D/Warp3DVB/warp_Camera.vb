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
    '/ Summary description for warp_Camera.
    '/ </summary>
    '/ 
    Public Class warp_Camera
        Public matrix As New warp_Matrix '= New warp_Matrix()
        Public normalmatrix As New warp_Matrix '= New warp_Matrix()

        Dim needsRebuild As Boolean = True  ' Flag indicating changes on matrix

        ' Camera settings
        Public pos As warp_Vector = New warp_Vector(0.0F, 0.0F, 0.0F)
        Public lookat As warp_Vector = New warp_Vector(0.0F, 0.0F, 0.0F)
        Public rollfactor As Single = 0.0F

        Public fovfact As Single
        Public screenwidth As Integer
        Public screenheight As Integer
        Public screenscale As Integer

        Public Sub New()
            setFov(90.0F)
        End Sub

        Public Sub New(ByVal fov As Single)
            setFov(fov)
        End Sub

        Public Function getMatrix() As warp_Matrix
            rebuildMatrices()
            Return matrix
        End Function

        Public Function getNormalMatrix() As warp_Matrix
            rebuildMatrices()
            Return normalmatrix
        End Function

        Private Sub rebuildMatrices()
            If Not needsRebuild Then Exit Sub
            needsRebuild = False

            Dim forward As warp_Vector, up As warp_Vector, right As warp_Vector

            forward = warp_Vector.sub(lookat, pos)
            up = New warp_Vector(0.0F, 1.0F, 0.0F)
            right = warp_Vector.getNormal(up, forward)
            up = warp_Vector.getNormal(forward, right)

            forward.normalize()
            up.normalize()
            right.normalize()

            normalmatrix = New warp_Matrix(right, up, forward)
            normalmatrix.rotate(0, 0, rollfactor)
            matrix = normalmatrix.getClone()
            matrix.shift(pos.x, pos.y, pos.z)

            normalmatrix = normalmatrix.inverse()
            matrix = matrix.inverse()
        End Sub

        Public Sub setFov(ByVal fov As Single)
            fovfact = CSng(Math.Tan(warp_Math.deg2rad(fov) / 2))
        End Sub

        Public Sub roll(ByVal angle As Single)
            rollfactor += angle
            needsRebuild = True
        End Sub

        Public Sub setPos(ByVal px As Single, ByVal py As Single, ByVal pz As Single)
            pos = New warp_Vector(px, py, pz)
            needsRebuild = True
        End Sub

        Public Sub setPos(ByVal p As warp_Vector)
            pos = p
            needsRebuild = True
        End Sub

        Public Sub setlookAt(ByVal px As Single, ByVal py As Single, ByVal pz As Single)
            lookat = New warp_Vector(px, py, pz)
            needsRebuild = True
        End Sub

        Public Sub setlookAt(ByVal p As warp_Vector)
            lookat = p
            needsRebuild = True
        End Sub

        Public Sub setScreensize(ByVal w As Integer, ByVal h As Integer)
            screenwidth = w
            screenheight = h
            If w < h Then screenscale = w Else screenscale = h
        End Sub

        Public Sub shift(ByVal dx As Single, ByVal dy As Single, ByVal dz As Single)
            pos = pos.transform(warp_Matrix.shiftMatrix(dx, dy, dz))
            lookat = lookat.transform(warp_Matrix.shiftMatrix(dx, dy, dz))
            needsRebuild = True
        End Sub

        Public Sub shift(ByVal v As warp_Vector)
            shift(v.x, v.y, v.z)
        End Sub

        Public Sub rotate(ByVal dx As Single, ByVal dy As Single, ByVal dz As Single)
            pos = pos.transform(warp_Matrix.rotateMatrix(dx, dy, dz))
            needsRebuild = True
        End Sub

        Public Sub rotate(ByVal v As warp_Vector)
            rotate(v.x, v.y, v.z)
        End Sub

        Public Shared Function FRONT() As warp_Camera
            Dim cam As New warp_Camera ' = New warp_Camera()
            cam.setPos(0, 0, -2.0F)
            Return cam
        End Function

        Public Shared Function LEFT() As warp_Camera
            Dim cam As New warp_Camera '= New warp_Camera()
            cam.setPos(2.0F, 0, 0)
            Return cam
        End Function

        Public Shared Function RIGHT() As warp_Camera
            Dim cam As New warp_Camera '= New warp_Camera()
            cam.setPos(-2.0F, 0, 0)
            Return cam
        End Function

        Public Shared Function TOP() As warp_Camera
            Dim cam As New warp_Camera '= New warp_Camera()
            cam.setPos(0, -2.0F, 0)
            Return cam
        End Function
    End Class
End Namespace
