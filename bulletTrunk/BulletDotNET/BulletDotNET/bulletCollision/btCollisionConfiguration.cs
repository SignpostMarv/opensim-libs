using System;
using System.Collections.Generic;
using System.Text;

namespace BulletDotNET
{
    public class btCollisionConfiguration
    {
        private IntPtr m_handle;

        public IntPtr Handle
        {
            get { return m_handle; }
            set { m_handle = value; }
        }

        public btCollisionConfiguration(IntPtr handle)
        {
            m_handle = handle;
        }
        public btCollisionConfiguration()
        {
            m_handle = IntPtr.Zero;
        }
    }
}
