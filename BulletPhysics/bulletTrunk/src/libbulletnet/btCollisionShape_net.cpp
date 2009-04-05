#include "btCollisionShape_net.h"
#include "LinearMath/btVector3.h"
#include "BulletCollision/CollisionShapes/btCollisionShape.h"


btCollisionShape *GetBtCollisionShape(IntPtr obj)
{
    return (btCollisionShape *) obj;
}

IntPtr BulletAPI_CreateBtCollisionShape()
{
    btCollisionShape* boxshape = new btBoxShape(*new btVector3(0.5f,0.5f,0.5f));
    return boxshape;
}

void BulletAPI_BtCollisionShape_calculateLocalInertia(IntPtr handle, float mass, IntPtr inertia)
{
    GetBtCollisionShape(handle)->calculateLocalInertia(mass, *(btVector3*)inertia);
}

float BulletAPI_BtCollisionShape_getAngularMotionDisk(IntPtr handle)
{
    return GetBtCollisionShape(handle)->getAngularMotionDisc();
}

IntPtr BulletAPI_BtCollisionShape_getLocalScaling(IntPtr handle)
{
    btVector3 result = GetBtCollisionShape(handle)->getLocalScaling();
    return result;
}
float BulletAPI_BtCollisionShape_getContactBreakingThreshold(IntPtr handle)
{
    return GetBtCollisionShape(handle)->getContactBreakingThreshold();
}
float BulletAPI_BtCollisionShape_getMargin(IntPtr handle)
{
    return GetBtCollisionShape(handle)->getMargin();
}

int BulletAPI_BtCollisionShape_getShapeType(IntPtr handle)
{
    return GetBtCollisionShape(handle)->getShapeType();
}

IntPtr BulletAPI_BtCollisionShape_getUserPointer(IntPtr handle)
{
    return GetBtCollisionShape(handle)->getUserPointer();
}

bool BulletAPI_BtCollisionShape_isCompound(IntPtr handle)
{
    _FIX_BOOL_MARSHAL_BUG(GetBtCollisionShape(handle)->isCompound());
}

bool BulletAPI_BtCollisionShape_isConcave(IntPtr handle)
{
    _FIX_BOOL_MARSHAL_BUG(GetBtCollisionShape(handle)->isConcave());
}

bool BulletAPI_BtCollisionShape_isConvex(IntPtr handle)
{
    _FIX_BOOL_MARSHAL_BUG(GetBtCollisionShape(handle)->isConvex());
}

bool BulletAPI_BtCollisionShape_isInfinite(IntPtr handle)
{
    _FIX_BOOL_MARSHAL_BUG(GetBtCollisionShape(handle)->isInfinite());
}

bool BulletAPI_BtCollisionShape_isPolyhedral(IntPtr handle)
{
    _FIX_BOOL_MARSHAL_BUG(GetBtCollisionShape(handle)->isPolyhedral());
}

void BulletAPI_BtCollisionShape_setLocalScaling(IntPtr handle, IntPtr scaling)
{
    GetBtCollisionShape(handle)->setLocalScaling(*(btVector3 *) scaling);
}

void BulletAPI_BtCollisionShape_setMargin(IntPtr handle, float margin)
{
    GetBtCollisionShape(handle)->setMargin(margin);
}


void BulletAPI_BtCollisionShape_setUserPointer(IntPtr handle, IntPtr ptr)
{
    GetBtCollisionShape(handle)->setUserPointer((void *)ptr);
}

void BulletAPI_BtCollisionShape_delete(IntPtr obj)
{
    delete GetBtCollisionShape(obj);
}