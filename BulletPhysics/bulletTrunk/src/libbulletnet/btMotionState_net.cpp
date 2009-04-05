#include "btMotionState_net.h"

btMotionState *GetBtMotionStateFromIntPtr(IntPtr obj)
{
    return (btMotionState *) obj;
}


void BulletAPI_BtMotionState_getWorldTransform(IntPtr handle, IntPtr centerOfMassWorldTrans )
{
    btTransform * result = (btTransform *)centerOfMassWorldTrans;
    GetBtMotionStateFromIntPtr(handle)->getWorldTransform(*result);
    centerOfMassWorldTrans = &result;
}


void BulletAPI_BtMotionState_setWorldTransform(IntPtr handle, IntPtr t)
{
    GetBtMotionStateFromIntPtr(handle)->setWorldTransform(*(btTransform *)t);
}


void BulletAPI_BtMotionState_delete(IntPtr handle)
{
    delete GetBtMotionStateFromIntPtr(handle);
}
