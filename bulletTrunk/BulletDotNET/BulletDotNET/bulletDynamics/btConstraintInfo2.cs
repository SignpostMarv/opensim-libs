using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace BulletDotNET
{
    public class btConstraintInfo2 : IDisposable
    {
        protected IntPtr m_handle = IntPtr.Zero;

        protected bool m_disposed = false;

        public static btConstraintInfo2 FromIntPtr(IntPtr handle)
        {
            return (btConstraintInfo2)Native.GetObject(handle, 
                typeof(btConstraintInfo2));
        }

        public IntPtr Handle
        {
            get { return m_handle; }
        }

        public btConstraintInfo2(IntPtr handle)
        {
            m_handle = handle;
        }

        public btConstraintInfo2(float fps, float erp, 
        float m_J1linearAxis,float m_J1angularAxis,float m_J2linearAxis,float m_J2angularAxis,
        int rowskip, float constraintError, float cfm, float lowerLimit, float upperLimit, float index)
        {
            m_handle = BulletAPI_BtTypedConstraint_CreateBtConstraintInfo2(fps, erp, m_J1linearAxis, m_J1angularAxis,
                                                                m_J2linearAxis, m_J2angularAxis, rowskip,
                                                                constraintError, cfm, lowerLimit, 
                                                                upperLimit, index);
        }

         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtTypedConstraint_CreateBtConstraintInfo2(float fps, float erp, 
        float m_J1linearAxis,float m_J1angularAxis,float m_J2linearAxis,float m_J2angularAxis,
        int rowskip, float constraintError, float cfm, float lowerLimit, float upperLimit, float index);

         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtTypedConstraint_BtConstraintInfo2_delete(IntPtr handle);


        #region IDisposable Members

        public void Dispose()
        { 
            Dispose(true);
            GC.SuppressFinalize(this);
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
                    // none yet
                    
                   
                }
                // Release unmanaged resources. If disposing is false, 
                // only the following code is executed.
                BulletAPI_BtTypedConstraint_BtConstraintInfo2_delete(m_handle);
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

        ~btConstraintInfo2()
        {
           // Do not re-create Dispose clean-up code here.
           // Calling Dispose(false) is optimal in terms of
           // readability and maintainability.
           Dispose(false);
        }
    }
}
