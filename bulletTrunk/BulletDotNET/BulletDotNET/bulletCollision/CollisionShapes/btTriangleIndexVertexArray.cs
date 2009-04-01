using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BulletDotNET
{
    public class btTriangleIndexVertexArray : btStridingMeshInterface, IDisposable
    {
        private bool usethisdispose = false;

        public static new btTriangleIndexVertexArray FromIntPtr(IntPtr handle)
        {
            return (btTriangleIndexVertexArray)Native.GetObject(handle, typeof(btTriangleIndexVertexArray));
        }

        public new IntPtr Handle
        {
            get { return m_handle; }
        }

        protected new IntPtr m_handle
        {
            get { return base.m_handle; }
            set { base.m_handle = value; }
        }

        //Expose m_disposed of base class to derived classes while still being protected.
        protected bool m_disposed
        {
            get { return m_displosed; }
            set { m_displosed = value; }
        }

        // Don't use this one except by a base class.  You're responsible for setting m_handle!
        public btTriangleIndexVertexArray()
        {

        }

        public btTriangleIndexVertexArray(IntPtr handle)
        {
            m_handle = handle;
        }

        public btTriangleIndexVertexArray(int numTriangles, int[] triangleIndexBase,
                                                                  int triangleIndexStride, int numVerticies,
                                                                  float[] vertexBase, int vertexStride)
        {
            usethisdispose = true;
            m_handle = BulletAPI_CreateBtTriangleIndexVertexArray(numTriangles, triangleIndexBase, triangleIndexStride,
                                                                  numVerticies, vertexBase, vertexStride);
        }



        #region Native Invokes
        [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtTriangleIndexVertexArray(int numTriangles, int[] triangleIndexBase,
                                                                  int triangleIndexStride, int numVerticies,
                                                                  float[] vertexBase, int vertexStride);
        [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtTriangleIndexVertexArray_delete(IntPtr handle);
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
                BulletAPI_BtTriangleIndexVertexArray_delete(m_handle);
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

        ~btTriangleIndexVertexArray()      
        {
           // Do not re-create Dispose clean-up code here.
           // Calling Dispose(false) is optimal in terms of
           // readability and maintainability.
           Dispose(false);
        }
    }
}
