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
    Public Class warp_Matrix
        Public m00 As Single = 1, m01 As Single = 0, m02 As Single = 0, m03 As Single = 0
        Public m10 As Single = 0, m11 As Single = 1, m12 As Single = 0, m13 As Single = 0
        Public m20 As Single = 0, m21 As Single = 0, m22 As Single = 1, m23 As Single = 0
        Public m30 As Single = 0, m31 As Single = 0, m32 As Single = 0, m33 As Single = 1

        Public Sub New(ByVal right As warp_Vector, ByVal up As warp_Vector, ByVal forward As warp_Vector)
		     m00 = Right.x
            m10 = right.y
            m20 = right.z
            m01 = up.x
            m11 = up.y
            m21 = up.z
            m02 = forward.x
            m12 = forward.y
            m22 = forward.z
        End Sub

        Default Public ReadOnly Property Item(ByVal column As Integer, ByVal row As Integer) As Single
            Get
                Select Case row
                    Case 0
                        If column = 0 Then Return m00
                        If column = 1 Then Return m01
                        If column = 2 Then Return m02
                        If column = 3 Then Return m03
                        Exit Select
                    Case 1
                        If column = 0 Then Return m11
                        If column = 1 Then Return m11
                        If column = 2 Then Return m12
                        If column = 3 Then Return m13
                        Exit Select
                    Case 2
                        If column = 0 Then Return m20
                        If column = 1 Then Return m21
                        If column = 2 Then Return m22
                        If column = 3 Then Return m23
                        Exit Select
                    Case 3
                        If column = 0 Then Return m30
                        If column = 1 Then Return m31
                        If column = 2 Then Return m32
                        If column = 3 Then Return m33
                        Exit Select

                    Case Else
                        Return 0
                End Select

                Return 0
            End Get
        End Property

        Public Sub New()
            '
            ' TODO: Add constructor logic here
            '
        End Sub

        Public Shared Function shiftMatrix(ByVal dx As Single, ByVal dy As Single, ByVal dz As Single) As warp_Matrix
            ' matrix for shifting
            Dim m As New warp_Matrix
            m.m03 = dx
            m.m13 = dy
            m.m23 = dz
            Return m
        End Function

        Public Shared Function quaternionMatrix(ByVal quat As warp_Quaternion) As warp_Matrix
            Dim m As New warp_Matrix


            Dim xx As Single = quat.X * quat.X
            Dim xy As Single = quat.X * quat.Y
            Dim xz As Single = quat.X * quat.Z
            Dim xw As Single = quat.X * quat.W
            Dim yy As Single = quat.Y * quat.Y
            Dim yz As Single = quat.Y * quat.Z
            Dim yw As Single = quat.Y * quat.W
            Dim zz As Single = quat.Z * quat.Z
            Dim zw As Single = quat.Z * quat.W

            m.m00 = 1 - 2 * (yy + zz)
            m.m01 = 2 * (xy - zw)
            m.m02 = 2 * (xz + yw)
            m.m10 = 2 * (xy + zw)
            m.m11 = 1 - 2 * (xx + zz)
            m.m12 = 2 * (yz - xw)
            m.m20 = 2 * (xz - yw)
            m.m21 = 2 * (yz + xw)
            m.m22 = 1 - 2 * (xx + yy)

            m.m03 = 0
            m.m13 = 0
            m.m23 = 0
            m.m30 = 0
            m.m31 = 0
            m.m32 = 0
            m.m33 = 1

            Return m
        End Function

        Public Function rotateMatrix(ByVal quat As warp_Quaternion) As warp_Matrix
            Reset()

            Dim temp As warp_Matrix = warp_Matrix.quaternionMatrix(quat)
            Dim result As warp_Matrix = warp_Matrix.multiply(Me, temp)

            Return result
        End Function

        Public Shared Function scaleMatrix(ByVal dx As Single, ByVal dy As Single, ByVal dz As Single) As warp_Matrix
            Dim m As warp_Matrix

            m.m00 = dx
            m.m11 = dy
            m.m22 = dz

            Return m
        End Function

        Public Function scaleMatrix(ByVal d As Single) As warp_Matrix
            Return warp_Matrix.scaleMatrix(d, d, d)
        End Function

        Public Shared Function rotateMatrix(ByVal dx As Single, ByVal dy As Single, ByVal dz As Single) As warp_Matrix
            Dim res As New warp_Matrix

            Dim SIN As Single
            Dim COS As Single

            If dx <> 0 Then
                Dim m As New warp_Matrix
                SIN = warp_Math.sin(dx)
                COS = warp_Math.cos(dx)
                m.m11 = COS
                m.m12 = SIN
                m.m21 = -SIN
                m.m22 = COS

                res.transform(m)
            End If
            If dy <> 0 Then
                Dim m As New warp_Matrix

                SIN = warp_Math.sin(dy)
                COS = warp_Math.cos(dy)
                m.m00 = COS
                m.m02 = SIN
                m.m20 = -SIN
                m.m22 = COS

                res.transform(m)
            End If

            If dz <> 0 Then
                Dim m As New warp_Matrix

                SIN = warp_Math.sin(dz)
                COS = warp_Math.cos(dz)
                m.m00 = COS
                m.m01 = SIN
                m.m10 = -SIN
                m.m11 = COS

                res.transform(m)
            End If


            Return res
        End Function


        Public Sub shift(ByVal dx As Single, ByVal dy As Single, ByVal dz As Single)

            transform(shiftMatrix(dx, dy, dz))
        End Sub

        Public Sub scale(ByVal dx As Single, ByVal dy As Single, ByVal dz As Single)
            transform(scaleMatrix(dx, dy, dz))
        End Sub

        Public Sub scale(ByVal d As Single)
            transform(scaleMatrix(d))
        End Sub

        Public Sub rotate(ByVal dx As Single, ByVal dy As Single, ByVal dz As Single)
            transform(rotateMatrix(dx, dy, dz))
        End Sub

        Public Sub rotate(ByVal quat As warp_Quaternion, ByVal x As Single, ByVal y As Single, ByVal z As Single)
            transform(rotateMatrix(quat))
        End Sub

        Public Sub rotate(ByVal m As warp_Matrix)
            transform(m)
        End Sub

        Public Sub scaleSelf(ByVal dx As Single, ByVal dy As Single, ByVal dz As Single)
            preTransform(scaleMatrix(dx, dy, dz))
        End Sub

        Public Sub scaleSelf(ByVal d As Single)
            preTransform(scaleMatrix(d))
        End Sub

        Public Sub shiftSelf(ByVal dx As Single, ByVal dy As Single, ByVal dz As Single)
            preTransform(shiftMatrix(dx, dy, dz))
        End Sub

        Public Sub rotateSelf(ByVal dx As Single, ByVal dy As Single, ByVal dz As Single)
            preTransform(rotateMatrix(dx, dy, dz))
        End Sub
        Public Sub rotateSelf(ByVal m As warp_Matrix)
            preTransform(m)
        End Sub

        Public Sub rotateSelf(ByVal quat As warp_Quaternion)
            preTransform(rotateMatrix(quat))
        End Sub


        Public Sub transform(ByVal n As warp_Matrix)
            Dim m As warp_Matrix = Me.getClone()

            m00 = n.m00 * m.m00 + n.m01 * m.m10 + n.m02 * m.m20
            m01 = n.m00 * m.m01 + n.m01 * m.m11 + n.m02 * m.m21
            m02 = n.m00 * m.m02 + n.m01 * m.m12 + n.m02 * m.m22
            m03 = n.m00 * m.m03 + n.m01 * m.m13 + n.m02 * m.m23 + n.m03
            m10 = n.m10 * m.m00 + n.m11 * m.m10 + n.m12 * m.m20
            m11 = n.m10 * m.m01 + n.m11 * m.m11 + n.m12 * m.m21
            m12 = n.m10 * m.m02 + n.m11 * m.m12 + n.m12 * m.m22
            m13 = n.m10 * m.m03 + n.m11 * m.m13 + n.m12 * m.m23 + n.m13
            m20 = n.m20 * m.m00 + n.m21 * m.m10 + n.m22 * m.m20
            m21 = n.m20 * m.m01 + n.m21 * m.m11 + n.m22 * m.m21
            m22 = n.m20 * m.m02 + n.m21 * m.m12 + n.m22 * m.m22
            m23 = n.m20 * m.m03 + n.m21 * m.m13 + n.m22 * m.m23 + n.m23
        End Sub

        Public Sub preTransform(ByVal n As warp_Matrix)
            Dim m As warp_Matrix = Me.getClone()

            m00 = m.m00 * n.m00 + m.m01 * n.m10 + m.m02 * n.m20
            m01 = m.m00 * n.m01 + m.m01 * n.m11 + m.m02 * n.m21
            m02 = m.m00 * n.m02 + m.m01 * n.m12 + m.m02 * n.m22
            m03 = m.m00 * n.m03 + m.m01 * n.m13 + m.m02 * n.m23 + m.m03
            m10 = m.m10 * n.m00 + m.m11 * n.m10 + m.m12 * n.m20
            m11 = m.m10 * n.m01 + m.m11 * n.m11 + m.m12 * n.m21
            m12 = m.m10 * n.m02 + m.m11 * n.m12 + m.m12 * n.m22
            m13 = m.m10 * n.m03 + m.m11 * n.m13 + m.m12 * n.m23 + m.m13
            m20 = m.m20 * n.m00 + m.m21 * n.m10 + m.m22 * n.m20
            m21 = m.m20 * n.m01 + m.m21 * n.m11 + m.m22 * n.m21
            m22 = m.m20 * n.m02 + m.m21 * n.m12 + m.m22 * n.m22
            m23 = m.m20 * n.m03 + m.m21 * n.m13 + m.m22 * n.m23 + m.m23
        End Sub

        Public Shared Function multiply(ByVal m1 As warp_Matrix, ByVal m2 As warp_Matrix) As warp_Matrix
            Dim m As warp_Matrix = New warp_Matrix()

            m.m00 = m1.m00 * m2.m00 + m1.m01 * m2.m10 + m1.m02 * m2.m20
            m.m01 = m1.m00 * m2.m01 + m1.m01 * m2.m11 + m1.m02 * m2.m21
            m.m02 = m1.m00 * m2.m02 + m1.m01 * m2.m12 + m1.m02 * m2.m22
            m.m03 = m1.m00 * m2.m03 + m1.m01 * m2.m13 + m1.m02 * m2.m23 + m1.m03
            m.m10 = m1.m10 * m2.m00 + m1.m11 * m2.m10 + m1.m12 * m2.m20
            m.m11 = m1.m10 * m2.m01 + m1.m11 * m2.m11 + m1.m12 * m2.m21
            m.m12 = m1.m10 * m2.m02 + m1.m11 * m2.m12 + m1.m12 * m2.m22
            m.m13 = m1.m10 * m2.m03 + m1.m11 * m2.m13 + m1.m12 * m2.m23 + m1.m13
            m.m20 = m1.m20 * m2.m00 + m1.m21 * m2.m10 + m1.m22 * m2.m20
            m.m21 = m1.m20 * m2.m01 + m1.m21 * m2.m11 + m1.m22 * m2.m21
            m.m22 = m1.m20 * m2.m02 + m1.m21 * m2.m12 + m1.m22 * m2.m22
            m.m23 = m1.m20 * m2.m03 + m1.m21 * m2.m13 + m1.m22 * m2.m23 + m1.m23

            Return m
        End Function

        ' 
        ' 		Public String toString()
        ' 		{
        ' 			' todo
        ' 		}
        ' */

        Public Function getClone() As warp_Matrix
            Dim m As warp_Matrix = New warp_Matrix()

            m.m00 = m00 : m.m01 = m01 : m.m02 = m02 : m.m03 = m03
            m.m10 = m10 : m.m11 = m11 : m.m12 = m12 : m.m13 = m13
            m.m20 = m20 : m.m21 = m21 : m.m22 = m22 : m.m23 = m23
            m.m30 = m30 : m.m31 = m31 : m.m32 = m32 : m.m33 = m33

            Return m
        End Function


        Public Function inverse() As warp_Matrix
            Dim m As warp_Matrix = New warp_Matrix()

            Dim q1 As Single = m12, q6 As Single = m10 * m01, q7 As Single = m10 * m21, q8 As Single = m02
            Dim q13 As Single = m20 * m01, q14 As Single = m20 * m11, q21 As Single = m02 * m21, q22 As Single = m03 * m21
            Dim q25 As Single = m01 * m12, q26 As Single = m01 * m13, q27 As Single = m02 * m11, q28 As Single = m03 * m11
            Dim q29 As Single = m10 * m22, q30 As Single = m10 * m23, q31 As Single = m20 * m12, q32 As Single = m20 * m13
            Dim q35 As Single = m00 * m22, q36 As Single = m00 * m23, q37 As Single = m20 * m02, q38 As Single = m20 * m03
            Dim q41 As Single = m00 * m12, q42 As Single = m00 * m13, q43 As Single = m10 * m02, q44 As Single = m10 * m03
            Dim q45 As Single = m00 * m11, q48 As Single = m00 * m21
            Dim q49 As Single = q45 * m22 - q48 * q1 - q6 * m22 + q7 * q8
            Dim q50 As Single = q13 * q1 - q14 * q8
            Dim q51 As Single = 1 / (q49 + q50)

            m.m00 = (m11 * m22 * m33 - m11 * m23 * m32 - m21 * m12 * m33 + m21 * m13 * m32 + m31 * m12 * m23 - m31 * m13 * m22) * q51
            m.m01 = -(m01 * m22 * m33 - m01 * m23 * m32 - q21 * m33 + q22 * m32) * q51
            m.m02 = (q25 * m33 - q26 * m32 - q27 * m33 + q28 * m32) * q51
            m.m03 = -(q25 * m23 - q26 * m22 - q27 * m23 + q28 * m22 + q21 * m13 - q22 * m12) * q51
            m.m10 = -(q29 * m33 - q30 * m32 - q31 * m33 + q32 * m32) * q51
            m.m11 = (q35 * m33 - q36 * m32 - q37 * m33 + q38 * m32) * q51
            m.m12 = -(q41 * m33 - q42 * m32 - q43 * m33 + q44 * m32) * q51
            m.m13 = (q41 * m23 - q42 * m22 - q43 * m23 + q44 * m22 + q37 * m13 - q38 * m12) * q51
            m.m20 = (q7 * m33 - q30 * m31 - q14 * m33 + q32 * m31) * q51
            m.m21 = -(q48 * m33 - q36 * m31 - q13 * m33 + q38 * m31) * q51
            m.m22 = (q45 * m33 - q42 * m31 - q6 * m33 + q44 * m31) * q51
            m.m23 = -(q45 * m23 - q42 * m21 - q6 * m23 + q44 * m21 + q13 * m13 - q38 * m11) * q51

            Return m
        End Function

        Public Sub reset()
            m00 = 1 : m01 = 0 : m02 = 0 : m03 = 0
            m10 = 0 : m11 = 1 : m12 = 0 : m13 = 0
            m20 = 0 : m21 = 0 : m22 = 1 : m23 = 0
            m30 = 0 : m31 = 0 : m32 = 0 : m33 = 1
        End Sub
    End Class
End Namespace
