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
    Public Class warp_TextureFactory

        Public Const pi As Single = 3.14159274F
        Public Const deg2rad As Single = pi / 180
        Private Shared noiseBuffer(,) As Single
        Private Shared noiseBufferInitialized As Boolean = False
        Shared minx, maxx, miny, maxy As Integer
        Public Const ALPHA As Integer = &HFF000000 ' // alpha mask

        Private Sub New()

        End Sub

        Public Shared Function SKY(ByVal w As Integer, ByVal h As Integer, ByVal density As Single) As warp_Texture
            Dim colors() As Integer = New Integer(1) {}
            colors(0) = &H3399
            colors(1) = &HFFFFFF
            Return PERLIN(w, h, 0.5F, 2.8F * density, 8, 1024).colorize(warp_Color.makeGradient(colors, 1024))
        End Function

        Public Shared Function MARBLE(ByVal w As Integer, ByVal h As Integer, ByVal density As Single) As warp_Texture
            Dim colors() As Integer = New Integer(2) {}
            colors(0) = &H111111
            colors(1) = &H696070
            colors(2) = &HFFFFFF
            Return WAVE(w, h, 0.5F, 0.64F * density, 6, 1024).colorize(warp_Color.makeGradient(colors, 1024))
        End Function

        Public Shared Function WOOD(ByVal w As Integer, ByVal h As Integer, ByVal density As Single) As warp_Texture
            Dim colors() As Integer = New Integer(2) {}
            colors(0) = &H332211
            colors(1) = &H523121
            colors(2) = &H996633

            Return GRAIN(w, h, 0.5F, 3.0F * density, 3, 8, 1024).colorize(warp_Color.makeGradient(colors, 1024))
        End Function

        Public Shared Function PERLIN(ByVal w As Integer, ByVal h As Integer, ByVal persistency As Single, ByVal density As Single, ByVal samples As Integer, ByVal scale As Integer) As warp_Texture
            initNoiseBuffer()
            Dim t As warp_Texture = New warp_Texture(w, h)
            Dim pos As Integer = 0
            Dim wavelength As Single = CSng(IIf(w > h, w, h)) / density

            Dim y As Integer
            For y = 0 To h - 1 'Step y + 1
                Dim x As Integer
                For x = 0 To w - 1 ' Step  x + 1
                    t.pixel(pos) = CInt(scale * perlin2d(x, y, wavelength, persistency, samples))
                    pos += 1
                Next
            Next
            Return t
        End Function


        Public Shared Function WAVE(ByVal w As Integer, ByVal h As Integer, ByVal persistency As Single, ByVal density As Single, ByVal samples As Integer, ByVal scale As Integer) As warp_Texture
            initnoisebuffer()
            Dim t As warp_Texture = New warp_Texture(w, h)
            Dim pos As Integer = 0
            Dim wavelength As Single
            If w > h Then wavelength = w / density Else wavelength = h / density

            For y As Integer = 0 To h - 1 'Step y + 1
                For x As Integer = 0 To w - 1  'Step  x + 1
                    t.pixel(pos) = CInt(CDbl(scale) * Math.Sin(32 * perlin2d(x, y, wavelength, persistency, samples)) * 0.5 + 0.5)
                    pos += 1
                Next
            Next
            Return t
        End Function


        Public Shared Function GRAIN(ByVal w As Integer, ByVal h As Integer, ByVal persistency As Single, ByVal density As Single, ByVal samples As Integer, ByVal levels As Integer, ByVal scale As Integer) As warp_Texture
            ' TIP: For wooden textures
            initNoiseBuffer()
            Dim t As warp_Texture = New warp_Texture(w, h)
            Dim pos As Integer = 0
            Dim wavelength As Single
            If w > h Then wavelength = w / density Else wavelength = h / density
            Dim perlin As Single

            For y As Integer = 0 To h - 1 'Step y + 1
                For x As Integer = 0 To w - 1 'Step x + 1
                    perlin = CSng(levels) * perlin2d(x, y, wavelength, persistency, samples)
                    t.pixel(pos) = CInt(CSng(scale) * (perlin - CSng(CInt(perlin))))
                    pos += 1
                Next
            Next
            Return t
        End Function

        '		// Perlin noise functions

        Private Shared Function perlin2d(ByVal x As Single, ByVal y As Single, ByVal wavelength As Single, ByVal persistence As Single, ByVal samples As Integer) As Single
            Dim sum As Single = 0
            Dim freq As Single = 1.0F / wavelength
            Dim amp As Single = persistence
            Dim range As Single = 0

            For i As Integer = 0 To samples - 1
                sum += amp * interpolatedNoise(x * freq, y * freq, i)
                range += amp
                amp *= persistence
                freq *= 2
            Next
            Return warp_Math.crop(sum / persistence * 0.5F + 0.5F, 0, 1)
        End Function

        '// Helper methods

        Private Shared Function interpolatedNoise(ByVal x As Single, ByVal y As Single, ByVal octave As Integer) As Single
            Dim intx As Integer = CInt(x)
            Dim inty As Integer = CInt(y)
            Dim fracx As Single = x - CSng(intx)
            Dim fracy As Single = y - CSng(inty)

            Dim i1 As Single = warp_Math.interpolate(noise(intx, inty, octave), noise(intx + 1, inty, octave), fracx)
            Dim i2 As Single = warp_Math.interpolate(noise(intx, inty + 1, octave), noise(intx + 1, inty + 1, octave), fracx)

            Return warp_Math.interpolate(i1, i2, fracy)
        End Function

        Private Shared Function smoothNoise(ByVal x As Integer, ByVal y As Integer, ByVal o As Integer) As Single
            Return (noise(x - 1, y - 1, o) + noise(x + 1, y - 1, o) + _
             noise(x - 1, y + 1, o) + noise(x + 1, y + 1, o)) / 16 + _
             (noise(x - 1, y, o) + noise(x + 1, y, o) + noise(x, y - 1, o) + _
             noise(x, y + 1, o)) / 8 + noise(x, y, o) / 4
        End Function

        Private Shared Function noise(ByVal x As Integer, ByVal y As Integer, ByVal octave As Integer) As Single
            Return noiseBuffer(octave And 3, (x + y * 57) And 8191)
        End Function

        Private Shared Function noise(ByVal seed As Integer, ByVal octave As Integer) As Single
            Dim id As Integer = octave And 3
            Dim n As Integer = CInt((seed << 13) ^ seed)

            If id = 0 Then
                Return (1.0F - _
                 ((n * (n * n * 15731 + 789221) + 1376312589) And _
                 &H7FFFFFFF) * _
                 9.313226E-10F)
            End If
            If id = 1 Then
                Return (1.0F - _
                 ((n * (n * n * 12497 + 604727) + 1345679039) And _
                 &H7FFFFFFF) * _
                 9.313226E-10F)
            End If
            If id = 2 Then
                Return (1.0F - _
                 ((n * (n * n * 19087 + 659047) + 1345679627) And _
                 &H7FFFFFFF) * _
                 9.313226E-10F)
            End If
            Return (1.0F - _
             ((n * (n * n * 16267 + 694541) + 1345679501) And _
             &H7FFFFFFF) * _
             9.313226E-10F)
        End Function

        Private Shared Sub initNoiseBuffer()
            If noiseBufferInitialized Then Return

            noiseBuffer = New Single(3, 8191) {}
            Dim octave As Integer
            For octave = 0 To 3 '4 - 1 'Step octave + 1
                Dim i As Integer
                For i = 0 To 8191 ' - 1 'Step i + 1
                    noiseBuffer(octave, i) = noise(i, octave)
                Next
            Next
            noiseBufferInitialized = True
        End Sub

        Public Shared Function CHECKERBOARD(ByVal w As Integer, ByVal h As Integer, ByVal cellbits As Integer, ByVal oddColor As Integer, ByVal evenColor As Integer) As warp_Texture
            Dim t As New warp_Texture(w, h)

            Dim pos As Integer = 0
            For y As Integer = 0 To h - 1
                For x As Integer = 0 To w - 1
                    Dim c As Integer = CInt(IIf((((x >> cellbits) + (y >> cellbits)) And 1) = 0, evenColor, oddColor))
                    t.pixel(pos) = c
                    pos += 1
                Next
            Next

            Return t
        End Function
    End Class
End Namespace