#include "btSphereShape_net.h"
#include "BulletCollision/CollisionShapes/btSphereShape.h"

btSphereShape *GetBtSphereShapeFromIntPtr(IntPtr handle)
{
    return (btSphereShape *) handle;
}

IntPtr BulletAPI_CreateBtSphereShape(float radius)
{
    return new btSphereShape(radius);
}

float BulletAPI_BtSphereShape_getMargin(IntPtr handle)
{
    return GetBtSphereShapeFromIntPtr(handle)->getMargin();
}

float BulletAPI_BtSphereShape_getRadius(IntPtr handle)
{
    return GetBtSphereShapeFromIntPtr(handle)->getRadius();
}

void BulletAPI_BtSphereShape_setUnscaledRadius(IntPtr handle, float unscaledRadius)
{
    GetBtSphereShapeFromIntPtr(handle)->setUnscaledRadius(unscaledRadius);
}

void BulletAPI_BtSphereShape_delete(IntPtr handle)
{
    delete GetBtSphereShapeFromIntPtr(handle);
}

