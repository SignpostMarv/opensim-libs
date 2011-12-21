using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BulletDotNET
{
    public class btCollisionWorld
    {
        protected IntPtr m_handle = IntPtr.Zero;
        protected bool m_disposed = false;

        public btCollisionWorld()
        {
            
        }

        public static btCollisionWorld FromIntPtr(IntPtr handle)
        {
            return (btCollisionWorld)Native.GetObject(handle, typeof(btCollisionWorld));
        }


        public btCollisionWorld(IntPtr handle)
        {
            m_handle = handle;
        }

        public void addCollisionObject(btCollisionObject collisionObject, int collisionFilterGroup, int collisionFilterMask)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_btCollisionWorld_addCollisionObject(m_handle, collisionObject.Handle, collisionFilterGroup,
                                                          collisionFilterMask);
        }

        public btBroadphaseInterface getBroadPhase()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btAxisSweep3.FromIntPtr(BulletAPI_btCollisionWorld_getBroadphase(m_handle));
        }

        public btDispatcher getDispatcher()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return btCollisionDispatcher.FromIntPtr(m_handle);
        }

        public int getNumCollisionObjects()
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            return BulletAPI_btCollisionWorld_getNumCollisionObjects(m_handle);
        }

        public void rayTest(btVector3 rayFromWorld, btVector3 rayToWorld, btRayResultCallback resultCallback)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_btCollisionWorld_rayTest(m_handle, rayFromWorld.Handle, rayToWorld.Handle, resultCallback.Handle);
        }

        public void removeCollisionObject(btCollisionObject obj)
        {
            if (m_disposed)
                throw new ObjectDisposedException(ToString());

            BulletAPI_btCollisionWorld_removeCollisionObject(m_handle, obj.Handle);

        }

        #region NativeInvokes

         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_btCollisionWorld_addCollisionObject(IntPtr handle, IntPtr collisionObject, int collisionFilterGroup, int collisionFilterMask);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_btCollisionWorld_getBroadphase(IntPtr handle); //btBroadphaseInterface*
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_btCollisionWorld_getPairCache(IntPtr handle); //btOverlappingPairCache*
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletAPI_btCollisionWorld_getDispatcher(IntPtr handle); //btDispatcher*	
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern int BulletAPI_btCollisionWorld_getNumCollisionObjects(IntPtr handle);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_btCollisionWorld_rayTest(IntPtr handle, IntPtr rayFromWorld, IntPtr rayToWorld, IntPtr resultCallback);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletAPI_btCollisionWorld_removeCollisionObject(IntPtr handle, IntPtr collisionObject);

        #endregion

    }
}
