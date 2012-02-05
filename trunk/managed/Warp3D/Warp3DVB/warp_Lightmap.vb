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
    '/ <summary>
    '/ Summary description for warp_Lightmap.
    '/ </summary>
    Public Class warp_Lightmap
        Public diffuse() As Integer = New Integer(65535) {}
        Public specular() As Integer = New Integer(65535) {}
        Private sphere() As Single = New Single(65535) {}
        Private light() As warp_Light
        Private lights As Integer
        Private ambient As Integer
        Private temp As Integer, overflow As Integer, color As Integer, pos As Integer, r As Integer, g As Integer, b As Integer

        Public Sub New(ByVal scene As warp_Scene)
            scene.rebuild()

            light = scene.light
            lights = scene.lights
            ambient = scene.environment.ambient

            buildSphereMap()
            rebuildLightmap()
        End Sub

        Private Sub buildSphereMap()
            Dim fnx As Single, fny As Single, fnz As Single
            Dim pos As Integer
            Dim ny As Integer
            For ny = -128 To 128 - 1 ' Step ny + 1
                fny = ny / 128.0F
                Dim nx As Integer
                For nx = -128 To 128 - 1 'Step nx + 1
                    pos = nx + 128 + ((ny + 128) << 8)
                    fnx = nx / 128.0F
                    fnz = CSng(1 - Math.Sqrt(fnx * fnx + fny * fny))
                    If fnz > 0 Then sphere(pos) = fnz Else sphere(pos) = 0
                Next
            Next
        End Sub

        Public Sub rebuildLightmap()
            ' System.Console.WriteLine(">> Rebuilding Light Map  ...  [" + lights + " light sources]")
            Dim l As warp_Vector
            Dim fnx As Single, fny As Single, phongfact As Single, sheen As Single, spread As Single
            Dim diffuse As Integer, specular As Integer, cos As Integer, dr As Integer, dg As Integer, db As Integer, sr As Integer, sg As Integer, sb As Integer
            Dim ny As Integer
            For ny = -128 To 128 - 1 'Step ny + 1
                fny = ny / 128.0F
                Dim nx As Integer
                For nx = -128 To 128 - 1 'Step nx + 1
                    pos = nx + 128 + ((ny + 128) << 8)
                    fnx = nx / 128.0F
                    sr = 0
                    sg = 0
                    sb = 0

                    dr = warp_Color.getRed(ambient)
                    dg = warp_Color.getGreen(ambient)
                    db = warp_Color.getBlue(ambient)

                    Dim i As Integer
                    For i = 0 To lights - 1 'Step i + 1
                        l = light(i).v
                        diffuse = light(i).diffuse
                        specular = light(i).specular
                        sheen = light(i).highlightSheen / 255.0F
                        spread = CSng(light(i).highlightSpread / 4096)
                        If spread < 0.01F Then spread = 0.01F 'else spread = spread
                        cos = CInt((255 * warp_Vector.angle(light(i).v, New warp_Vector(fnx, fny, sphere(pos)))))
                        If cos <= 0 Then cos = 0
                        dr += (warp_Color.getRed(diffuse) * cos) >> 8
                        dg += (warp_Color.getGreen(diffuse) * cos) >> 8
                        db += (warp_Color.getBlue(diffuse) * cos) >> 8
                        phongfact = CSng(sheen * Math.Pow(cos / 255.0F, 1 / spread))
                        sr = CInt(sr + warp_Color.getRed(specular) * phongfact)
                        sg = CInt(sg + warp_Color.getGreen(specular) * phongfact)
                        sb = CInt(sb + warp_Color.getBlue(specular) * phongfact)
                    Next
                    Me.diffuse(pos) = warp_Color.getCropColor(dr, dg, db)
                    Me.specular(pos) = warp_Color.getCropColor(sr, sg, sb)
                Next
            Next
        End Sub
    End Class
End Namespace