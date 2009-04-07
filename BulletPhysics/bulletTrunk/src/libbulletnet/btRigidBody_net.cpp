#include "btRigidBody_net.h"

btRigidBody *GetBtRigidBodyFromIntPtr(IntPtr obj)
{
    return (btRigidBody *) obj;
}

IntPtr BulletAPI_CreateBtRigidBody(IntPtr constructionInfo)
{
    return new btRigidBody(*(btRigidBody::btRigidBodyConstructionInfo *)constructionInfo);
}

IntPtr BulletAPI_CreateBtRigidBodyParams(float mass, IntPtr motionState, IntPtr collisionShape, IntPtr localInertia)
{
    return new btRigidBody(mass,(btMotionState *)motionState, (btCollisionShape *) collisionShape, *(btVector3 *) localInertia);
}

void BulletAPI_BtRigidBody_applyCentralForce(IntPtr handle, IntPtr force)//btVector3
{
    GetBtRigidBodyFromIntPtr(handle)->applyCentralForce(*(btVector3 *)force);
}

void BulletAPI_BtRigidBody_applyCentralImpulse(IntPtr handle, IntPtr impulse)//btVector3
{
    GetBtRigidBodyFromIntPtr(handle)->applyCentralImpulse(*(btVector3 *)impulse);
}

void BulletAPI_BtRigidBody_applyForce(IntPtr handle, IntPtr force, IntPtr rel_pos)//btVector3
{
    GetBtRigidBodyFromIntPtr(handle)->applyForce(*(btVector3 *)force, *(btVector3 *)rel_pos);
}

void BulletAPI_BtRigidBody_applyImpulse(IntPtr handle, IntPtr impulse, IntPtr rel_pos)//btVector3
{
    GetBtRigidBodyFromIntPtr(handle)->applyImpulse(*(btVector3 *)impulse, *(btVector3 *)rel_pos);
}

void BulletAPI_BtRigidBody_applyTorque(IntPtr handle, IntPtr torque)//btVector3
{
    GetBtRigidBodyFromIntPtr(handle)->applyTorque(*(btVector3 *)torque);
}

void BulletAPI_BtRigidBody_applyTorqueImpulse(IntPtr handle, IntPtr torque)//btVector3
{
    GetBtRigidBodyFromIntPtr(handle)->applyTorqueImpulse(*(btVector3 *)torque);
}

void BulletAPI_BtRigidBody_clearForces(IntPtr handle)
{
    GetBtRigidBodyFromIntPtr(handle)->clearForces();
}

float BulletAPI_BtRigidBody_computeAngularImpulseDenominator(IntPtr handle, IntPtr axis)//btVector3
{
    return GetBtRigidBodyFromIntPtr(handle)->computeAngularImpulseDenominator(*(btVector3 *) axis);
}

float BulletAPI_BtRigidBody_computeImpulseDenominator(IntPtr handle, IntPtr pos, IntPtr normal)//btVector3
{
    return GetBtRigidBodyFromIntPtr(handle)->computeImpulseDenominator(*(btVector3 *) pos, *(btVector3 *) normal);
}

float BulletAPI_BtRigidBody_getAngularDamping(IntPtr handle)
{
    return GetBtRigidBodyFromIntPtr(handle)->getAngularDamping();
}

IntPtr BulletAPI_BtRigidBody_getAngularFactor(IntPtr handle)
{
    btVector3 result = GetBtRigidBodyFromIntPtr(handle)->getAngularFactor();
    return result;
}

float BulletAPI_BtRigidBody_getAngularSleepingThreshold(IntPtr handle)
{
    return GetBtRigidBodyFromIntPtr(handle)->getAngularSleepingThreshold();
}

IntPtr BulletAPI_BtRigidBody_getAngularVelocity(IntPtr handle) //btVector3
{
   btVector3 result = GetBtRigidBodyFromIntPtr(handle)->getAngularVelocity();
   return new btVector3(result.x(),result.y(),result.z());
}

float BulletAPI_BtRigidBody_getLinearSleepingThreshold(IntPtr handle)
{
    return GetBtRigidBodyFromIntPtr(handle)->getLinearSleepingThreshold();
}

    // getBroadPhaseProxy implemented in base
IntPtr BulletAPI_BtRigidBody_getCenterOfMassPosition(IntPtr handle) //btVector3
{
    btVector3 result = GetBtRigidBodyFromIntPtr(handle)->getCenterOfMassPosition();
    return new btVector3(result.x(),result.y(),result.z());
}

IntPtr BulletAPI_BtRigidBody_getCenterOfMassTransform(IntPtr handle) //btTransform
{
    //btTransform result = 
    
    btTransform obj = (GetBtRigidBodyFromIntPtr(handle)->getCenterOfMassTransform());
    btVector3 origin = obj.getOrigin();
    btQuaternion quat = obj.getRotation();
    return new btTransform(quat,origin);
}

    // getCollisionShape implemented in base
IntPtr BulletAPI_BtRigidBody_getConstraintRef(IntPtr handle, int index) //btTypedConstraint
{
    return GetBtRigidBodyFromIntPtr(handle)->getConstraintRef(index);
}

