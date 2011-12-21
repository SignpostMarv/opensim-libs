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
Imports System.Collections

Namespace Rednettle.Warp3D
    Public Class warp_RenderPipeline
        Implements IDisposable
        Public screen As warp_Screen
        Dim scene As warp_Scene
        Public lightmap As warp_Lightmap

        Private resizingRequested As Boolean = False
        Private requestedWidth As Integer
        Private requestedHeight As Integer
        Private splitOffset As Double
        Public useId As Boolean = False

        Dim rasterizer As warp_Rasterizer

        Dim opaqueQueue As ArrayList = New ArrayList()
        Dim transparentQueue As ArrayList = New ArrayList()

        Dim zFar As Integer = &HFFFFFF

        Public zBuffer() As Integer
        Public idBuffer() As Integer


        Public Sub New(ByVal scene As warp_Scene, ByVal w As Integer, ByVal h As Integer)
            Me.scene = scene

            screen = New warp_Screen(w, h)
            ReDim zBuffer(screen.width * screen.height)
            rasterizer = New warp_Rasterizer(Me)
        End Sub

        Public Function getFPS() As Single
            Return (screen.FPS * 100) / 100
        End Function

        Public Sub buildLightMap()
			if lightmap is nothing
                lightmap = New warp_Lightmap(scene)
            Else
                lightmap.rebuildLightmap()
            End If

            rasterizer.loadLightmap(lightmap)
        End Sub

        Public Sub useIdBuffer(ByVal useId As Boolean)
            Me.useId = useId
            If useId Then
                ReDim idBuffer(screen.width * screen.height)
            Else
                idBuffer = Nothing
            End If
        End Sub

        Public Sub render(ByVal cam As warp_Camera)
            rasterizer.rebuildReferences(Me)

            warp_Math.clearBuffer(zBuffer, zFar)
            '//System.Array.Copy(screen.zBuffer,0,zBuffer,0,zBuffer.Length);

            If useId Then
                warp_Math.clearBuffer(idBuffer, -1)
            End If
            If scene.environment.background IsNot Nothing Then
                screen.drawBackground(scene.environment.background, 0, 0, screen.width, screen.height)
            Else
                screen.clear(scene.environment.bgcolor)
            End If

            cam.setScreensize(screen.width, screen.height)
            scene.prepareForRendering()
            emptyQueues()

            '// Project
            Dim m As warp_Matrix = warp_Matrix.multiply(cam.getMatrix(), scene.matrix)
            Dim nm As warp_Matrix = warp_Matrix.multiply(cam.getNormalMatrix(), scene.normalmatrix)
            Dim vertexProjection, normalProjection As warp_Matrix
            Dim obj As warp_Object
            Dim t As warp_Triangle
            Dim v As warp_Vertex
            Dim w As Integer = screen.width
            Dim h As Integer = screen.height
            For id As Integer = scene.objects - 1 To 0 Step -1
                obj = scene.wobject(id)
                If obj.visible Then
                    vertexProjection = obj.matrix.getClone()
                    normalProjection = obj.normalmatrix.getClone()
                    vertexProjection.transform(m)
                    normalProjection.transform(nm)

                    For i As Integer = obj.vertices - 1 To 0 Step -1
                        v = obj.fastvertex(i)
                        v.project(vertexProjection, normalProjection, cam)
                        v.clipFrustrum(w, h)
                    Next
                    For i As Integer = obj.triangles - 1 To 0 Step -1
                        t = obj.fasttriangle(i)
                        t.project(normalProjection)
                        t.clipFrustrum(w, h)
                        enqueueTriangle(t)
                    Next
                End If
                    Next

            '//screen.lockImage();

            Dim tri() As warp_Triangle
            tri = getOpaqueQueue()
            If tri IsNot Nothing Then
                For i As Integer = tri.GetLength(0) - 1 To 0 Step -1
                    rasterizer.loadMaterial(tri(i).parent.material)
                    rasterizer.render(tri(i))
                Next
            End If

            tri = getTransparentQueue()
            If tri IsNot Nothing Then
                For i As Integer = 0 To tri.GetLength(0) - 1
                    rasterizer.loadMaterial(tri(i).parent.material)
                    rasterizer.render(tri(i))
                Next
            End If

            '//screen.unlockImage();
        End Sub


        Private Sub performResizing()
            resizingRequested = False
            'screen.resize(requestedWidth, requestedHeight);

            zBuffer = New Integer(screen.width * screen.height) {}
            If useId Then
                idBuffer = New Integer(screen.width * screen.height) {}
            End If
        End Sub

        ' Triangle sorting
        Private Sub emptyQueues()
            opaqueQueue.Clear()
            transparentQueue.Clear()
        End Sub


        Private Sub enqueueTriangle(ByVal tri As warp_Triangle)
            If tri.parent.material Is Nothing Then
                Return
            End If
            If tri.visible = False Then
                Return
            End If
            If (tri.parent.material.transparency = 255) AndAlso (tri.parent.material.reflectivity = 0) Then
                Return
            End If

            If tri.parent.material.transparency > 0 Then
                transparentQueue.Add(tri)
            Else
                opaqueQueue.Add(tri)
            End If
        End Sub

        Private Function getOpaqueQueue() As warp_Triangle()
            If opaqueQueue.Count = 0 Then Return Nothing

            Dim enumerator As IEnumerator = opaqueQueue.GetEnumerator()
            Dim tri(opaqueQueue.Count) As warp_Triangle

            Dim id As Integer = 0
            While enumerator.MoveNext()
                tri(id) = CType(enumerator.Current, Warp3D.warp_Triangle)
                id += 1
            End While

            Return sortTriangles(tri, 0, tri.GetLength(0) - 1)
        End Function

        Private Function getTransparentQueue() As warp_Triangle()
            If transparentQueue.Count = 0 Then Return Nothing
            Dim enumerator As IEnumerator = transparentQueue.GetEnumerator()
            Dim tri(transparentQueue.Count) As warp_Triangle

            Dim id As Integer = 0
            While enumerator.MoveNext()
                tri(id) = CType(enumerator.Current, Warp3D.warp_Triangle)
                id += 1
            End While

            Return sortTriangles(tri, 0, tri.GetLength(0) - 1)
        End Function

        Private Function sortTriangles(ByVal tri() As warp_Triangle, ByVal L As Integer, ByVal R As Integer) As warp_Triangle()
            Dim m As Single = (tri(L).dist + tri(R).dist) / 2
            Dim i As Integer = L
            Dim j As Integer = R
            Dim temp As warp_Triangle

            Do
                While tri(i).dist > m
                    i += 1
                End While
                While tri(j).dist < m
                    j -= 1
                End While

                If i <= j Then
                    temp = tri(i)
                    tri(i) = tri(j)
                    tri(j) = temp
                    i += 1
                    j -= 1
                End If
                
            Loop While j >= i

            If L < j Then sortTriangles(tri, L, j)
            If R > i Then sortTriangles(tri, i, R)

            Return tri
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose

        End Sub
    End Class
End Namespace