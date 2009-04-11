using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BulletDotNET
{
    public class btDiscreteDynamicsWorld : btCollisionWorld, IDisposable
    {
       

        protected static btDiscreteDynamicsWorld FromIntPtr(IntPtr handle)
        {
            return (btDiscreteDynamicsWorld)Native.GetObject(handle, 
                typeof(btDiscreteDynamicsWorld));
        }

        public IntPtr Handle
        {
            get { return m_handle; }
        }

        public btDiscreteDynamicsWorld(IntPtr handle) : base(handle)
        {
            
        }

        public btDiscreteDynamicsWorld(btDispatcher dispatcher, btBroadphaseInterface broadphase, btConstraintSolver solver, btCollisionConfiguration collisionconfig)
        {
            m_handle = BulletAPI_CreateBtDynamicsWorld(dispatcher.Handle, broadphase.Handle, solver.Handle, collisionconfig.Handle);
        }

        public void setGravity(btVector3 v)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtDynamicsWorld_setGravity(m_handle,v.Handle);
        }

        public int stepSimulation(float timeStep, int maxSubSteps, float fixedTimeStep)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtDynamicsWorld_stepSimulation(m_handle, timeStep, maxSubSteps, fixedTimeStep);
        }

        public void addRigidBody(btRigidBody body)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtDynamicsWorld_addRigidBody(m_handle, body.Handle);
        }

        public void removeRigidBody(btRigidBody body)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtDynamicsWorld_removeRigidBody(m_handle, body.Handle);
        }


        public void removeConstraint(btTypedConstraint pconstraint)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            if (pconstraint.Handle == IntPtr.Zero)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtDynamicsWorld_removeConstraint(m_handle, pconstraint.Handle);
        }

        public void addConstraint(btTypedConstraint pconstraint)
        {
            addConstraint(pconstraint, false);
        }

        public void addConstraint(btTypedConstraint pconstraint, bool disableLinkedCollisions)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            if (pconstraint.Handle == IntPtr.Zero)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtDynamicsWorld_addConstraint(m_handle, pconstraint.Handle, disableLinkedCollisions);
        }


        #region Native Invokes
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtDynamicsWorld(IntPtr dispatcher, IntPtr broadphase, IntPtr solver, IntPtr collisionconfig);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtDynamicsWorld_Specific(IntPtr dispatcher, IntPtr broadphase, IntPtr solver, IntPtr collisionconfig);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtDynamicsWorld_addRigidBody(IntPtr handle, IntPtr body);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtDynamicsWorld_removeRigidBody(IntPtr handle, IntPtr body);

         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
         static extern void BulletAPI_BtDynamicsWorld_addConstraint(IntPtr dynamicsWorld, IntPtr constraint, bool disableLinkedCollisions);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtDynamicsWorld_setGravity(IntPtr dynamicsWorld, IntPtr v);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern int BulletAPI_BtDynamicsWorld_stepSimulation(IntPtr dynamicsWorld, float timeStep, int maxSubSteps,
                                                             float fixedTimeStep);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
         static extern void BulletAPI_BtDynamicsWorld_removeConstraint(IntPtr dynamicsWorld, IntPtr constraint);

         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtDynamicsWorld_delete(IntPtr obj);
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
                BulletAPI_BtDynamicsWorld_delete(m_handle);
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

        ~btDiscreteDynamicsWorld()
        {
           // Do not re-create Dispose clean-up code here.
           // Calling Dispose(false) is optimal in terms of
           // readability and maintainability.
           Dispose(false);
        }
    }
}
