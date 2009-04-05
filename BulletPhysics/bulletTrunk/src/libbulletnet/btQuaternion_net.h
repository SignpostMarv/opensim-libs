#include "main.h"

extern "C"
{
    EXPORT IntPtr BulletAPI_CreateBtQuaternion();
    EXPORT IntPtr BulletAPI_CreateBtQuaternionFourFloats(float x, float y, float z, float w);
    EXPORT IntPtr BulletAPI_CreateBtQuaternionAxisAngle(IntPtr axis, float angle);
    EXPORT IntPtr BulletAPI_CreateBtQuaternionEuler(float yaw, float pitch, float roll);
    
    EXPORT float BulletAPI_BtQuaternion_angle(IntPtr handle, IntPtr q);
    EXPORT float BulletAPI_BtQuaternion_dot(IntPtr handle, IntPtr q);
    EXPORT IntPtr BulletAPI_BtQuaternion_farthest(IntPtr handle, IntPtr qd);
    EXPORT float BulletAPI_BtQuaternion_getAngle(IntPtr handle);
    // missing getIdentity..   as it's too complicated and CPU intensive to do somewhere else :P
    EXPORT float BulletAPI_BtQuaternion_getW(IntPtr handle);
    EXPORT void BulletAPI_BtQuaternion_getValues(IntPtr handle, float* v);
    EXPORT float BulletAPI_BtQuaternion_getX(IntPtr handle);
    EXPORT float BulletAPI_BtQuaternion_getY(IntPtr handle);
    EXPORT float BulletAPI_BtQuaternion_getZ(IntPtr handle);
    EXPORT IntPtr BulletAPI_BtQuaternion_inverse(IntPtr handle);

    EXPORT float BulletAPI_BtQuaternion_length(IntPtr handle);
    EXPORT float BulletAPI_BtQuaternion_length2(IntPtr handle);
    EXPORT IntPtr BulletAPI_BtQuaternion_normalize(IntPtr handle);
    EXPORT IntPtr BulletAPI_BtQuaternion_normalized(IntPtr handle);
    EXPORT void BulletAPI_BtQuaternion_setEuler(IntPtr handle, float yaw, float pitch, float roll);
    EXPORT void BulletAPI_BtQuaternion_setEulerZYX(IntPtr handle, float yaw, float pitch, float roll);
    EXPORT void BulletAPI_BtQuaternion_setRotation(IntPtr handle, IntPtr axis, float angle);
    EXPORT IntPtr BulletAPI_BtQuaternion_slerp(IntPtr handle, IntPtr q, float t);

    EXPORT void BulletAPI_BtQuaternion_delete(IntPtr handle);
}