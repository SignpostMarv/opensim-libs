#include "main.h"


extern "C"
{
    EXPORT IntPtr BulletAPI_CreateBtCollisionDispatcher(IntPtr config);
    // This is non-bullet API, but here so that one can register GImpact and have GImpactMeshShape
    EXPORT void BulletAPI_BtCollisionDispatcher_RegisterGImpact(IntPtr handle);
    EXPORT void BulletAPI_BtCollisionDispatcher_delete(IntPtr obj);
}