using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace BulletDotNET
{
    public class btCollisionObject : IDisposable
    {
        protected IntPtr m_handle = IntPtr.Zero;

        protected bool m_disposed = false;

        public static btCollisionObject FromIntPtr(IntPtr handle)
        {
            return (btCollisionObject)Native.GetObject(handle,
                typeof(btCollisionObject));
        }

        public IntPtr Handle
        {
            get { return m_handle; }
        }

        public btCollisionObject(IntPtr handle)
        {
            m_handle = handle;
        }

        public btCollisionObject()
        {
            //m_handle = BulletAPI_CreateBtCollisionObject();
        }

        public void activate(bool forceActivate)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionObject_activate(m_handle, forceActivate);
        }

        public bool checkCollideWith(btCollisionObject collisionObject)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionObject_checkCollideWith(m_handle, collisionObject.Handle);
        }

        public void forceActivationState(int newState)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionObject_forceActivationState(m_handle, newState);
        }

        public int getActivationState()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionObject_getActivationState(m_handle);            
        }

        public btVector3 getAnisotropicFriction()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btVector3.FromIntPtr(BulletAPI_BtCollisionObject_getAnisotropicFriction(m_handle));
        }

        // TODO: FIXME: create btBroadPhaseProxy object
        private IntPtr getBroadPhaseHandle()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionObject_getBroadphaseHandle(m_handle);
        }

        public float getCcdMotionThreshold()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionObject_getCcdMotionThreshold(m_handle);
        }

        public float getCcdSquareMotionThreshold()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionObject_getCcdSquareMotionThreshold(m_handle);
        }

        public float getCcdSweptSphereRadius()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionObject_getCcdSweptSphereRadius(m_handle);
        }

        public int getCollisionFlags()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionObject_getCollisionFlags(m_handle);
        }

        public btCollisionShape getCollisionShape()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btCollisionShape.FromIntPtr(BulletAPI_BtCollisionObject_getCollisionShape(m_handle));
        }

        public int getCompanionId()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionObject_getCompanionId(m_handle);
        }

        public float getContactProcessingThreshold()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionObject_getContactProcessingThreshold(m_handle);
        }

        public float getDeactivationTime()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionObject_getDeactivationTime(m_handle);
        }

        public float getFriction()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionObject_getFriction(m_handle);
        }

        public float getHitFraction()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionObject_getHitFraction(m_handle);
        }

        public btVector3 getInterpolationAngularVelocity()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btVector3.FromIntPtr(BulletAPI_BtCollisionObject_getInterpolationAngularVelocity(m_handle));
        }

        public btVector3 getInterpolationLinearVelocity()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btVector3.FromIntPtr(BulletAPI_BtCollisionObject_getInterpolationLinearVelocity(m_handle));
        }

        public btTransform getInterpolationWorldTransform()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btTransform.FromIntPtr(BulletAPI_BtCollisionObject_getInterpolationWorldTransform(m_handle));
        }

        public int getIslandTag()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionObject_getIslandTag(m_handle);
        }

        public float getRestitution()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionObject_getRestitution(m_handle);
        }
        
        public btCollisionShape getRootCollisionShape()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btCollisionShape.FromIntPtr(BulletAPI_BtCollisionObject_getRootCollisionShape(m_handle));
        }

        public IntPtr getUserPointer()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionObject_getUserPointer(m_handle);
        }

        public btTransform getWorldTransform()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btTransform.FromIntPtr(BulletAPI_BtCollisionObject_getWorldTransform(m_handle));
        }

        public bool hasAnisotropicFriction()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionObject_hasAnisotropicFriction(m_handle);
        }

        public bool hasContactResponse()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionObject_hasContactResponse(m_handle);
        }

        public bool isActive()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionObject_isActive(m_handle);
        }

        public bool isKinematicObject()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionObject_isKinematicObject(m_handle);
        }

        public bool isStaticObject()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionObject_isStaticObject(m_handle);
        }

        public bool isStaticOrKinematicObject()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionObject_isStaticOrKinematicObject(m_handle);
        }

        public bool mergesSimulationIslands()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_BtCollisionObject_mergesSimulationIslands(m_handle);
        }


        public void setActivationState(int newState)
        {
             if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionObject_setActivationState(m_handle, newState);
        }


        public void setAnisotropicFriction(btVector3 anisotropicFriction)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionObject_setAnisotropicFriction(m_handle, anisotropicFriction.Handle);
        }

        // TODO: FIXME: add btBroadPhaseProxyObject
        private void setBroadphaseHandle(IntPtr bphandle)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionObject_setBroadphaseHandle(m_handle, bphandle);
        }

        public void setCcdMotionThreshold(float ccdMotionThreshold)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionObject_setCcdMotionThreshold(m_handle, ccdMotionThreshold);
        }

        public void setCcdSweptSphereRadius(float radius)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionObject_setCcdSweptSphereRadius(m_handle, radius);
        }

        public void setCollisionFlags(int flags)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionObject_setCollisionFlags(m_handle, flags);
        }

        public void setCollisionShape(btCollisionShape collisionShape)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionObject_setCollisionShape(m_handle, collisionShape.Handle);
        }

        public void setCompanionId(int id)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionObject_setCompanionId(m_handle, id);
        }

        public void setContactProcessingThreshold(float contactProcessingThreshold)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionObject_setContactProcessingThreshold(m_handle, contactProcessingThreshold);
        }

        public void setDeactivationTime(float time)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionObject_setDeactivationTime(m_handle, time);
        }

        public void setFriction(float frict)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionObject_setFriction(m_handle, frict);
        }

        public void setHitFraction(float hitFraction)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionObject_setHitFraction(m_handle, hitFraction);
        }

        public void setInterpolationAngularVelocity(btVector3 angvel)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionObject_setInterpolationAngularVelocity(m_handle, angvel.Handle);
        }

        public void setInterpolationLinearVelocity(btVector3 linvel)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionObject_setInterpolationLinearVelocity(m_handle, linvel.Handle);
        }

        public void setInterpolationWorldTransform(btTransform trans)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionObject_setInterpolationWorldTransform(m_handle, trans.Handle);
        }

        public void setIslandTag(int tag)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionObject_setIslandTag(m_handle, tag);
        }

        public void setRestitution(float rest)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionObject_setRestitution(m_handle, rest);
        }

        public void setUserPointer(IntPtr userPointer)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionObject_setUserPointer(m_handle, userPointer);
        }

        public void setWorldTransform(btTransform worldTrans)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_BtCollisionObject_setWorldTransform(m_handle, worldTrans.Handle);
        }


        #region Native Invokes
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_CreateBtCollisionObject();
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionObject_activate(IntPtr handle, bool forceActivate);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
         static extern bool BulletAPI_BtCollisionObject_checkCollideWith(IntPtr handle, IntPtr collisionObject);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionObject_forceActivationState(IntPtr handle, int newState);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern int BulletAPI_BtCollisionObject_getActivationState(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtCollisionObject_getAnisotropicFriction(IntPtr handle);//btVector3
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtCollisionObject_getBroadphaseHandle(IntPtr handle); // btBroadPhaseProxy
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtCollisionObject_getCcdMotionThreshold(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtCollisionObject_getCcdSquareMotionThreshold(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtCollisionObject_getCcdSweptSphereRadius(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern int BulletAPI_BtCollisionObject_getCollisionFlags(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtCollisionObject_getCollisionShape(IntPtr handle); //btCollisionShape
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern int BulletAPI_BtCollisionObject_getCompanionId(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtCollisionObject_getContactProcessingThreshold(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtCollisionObject_getDeactivationTime(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtCollisionObject_getFriction(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtCollisionObject_getHitFraction(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtCollisionObject_getInterpolationAngularVelocity(IntPtr handle);//btVector3
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtCollisionObject_getInterpolationLinearVelocity(IntPtr handle);//btVector3
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtCollisionObject_getInterpolationWorldTransform(IntPtr handle);//btTransform
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern int BulletAPI_BtCollisionObject_getIslandTag(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern float BulletAPI_BtCollisionObject_getRestitution(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtCollisionObject_getRootCollisionShape(IntPtr handle); //btCollisionShape
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtCollisionObject_getUserPointer(IntPtr handle); // pointer
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_BtCollisionObject_getWorldTransform(IntPtr handle); //btTransform
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern bool BulletAPI_BtCollisionObject_hasAnisotropicFriction(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern bool BulletAPI_BtCollisionObject_hasContactResponse(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern bool BulletAPI_BtCollisionObject_isActive(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern bool BulletAPI_BtCollisionObject_isKinematicObject(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern bool BulletAPI_BtCollisionObject_isStaticObject(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern bool BulletAPI_BtCollisionObject_isStaticOrKinematicObject(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern bool BulletAPI_BtCollisionObject_mergesSimulationIslands(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionObject_setActivationState(IntPtr handle, int newState);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionObject_setAnisotropicFriction(IntPtr handle, IntPtr anisotropicFriction); // anisotropicFriction btVector3
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionObject_setBroadphaseHandle(IntPtr handle, IntPtr bphandle);  //btBroadphaseProxy
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionObject_setCcdMotionThreshold(IntPtr handle, float ccdMotionThreshold);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionObject_setCcdSweptSphereRadius(IntPtr handle, float radius);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionObject_setCollisionFlags(IntPtr handle, int flags);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionObject_setCollisionShape(IntPtr handle, IntPtr collisionShape); // btCollisionShape
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionObject_setCompanionId(IntPtr handle, int id);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionObject_setContactProcessingThreshold(IntPtr handle, float contactProcessingThreshold);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionObject_setDeactivationTime(IntPtr handle, float time);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionObject_setFriction(IntPtr handle, float frict);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionObject_setHitFraction(IntPtr handle, float hitFraction);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionObject_setInterpolationAngularVelocity(IntPtr handle, IntPtr angvel);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionObject_setInterpolationLinearVelocity(IntPtr handle, IntPtr linvel);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionObject_setInterpolationWorldTransform(IntPtr handle, IntPtr trans);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionObject_setIslandTag(IntPtr handle, int tag);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionObject_setRestitution(IntPtr handle, float rest);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionObject_setUserPointer(IntPtr handle, IntPtr userPointer);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionObject_setWorldTransform(IntPtr handle, IntPtr worldTrans);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_BtCollisionObject_delete(IntPtr handle);
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
                BulletAPI_BtCollisionObject_delete(m_handle);
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

        ~btCollisionObject()
        {
           // Do not re-create Dispose clean-up code here.
           // Calling Dispose(false) is optimal in terms of
           // readability and maintainability.
           Dispose(false);
        }
    }
}
