using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BulletDotNET
{
    public class btTypedConstraint
    {
        protected IntPtr m_handle = IntPtr.Zero;

        protected bool m_disposed = false;

        public static btTypedConstraint FromIntPtr(IntPtr handle)
        {
            return (btTypedConstraint)Native.GetObject(handle,
                typeof(btTypedConstraint));
        }

        public IntPtr Handle
        {
            get { return m_handle; }   
        }

        public btTypedConstraint(IntPtr handle)
        {
            m_handle = handle;
        }

        /// <summary>
        /// Don't use this!
        /// purely for classes derived from this
        /// </summary>
        public btTypedConstraint()
        {
            
        }

        public void getInfo2(ref btConstraintInfo2 b)
        {
            BulletAPI_BtTypedConstraint_getInfo2(m_handle, b.Handle);
        }

        [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtTypedConstraint_getInfo2(IntPtr handle, IntPtr b);
    }
}
