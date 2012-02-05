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
imports System.Collections
imports System.ComponentModel
imports System.Drawing
imports System.Data
imports System.Text
imports System.Windows.Forms
Imports System.Runtime.InteropServices
Imports Warp3DVB.Rednettle.Warp3D

namespace Warp3D
    Partial Public Class Warp3D
        Inherits UserControl

        Dim _scene As Rednettle.Warp3D.warp_Scene = Nothing
        Dim _waiting As String = "Waiting..."

        Private _plugins As New Hashtable()
        Private _models As New Hashtable()

        Private _dragging As Boolean = False
        Private _oldx As Integer = 0
        Private _oldy As Integer = 0
        Private _controlkey As Boolean = False
        Public _keargs As KeyEventArgs

        'Public Sub New()
        ' me.InitializeComponent

        '  Me.MouseUp = New MouseEventHandler(AddressOf OnMouseUp)
        '  Me.MouseDown = New MouseEventHandler(AddressOf OnMouseDown)
        '  Me.MouseMove = New MouseEventHandler(AddressOf OnMouseMove)
        'End Sub
        Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
            MyBase.OnPaint(e)

            Dim g As Graphics = e.Graphics

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic

            If _scene IsNot Nothing Then
                g.DrawImage(_scene.getImage(), 0, 0, Me.Width, Me.Height)
            Else
                Dim drawFont As Font = New Font("Verdana", 8)

                Dim drawBrush As SolidBrush = New SolidBrush(Color.Black)
                Dim drawRect As RectangleF = New RectangleF(-1, -1, Width + 1, Height + 1)

                e.Graphics.FillRectangle(New SolidBrush(Me.BackColor), drawRect)
                e.Graphics.DrawString(_waiting, drawFont, drawBrush, drawRect)
            End If
        End Sub

        Public Sub OnMouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles MyBase.MouseMove
            If _dragging Then
                Dim dx As Single = (e.Y - _oldy) / 50.0F
                Dim dy As Single = (_oldx - e.X) / 50.0F

                If _controlkey Then
                    _scene.defaultCamera.shift(0, 0, dx)
                Else
                    _scene.rotate(dx, dy, 0)
                End If

                _scene.render()

                _oldx = e.X
                _oldy = e.Y

                Refresh()
            End If
        End Sub

        Public Sub OnMouseUp(ByVal sender As Object, ByVal e As MouseEventArgs)
            _dragging = False
        End Sub

        Public Sub OnMouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
            _oldx = e.X
            _oldy = e.Y

            _dragging = True
        End Sub

        Protected Overrides Sub OnPaintBackground(ByVal e As PaintEventArgs)
        End Sub

        Public Function RegisterPlugIN(ByVal name As String, ByVal plugin As Rednettle.Warp3D.warp_FXPlugin) As Boolean
            If _scene Is Nothing Then Return False

            _plugins.Add(name, plugin)

            Return True
        End Function

        Public Sub ShiftDefaultCamera(ByVal x As Single, ByVal y As Single, ByVal z As Single)
            _scene.defaultCamera.shift(x, y, z)
        End Sub

        Public Function AddSphere(ByVal name As String, ByVal radius As Single, ByVal segments As Integer) As Boolean
            If _scene Is Nothing Then Return False

            Dim o As warp_Object = warp_ObjectFactory.SPHERE(radius, segments)

            If o Is Nothing Then Return False

            _scene.addObject(name, o)
            _scene.rebuild()

            Return True
        End Function


        Public Function AddPlane(ByVal name As String, ByVal size As Single) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim o As warp_Object = warp_ObjectFactory.SIMPLEPLANE(size, True)

            If o Is Nothing Then
                Return False
            End If

            _scene.addObject(name, o)

            Return True
        End Function

        Public Function AddCube(ByVal name As String, ByVal size As Single) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim o As warp_Object = warp_ObjectFactory.CUBE(size)

            If o Is Nothing Then
                Return False
            End If

            _scene.addObject(name, o)
            _scene.rebuild()

            Return True
        End Function

        Public Function AddBox(ByVal name As String, ByVal x As Single, ByVal y As Single, ByVal z As Single) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim o As warp_Object = warp_ObjectFactory.BOX(x, y, z)

            If o Is Nothing Then
                Return False
            End If

            _scene.addObject(name, o)
            _scene.rebuild()

            Return True
        End Function


        Public Function ProjectFrontal(ByVal name As String) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim o As warp_Object = _scene.sceneobject(name)
            If o Is Nothing Then
                Return False
            End If

            warp_TextureProjector.projectFrontal(o)

            Return True
        End Function

        Public Function ProjectCylindric(ByVal name As String) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim o As warp_Object = _scene.sceneobject(name)
            If o Is Nothing Then
                Return False
            End If

            warp_TextureProjector.projectCylindric(o)

            Return True
        End Function

        Public Function ShiftObject(ByVal name As String, ByVal x As Single, ByVal y As Single, ByVal z As Single) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim o As warp_Object = _scene.sceneobject(name)
            If o Is Nothing Then
                Return False
            End If

            o.shift(x, y, z)

            Return True
        End Function

        Public Function SetPos(ByVal name As String, ByVal x As Single, ByVal y As Single, ByVal z As Single) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim o As warp_Object = _scene.sceneobject(name)
            If o Is Nothing Then
                Return False
            End If

            o.setPos(x, y, z)

            Return True
        End Function


        Public Function AddLensFlare(ByVal name As String) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim lensFlare As warp_FXLensFlare = New warp_FXLensFlare(name, _scene, False)
            lensFlare.preset1()

            RegisterPlugIN(name, lensFlare)

            Return True
        End Function

        Public Function NormaliseScene() As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            _scene.normalize()

            Return True
        End Function

        Public Function SetAmbient(ByVal c As Integer) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            _scene.setAmbient(c)

            Return True
        End Function

        Public Function RotateScene(ByVal quat As warp_Quaternion, ByVal x As Single, ByVal y As Single, ByVal z As Single) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            _scene.rotate(quat, x, y, z)

            Return True
        End Function

        Public Function RotateScene(ByVal x As Single, ByVal y As Single, ByVal z As Single) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            _scene.rotate(x, y, z)

            Return True
        End Function

        Public Function RotateScene(ByVal m As warp_Matrix) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            _scene.rotate(m)

            Return True
        End Function

        Public Function ScaleScene(ByVal x As Single, ByVal y As Single, ByVal z As Single) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            _scene.scale(x, y, z)

            Return True
        End Function

        Public Function TranslateScene(ByVal x As Single, ByVal y As Single, ByVal z As Single) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            _scene.shift(x, y, z)

            Return True
        End Function


        Public Function RotateObject(ByVal name As String, ByVal x As Single, ByVal y As Single, ByVal z As Single) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim o As warp_Object = _scene.sceneobject(name)
            If o Is Nothing Then
                Return False
            End If

            o.rotate(x, y, z)

            Return True
        End Function

        Public Function RotateSelf(ByVal name As String, ByVal x As Single, ByVal y As Single, ByVal z As Single) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim o As warp_Object = _scene.sceneobject(name)
            If o Is Nothing Then
                Return False
            End If

            o.rotateSelf(x, y, z)

            Return True
        End Function

        Public Function RotateSelf(ByVal name As String, ByVal m As warp_Matrix) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim o As warp_Object = _scene.sceneobject(name)
            If o Is Nothing Then
                Return False
            End If

            o.rotateSelf(m)

            Return True
        End Function

        Public Function RotateSelf(ByVal name As String, ByVal quat As warp_Quaternion) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim o As warp_Object = _scene.sceneobject(name)
            If o Is Nothing Then
                Return False
            End If

            o.rotateSelf(quat)

            Return True
        End Function


        Public Function ScaleObject(ByVal name As String, ByVal s As Single) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim o As warp_Object = _scene.sceneobject(name)
            If o Is Nothing Then
                Return False
            End If

            o.scale(s)

            Return True
        End Function

        Public Function SetObjectMaterial(ByVal name As String, ByVal m As String) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim material As warp_Material = CType(_scene.materialData(m), warp_Material)
            If material Is Nothing Then
                Return False
            End If

            _scene.sceneobject(name).setMaterial(material)

            Return True
        End Function

        Public Function SetEnvMap(ByVal name As String, ByVal path As String) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim texture As warp_Texture = Nothing
            Try
                texture = New warp_Texture(path)
            Catch
                Return False
            End Try

            Dim material As warp_Material = CType(_scene.materialData(name), warp_Material)
            If material Is Nothing Then
                Return False
            End If

            material.setEnvmap(texture)

            Return True
        End Function

        Public Function AddLight(ByVal name As String, ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal color As Integer, ByVal d As Integer, ByVal s As Integer) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            _scene.addLight(name, New warp_Light(New warp_Vector(x, y, z), color, d, s))

            Return True
        End Function

        Public Function RotateModelSelf(ByVal name As String, ByVal x As Single, ByVal y As Single, ByVal z As Single) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim model As Hashtable = CType(_models(name), Hashtable)
            If model Is Nothing Then
                Return False
            End If

            Dim myDE As DictionaryEntry
            For Each myDE In model
                Dim key As String = CType(myDE.Key, String)
                Dim o As warp_Object = CType(myDE.Value, warp_Object)

                o.rotateSelf(x, y, z)
            Next

            Return True
        End Function

        Public Function RotateModel(ByVal name As String, ByVal x As Single, ByVal y As Single, ByVal z As Single) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim model As Hashtable = CType(_models(name), Hashtable)
            If model Is Nothing Then
                Return False
            End If

            Dim myDE As DictionaryEntry
            For Each myDE In model
                Dim key As String = CType(myDE.Key, String)
                Dim o As warp_Object = CType(myDE.Value, warp_Object)

                o.rotate(x, y, z)
            Next

            Return True
        End Function

        Public Function TranslateModel(ByVal name As String, ByVal x As Single, ByVal y As Single, ByVal z As Single) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim model As Hashtable = CType(_models(name), Hashtable)
            If model Is Nothing Then
                Return False
            End If

            Dim myDE As DictionaryEntry
            For Each myDE In model
                Dim key As String = CType(myDE.Key, String)
                Dim o As warp_Object = CType(myDE.Value, warp_Object)

                o.shift(x, y, z)
            Next

            Return True
        End Function

        Public Function ScaleModel(ByVal name As String, ByVal scale As Single) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim model As Hashtable = CType(_models(name), Hashtable)
            If model Is Nothing Then
                Return False
            End If

            Dim myDE As DictionaryEntry
            For Each myDE In model
                Dim key As String = CType(myDE.Key, String)
                Dim o As warp_Object = CType(myDE.Value, warp_Object)

                o.scaleSelf(scale)
            Next

            Return True
        End Function


        Public Function Import3Ds(ByVal name As String, ByVal path As String, ByVal addtoscene As Boolean) As Hashtable
            If _scene Is Nothing Then
                Return Nothing
            End If

            Dim list As Hashtable = Nothing
            Dim studio As warp_3ds_Importer = New warp_3ds_Importer()
            Try
                list = studio.importFromFile(name, path)

                If addtoscene Then
                    Dim myDE As DictionaryEntry
                    For Each myDE In list
                        Dim key As String = CType(myDE.Key, String)
                        Dim o As warp_Object = CType(myDE.Value, warp_Object)

                        _scene.addObject(key, o)
                    Next
                End If

                _scene.rebuild()
                _models.Add(name, list)
            Catch
                Return Nothing
            End Try

            Return list
        End Function

        Public Function SetBackgroundColor(ByVal c As Integer) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            _scene.environment.bgcolor = c

            Return True
        End Function

        Public Function SetBackgroundTexture(ByVal path As String) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim texture As warp_Texture = Nothing
            Try
                texture = New warp_Texture(path)
            Catch
                Return False
            End Try

            _scene.environment.setBackground(texture)

            Return True
        End Function

        Public Function SetBackgroundMaterial(ByVal path As String) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim material As warp_Material = Nothing
            Try
                material = New warp_Material(path)
            Catch
                Return False
            End Try

            Dim texture As warp_Texture = material.getTexture()
            If texture Is Nothing Then
                Return False
            End If

            _scene.environment.setBackground(texture)
            Return True
        End Function

        Public Function SetWireframe(ByVal name As String, ByVal w As Boolean) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim material As warp_Material = CType(_scene.materialData(name), warp_Material)
            If material Is Nothing Then
                Return False
            End If

            material.setWireframe(w)

            Return True
        End Function

        Public Function SetTexture(ByVal name As String, ByVal path As String) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim texture As warp_Texture = Nothing
            Try
                texture = New warp_Texture(path)
            Catch
                Return False
            End Try

            Dim material As warp_Material = CType(_scene.materialData(name), warp_Material)
            If material Is Nothing Then
                Return False
            End If

            material.setTexture(texture)

            Return True
        End Function

        Public Function AddMaterial(ByVal name As String) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim material As warp_Material = New warp_Material()
            _scene.addMaterial(name, material)

            Return True
        End Function

        Public Function AddMaterial(ByVal name As String, ByVal color As Integer) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim material As warp_Material = New warp_Material(color)
            _scene.addMaterial(name, material)

            Return True
        End Function

        Public Function AddMaterial(ByVal name As String, ByVal path As String) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim material As warp_Material = Nothing
            Try
                material = New warp_Material(path)
            Catch
                Return False
            End Try

            _scene.addMaterial(name, material)

            Return True
        End Function

        Public Function SetReflectivity(ByVal name As String, ByVal r As Integer) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim material As warp_Material = CType(_scene.materialData(name), warp_Material)
            If material Is Nothing Then
                Return False
            End If

            material.setReflectivity(r)

            Return True
        End Function

        Public Function SetTransparency(ByVal name As String, ByVal t As Integer) As Boolean
            If _scene Is Nothing Then
                Return False
            End If

            Dim material As warp_Material = CType(_scene.materialData(name), warp_Material)
            If material Is Nothing Then
                Return False
            End If

            material.setTransparency(t)

            Return True
        End Function

        Public Function Render() As Boolean
            Try
                _scene.render()

                Dim myDE As DictionaryEntry
                For Each myDE In _plugins
                    Dim plugin As warp_FXPlugin = CType(myDE.Value, warp_FXPlugin)
                    plugin.apply()
                Next
            Catch
                Return False
            End Try

            Return True
        End Function

        Public Function CreateScene(ByVal width As Integer, ByVal height As Integer) As Boolean
            Try
                _scene = New warp_Scene(width, height)

                _plugins.Clear()
                _models.Clear()
            Catch
                Reset()
                Return False
            End Try

            Return True
        End Function

        Public Sub Reset()
            _scene = Nothing
            System.GC.Collect()
        End Sub


        Public Sub DisplayDefaultScene()
            _scene = New warp_Scene(512, 512)

            Dim crystal As warp_Material = New warp_Material(warp_TextureFactory.MARBLE(128, 128, 0.15F))
            _scene.addMaterial("crystal", crystal)

            Dim c As warp_Material = CType(_scene.materialData("crystal"), warp_Material)
            c.setReflectivity(255)
            c.setTransparency(100)

            _scene.environment.setBackground(warp_TextureFactory.CHECKERBOARD(128, 128, 3, &H0, &H999999))

            _scene.addLight("light1", New warp_Light(New warp_Vector(0.2F, 0.2F, 1.0F), &HFFFFFF, 320, 80))
            _scene.addLight("light2", New warp_Light(New warp_Vector(-1.0F, -1.0F, 1.0F), &HFFFFFF, 100, 40))

            Dim path() As warp_Vector = New warp_Vector(15) {}

            path(0) = New warp_Vector(0.0F, 0.2F, 0)
            path(1) = New warp_Vector(0.13F, 0.25F, 0)
            path(2) = New warp_Vector(0.33F, 0.3F, 0)
            path(3) = New warp_Vector(0.43F, 0.6F, 0)
            path(4) = New warp_Vector(0.48F, 0.9F, 0)
            path(5) = New warp_Vector(0.5F, 0.9F, 0)
            path(6) = New warp_Vector(0.45F, 0.6F, 0)
            path(7) = New warp_Vector(0.35F, 0.3F, 0)
            path(8) = New warp_Vector(0.25F, 0.2F, 0)
            path(9) = New warp_Vector(0.1F, 0.15F, 0)
            path(10) = New warp_Vector(0.1F, 0.0F, 0)
            path(11) = New warp_Vector(0.1F, -0.5F, 0)
            path(12) = New warp_Vector(0.35F, -0.55F, 0)
            path(13) = New warp_Vector(0.4F, -0.6F, 0)
            path(14) = New warp_Vector(0.0F, -0.6F, 0)

            _scene.addObject("wineglass", warp_ObjectFactory.ROTATIONOBJECT(path, 32))
            _scene.sceneobject("wineglass").setMaterial(_scene.material("crystal"))

            _scene.sceneobject("wineglass").scale(0.8F, 0.8F, 0.8F)
            _scene.sceneobject("wineglass").rotate(0.5F, 0.0F, 0.0F)

            _scene.render()

            Refresh()
        End Sub

        Private Sub Warp3D_Load(ByVal sender As Object, ByVal e As EventArgs)

        End Sub

        Private Sub Warp3D_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)
            _keargs = e
            _controlkey = e.Control
        End Sub

        Private Sub Warp3D_KeyUp(ByVal sender As Object, ByVal e As KeyEventArgs)
            _keargs = e
            _controlkey = e.Control
        End Sub


    End Class
end namespace