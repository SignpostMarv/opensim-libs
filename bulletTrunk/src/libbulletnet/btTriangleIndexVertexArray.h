#include "main.h"

extern "C"
{
    EXPORT IntPtr BulletAPI_CreateBtTriangleIndexVertexArray(int numTriangles, int * triangleIndexBase, int triangleIndexStride, int numVerticies, float * vertexBase, int vertexStride);
    EXPORT void BulletAPI_BtTriangleIndexVertexArray_delete(IntPtr handle);
}