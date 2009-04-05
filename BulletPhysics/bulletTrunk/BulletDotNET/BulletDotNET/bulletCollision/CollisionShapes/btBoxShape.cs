using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BulletDotNET
{
    public class btBoxShape : btCollisionShape, IDisposable
    {
        public static new btBoxShape FromIntPtr(IntPtr handle)
        {
            return (btBoxShape)Native.GetObject(handle, typeof(btBoxShape));
        }

        public btBoxShape(IntPtr handle)
            : base(handle)
        {

        }

        public btBoxShape(btVector3 boxHalfExtents)
            : base()
        {
            m_handle = BulletAPI_CreateBtBoxShape(boxHalfExtents.Handle);
        }

        public btVector3 getHalfExtentsWithMargin()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btVector3.FromIntPtr(BulletAPI_BtBoxShape_getHalfExtentsWithMargin(m_handle));
        }

        public btVector3 getHalfExtentsWithoutMargin()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btVector3.FromIntPtr(BulletAPI_BtBoxShape_getHalfExtentsWithoutMargin(m_handle));
        }

        public static int getNumPlanes()
        {
            return 6;
        }

        public static int getNumVerticies()
        {
            return 8;
        }

        public static int getNumEdges()
        {
            return 12;
        }

        public btVector3 getVertex(int i)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btVector3.FromIntPtr(BulletAPI_BtBoxShape_getVertex(m_handle, i));
        }

        public bool isInside(btVector3 pt, float tolerance)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtBoxShape_isInside(m_handle, pt.Handle, tolerance);
        }

        public void setMargins(float collisionMargin)
        {
            BulletAPI_BtBoxShape_setMargins(m_handle,collisionMargin);
        }

        #region Native Invokes

        [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtBoxShape(IntPtr boxHalfExtents);
        [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern  IntPtr BulletAPI_BtBoxShape_getHalfExtentsWithMargin(IntPtr handle);
        [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtBoxShape_getHalfExtentsWithoutMargin(IntPtr handle);
        [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtBoxShape_getVertex(IntPtr handle, int i);
        [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern bool BulletAPI_BtBoxShape_isInside(IntPtr handle, IntPtr pt, float tolerance);
        [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtBoxShape_setLocalScaling(IntPtr handle, IntPtr scaling);
        [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtBoxShape_setMargins(IntPtr handle, float collisionMargin);
        [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtBoxShape_delete(IntPtr handle);


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

        ~btBoxShape()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

    }
}
