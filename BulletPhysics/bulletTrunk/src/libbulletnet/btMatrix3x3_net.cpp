#include "btMatrix3x3_net.h"

btMatrix3x3 *GetBtMatrix3x3FromIntPtr(IntPtr obj)
{
    return (btMatrix3x3 *) obj;
}

IntPtr BulletAPI_CreateBtMatrix3x3()
{
    return new btMatrix3x3();
}

IntPtr BulletAPI_CreateBtMatrix3x3Quaternion(IntPtr q)
{
    return new btMatrix3x3(*(btQuaternion *) q);
}

IntPtr BulletAPI_CreateBtMatrix3x3Floats(float xx, float xy, float xz, float yx, float yy, float yz, float zx, float zy, float zz)
{
    return new btMatrix3x3(xx,xy,xz,yx,yy,yz,zx,zy,zz);
}

IntPtr BulletAPI_CreateBtMatrix3x3Other(IntPtr m)
{
    return new btMatrix3x3(*(btMatrix3x3 *) m);
}

IntPtr BulletAPI_BtMatrix3x3_absolute(IntPtr handle)
{
    return &GetBtMatrix3x3FromIntPtr(handle)->absolute();
}

IntPtr BulletAPI_BtMatrix3x3_adjoint(IntPtr handle)
{
    return &GetBtMatrix3x3FromIntPtr(handle)->adjoint();
}

IntPtr BulletAPI_BtMatrix3x3_inverse(IntPtr handle)
{
    return &GetBtMatrix3x3FromIntPtr(handle)->inverse();
}

IntPtr BulletAPI_BtMatrix3x3_determinant(IntPtr handle)
{
    return &GetBtMatrix3x3FromIntPtr(handle)->inverse();
}

void BulletAPI_BtMatrix3x3_diagonalize(IntPtr handle,IntPtr rot, float threshold, int maxSteps)
{
    GetBtMatrix3x3FromIntPtr(handle)->diagonalize(*(btMatrix3x3*) rot, threshold, maxSteps);
}

IntPtr BulletAPI_BtMatrix3x3_getColumn(IntPtr handle,int i)
{
    return &GetBtMatrix3x3FromIntPtr(handle)->getColumn(i);
}

void BulletAPI_BtMatrix3x3_getOpenGLSubMatrix(IntPtr handle,float * m)
{
    GetBtMatrix3x3FromIntPtr(handle)->getOpenGLSubMatrix(m);
}

void BulletAPI_BtMatrix3x3_setRotation(IntPtr handle, IntPtr q)
{
    GetBtMatrix3x3FromIntPtr(handle)->setRotation(*(btQuaternion *)q);
}

void BulletAPI_BtMatrix3x3_delete(IntPtr handle)
{
    delete GetBtMatrix3x3FromIntPtr(handle);
}