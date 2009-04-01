#include "main.h"


extern "C"
{
    EXPORT IntPtr BulletAPI_CreateBtVector3(float x, float y, float z);
    EXPORT IntPtr BulletAPI_BtVector3_absolute(IntPtr umv);
    EXPORT float BulletAPI_BtVector3_angle(IntPtr umv, IntPtr v);
    EXPORT int BulletAPI_BtVector3_closestAxis(IntPtr umv);
    EXPORT IntPtr BulletAPI_BtVector3_cross(IntPtr umv, IntPtr v);
    EXPORT float BulletAPI_BtVector3_distance(IntPtr umv, IntPtr v);
    EXPORT float BulletAPI_BtVector3_distance2(IntPtr umv, IntPtr v);
    EXPORT float BulletAPI_BtVector3_dot(IntPtr umv, IntPtr v);
    EXPORT int BulletAPI_BtVector3_furthestAxis(IntPtr umv);
    EXPORT float BulletAPI_BtVector3_GetX(IntPtr umv);
    EXPORT float BulletAPI_BtVector3_GetY(IntPtr umv);
    EXPORT float BulletAPI_BtVector3_GetZ(IntPtr umv);
    EXPORT IntPtr BulletAPI_BtVector3_normalized(IntPtr umv);
    EXPORT void BulletAPI_BtVector3_setInterpolate3(IntPtr umv, IntPtr v0, IntPtr v1, float rt);
    EXPORT void BulletAPI_BtVector3_setValue(IntPtr umv, float x, float y, float z);
    EXPORT void BulletAPI_BtVector3_setX(IntPtr umv, float x);
    EXPORT void BulletAPI_BtVector3_setY(IntPtr umv, float y);
    EXPORT void BulletAPI_BtVector3_setZ(IntPtr umv, float z);
    EXPORT void BulletAPI_BtVector3_delete(IntPtr umv);
}