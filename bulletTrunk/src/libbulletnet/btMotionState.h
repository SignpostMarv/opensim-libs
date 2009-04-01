#include "main.h"

extern "C"
{
    EXPORT void BulletAPI_BtMotionState_getWorldTransform(IntPtr handle, IntPtr centerOfMassWorldTrans );
    EXPORT void BulletAPI_BtMotionState_setWorldTransform(IntPtr handle, IntPtr t);
    EXPORT void BulletAPI_BtMotionState_delete(IntPtr handle);
}