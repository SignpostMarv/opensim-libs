#include "btGImpactMeshShape_net.h"
#include "BulletCollision/Gimpact/btGImpactShape.h"

btGImpactMeshShape *GetBtGImpactMeshShapeFromIntPtr(IntPtr handle)
{
    return (btGImpactMeshShape *) handle;
}

IntPtr BulletAPI_CreateBtGImpactMeshShape(IntPtr meshInterface)
{
    return new btGImpactMeshShape((btStridingMeshInterface *) meshInterface);
}

void BulletAPI_BtGImpactMeshShape_updateBound(IntPtr handle)
{
    GetBtGImpactMeshShapeFromIntPtr(handle)->updateBound();
}

void BulletAPI_BtGImpactMeshShape_delete(IntPtr handle)
{
    delete GetBtGImpactMeshShapeFromIntPtr(handle);
}
