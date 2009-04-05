#include "btHeightfieldTerrainShape_net.h"
#include "BulletCollision/CollisionShapes/btHeightfieldTerrainShape.h"

btHeightfieldTerrainShape *GetBtHeightfieldTerrainShapeFromIntPtr(IntPtr handle)
{
    return (btHeightfieldTerrainShape *) handle;
}

IntPtr BulletAPI_CreateBtHeightfieldTerrainShape (int heightStickWidth, int heightStickLength, float * heightfieldData, float heightScale, float minHeight, float maxHeight,int upAxis, int heightDataType, bool flipQuadEdges)
{
    int hfsize = heightStickWidth * heightStickLength;
    float* hfvalues = new float[hfsize];
    for (int i=0;i < hfsize; i++)
        hfvalues[i] = heightfieldData[i];

    return new btHeightfieldTerrainShape(heightStickWidth,heightStickLength,hfvalues,heightScale,minHeight,maxHeight,upAxis,(PHY_ScalarType)heightDataType,flipQuadEdges);
}

IntPtr BulletAPI_BtHeightFieldferrainShape_getLocalScaling(IntPtr handle) // btVector3
{
    return (void *)&(GetBtHeightfieldTerrainShapeFromIntPtr(handle)->getLocalScaling());
}

void BulletAPI_BtHeightfieldTerrainShape_setLocalScaling(IntPtr handle, IntPtr scaling)//btVector3
{
    GetBtHeightfieldTerrainShapeFromIntPtr(handle)->setLocalScaling(*(btVector3 *)scaling);
}

void BulletAPI_BtHeightfieldTerrainShape_delete(IntPtr handle)
{
    delete GetBtHeightfieldTerrainShapeFromIntPtr(handle);
}