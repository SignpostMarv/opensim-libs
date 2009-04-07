#include "btCollisionObject_net.h"

btCollisionObject *GetBtCollisionObjectFromIntPtr(IntPtr obj)
{
    return (btCollisionObject *) obj;
}

IntPtr BulletAPI_CreateBtCollisionObject()
{
    return new btCollisionObject();
}

void BulletAPI_BtCollisionObject_activate(IntPtr handle, bool forceActivate)
{
    GetBtCollisionObjectFromIntPtr(handle)->activate(forceActivate);
}

bool BulletAPI_BtCollisionObject_checkCollideWith(IntPtr handle, IntPtr collisionObject)
{
    _FIX_BOOL_MARSHAL_BUG(GetBtCollisionObjectFromIntPtr(handle)->checkCollideWith((btCollisionObject *) collisionObject));
}

void BulletAPI_BtCollisionObject_forceActivationState(IntPtr handle, int newState)
{
    GetBtCollisionObjectFromIntPtr(handle)->forceActivationState(newState);
}

int BulletAPI_BtCollisionObject_getActivationState(IntPtr handle)
{
    return GetBtCollisionObjectFromIntPtr(handle)->getActivationState();
}

IntPtr BulletAPI_BtCollisionObject_getAnisotropicFriction(IntPtr handle)
{
    return (void *)&GetBtCollisionObjectFromIntPtr(handle)->getAnisotropicFriction();
}

IntPtr BulletAPI_BtCollisionObject_getBroadphaseHandle(IntPtr handle)
{
    return GetBtCollisionObjectFromIntPtr(handle)->getBroadphaseHandle();
}

float BulletAPI_BtCollisionObject_getCcdMotionThreshold(IntPtr handle)
{
    return GetBtCollisionObjectFromIntPtr(handle)->getCcdMotionThreshold();
}

float BulletAPI_BtCollisionObject_getCcdSquareMotionThreshold(IntPtr handle)
{
    return GetBtCollisionObjectFromIntPtr(handle)->getCcdSquareMotionThreshold();
}

float BulletAPI_BtCollisionObject_getCcdSweptSphereRadius(IntPtr handle)
{
    return GetBtCollisionObjectFromIntPtr(handle)->getCcdSweptSphereRadius();
}

int BulletAPI_BtCollisionObject_getCollisionFlags(IntPtr handle)
{
    return GetBtCollisionObjectFromIntPtr(handle)->getCollisionFlags();
}

IntPtr BulletAPI_BtCollisionObject_getCollisionShape(IntPtr handle)
{
    return GetBtCollisionObjectFromIntPtr(handle)->getCollisionShape();
}

int BulletAPI_BtCollisionObject_getCompanionId(IntPtr handle)
{
    return GetBtCollisionObjectFromIntPtr(handle)->getCompanionId();
}

float BulletAPI_BtCollisionObject_getContactProcessingThreshold(IntPtr handle)
{
    return GetBtCollisionObjectFromIntPtr(handle)->getContactProcessingThreshold();
}

float BulletAPI_BtCollisionObject_getDeactivationTime(IntPtr handle)
{
    return GetBtCollisionObjectFromIntPtr(handle)->getDeactivationTime();
}
float BulletAPI_BtCollisionObject_getFriction(IntPtr handle)
{
    return GetBtCollisionObjectFromIntPtr(handle)->getFriction();
}

float BulletAPI_BtCollisionObject_getHitFraction(IntPtr handle)
{
    return GetBtCollisionObjectFromIntPtr(handle)->getHitFraction();
}

IntPtr BulletAPI_BtCollisionObject_getInterpolationAngularVelocity(IntPtr handle)
{
    btVector3 ret = GetBtCollisionObjectFromIntPtr(handle)->getInterpolationAngularVelocity();
    return new btVector3(ret.getX(),ret.getY(),ret.getZ());
}

