using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BulletDotNET
{
    public class btCapsuleShape : btCollisionShape, IDisposable
    {
        public static new btCapsuleShape FromIntPtr(IntPtr handle)
        {
            return (btCapsuleShape)Native.GetObject(handle, typeof(btCapsuleShape));
        }

        public btCapsuleShape(IntPtr handle)
            : base(handle)
        {
            
        }

        public btCapsuleShape(float radius, float height) : base()
        {
            m_handle = BulletAPI_CreateBtCapsuleShape(radius, height);
        }

        public float getRadius()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            
            return BulletAPI_BtCapsuleShape_getRadius(m_handle);
        }

        public float getHalfHeight()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            
            return BulletAPI_BtCapsuleShape_getHalfHeight(m_handle);
        }

        #region Native Invokes

         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtCapsuleShape(float radius, float height);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern  float BulletAPI_BtCapsuleShape_getRadius(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtCapsuleShape_getHalfHeight(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCapsuleShape_delete(IntPtr handle);

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

        ~btCapsuleShape()
        {
           // Do not re-create Dispose clean-up code here.
           // Calling Dispose(false) is optimal in terms of
           // readability and maintainability.
           Dispose(false);
        }

    }
}
