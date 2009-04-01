using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BulletDotNET
{
    public class btCollisionShape : IDisposable
    {
        protected IntPtr m_handle = IntPtr.Zero;

        protected bool m_disposed = false;

        public static btCollisionShape FromIntPtr(IntPtr handle)
        {
            return (btCollisionShape)Native.GetObject(handle, 
                typeof(btCollisionShape));
        }

        public IntPtr Handle
        {
            get { return m_handle; }
        }

        public btCollisionShape(IntPtr handle)
        {
            m_handle = handle;
        }

        public btCollisionShape()
        {
            m_handle = BulletAPI_CreateBtCollisionShape();
        }

        public void calculateLocalInertia(float mass, btVector3 inertia)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());
            BulletAPI_BtCollisionShape_calculateLocalInertia(m_handle, mass, inertia.Handle);
        }

        public float getAngularMotionDisk()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionShape_getAngularMotionDisk(m_handle);
        }

        public btVector3 getLocalScaling()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());
            return btVector3.FromIntPtr(BulletAPI_BtCollisionShape_getLocalScaling(m_handle));
        }

        public float getContactBreakingThreshold()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionShape_getContactBreakingThreshold(m_handle);
        }

        public float getMargin()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionShape_getMargin(m_handle);
        }

        public int getShapeType()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionShape_getShapeType(m_handle);
        }

        public IntPtr getUserPointer()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionShape_getUserPointer(m_handle);
        }

        public bool isCompound()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionShape_isCompound(m_handle);
        }

        public bool isConcave()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionShape_isConcave(m_handle);
        }

        public bool isConvex()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionShape_isConvex(m_handle);
        }

        public bool isInfinite()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionShape_isInfinite(m_handle);
        }

        public bool isPolyhedral()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionShape_isPolyhedral(m_handle);
        }

        public void setLocalScaling(btVector3 scaling)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());
            
            BulletAPI_BtCollisionShape_setLocalScaling(m_handle, scaling.Handle);
        }

        public void setMargin(float scaling)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionShape_setMargin(m_handle, scaling);
        }

        public void setUserPointer(IntPtr ptr)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionShape_setUserPointer(m_handle, ptr);
        }

        #region Native Invokes
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtCollisionShape();
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionShape_calculateLocalInertia(IntPtr handle, float mass, IntPtr inertia);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtCollisionShape_getAngularMotionDisk(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtCollisionShape_getLocalScaling(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtCollisionShape_getContactBreakingThreshold(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtCollisionShape_getMargin(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern int BulletAPI_BtCollisionShape_getShapeType(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtCollisionShape_getUserPointer(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern bool BulletAPI_BtCollisionShape_isCompound(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern bool BulletAPI_BtCollisionShape_isConcave(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern bool BulletAPI_BtCollisionShape_isConvex(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern bool BulletAPI_BtCollisionShape_isInfinite(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern bool BulletAPI_BtCollisionShape_isPolyhedral(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionShape_setLocalScaling(IntPtr handle, IntPtr scaling);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionShape_setMargin(IntPtr handle, float margin);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionShape_setUserPointer(IntPtr handle, IntPtr ptr);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionShape_delete(IntPtr obj);

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
                    // If we deliberately called Dispose, then call the virtual dispose
                    // If a derived class was disposed, then let it handle the unmanaged memory
                    BulletAPI_BtCollisionShape_delete(m_handle);
                }
                // Release unmanaged resources. If disposing is false, 
                // only the following code is executed.
                //
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

        ~btCollisionShape()
        {
           // Do not re-create Dispose clean-up code here.
           // Calling Dispose(false) is optimal in terms of
           // readability and maintainability.
           Dispose(false);
        }
    }
}
