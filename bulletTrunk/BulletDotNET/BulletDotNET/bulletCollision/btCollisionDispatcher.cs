using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BulletDotNET
{
    public class btCollisionDispatcher : btDispatcher, IDisposable
    {
        private IntPtr m_handle = IntPtr.Zero;

        private bool m_disposed = false;

        public static btCollisionDispatcher FromIntPtr(IntPtr handle)
        {
            return (btCollisionDispatcher)Native.GetObject(handle, typeof(btCollisionDispatcher));
        }

        public new IntPtr Handle
        {
            get { return m_handle; }
        }

        public btCollisionDispatcher(IntPtr handle) : base(handle)
        {
            m_handle = handle;
        }

        public btCollisionDispatcher(btCollisionConfiguration config)
        {
            m_handle = BulletAPI_CreateBtCollisionDispatcher(config.Handle);
            base.Handle = m_handle;
        }

        // Registers using the GImpact collision algorithm (so things will collide)
        public void RegisterGImpact()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionDispatcher_RegisterGImpact(m_handle);
        }

         #region Native Invokes
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtCollisionDispatcher(IntPtr config);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionDispatcher_RegisterGImpact(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionDispatcher_delete(IntPtr obj);
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
                BulletAPI_BtCollisionDispatcher_delete(m_handle);
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

        ~btCollisionDispatcher()      
        {
           // Do not re-create Dispose clean-up code here.
           // Calling Dispose(false) is optimal in terms of
           // readability and maintainability.
           Dispose(false);
        }
    }
}
