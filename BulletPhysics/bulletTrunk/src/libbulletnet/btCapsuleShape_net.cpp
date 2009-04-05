#include "btCapsuleShape_net.h"

btCapsuleShape *GetBtCapsuleShapeFromIntPtr(IntPtr handle)
{
    return (btCapsuleShape *) handle;
}

IntPtr BulletAPI_CreateBtCapsuleShape(float radius,float height)
{
    return new btCapsuleShape(radius,height);
}

float BulletAPI_BtCapsuleShape_getRadius(IntPtr handle)
{
     return GetBtCapsuleShapeFromIntPtr(handle)->getRadius();
}

float BulletAPI_BtCapsuleShape_getHalfHeight(IntPtr handle)
{
    return GetBtCapsuleShapeFromIntPtr(handle)->getHalfHeight();
}

void BulletAPI_BtCapsuleShape_delete(IntPtr handle)
{
    delete GetBtCapsuleShapeFromIntPtr(handle);
}