IntPtr BulletAPI_BtRigidBody_getGravity(IntPtr handle) //btVector3
{
    btVector3 result = GetBtRigidBodyFromIntPtr(handle)->getGravity();
    return new btVector3(result.x(),result.y(),result.z());
}

IntPtr BulletAPI_BtRigidBody_getInvInertiaDiagLocal(IntPtr handle) //btVector3
{
    
    btVector3 result = (GetBtRigidBodyFromIntPtr(handle)->getInvInertiaDiagLocal());
    return new btVector3(result.x(),result.y(),result.z());
}

IntPtr BulletAPI_BtRigidBody_getInvInertiaTensorWorld(IntPtr handle) //btMatrix3x3
{
    return new btMatrix3x3(GetBtRigidBodyFromIntPtr(handle)->getInvInertiaTensorWorld());
}

float BulletAPI_BtRigidBody_getInvMass(IntPtr handle)
{
    return GetBtRigidBodyFromIntPtr(handle)->getInvMass();
}

float BulletAPI_BtRigidBody_getLinearDamping(IntPtr handle)
{
    return GetBtRigidBodyFromIntPtr(handle)->getLinearDamping();
}

IntPtr BulletAPI_BtRigidBody_getLinearVelocity(IntPtr handle)//btVector3
{
    btVector3 result = (GetBtRigidBodyFromIntPtr(handle)->getLinearVelocity());
     return new btVector3(result.x(),result.y(),result.z());
}

IntPtr BulletAPI_BtRigidBody_getMotionState(IntPtr handle)//btMotionState
{
    return GetBtRigidBodyFromIntPtr(handle)->getMotionState();
}

int BulletAPI_BtRigidBody_getNumConstraintRefs(IntPtr handle)
{
    return GetBtRigidBodyFromIntPtr(handle)->getNumConstraintRefs();
}


IntPtr BulletAPI_BtRigidBody_getTotalForce(IntPtr handle)//btVector3
{
     btVector3 result = GetBtRigidBodyFromIntPtr(handle)->getTotalForce();
     return new btVector3(result.x(),result.y(),result.z());
}

IntPtr BulletAPI_BtRigidBody_getTotalTorque(IntPtr handle)//btVector3
{
    btVector3 result = GetBtRigidBodyFromIntPtr(handle)->getTotalTorque();
    return new btVector3(result.x(),result.y(),result.z());
}

IntPtr BulletAPI_BtRigidBody_getVelocityInLocalPoint(IntPtr handle, IntPtr rel_pos)//btVector3
{
    btVector3 result = GetBtRigidBodyFromIntPtr(handle)->getVelocityInLocalPoint(*(btVector3 *)rel_pos);
    return new btVector3(result.x(),result.y(),result.z());
}

bool BulletAPI_BtRigidBody_isInWorld(IntPtr handle)
{
    _FIX_BOOL_MARSHAL_BUG(GetBtRigidBodyFromIntPtr(handle)->isInWorld());
}

void BulletAPI_BtRigidBody_setAngularFactor(IntPtr handle, float angFac)
{
    GetBtRigidBodyFromIntPtr(handle)->setAngularFactor(angFac);
}

void BulletAPI_BtRigidBody_setAngularVelocity(IntPtr handle, IntPtr ang_vel)//btVector3
{
    GetBtRigidBodyFromIntPtr(handle)->setAngularVelocity(*(btVector3 *) ang_vel);
}

void BulletAPI_BtRigidBody_setInvInertiaDiagLocal(IntPtr handle, IntPtr diagInvInertia)//btVector3
{
    GetBtRigidBodyFromIntPtr(handle)->setInvInertiaDiagLocal(*(btVector3 *) diagInvInertia);
}

void BulletAPI_BtRigidBody_setLinearVelocity(IntPtr handle, IntPtr lin_vel)//btVector3
{
    GetBtRigidBodyFromIntPtr(handle)->setLinearVelocity(*(btVector3 *) lin_vel);
}

void BulletAPI_BtRigidBody_setMotionState(IntPtr handle, IntPtr motionState)//btMotionState
{
    GetBtRigidBodyFromIntPtr(handle)->setMotionState((btMotionState *) motionState);
}

void BulletAPI_BtRigidBody_setNewBroadphaseProxy(IntPtr handle, IntPtr broadphaseProxy)//btBroadphaseProxy
{
    GetBtRigidBodyFromIntPtr(handle)->setNewBroadphaseProxy((btBroadphaseProxy *)broadphaseProxy);
}

void BulletAPI_BtRigidBody_setSleepingThresholds(IntPtr handle, float linear, float angular)
{
    GetBtRigidBodyFromIntPtr(handle)->setSleepingThresholds(linear, angular);
}

void BulletAPI_BtRigidBody_translate(IntPtr handle, IntPtr v)//btVector3
{
    GetBtRigidBodyFromIntPtr(handle)->translate(*(btVector3 *) v);
}

void BulletAPI_BtRigidBody_updateDeactivation(IntPtr handle, float timeStep)
{
    GetBtRigidBodyFromIntPtr(handle)->updateDeactivation(timeStep);
}


void BulletAPI_BtRigidBody_delete(IntPtr handle)
{
    delete GetBtRigidBodyFromIntPtr(handle);
}