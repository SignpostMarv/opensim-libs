using System;

namespace BulletDotNET
{
    public class btRayResultCallback
    {
        protected IntPtr m_handle;
        protected bool m_disposed;

        public IntPtr Handle
        {
            get { return m_handle; }
        }
    }
}
