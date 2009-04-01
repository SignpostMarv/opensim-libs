#include "main.h"

extern "C"
{
    EXPORT IntPtr BulletAPI_CreateBtCapsuleShape(float radius,float height);
    EXPORT float BulletAPI_BtCapsuleShape_getRadius(IntPtr handle);
    EXPORT float BulletAPI_BtCapsuleShape_getHalfHeight(IntPtr handle);
    EXPORT void BulletAPI_BtCapsuleShape_delete(IntPtr handle);
}