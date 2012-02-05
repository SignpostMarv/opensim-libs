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
Imports System.Drawing
Imports System.Timers

Namespace Rednettle.Warp3D
    '/ <summary>
    '/ Summary description for warp_Scene.
    '/ </summary>
    Public Class warp_Scene
        Inherits warp_CoreObject
        Public Shared version As String = "1.0.0"
        Public Shared release As String = "0010"

        Public renderPipeline As warp_RenderPipeline
        Public width As Integer, height As Integer

        Public environment As New warp_Environment '= New warp_Environment()
        Public defaultCamera As warp_Camera = warp_Camera.FRONT()

        Public wobject() As warp_Object
        Public light() As warp_Light

        Public objects As Integer = 0
        Public lights As Integer = 0

        Private objectsNeedRebuild As Boolean = True
        Private lightsNeedRebuild As Boolean = True

        Protected preparedForRendering As Boolean = False

        Public normalizedOffset As warp_Vector = New warp_Vector(0.0F, 0.0F, 0.0F)
        Public normalizedScale As Single = 1.0F
        Private Shared instancesRunning As Boolean = False

        Public objectData As Hashtable = New Hashtable()
        Public lightData As Hashtable = New Hashtable()
        Public materialData As Hashtable = New Hashtable()
        Public cameraData As Hashtable = New Hashtable()

        Dim probes As Integer = 0
        Dim perf As Integer = 0

        Public fps As String = "0"

        Public rendertime As Integer = 500

        Public Sub New()
        End Sub

        Public Sub New(ByVal w As Integer, ByVal h As Integer)
            showInfo()
            width = w
            height = h
            renderPipeline = New warp_RenderPipeline(Me, w, h)
        End Sub

        Public Sub showInfo()
            If instancesRunning Then Exit Sub

            System.Console.WriteLine()
            System.Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~")
            System.Console.WriteLine("WARP 3D Kernel " + version + " [Build " + release + "]")
            System.Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~")

            instancesRunning = True
        End Sub

        Public Sub removeAllObjects()
            objectData = New Hashtable()
            objectsNeedRebuild = True
            rebuild()
        End Sub

        Public Sub rebuild()

            If objectsNeedRebuild Then

                objectsNeedRebuild = False

                objects = objectData.Count
                wobject = New warp_Object(objects - 1) {}
                Dim enumerator As IDictionaryEnumerator = objectData.GetEnumerator()

                For i As Integer = objects - 1 To 0 Step -1
                    enumerator.MoveNext()
                    wobject(i) = CType(enumerator.Value, warp_Object)

                    wobject(i).id = i
                    wobject(i).rebuild()
                Next
            End If

            If lightsNeedRebuild Then
                lightsNeedRebuild = False
                lights = lightData.Count
                light = New warp_Light(lights - 1) {}
                Dim enumerator As IDictionaryEnumerator = lightData.GetEnumerator()
                For i As Integer = lights - 1 To 0 Step -1
                    enumerator.MoveNext()
                    light(i) = CType(enumerator.Value, warp_Light)

                Next
            End If
        End Sub

        Public Function sceneobject(ByVal key As String) As warp_Object
            Return CType(objectData(key), warp_Object)
        End Function

        Public Function material(ByVal key As String) As warp_Material
            Return CType(materialData(key), warp_Material)
        End Function

        Public Function camera(ByVal key As String) As warp_Camera
            Return CType(cameraData(key), warp_Camera)
        End Function

        Public Sub addObject(ByVal key As String, ByVal obj As warp_Object)
            obj.name = key
            objectData.Add(key, obj)
            obj.parent = Me
            objectsNeedRebuild = True
        End Sub

        Public Sub removeObject(ByVal key As String)
            objectData.Remove(key)
            objectsNeedRebuild = True
            preparedForRendering = False
        End Sub

        Public Sub addMaterial(ByVal key As String, ByVal m As warp_Material)
            materialData.Add(key, m)
        End Sub

        Public Sub removeMaterial(ByVal key As String)
            materialData.Remove(key)
        End Sub

        Public Sub addCamera(ByVal key As String, ByVal c As warp_Camera)
            cameraData.Add(key, c)
        End Sub

        Public Sub removeCamera(ByVal key As String)
            cameraData.Remove(key)
        End Sub

        Public Sub addLight(ByVal key As String, ByVal l As warp_Light)
            lightData.Add(key, l)
            lightsNeedRebuild = True
        End Sub

        Public Sub removeLight(ByVal key As String)
            lightData.Remove(key)
            lightsNeedRebuild = True
            preparedForRendering = False
        End Sub

        Public Sub prepareForRendering()
            If preparedForRendering = True Then
                Return
            End If
            preparedForRendering = True

            System.Console.WriteLine("warp_Scene| prepareForRendering : Preparing for realtime rendering ...")
            rebuild()
            renderPipeline.buildLightMap()
            printSceneInfo()
        End Sub

        Public Sub printSceneInfo()
            System.Console.WriteLine("warp_Scene| Objects   : " & objects)
            System.Console.WriteLine("warp_Scene| Vertices  : " & countVertices())
            System.Console.WriteLine("warp_Scene| Triangles : " & countTriangles())
        End Sub

        Public Sub render()

            Dim n As DateTime = DateTime.Now
            renderPipeline.render(Me.defaultCamera)

            Dim s As TimeSpan = DateTime.Now - n

            perf += s.Milliseconds
            probes += 1
            If probes = 32 Then
                probes = 0
                fps = (perf / 32) & "," & (1000 / (perf / 32))

                perf = 0
            End If

        End Sub

        Public Function getImage() As Bitmap
            Return renderPipeline.screen.getImage()
        End Function

        Public Function getFPS() As String
            Return fps '& " - yes"
        End Function
        ' 		
        ' 		Public java.awt.Dimension size()
        ' 		{
        ' 			Return New java.awt.Dimension(width,height)
        ' 		}
        ' 		*/

        Public Sub setBackgroundColor(ByVal bgcolor As Integer)
            environment.bgcolor = bgcolor
        End Sub

        Public Sub setBackground(ByVal t As warp_Texture)
            environment.setBackground(t)
        End Sub

        Public Sub setAmbient(ByVal ambientcolor As Integer)
            environment.ambient = ambientcolor
        End Sub


        Public Function countVertices() As Integer
            Dim counter As Integer = 0
            For i As Integer = 0 To objects - 1 'Step i + 1
                counter += wobject(i).vertices
            Next
            Return counter
        End Function

        Public Function countTriangles() As Integer
            Dim counter As Integer = 0

            For i As Integer = 0 To objects - 1 'Step i + 1
                counter += wobject(i).triangles
            Next
            Return counter
        End Function

        Public Sub useIdBuffer(ByVal buffer As Boolean)
            renderPipeline.useIdBuffer(Buffer)
        End Sub

        Public Function identifyTriangleAt(ByVal xpos As Integer, ByVal ypos As Integer) As warp_Triangle
            If Not renderPipeline.useId Then Return Nothing
            If xpos < 0 Or xpos >= width Then Return Nothing
            If ypos < 0 Or ypos >= height Then Return Nothing

            Dim pos As Integer = xpos + renderPipeline.screen.width * ypos
            Dim idCode As Integer = renderPipeline.idBuffer(pos)
            If idCode < 0 Then Return Nothing

            Return wobject(idCode >> 16).fasttriangle(idCode And &HFFFF)
        End Function

        Public Function identifyObjectAt(ByVal xpos As Integer, ByVal ypos As Integer) As warp_Object
            Dim tri As warp_Triangle = identifyTriangleAt(xpos, ypos)
            If tri Is Nothing Then Return Nothing
            Return tri.parent
        End Function

        Public Sub normalize()
            objectsNeedRebuild = True
            rebuild()

            Dim min As warp_Vector, max As warp_Vector, tempmax As warp_Vector, tempmin As warp_Vector
            If objects = 0 Then Exit Sub

            matrix = New warp_Matrix()
            normalmatrix = New warp_Matrix()

            max = wobject(0).maximum()
            min = wobject(0).maximum()

            For i As Integer = 0 To objects - 1 'Step i + 1
                tempmax = wobject(i).maximum()
                tempmin = wobject(i).maximum()
                If tempmax.x > max.x Then
                    max.x = tempmax.x
                End If
                If tempmax.y > max.y Then
                    max.y = tempmax.y
                End If
                If tempmax.z > max.z Then
                    max.z = tempmax.z
                End If
                If tempmin.x < min.x Then
                    min.x = tempmin.x
                End If
                If tempmin.y < min.y Then
                    min.y = tempmin.y
                End If
                If tempmin.z < min.z Then
                    min.z = tempmin.z
                End If
            Next
            Dim xdist As Single = max.x - min.x
            Dim ydist As Single = max.y - min.y
            Dim zdist As Single = max.z - min.z
            Dim xmed As Single = (max.x + min.x) / 2
            Dim ymed As Single = (max.y + min.y) / 2
            Dim zmed As Single = (max.z + min.z) / 2

            Dim diameter As Single
            If xdist > ydist Then diameter = xdist Else diameter = ydist
            If zdist > diameter Then diameter = zdist

            normalizedOffset = New warp_Vector(xmed, ymed, zmed)
            normalizedScale = 2 / diameter

            shift(normalizedOffset.reverse())
            scale(normalizedScale)
        End Sub
    End Class
End Namespace
