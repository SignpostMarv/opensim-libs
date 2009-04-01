#include "main.h"

extern "C"
{
    EXPORT IntPtr BulletAPI_CreateBtSphereShape(float radius);
    EXPORT float BulletAPI_BtSphereShape_getMargin(IntPtr handle);
    EXPORT float BulletAPI_BtSphereShape_getRadius(IntPtr handle);
    EXPORT void BulletAPI_BtSphereShape_setUnscaledRadius(IntPtr handle, float unscaledRadius);
    EXPORT void BulletAPI_BtSphereShape_delete(IntPtr handle);
}