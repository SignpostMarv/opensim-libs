#include "btCollisionDispatcher_net.h"
#include "BulletCollision/Gimpact/btGImpactCollisionAlgorithm.h"

btCollisionConfiguration *GetBtCollisionConfiguration_btCollisionDispatcher(IntPtr object)
{
    return (btCollisionConfiguration *) object;
}

btCollisionDispatcher *GetBtCollisionDispatcherFromIntPtr(IntPtr object)
{
    return (btCollisionDispatcher*) object;
}

IntPtr BulletAPI_CreateBtCollisionDispatcher(IntPtr config)
{
    return new btCollisionDispatcher(GetBtCollisionConfiguration_btCollisionDispatcher(config));
}

void BulletAPI_BtCollisionDispatcher_RegisterGImpact(IntPtr handle)
{
    btGImpactCollisionAlgorithm::registerAlgorithm((btCollisionDispatcher *) handle);
}

void BulletAPI_BtCollisionDispatcher_delete(IntPtr obj)
{
    delete GetBtCollisionDispatcherFromIntPtr(obj);
}