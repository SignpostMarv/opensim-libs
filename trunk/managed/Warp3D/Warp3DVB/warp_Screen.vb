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
Imports System.Runtime.InteropServices

Namespace Rednettle.Warp3D
    Public Class warp_Screen
        Implements IDisposable

        Public width As Integer
        Public height As Integer

        ' benchmark stuff
        Private timestamp As DateTime
        Private time As DateTime
        Private probes As Integer = 32
        Public FPS As Single = 0

        Dim image As Bitmap = Nothing
        Public pixels() As Integer
        Private handle As GCHandle

        Public Sub New(ByVal w As Integer, ByVal h As Integer)
            width = w
            height = h

            pixels = New Integer(w * h) {}   

            handle = GCHandle.Alloc(pixels, GCHandleType.Pinned)
            Dim pointer As IntPtr = Marshal.UnsafeAddrOfPinnedArrayElement(pixels, 0)

            image = New Bitmap(w, h, w * 4, PixelFormat.Format32bppPArgb, pointer)
        End Sub

        Public Sub clear(ByVal c As Integer)
            warp_Math.clearBuffer(pixels, &HFF000000 Or c)
        End Sub

        Public Sub draw(ByVal texture As warp_Texture, ByVal posx As Integer, ByVal posy As Integer, ByVal xsize As Integer, ByVal ysize As Integer)
            draw(width, height, texture, posx, posy, xsize, ysize)
        End Sub

        Public Sub drawBackground(ByVal texture As warp_Texture, ByVal posx As Integer, ByVal posy As Integer, ByVal xsize As Integer, ByVal ysize As Integer)
            draw(width, height, texture, posx, posy, xsize, ysize)
        End Sub

        Public Function getImage() As Bitmap
            Return image
        End Function

        Private Sub draw(ByVal width As Integer, ByVal height As Integer, ByVal texture As warp_Texture, ByVal posx As Integer, ByVal posy As Integer, ByVal xsize As Integer, ByVal ysize As Integer)
            If texture Is Nothing Then Return

            Dim w As Integer = xsize
            Dim h As Integer = ysize
            Dim xBase As Integer = posx
            Dim yBase As Integer = posy
            Dim tx As Integer = texture.width * 255
            Dim ty As Integer = texture.height * 255
            Dim tw As Integer = texture.width
            Dim dtx As Integer = CInt(tx / w)
            Dim dty As Integer = CInt(ty / h)
            Dim txBase As Integer = warp_Math.crop(-xBase * dtx, 0, 255 * tx)
            Dim tyBase As Integer = warp_Math.crop(-yBase * dty, 0, 255 * ty)
            Dim xend As Integer = warp_Math.crop(xBase + w, 0, width)
            Dim yend As Integer = warp_Math.crop(yBase + h, 0, height)
            Dim offset1 As Integer, offset2 As Integer
            xBase = warp_Math.crop(xBase, 0, width)
            yBase = warp_Math.crop(yBase, 0, height)

            ty = tyBase
            Dim j As Integer
            For j = yBase To yend - 1 'Step j + 1
                tx = txBase
                offset1 = j * width
                offset2 = (ty >> 8) * tw
                Dim i As Integer
                For i = xBase To xend - 1 'Step i + 1
                    pixels(i + offset1) = &HFF000000 Or texture.pixel((tx >> 8) + offset2)
                    tx += dtx
                Next
                ty += dty
            Next
        End Sub

        Public Sub add(ByVal texture As warp_Texture, ByVal posx As Integer, ByVal posy As Integer, ByVal xsize As Integer, ByVal ysize As Integer)
            add(width, height, texture, posx, posy, xsize, ysize)
        End Sub

        Private Sub add(ByVal width As Integer, ByVal height As Integer, ByVal texture As warp_Texture, ByVal posx As Integer, ByVal posy As Integer, ByVal xsize As Integer, ByVal ysize As Integer)
            If texture Is Nothing Then Return

            Dim w As Integer = xsize
            Dim h As Integer = ysize
            Dim xBase As Integer = posx
            Dim yBase As Integer = posy
            Dim tx As Integer = texture.width * 255
            Dim ty As Integer = texture.height * 255
            Dim tw As Integer = texture.width
            Dim dtx As Integer = CInt(tx / w)
            Dim dty As Integer = CInt(ty / h)
            Dim txBase As Integer = warp_Math.crop(-xBase * dtx, 0, 255 * tx)
            Dim tyBase As Integer = warp_Math.crop(-yBase * dty, 0, 255 * ty)
            Dim xend As Integer = warp_Math.crop(xBase + w, 0, width)
            Dim yend As Integer = warp_Math.crop(yBase + h, 0, height)
            Dim pos As Integer, offset1 As Integer, offset2 As Integer
            xBase = warp_Math.crop(xBase, 0, width)
            yBase = warp_Math.crop(yBase, 0, height)

            ty = tyBase
            Dim j As Integer
            For j = yBase To yend - 1 'Step j + 1
                tx = txBase
                offset1 = j * width
                offset2 = (ty >> 8) * tw
                Dim i As Integer
                For i = xBase To xend - 1 'Step i + 1
                    pixels(i + offset1) = &HFF000000 Or warp_Color.add(texture.pixel((tx >> 8) + offset2), pixels(i + offset1))
                    tx += dtx
                Next
                ty += dty
            Next
        End Sub

        Private Sub performBench()
            probes += 1
            If probes > 32 Then
                time = DateTime.Now
                Dim span As TimeSpan = time - timestamp

                FPS = 32.0F / (span.Milliseconds / 1000.0F)

                timestamp = time
                probes = 0
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            If Not image Is Nothing Then
                handle.Free()
                image.Dispose()
                image = Nothing
                pixels = Nothing
            End If
        End Sub

    End Class
End Namespace
