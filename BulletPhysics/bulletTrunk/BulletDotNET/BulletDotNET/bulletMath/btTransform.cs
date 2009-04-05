using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace BulletDotNET
{
    public class btTransform : IDisposable
    {
        private IntPtr m_handle = IntPtr.Zero;

        private bool m_disposed = false;

        public static btTransform FromIntPtr(IntPtr handle)
        {
            return (btTransform)Native.GetObject(handle, 
                typeof(btTransform));
        }

        public IntPtr Handle
        {
            get { return m_handle; }
        }

        public btTransform(IntPtr handle)
        {
            m_handle = handle;
        }

        public btTransform()
        {
            m_handle = BulletAPI_CreateBtTransform();
        }

        public btTransform(btQuaternion q, btVector3 v)
        {
            m_handle = BulletAPI_CreateBtTransformQuaternionVector3(q.Handle, v.Handle);
        }

        public btTransform(btTransform t)
        {
            m_handle = BulletAPI_CreateBtTransformOtherTransform(t.Handle);
        }

        public btMatrix3x3 getBasis()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btMatrix3x3.FromIntPtr(BulletAPI_BtTransform_getBasis(m_handle));
        }

        public static btTransform getIdentity()
        {
            return new btTransform(new btQuaternion(0,0,0,1),new btVector3(0,0,0));
        }

        public void getOpenGLMatrix(out float[] m)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            m = new float[12];
            BulletAPI_BtTransform_getOpenGLMatrix(m_handle, m);
        }

        public btVector3 getOrigin()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btVector3.FromIntPtr(BulletAPI_BtTransform_getOrigin(m_handle));
        }

        public btQuaternion getRotation()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btQuaternion.FromIntPtr(BulletAPI_BtTransform_getRotation(m_handle));
        }

        public btTransform inverse()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return FromIntPtr(BulletAPI_BtTransform_inverse(m_handle));
        }

        public btTransform inverseTimes(btTransform t)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return FromIntPtr(BulletAPI_BtTransform_inverseTimes(m_handle,t.Handle));
        }

        public btTransform invXform(btVector3 inVec)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return FromIntPtr(BulletAPI_BtTransform_invXform(m_handle, inVec.Handle));
        }

        public void mult(btTransform t1, btTransform t2)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtTransform_mult(m_handle, t1.Handle, t2.Handle);
        }

        public void setBasis(btMatrix3x3 basis)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtTransform_setBasis(m_handle, basis.Handle);
        }
        
        public void setFromOpenGLMatrix(float[] m)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtTransform_setFromOpenGLMatrix(m_handle, m);
        }

        public void setOrigin(btMatrix3x3 origin)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtTransform_setOrigin(m_handle, origin.Handle);
        }

        public void setRotation(btQuaternion q)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtTransform_setRotation(m_handle, q.Handle);
        }

        #region Native Invokes
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr  BulletAPI_CreateBtTransform();
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
         static extern IntPtr BulletAPI_CreateBtTransformQuaternionVector3(IntPtr q, IntPtr v);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtTransformMatrix3Vector3(IntPtr m, IntPtr v);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtTransformOtherTransform(IntPtr t);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtTransform_getBasis(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtTransform_getIdentity(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtTransform_getOpenGLMatrix(IntPtr handle, [MarshalAs(UnmanagedType.LPArray)] float[] m); // float array!
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtTransform_getOrigin(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtTransform_getRotation(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtTransform_inverse(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtTransform_inverseTimes(IntPtr handle, IntPtr t);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtTransform_invXform(IntPtr handle, IntPtr inVec); // // btVector3
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtTransform_mult(IntPtr handle, IntPtr t1, IntPtr t2);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtTransform_setBasis(IntPtr handle, IntPtr basis);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtTransform_setFromOpenGLMatrix(IntPtr handle, [MarshalAs(UnmanagedType.LPArray)] float[] m);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtTransform_setOrigin(IntPtr handle, IntPtr origin);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtTransform_setRotation(IntPtr handle, IntPtr q);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtTransform_delete(IntPtr handle);
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
                try
                {
                    BulletAPI_BtTransform_delete(m_handle);
                }
                catch (AccessViolationException)
                {
                }
               
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

        ~btTransform()
        {
           // Do not re-create Dispose clean-up code here.
           // Calling Dispose(false) is optimal in terms of
           // readability and maintainability.
           Dispose(false);
        }
    }
}
