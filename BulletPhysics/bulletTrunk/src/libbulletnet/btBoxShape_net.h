#include "main.h"

extern "C"
{
    EXPORT IntPtr BulletAPI_CreateBtBoxShape(IntPtr boxHalfExtents);
    EXPORT IntPtr BulletAPI_BtBoxShape_getHalfExtentsWithMargin(IntPtr handle);
    EXPORT IntPtr BulletAPI_BtBoxShape_getHalfExtentsWithoutMargin(IntPtr handle);
    EXPORT void BulletAPI_BtBoxShape_calculateLocalInertia(IntPtr handle, float mass, IntPtr inertia);// btVector3
    //GetNumPlanes() - 6, GetNumVertices() - 8, getNumEdges-12
    EXPORT IntPtr BulletAPI_BtBoxShape_getVertex(IntPtr handle, int i); // weird
    EXPORT bool BulletAPI_BtBoxShape_isInside(IntPtr handle, IntPtr pt, float tolerance);
    EXPORT void BulletAPI_BtBoxShape_setLocalScaling(IntPtr handle, IntPtr scaling);
    EXPORT void BulletAPI_BtBoxShape_setMargins(IntPtr handle, float collisionMargin);
    EXPORT void BulletAPI_BtBoxShape_delete(IntPtr handle);
}