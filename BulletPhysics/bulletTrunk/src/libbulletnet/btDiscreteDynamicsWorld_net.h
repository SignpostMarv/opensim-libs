#include "main.h"

extern "C"
{
    EXPORT IntPtr BulletAPI_CreateBtDynamicsWorld(IntPtr dispatcher, IntPtr broadphase, IntPtr solver, IntPtr collisionconfig);
    EXPORT void BulletAPI_BtDynamicsWorld_addConstraint(IntPtr handle, IntPtr constraint, bool disableLinkedCollisions);
    EXPORT void BulletAPI_BtDynamicsWorld_setGravity(IntPtr handle, IntPtr v);
    EXPORT int BulletAPI_BtDynamicsWorld_stepSimulation(IntPtr handle, float timeStep, int maxSubSteps, float fixedTimeStep);
    EXPORT void BulletAPI_BtDynamicsWorld_addRigidBody(IntPtr handle, IntPtr body);
    
    EXPORT void BulletAPI_BtDynamicsWorld_removeRigidBody(IntPtr handle, IntPtr body);
    EXPORT void BulletAPI_BtDynamicsWorld_removeConstraint(IntPtr handle, IntPtr constraint);
    EXPORT void BulletAPI_BtDynamicsWorld_delete(IntPtr obj);
}