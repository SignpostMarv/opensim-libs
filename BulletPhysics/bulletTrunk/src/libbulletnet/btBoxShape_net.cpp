#include "btBoxShape_net.h"

btBoxShape *GetBtBoxShapeFromIntPtr(IntPtr handle)
{
    return (btBoxShape *)handle;
}

IntPtr BulletAPI_CreateBtBoxShape(IntPtr boxHalfExtents)
{
    return new btBoxShape(*(btVector3 *)boxHalfExtents );
}


IntPtr BulletAPI_BtBoxShape_getHalfExtentsWithMargin(IntPtr handle)
{
    btVector3 val = GetBtBoxShapeFromIntPtr(handle)->getHalfExtentsWithMargin();
    
    return new btVector3(val.getX(), val.getY(), val.getZ());
}

IntPtr BulletAPI_BtBoxShape_getHalfExtentsWithoutMargin(IntPtr handle)
{
    btVector3 val = GetBtBoxShapeFromIntPtr(handle)->getHalfExtentsWithoutMargin();
    
    return new btVector3(val.getX(), val.getY(), val.getZ());
}

void BulletAPI_BtBoxShape_calculateLocalInertia(IntPtr handle, float mass, IntPtr inertia)// btVector3
    //GetNumPlanes() - 6, GetNumVertices() - 8, getNumEdges-12
{
    GetBtBoxShapeFromIntPtr(handle)->calculateLocalInertia(mass,*(btVector3 *)inertia);
}

IntPtr BulletAPI_BtBoxShape_getVertex(IntPtr handle, int i) // weird
{
    btVector3* result = new btVector3();
    GetBtBoxShapeFromIntPtr(handle)->getVertex(i,*(btVector3 *)result);
    return result;
}

bool BulletAPI_BtBoxShape_isInside(IntPtr handle, IntPtr pt, float tolerance)
{
   _FIX_BOOL_MARSHAL_BUG(GetBtBoxShapeFromIntPtr(handle)->isInside(*(btVector3 *)pt, tolerance));
}

void BulletAPI_BtBoxShape_setLocalScaling(IntPtr handle, IntPtr scaling)
{
    GetBtBoxShapeFromIntPtr(handle)->setLocalScaling(*(btVector3 *) scaling);
}

void BulletAPI_BtBoxShape_setMargins(IntPtr handle, float collisionMargin)
{
    GetBtBoxShapeFromIntPtr(handle)->setMargin(collisionMargin);
}

void BulletAPI_BtBoxShape_delete(IntPtr handle)
{
    delete GetBtBoxShapeFromIntPtr(handle);
}

