using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BulletDotNET
{
    public class btRigidBody : btCollisionObject
    {

        protected btRigidBodyConstructionInfo m_constructionInfo;
        
        

        public static new btRigidBody FromIntPtr(IntPtr handle)
        {
            return (btRigidBody)Native.GetObject(handle, typeof(btRigidBody));
        }

        public btRigidBody(IntPtr handle) : base(handle)
        {
            
        }

        public btRigidBody(btRigidBodyConstructionInfo constructionInfo) : base()
        {
            m_handle = BulletAPI_CreateBtRigidBody(constructionInfo.Handle);
            m_constructionInfo = constructionInfo;
        }

        public btRigidBody(float mass, btMotionState motionState, btCollisionShape collisionShape)
        {
           
            m_constructionInfo = new btRigidBodyConstructionInfo();
            m_constructionInfo.m_collisionShape = collisionShape;
            m_constructionInfo.m_localInertia = new btVector3(0, 0, 0);
            m_constructionInfo.m_motionState = motionState;
            m_constructionInfo.m_startWorldTransform = btTransform.getIdentity();
            m_constructionInfo.SetGenericDefaultValues();
            m_constructionInfo.m_mass = mass;
            m_constructionInfo.commit();
            m_handle = BulletAPI_CreateBtRigidBody(m_constructionInfo.Handle);
        }

        public btRigidBody(float mass, btMotionState motionState, btCollisionShape collisionShape, btVector3 localInertia)
        {
            
            m_constructionInfo = new btRigidBodyConstructionInfo();
            m_constructionInfo.m_collisionShape = collisionShape;
            m_constructionInfo.m_localInertia = localInertia;
            m_constructionInfo.m_motionState = motionState;
            m_constructionInfo.m_startWorldTransform = btTransform.getIdentity();
            m_constructionInfo.SetGenericDefaultValues();
            m_constructionInfo.m_mass = mass;
            m_constructionInfo.commit();
            m_handle = BulletAPI_CreateBtRigidBody(m_constructionInfo.Handle);
         }


        public void applyCentralForce(btVector3 force)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtRigidBody_applyCentralForce(m_handle, force.Handle);
        }

        public void applyCentralImpulse(btVector3 force)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtRigidBody_applyCentralImpulse(m_handle, force.Handle);
        }

        public void applyForce(btVector3 force, btVector3 rel_pos)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtRigidBody_applyForce(m_handle, force.Handle, rel_pos.Handle);
        }

        public void applyImpulse(btVector3 force, btVector3 rel_pos)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtRigidBody_applyImpulse(m_handle, force.Handle, rel_pos.Handle);
        }

        public void applyTorque(btVector3 torque)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtRigidBody_applyTorque(m_handle, torque.Handle);
        }

        public void applyTorqueImpulse(btVector3 torque)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtRigidBody_applyTorqueImpulse(m_handle, torque.Handle);
        }

        public void clearForces()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtRigidBody_clearForces(m_handle);
        }

        public float computeAngularImpulseDenominator(btVector3 axis)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtRigidBody_computeAngularImpulseDenominator(m_handle,axis.Handle);
        }

        public float computeImpulseDenominator(btVector3 pos, btVector3 normal)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtRigidBody_computeImpulseDenominator(m_handle, pos.Handle, normal.Handle);
        }

        public float getAngularDamping()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtRigidBody_getAngularDamping(m_handle);
        }

        public btVector3 getAngularFactor()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btVector3.FromIntPtr(BulletAPI_BtRigidBody_getAngularFactor(m_handle));
        }

        public float getAngularSleepingThreshold()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtRigidBody_getAngularSleepingThreshold(m_handle);
        }

        public btVector3 getCenterOfMassPosition()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btVector3.FromIntPtr(BulletAPI_BtRigidBody_getCenterOfMassPosition(m_handle));
        }

        public btTransform getCenterOfMassTransform()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btTransform.FromIntPtr(BulletAPI_BtRigidBody_getCenterOfMassTransform(m_handle));
        }

        public btTypedConstraint getConstraintRef(int index)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btTypedConstraint.FromIntPtr(BulletAPI_BtRigidBody_getConstraintRef(m_handle, index));
        }

        public btVector3 getGravity()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btVector3.FromIntPtr(BulletAPI_BtRigidBody_getGravity(m_handle));
        }

        public btVector3 getInvInertiaDiagLocal()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btVector3.FromIntPtr(BulletAPI_BtRigidBody_getInvInertiaDiagLocal(m_handle));
        }

        public btMatrix3x3 getInvInertiaTensorWorld()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btMatrix3x3.FromIntPtr(BulletAPI_BtRigidBody_getInvInertiaTensorWorld(m_handle));
        }

        public float getInvMass()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtRigidBody_getInvMass(m_handle);
        }

        public float getLinearDamping()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtRigidBody_getLinearDamping(m_handle);
        }

        public btMotionState getMotionState()
        {
            return btMotionState.FromIntPtr(BulletAPI_BtRigidBody_getMotionState(m_handle));
        }

        public int getNumConstraintRefs()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtRigidBody_getNumConstraintRefs(m_handle);
        }

        public btVector3 getTotalForce()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btVector3.FromIntPtr(BulletAPI_BtRigidBody_getTotalForce(m_handle));
        }

        public btVector3 getVelocityInLocalPoint(btVector3 rel_pos)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btVector3.FromIntPtr(BulletAPI_BtRigidBody_getVelocityInLocalPoint(m_handle,rel_pos.Handle));
        }

        public bool isInWorld()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtRigidBody_isInWorld(m_handle);
        }

        public void setAngularFactor(float angFac)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());
 
            BulletAPI_BtRigidBody_setAngularFactor(m_handle, angFac);
        }

        public void setAngularVelocity(btVector3 ang_vel)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtRigidBody_setAngularVelocity(m_handle, ang_vel.Handle);
        }

        public void setInvInertiaDiagLocal(btVector3 diagInvInertia)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtRigidBody_setInvInertiaDiagLocal(m_handle, diagInvInertia.Handle);
        }

        public void setLinearVelocity(btVector3 lin_vel)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtRigidBody_setLinearVelocity(m_handle, lin_vel.Handle);
        }

        public void setMotionState(btMotionState motionState)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtRigidBody_setMotionState(m_handle, motionState.Handle);
        }

        public void setSleepingThresholds(float linear, float angular)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtRigidBody_setSleepingThresholds(m_handle, linear, angular);
        }

        public void translate(btVector3 v)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtRigidBody_translate(m_handle, v.Handle);
        }

        public void updateDeactivation(float timeStep)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtRigidBody_updateDeactivation(m_handle, timeStep);
        }

        #region Native Invokes

         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtRigidBody(IntPtr constructionInfo);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtRigidBodyParams(float mass, IntPtr motionState, IntPtr collisionShape, IntPtr localInertia);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtRigidBody_applyCentralForce(IntPtr handle, IntPtr force);//btVector3
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtRigidBody_applyCentralImpulse(IntPtr handle, IntPtr impulse);//btVector3
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtRigidBody_applyForce(IntPtr handle, IntPtr force, IntPtr rel_pos);//btVector3
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtRigidBody_applyImpulse(IntPtr handle, IntPtr impulse, IntPtr rel_pos);//btVector3
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtRigidBody_applyTorque(IntPtr handle, IntPtr torque);//btVector3
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtRigidBody_applyTorqueImpulse(IntPtr handle, IntPtr torque);//btVector3
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtRigidBody_clearForces(IntPtr handle);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtRigidBody_computeAngularImpulseDenominator(IntPtr handle, IntPtr axis);//btVector3
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtRigidBody_computeImpulseDenominator(IntPtr handle, IntPtr pos, IntPtr normal);//btVector3
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtRigidBody_getAngularDamping(IntPtr handle);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtRigidBody_getAngularFactor(IntPtr handle); //btVector3
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtRigidBody_getAngularSleepingThreshold(IntPtr handle);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtRigidBody_getAngularVelocity(IntPtr handle); //btVector3
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtRigidBody_getLinearSleepingThreshold(IntPtr handle);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtRigidBody_getCenterOfMassPosition(IntPtr handle); //btVector3
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtRigidBody_getCenterOfMassTransform(IntPtr handle); //btTransform
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtRigidBody_getConstraintRef(IntPtr handle, int index); //btTypedConstraint
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtRigidBody_getGravity(IntPtr handle); //btVector3
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity] 
        static extern IntPtr BulletAPI_BtRigidBody_getInvInertiaDiagLocal(IntPtr handle); //btVector3
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtRigidBody_getInvInertiaTensorWorld(IntPtr handle); //btMatrix3x3
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtRigidBody_getInvMass(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtRigidBody_getLinearDamping(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtRigidBody_getLinearVelocity(IntPtr handle);//btVector3
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtRigidBody_getMotionState(IntPtr handle);//btMotionState
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern int BulletAPI_BtRigidBody_getNumConstraintRefs(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtRigidBody_getTotalForce(IntPtr handle);//btVector3
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtRigidBody_getTotalTorque(IntPtr handle);//btVector3
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtRigidBody_getVelocityInLocalPoint(IntPtr handle, IntPtr rel_pos);//btVector3
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern bool BulletAPI_BtRigidBody_isInWorld(IntPtr handle);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtRigidBody_setAngularFactor(IntPtr handle, float angFac);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtRigidBody_setAngularVelocity(IntPtr handle, IntPtr ang_vel);//btVector3
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtRigidBody_setInvInertiaDiagLocal(IntPtr handle, IntPtr diagInvInertia);//btVector3
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtRigidBody_setLinearVelocity(IntPtr handle, IntPtr lin_vel);//btVector3
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtRigidBody_setMotionState(IntPtr handle, IntPtr motionState);//btMotionState
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtRigidBody_setNewBroadphaseProxy(IntPtr handle, IntPtr broadphaseProxy);//btbroadphaseProxy
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtRigidBody_setSleepingThresholds(IntPtr handle, float linear, float angular);
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtRigidBody_translate(IntPtr handle, IntPtr v);//btVector3
         [DllImport(Native.Dll),SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtRigidBody_updateDeactivation(IntPtr handle, float timeStep);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtRigidBody_delete(IntPtr obj);

        #endregion

         #region IDisposable Members

        public new void Dispose()
        { 
            Dispose(true);
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
                    
                    if (m_constructionInfo.Handle != IntPtr.Zero)
                    {
                        m_constructionInfo.Dispose();
                    }
                    base.Dispose(false);

                }
                // Release unmanaged resources. If disposing is false, 
                // only the following code is executed.
                BulletAPI_BtRigidBody_delete(m_handle);
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

        ~btRigidBody()
        {
           // Do not re-create Dispose clean-up code here.
           // Calling Dispose(false) is optimal in terms of
           // readability and maintainability.
           Dispose(false);
        }
    }
}
