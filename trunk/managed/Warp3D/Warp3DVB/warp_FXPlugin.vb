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
    Public MustInherit Class warp_FXPlugin
        Public scene As warp_Scene = Nothing
        Public screen As warp_Screen = Nothing

        Public Sub New(ByVal scene As warp_Scene)
            Me.scene = scene
            screen = scene.renderPipeline.screen
        End Sub

        Public MustOverride Sub apply()

    End Class
End Namespace