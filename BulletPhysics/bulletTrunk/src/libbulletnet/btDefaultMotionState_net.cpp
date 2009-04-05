#include "btDefaultMotionState_net.h"

btDefaultMotionState *GetBtDefaultMotionStateFromIntPtr(IntPtr obj)
{
    return (btDefaultMotionState *) obj;
}

IntPtr BulletAPI_CreateBtDefaultMotionState()
{
    return new btDefaultMotionState();
}

IntPtr BulletAPI_CreateBtDefaultMotionStateStartTrans(IntPtr startTrans)
{
    return new btDefaultMotionState(*(btTransform *)startTrans);
}


IntPtr BulletAPI_CreateBtDefaultMotionStateStartTransCenterOfMassOffset(IntPtr startTrans, IntPtr centerOfMassOffset)
{
    return new btDefaultMotionState(*(btTransform *)startTrans, *(btTransform *)centerOfMassOffset );
}


void BulletAPI_BtDefaultMotionState_getWorldTransform(IntPtr handle, IntPtr centerOfMassWorldTrans )
{
    btTransform * result = (btTransform *)centerOfMassWorldTrans;
    GetBtDefaultMotionStateFromIntPtr(handle)->getWorldTransform(*result);
    centerOfMassWorldTrans = &result;
}


void BulletAPI_BtDefaultMotionState_setWorldTransform(IntPtr handle, IntPtr t)
{
    GetBtDefaultMotionStateFromIntPtr(handle)->setWorldTransform(*(btTransform *)t);
}


void BulletAPI_BtDefaultMotionState_delete(IntPtr handle)
{
    delete GetBtDefaultMotionStateFromIntPtr(handle);
}

