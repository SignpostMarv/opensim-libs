#include "btTriangleIndexVertexArray_net.h"
#include "BulletCollision/CollisionShapes/btTriangleIndexVertexArray.h"

btTriangleIndexVertexArray *GetBtTriangleIndexVertexArrayFromIntPtr(IntPtr handle)
{
    return (btTriangleIndexVertexArray *)handle;
}

IntPtr BulletAPI_CreateBtTriangleIndexVertexArray(int numTriangles, int* triangleIndexBase, int triangleIndexStride, int numVerticies, float* vertexBase, int vertexStride)
{

    return new btTriangleIndexVertexArray(numTriangles, triangleIndexBase,triangleIndexStride, numVerticies, vertexBase, vertexStride);
}

void BulletAPI_BtTriangleIndexVertexArray_delete(IntPtr handle)
{
    delete GetBtTriangleIndexVertexArrayFromIntPtr(handle);
}
