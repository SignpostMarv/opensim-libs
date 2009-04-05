using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BulletDotNET
{
    public class ClosestNotMeRayResultCallback : btRayResultCallback, IDisposable
    {

        public static ClosestNotMeRayResultCallback FromIntPtr(IntPtr handle)
        {
            return (ClosestNotMeRayResultCallback)Native.GetObject(handle,
                typeof(ClosestNotMeRayResultCallback));
        }

        public ClosestNotMeRayResultCallback(IntPtr handle)
        {
            m_handle = handle;
        }

        public ClosestNotMeRayResultCallback(btRigidBody body)
        {
            m_handle = BulletHelper_CreateClosestNotMeRaycastResultCallback(body.Handle);
        }

        public bool hasHit()
        {
            return BulletHelper_ClosestNotMeRaycastResultCallback_hasHit(m_handle);
        }

        public btVector3 getHitPointWorld()
        {
            IntPtr resultHandle = BulletHelper_ClosestNotMeRaycastResultCallback_getHitPointWorld(m_handle);
            if (resultHandle != IntPtr.Zero)
                return btVector3.FromIntPtr(resultHandle);
            else
                return null;
        }


        #region NativeInvokes

          
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletHelper_CreateClosestNotMeRaycastResultCallback(IntPtr body);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern bool BulletHelper_ClosestNotMeRaycastResultCallback_hasHit(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletHelper_ClosestNotMeRaycastResultCallback_getHitPointWorld(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletHelper_ClosestNotMeRaycastResultCallback_delete(IntPtr handle);
        #endregion

        #region IDisposable Members

        public void Dispose()
        { 
            Dispose(true);
             GC.SuppressFinalize(this);
        }
        
        #endregion

        protected virtual void Dispose(bool disposing)
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
                }
                // Release unmanaged resources. If disposing is false, 
                // only the following code is executed.
                BulletHelper_ClosestNotMeRaycastResultCallback_delete(m_handle);
                m_handle = IntPtr.Zero;
                // Note that this is not thread safe.
                // Another thread could start disposing the object
                // after the managed resources are disposed,
                // but before the disposed flag is set to true.
                // If thread safety is necessary, it must be
                // implemented by the client.

            }
            m_disposed = true;
        }

        ~ClosestNotMeRayResultCallback()
        {
           // Do not re-create Dispose clean-up code here.
           // Calling Dispose(false) is optimal in terms of
           // readability and maintainability.
           Dispose(false);
        }
    }
}
