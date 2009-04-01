#include "main.h"

extern "C"
{
    EXPORT IntPtr BulletAPI_CreateBtCollisionObject();
    EXPORT void BulletAPI_BtCollisionObject_activate(IntPtr handle, bool forceActivate);
    EXPORT bool BulletAPI_BtCollisionObject_checkCollideWith(IntPtr handle, IntPtr collisionObject);
    EXPORT void BulletAPI_BtCollisionObject_forceActivationState(IntPtr handle, int newState);
    EXPORT int BulletAPI_BtCollisionObject_getActivationState(IntPtr handle);
    EXPORT IntPtr BulletAPI_BtCollisionObject_getAnisotropicFriction(IntPtr handle);//btVector3
    EXPORT IntPtr BulletAPI_BtCollisionObject_getBroadphaseHandle(IntPtr handle); // btBroadPhaseProxy
    EXPORT float BulletAPI_BtCollisionObject_getCcdMotionThreshold(IntPtr handle);
    EXPORT float BulletAPI_BtCollisionObject_getCcdSquareMotionThreshold(IntPtr handle);
    EXPORT float BulletAPI_BtCollisionObject_getCcdSweptSphereRadius(IntPtr handle);
    EXPORT int BulletAPI_BtCollisionObject_getCollisionFlags(IntPtr handle);
    EXPORT IntPtr BulletAPI_BtCollisionObject_getCollisionShape(IntPtr handle); //btCollisionShape
    EXPORT int BulletAPI_BtCollisionObject_getCompanionId(IntPtr handle);
    EXPORT float BulletAPI_BtCollisionObject_getContactProcessingThreshold(IntPtr handle);
    EXPORT float BulletAPI_BtCollisionObject_getDeactivationTime(IntPtr handle);
    EXPORT float BulletAPI_BtCollisionObject_getFriction(IntPtr handle);
    EXPORT float BulletAPI_BtCollisionObject_getHitFraction(IntPtr handle);
    EXPORT IntPtr BulletAPI_BtCollisionObject_getInterpolationAngularVelocity(IntPtr handle);//btVector3
    EXPORT IntPtr BulletAPI_BtCollisionObject_getInterpolationLinearVelocity(IntPtr handle);//btVector3
    EXPORT IntPtr BulletAPI_BtCollisionObject_getInterpolationWorldTransform(IntPtr handle);//btTransform
    EXPORT int BulletAPI_BtCollisionObject_getIslandTag(IntPtr handle);
    EXPORT float BulletAPI_BtCollisionObject_getRestitution(IntPtr handle);
    EXPORT IntPtr BulletAPI_BtCollisionObject_getRootCollisionShape(IntPtr handle); //btCollisionShape
    EXPORT IntPtr BulletAPI_BtCollisionObject_getUserPointer(IntPtr handle); // pointer
    EXPORT IntPtr BulletAPI_BtCollisionObject_getWorldTransform(IntPtr handle); //btTransform
    EXPORT bool BulletAPI_BtCollisionObject_hasAnisotropicFriction(IntPtr handle);
    EXPORT bool BulletAPI_BtCollisionObject_hasContactResponse(IntPtr handle);
    EXPORT bool BulletAPI_BtCollisionObject_isActive(IntPtr handle);
    EXPORT bool BulletAPI_BtCollisionObject_isKinematicObject(IntPtr handle);
    EXPORT bool BulletAPI_BtCollisionObject_isStaticObject(IntPtr handle);
    EXPORT bool BulletAPI_BtCollisionObject_isStaticOrKinematicObject(IntPtr handle);
    EXPORT bool BulletAPI_BtCollisionObject_mergesSimulationIslands(IntPtr handle);
    EXPORT void BulletAPI_BtCollisionObject_setActivationState(IntPtr handle, int newState);
    EXPORT void BulletAPI_BtCollisionObject_setAnisotropicFriction(IntPtr handle, IntPtr anisotropicFriction);
    EXPORT void BulletAPI_BtCollisionObject_setBroadphaseHandle(IntPtr handle, IntPtr bphandle);
    EXPORT void BulletAPI_BtCollisionObject_setCcdMotionThreshold(IntPtr handle, float ccdMotionThreshold);
    EXPORT void BulletAPI_BtCollisionObject_setCcdSweptSphereRadius(IntPtr handle, float radius);
    EXPORT void BulletAPI_BtCollisionObject_setCollisionFlags(IntPtr handle, int flags);
    EXPORT void BulletAPI_BtCollisionObject_setCollisionShape(IntPtr handle, IntPtr collisionShape);
    EXPORT void BulletAPI_BtCollisionObject_setCompanionId(IntPtr handle, int id);
    EXPORT void BulletAPI_BtCollisionObject_setContactProcessingThreshold(IntPtr handle, float contactProcessingThreshold);
    EXPORT void BulletAPI_BtCollisionObject_setDeactivationTime(IntPtr handle, float time);
    EXPORT void BulletAPI_BtCollisionObject_setFriction(IntPtr handle, float frict);
    EXPORT void BulletAPI_BtCollisionObject_setHitFraction(IntPtr handle, float hitFraction);
    EXPORT void BulletAPI_BtCollisionObject_setInterpolationAngularVelocity(IntPtr handle, IntPtr angvel);
    EXPORT void BulletAPI_BtCollisionObject_setInterpolationLinearVelocity(IntPtr handle, IntPtr linvel);
    EXPORT void BulletAPI_BtCollisionObject_setInterpolationWorldTransform(IntPtr handle, IntPtr trans);
    EXPORT void BulletAPI_BtCollisionObject_setIslandTag(IntPtr handle, int tag);
    EXPORT void BulletAPI_BtCollisionObject_setRestitution(IntPtr handle, float rest);
    EXPORT void BulletAPI_BtCollisionObject_setUserPointer(IntPtr handle, IntPtr userPointer);
    EXPORT void BulletAPI_BtCollisionObject_setWorldTransform(IntPtr handle, IntPtr worldTrans);
    EXPORT void BulletAPI_BtCollisionObject_delete(IntPtr handle);
}