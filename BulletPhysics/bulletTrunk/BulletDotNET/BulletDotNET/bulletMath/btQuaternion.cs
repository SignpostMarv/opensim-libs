using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace BulletDotNET
{
    public class btQuaternion : IDisposable
    {
        private IntPtr m_handle = IntPtr.Zero;

        private bool m_disposed = false;

        public static btQuaternion FromIntPtr(IntPtr handle)
        {
            return (btQuaternion)Native.GetObject(handle, 
                typeof(btQuaternion));
        }

        public IntPtr Handle
        {
            get { return m_handle; }
        }

        public btQuaternion(IntPtr handle)
        {
            m_handle = handle;
        }

        public btQuaternion()
        {
            m_handle = BulletAPI_CreateBtQuaternion();
        }

        public btQuaternion(float x, float y, float z, float w)
        {
            m_handle = BulletAPI_CreateBtQuaternionFourFloats(x,y,z,w);
        }

        public btQuaternion(btVector3 axis, float w)
        {
            m_handle = BulletAPI_CreateBtQuaternionAxisAngle(axis.Handle, w);
        }

        public btQuaternion(float yaw, float pitch, float roll)
        {
            m_handle = BulletAPI_CreateBtQuaternionEuler(yaw,pitch,roll);
        }

        public float angle(btQuaternion q)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtQuaternion_angle(m_handle, q.Handle);
        }

        public float dot(btQuaternion q)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtQuaternion_dot(m_handle, q.Handle);
        }

        public btQuaternion farthest(btQuaternion q)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return FromIntPtr(BulletAPI_BtQuaternion_farthest(m_handle, q.Handle));
        }

        public float getAngle()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtQuaternion_getAngle(m_handle);
        }

        public btQuaternion inverse()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return FromIntPtr(BulletAPI_BtQuaternion_inverse(m_handle));
        }

        public btQuaternion getIdentity()
        {
            return new btQuaternion(0, 0, 0, 1);
        }

        public float length()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtQuaternion_length(m_handle);
        }

        public float length2()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtQuaternion_length2(m_handle);
        }

        public btQuaternion normalize()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return FromIntPtr(BulletAPI_BtQuaternion_normalize(m_handle));
        }

        public btQuaternion normalized()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return FromIntPtr(BulletAPI_BtQuaternion_normalized(m_handle));
        }

        public void setEuler(float yaw, float pitch, float roll)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtQuaternion_setEuler(m_handle, yaw, pitch, roll);
        }

        public void setEulerZYX(float yaw, float pitch, float roll)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtQuaternion_setEulerZYX(m_handle, yaw, pitch, roll);
        }

        public void setRotation(btVector3 axis, float angle)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtQuaternion_setRotation(m_handle, axis.Handle, angle);
        }

        public btQuaternion slerp(btQuaternion q, float t)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return FromIntPtr(BulletAPI_BtQuaternion_slerp(m_handle, q.Handle, t));
        }

        public string testStr()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            StringBuilder resulttxt = new StringBuilder();
            float[] vals = getValues();
            resulttxt.Append("<");
            resulttxt.Append(getX());
            resulttxt.Append(",");
            resulttxt.Append(getY());
            resulttxt.Append(",");
            resulttxt.Append(getZ());
            resulttxt.Append(",");
            resulttxt.Append(getW());
            resulttxt.Append(">");
            return resulttxt.ToString();
        }

        public float getW()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtQuaternion_getW(m_handle);
        }

        public float getX()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtQuaternion_getX(m_handle);
        }
        public float getY()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtQuaternion_getY(m_handle);
        }
        public float getZ()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtQuaternion_getZ(m_handle);
        }
        public float[] getValues()
        {
            float[] result = new float[4];

            BulletAPI_BtQuaternion_getValues(m_handle, result);
            float[] resultm = new float[4];
            int i = 0;
            resultm[i] = result[i++];
            resultm[i] = result[i++];
            resultm[i] = result[i++];
            resultm[i] = result[i];
            return resultm;
        }
        #region Native Invokes
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtQuaternion();
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtQuaternionFourFloats(float x, float y, float z, float w);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtQuaternionAxisAngle(IntPtr axis, float angle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtQuaternionEuler(float yaw, float pitch, float roll);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtQuaternion_angle(IntPtr handle, IntPtr q);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtQuaternion_dot(IntPtr handle, IntPtr q);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtQuaternion_farthest(IntPtr handle, IntPtr qd);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtQuaternion_getAngle(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtQuaternion_getW(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtQuaternion_getValues(IntPtr handle,[MarshalAs(UnmanagedType.LPArray)] float[] v);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtQuaternion_getX(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtQuaternion_getY(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtQuaternion_getZ(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtQuaternion_inverse(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtQuaternion_length(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtQuaternion_length2(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtQuaternion_normalize(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtQuaternion_normalized(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtQuaternion_setEuler(IntPtr handle, float yaw, float pitch, float roll);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtQuaternion_setEulerZYX(IntPtr handle, float yaw, float pitch, float roll);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtQuaternion_setRotation(IntPtr handle, IntPtr axis, float angle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtQuaternion_slerp(IntPtr handle, IntPtr q, float t);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtQuaternion_delete(IntPtr obj);


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
                BulletAPI_BtQuaternion_delete(m_handle);
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

        ~btQuaternion()
        {
           // Do not re-create Dispose clean-up code here.
           // Calling Dispose(false) is optimal in terms of
           // readability and maintainability.
           Dispose(false);
        }
    }
}
