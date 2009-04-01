using System;
using System.Runtime.InteropServices;
using System.Security;


namespace BulletDotNET
{
    public class btAxisSweep3 : btBroadphaseInterface, IDisposable
    {
        private IntPtr m_handle = IntPtr.Zero;

        private bool m_disposed = false;

        public static btAxisSweep3 FromIntPtr(IntPtr handle)
        {
            return (btAxisSweep3)Native.GetObject(handle, typeof(btAxisSweep3));
        }

        public new IntPtr Handle
        {
            get { return m_handle; }
        }

        public btAxisSweep3(IntPtr handle) : base(handle)
        {
            m_handle = handle;
        }

        public btAxisSweep3(btVector3 worldAabbMin, btVector3 worldAabbMax, int MaxProxies)
        {
            m_handle = BulletAPI_CreateBtAxisSweep3(worldAabbMin.Handle, worldAabbMax.Handle, MaxProxies);
            base.Handle = m_handle;
        }

        #region Native Invokes

         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtAxisSweep3(IntPtr worldAabbMin, IntPtr worldAabbMax, int MaxProxies);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtAxisSweep3_delete(IntPtr ax);

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
                BulletAPI_BtAxisSweep3_delete(m_handle);
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

        ~btAxisSweep3()      
        {
           // Do not re-create Dispose clean-up code here.
           // Calling Dispose(false) is optimal in terms of
           // readability and maintainability.
           Dispose(false);
        }

    }
}
