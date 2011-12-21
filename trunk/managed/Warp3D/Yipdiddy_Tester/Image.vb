' Created by: X
' http://www.createdbyx.com/
' ----------------------
' Thanks to Alan Simes ( http://alansimes.blogdns.net/ ) for creating the Warp3D library from which I learned
' about the code found i the Image class constructor.
' http://alansimes.blogdns.net/cs/files/default.aspx
' ----------------------

Imports System
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Public Class Image
    Implements IDisposable

    Private mintWidth As Integer
    Private mintHeight As Integer

    Private mobjImage As Bitmap = Nothing
    Private mintPixels() As Integer
    Private mobjHandle As GCHandle


    Public ReadOnly Property Width() As Integer
        Get
            Return mintWidth
        End Get
    End Property

    Public ReadOnly Property Height() As Integer
        Get
            Return mintHeight
        End Get
    End Property

    Public Sub New(ByVal w As Integer, ByVal h As Integer)
        mintWidth = w
        mintHeight = h

        mintPixels = New Integer((w * h) - 1) {}

        mobjHandle = GCHandle.Alloc(mintPixels, GCHandleType.Pinned)
        Dim pointer As IntPtr = Marshal.UnsafeAddrOfPinnedArrayElement(mintPixels, 0)

        mobjImage = New Bitmap(w, h, w * 4, PixelFormat.Format32bppPArgb, pointer)
    End Sub

    Public Sub New(ByVal map As Bitmap)
        mintWidth = map.Width
        mintHeight = map.Height

        mintPixels = New Integer((mintWidth * mintHeight) - 1) {}

        'Dim scanline As Integer = bmData.Stride
        'Dim Scan0 As System.IntPtr = bmData.Scan0
        Dim pIndex As Integer = 0
        '	Dim *p As Byte = (Byte *)(void *)Scan0 
        'Dim nOffset As Integer = bmData.Stride - map.Width * 3
        Dim nPixel As Integer = 0
        Dim p((mintWidth * mintHeight * 4) - 1) As Byte

        Dim bmData As BitmapData = map.LockBits(New Rectangle(0, 0, map.Width, map.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
        System.Runtime.InteropServices.Marshal.Copy(bmData.Scan0, p, 0, p.Length)
        map.UnlockBits(bmData)

        For i As Integer = 0 To map.Height - 1 'Step i + 1
            For j As Integer = 0 To map.Width - 1 'Step j + 1
                mintPixels(nPixel) = Color.FromArgb(p(pIndex + 3), p(pIndex + 2), p(pIndex + 1), p(pIndex)).ToArgb
                nPixel += 1

                pIndex += 4
            Next

            ' pIndex += nOffset
        Next

        ' resize()
    End Sub

    Public Sub clear()
        Array.Clear(mintPixels, 0, mintPixels.Length)
    End Sub

    Public Sub clear(ByVal Color As Integer)
        SyncLock mintPixels
            For idx As Integer = 0 To mintPixels.Length - 1
                mintPixels(idx) = Color
            Next
        End SyncLock
    End Sub

    Public Function getImage() As Bitmap
        Return mobjImage
    End Function

    Public Sub draw(ByVal SourceImage As Image, ByVal X As Integer, ByVal Y As Integer)
        If SourceImage Is Nothing Then Exit Sub
        If X > Me.mintWidth - 1 Then Exit Sub
        If Y > Me.mintHeight - 1 Then Exit Sub

        If X + SourceImage.mintWidth < 0 Then Exit Sub
        If Y + SourceImage.mintHeight < 0 Then Exit Sub

        Dim DestIndex As Integer
        Dim SrcIndex As Integer = 0
        Dim StartSourceX As Integer = 0
        Dim EndSourceX As Integer = SourceImage.mintWidth - 1
        Dim StartSourceY As Integer = 0
        ' Dim EndSourceY As Integer = SourceImage.mintHeight - 1

        Dim NewSrcW As Integer
        Dim NewSrcH As Integer
        Dim NewX As Integer = X
        Dim NewY As Integer = Y


        If X < 0 Then
            NewX = 0
            StartSourceX = Math.Abs(X)
            NewSrcW = SourceImage.mintWidth - StartSourceX
        ElseIf X + (SourceImage.mintWidth - 1) > Me.mintWidth - 1 Then
            NewSrcW = (Me.mintWidth - 1) - X ' (X + (SourceImage.mintWidth - 1)) - (Me.mintWidth - 1)
        Else
            NewSrcW = SourceImage.mintWidth
        End If

        If Y < 0 Then
            NewY = 0
            StartSourceY = Math.Abs(Y)
            NewSrcH = SourceImage.mintHeight - StartSourceY
        ElseIf Y + (SourceImage.mintHeight - 1) > Me.mintHeight - 1 Then
            NewSrcH = (Me.mintHeight - 1) - Y '+ (SourceImage.mintHeight - 1))
        Else
            NewSrcH = SourceImage.mintHeight
        End If


        For SourceYPos As Integer = StartSourceY To StartSourceY + (NewSrcH - 1)
            DestIndex = ((NewY + (SourceYPos - StartSourceY)) * Me.mintWidth) + NewX
            SrcIndex = (SourceYPos * SourceImage.mintWidth) + StartSourceX

            Array.Copy(SourceImage.mintPixels, SrcIndex, Me.mintPixels, DestIndex, NewSrcW)
        Next
    End Sub

    'ByVal width As Integer, ByVal height As Integer,
    Public Sub draw(ByVal SourceImage As Image, ByVal posx As Integer, ByVal posy As Integer, ByVal xsize As Integer, ByVal ysize As Integer)
        If SourceImage Is Nothing Then Return

        Dim w As Integer = xsize
        Dim h As Integer = ysize
        Dim xBase As Integer = posx
        Dim yBase As Integer = posy
        Dim tx As Integer = SourceImage.mintWidth * 255
        Dim ty As Integer = SourceImage.mintHeight * 255
        Dim tw As Integer = SourceImage.mintWidth
        Dim dtx As Integer = CInt(tx / w)
        Dim dty As Integer = CInt(ty / h)
        Dim txBase As Integer = Crop(-xBase * dtx, 0, 255 * tx)
        Dim tyBase As Integer = Crop(-yBase * dty, 0, 255 * ty)
        Dim xend As Integer = Crop(xBase + w, 0, mintWidth)
        Dim yend As Integer = Crop(yBase + h, 0, mintHeight)
        Dim offset1 As Integer, offset2 As Integer
        xBase = Crop(xBase, 0, mintWidth)
        yBase = Crop(yBase, 0, mintHeight)

        ty = tyBase
        Dim j As Integer
        For j = yBase To yend - 1 'Step j + 1
            tx = txBase
            offset1 = j * mintWidth
            offset2 = (ty >> 8) * tw
            Dim i As Integer
            For i = xBase To xend - 1 'Step i + 1
                mintPixels(i + offset1) = &HFF000000 Or SourceImage.mintPixels((tx >> 8) + offset2) '&HFF000000 Or SourceImage.pixels((tx >> 8) + offset2)
                tx += dtx
            Next
            ty += dty
        Next
    End Sub

    Public Sub Modulate(ByVal SourceImage As Image, ByVal posx As Integer, ByVal posy As Integer, ByVal xsize As Integer, ByVal ysize As Integer, ByVal Scale As Single)
        If SourceImage Is Nothing Then Return

        Dim w As Integer = xsize
        Dim h As Integer = ysize
        Dim xBase As Integer = posx
        Dim yBase As Integer = posy
        Dim tx As Integer = SourceImage.Width * 255
        Dim ty As Integer = SourceImage.Height * 255
        Dim tw As Integer = SourceImage.Width
        Dim dtx As Integer = tx / w
        Dim dty As Integer = ty / h
        'Dim txBase As Integer = Crop(-xBase * dtx, 0, 255 * tx)
        'Dim tyBase As Integer = Crop(-yBase * dty, 0, 255 * ty)
        'Dim xend As Integer = Crop(xBase + w, 0, mintWidth)
        'Dim yend As Integer = Crop(yBase + h, 0, mintHeight)

        Dim txBase As Integer = -xBase * dtx
        If txBase < 0 Then txBase = 0
        If txBase > 255 * tx Then txBase = 255 * tx
        Dim tyBase As Integer = -yBase * dty
        If tyBase < 0 Then tyBase = 0
        If tyBase > 255 * ty Then tyBase = 255 * ty
        Dim xend As Integer = xBase + w
        If xend < 0 Then xend = 0
        If xend > mintWidth Then xend = mintWidth
        Dim yend As Integer = yBase + h
        If yend < 0 Then yend = 0
        If yend > mintHeight Then yend = mintHeight

        Dim offset1 As Integer, offset2 As Integer
        'xBase = Crop(xBase, 0, Width)
        'yBase = Crop(yBase, 0, Height)

        If xBase < 0 Then xBase = 0
        If xBase > Width Then xBase = Width
        If yBase < 0 Then yBase = 0
        If yBase > Height Then yBase = Height

        ty = tyBase
        Dim j As Integer
        For j = yBase To yend - 1 'Step j + 1
            tx = txBase
            offset1 = j * Width
            offset2 = (ty >> 8) * tw
            Dim i As Integer
            For i = xBase To xend - 1 'Step i + 1
                Dim c1 As Integer = SourceImage.mintPixels((tx >> 8) + offset2)
                Dim c2 As Integer = mintPixels(i + offset1)
                Dim pixel As Integer = (c1 And &HFEFEFF) + (c2 And &HFEFEFF)
                Dim overflow As Integer = pixel And &H1010100
                overflow = overflow - (overflow >> 8)

                Dim col As Integer = &HFF000000 Or overflow Or pixel
                mintPixels(i + offset1) = &HFF000000 Or col 'Modulation(c1, c2, Scale)
                tx += dtx
            Next
            ty += dty
        Next
    End Sub

    'Private Function Modulation(ByVal color1 As Integer, ByVal color2 As Integer, ByVal Scale As Single) As Integer
    '    Dim pixel As Integer = (color1 And &HFEFEFF) + (color2 And &HFEFEFF)
    '    Dim overflow As Integer = pixel And &H1010100
    '    overflow = overflow - (overflow >> 8)

    '    Return &HFF000000 Or overflow Or pixel
    'End Function

    'Public Function Modulation(ByVal color1 As Integer, ByVal color2 As Integer, ByVal Scale As Single) As Integer
    '    Dim R1 As Integer = Color.FromArgb(color1).R
    '    Dim G1 As Integer = Color.FromArgb(color1).G
    '    Dim B1 As Integer = Color.FromArgb(color1).B
    '    Dim A1 As Integer = Color.FromArgb(color1).A
    '    Dim R2 As Integer = Color.FromArgb(color2).R
    '    Dim G2 As Integer = Color.FromArgb(color2).G
    '    Dim B2 As Integer = Color.FromArgb(color2).B
    '    Dim A2 As Integer = Color.FromArgb(color2).A
    '    Dim A, R, G, B As Integer
    '    R = Crop((R1 + R2) * Scale, 0, 255) '* 2
    '    G = Crop((G1 + G2) * Scale, 0, 255) '* 2
    '    B = Crop((B1 + B2) * Scale, 0, 255) '* 2
    '    a = Crop((a1 + a2) * Scale, 0, 255) '* 2
    '    Return Color.FromArgb(A, R, G, B).ToArgb
    'End Function

    'Public Function Modulation(ByVal color1 As Integer, ByVal color2 As Integer, ByVal Scale As Single) As Integer
    '    Dim R1 As Single = (color1 And &HFF0000) >> 16
    '    Dim G1 As Single = (color1 And &HFF00) >> 8
    '    Dim B1 As Single = color1 And &HFF
    '    Dim R2 As Single = (color2 And &HFF0000) >> 16
    '    Dim G2 As Single = (color2 And &HFF00) >> 8
    '    Dim B2 As Single = color2 And &HFF
    '    Dim R, G, B As Single
    '    R = Crop((R1 + R2) * Scale, 0, 255) '* 2
    '    G = Crop((G1 + G2) * Scale, 0, 255) '* 2
    '    B = Crop((B1 + B2) * Scale, 0, 255) '* 2
    '    If R < 0 Then R = 0
    '    If R > 255 Then R = 255
    '    If G < 0 Then G = 0
    '    If G > 255 Then G = 255
    '    If B < 0 Then B = 0
    '    If B > 255 Then B = 255
    '    Return &HFF000000 Or (R << 16) Or (G << 8) Or B

    'End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        If Not mobjImage Is Nothing Then
            mobjHandle.Free()
            mobjImage.Dispose()
            mobjImage = Nothing
            mintPixels = Nothing
        End If
    End Sub
End Class
