using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BulletDotNET
{

    public class btRigidBodyConstructionInfo : IDisposable
    {
        public float m_mass;

        ///When a motionState is provided, the rigid body will initialize its world transform from the motion state
        ///In this case, m_startWorldTransform is ignored.
        public btMotionState m_motionState;
        public btTransform m_startWorldTransform;
        public btCollisionShape m_collisionShape;
        public btVector3 m_localInertia;
        public float m_linearDamping;
        public float m_angularDamping;

        public float m_friction;
        ///best simulation results when friction is non-zero
        public float m_restitution;
        public float m_linearSleepingThreshold;
        public float m_angularSleepingThreshold;

        //Additional damping can help avoiding lowpass jitter motion, help stability for ragdolls etc.
        //Such damping is undesirable, so once the overall simulation quality of the rigid body dynamics system has improved, this should become obsolete
        public bool m_additionalDamping;
        public float m_additionalDampingFactor;
        public float m_additionalLinearDampingThresholdSqr;
        public float m_additionalAngularDampingThresholdSqr;
        public float m_additionalAngularDampingFactor;

        protected IntPtr m_handle = IntPtr.Zero;

        protected bool m_disposed = false;

        public static btMotionState FromIntPtr(IntPtr handle)
        {
            return (btMotionState)Native.GetObject(handle, typeof(btMotionState));
        }

        public IntPtr Handle
        {
            get { return m_handle; }
        }

        public void commit()
        {
            
           BulletAPI_BtRigidBodyConstructionInfo_Commit(m_handle,
             m_mass, m_motionState.Handle, m_startWorldTransform.Handle, m_collisionShape.Handle,
             m_localInertia.Handle, m_linearDamping, m_angularDamping, m_friction,
             m_restitution, m_linearSleepingThreshold, m_angularSleepingThreshold,
             m_additionalDamping, m_additionalDampingFactor,
             m_additionalLinearDampingThresholdSqr, m_additionalAngularDampingThresholdSqr,
             m_additionalAngularDampingFactor);
        }

        public btRigidBodyConstructionInfo()
        {
            m_handle = BulletAPI_CreateBtRigidBodyConstructionInfo();
        }

        public void SetGenericDefaultValues()
        {
            m_linearDamping = 0;
            m_angularDamping = 0;
            m_friction = 0.5f;
            m_restitution = 0;
            m_linearSleepingThreshold = 0.8f;
            m_angularSleepingThreshold = 1;
            m_additionalDamping = false;
            m_additionalDampingFactor = 0.005f;
            m_additionalLinearDampingThresholdSqr = 0.01f;
            m_additionalAngularDampingThresholdSqr = 0.01f;
            m_additionalAngularDampingFactor = 0.01f;
        }

        public btRigidBodyConstructionInfo(IntPtr handle)
        {
            m_handle = handle;
        }

         #region Native Invokes

         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtRigidBodyConstructionInfo();
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtRigidBodyConstructionInfo_Commit(IntPtr constructionInfoloc,
             float mass, IntPtr motionState, IntPtr startWorldTransform, IntPtr collisionShape,
             IntPtr localInertia, float linearDamping, float angularDamping, float friction,
             float restitution, float linearSleepingThreshold, float angularSleepingThreshold,
             bool additionalDamping, float additionalDampingFactor,
             float additionalLinearDampingThresholdSqr, float additionalAngularDampingThresholdSqr,
             float additionalAngularDampingFactor);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtRigidBodyConstructionInfo_delete(IntPtr obj);
       
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
                BulletAPI_BtRigidBodyConstructionInfo_delete(m_handle);
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

        ~btRigidBodyConstructionInfo()
        {
           // Do not re-create Dispose clean-up code here.
           // Calling Dispose(false) is optimal in terms of
           // readability and maintainability.
           Dispose(false);
        }
    
    }
}
