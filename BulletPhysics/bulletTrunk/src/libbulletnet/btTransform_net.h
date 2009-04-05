#include "main.h"

extern "C"
{
    EXPORT IntPtr BulletAPI_CreateBtTransform();
    EXPORT IntPtr BulletAPI_CreateBtTransformQuaternionVector3(IntPtr q, IntPtr v);
    EXPORT IntPtr BulletAPI_CreateBtTransformMatrix3Vector3(IntPtr m, IntPtr v);
    EXPORT IntPtr BulletAPI_CreateBtTransformOtherTransform(IntPtr t);
    EXPORT IntPtr BulletAPI_BtTransform_getBasis(IntPtr handle);
    EXPORT IntPtr BulletAPI_BtTransform_getIdentity(IntPtr handle);
    EXPORT void BulletAPI_BtTransform_getOpenGLMatrix(IntPtr handle, float * m); // float array!
    EXPORT IntPtr BulletAPI_BtTransform_getOrigin(IntPtr handle);
    EXPORT IntPtr BulletAPI_BtTransform_getRotation(IntPtr handle);
    EXPORT IntPtr BulletAPI_BtTransform_inverse(IntPtr handle);
    EXPORT IntPtr BulletAPI_BtTransform_inverseTimes(IntPtr handle, IntPtr t);
    EXPORT IntPtr BulletAPI_BtTransform_invXform(IntPtr handle, IntPtr inVec); // // btVector3
    EXPORT void BulletAPI_BtTransform_mult(IntPtr handle, IntPtr t1, IntPtr t2);
    EXPORT void BulletAPI_BtTransform_setBasis(IntPtr handle, IntPtr basis);
    EXPORT void BulletAPI_BtTransform_setFromOpenGLMatrix(IntPtr handle, float *m);
    EXPORT void BulletAPI_BtTransform_setOrigin(IntPtr handle, IntPtr origin);
    EXPORT void BulletAPI_BtTransform_setRotation(IntPtr handle, IntPtr q);
    EXPORT void BulletAPI_BtTransform_delete(IntPtr handle);
}