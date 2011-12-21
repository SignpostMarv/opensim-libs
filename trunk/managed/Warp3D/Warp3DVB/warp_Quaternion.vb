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
Imports System.Text
Imports System.Diagnostics

Namespace Rednettle.Warp3D
    Public Class warp_Quaternion
        Public X As Single
        Public Y As Single
        Public Z As Single
        Public W As Single

        Public Sub New()
        End Sub

        Public Sub New(ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal w As Single)
            Me.X = x
            Me.Y = y
            Me.Z = z
            Me.W = w
        End Sub

        Public Function getClone() As warp_Quaternion
            Return New warp_Quaternion(Me.X, Me.Y, Me.Z, Me.W)
        End Function

        Public Shared Function matrix(ByVal xfrm As warp_Matrix) As warp_Quaternion
            Dim quat As warp_Quaternion = New warp_Quaternion()
            ' Check the sum of the diagonal
            Dim tr As Single = xfrm(0, 0) + xfrm(1, 1) + xfrm(2, 2)
            If tr > 0.0F Then
                ' The sum is positive
                ' 4 muls, 1 div, 6 adds, 1 trig function call
                Dim s As Single = CSng(Math.Sqrt(tr + 1.0F))
                quat.W = s * 0.5F
                s = 0.5F / s
                quat.X = (xfrm(1, 2) - xfrm(2, 1)) * s
                quat.Y = (xfrm(2, 0) - xfrm(0, 2)) * s
                quat.Z = (xfrm(0, 1) - xfrm(1, 0)) * s
            Else
                ' The sum is negative
                ' 4 muls, 1 div, 8 adds, 1 trig function call
                Dim nIndex() As Integer = {1, 2, 0}

                Dim i As Integer, j As Integer, k As Integer
                i = 0
                If xfrm(1, 1) > xfrm(i, i) Then i = 1
                If xfrm(2, 2) > xfrm(i, i) Then i = 2
                j = nIndex(i)
                k = nIndex(j)

                Dim s As Single = CSng(Math.Sqrt((xfrm(i, i) - (xfrm(j, j) + xfrm(k, k))) + 1.0F))
                quat(i) = s * 0.5F
                If s <> 0.0 Then s = 0.5F / s
                quat(j) = (xfrm(i, j) + xfrm(j, i)) * s
                quat(k) = (xfrm(i, k) + xfrm(k, i)) * s
                quat(3) = (xfrm(j, k) - xfrm(k, j)) * s
            End If

            Return quat
        End Function

        Default Public Property Item(ByVal index As Integer) As Single
            Get
                Debug.Assert(0 <= index AndAlso index <= 3)
                If index <= 1 Then
                    If index = 0 Then Return Me.X
                    Return Me.Y
                End If
                If index = 2 Then Return Me.Z
                Return Me.W
            End Get
            Set(ByVal Value As Single)
                Debug.Assert(0 <= index AndAlso index <= 3)
                If index <= 1 Then
                    If index = 0 Then
                        Me.X = Value
                    Else
                        Me.Y = Value
                    End If
                Else
                    If index = 2 Then
                        Me.Z = Value
                    Else
                        Me.W = Value
                    End If
                End If
            End Set
        End Property
    End Class
End Namespace
