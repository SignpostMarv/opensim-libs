#include "ClosestNotMeRaycastResultCallback_net.h"
#include "BulletCollision/CollisionDispatch/btCollisionWorld.h"


class ClosestNotMeRaycastResultCallback : public btCollisionWorld::ClosestRayResultCallback
{
public:
	ClosestNotMeRaycastResultCallback (btRigidBody* me) : btCollisionWorld::ClosestRayResultCallback(btVector3(0.0, 0.0, 0.0), btVector3(0.0, 0.0, 0.0))
	{
		m_me = me;
	}

	virtual btScalar addSingleResult(btCollisionWorld::LocalRayResult& rayResult,bool normalInWorldSpace)
	{
		if (rayResult.m_collisionObject->getUserPointer() == m_me->getUserPointer())
			return 1.0;

		return ClosestRayResultCallback::addSingleResult (rayResult, normalInWorldSpace );
    }
protected:
	btRigidBody* m_me;
        //bool IsCallbackDefined;
        //EVENTCALLBACK _callback;
};

ClosestNotMeRaycastResultCallback *GetClosestNotMeRaycastResultCallbackFromIntPtr(IntPtr handle)
{
    return (ClosestNotMeRaycastResultCallback *)handle;
}


/*
ClosestNotMe rayCallback(m_rigidBody);

int i = 0;
for (i = 0; i < 2; i++)
{
	rayCallback.m_closestHitFraction = 1.0;
	collisionWorld->rayTest (m_raySource[i], m_rayTarget[i], rayCallback);
	if (rayCallback.hasHit())
	{
		m_rayLambda[i] = rayCallback.m_closestHitFraction;
	} else {
		m_rayLambda[i] = 1.0;
	}
}
*/


IntPtr BulletHelper_CreateClosestNotMeRaycastResultCallback(IntPtr body)
{
    return new ClosestNotMeRaycastResultCallback((btRigidBody *)body);
}

bool BulletHelper_ClosestNotMeRaycastResultCallback_hasHit(IntPtr handle)
{
    _FIX_BOOL_MARSHAL_BUG(GetClosestNotMeRaycastResultCallbackFromIntPtr(handle)->hasHit());
}

IntPtr BulletHelper_ClosestNotMeRaycastResultCallback_getHitPointWorld(IntPtr handle)
{
    btVector3 returnval = GetClosestNotMeRaycastResultCallbackFromIntPtr(handle)->m_hitPointWorld;
    
    if (returnval)
        return new btVector3(returnval.getX(), returnval.getY(), returnval.getZ());
    else
        return 0;
}

void BulletHelper_ClosestNotMeRaycastResultCallback_delete(IntPtr handle)
{
    delete GetClosestNotMeRaycastResultCallbackFromIntPtr(handle);
}
//_FIX_BOOL_MARSHAL_BUG(