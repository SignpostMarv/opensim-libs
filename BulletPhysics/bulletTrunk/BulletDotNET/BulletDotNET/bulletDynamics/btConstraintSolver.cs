using System;


namespace BulletDotNET
{
    public class btConstraintSolver
    {
        private IntPtr m_handle;

        public IntPtr Handle
        {
            get { return m_handle; }
            set { m_handle = value; }
        }

        public btConstraintSolver(IntPtr handle)
        {
            m_handle = handle;
        }
        public btConstraintSolver()
        {
            m_handle = IntPtr.Zero;
        }
    }
}