IntPtr BulletAPI_BtCollisionObject_getInterpolationLinearVelocity(IntPtr handle)
{
    btVector3 ret = GetBtCollisionObjectFromIntPtr(handle)->getInterpolationLinearVelocity();
    return new btVector3(ret.getX(),ret.getY(),ret.getZ());
}

IntPtr BulletAPI_BtCollisionObject_getInterpolationWorldTransform(IntPtr handle)
{
    btTransform obj = GetBtCollisionObjectFromIntPtr(handle)->getInterpolationWorldTransform();
    btVector3 origin = obj.getOrigin();
    btQuaternion quat = obj.getRotation();
    return new btTransform(quat,origin);
}

int BulletAPI_BtCollisionObject_getIslandTag(IntPtr handle)
{
    return GetBtCollisionObjectFromIntPtr(handle)->getIslandTag();
}

float BulletAPI_BtCollisionObject_getRestitution(IntPtr handle)
{
    return GetBtCollisionObjectFromIntPtr(handle)->getRestitution();
}

IntPtr BulletAPI_BtCollisionObject_getRootCollisionShape(IntPtr handle)
{
    return GetBtCollisionObjectFromIntPtr(handle)->getRootCollisionShape();
}

IntPtr BulletAPI_BtCollisionObject_getUserPointer(IntPtr handle)
{
    return GetBtCollisionObjectFromIntPtr(handle)->getUserPointer();
}

IntPtr BulletAPI_BtCollisionObject_getWorldTransform(IntPtr handle)
{
    btTransform obj = GetBtCollisionObjectFromIntPtr(handle)->getInterpolationWorldTransform();
    btVector3 origin = obj.getOrigin();
    btQuaternion quat = obj.getRotation();
    return new btTransform(quat,origin);
}

bool BulletAPI_BtCollisionObject_hasAnisotropicFriction(IntPtr handle)
{
    _FIX_BOOL_MARSHAL_BUG(GetBtCollisionObjectFromIntPtr(handle)->hasAnisotropicFriction());
}

bool BulletAPI_BtCollisionObject_hasContactResponse(IntPtr handle)
{
    _FIX_BOOL_MARSHAL_BUG(GetBtCollisionObjectFromIntPtr(handle)->hasContactResponse());
}

bool BulletAPI_BtCollisionObject_isActive(IntPtr handle)
{
    _FIX_BOOL_MARSHAL_BUG(GetBtCollisionObjectFromIntPtr(handle)->isActive());
}

bool BulletAPI_BtCollisionObject_isKinematicObject(IntPtr handle)
{
    _FIX_BOOL_MARSHAL_BUG(GetBtCollisionObjectFromIntPtr(handle)->isKinematicObject());
}

bool BulletAPI_BtCollisionObject_isStaticObject(IntPtr handle)
{
    _FIX_BOOL_MARSHAL_BUG(GetBtCollisionObjectFromIntPtr(handle)->isStaticObject());
}

bool BulletAPI_BtCollisionObject_isStaticOrKinematicObject(IntPtr handle)
{
    _FIX_BOOL_MARSHAL_BUG(GetBtCollisionObjectFromIntPtr(handle)->isStaticOrKinematicObject());
}

bool BulletAPI_BtCollisionObject_mergesSimulationIslands(IntPtr handle)
{
    _FIX_BOOL_MARSHAL_BUG(GetBtCollisionObjectFromIntPtr(handle)->mergesSimulationIslands());
}

void BulletAPI_BtCollisionObject_setActivationState(IntPtr handle, int newState)
{
    GetBtCollisionObjectFromIntPtr(handle)->setActivationState(newState);
}

void BulletAPI_BtCollisionObject_setAnisotropicFriction(IntPtr handle, IntPtr anisotropicFriction)
{
    GetBtCollisionObjectFromIntPtr(handle)->setAnisotropicFriction(*(btVector3 *)anisotropicFriction);
}

