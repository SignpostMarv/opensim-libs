using System;
using System.Runtime.InteropServices;
using System.Security;


namespace BulletDotNET
{
    public class btGeneric6DofConstraint : btTypedConstraint, IDisposable
    {

        public static new btGeneric6DofConstraint FromIntPtr(IntPtr handle)
        {
            return (btGeneric6DofConstraint)Native.GetObject(handle, typeof(btGeneric6DofConstraint));
        }

        public btGeneric6DofConstraint(IntPtr handle)
            : base(handle)
        {

        }

        public btGeneric6DofConstraint(btRigidBody rbA, btRigidBody rbB, btTransform frameInA, btTransform frameInB, bool useLinearReferenceFrameA)
            : base()
        {
            m_handle = BulletAPI_CreateBtGeneric6DofConstraint(rbA.Handle, rbB.Handle, frameInA.Handle, frameInB.Handle, useLinearReferenceFrameA);
        }

        public void setLinearUpperLimit(btVector3 linearUpper)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtGeneric6DofConstraint_setLinearUpperLimit(m_handle, linearUpper.Handle);
        }
        public void setLinearLowerLimit(btVector3 linearLower)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtGeneric6DofConstraint_setLinearLowerLimit(m_handle, linearLower.Handle);
        }

        public void setAngularUpperLimit(btVector3 linearUpper)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtGeneric6DofConstraint_setAngularUpperLimit(m_handle, linearUpper.Handle);
        }

        public void setAngularLowerLimit(btVector3 linearLower)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtGeneric6DofConstraint_setAngularLowerLimit(m_handle, linearLower.Handle);
        }

        public void setLimits(int axis, float lo, float hi)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtGeneric6DofConstraint_setLimits(m_handle, axis, lo, hi);
        }

        public bool isLimted(int limitIndex)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtGeneric6DofConstraint_isLimited(m_handle, limitIndex);
        }

         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtGeneric6DofConstraint(IntPtr rbA, IntPtr rbB, IntPtr frameInA, IntPtr frameInB, bool useLinearReferenceFrameA);//btRigidBody/btTranform
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtGeneric6DofConstraint_delete(IntPtr handle);//btVector3
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtGeneric6DofConstraint_setLinearUpperLimit(IntPtr handle, IntPtr linearUpper);//btVector3
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtGeneric6DofConstraint_setLinearLowerLimit(IntPtr handle, IntPtr linearLower);//btMotionState
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtGeneric6DofConstraint_setAngularUpperLimit(IntPtr handle, IntPtr angularUpper);//btbroadphaseProxy
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtGeneric6DofConstraint_setAngularLowerLimit(IntPtr handle, IntPtr angularLower);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern bool BulletAPI_BtGeneric6DofConstraint_isLimited(IntPtr handle, int limitIndex);//btVector3
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtGeneric6DofConstraint_setLimits(IntPtr handle, int axis, float lo, float hi);

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
                BulletAPI_BtGeneric6DofConstraint_delete(m_handle);
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

        ~btGeneric6DofConstraint()
        {
           // Do not re-create Dispose clean-up code here.
           // Calling Dispose(false) is optimal in terms of
           // readability and maintainability.
           Dispose(false);
        }
    }
}
