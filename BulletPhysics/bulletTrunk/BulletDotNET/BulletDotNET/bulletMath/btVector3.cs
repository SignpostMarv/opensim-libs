using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace BulletDotNET
{
    public class btVector3 : IDisposable
    {
        private IntPtr m_handle = IntPtr.Zero;

        private bool m_disposed = false;

        public static btVector3 FromIntPtr(IntPtr handle)
        {
            return (btVector3)Native.GetObject(handle, typeof(btVector3));
        }

        public IntPtr Handle
        {
            get { return m_handle; }
        }

        public btVector3(float x, float y, float z)
        {
            m_handle = BulletAPI_CreateBtVector3(x, y, z);
        }

        public btVector3(IntPtr handle)
        {
            m_handle = handle;
        }

        public btVector3 absolute()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());
            return FromIntPtr(BulletAPI_BtVector3_absolute(m_handle));
        }

        public float angle(btVector3 v)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());
            return BulletAPI_BtVector3_angle(m_handle, v.Handle);
        }
            
        public int closestAxis()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());
            return BulletAPI_BtVector3_closestAxis(m_handle);
        }

        public btVector3 cross(btVector3 v)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());
            return FromIntPtr(BulletAPI_BtVector3_cross(m_handle, v.Handle));
        }

        public float distance(btVector3 v)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());
            return BulletAPI_BtVector3_distance(m_handle, v.Handle);
        }

        public float distance2(btVector3 v)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());
            return BulletAPI_BtVector3_distance2(m_handle, v.Handle);
        }

        public float dot(btVector3 v)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());
            return BulletAPI_BtVector3_dot(m_handle, v.Handle);
        }

        public int furthestAxis()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());
            return BulletAPI_BtVector3_furthestAxis(m_handle);
        }

        public float getX()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());
            return BulletAPI_BtVector3_GetX(m_handle);
        }

        public float getY()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());
            return BulletAPI_BtVector3_GetY(m_handle);
        }

        public float getZ()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());
            return BulletAPI_BtVector3_GetZ(m_handle);
        }

        public btVector3 normalized()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());
            return FromIntPtr(BulletAPI_BtVector3_normalized(m_handle));
        }

        public void setInterpolate3(btVector3 v0, btVector3 v1, float rt)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());
            BulletAPI_BtVector3_setInterpolate3(m_handle, v0.Handle, v1.Handle, rt);
        }

        public void setValue(float x, float y, float z)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());
            BulletAPI_BtVector3_setValue(m_handle, x, y, z);
        }

        public void setX(float x)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());
            BulletAPI_BtVector3_setX(m_handle, x);
        }

        public void setY(float y)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());
            BulletAPI_BtVector3_setY(m_handle, y);
        }

        public void setZ(float z)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());
            BulletAPI_BtVector3_setZ(m_handle, z);
        }


        public string testStr()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            StringBuilder resulttxt = new StringBuilder();
            resulttxt.Append("<");
            resulttxt.Append(getX());
            resulttxt.Append(",");
            resulttxt.Append(getY());
            resulttxt.Append(",");
            resulttxt.Append(getZ());
            resulttxt.Append(">");
            return resulttxt.ToString();
        }

        #region Native Invokes

         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtVector3(float x, float y, float z);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtVector3_absolute(IntPtr umv);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtVector3_angle(IntPtr umv, IntPtr v);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern int BulletAPI_BtVector3_closestAxis(IntPtr umv);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtVector3_cross(IntPtr umv, IntPtr v);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtVector3_distance(IntPtr umv, IntPtr v);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtVector3_distance2(IntPtr umv, IntPtr v);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtVector3_dot(IntPtr umv, IntPtr v);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern int BulletAPI_BtVector3_furthestAxis(IntPtr umv);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtVector3_GetX(IntPtr umv);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtVector3_GetY(IntPtr umv);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtVector3_GetZ(IntPtr umv);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtVector3_normalized(IntPtr umv);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtVector3_setInterpolate3(IntPtr umv, IntPtr v0, IntPtr v1, float rt);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtVector3_setValue(IntPtr umv, float x, float y, float z);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtVector3_setX(IntPtr umv, float x);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtVector3_setY(IntPtr umv, float y);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtVector3_setZ(IntPtr umv, float z);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtVector3_delete(IntPtr umv);

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
                BulletAPI_BtVector3_delete(m_handle);
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

        ~btVector3()      
        {
           // Do not re-create Dispose clean-up code here.
           // Calling Dispose(false) is optimal in terms of
           // readability and maintainability.
           Dispose(false);
        }


    }
}
