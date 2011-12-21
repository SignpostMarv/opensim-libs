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
    '/ Summary description for warp_Color.
    '/ </summary>
    Partial Public Class warp_Color

        Public Shared Function Modulate(ByVal color1 As Integer, ByVal color2 As Integer) As Integer
            Dim R1 As Integer = (color1 And RED) >> 16
            Dim G1 As Integer = (color1 And GREEN) >> 8
            Dim B1 As Integer = color1 And BLUE
            Dim R2 As Integer = (color2 And RED) >> 16
            Dim G2 As Integer = (color2 And GREEN) >> 8
            Dim B2 As Integer = color2 And BLUE
            Dim R, G, B As Integer
            R = (R1 * R2) '* 2
            G = (G1 * G2) '* 2
            B = (B1 * B2) '* 2
            Return ALPHA Or (R << 16) Or (G << 8) Or B

        End Function

    End Class
End Namespace
