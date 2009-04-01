using System;

namespace BulletDotNET
{
    public class btDispatcher
    {
        private IntPtr m_handle;

        public IntPtr Handle
        {
            get { return m_handle; }
            set { m_handle = value; }
        }

        public btDispatcher(IntPtr handle)
        {
            m_handle = handle;
        }
        public btDispatcher()
        {
            m_handle = IntPtr.Zero;
        }
    }
}
