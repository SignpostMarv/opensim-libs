using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BulletDotNET
{
    public class btHeightfieldTerrainShape : btCollisionShape, IDisposable
    {
        public enum UPAxis : int
        {
            X = 0,
            Y = 1,
            Z = 2
        }
        public enum PHY_ScalarType : int
        {
            PHY_FLOAT = 0,
            PHY_DOUBLE = 1,
            PHY_INTEGER = 2,
            PHY_SHORT = 3,
            PHY_FIXEDPOINT88 = 4,
            PHY_UCHAR = 5
        }

        public static new btHeightfieldTerrainShape FromIntPtr(IntPtr handle)
        {
            return (btHeightfieldTerrainShape)Native.GetObject(handle, typeof(btHeightfieldTerrainShape));
        }

        public btHeightfieldTerrainShape(IntPtr handle) : base(handle)
        {
            
        }

        public btHeightfieldTerrainShape(int heightStickWidth, int heightStickLength, float[] heightfieldData, float heightScale, float minHeight, float maxHeight, int upAxis, int heightDataType, bool flipQuadEdges)
             : base()
         {
            m_handle = BulletAPI_CreateBtHeightfieldTerrainShape(heightStickWidth, heightStickLength, heightfieldData, heightScale, minHeight, maxHeight, upAxis, heightDataType, flipQuadEdges);
         }

        public btVector3 getLocalScaling()
        {
            return btVector3.FromIntPtr(BulletAPI_BtHeightFieldTerrainShape_getLocalScaling(m_handle));
        }

        public void setLocalScaling(btVector3 scaling)
        {
            BulletAPI_BtHeightFieldTerrainShape_setLocalScaling(m_handle, scaling.Handle);
        }

        #region Native Invokes

         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtHeightfieldTerrainShape (int heightStickWidth, int heightStickLength, [MarshalAs(UnmanagedType.LPArray)] float[] heightfieldData, float heightScale, float minHeight, float maxHeight,int upAxis, int heightDataType, bool flipQuadEdges);

         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtHeightFieldTerrainShape_getLocalScaling(IntPtr handle); // btVector3
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtHeightFieldTerrainShape_setLocalScaling(IntPtr handle, IntPtr scaling);//btVector3
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtHeightFieldTerrainShape_delete(IntPtr handle);

        #endregion


        #region IDisposable Members

        public new void Dispose()
        { 
            Dispose(true);
        }
        
        #endregion

        protected virtual new void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!m_disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    base.Dispose();

                }
                // Release unmanaged resources. If disposing is false, 
                // only the following code is executed.
                //BulletAPI_BtRigidBody_delete(m_handle);
                //m_handle = IntPtr.Zero;
                // Note that this is not thread safe.
                // Another thread could start disposing the object
                // after the managed resources are disposed,
                // but before the disposed flag is set to true.
                // If thread safety is necessary, it must be
                // implemented by the client.

            }
            m_disposed = true;
        }

        ~btHeightfieldTerrainShape()
        {
           // Do not re-create Dispose clean-up code here.
           // Calling Dispose(false) is optimal in terms of
           // readability and maintainability.
           Dispose(false);
        }
    }
}
