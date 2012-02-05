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
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Net

Namespace Rednettle.Warp3D
    '/ <summary>
    '/ Summary description for warp_Texture.
    '/ </summary>

    Public Class warp_Texture
        Public width As Integer
        Public height As Integer
        Public bitWidth As Integer
        Public bitHeight As Integer
        Public pixel() As Integer
        Public path As String = Nothing

        Public Shared ALPHA As Integer = &HFF000000  ' alpha mask

        Public Sub New(ByVal w As Integer, ByVal h As Integer)
            height = h
            width = w
            pixel = New Integer(w * h) {}
            cls()
        End Sub

        Public Sub New(ByVal w As Integer, ByVal h As Integer, ByVal data() As Integer)
            height = h
            width = w
            pixel = New Integer(width * height) {}

            System.Array.Copy(data, pixel, pixel.Length)
        End Sub

        Public Sub New(ByVal path As String)
            Dim map As Bitmap

            If path.StartsWith("http") Then
                Dim webrq As WebRequest = WebRequest.Create(path)
                map = CType(Bitmap.FromStream(webrq.GetResponse().GetResponseStream()), Bitmap)
            Else
                map = New Bitmap(path, False)
            End If

            loadTexture(map)
        End Sub


        Public Sub resize()
            Dim log2inv As Double = 1 / Math.Log(2)

            '    int w = ( int )Math.Pow( 2, bitWidth = ( int )Math.Ceiling( ( Math.Log( width ) * log2inv ) ) );
            '   int h = ( int )Math.Pow( 2, bitHeight = ( int )Math.Ceiling( ( Math.Log( height ) * log2inv ) ) );
            bitWidth = CInt(Math.Ceiling(Math.Log(width) * log2inv))
            bitHeight = CInt(Math.Ceiling(Math.Log(height) * log2inv))
            Dim w As Integer = CInt(Math.Pow(2, bitWidth))
            Dim h As Integer = CInt(Math.Pow(2, bitHeight))

            If Not (w = width AndAlso h = height) Then resize(w, h)
        End Sub


        Public Sub resize(ByVal w As Integer, ByVal h As Integer)
            '   System.Console.WriteLine("warp_Texture| resize :" + w + "," + h)
            setSize(w, h)
        End Sub

        Public Sub cls()
            warp_Math.clearBuffer(pixel, 0)
        End Sub

        Public Function toAverage() As warp_Texture
            Dim i As Integer
            For i = width * height - 1 To 0 Step -1
                pixel(i) = warp_Color.getAverage(pixel(i))
            Next

            Return Me
        End Function

        Public Function toGray() As warp_Texture
            Dim i As Integer
            For i = width * height - 1 To 0 Step -1
                pixel(i) = warp_Color.getGray(pixel(i))
            Next

            Return Me
        End Function

        Public Function valToGray() As warp_Texture
            Dim intensity As Integer
            Dim i As Integer
            For i = width * height - 1 To 0 Step -1
                intensity = warp_Math.crop(pixel(i), 0, 255)
                pixel(i) = warp_Color.getColor(intensity, intensity, intensity)
            Next

            Return Me
        End Function

        Public Function colorize(ByVal pal() As Integer) As warp_Texture
            Dim range As Integer = pal.GetLength(0) - 1
            Dim i As Integer
            For i = width * height - 1 To 0 Step -1
                pixel(i) = pal(warp_Math.crop(pixel(i), 0, range))
            Next

            Return Me
        End Function

        Private Sub loadTexture(ByVal map As Bitmap)
            width = map.Width
            height = map.Height

            pixel = New Integer((width * height) - 1) {}

            'Dim scanline As Integer = bmData.Stride
            'Dim Scan0 As System.IntPtr = bmData.Scan0
            Dim pIndex As Integer = 0
            '	Dim *p As Byte = (Byte *)(void *)Scan0 
            'Dim nOffset As Integer = bmData.Stride - map.Width * 3
            Dim nPixel As Integer = 0
            Dim p((width * height * 3) - 1) As Byte

            Dim bmData As BitmapData = map.LockBits(New Rectangle(0, 0, map.Width, map.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb)
            System.Runtime.InteropServices.Marshal.Copy(bmData.Scan0, p, 0, p.Length)
            map.UnlockBits(bmData)

            For i As Integer = 0 To map.Height - 1 'Step i + 1
                For j As Integer = 0 To map.Width - 1 'Step j + 1
                    ' Dim Col As Color
                    ' Col = map.GetPixel(j, i)
                    Dim blue As Integer = p(pIndex)   'Col.B 
                    Dim green As Integer = p(pIndex + 1)  'Col.G
                    Dim red As Integer = p(pIndex + 2)        'Col.R 
                    pixel(nPixel) = ALPHA Or red << 16 Or green << 8 Or blue
                    nPixel += 1

                    pIndex += 3
                Next

                ' pIndex += nOffset
            Next

            resize()
        End Sub

        'Private Sub loadTexture(ByVal map As Bitmap)
        '    width = map.Width
        '    height = map.Height

        '    pixel = New Integer((width * height) - 1) {}

        '    Dim bmData As BitmapData = map.LockBits(New Rectangle(0, 0, map.Width, map.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb)
        '    Dim scanline As Integer = bmData.Stride
        '    Dim Scan0 As System.IntPtr = bmData.Scan0
        '    Dim pIndex As Integer = 0
        '    '	Dim *p As Byte = (Byte *)(void *)Scan0 
        '    Dim nOffset As Integer = bmData.Stride - map.Width * 3
        '    Dim nPixel As Integer = 0
        '    Dim p(width * height * 3) As Byte

        '    System.Runtime.InteropServices.Marshal.Copy(Scan0, p, nOffset, p.Length)

        '    For i As Integer = 0 To map.Height - 1 'Step i + 1
        '        For j As Integer = 0 To map.Width - 1 'Step j + 1
        '            Dim blue As Integer = p(pIndex)
        '            Dim green As Integer = p(pIndex + 1)
        '            Dim red As Integer = p(pIndex + 2)
        '            pixel(nPixel) = ALPHA Or red << 16 Or green << 8 Or blue
        '            nPixel += 1

        '            pIndex += 3
        '        Next

        '        pIndex += nOffset
        '    Next

        '    map.UnlockBits(bmData)
        '    resize()
        'End Sub
 
        Private Sub setSize(ByVal w As Integer, ByVal h As Integer)
            Dim offset As Integer = w * h
            Dim offset2 As Integer
            If w * h <> 0 Then
                Dim Newpixels() As Integer = New Integer(w * h) {}
                Dim j As Integer
                For j = h - 1 To 0 Step -1
                    offset -= w
                    offset2 = CInt((j * height / h) * width)
                    Dim i As Integer
                    For i = w - 1 To 0 Step -1
                        Newpixels(i + offset) = pixel(CInt((i * width / w) + offset2))
                    Next
                Next

                width = w
                height = h
                pixel = Newpixels
            End If
        End Sub

        Private Function inrange(ByVal a As Integer, ByVal b As Integer, ByVal c As Integer) As Boolean
            Return (a >= b) And (a < c)
        End Function

        Public Function getClone() As warp_Texture
            Dim t As warp_Texture = New warp_Texture(width, height)
            warp_Math.copyBuffer(pixel, t.pixel)
            Return t
        End Function

    End Class
End Namespace