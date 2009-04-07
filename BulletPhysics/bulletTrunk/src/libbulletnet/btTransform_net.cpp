#include "btTransform_net.h"

btTransform *GetBtTransformFromIntPtr(IntPtr obj)
{
    return (btTransform *)obj;
}

IntPtr BulletAPI_CreateBtTransform()
{
    return new btTransform();
}

IntPtr BulletAPI_CreateBtTransformQuaternionVector3(IntPtr q, IntPtr v)
{
    return new btTransform(*(btQuaternion *) q , *(btVector3 *)v);
}

IntPtr BulletAPI_CreateBtTransformMatrix3Vector3(IntPtr m, IntPtr v)
{
    return new btTransform(*(btMatrix3x3 *) m , *(btVector3 *)v);
}

IntPtr BulletAPI_CreateBtTransformOtherTransform( IntPtr t)
{
    return new btTransform(*(btTransform *) t);
}

// matrix3x3
IntPtr BulletAPI_BtTransform_getBasis(IntPtr handle)
{
    return &GetBtTransformFromIntPtr(handle)->getBasis();
}

// bttransform
IntPtr BulletAPI_BtTransform_getIdentity(IntPtr handle)
{
    return new btTransform(btTransform::getIdentity());
}

void BulletAPI_BtTransform_getOpenGLMatrix(IntPtr handle, float * m) // float array!
{
    GetBtTransformFromIntPtr(handle)->getOpenGLMatrix(m);
}

//btvector3
IntPtr BulletAPI_BtTransform_getOrigin(IntPtr handle)
{
    btVector3 obj = GetBtTransformFromIntPtr(handle)->getOrigin();
    return new btVector3(obj.x(), obj.y(), obj.z());
}

//btQuaternion
IntPtr BulletAPI_BtTransform_getRotation(IntPtr handle)
{
    
    btQuaternion quat = (GetBtTransformFromIntPtr(handle)->getRotation());
    return new btQuaternion(quat.getX(), quat.getY(), quat.getZ(), quat.getW());
}

//btTransform
IntPtr BulletAPI_BtTransform_inverse(IntPtr handle)
{
    return &GetBtTransformFromIntPtr(handle)->inverse();
}

//btTransform
IntPtr BulletAPI_BtTransform_inverseTimes(IntPtr handle, IntPtr t) // btTransform
{
    return &GetBtTransformFromIntPtr(handle)->inverseTimes(*(btTransform*)t);
}

//btTransform
IntPtr BulletAPI_BtTransform_invXform(IntPtr handle, IntPtr inVec) // // btVector3
{
    return GetBtTransformFromIntPtr(handle)->invXform(*(btVector3*)inVec);
}

void BulletAPI_BtTransform_mult(IntPtr handle, IntPtr t1, IntPtr t2) // btTransform
{
     GetBtTransformFromIntPtr(handle)->mult(*(btTransform*)t1,*(btTransform*)t2);
}

void BulletAPI_BtTransform_setBasis(IntPtr handle, IntPtr basis) // btMatrix3x3
{
     GetBtTransformFromIntPtr(handle)->setBasis(*(btMatrix3x3*)basis);
}

void BulletAPI_BtTransform_setFromOpenGLMatrix(IntPtr handle, float *m) // btMatrix3x3
{
     GetBtTransformFromIntPtr(handle)->setFromOpenGLMatrix(m);
}

void BulletAPI_BtTransform_setOrigin(IntPtr handle, IntPtr origin) // btMatrix3x3
{
     GetBtTransformFromIntPtr(handle)->setOrigin(*(btVector3 *)origin);
}

void BulletAPI_BtTransform_setRotation(IntPtr handle, IntPtr q) // btMatrix3x3
{
     GetBtTransformFromIntPtr(handle)->setRotation(*(btQuaternion *)q);
}

void BulletAPI_BtTransform_delete(IntPtr handle)
{
    btTransform * obj = GetBtTransformFromIntPtr(handle);
    if (obj)
    {
        delete obj;
        obj = NULL;
    }
}