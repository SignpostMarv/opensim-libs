#include "btDiscreteDynamicsWorld_net.h"

btDiscreteDynamicsWorld *GetBtDiscreteDynamicsWorldFromIntPtr(IntPtr object)
{
    return (btDiscreteDynamicsWorld*) object;
}


IntPtr BulletAPI_CreateBtDynamicsWorld(IntPtr dispatcher, IntPtr broadphase, IntPtr solver, IntPtr collisionconfig)
{
    btDiscreteDynamicsWorld* wrld = new btDiscreteDynamicsWorld(
        (btDispatcher *) dispatcher, 
        (btBroadphaseInterface *) broadphase, 
        (btConstraintSolver *) solver, 
        (btCollisionConfiguration *) collisionconfig
        );
    return wrld;
}

void BulletAPI_BtDynamicsWorld_addRigidBody(IntPtr handle, IntPtr body)
{
    GetBtDiscreteDynamicsWorldFromIntPtr(handle)->addRigidBody((btRigidBody *)body);
}
void BulletAPI_BtDynamicsWorld_removeRigidBody(IntPtr handle, IntPtr body)
{
    GetBtDiscreteDynamicsWorldFromIntPtr(handle)->removeRigidBody((btRigidBody *)body);
}

void BulletAPI_BtDynamicsWorld_addConstraint(IntPtr handle, IntPtr constraint, bool disableLinkedCollisions)
{
    GetBtDiscreteDynamicsWorldFromIntPtr(handle)->addConstraint((btTypedConstraint *) constraint,disableLinkedCollisions);
}

void BulletAPI_BtDynamicsWorld_setGravity(IntPtr handle, IntPtr v)
{
    GetBtDiscreteDynamicsWorldFromIntPtr(handle)->setGravity(*(btVector3 *) v);
}
int BulletAPI_BtDynamicsWorld_stepSimulation(IntPtr handle, float timeStep, int maxSubSteps, float fixedTimeStep)
{
    int result = GetBtDiscreteDynamicsWorldFromIntPtr(handle)->stepSimulation(timeStep,maxSubSteps, fixedTimeStep);
    return result;
}

void BulletAPI_BtDynamicsWorld_removeConstraint(IntPtr handle, IntPtr constraint)
{
    GetBtDiscreteDynamicsWorldFromIntPtr(handle)->removeConstraint((btTypedConstraint *) constraint);
}

void BulletAPI_BtDynamicsWorld_delete(IntPtr obj)
{
    btDiscreteDynamicsWorld* wrld = GetBtDiscreteDynamicsWorldFromIntPtr(obj);
    delete wrld;
}