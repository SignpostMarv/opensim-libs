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
    Public Class warp_Rasterizer
        Private materialLoaded As Boolean = False
        Private lightmapLoaded As Boolean = False
        Public ready As Boolean = False

        ' Current material settings
        Private color As Integer = 0
        Private currentColor As Integer = 0
        Private transparency As Integer = 0
        Private reflectivity As Integer = 0
        Private refraction As Integer = 0
        Private texture As warp_Texture = Nothing
        Private envmap() As Integer = Nothing
        Private diffuse() As Integer = Nothing
        Private specular() As Integer = Nothing
        Private refractionMap() As Short = Nothing
        Private tw As Integer = 0
        Private th As Integer = 0
        Private tbitW As Integer = 0
        Private tbitH As Integer = 0

        ' Rasterizer hints
        Private mode As Integer = 0
        Private F As Integer = 0  ' FLAT
        Private W As Integer = 1  ' WIREFRAME
        Private P As Integer = 2  ' PHONG
        Private E As Integer = 4  ' ENVMAP
        Private T As Integer = 8  ' TEXTURED
        Private SHADED As Integer = 0

        Dim p1 As warp_Vertex, p2 As warp_Vertex, p3 As warp_Vertex, tempVertex As warp_Vertex

        Private bkgrd, c, s, lutID, r, _
  x1, x2, x3, x4, y1, y2, y3, z1, z2, z3, z4, _
  x, y, z, k, dx, dy, dz, offset, pos, temp, _
  xL, xR, xBase, zBase, xMax, yMax, dxL, dxR, dzBase, _
                                                   nx1, nx2, nx3, nx4, ny1, ny2, ny3, ny4, _
  nxBase, nyBase, _
  dnx4, dny4, _
  dnx, dny, nx, ny, _
  dnxBase, dnyBase, _
  dtx4, dty4 As Integer

        Private tx1, tx2, tx3, tx4, ty1, ty2, ty3, ty4, _
  txBase, tyBase, _
  tx, ty, sw, _
  dtxBase, dtyBase, _
  dtx, dty, _
  sw1, sw2, sw3, sw4, swBase, dsw, dswBase As Single

        Dim screen As warp_Screen
        Dim zBuffer() As Integer
        Dim idBuffer() As Integer
        Dim width As Integer, height As Integer
        Dim useIdBuffer As Boolean
        Dim zFar As Integer = &HFFFFFF
        Dim currentId As Integer = 0

        '// Constructor
        Public Sub New(ByVal pipeline As warp_RenderPipeline)
            SHADED = P Or E Or T

            rebuildReferences(pipeline)
            loadLightmap(pipeline.lightmap)
        End Sub

        ' References
        Public Sub rebuildReferences(ByVal pipeline As warp_RenderPipeline)
            screen = pipeline.screen
            zBuffer = pipeline.zBuffer
            idBuffer = pipeline.idBuffer
            width = screen.width
            height = screen.height
            useIdBuffer = pipeline.useId
        End Sub

        ' Lightmap loader
        Public Sub loadLightmap(ByVal lm As warp_Lightmap)
            If lm Is Nothing Then Exit Sub
            diffuse = lm.diffuse
            specular = lm.specular
            lightmapLoaded = True
            ready = lightmapLoaded AndAlso materialLoaded
        End Sub

        ' Material loader
        Public Sub loadMaterial(ByVal material As warp_Material)
            color = material.color
            transparency = material.transparency
            reflectivity = material.reflectivity
            texture = material.texture
            If material.envmap IsNot Nothing Then
                envmap = material.envmap.pixel
            Else
                envmap = Nothing
            End If

            If texture IsNot Nothing Then
                tw = texture.width - 1
                th = texture.height - 1
                tbitW = texture.bitWidth
                tbitH = texture.bitHeight
            End If

            mode = 0
            If Not material.flat Then mode = mode Or P
            If envmap IsNot Nothing Then mode = mode Or E
            If texture IsNot Nothing Then mode = mode Or T
            If material.wireframe Then mode = mode Or W
            materialLoaded = True
            ready = lightmapLoaded AndAlso materialLoaded
        End Sub


        Public Sub render(ByVal tri As warp_Triangle)
            If Not ready Then
                Return
            End If
            If tri.parent Is Nothing Then
                Return
            End If
            If (mode And W) <> 0 Then
                drawWireframe(tri, color)
                If (mode And W) = 0 Then
                    Return
                End If
            End If

            p1 = tri.p1
            p2 = tri.p2
            p3 = tri.p3

            If p1.y > p2.y Then tempVertex = p1 : p1 = p2 : p2 = tempVertex
            If p2.y > p3.y Then tempVertex = p2 : p2 = p3 : p3 = tempVertex
            If p1.y > p2.y Then tempVertex = p1 : p1 = p2 : p2 = tempVertex

            If p1.y >= height Then Exit Sub
            If p3.y < 0 Then Exit Sub
            If p1.y = p3.y Then Exit Sub

            If mode = F Then
                lutID = CInt(tri.n2.x * 127 + 127) + (CInt(tri.n2.y * 127 + 127) << 8)
                c = warp_Color.multiply(color, diffuse(lutID))
                s = warp_Color.scale(specular(lutID), reflectivity)
                currentColor = warp_Color.add(c, s)
            End If

            currentId = (tri.parent.id << 16) Or tri.id

            x1 = p1.x << 8
            x2 = p2.x << 8
            x3 = p3.x << 8
            y1 = p1.y
            y2 = p2.y
            y3 = p3.y

            x4 = CInt(x1 + (x3 - x1) * (y2 - y1) / (y3 - y1))
            x1 <<= 8
            x2 <<= 8
            x3 <<= 8
            x4 <<= 8

            z1 = p1.z
            z2 = p2.z
            z3 = p3.z
            nx1 = p1.nx << 16
            nx2 = p2.nx << 16
            nx3 = p3.nx << 16
            ny1 = p1.ny << 16
            ny2 = p2.ny << 16
            ny3 = p3.ny << 16

            sw1 = 1.0F / p1.sw
            sw2 = 1.0F / p2.sw
            sw3 = 1.0F / p3.sw

            tx1 = p1.tx * sw1
            tx2 = p2.tx * sw2
            tx3 = p3.tx * sw3
            ty1 = p1.ty * sw1
            ty2 = p2.ty * sw2
            ty3 = p3.ty * sw3

            dx = (x4 - x2) >> 16
            If dx = 0 Then Exit Sub

            temp = CInt(256 * (y2 - y1) / (y3 - y1))

            z4 = z1 + ((z3 - z1) >> 8) * temp
            nx4 = nx1 + ((nx3 - nx1) >> 8) * temp
            ny4 = ny1 + ((ny3 - ny1) >> 8) * temp

            Dim tf As Single = CSng((y2 - y1) / (y3 - y1))

            tx4 = tx1 + ((tx3 - tx1) * tf)
            ty4 = ty1 + ((ty3 - ty1) * tf)

            sw4 = sw1 + ((sw3 - sw1) * tf)

            dz = CInt((z4 - z2) / dx)
            dnx = CInt((nx4 - nx2) / dx)
            dny = CInt((ny4 - ny2) / dx)
            dtx = (tx4 - tx2) / dx
            dty = (ty4 - ty2) / dx
            dsw = (sw4 - sw2) / dx

            If (dx < 0) Then
                temp = x2
                x2 = x4
                x4 = temp
                z2 = z4
                tx2 = tx4
                ty2 = ty4
                sw2 = sw4
                nx2 = nx4
                ny2 = ny4
            End If
            If (y2 >= 0) Then
                dy = y2 - y1
                If dy <> 0 Then
                    dxL = CInt((x2 - x1) / dy)
                    dxR = CInt((x4 - x1) / dy)
                    dzBase = CInt((z2 - z1) / dy)
                    dnxBase = CInt((nx2 - nx1) / dy)
                    dnyBase = CInt((ny2 - ny1) / dy)
                    dtxBase = (tx2 - tx1) / dy
                    dtyBase = (ty2 - ty1) / dy
                    dswBase = (sw2 - sw1) / dy
                End If

                xBase = x1
                xMax = x1
                zBase = z1
                nxBase = nx1
                nyBase = ny1
                txBase = tx1
                tyBase = ty1
                swBase = sw1

                If (y1 < 0) Then
                    xBase -= y1 * dxL
                    xMax -= y1 * dxR
                    zBase -= y1 * dzBase
                    nxBase -= y1 * dnxBase
                    nyBase -= y1 * dnyBase
                    txBase -= y1 * dtxBase
                    tyBase -= y1 * dtyBase
                    swBase -= y1 * dswBase
                    y1 = 0
                End If

                y2 = CInt(IIf(y2 < height, y2, height))
                offset = y1 * width
                For y = y1 To y2 - 1 'Step y + 1
                    renderLine()
                Next
            End If

            If (y2 < height) Then
                dy = y3 - y2
                If dy <> 0 Then
                    dxL = CInt((x3 - x2) / dy)
                    dxR = CInt((x3 - x4) / dy)
                    dzBase = CInt((z3 - z2) / dy)
                    dnxBase = CInt((nx3 - nx2) / dy)
                    dnyBase = CInt((ny3 - ny2) / dy)
                    dtxBase = (tx3 - tx2) / dy
                    dtyBase = (ty3 - ty2) / dy
                    dswBase = (sw3 - sw2) / dy
                End If

                xBase = x2
                xMax = x4
                zBase = z2
                nxBase = nx2
                nyBase = ny2
                txBase = tx2
                tyBase = ty2
                swBase = sw2

                If (y2 < 0) Then
                    xBase -= y2 * dxL
                    xMax -= y2 * dxR
                    zBase -= y2 * dzBase
                    nxBase -= y2 * dnxBase
                    nyBase -= y2 * dnyBase
                    txBase -= y2 * dtxBase
                    tyBase -= y2 * dtyBase
                    swBase -= y2 * dswBase
                    y2 = 0
                End If

                y3 = CInt(IIf(y3 < height, y3, height))
                offset = y2 * width

                For y = y2 To y3 - 1 'Step y + 1
                    renderLine()
                Next
            End If
        End Sub


        Private Sub renderLine()
            xL = xBase >> 16
            xR = xMax >> 16
            z = zBase
            nx = nxBase
            ny = nyBase
            tx = txBase
            ty = tyBase
            sw = swBase

            If (xL < 0) Then
                z -= xL * dz
                nx -= xL * dnx
                ny -= xL * dny
                tx -= xL * dtx
                ty -= xL * dty
                sw -= xL * dsw
                xL = 0
            End If
            xR = CInt(IIf(xR < width, xR, width))

            If mode = F Then
                renderLineF()
            ElseIf (mode And SHADED) = P Then
                renderLineP()
            ElseIf (mode And SHADED) = E Then
                renderLineE()
            ElseIf (mode And SHADED) = T Then
                renderLineT()
            ElseIf (mode And SHADED) = (P Or E) Then
                renderLinePE()
            ElseIf (mode And SHADED) = (P Or T) Then
                renderLinePT()
            ElseIf (mode And SHADED) = (P Or E Or T) Then
                renderLinePET()
            End If

            offset += width
            xBase += dxL
            xMax += dxR
            zBase += dzBase
            nxBase += dnxBase
            nyBase += dnyBase
            txBase += dtxBase
            tyBase += dtyBase
            swBase += dswBase
        End Sub

         ' Fast scanline rendering
        Private Sub renderLineF()
            For x = xL To xR - 1 'Step x + 1
                pos = x + offset
                If z < zBuffer(pos) Then
                    bkgrd = screen.pixels(pos)
                    c = warp_Color.transparency(bkgrd, currentColor, transparency)
                    screen.pixels(pos) = c
                    zBuffer(pos) = z
                    If useIdBuffer Then idBuffer(pos) = currentId
                End If
                z += dz
            Next
        End Sub

        Private Sub renderLineP()
            For x = xL To xR - 1 'Step x + 1
                pos = x + offset
                If z < zBuffer(pos) Then
                    lutID = ((nx >> 16) And 255) + (((ny >> 16) And 255) << 8)
                    bkgrd = screen.pixels(pos)
                    c = warp_Color.multiply(color, diffuse(lutID))
                    s = specular(lutID)
                    s = warp_Color.scale(s, reflectivity)
                    c = warp_Color.transparency(bkgrd, c, transparency)
                    c = warp_Color.add(c, s)
                    screen.pixels(pos) = c
                    zBuffer(pos) = z

                    If useIdBuffer Then idBuffer(pos) = currentId
                End If
                z += dz
                nx += dnx
                ny += dny
            Next

        End Sub

        Private Sub renderLineE()
            For x = xL To xR - 1 'Step x + 1
                pos = x + offset
                If z < zBuffer(pos) Then
                    lutID = ((nx >> 16) And 255) + (((ny >> 16) And 255) << 8)
                    bkgrd = screen.pixels(pos)
                    s = warp_Color.add(specular(lutID), envmap(lutID))
                    s = warp_Color.scale(s, reflectivity)
                    c = warp_Color.transparency(bkgrd, s, transparency)
                    screen.pixels(pos) = c
                    zBuffer(pos) = z

                    If useIdBuffer Then idBuffer(pos) = currentId
                End If
                z += dz
                nx += dnx
                ny += dny
            Next
        End Sub

        Private Sub renderLineT()
            For x = xL To xR - 1 'Step x + 1
                pos = x + offset
                If z < zBuffer(pos) Then
                    bkgrd = screen.pixels(pos)
                    c = texture.pixel(((CInt(tx / sw)) And tw) + (((CInt(ty / sw)) And th) << tbitW))
                    c = warp_Color.transparency(bkgrd, c, transparency)
                    screen.pixels(pos) = c
                    zBuffer(pos) = z

                    If useIdBuffer Then idBuffer(pos) = currentId
                End If
                z += dz
                tx += dtx
                ty += dty
                sw += dsw
            Next
        End Sub

		private sub renderLinePE()
            For x = xL To xR - 1 'Step x + 1
                pos = x + offset
                If z < zBuffer(pos) Then
                    lutID = ((nx >> 16) And 255) + (((ny >> 16) And 255) << 8)

                    bkgrd = screen.pixels(pos)
                    c = warp_Color.multiply(color, diffuse(lutID))
                    s = warp_Color.add(specular(lutID), envmap(lutID))
                    s = warp_Color.scale(s, reflectivity)
                    c = warp_Color.transparency(bkgrd, c, transparency)
                    c = warp_Color.add(c, s)
                    screen.pixels(pos) = c
                    zBuffer(pos) = z

                    If useIdBuffer Then idBuffer(pos) = currentId
                End If
                z += dz
                nx += dnx
                ny += dny
            Next
        End Sub

        Private Sub renderLinePT()
            For x = xL To xR - 1 'Step x + 1
                pos = x + offset
                If z < zBuffer(pos) Then
                    lutID = ((nx >> 16) And 255) + (((ny >> 16) And 255) << 8)

                    bkgrd = screen.pixels(pos)
                    c = texture.pixel((CInt(tx / sw) And tw) + ((CInt(ty / sw) And th) << tbitW))
                    c = warp_Color.multiply(c, diffuse(lutID))
                    s = warp_Color.scale(specular(lutID), reflectivity)
                    c = warp_Color.transparency(bkgrd, c, transparency)
                    c = warp_Color.add(c, s)
                    screen.pixels(pos) = c
                    zBuffer(pos) = z

                    If useIdBuffer Then idBuffer(pos) = currentId
                End If

                z += dz
                nx += dnx
                ny += dny
                tx += dtx
                ty += dty
                sw += dsw
            Next
        End Sub

        Private Sub renderLinePET()
            For x = xL To xR - 1 'Step x + 1
                pos = x + offset
                If z < zBuffer(pos) Then
                    lutID = ((nx >> 16) And 255) + (((ny >> 16) And 255) << 8)
                    bkgrd = screen.pixels(pos)
                    c = texture.pixel((CInt(tx / sw) And tw) + ((CInt(ty / sw) And th) << tbitW))
                    c = warp_Color.multiply(c, diffuse(lutID))
                    s = warp_Color.add(specular(lutID), envmap(lutID))
                    s = warp_Color.scale(s, reflectivity)
                    c = warp_Color.transparency(bkgrd, c, transparency)
                    c = warp_Color.add(c, s)

                    screen.pixels(pos) = c
                    zBuffer(pos) = z
                    If useIdBuffer Then idBuffer(pos) = currentId
                End If
                z += dz
                nx += dnx
                ny += dny
                tx += dtx
                ty += dty
                sw += dsw
            Next
        End Sub

        Private Sub drawWireframe(ByVal tri As warp_Triangle, ByVal defaultcolor As Integer)
            drawLine(tri.p1, tri.p2, defaultcolor)
            drawLine(tri.p2, tri.p3, defaultcolor)
            drawLine(tri.p3, tri.p1, defaultcolor)
        End Sub

        Private Sub drawLine(ByVal a As warp_Vertex, ByVal b As warp_Vertex, ByVal color As Integer)
            Dim temp As warp_Vertex
            If (a.clipcode And b.clipcode) <> 0 Then Exit Sub

            dx = Math.Abs(a.x - b.x)
            dy = Math.Abs(a.y - b.y)
            dz = 0

            If dx > dy Then
                If a.x > b.x Then
                    temp = a
                    a = b
                    b = temp
                End If

                If dx > 0 Then
                    dz = CInt((b.z - a.z) / dx)
                    dy = CInt(((b.y - a.y) << 16) / dx)
                End If

                z = a.z
                y = a.y << 16
                For x = a.x To b.x 'Step x + 1
                    y2 = y >> 16
                    If warp_Math.inrange(x, 0, width - 1) AndAlso warp_Math.inrange(y2, 0, height - 1) Then
                        offset = y2 * width
                        If z < zBuffer(x + offset) Then
                            screen.pixels(x + offset) = color
                            zBuffer(x + offset) = z
                        End If
                        If useIdBuffer Then idBuffer(x + offset) = currentId
                    End If
                    z += dz
                    y += dy
                Next

            Else

                If a.y > b.y Then
                    temp = a
                    a = b
                    b = temp
                End If

                If dy > 0 Then
                    dz = CInt((b.z - a.z) / dy)
                    dx = CInt(((b.x - a.x) << 16) / dy)
                End If

                z = a.z
                x = a.x << 16
                For y = a.y To b.y 'Step y + 1
                    x2 = x >> 16
                    If warp_Math.inrange(x2, 0, width - 1) AndAlso warp_Math.inrange(y, 0, height - 1) Then
                        offset = y * width
                        If z < zBuffer(x2 + offset) Then
                            screen.pixels(x2 + offset) = color
                            zBuffer(x2 + offset) = z
                        End If
                        If useIdBuffer Then idBuffer(x2 + offset) = currentId
                    End If
                    z += dz
                    x += dx
                Next
            End If
        End Sub
    End Class
End Namespace