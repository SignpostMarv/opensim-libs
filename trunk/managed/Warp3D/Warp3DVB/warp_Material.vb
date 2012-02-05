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
Imports System.IO

Namespace Rednettle.Warp3D
    '/ <summary>
    '/ Summary description for warp_Material.
    '/ </summary>
    Public Class warp_Material
        Public color As Integer = 0
        Public transparency As Integer = 0
        Public reflectivity As Integer = 255
        Public type As Integer = 0
        Public texture As warp_Texture = Nothing
        Public envmap As warp_Texture = Nothing
        Public flat As Boolean = False
        Public wireframe As Boolean = False
        Public opaque As Boolean = True
        Public texturePath As String = Nothing
        Public envmapPath As String = Nothing
        Public textureSettings As warp_TextureSettings = Nothing
        Public envmapSettings As warp_TextureSettings = Nothing
        Public rwx As Double = 500.0
        Public rwy As Double = 500.0
        Public pt As Point = New Point(0, 0)
        Public offset As Integer = 0

        Public Sub New()
            '
            ' TODO: Add constructor logic here
            '
        End Sub

        Public Sub New(ByVal color As Integer)
            setColor(color)
        End Sub

        Public Sub New(ByVal t As warp_Texture)
            setTexture(t)
            reflectivity = 255
        End Sub

        Public Sub New(ByVal path As String)


            Dim fs As FileStream = New FileStream(Path, FileMode.Open)
            Dim br As BinaryReader = New BinaryReader(fs)

            importFromStream(br)
        End Sub


        Private Sub importFromStream(ByVal inStream As BinaryReader)
            readSettings(inStream)
            readTexture(inStream, True)
            readTexture(inStream, False)
        End Sub

        Private Sub readSettings(ByVal inStream As BinaryReader)
            setColor(readInt(inStream))
            setTransparency(inStream.ReadByte())
            setReflectivity(inStream.ReadByte())
            setFlat(inStream.ReadBoolean())
        End Sub


        Private Function readInt(ByVal inStream As BinaryReader) As Integer
            Return (inStream.ReadByte() << 24) Or (inStream.ReadByte() << 16) Or (inStream.ReadByte() << 8) Or inStream.ReadByte()
        End Function

        Private Function readString(ByVal inStream As BinaryReader) As String
            Dim result As String = ""
            Dim inByte As Byte
            inByte = inStream.ReadByte()
            While inByte <> 60
                result &= inByte
                inByte = inStream.ReadByte()
            End While

            Return result
        End Function



        Private Sub readTexture(ByVal inStream As BinaryReader, ByVal textureId As Boolean)
            Dim t As warp_Texture = Nothing
            Dim id As Integer = inStream.ReadSByte()
            If id = 1 Then
                t = New warp_Texture("c:/source/warp3d/materials/skymap.jpg")

                If t IsNot Nothing AndAlso textureId Then
                    texturePath = t.path
                    textureSettings = Nothing
                    setTexture(t)
                End If
                If t IsNot Nothing AndAlso Not textureId Then
                    envmapPath = t.path
                    envmapSettings = Nothing
                    setEnvmap(t)
                End If
            End If

            If id = 2 Then
                Dim w As Integer = readInt(inStream)
                Dim h As Integer = readInt(inStream)
                Dim type As Integer = inStream.ReadSByte()

                Dim persistency As Single = readInt(inStream)
                Dim density As Single = readInt(inStream)

                persistency = 0.5F
                density = 0.5F

                Dim samples As Integer = inStream.ReadByte()
                Dim numColors As Integer = inStream.ReadByte()
                Dim colors() As Integer = New Integer(numColors - 1) {}
                Dim i As Integer
                For i = 0 To colors.Length - 1 ' numColors - 2 ' Step i + 1
                    colors(i) = readInt(inStream)

                Next
                If type = 1 Then
                    t = warp_TextureFactory.PERLIN(w, h, persistency, density, samples, 1024).colorize(warp_Color.makeGradient(colors, 1024))
                End If
                If type = 2 Then
                    t = warp_TextureFactory.WAVE(w, h, persistency, density, samples, 1024).colorize(warp_Color.makeGradient(colors, 1024))
                End If
                If type = 3 Then
                    t = warp_TextureFactory.GRAIN(w, h, persistency, density, samples, 20, 1024).colorize(warp_Color.makeGradient(colors, 1024))

                End If

                If textureId Then
                    texturePath = Nothing
                    textureSettings = New warp_TextureSettings(t, w, h, type, persistency, density, samples, colors)
                    setTexture(t)
                Else
                    envmapPath = Nothing
                    envmapSettings = New warp_TextureSettings(t, w, h, type, persistency, density, samples, colors)
                    setEnvmap(t)
                End If
            End If
        End Sub

        Public Sub setTexture(ByVal t As warp_Texture)
            texture = t
            If texture IsNot Nothing Then texture.resize()
        End Sub

        Public Sub setEnvmap(ByVal env As warp_Texture)
            envmap = env
            env.resize(256, 256)
        End Sub

        Public Sub setColor(ByVal c As Integer)
            color = c
        End Sub

        Public Sub setTransparency(ByVal factor As Integer)
            transparency = warp_Math.crop(factor, 0, 255)
            opaque = (transparency = 0)
        End Sub

        Public Sub setReflectivity(ByVal factor As Integer)
            reflectivity = warp_Math.crop(factor, 0, 255)
        End Sub

        Public Sub setFlat(ByVal flat As Boolean)
            Me.flat = flat
        End Sub

        Public Sub setWireframe(ByVal wireframe As Boolean)
            Me.wireframe = wireframe
        End Sub

        Public Sub setType(ByVal type As Integer)
            Me.type = type
        End Sub

        Public Function getTexture() As warp_Texture
            Return texture
        End Function

        Public Function getEnvmap() As warp_Texture
            Return envmap
        End Function

        Public Function getColor() As Integer
            Return color
        End Function

        Public Function getTransparency() As Integer
            Return transparency
        End Function

        Public Function getReflectivity() As Integer
            Return reflectivity
        End Function

        Public Function [getType]() As Integer
            Return type
        End Function

        Public Function isFlat() As Boolean
            Return flat
        End Function

        Public Function isWireframe() As Boolean
            Return wireframe
        End Function

    End Class
End Namespace