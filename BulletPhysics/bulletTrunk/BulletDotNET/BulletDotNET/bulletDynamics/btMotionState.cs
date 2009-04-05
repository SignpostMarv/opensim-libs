using System;
using System.Runtime.InteropServices;
using System.Security;


namespace BulletDotNET
{
    public class btMotionState : IDisposable
    {
        protected IntPtr m_handle = IntPtr.Zero;

        protected bool m_disposed = false;

        public static btMotionState FromIntPtr(IntPtr handle)
        {
            return (btMotionState)Native.GetObject(handle, typeof(btMotionState));
        }

        public IntPtr Handle
        {
            get { return m_handle; }
        }

        /// <summary>
        /// Do not use!, for C++ Interop only!
        /// </summary>
        public btMotionState()
        {
            
        }

        public btMotionState(IntPtr handle)
        {
            m_handle = handle;
        }
        #region Native Invokes

        [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtMotionState_getWorldTransform(IntPtr handle, IntPtr centerOfMassWorldTrans);
        [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtMotionState_setWorldTransform(IntPtr handle, IntPtr t);
        [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtMotionState_delete(IntPtr obj);
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
                BulletAPI_BtMotionState_delete(m_handle);
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

        ~btMotionState()
        {
           // Do not re-create Dispose clean-up code here.
           // Calling Dispose(false) is optimal in terms of
           // readability and maintainability.
           Dispose(false);
        }
    }
}
