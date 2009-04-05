#include "btRigidBodyConstructionInfo_net.h"
#include "BulletDynamics/Dynamics/btRigidBody.h"

btRigidBody::btRigidBodyConstructionInfo *GetBtRigidBodyConstructionInfoFromIntPtr(IntPtr handle)
{
    return (btRigidBody::btRigidBodyConstructionInfo *) handle;
}

IntPtr BulletAPI_CreateBtRigidBodyConstructionInfo()
{

    //btRigidBody::btRigidBodyConstructionInfo* rigidBodyConstructionInfo = 
    return new btRigidBody::btRigidBodyConstructionInfo(0,NULL,NULL);
}

void BulletAPI_BtRigidBodyConstructionInfo_Commit(IntPtr constructionInfoloc, 
            float mass, IntPtr motionState, IntPtr startWorldTransform, IntPtr collisionShape,
            IntPtr localInertia, float linearDamping, float angularDamping, float friction,
            float restitution, float linearSleepingThreshold, float angularSleepingThreshold,
            bool additionalDamping,float additionalDampingFactor,
            float additionalLinearDampingThresholdSqr,float additionalAngularDampingThresholdSqr,
            float additionalAngularDampingFactor)
{
    btRigidBody::btRigidBodyConstructionInfo* obj = (btRigidBody::btRigidBodyConstructionInfo *)constructionInfoloc;
    obj->m_mass = mass;
    obj->m_motionState = (btMotionState *)motionState;
    obj->m_startWorldTransform = *(btTransform *)startWorldTransform;
    obj->m_collisionShape = (btCollisionShape *)collisionShape;
    obj->m_localInertia = *(btVector3 *)localInertia;
    obj->m_linearDamping = linearDamping;
    obj->m_angularDamping = angularDamping;
    obj->m_friction = friction;
    obj->m_restitution = restitution;
    obj->m_linearSleepingThreshold = linearSleepingThreshold;
    obj->m_angularSleepingThreshold = angularSleepingThreshold;
    obj->m_additionalDamping = additionalDamping;
    obj->m_additionalDampingFactor = additionalDampingFactor;
    obj->m_additionalLinearDampingThresholdSqr = additionalLinearDampingThresholdSqr;
    obj->m_additionalAngularDampingThresholdSqr = additionalAngularDampingThresholdSqr;
    obj->m_additionalAngularDampingFactor = additionalAngularDampingFactor;

}

void BulletAPI_BtRigidBodyConstructionInfo_delete(IntPtr handle)
{
    delete GetBtRigidBodyConstructionInfoFromIntPtr(handle);
}