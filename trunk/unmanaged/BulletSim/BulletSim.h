/*
 * Copyright (c) Contributors, http://opensimulator.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyrightD
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the OpenSimulator Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
#pragma once

#ifndef BULLET_SIM_H
#define BULLET_SIM_H

#include "ArchStuff.h"
#include "APIData.h"
#include "IPhysObject.h"
#include "TerrainObject.h"
#include "ObjectCollection.h"
#include "WorldData.h"
#include "BSLogger.h"

#include "BulletCollision/CollisionDispatch/btGhostObject.h"
#include "LinearMath/btAlignedObjectArray.h"
#include "LinearMath/btMotionState.h"
#include "btBulletDynamicsCommon.h"

#include <map>

// #define TOLERANCE 0.00001
// these values match the ones in SceneObjectPart.SendScheduledUpdates()
#define POSITION_TOLERANCE 0.05f
#define VELOCITY_TOLERANCE 0.001f
#define ROTATION_TOLERANCE 0.01f
#define ANGULARVELOCITY_TOLERANCE 0.01f

// TODO: find a way to build this
static char BulletSimVersionString[] = "v0002";

// Helper method to determine if an object is phantom or not
static bool IsPhantom(const btCollisionObject* obj)
{
	// Characters are never phantom, but everything else with CF_NO_CONTACT_RESPONSE is
	// TODO: figure out of this assumption for phantom sensing is still true
	return obj->getCollisionShape()->getShapeType() != CAPSULE_SHAPE_PROXYTYPE &&
		(obj->getCollisionFlags() & btCollisionObject::CF_NO_CONTACT_RESPONSE) != 0;
};

// ============================================================================================
// Callback to managed code for logging
// This is a callback into the managed code that writes a text message to the log.
// This callback is only initialized if the simulator is running in DEBUG mode.
typedef void DebugLogCallback(const char*);
extern DebugLogCallback* debugLogCallback;

// ============================================================================================
// Motion state for rigid bodies in the scene. Updates the map of changed 
// entities whenever the setWorldTransform callback is fired
class SimMotionState : public btMotionState
{
public:
	btRigidBody* RigidBody;
	Vector3 ZeroVect;

    SimMotionState(IDTYPE id, const btTransform& startTransform, std::map<IDTYPE, EntityProperties*>* updatesThisFrame)
		: m_properties(id, startTransform), m_lastProperties(id, startTransform)
	{
        m_xform = startTransform;
		m_updatesThisFrame = updatesThisFrame;
    }

    virtual ~SimMotionState()
	{
		m_updatesThisFrame->erase(m_properties.ID);
    }

    virtual void getWorldTransform(btTransform& worldTrans) const
	{
        worldTrans = m_xform;
    }

    virtual void setWorldTransform(const btTransform& worldTrans)
	{
		m_xform = worldTrans;

		// Put the new transform into m_properties
		m_properties.Position = m_xform.getOrigin();
		m_properties.Rotation = m_xform.getRotation();
		// A problem in stock Bullet is that we don't get an event when an object is deactivated.
		// This means that the last non-zero values for linear and angular velocity
		// are left in the viewer who does dead reconning and the objects look like
		// they float off.
		// BulletSim ships with a patch to Bullet which creates such an event.
		m_properties.Velocity = RigidBody->getLinearVelocity();
		m_properties.AngularVelocity = RigidBody->getAngularVelocity();

		// Is this transform any different from the previous one?
		if (   !m_properties.Position.AlmostEqual(m_lastProperties.Position, POSITION_TOLERANCE)
			|| !m_properties.Rotation.AlmostEqual(m_lastProperties.Rotation, ROTATION_TOLERANCE)
			// If the Velocity and AngularVelocity are zero, most likely the object has
			//    been deactivated. If they both are zero and they have become zero recently,
			//    make sure a property update is sent so the zeros make it to the viewer.
			|| ((m_properties.Velocity == ZeroVect && m_properties.AngularVelocity == ZeroVect)
				&& (m_properties.Velocity != m_lastProperties.Velocity || m_properties.AngularVelocity != m_lastProperties.AngularVelocity))
			//	If Velocity and AngularVelocity are non-zero but more than almost different, send an update.
			|| !m_properties.Velocity.AlmostEqual(m_lastProperties.Velocity, VELOCITY_TOLERANCE)
			|| !m_properties.AngularVelocity.AlmostEqual(m_lastProperties.AngularVelocity, ANGULARVELOCITY_TOLERANCE)
			)
		{
			// If so, update the previous transform and add this update to the list of 
			// updates this frame
			m_lastProperties = m_properties;
			(*m_updatesThisFrame)[m_properties.ID] = &m_properties;
		}
    }

private:
	std::map<IDTYPE, EntityProperties*>* m_updatesThisFrame;
    btTransform m_xform;
	EntityProperties m_properties;
	EntityProperties m_lastProperties;
};

// ============================================================================================
// Callback for convex sweeps that excludes the object being swept
class ClosestNotMeConvexResultCallback : public btCollisionWorld::ClosestConvexResultCallback
{
public:
	ClosestNotMeConvexResultCallback (btCollisionObject* me) : btCollisionWorld::ClosestConvexResultCallback(btVector3(0.0, 0.0, 0.0), btVector3(0.0, 0.0, 0.0))
	{
		m_me = me;
	}

	virtual btScalar addSingleResult(btCollisionWorld::LocalConvexResult& convexResult,bool normalInWorldSpace)
	{
		// Ignore collisions with ourself and phantom objects
		if (convexResult.m_hitCollisionObject == m_me || IsPhantom(convexResult.m_hitCollisionObject))
			return 1.0;

		return ClosestConvexResultCallback::addSingleResult (convexResult, normalInWorldSpace);
	}
protected:
	btCollisionObject* m_me;
};

// ============================================================================================
// Callback for raycasts that excludes the object doing the raycast
class ClosestNotMeRayResultCallback : public btCollisionWorld::ClosestRayResultCallback
{
public:
	ClosestNotMeRayResultCallback (btCollisionObject* me) : btCollisionWorld::ClosestRayResultCallback(btVector3(0.0, 0.0, 0.0), btVector3(0.0, 0.0, 0.0))
	{
		m_me = me;
	}

	virtual btScalar addSingleResult(btCollisionWorld::LocalRayResult& rayResult,bool normalInWorldSpace)
	{
		if (rayResult.m_collisionObject == m_me || IsPhantom(rayResult.m_collisionObject))
			return 1.0;

		return ClosestRayResultCallback::addSingleResult (rayResult, normalInWorldSpace);
	}
protected:
	btCollisionObject* m_me;
};

// ============================================================================================
// Callback for non-moving overlap tests
class ContactSensorCallback : public btCollisionWorld::ContactResultCallback
{
public:
	btVector3 mOffset;

	ContactSensorCallback(btCollisionObject* collider)
		: btCollisionWorld::ContactResultCallback(), m_me(collider), m_maxPenetration(0.0), mOffset(0.0, 0.0, 0.0)
	{
	}

	virtual	btScalar addSingleResult(btManifoldPoint& cp, const btCollisionObject* colObj0, int partId0, int index0, const btCollisionObject* colObj1, int partId1, int index1)
	{
		// Ignore terrain collisions
		if (colObj0->getCollisionShape()->getShapeType() == TRIANGLE_SHAPE_PROXYTYPE ||
			colObj1->getCollisionShape()->getShapeType() == TRIANGLE_SHAPE_PROXYTYPE)
		{
			return 0;
		}

		// Ignore collisions with phantom objects
		if (IsPhantom(colObj0) || IsPhantom(colObj1))
		{
			return 0;
		}

		btScalar distance = cp.getDistance();

		if (distance < m_maxPenetration)
		{
			m_maxPenetration = distance;

			// Figure out if we are the first or second body in the collision 
			// pair so we know which direction to point the collision normal
			btScalar directionSign = (colObj0 == m_me) ? btScalar(-1.0) : btScalar(1.0);

			// Push offset back using the collision normal and the depth of the collision
			btVector3 touchingNormal = cp.m_normalWorldOnB * directionSign;
			mOffset = touchingNormal * distance;
		}

		return 0;
	}

protected:
	btCollisionObject* m_me;
	btScalar m_maxPenetration;
};

// ============================================================================================
// The main physics simulation class.
class BulletSim
{
private:
	// Bullet world objects
	btBroadphaseInterface* m_broadphase;
	btCollisionDispatcher* m_dispatcher;
	btConstraintSolver*	m_solver;
	btDefaultCollisionConfiguration* m_collisionConfiguration;

	// Terrain and world metadata
	TerrainObject* m_terrainObject;

	// Information about the world that is shared with all the objects
	WorldData m_worldData;

	// Where we process the tick's updates for passing back to managed code
	int m_maxUpdatesPerFrame;
	EntityProperties* m_updatesThisFrameArray;

	// Used to expose colliders from Bullet to the BulletSim API
	int m_maxCollisionsPerFrame;
	CollisionDesc* m_collidersThisFrameArray;

	// Special avatar debugging stuff
	IPhysObject* m_lastAvatarObject;
	IDTYPE m_lastAvatarID;

public:

	BulletSim(btScalar maxX, btScalar maxY, btScalar maxZ);

	virtual ~BulletSim()
	{
		exitPhysics();
	}

	void initPhysics(ParamBlock* parms, int maxCollisions, CollisionDesc* collisionArray, int maxUpdates, EntityProperties* updateArray);
	void exitPhysics();

	int PhysicsStep(btScalar timeStep, int maxSubSteps, btScalar fixedTimeStep, 
		int* updatedEntityCount, EntityProperties** updatedEntities, int* collidersCount, CollisionDesc** colliders);
	void SetHeightmap(float* heightmap);

	bool RegisterStepCallback(IDTYPE id, IPhysObject* target);
	bool UnregisterStepCallback(IDTYPE id);

	bool CreateHull(unsigned long long meshKey, int hullCount, float* hulls);
	bool DestroyHull(unsigned long long meshKey);
	bool CreateMesh(unsigned long long meshKey, int indicesCount, int* indices, int verticesCount, float* vertices);
	bool DestroyMesh(unsigned long long id);

	bool CreateObject(ShapeData* shapeData);
	bool DestroyObject(IDTYPE id);
	bool HasObject(IDTYPE id);

	btVector3 GetObjectPosition(IDTYPE id);
	btQuaternion GetObjectOrientation(IDTYPE id);
	bool SetObjectTranslation(IDTYPE id, btVector3& position, btQuaternion& rotation);
	bool SetObjectVelocity(IDTYPE id, btVector3& velocity);
	bool SetObjectAngularVelocity(IDTYPE id, btVector3& angularVelocity);
	bool SetObjectForce(IDTYPE id, btVector3& force);
	bool SetObjectScaleMass(IDTYPE id, btVector3& scale, float mass, bool isDynamic);
	bool SetObjectCollidable(IDTYPE id, bool collidable);
	bool SetObjectDynamic(IDTYPE id, bool isDynamic, float mass);
	bool SetObjectBuoyancy(IDTYPE id, float buoyancy);
	bool SetObjectProperties(IDTYPE id, bool isStatic, bool isSolid, bool genCollisions, float mass);

	SweepHit ConvexSweepTest(IDTYPE id, btVector3& fromPos, btVector3& targetPos, btScalar extraMargin);
	RaycastHit RayTest(IDTYPE id, btVector3& from, btVector3& to);
	const btVector3 RecoverFromPenetration(IDTYPE id);

	WorldData* getWorldData() { return &m_worldData; }
	btDynamicsWorld* getDynamicsWorld() { return m_worldData.dynamicsWorld; };

	bool UpdateParameter(IDTYPE localID, const char* parm, float value);
	void DumpPhysicsStats();

protected:
	void CreateGroundPlane();
	void CreateTerrain();
};

#endif //BULLET_SIM_H
