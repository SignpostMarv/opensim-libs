#include "main.h"

extern "C"
{
    EXPORT IntPtr BulletAPI_CreateBtGhostObject();
    EXPORT int BulletAPI_BtGhostObject_getNumOverlappingObjects(IntPtr handle);
    EXPORT IntPtr BulletAPI_BtGhostObject_getOverlappingObject(IntPtr handle);
    EXPORT void BulletAPI_BtGhostObject_delete(IntPtr handle);
    EXPORT IntPtr BulletAPI_CreateBtPairCachingGhostObject();
    EXPORT void BulletAPI_BtPairCachingGhostObject_delete(IntPtr handle);
}