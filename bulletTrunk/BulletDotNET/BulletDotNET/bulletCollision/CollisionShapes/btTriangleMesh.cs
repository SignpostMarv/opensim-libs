using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BulletDotNET
{
    public class btTriangleMesh : btTriangleIndexVertexArray, IDisposable
    {
        
        public static new btTriangleMesh FromIntPtr(IntPtr handle)
        {
            return (btTriangleMesh)Native.GetObject(handle, typeof(btTriangleMesh));
        }

        public new IntPtr Handle
        {
            get { return m_handle; }
        }

        public btTriangleMesh(bool use32BitIndices, bool use4componentVertices)
        {
            m_handle = BulletAPI_CreateBtTriangleMesh(use32BitIndices, use4componentVertices);
        }

        public void addIndex(int index)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtTriangleMesh_addIndex(m_handle, index);
        }

        public void addTriangle( btVector3 vertex0, btVector3 vertex1, btVector3 vertex2, bool removeDuplicates)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtTriangleMesh_addTriangle(m_handle, vertex0.Handle, vertex1.Handle, vertex2.Handle,
                                                 removeDuplicates);
        }

        public int findOrAddVertex(btVector3 vertex, bool removeDuplicateVertices)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtTriangleMesh_findOrAddVertex(m_handle, vertex.Handle, removeDuplicateVertices);
        }

        public int getNumTriangles()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtTriangleMesh_getNumTriangles(m_handle);
        }

        public bool getUse32BitIndices()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtTriangleMesh_getUse32BitIndices(m_handle);
        }

        public bool getUse4componentVertices()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtTriangleMesh_getUse4componentVertices(m_handle);
        }


        #region Native Invokes

        [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtTriangleMesh(bool use32BitIndices, bool use4componentVerticies);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtTriangleMesh_addIndex(IntPtr handle, int index);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtTriangleMesh_addTriangle(IntPtr handle, IntPtr vertex0, IntPtr vertex1, IntPtr vertex2, bool removeDuplicateVertices);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern int BulletAPI_BtTriangleMesh_findOrAddVertex(IntPtr handle, IntPtr vertex, bool removeDuplicateVertices);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern int BulletAPI_BtTriangleMesh_getNumTriangles(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern bool BulletAPI_BtTriangleMesh_getUse32BitIndices(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity] 
        static extern bool BulletAPI_BtTriangleMesh_getUse4componentVertices(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtTriangleMesh_delete(IntPtr handle);

        #endregion


        #region IDisposable Members

        public new void Dispose()
        { 
            Dispose(true);
             GC.SuppressFinalize(this);
        }
        
        #endregion

        protected new virtual void Dispose(bool disposing)
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
                BulletAPI_BtTriangleMesh_delete(m_handle);
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

        ~btTriangleMesh()      
        {
           // Do not re-create Dispose clean-up code here.
           // Calling Dispose(false) is optimal in terms of
           // readability and maintainability.
           Dispose(false);
        }
    }
}
