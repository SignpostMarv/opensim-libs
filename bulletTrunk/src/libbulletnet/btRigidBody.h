#include "main.h"

extern "C"
{
    EXPORT IntPtr BulletAPI_CreateBtRigidBody(IntPtr constructionInfo);
    EXPORT IntPtr BulletAPI_CreateBtRigidBodyParams(float mass, IntPtr motionState, IntPtr collisionShape, IntPtr localInertia);
    EXPORT void BulletAPI_BtRigidBody_applyCentralForce(IntPtr handle, IntPtr force);//btVector3
    EXPORT void BulletAPI_BtRigidBody_applyCentralImpulse(IntPtr handle, IntPtr impulse);//btVector3
    EXPORT void BulletAPI_BtRigidBody_applyForce(IntPtr handle, IntPtr force, IntPtr rel_pos);//btVector3
    EXPORT void BulletAPI_BtRigidBody_applyImpulse(IntPtr handle, IntPtr impulse, IntPtr rel_pos);//btVector3
    EXPORT void BulletAPI_BtRigidBody_applyTorque(IntPtr handle, IntPtr torque);//btVector3
    EXPORT void BulletAPI_BtRigidBody_applyTorqueImpulse(IntPtr handle, IntPtr torque);//btVector3
    EXPORT void BulletAPI_BtRigidBody_clearForces(IntPtr handle);
    EXPORT float BulletAPI_BtRigidBody_computeAngularImpulseDenominator(IntPtr handle, IntPtr axis);//btVector3
    EXPORT float BulletAPI_BtRigidBody_computeImpulseDenominator(IntPtr handle, IntPtr pos, IntPtr normal);//btVector3
    EXPORT float BulletAPI_BtRigidBody_getAngularDamping(IntPtr handle);
    EXPORT IntPtr BulletAPI_BtRigidBody_getAngularFactor(IntPtr handle);
    EXPORT float BulletAPI_BtRigidBody_getAngularSleepingThreshold(IntPtr handle);
    EXPORT IntPtr BulletAPI_BtRigidBody_getAngularVelocity(IntPtr handle); //btVector3
    EXPORT float BulletAPI_BtRigidBody_getLinearSleepingThreshold(IntPtr handle);
    // getBroadPhaseProxy implemented in base
    EXPORT IntPtr BulletAPI_BtRigidBody_getCenterOfMassPosition(IntPtr handle); //btVector3
    EXPORT IntPtr BulletAPI_BtRigidBody_getCenterOfMassTransform(IntPtr handle); //btTransform
    // getCollisionShape implemented in base
    EXPORT IntPtr BulletAPI_BtRigidBody_getConstraintRef(IntPtr handle, int index); //btTypedConstraint
    EXPORT IntPtr BulletAPI_BtRigidBody_getGravity(IntPtr handle); //btVector3
    EXPORT IntPtr BulletAPI_BtRigidBody_getInvInertiaDiagLocal(IntPtr handle); //btVector3
    EXPORT IntPtr BulletAPI_BtRigidBody_getInvInertiaTensorWorld(IntPtr handle); //btMatrix3x3
    EXPORT float BulletAPI_BtRigidBody_getInvMass(IntPtr handle);
    EXPORT float BulletAPI_BtRigidBody_getLinearDamping(IntPtr handle);
    EXPORT IntPtr BulletAPI_BtRigidBody_getLinearVelocity(IntPtr handle);//btVector3
    EXPORT IntPtr BulletAPI_BtRigidBody_getMotionState(IntPtr handle);//btMotionState
    EXPORT int BulletAPI_BtRigidBody_getNumConstraintRefs(IntPtr handle);
    EXPORT IntPtr BulletAPI_BtRigidBody_getTotalForce(IntPtr handle);//btVector3
    EXPORT IntPtr BulletAPI_BtRigidBody_getTotalTorque(IntPtr handle);//btVector3
    EXPORT IntPtr BulletAPI_BtRigidBody_getVelocityInLocalPoint(IntPtr handle, IntPtr rel_pos);//btVector3
    EXPORT bool BulletAPI_BtRigidBody_isInWorld(IntPtr handle);
    EXPORT void BulletAPI_BtRigidBody_setAngularFactor(IntPtr handle, float angFac);
    EXPORT void BulletAPI_BtRigidBody_setAngularVelocity(IntPtr handle, IntPtr ang_vel);//btVector3
    EXPORT void BulletAPI_BtRigidBody_setInvInertiaDiagLocal(IntPtr handle, IntPtr diagInvInertia);//btVector3
    EXPORT void BulletAPI_BtRigidBody_setLinearVelocity(IntPtr handle, IntPtr lin_vel);//btVector3
    EXPORT void BulletAPI_BtRigidBody_setMotionState(IntPtr handle, IntPtr motionState);//btMotionState
    EXPORT void BulletAPI_BtRigidBody_setNewBroadphaseProxy(IntPtr handle, IntPtr broadphaseProxy);//btbroadphaseProxy
    EXPORT void BulletAPI_BtRigidBody_setSleepingThresholds(IntPtr handle, float linear, float angular);
    EXPORT void BulletAPI_BtRigidBody_translate(IntPtr handle, IntPtr v);//btVector3
    EXPORT void BulletAPI_BtRigidBody_updateDeactivation(IntPtr handle, float timeStep);
    
    EXPORT void BulletAPI_BtRigidBody_delete(IntPtr handle);
}