#include "main.h"

extern "C"
{
    EXPORT IntPtr BulletAPI_CreateBtRigidBodyConstructionInfo();
    EXPORT void BulletAPI_BtRigidBodyConstructionInfo_Commit(IntPtr constructionInfoloc, 
            float mass, IntPtr motionState, IntPtr startWorldTransform, IntPtr collisionShape,
            IntPtr localInertia, float m_linearDamping, float m_angularDamping, float m_friction,
            float m_restitution, float m_linearSleepingThreshold, float m_angularSleepingThreshold,
            bool m_additionalDamping,float m_additionalDampingFactor,
            float m_additionalLinearDampingThresholdSqr,float m_additionalAngularDampingThresholdSqr,
            float m_additionalAngularDampingFactor);
    EXPORT void BulletAPI_BtRigidBodyConstructionInfo_delete(IntPtr handle);
}