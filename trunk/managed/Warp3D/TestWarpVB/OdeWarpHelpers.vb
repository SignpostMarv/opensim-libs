Imports System
Imports Warp3DVB.Rednettle.Warp3D ' Rednettle.Warp3D

'Namespace Ode
''' <summary>
''' Summary description for OdeDxHelpers.
''' </summary>
Public NotInheritable Class OdeWarpHelpers
    Private Sub New()
    End Sub

    Public Shared Function Ode2WarpMatrix(ByVal m As Ode.Matrix3) As warp_Matrix
        Dim result As New warp_Matrix()

        result.m11 = m.m11
        result.m12 = m.m12
        result.m13 = m.m13

        result.m21 = m.m21
        result.m22 = m.m22
        result.m23 = m.m23

        result.m31 = m.m31
        result.m32 = m.m32
        result.m33 = m.m33

        Return result
    End Function

    Public Shared Function Warp2OdeMatrix3(ByVal m As warp_Matrix) As Ode.Matrix3
        Dim odeM As New Ode.Matrix3()

        odeM.m11 = m.m11
        odeM.m12 = m.m12
        odeM.m13 = m.m13

        odeM.m21 = m.m21
        odeM.m22 = m.m22
        odeM.m23 = m.m23

        odeM.m31 = m.m31
        odeM.m32 = m.m32
        odeM.m33 = m.m33

        Return odeM
    End Function

    Public Shared Function Ode2WarpQuaternion(ByVal odeQ As Ode.Quaternion) As warp_Quaternion
        Dim q As New warp_Quaternion( _
            odeQ.X, _
            odeQ.Y, _
            odeQ.Z, _
            odeQ.W)

        Return q
    End Function

    Public Shared Function Warp2OdeQuaternion(ByVal q As warp_Quaternion) As Ode.Quaternion
        Dim odeQ As New Ode.Quaternion()

        odeQ.W = q.W
        odeQ.X = q.X
        odeQ.Y = q.Y
        odeQ.Z = q.Z

        Return odeQ
    End Function

    Public Shared Function Ode2DxVector3(ByVal odeV As Ode.Vector3) As warp_Vector
        Return New warp_Vector(odeV.X, odeV.Y, odeV.Z)
    End Function

    Public Shared Function Dx2OdeVector3(ByVal dxV As warp_Vector) As Ode.Vector3
        Return New Ode.Vector3(dxV.x, dxV.y, dxV.z)
    End Function
End Class
