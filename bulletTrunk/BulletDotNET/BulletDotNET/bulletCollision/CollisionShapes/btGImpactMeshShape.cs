using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BulletDotNET
{
    public class btGImpactMeshShape : btCollisionShape, IDisposable
    {
        public static new btGImpactMeshShape FromIntPtr(IntPtr handle)
        {
            return (btGImpactMeshShape)Native.GetObject(handle, typeof(btGImpactMeshShape));
        }

        public btGImpactMeshShape(IntPtr handle)
            : base(handle)
        {

        }

        public btGImpactMeshShape(btStridingMeshInterface meshInterface)
            : base()
        {
            m_handle = BulletAPI_CreateBtGImpactMeshShape(meshInterface.Handle);
        }

        public void updateBound()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtGImpactMeshShape_updateBound(m_handle);
        }

        #region Native Invokes

        [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtGImpactMeshShape(IntPtr meshInterface);
        [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtGImpactMeshShape_updateBound(IntPtr handle);
        [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_btSphereShape_delete(IntPtr handle);

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

        ~btGImpactMeshShape()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

    }
}
