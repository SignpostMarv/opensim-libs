#include "main.h"

extern "C"
{
    EXPORT IntPtr BulletAPI_CreateBtDefaultMotionState();
    EXPORT IntPtr BulletAPI_CreateBtDefaultMotionStateStartTrans(IntPtr startTrans);
    EXPORT IntPtr BulletAPI_CreateBtDefaultMotionStateStartTransCenterOfMassOffset(IntPtr startTrans, IntPtr centerOfMassOffset);
    EXPORT void BulletAPI_BtDefaultMotionState_getWorldTransform(IntPtr handle, IntPtr centerOfMassWorldTrans );
    EXPORT void BulletAPI_BtDefaultMotionState_setWorldTransform(IntPtr handle, IntPtr t);
    EXPORT void BulletAPI_BtDefaultMotionState_delete(IntPtr handle);
}