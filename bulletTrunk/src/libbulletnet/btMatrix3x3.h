#include "main.h"

extern "C"
{
    EXPORT IntPtr BulletAPI_CreateBtMatrix3x3();
    EXPORT IntPtr BulletAPI_CreateBtMatrix3x3Quaternion(IntPtr q);
    EXPORT IntPtr BulletAPI_CreateBtMatrix3x3Floats(float xx, float xy, float xz, float yx, float yy, float yz, float zx, float zy, float zz);
    EXPORT IntPtr BulletAPI_CreateBtMatrix3x3Other(IntPtr m);
    EXPORT IntPtr BulletAPI_BtMatrix3x3_absolute(IntPtr handle);
    EXPORT IntPtr BulletAPI_BtMatrix3x3_adjoint(IntPtr handle);
    EXPORT IntPtr BulletAPI_BtMatrix3x3_inverse(IntPtr handle);
    EXPORT IntPtr BulletAPI_BtMatrix3x3_determinant(IntPtr handle);
    EXPORT void BulletAPI_BtMatrix3x3_diagonalize(IntPtr handle,IntPtr rot, float threshold, int maxSteps);
    EXPORT IntPtr BulletAPI_BtMatrix3x3_getColumn(IntPtr handle,int i);
    EXPORT void BulletAPI_BtMatrix3x3_getOpenGLSubMatrix(IntPtr handle,float * m);
    EXPORT void BulletAPI_BtMatrix3x3_setRotation(IntPtr handle, IntPtr q);
    EXPORT void BulletAPI_BtMatrix3x3_delete(IntPtr handle);
}