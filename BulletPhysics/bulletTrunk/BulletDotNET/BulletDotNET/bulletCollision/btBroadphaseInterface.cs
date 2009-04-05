using System;


namespace BulletDotNET
{
    public class btBroadphaseInterface
    {
        private IntPtr m_handle;

        public IntPtr Handle
        {
            get { return m_handle; }
            set { m_handle = value; }
        }

        public btBroadphaseInterface(IntPtr handle)
        {
            m_handle = handle;
        }
        public btBroadphaseInterface()
        {
            m_handle = IntPtr.Zero;
        }
    }
}
