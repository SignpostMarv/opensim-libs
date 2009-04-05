using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BulletDotNET
{
    public class btDefaultMotionState : btMotionState, IDisposable
    {

        public static new btDefaultMotionState FromIntPtr(IntPtr handle)
        {
            return (btDefaultMotionState)Native.GetObject(handle, typeof(btDefaultMotionState));
        }


        public btDefaultMotionState(IntPtr handle) : base(handle)
        {
            m_handle = handle;
        }

        public btDefaultMotionState(): base()
        {
            m_handle = BulletAPI_CreateBtDefaultMotionState();
        }

        public btDefaultMotionState(btTransform startTrans) : base()
        {
            m_handle = BulletAPI_CreateBtDefaultMotionStateStartTrans(startTrans.Handle);
        }

        public btDefaultMotionState(btTransform startTrans, btTransform centerOfMassOffset)
        {
            m_handle = BulletAPI_CreateBtDefaultMotionStateStartTransCenterOfMassOffset(startTrans.Handle, centerOfMassOffset.Handle);
        }

        public void getWorldTransform(out btTransform result)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            result = btTransform.getIdentity();
            BulletAPI_BtDefaultMotionState_getWorldTransform(m_handle, result.Handle);
        }

        public void setWorldTransform(btTransform t)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());
            BulletAPI_BtDefaultMotionState_setWorldTransform(m_handle, t.Handle);
        }

        #region Native Invokes
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtDefaultMotionState();
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtDefaultMotionStateStartTrans(IntPtr startTrans);
        [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtDefaultMotionStateStartTransCenterOfMassOffset(IntPtr startTrans,
                                                                                                     IntPtr
                                                                                                         centerOfMassOffset);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtDefaultMotionState_getWorldTransform(IntPtr handle, IntPtr centerOfMassWorldTrans );
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtDefaultMotionState_setWorldTransform(IntPtr handle, IntPtr t );
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtDefaultMotionState_delete(IntPtr obj);

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

        ~btDefaultMotionState()
        {
           // Do not re-create Dispose clean-up code here.
           // Calling Dispose(false) is optimal in terms of
           // readability and maintainability.
           Dispose(false);
        }
    }
}
