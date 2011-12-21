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
    Partial Public Class warp_Screen
       
        Public Sub Subtract(ByVal texture As warp_Texture, ByVal posx As Integer, ByVal posy As Integer, ByVal xsize As Integer, ByVal ysize As Integer)
            Subtract(width, height, texture, posx, posy, xsize, ysize)
        End Sub

        Private Sub Subtract(ByVal width As Integer, ByVal height As Integer, ByVal texture As warp_Texture, ByVal posx As Integer, ByVal posy As Integer, ByVal xsize As Integer, ByVal ysize As Integer)
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
                    pixels(i + offset1) = &HFF000000 Or warp_Color.sub(texture.pixel((tx >> 8) + offset2), pixels(i + offset1))
                    tx += dtx
                Next
                ty += dty
            Next
        End Sub

        Public Sub Mix(ByVal texture As warp_Texture, ByVal posx As Integer, ByVal posy As Integer, ByVal xsize As Integer, ByVal ysize As Integer)
            Mix(width, height, texture, posx, posy, xsize, ysize)
        End Sub

        Private Sub Mix(ByVal width As Integer, ByVal height As Integer, ByVal texture As warp_Texture, ByVal posx As Integer, ByVal posy As Integer, ByVal xsize As Integer, ByVal ysize As Integer)
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
                    pixels(i + offset1) = &HFF000000 Or warp_Color.mix(texture.pixel((tx >> 8) + offset2), pixels(i + offset1))
                    tx += dtx
                Next
                ty += dty
            Next
        End Sub

        Public Sub Multiply(ByVal texture As warp_Texture, ByVal posx As Integer, ByVal posy As Integer, ByVal xsize As Integer, ByVal ysize As Integer)
            multiply(width, height, texture, posx, posy, xsize, ysize)
        End Sub

        Private Sub Multiply(ByVal width As Integer, ByVal height As Integer, ByVal texture As warp_Texture, ByVal posx As Integer, ByVal posy As Integer, ByVal xsize As Integer, ByVal ysize As Integer)
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
                    pixels(i + offset1) = &HFF000000 Or warp_Color.multiply(texture.pixel((tx >> 8) + offset2), pixels(i + offset1))
                    tx += dtx
                Next
                ty += dty
            Next
        End Sub

        Public Sub SubNegative(ByVal texture As warp_Texture, ByVal posx As Integer, ByVal posy As Integer, ByVal xsize As Integer, ByVal ysize As Integer)
            SubNegative(width, height, texture, posx, posy, xsize, ysize)
        End Sub

        Private Sub SubNegative(ByVal width As Integer, ByVal height As Integer, ByVal texture As warp_Texture, ByVal posx As Integer, ByVal posy As Integer, ByVal xsize As Integer, ByVal ysize As Integer)
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
                    pixels(i + offset1) = &HFF000000 Or warp_Color.subneg(texture.pixel((tx >> 8) + offset2), pixels(i + offset1))
                    tx += dtx
                Next
                ty += dty
            Next
        End Sub

        Public Sub Invert(ByVal texture As warp_Texture, ByVal posx As Integer, ByVal posy As Integer, ByVal xsize As Integer, ByVal ysize As Integer)
            Invert(width, height, texture, posx, posy, xsize, ysize)
        End Sub

        Private Sub Invert(ByVal width As Integer, ByVal height As Integer, ByVal texture As warp_Texture, ByVal posx As Integer, ByVal posy As Integer, ByVal xsize As Integer, ByVal ysize As Integer)
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
                    pixels(i + offset1) = &HFF000000 Or warp_Color.inv(pixels(i + offset1)) ',    texture.pixel((tx >> 8) + offset2)
                    tx += dtx
                Next
                ty += dty
            Next
        End Sub

        Public Sub Modulate(ByVal texture As warp_Texture, ByVal posx As Integer, ByVal posy As Integer, ByVal xsize As Integer, ByVal ysize As Integer)
            Modulate(width, height, texture, posx, posy, xsize, ysize)
        End Sub

        Private Sub Modulate(ByVal width As Integer, ByVal height As Integer, ByVal texture As warp_Texture, ByVal posx As Integer, ByVal posy As Integer, ByVal xsize As Integer, ByVal ysize As Integer)
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
                    pixels(i + offset1) = &HFF000000 Or warp_Color.Modulate(texture.pixel((tx >> 8) + offset2), pixels(i + offset1))
                    tx += dtx
                Next
                ty += dty
            Next
        End Sub
    End Class
End Namespace
