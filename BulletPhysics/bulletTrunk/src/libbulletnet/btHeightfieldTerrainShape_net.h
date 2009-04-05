#include "main.h"

extern "C"
{
    EXPORT IntPtr BulletAPI_CreateBtHeightfieldTerrainShape (int heightStickWidth, int heightStickLength, float * heightfieldData, float heightScale, float minHeight, float maxHeight,int upAxis, int heightDataType, bool flipQuadEdges);

    EXPORT IntPtr BulletAPI_BtHeightfieldTerrainShape_getLocalScaling(IntPtr handle); // btVector3
    EXPORT void BulletAPI_BtHeightfieldTerrainShape_setLocalScaling(IntPtr handle, IntPtr scaling);//btVector3
    EXPORT void BulletAPI_BtHeightfieldTerrainShape_delete(IntPtr handle);
}