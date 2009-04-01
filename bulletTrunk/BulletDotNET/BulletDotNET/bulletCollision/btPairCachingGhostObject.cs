using System;
using System.Runtime.InteropServices;
using System.Security;
namespace BulletDotNET
{
    public class btPairCachingGhostObject : btGhostObject
    {
        public static new btPairCachingGhostObject FromIntPtr(IntPtr handle)
        {
            return (btPairCachingGhostObject)Native.GetObject(handle, typeof(btPairCachingGhostObject));
        }

        public btPairCachingGhostObject(IntPtr handle)
            : base(handle)
        {

        }

        public btPairCachingGhostObject()
            : base()
        {
            m_handle = BulletAPI_CreateBtPairCachingGhostObject();
        }


        #region Native Invokes

        [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtPairCachingGhostObject();
        [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtPairCachingGhostObject_delete(IntPtr handle);

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
                    // none yet
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

        ~btPairCachingGhostObject()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
    }
}
