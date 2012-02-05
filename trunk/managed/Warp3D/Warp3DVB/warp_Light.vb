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
    '/ Summary description for warp_Light.
    '/ </summary>
    Public Class warp_Light
        ' F I E L D S

        Public v As warp_Vector  'Light direction()
        Public v2 As warp_Vector  'projected Light direction 
        Public diffuse As Integer = 0
        Public specular As Integer = 0
        Public highlightSheen As Integer = 0
        Public highlightSpread As Integer = 0

        Dim matrix2 As warp_Matrix

        Private Sub New()
        End Sub

        Public Sub New(ByVal direction As warp_Vector)
            v = direction.getClone()
            v.normalize()
        End Sub

        Public Sub New(ByVal direction As warp_Vector, ByVal diffuse As Integer)
            v = direction.getClone()
            v.normalize()
            Me.diffuse = diffuse
        End Sub

        Public Sub New(ByVal direction As warp_Vector, ByVal color As Integer, ByVal highlightSheen As Integer, ByVal highlightSpread As Integer)
            v = direction.getClone()
            v.normalize()
            Me.diffuse = color
            Me.specular = color
            Me.highlightSheen = highlightSheen
            Me.highlightSpread = highlightSpread
        End Sub

        Public Sub New(ByVal direction As warp_Vector, ByVal diffuse As Integer, ByVal specular As Integer, ByVal highlightSheen As Integer, ByVal highlightSpread As Integer)
            v = direction.getClone()
            v.normalize()
            Me.diffuse = diffuse
            Me.specular = specular
            Me.highlightSheen = highlightSheen
            Me.highlightSpread = highlightSpread
        End Sub

        Public Sub project(ByVal m As warp_Matrix)
            matrix2 = m.getClone()
            matrix2.transform(m)
            v2 = v.transform(matrix2)
        End Sub
    End Class
End Namespace