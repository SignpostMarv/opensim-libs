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
    '/ Summary description for warp_TextureSettings.
    '/ </summary>
    '/ 
    Public Class warp_TextureSettings
        Public texture As warp_Texture
        Public width As Integer
        Public height As Integer
        Public type As Integer
        Public persistency As Single
        Public density As Single
        Public samples As Integer
        Public numColors As Integer
        Public colors() As Integer

        Public Sub New(ByVal tex As warp_Texture, ByVal w As Integer, ByVal h As Integer, ByVal t As Integer, ByVal p As Single, ByVal d As Single, ByVal s As Integer, ByVal c() As Integer)
            texture = tex
            width = w
            height = h
            type = t
            persistency = p
            density = d
            samples = s
            colors = c
        End Sub
    End Class
End Namespace
