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
Imports System.Collections.Generic
Imports System.Text

Namespace Rednettle.Warp3D
    Public Class warp_FXLensFlare
        Inherits warp_FXPlugin
        Public flareObject As warp_Object

        Private flares As Integer = 0
        Private zBufferSensitive As Boolean = True
        Private flare() As warp_Texture
        Private flareDist() As Single

        Private Sub New(ByVal scene As warp_Scene)
            MyBase.New(scene)
        End Sub

        Public Sub New(ByVal name As String, ByVal scene As warp_Scene, ByVal zBufferSensitive As Boolean)
            MyBase.New(scene)
            Me.zBufferSensitive = zBufferSensitive

            flareObject = New warp_Object()
            flareObject.addVertex(New warp_Vertex(1.0F, 1.0F, 1.0F))
            flareObject.rebuild()

            scene.addObject(name, flareObject)
        End Sub

        Public Sub preset1()
            clear()

            addGlow(144, &H330099)
            addStrike(320, 48, &H3366)
            addStrike(48, 240, &H660033)
            addRing(120, &H660000)
            addRays(320, 32, 20, &H111111)
            addSecs(12, 100, 64, &H6633CC, 64)
        End Sub

        Public Sub preset2()
            clear()
            addGlow(144, &H995500)
            addStrike(640, 64, &HCC0000)
            addStrike(32, 480, &HFF)
            addStrike(64, 329, &H330099)
            addRing(160, &H990000)
            addRays(320, 24, 32, &H332211)
            addSecs(12, 100, 64, &H333333, 100)
            addSecs(12, 60, 40, &H336699, 80)
        End Sub

        Public Sub preset3()
            clear()
            addGlow(144, &H993322)
            addStrike(400, 200, &HCC00FF)
            addStrike(480, 32, &HFF)
            addRing(180, &H990000)
            addRays(240, 32, 48, &H332200)
            addSecs(12, 80, 64, &H555555, 50)
            addSecs(12, 60, 40, &H336699, 80)
        End Sub

        Public Sub setPos(ByVal pos As warp_Vector)
            flareObject.fastvertex(0).pos = pos
        End Sub

        Public Sub clear()
            flares = 0
            flare = Nothing
            flareDist = Nothing
        End Sub

        Public Sub addGlow(ByVal size As Integer, ByVal color As Integer)
            addFlare(createGlow(size, size, color, 256), 0.0F)
        End Sub

        Public Sub addStrike(ByVal width As Integer, ByVal height As Integer, ByVal color As Integer)
            addFlare(createGlow(width, height, color, 48), 0.0F)
        End Sub

        Public Sub addRing(ByVal size As Integer, ByVal color As Integer)
            addFlare(createRing(size, color), 0.0F)
        End Sub

        Public Sub addRays(ByVal size As Integer, ByVal num As Integer, ByVal rad As Integer, ByVal color As Integer)
            addFlare(createRays(size, num, rad, color), 0.0F)
        End Sub

        Public Sub addSecs(ByVal count As Integer, ByVal averidgeSize As Integer, ByVal sizeDelta As Integer, ByVal averidgeColor As Integer, ByVal colorDelta As Integer)
            For i As Integer = 0 To count - 1 'Step i + 1
                addFlare(createSec(averidgeSize, sizeDelta, averidgeColor, colorDelta), warp_Math.random(-0.5F, 3.0F))
            Next
        End Sub


        Public Overrides Sub apply()
            Dim px As Integer = flareObject.fastvertex(0).x
            Dim py As Integer = flareObject.fastvertex(0).y

            If Not flareObject.fastvertex(0).visible Then Exit Sub
            If zBufferSensitive AndAlso (flareObject.fastvertex(0).z > scene.renderPipeline.zBuffer(px + py * screen.width)) Then Exit Sub

            Dim cx As Integer = screen.width \ 2
            Dim cy As Integer = screen.height \ 2

            Dim dx As Single = (cx - px)
            Dim dy As Single = (cy - py)
            Dim posx As Integer, posy As Integer, xsize As Integer, ysize As Integer
            Dim zoom As Single

            Dim i As Integer
            For i = 0 To flares - 1 'Step i + 1
                zoom = warp_Math.pythagoras(dx, dy) / warp_Math.pythagoras(cx, cy)
                zoom = (1 - zoom) / 2 + 1
                xsize = flare(i).width
                ysize = flare(i).height
                posx = CInt(px + (dx * flareDist(i)))
                posy = CInt(py + (dy * flareDist(i)))

                screen.add(flare(i), posx - xsize \ 2, posy - ysize \ 2, xsize, ysize)
            Next
        End Sub

        Private Sub addFlare(ByVal texture As warp_Texture, ByVal relPos As Single)
            flares = flares + 1

            If flares = 1 Then
                flare = New warp_Texture(1) {}
                flareDist = New Single(1) {}
            Else
                Dim temp1() As warp_Texture = New warp_Texture(flares - 1) {}
                System.Array.Copy(flare, 0, temp1, 0, flares - 1)
                flare = temp1

                Dim temp2() As Single = New Single(flares - 1) {}
                System.Array.Copy(flareDist, 0, temp2, 0, flares - 1)
                flareDist = temp2
            End If

            flare(flares - 1) = texture
            flareDist(flares - 1) = relPos
        End Sub

        Private Function createRadialTexture(ByVal w As Integer, ByVal h As Integer, ByVal colormap() As Integer, ByVal alphamap() As Integer) As warp_Texture
            Dim offset As Integer
            Dim relX As Single, relY As Single
            Dim NewTexture As warp_Texture = New warp_Texture(w, h)
            Dim palette() As Integer = getPalette(colormap, alphamap)

             For y As Integer = h - 1 To 0 Step -1
                offset = y * w

                For x As Integer = w - 1 To 0 Step -1
                    relX = CSng((x - (w >> 1)) / (w >> 1))
                    relY = CSng((y - (h >> 1)) / (h >> 1))
                    NewTexture.pixel(offset + x) = palette(warp_Math.crop(CInt(255 * Math.Sqrt(relX * relX + relY * relY)), 0, 255))
                Next
            Next

            Return NewTexture
        End Function

        Private Function getPalette(ByVal color() As Integer, ByVal alpha() As Integer) As Integer()
            Dim r As Integer, g As Integer, b As Integer
            Dim palette(255) As Integer '= New Integer(255) {}

            For i As Integer = 255 To 0 Step -1
                r = (((color(i) >> 16) And 255) * alpha(i)) >> 8
                g = (((color(i) >> 8) And 255) * alpha(i)) >> 8
                b = ((color(i) And 255) * alpha(i)) >> 8
                palette(i) = warp_Color.getColor(r, g, b)
            Next
            Return palette
        End Function

        Private Function createGlow(ByVal w As Integer, ByVal h As Integer, ByVal color As Integer, ByVal alpha As Integer) As warp_Texture
            Return createRadialTexture(w, h, getGlowPalette(color), getConstantAlpha(alpha))
        End Function

        Private Function createRing(ByVal size As Integer, ByVal color As Integer) As warp_Texture
            Return createRadialTexture(size, size, getColorPalette(color, color), getRingAlpha(40))
        End Function

        Private Function createSec(ByVal size As Integer, ByVal sizedelta As Integer, ByVal color As Integer, ByVal colordelta As Integer) As warp_Texture
            Dim s As Integer = CInt(warp_Math.randomWithDelta(size, sizedelta))
            Dim c1 As Integer = warp_Color.random(color, colordelta)
            Dim c2 As Integer = warp_Color.random(color, colordelta)
            Return createRadialTexture(s, s, getColorPalette(c1, c2), getSecAlpha())
        End Function

        Private Function createRays(ByVal size As Integer, ByVal rays As Integer, ByVal rad As Integer, ByVal color As Integer) As warp_Texture
            Dim pos As Integer
            Dim relPos As Single
            Dim texture As warp_Texture = New warp_Texture(size, size)
            Dim radialMap(1023) As Integer '= New Integer(1023) {}
            warp_Math.clearBuffer(radialMap, 0)


            For i As Integer = 0 To rays - 1 'Step i + 1
                pos = CInt(warp_Math.random(rad, 1023 - rad))

                For k As Integer = pos - rad To pos + rad 'Step k + 1
                    relPos = (k - pos + rad) / (rad * 2.0F)
                    radialMap(k) = CInt(radialMap(k) + (255 * (1 + Math.Sin((relPos - 0.25) * 3.14159 * 2)) / 2))
                Next
            Next

            Dim angle As Integer, offset As Integer, reldist As Integer
            Dim xrel As Single, yrel As Single
            Dim y As Integer
            For y = size - 1 To 0 Step -1
                offset = y * size
                Dim x As Integer
                For x = size - 1 To 0 Step -1
                    xrel = (2.0F * x - size) / size
                    yrel = (2.0F * y - size) / size
                    angle = CInt(1023 * Math.Atan2(xrel, yrel) / 3.14159 / 2) And 1023
                    reldist = CInt(Math.Max((255 - 255 * warp_Math.pythagoras(xrel, yrel)), 0))
                    texture.pixel(x + offset) = warp_Color.scale(color, radialMap(angle) * reldist \ 255)
                Next
            Next

            Return texture
        End Function

        Private Function getGlowPalette(ByVal color As Integer) As Integer()
            Dim r As Integer, g As Integer, b As Integer
            Dim relDist As Single, diffuse As Single, specular As Single
            Dim palette(255) As Integer '= New Integer(256) {}
            Dim cr As Integer = (color >> 16) And 255
            Dim cg As Integer = (color >> 8) And 255
            Dim cb As Integer = color And 255
            Dim i As Integer
            For i = 255 To 0 Step -1
                relDist = i / 255.0F
                diffuse = CSng(Math.Cos(relDist * 1.57))
                specular = CSng((255 / Math.Pow(2.718, relDist * 2.718) - i / 16))
                r = CInt((cr * diffuse + specular))
                g = CInt((cg * diffuse + specular))
                b = CInt((cb * diffuse + specular))
                palette(i) = warp_Color.getCropColor(r, g, b)
            Next

            Return palette
        End Function

        Private Function getConstantAlpha(ByVal alpha As Integer) As Integer()
            Dim alphaPalette(255) As Integer '= New Integer(256) {}

            For i As Integer = 255 To 0 Step -1
                alphaPalette(i) = alpha
            Next
            Return alphaPalette
        End Function

        Private Function getLinearAlpha() As Integer()
            Dim alphaPalette(255) As Integer '= New Integer(256) {}

            For i As Integer = 255 To 0 Step -1
                alphaPalette(i) = 255 - i
            Next

            Return alphaPalette
        End Function

        Private Function getRingAlpha(ByVal ringsize As Integer) As Integer()
            Dim alphaPalette(255) As Integer ' = New Integer(256) {}
            Dim angle As Single

            'For i As Integer = 0 To 256 - 1 ' Step i + 1
            '    alphaPalette(i) = 0
            'Next

            For i As Integer = 0 To ringsize - 1 'Step i + 1
                angle = 3.14159F / 180 * (180.0F * i / ringsize)
                alphaPalette(255 - ringsize + i) = CInt((64 * Math.Sin(angle)))
            Next
            Return alphaPalette
        End Function

        Private Function getSecAlpha() As Integer()
            Dim alphaPalette() As Integer = getRingAlpha(CInt(warp_Math.random(0, 255)))

            For i As Integer = 0 To 255 '6 - 1 Step i + 1
                alphaPalette(i) = (alphaPalette(i) + 255 - i) >> 2
            Next

            Return alphaPalette
        End Function


        Private Function getColorPalette(ByVal color1 As Integer, ByVal color2 As Integer) As Integer()
            Dim palette() As Integer = New Integer(256) {}

            Dim r1 As Integer = (color1 >> 16) And 255
            Dim g1 As Integer = (color1 >> 8) And 255
            Dim b1 As Integer = color1 And 255
            Dim r2 As Integer = (color2 >> 16) And 255
            Dim g2 As Integer = (color2 >> 8) And 255
            Dim b2 As Integer = color2 And 255
            Dim dr As Integer = r2 - r1
            Dim dg As Integer = g2 - g1
            Dim db As Integer = b2 - b1
            Dim r As Integer = r1 << 8
            Dim g As Integer = g1 << 8
            Dim b As Integer = b1 << 8


            For i As Integer = 0 To 255 '6 - 1 Step i + 1
                palette(i) = warp_Color.getColor(r >> 8, g >> 8, b >> 8)
                r += dr
                g += dg
                b += db
            Next
            Return palette
        End Function
    End Class
End Namespace