void BulletAPI_BtCollisionObject_setBroadphaseHandle(IntPtr handle, IntPtr bphandle)
{
    GetBtCollisionObjectFromIntPtr(handle)->setBroadphaseHandle((btBroadphaseProxy*) bphandle);
}

void BulletAPI_BtCollisionObject_setCcdMotionThreshold(IntPtr handle, float ccdMotionThreshold)
{
    GetBtCollisionObjectFromIntPtr(handle)->setCcdMotionThreshold(ccdMotionThreshold);
}

void BulletAPI_BtCollisionObject_setCcdSweptSphereRadius(IntPtr handle, float radius)
{
    GetBtCollisionObjectFromIntPtr(handle)->setCcdSweptSphereRadius(radius);
}

void BulletAPI_BtCollisionObject_setCollisionFlags(IntPtr handle, int flags)
{
    GetBtCollisionObjectFromIntPtr(handle)->setCollisionFlags(flags);
}

void BulletAPI_BtCollisionObject_setCollisionShape(IntPtr handle, IntPtr collisionShape)
{
    GetBtCollisionObjectFromIntPtr(handle)->setCollisionShape((btCollisionShape *)collisionShape);
}

void BulletAPI_BtCollisionObject_setCompanionId(IntPtr handle, int id)
{
    GetBtCollisionObjectFromIntPtr(handle)->setCompanionId(id);
}

void BulletAPI_BtCollisionObject_setContactProcessingThreshold(IntPtr handle, float contactProcessingThreshold)
{
    GetBtCollisionObjectFromIntPtr(handle)->setContactProcessingThreshold(contactProcessingThreshold);
}

void BulletAPI_BtCollisionObject_setDeactivationTime(IntPtr handle, float time)
{
    GetBtCollisionObjectFromIntPtr(handle)->setDeactivationTime(time);
}

void BulletAPI_BtCollisionObject_setFriction(IntPtr handle, float frict)
{
    GetBtCollisionObjectFromIntPtr(handle)->setFriction(frict);
}

void BulletAPI_BtCollisionObject_setHitFraction(IntPtr handle, float hitFraction)
{
    GetBtCollisionObjectFromIntPtr(handle)->setHitFraction(hitFraction);
}

void BulletAPI_BtCollisionObject_setInterpolationAngularVelocity(IntPtr handle, IntPtr angvel)
{
    GetBtCollisionObjectFromIntPtr(handle)->setInterpolationAngularVelocity(*(btVector3 *) angvel);
}

void BulletAPI_BtCollisionObject_setInterpolationLinearVelocity(IntPtr handle, IntPtr linvel)
{
    GetBtCollisionObjectFromIntPtr(handle)->setInterpolationLinearVelocity(*(btVector3 *) linvel);
}

void BulletAPI_BtCollisionObject_setInterpolationWorldTransform(IntPtr handle, IntPtr trans)
{
    GetBtCollisionObjectFromIntPtr(handle)->setInterpolationWorldTransform(*(btTransform *)trans);
}

void BulletAPI_BtCollisionObject_setIslandTag(IntPtr handle, int tag)
{
    GetBtCollisionObjectFromIntPtr(handle)->setIslandTag(tag);
}

void BulletAPI_BtCollisionObject_setRestitution(IntPtr handle, float rest)
{
    GetBtCollisionObjectFromIntPtr(handle)->setRestitution(rest);
}

void BulletAPI_BtCollisionObject_setUserPointer(IntPtr handle, IntPtr userPointer)
{
    GetBtCollisionObjectFromIntPtr(handle)->setUserPointer((void*)userPointer);
}

void BulletAPI_BtCollisionObject_setWorldTransform(IntPtr handle, IntPtr worldTrans)
{
    GetBtCollisionObjectFromIntPtr(handle)->setWorldTransform(*(btTransform *)worldTrans);
}

void BulletAPI_BtCollisionObject_delete(IntPtr handle)
{
    delete GetBtCollisionObjectFromIntPtr(handle);
}