#include "main.h"

extern "C"
{
    EXPORT IntPtr BulletAPI_CreateBtTriangleMesh(bool use32BitIndices, bool use4componentVerticies);
    EXPORT void BulletAPI_BtTriangleMesh_addIndex(IntPtr handle, int index);
    EXPORT void BulletAPI_BtTriangleMesh_addTriangle(IntPtr handle, IntPtr vertex0, IntPtr vertex1, IntPtr vertex2, bool removeDuplicateVertices);
    EXPORT int BulletAPI_BtTriangleMesh_findOrAddVertex(IntPtr handle, IntPtr vertex, bool removeDuplicateVertices);
    EXPORT int BulletAPI_BtTriangleMesh_getNumTriangles(IntPtr handle);
    EXPORT bool BulletAPI_BtTriangleMesh_getUse32BitIndices(IntPtr handle);
    EXPORT bool BulletAPI_BtTriangleMesh_getUse4componentVertices(IntPtr handle);
    EXPORT void BulletAPI_BtTriangleMesh_delete(IntPtr handle);
}