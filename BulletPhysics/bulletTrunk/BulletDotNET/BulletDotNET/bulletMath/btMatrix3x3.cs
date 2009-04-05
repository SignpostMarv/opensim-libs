using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace BulletDotNET
{
    public class btMatrix3x3 : IDisposable
    {

        private IntPtr m_handle = IntPtr.Zero;

        private bool m_disposed = false;

        public static btMatrix3x3 FromIntPtr(IntPtr handle)
        {
            return (btMatrix3x3)Native.GetObject(handle, 
                typeof(btMatrix3x3));
        }

        public IntPtr Handle
        {
            get { return m_handle; }
        }

        public btMatrix3x3(IntPtr handle)
        {
            m_handle = handle;
        }

        public btMatrix3x3()
        {
            m_handle = BulletAPI_CreateBtMatrix3x3();
        }

        public btMatrix3x3(btQuaternion q)
        {
            m_handle = BulletAPI_CreateBtMatrix3x3Quaternion(q.Handle);
        }

        public btMatrix3x3(float xx, float xy, float xz, float yx, float yy, float yz, float zx, float zy, float zz)
        {
            m_handle = BulletAPI_CreateBtMatrix3x3Floats(xx, xy, xz, yx, yy, yz, zx, zy, zz);
        }

        public btMatrix3x3 absolute()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return FromIntPtr(BulletAPI_BtMatrix3x3_absolute(m_handle));
        }
        
        public btMatrix3x3 adjoint()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return FromIntPtr(BulletAPI_BtMatrix3x3_adjoint(m_handle));
        }
        
        public btMatrix3x3 inverse()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return FromIntPtr(BulletAPI_BtMatrix3x3_inverse(m_handle));
        }
        
        public btMatrix3x3 determinant()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return FromIntPtr(BulletAPI_BtMatrix3x3_determinant(m_handle));
        }

        public btVector3 getColumn(int i)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btVector3.FromIntPtr(BulletAPI_BtMatrix3x3_getColumn(m_handle, i));
        }

        public void getOpenGLSubMatrix(out float[] m)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            m = new float[12];
            BulletAPI_BtMatrix3x3_getOpenGLSubMatrix(m_handle, m);
        }

        public void setRotation(btQuaternion q)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtMatrix3x3_setRotation(m_handle, q.Handle);
        }

        #region Native Invokes
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtMatrix3x3();
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtMatrix3x3Quaternion(IntPtr q);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtMatrix3x3Floats(float xx, float xy, float xz, float yx, float yy, float yz, float zx, float zy, float zz);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtMatrix3x3Other(IntPtr m);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtMatrix3x3_absolute(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtMatrix3x3_adjoint(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtMatrix3x3_inverse(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtMatrix3x3_determinant(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtMatrix3x3_diagonalize(IntPtr handle, IntPtr rot, float threshold, int maxSteps);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtMatrix3x3_getColumn(IntPtr handle,int i);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtMatrix3x3_getOpenGLSubMatrix(IntPtr handle, [MarshalAs(UnmanagedType.LPArray)]float[] f);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtMatrix3x3_setRotation(IntPtr handle, IntPtr q);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtMatrix3x3_delete(IntPtr obj);
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
                BulletAPI_BtMatrix3x3_delete(m_handle);
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

        ~btMatrix3x3()
        {
           // Do not re-create Dispose clean-up code here.
           // Calling Dispose(false) is optimal in terms of
           // readability and maintainability.
           Dispose(false);
        }
    }
}
