#include "btCapsuleShape_net.h"
#include "BulletCollision/CollisionDispatch/btGhostObject.h"


btGhostObject *GetBtGhostObjectFromIntPtr(IntPtr handle)
{
    return (btGhostObject *) handle;
}

btPairCachingGhostObject *GetBtPairCachingGhostObjectFromIntPtr(IntPtr handle)
{
    return (btPairCachingGhostObject *) handle;
}


IntPtr BulletAPI_CreateBtGhostObject()
{
    return new btGhostObject();
}

int BulletAPI_BtGhostObject_getNumOverlappingObjects(IntPtr handle)
{
    return GetBtGhostObjectFromIntPtr(handle)->getNumOverlappingObjects();
}

IntPtr BulletAPI_BtGhostObject_getOverlappingObject(IntPtr handle, int index)
{
    return GetBtGhostObjectFromIntPtr(handle)->getOverlappingObject(index);
}

void BulletAPI_BtGhostObject_delete(IntPtr handle)
{
    delete GetBtGhostObjectFromIntPtr(handle);
}

IntPtr BulletAPI_CreateBtPairCachingGhostObject()
{
    return new btPairCachingGhostObject();
}

void BulletAPI_BtPairCachingGhostObject_delete(IntPtr handle)
{
    delete GetBtPairCachingGhostObjectFromIntPtr(handle);
}
