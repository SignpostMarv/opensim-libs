#include "btTriangleMesh_net.h"
#include "BulletCollision/CollisionShapes/btTriangleMesh.h"

btTriangleMesh *GetBtTriangleMeshFromIntPtr(IntPtr handle)
{
    return (btTriangleMesh *) handle;
}

IntPtr BulletAPI_CreateBtTriangleMesh(bool use32BitIndices, bool use4componentVertices)
{
    return new btTriangleMesh(use32BitIndices, use4componentVertices);
}


void BulletAPI_BtTriangleMesh_addIndex(IntPtr handle, int index)
{
    return GetBtTriangleMeshFromIntPtr(handle)->addIndex(index);
}

void BulletAPI_BtTriangleMesh_addTriangle(IntPtr handle, IntPtr vertex0, IntPtr vertex1, IntPtr vertex2, bool removeDuplicateVertices)
{
    GetBtTriangleMeshFromIntPtr(handle)->addTriangle(*(btVector3 *)vertex0, *(btVector3 *)vertex1, *(btVector3 *)vertex2, removeDuplicateVertices);
}

int BulletAPI_BtTriangleMesh_findOrAddVertex(IntPtr handle, IntPtr vertex, bool removeDuplicateVertices)
{
    return GetBtTriangleMeshFromIntPtr(handle)->findOrAddVertex(*(btVector3 *)vertex, removeDuplicateVertices);
}

int BulletAPI_BtTriangleMesh_getNumTriangles(IntPtr handle)
{
    return GetBtTriangleMeshFromIntPtr(handle)->getNumTriangles();
}

bool BulletAPI_BtTriangleMesh_getUse32BitIndices(IntPtr handle)
{
    _FIX_BOOL_MARSHAL_BUG(GetBtTriangleMeshFromIntPtr(handle)->getUse32bitIndices());
}

bool BulletAPI_BtTriangleMesh_getUse4componentVertices(IntPtr handle)
{
    _FIX_BOOL_MARSHAL_BUG(GetBtTriangleMeshFromIntPtr(handle)->getUse4componentVertices());
}

void BulletAPI_BtTriangleMesh_delete(IntPtr handle)
{
    delete GetBtTriangleMeshFromIntPtr(handle);
}

