#include "btCollisionWorld_net.h"
#include "BulletCollision/CollisionDispatch/btCollisionWorld.h"

btCollisionWorld *GetBtCollisionWorldFromIntPtr(IntPtr handle)
{
    return (btCollisionWorld *)handle;
}

void addCollisionObject(IntPtr handle, IntPtr collisionObject, int collisionFilterGroup, int collisionFilterMask)
{
    GetBtCollisionWorldFromIntPtr(handle)->addCollisionObject((btCollisionObject *)collisionObject,collisionFilterGroup, collisionFilterMask);
}

IntPtr getBroadphase(IntPtr handle)
{
    return (GetBtCollisionWorldFromIntPtr(handle)->getBroadphase());
}
 //btBroadphaseInterface*
IntPtr getPairCache(IntPtr handle)
{
    return (GetBtCollisionWorldFromIntPtr(handle)->getPairCache());
}
//btOverlappingPairCache*
IntPtr getDispatcher(IntPtr handle)
{
    return (GetBtCollisionWorldFromIntPtr(handle)->getDispatcher());
}
//btDispatcher*	
int getNumCollisionObjects(IntPtr handle)
{
    return GetBtCollisionWorldFromIntPtr(handle)->getNumCollisionObjects();
}

void BulletAPI_btCollisionWorld_rayTest(IntPtr handle, IntPtr rayFromWorld, IntPtr rayToWorld, IntPtr resultCallback)
{
    GetBtCollisionWorldFromIntPtr(handle)->rayTest(*(btVector3 *)rayFromWorld,*(btVector3 *)rayToWorld,*(btCollisionWorld::RayResultCallback *)resultCallback);
}

void BulletAPI_btCollisionWorld_removeCollisionObject(IntPtr handle, IntPtr collisionObject)
{
    GetBtCollisionWorldFromIntPtr(handle)->removeCollisionObject((btCollisionObject *) collisionObject);
}
