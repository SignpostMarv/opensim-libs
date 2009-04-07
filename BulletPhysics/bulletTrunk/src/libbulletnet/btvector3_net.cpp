#include "btvector3_net.h"


btVector3 *GetBtVector3FromIntPtr(IntPtr object)
{
    return (btVector3*) object;
}

IntPtr BulletAPI_CreateBtVector3(float x, float y, float z)
{
    return new btVector3(x,y,z);;
}

IntPtr BulletAPI_BtVector3_absolute(IntPtr umv)
{
    btVector3 absoluteresult = GetBtVector3FromIntPtr(umv)->absolute();
    btScalar x = absoluteresult.getX();
    btScalar y = absoluteresult.getY();
    btScalar z = absoluteresult.getZ();
    return new btVector3(x,y,z);
}

float BulletAPI_BtVector3_angle(IntPtr umv, IntPtr v)
{
    return (GetBtVector3FromIntPtr(umv))->angle(*GetBtVector3FromIntPtr(v));
}

int BulletAPI_BtVector3_closestAxis(IntPtr umv)
{
    return (GetBtVector3FromIntPtr(umv))->closestAxis();
}

IntPtr BulletAPI_BtVector3_cross(IntPtr umv, IntPtr v)
{
    
    btVector3 crossresult = GetBtVector3FromIntPtr(umv)->cross( *GetBtVector3FromIntPtr(v) );
    btScalar x = crossresult.getX();
    btScalar y = crossresult.getY();
    btScalar z = crossresult.getZ();
    return new btVector3(x,y,z);
   
}

float BulletAPI_BtVector3_distance(IntPtr umv, IntPtr v)
{
    return (GetBtVector3FromIntPtr(umv))->distance(*GetBtVector3FromIntPtr(v));
}

float BulletAPI_BtVector3_distance2(IntPtr umv, IntPtr v)
{
    return (GetBtVector3FromIntPtr(umv))->distance2(*GetBtVector3FromIntPtr(v));
}

float BulletAPI_BtVector3_dot(IntPtr umv, IntPtr v)
{
    return (GetBtVector3FromIntPtr(umv))->dot(*GetBtVector3FromIntPtr(v));
}

int BulletAPI_BtVector3_furthestAxis(IntPtr umv)
{
    return (GetBtVector3FromIntPtr(umv))->furthestAxis();
}

float BulletAPI_BtVector3_GetX(IntPtr umv)
{
    //printf("%p - GetX<%f,%f,%f>\n",btv, (float)btv->x(), (float)btv->y(), (float)btv->z());
    return (GetBtVector3FromIntPtr(umv))->getX();
}

float BulletAPI_BtVector3_GetY(IntPtr umv)
{
    return (GetBtVector3FromIntPtr(umv))->getY();
}

float BulletAPI_BtVector3_GetZ(IntPtr umv)
{
    return (GetBtVector3FromIntPtr(umv))->getZ();
}

IntPtr BulletAPI_BtVector3_normalized(IntPtr umv)
{
    btVector3 normalizedresult = GetBtVector3FromIntPtr(umv)->normalized();
    btScalar x = normalizedresult.getX();
    btScalar y = normalizedresult.getY();
    btScalar z = normalizedresult.getZ();
    return new btVector3(x,y,z);
}

void BulletAPI_BtVector3_setInterpolate3(IntPtr umv, IntPtr v0, IntPtr v1, float rt)
{
    (GetBtVector3FromIntPtr(umv))->setInterpolate3(*GetBtVector3FromIntPtr(v0),*GetBtVector3FromIntPtr(v1), rt);
}

void BulletAPI_BtVector3_setValue(IntPtr umv, float x, float y, float z)
{
    (GetBtVector3FromIntPtr(umv))->setValue(x,y,z);
}

void BulletAPI_BtVector3_setX(IntPtr umv, float x)
{
    (GetBtVector3FromIntPtr(umv))->setX(x);
}

void BulletAPI_BtVector3_setY(IntPtr umv, float y)
{
    (GetBtVector3FromIntPtr(umv))->setY(y);
}

void BulletAPI_BtVector3_setZ(IntPtr umv, float z)
{
    (GetBtVector3FromIntPtr(umv))->setZ(z);
}


void BulletAPI_BtVector3_delete(IntPtr umv)
{
    btVector3 * obj = GetBtVector3FromIntPtr(umv);
    if (obj)
    {
        delete obj;
        obj = NULL;
    }
}


