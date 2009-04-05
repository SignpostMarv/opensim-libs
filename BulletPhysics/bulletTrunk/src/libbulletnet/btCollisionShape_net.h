#include "main.h"

extern "C"
{
    EXPORT IntPtr BulletAPI_CreateBtCollisionShape();
    EXPORT void BulletAPI_BtCollisionShape_calculateLocalInertia(IntPtr handle, float mass, IntPtr inertia);
    EXPORT float BulletAPI_BtCollisionShape_getAngularMotionDisk(IntPtr handle);
    EXPORT IntPtr BulletAPI_BtCollisionShape_getLocalScaling(IntPtr handle);
    EXPORT float BulletAPI_BtCollisionShape_getContactBreakingThreshold(IntPtr handle);
    EXPORT float BulletAPI_BtCollisionShape_getMargin(IntPtr handle);
    EXPORT int BulletAPI_BtCollisionShape_getShapeType(IntPtr handle);
    EXPORT IntPtr BulletAPI_BtCollisionShape_getUserPointer(IntPtr handle);
    EXPORT bool BulletAPI_BtCollisionShape_isCompound(IntPtr handle);
    EXPORT bool BulletAPI_BtCollisionShape_isConcave(IntPtr handle);
    EXPORT bool BulletAPI_BtCollisionShape_isConvex(IntPtr handle);
    EXPORT bool BulletAPI_BtCollisionShape_isInfinite(IntPtr handle);
    EXPORT bool BulletAPI_BtCollisionShape_isPolyhedral(IntPtr handle);
    EXPORT void BulletAPI_BtCollisionShape_setLocalScaling(IntPtr handle, IntPtr scaling);
    EXPORT void BulletAPI_BtCollisionShape_setMargin(IntPtr handle, float margin);
    EXPORT void BulletAPI_BtCollisionShape_setUserPointer(IntPtr handle, IntPtr ptr);
    EXPORT void BulletAPI_BtCollisionShape_delete(IntPtr obj);
}