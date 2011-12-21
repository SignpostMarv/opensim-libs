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
    Public Class warp_Color
        Public Shared ALPHA As Integer = &HFF000000  ' alpha mask
        Public Shared RED As Integer = &HFF0000  ' red mask
        Public Shared GREEN As Integer = &HFF00  ' green mask
        Public Shared BLUE As Integer = &HFF  ' blue mask
        Public Shared MASK7Bit As Integer = &HFEFEFF  ' mask for additive/subtractive shading
        Public Shared MASK6Bit As Integer = &HFCFCFC  ' mask for additive/subtractive shading
        Public Shared RGB As Integer = &HFFFFFF  ' rgb mask

        Private Shared pixel As Integer, color As Integer, overflow As Integer, colorscale As Integer, r As Integer, g As Integer, b As Integer

        Private Sub New()
        End Sub

        Public Shared Function random(ByVal color As Integer, ByVal delta As Integer) As Integer
            Dim rnd As Random = New Random()

            Dim r As Integer = (color >> 16) And 255
            Dim g As Integer = (color >> 8) And 255
            Dim b As Integer = color And 255

            r = CInt(r + (rnd.NextDouble() * delta))
            g = CInt(g + (rnd.NextDouble() * delta))
            b = CInt(b + (rnd.NextDouble() * delta))

            Return getCropColor(r, g, b)
        End Function

        Public Shared Function random() As Integer
            Dim rnd As Random = New Random()
            Return CInt((rnd.NextDouble() * 16777216))
        End Function

        Public Shared Function getRed(ByVal c As Integer) As Integer
            Return (c And RED) >> 16
        End Function

        Public Shared Function getGreen(ByVal c As Integer) As Integer
            Return (c And GREEN) >> 8
        End Function

        Public Shared Function getBlue(ByVal c As Integer) As Integer
            Return c And BLUE
        End Function

        Public Shared Function makeGradient(ByVal colors() As Integer, ByVal size As Integer) As Integer()
            Dim pal(size - 1) As Integer ' = New Integer(size) {}
            Dim c1 As Integer, c2 As Integer, pos1 As Integer, pos2 As Integer, range As Integer
            Dim r As Integer, g As Integer, b As Integer, r1 As Integer, g1 As Integer, b1 As Integer, r2 As Integer, g2 As Integer, b2 As Integer, dr As Integer, dg As Integer, db As Integer
            If colors.GetLength(0) = 1 Then
                c1 = colors(0)

                For i As Integer = 0 To size - 1 'Step i + 1
                    pal(i) = c1
                Next
                Return pal
            End If


            For c As Integer = 0 To colors.GetLength(0) - 2 ' Step c + 1
                c1 = colors(c)
                c2 = colors(c + 1)
                pos1 = size * c \ (colors.GetLength(0) - 1)
                pos2 = size * (c + 1) \ (colors.GetLength(0) - 1)
                range = pos2 - pos1
                r1 = warp_Color.getRed(c1) << 16
                g1 = warp_Color.getGreen(c1) << 16
                b1 = warp_Color.getBlue(c1) << 16
                r2 = warp_Color.getRed(c2) << 16
                g2 = warp_Color.getGreen(c2) << 16
                b2 = warp_Color.getBlue(c2) << 16
                dr = (r2 - r1) \ range
                dg = (g2 - g1) \ range
                db = (b2 - b1) \ range
                r = r1
                g = g1
                b = b1

                For i As Integer = pos1 To pos2 - 1 'Step i + 1
                    pal(i) = getColor(r >> 16, g >> 16, b >> 16)
                    r += dr
                    g += dg
                    b += db
                Next
            Next

            Return pal
        End Function

        Public Shared Function add(ByVal color1 As Integer, ByVal color2 As Integer) As Integer
            pixel = (color1 And MASK7Bit) + (color2 And MASK7Bit)
            overflow = pixel And &H1010100
            overflow = overflow - (overflow >> 8)

            Return ALPHA Or overflow Or pixel
        End Function

        ' TOTO: Make sure using xor works properly xor replaces C# ~ operator
        Public Shared Function [sub](ByVal color1 As Integer, ByVal color2 As Integer) As Integer
            pixel = (color1 And MASK7Bit) + ((color2 Xor color2) And MASK7Bit)
            overflow = (pixel Xor pixel) And &H1010100
            overflow = overflow - (overflow >> 8)
            Return ALPHA Or ((overflow Xor overflow) And pixel)
        End Function

        Public Shared Function subneg(ByVal color1 As Integer, ByVal color2 As Integer) As Integer
            pixel = (color1 And MASK7Bit) + (color2 And MASK7Bit)
            overflow = (pixel Xor pixel) And &H1010100
            overflow = overflow - (overflow >> 8)
            Return ALPHA Or ((overflow Xor overflow) And pixel)
        End Function

        Public Shared Function inv(ByVal color As Integer) As Integer
            Return ALPHA Or (color Xor color)
        End Function

        Public Shared Function mix(ByVal color1 As Integer, ByVal color2 As Integer) As Integer
            ' Returns the averidge color from 2 colors
            Return ALPHA Or (((color1 And MASK7Bit) >> 1) + ((color2 And MASK7Bit) >> 1))
        End Function

        Public Shared Function scale(ByVal color As Integer, ByVal factor As Integer) As Integer
            If factor = 0 Then Return 0
            If factor = 255 Then Return color
            If factor = 127 Then Return (color And &HFEFEFE) >> 1

            r = (((color >> 16) And 255) * factor) >> 8
            g = (((color >> 8) And 255) * factor) >> 8
            b = ((color And 255) * factor) >> 8
            Return ALPHA Or (r << 16) Or (g << 8) Or b
        End Function

        Public Shared Function multiply(ByVal color1 As Integer, ByVal color2 As Integer) As Integer
            If (color1 And RGB) = 0 Then Return 0
            If (color2 And RGB) = 0 Then Return 0
            r = (((color1 >> 16) And 255) * ((color2 >> 16) And 255)) >> 8
            g = (((color1 >> 8) And 255) * ((color2 >> 8) And 255)) >> 8
            b = ((color1 And 255) * (color2 And 255)) >> 8
            Return ALPHA Or (r << 16) Or (g << 8) Or b
        End Function

        Public Shared Function transparency(ByVal bkgrd As Integer, ByVal color As Integer, ByVal alpha As Integer) As Integer
            If alpha = 0 Then Return color
             If alpha = 255 Then Return bkgrd
            If alpha = 127 Then Return mix(bkgrd, color)

            r = (alpha * (((bkgrd >> 16) And 255) - ((color >> 16) And 255)) >> 8) + ((color >> 16) And 255)
            g = (alpha * (((bkgrd >> 8) And 255) - ((color >> 8) And 255)) >> 8) + ((color >> 8) And 255)
            b = (alpha * ((bkgrd And 255) - (color And 255)) >> 8) + (color And 255)

            Return alpha Or (r << 16) Or (g << 8) Or b
        End Function

        Public Shared Function getCropColor(ByVal r As Integer, ByVal g As Integer, ByVal b As Integer) As Integer
            Return ALPHA Or (warp_Math.crop(r, 0, 255) << 16) Or (warp_Math.crop(g, 0, 255) << 8) Or warp_Math.crop(b, 0, 255)
        End Function


        Public Shared Function getColor(ByVal r As Integer, ByVal g As Integer, ByVal b As Integer) As Integer
            Return ALPHA Or (r << 16) Or (g << 8) Or b
        End Function

        Public Shared Function getGray(ByVal color As Integer) As Integer
            Dim r As Integer = ((color And RED) >> 16)
            Dim g As Integer = ((color And GREEN) >> 8)
            Dim b As Integer = (color And BLUE)
            Dim Y As Integer = CInt((r * 3 + g * 6 + b) / 10)
            Return ALPHA Or (Y << 16) Or (Y << 8) Or Y
        End Function

        Public Shared Function getAverage(ByVal color As Integer) As Integer
            Return (((color And RED) >> 16) + ((color And GREEN) >> 8) + (color And BLUE)) \ 3
        End Function
    End Class
End Namespace
