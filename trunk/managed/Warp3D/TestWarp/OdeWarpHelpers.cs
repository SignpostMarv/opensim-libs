using System;
using Rednettle.Warp3D;

namespace Ode
{
    /// <summary>
    /// Summary description for OdeDxHelpers.
    /// </summary>
    public sealed class OdeWarpHelpers
    {
        private OdeWarpHelpers() { }

        public static warp_Matrix Ode2WarpMatrix( Ode.Matrix3 m )
        {
            warp_Matrix result = new warp_Matrix();

            result.m11 = ( float )m.m11;
            result.m12 = ( float )m.m12;
            result.m13 = ( float )m.m13;

            result.m21 = ( float )m.m21;
            result.m22 = ( float )m.m22;
            result.m23 = ( float )m.m23;

            result.m31 = ( float )m.m31;
            result.m32 = ( float )m.m32;
            result.m33 = ( float )m.m33;

            return result;
        }

        public static Ode.Matrix3 Warp2OdeMatrix3( warp_Matrix m )
        {
            Ode.Matrix3 odeM = new Ode.Matrix3();

            odeM.m11 = m.m11;
            odeM.m12 = m.m12;
            odeM.m13 = m.m13;

            odeM.m21 = m.m21;
            odeM.m22 = m.m22;
            odeM.m23 = m.m23;

            odeM.m31 = m.m31;
            odeM.m32 = m.m32;
            odeM.m33 = m.m33;

            return odeM;
        }

        public static warp_Quaternion Ode2WarpQuaternion( Ode.Quaternion odeQ )
        {
            warp_Quaternion q = new warp_Quaternion(
                ( float )odeQ.X,
                ( float )odeQ.Y,
                ( float )odeQ.Z,
                ( float )odeQ.W );

            return q;
        }

        public static Ode.Quaternion Warp2OdeQuaternion( warp_Quaternion q )
        {
            Ode.Quaternion odeQ = new Ode.Quaternion();

            odeQ.W = q.W;
            odeQ.X = q.X;
            odeQ.Y = q.Y;
            odeQ.Z = q.Z;
            
            return odeQ;
        }

        public static warp_Vector Ode2DxVector3( Ode.Vector3 odeV )
        {
            return new warp_Vector( ( float )odeV.X, ( float )odeV.Y, ( float )odeV.Z );
        }

        public static Ode.Vector3 Dx2OdeVector3( warp_Vector dxV )
        {
            return new Ode.Vector3( dxV.x, dxV.y, dxV.z );
        }
    }
}
