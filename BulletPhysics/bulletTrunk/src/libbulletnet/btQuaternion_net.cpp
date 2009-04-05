#include "btQuaternion_net.h"

btQuaternion *GetBtQuaternionFromIntPtr(IntPtr obj)
{
    return (btQuaternion *)obj;
}

IntPtr BulletAPI_CreateBtQuaternion()
{
    return new btQuaternion();
}

IntPtr BulletAPI_CreateBtQuaternionFourFloats(float x, float y, float z, float w)
{
    return new btQuaternion(x,y,z,w);
}

IntPtr BulletAPI_CreateBtQuaternionAxisAngle(IntPtr axis, float angle)
{
    return new btQuaternion(*(btVector3 *)axis, angle);
}
IntPtr BulletAPI_CreateBtQuaternionEuler(float yaw, float pitch, float roll)
{
    return new btQuaternion(yaw, pitch, roll);
}

float BulletAPI_BtQuaternion_angle(IntPtr handle, IntPtr q)
{
    return GetBtQuaternionFromIntPtr(handle)->angle(*(btQuaternion *)q);
}

float BulletAPI_BtQuaternion_dot(IntPtr handle, IntPtr q)
{
    return GetBtQuaternionFromIntPtr(handle)->dot(*(btQuaternion *)q);
}

IntPtr BulletAPI_BtQuaternion_farthest(IntPtr handle, IntPtr qd)
{
    
    return &(GetBtQuaternionFromIntPtr(handle)->farthest(*(btQuaternion *)qd));
}

float BulletAPI_BtQuaternion_getAngle(IntPtr handle)
{
    return GetBtQuaternionFromIntPtr(handle)->getAngle();
}

float BulletAPI_BtQuaternion_getW(IntPtr handle)
{
    //float
    return  GetBtQuaternionFromIntPtr(handle)->getW();

}

void BulletAPI_BtQuaternion_getValues(IntPtr handle, float* v)
{
    //float
    btQuadWord* vals = (btQuadWord *)GetBtQuaternionFromIntPtr(handle);
    v[0]= vals->getX();
    v[1]= vals->getY();
    v[2]= vals->getZ();
    v[3]= vals->w();
}

float BulletAPI_BtQuaternion_getX(IntPtr handle)
{
    return GetBtQuaternionFromIntPtr(handle)->getX();
}
float BulletAPI_BtQuaternion_getY(IntPtr handle)
{
    return GetBtQuaternionFromIntPtr(handle)->getY();
}
float BulletAPI_BtQuaternion_getZ(IntPtr handle)
{
    return GetBtQuaternionFromIntPtr(handle)->getZ();
}


IntPtr BulletAPI_BtQuaternion_inverse(IntPtr handle)
{
    return &GetBtQuaternionFromIntPtr(handle)->inverse();
}

float BulletAPI_BtQuaternion_length(IntPtr handle)
{
    return GetBtQuaternionFromIntPtr(handle)->length();
}

float BulletAPI_BtQuaternion_length2(IntPtr handle)
{
    return GetBtQuaternionFromIntPtr(handle)->length2();
}

IntPtr BulletAPI_BtQuaternion_normalize(IntPtr handle)
{
    return GetBtQuaternionFromIntPtr(handle)->normalize();
}

IntPtr BulletAPI_BtQuaternion_normalized(IntPtr handle)
{
    btQuadWord* vals = (btQuadWord *)GetBtQuaternionFromIntPtr(handle);
    btQuaternion * result = new btQuaternion(vals->x(),vals->y(), vals->z(), vals->w());
    return &result->normalize();
}

void BulletAPI_BtQuaternion_setEuler(IntPtr handle, float yaw, float pitch, float roll)
{
    GetBtQuaternionFromIntPtr(handle)->setEuler(yaw, pitch, roll);
}

void BulletAPI_BtQuaternion_setEulerZYX(IntPtr handle, float yaw, float pitch, float roll)
{
    GetBtQuaternionFromIntPtr(handle)->setEulerZYX(yaw, pitch, roll);
}

void BulletAPI_BtQuaternion_setRotation(IntPtr handle, IntPtr axis, float angle)
{
    GetBtQuaternionFromIntPtr(handle)->setRotation(*(btVector3 *)axis, angle);
}

IntPtr BulletAPI_BtQuaternion_slerp(IntPtr handle, IntPtr q, float t)
{
    return GetBtQuaternionFromIntPtr(handle)->slerp(*(btQuaternion *)q, t);
}

void BulletAPI_BtQuaternion_delete(IntPtr handle)
{
    delete GetBtQuaternionFromIntPtr(handle);   
}