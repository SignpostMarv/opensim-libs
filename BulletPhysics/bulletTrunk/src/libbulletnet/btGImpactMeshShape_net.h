#include "main.h"

extern "C"
{
    EXPORT IntPtr BulletAPI_CreateBtGImpactMeshShape(IntPtr meshInterface);
    EXPORT void BulletAPI_BtGImpactMeshShape_updateBound(IntPtr handle);
    EXPORT void BulletAPI_BtGImpactMeshShape_delete(IntPtr handle);
}