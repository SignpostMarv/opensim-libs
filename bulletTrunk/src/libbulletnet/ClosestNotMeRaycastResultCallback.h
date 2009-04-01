#include "main.h"

extern "C"
{
    EXPORT IntPtr BulletHelper_CreateClosestNotMeRaycastResultCallback(IntPtr body);
    EXPORT bool BulletHelper_ClosestNotMeRaycastResultCallback_hasHit(IntPtr handle);
    EXPORT IntPtr BulletHelper_ClosestNotMeRaycastResultCallback_getHitPointWorld(IntPtr handle);
    EXPORT void BulletHelper_ClosestNotMeRaycastResultCallback_delete(IntPtr handle);
}