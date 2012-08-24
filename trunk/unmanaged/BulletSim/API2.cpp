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

#include "BulletSim.h"
#include <stdarg.h>

#include "BulletCollision/CollisionShapes/btHeightfieldTerrainShape.h"

#include <map>

#ifdef WIN32
	#define DLL_EXPORT __declspec( dllexport )
	#define DLL_IMPORT __declspec( dllimport )
#else
	#define DLL_EXPORT
	#define DLL_IMPORT
#endif
#ifdef __cplusplus
    #define EXTERN_C extern "C"
#else
    #define EXTERN_C extern
#endif

#pragma warning( disable: 4190 ) // Warning about returning Vector3 that we can safely ignore

// A structure for keeping track of terrain heightmaps
struct HeightMapThing {
	int width;
	int length;
	btScalar minHeight;
	btScalar maxHeight;
	btVector3 minCoords;
	btVector3 maxCoords;
	float* heightMap;
};

/**
 * Returns a string that identifies the version of the BulletSim.dll
 * @return static string of version information
 */
EXTERN_C DLL_EXPORT char* GetVersion2()
{
	return &BulletSimVersionString[0];
}

/**
 * Initializes the physical simulation.
 * @param maxPosition Top north-east corner of the simulation, with Z being up. The bottom south-west corner is 0,0,0.
 * @param maxCollisions maximum number of collisions that can be reported each tick
 * @param updateArray pointer to pinned memory to return the collision info
 * @param maxUpdates maximum number of property updates that can be reported each tick
 * @param maxCollisions pointer to pinned memory to return the update information
 * @return pointer to the created simulator
 */
EXTERN_C DLL_EXPORT BulletSim* Initialize2(Vector3 maxPosition, ParamBlock* parms,
											int maxCollisions, CollisionDesc* collisionArray,
											int maxUpdates, EntityProperties* updateArray)
{
	BulletSim* sim = new BulletSim(maxPosition.X, maxPosition.Y, maxPosition.Z);
	sim->initPhysics(parms, maxCollisions, collisionArray, maxUpdates, updateArray);

	return sim;
}

/**
 * Update the internal value of a parameter. Some parameters require changing world state.
 * @param worldID ID of the world to change the paramter in
 * @param localID ID of the object to change the paramter on or special values for NONE or ALL
 * @param parm the name of the parameter to change (must be passed in as lower case)
 * @param value the value to change the parameter to
 */
EXTERN_C DLL_EXPORT bool UpdateParameter2(BulletSim* sim, unsigned int localID, const char* parm, float value)
{
	return sim->UpdateParameter(localID, parm, value);
}

/**
 * Shuts down the physical simulation.
 * @param worldID ID of the world to shut down.
 */
EXTERN_C DLL_EXPORT void Shutdown2(BulletSim* sim)
{
	sim->exitPhysics();
	delete sim;
}

/**
 * Steps the simulation forward a given amount of time and retrieves any physics updates.
 * @param worldID ID of the world to step.
 * @param timeStep Length of time in seconds to move the simulation forward.
 * @param maxSubSteps Clamps the maximum number of fixed duration sub steps taken this step.
 * @param fixedTimeStep Length in seconds of the sub steps Bullet actually uses for simulation. Example: 1.0 / TARGET_FPS.
 * @param updatedEntityCount Pointer to the number of EntityProperties generated this call.
 * @param updatedEntities Pointer to an array of pointers to EntityProperties containing physics updates generated this call.
 * @param collidersCount Pointer to the number of colliders detected this call.
 * @param colliders Pointer to an array of colliding object IDs (in pairs of two).
 * @return Number of sub steps that were taken this call.
 */
EXTERN_C DLL_EXPORT int PhysicsStep2(BulletSim* sim, float timeStep, int maxSubSteps, float fixedTimeStep, 
			int* updatedEntityCount, EntityProperties** updatedEntities, int* collidersCount, CollisionDesc** colliders)
{
	return sim->PhysicsStep(timeStep, maxSubSteps, fixedTimeStep, updatedEntityCount, updatedEntities, collidersCount, colliders);
}

// Cause a position update to happen next physics step.
// This works by placing an entry for this object in the SimMotionState's
//    update event array. This will be sent to the simulator after the
//    physics step has added other updates.
EXTERN_C DLL_EXPORT bool PushUpdate2(btCollisionObject* obj)
{
	bool ret = false;
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb != NULL)
	{
		SimMotionState* sms = (SimMotionState*)rb->getMotionState();
		if (sms != NULL)
		{
			btTransform wt;
			sms->getWorldTransform(wt);
			sms->setWorldTransform(wt, true);
			ret = true;
		}
	}
	return ret;
}

EXTERN_C DLL_EXPORT btCollisionShape* CreateMeshShape2(BulletSim* sim, 
						int indicesCount, int* indices, int verticesCount, float* vertices )
{
	return sim->CreateMeshShape2(indicesCount, indices, verticesCount, vertices);
}

EXTERN_C DLL_EXPORT btCollisionShape* CreateHullShape2(BulletSim* sim, 
						int hullCount, float* hulls )
{
	return sim->CreateHullShape2(hullCount, hulls);
}

EXTERN_C DLL_EXPORT btCollisionShape* BuildHullShape2(BulletSim* sim, btCollisionShape* mesh) {
	return sim->BuildHullShape2(mesh);
}

EXTERN_C DLL_EXPORT btCollisionShape* CreateNativeShape2(BulletSim* sim,
						float shapeType, float collisionMargin, Vector3 scale)
{
	btCollisionShape* shape = NULL;
	switch ((int)shapeType)
	{
		case ShapeData::SHAPE_BOX:
			// btBoxShape subtracts the collision margin from the half extents, so no 
			// fiddling with scale necessary
			// boxes are defined by their half extents
			shape = new btBoxShape(btVector3(0.5, 0.5, 0.5));	// this is really a unit box
			break;
		// case ShapeData::SHAPE_CONE:	// TODO:
		// 	shape = new btConeShapeZ(0.5, 1.0);
		// 	break;
		// case ShapeData::SHAPE_CYLINDER:	// TODO:
		// 	shape = new btCylinderShapeZ(btVector3(0.5f, 0.5f, 0.5f));
		// 	break;
		case ShapeData::SHAPE_SPHERE:
			shape = new btSphereShape(0.5);		// this is really a unit sphere
			break;
	}
	if (shape != NULL)
	{
		shape->setMargin(btScalar(collisionMargin));
		shape->setLocalScaling(scale.GetBtVector3());
	}

	return shape;
}

EXTERN_C DLL_EXPORT bool DeleteCollisionShape2(BulletSim* sim, btCollisionShape* shape)
{
	delete shape;
	return true;
}

// =====================================================================
EXTERN_C DLL_EXPORT btCollisionObject* CreateTerrainBody2(
	IDTYPE id,
	HeightMapThing* mapInfo,
	float collisionMargin
	)
{
	const int upAxis = 2;
	const btScalar scaleFactor(1.0);
	btHeightfieldTerrainShape* heightfieldShape = new btHeightfieldTerrainShape(
			mapInfo->width, mapInfo->length, 
			mapInfo->heightMap, scaleFactor, 
			mapInfo->minHeight, mapInfo->maxHeight, upAxis, PHY_FLOAT, false);
	// there is no room between the terrain and an object
	heightfieldShape->setMargin(btScalar(collisionMargin));
	// m_heightfieldShape->setMargin(gCollisionMargin);
	heightfieldShape->setUseDiamondSubdivision(true);

	// Add the localID to the object so we know about collisions
	heightfieldShape->setUserPointer(PACKLOCALID(id));

	// Compute and set the heightfield origin
	btTransform heightfieldTr;
	heightfieldTr.setIdentity();
	heightfieldTr.setOrigin(btVector3(
			((float)mapInfo->width) * 0.5f + mapInfo->minCoords.getX(),
			((float)length) * 0.5f + mapInfo->minCoords.getY(),
			mapInfo->minHeight + (mapInfo->maxHeight - mapInfo->minHeight) * 0.5f));

	// Use the default motion state since we are not interested in the
	//   terrain reporting its collisions. Other objects will report their
	//   collisions with the terrain.
	btDefaultMotionState* motionState = new btDefaultMotionState(heightfieldTr);
	btRigidBody::btRigidBodyConstructionInfo cInfo(0.0, motionState, heightfieldShape);
	btRigidBody* body = new btRigidBody(cInfo);

	body->setCollisionFlags(btCollisionObject::CF_STATIC_OBJECT);

	return body;
}

EXTERN_C DLL_EXPORT btCollisionObject* CreateGroundPlaneBody2(
	IDTYPE id,
	float height,	// usually 1
	float collisionMargin)
{
	// Initialize the ground plane
	btVector3 groundPlaneNormal = btVector3(0, 0, 1);	// Z up
	btStaticPlaneShape* m_planeShape = new btStaticPlaneShape(groundPlaneNormal, (int)height);
	m_planeShape->setMargin(collisionMargin);

	m_planeShape->setUserPointer(PACKLOCALID(id));

	btDefaultMotionState* motionState = new btDefaultMotionState();
	btRigidBody::btRigidBodyConstructionInfo cInfo(0.0, motionState, m_planeShape);
	btRigidBody* body = new btRigidBody(cInfo);

	body->setCollisionFlags(btCollisionObject::CF_STATIC_OBJECT);

	return body;
}

// Bullet requires us to manage the heightmap array so these methods create
//    and release the memory for the heightmap.
EXTERN_C DLL_EXPORT HeightMapThing* CreateHeightmapArray(Vector3 minCoords, Vector3 maxCoords, float initialValue)
{
	HeightMapThing* mapInfo;

	mapInfo = new HeightMapThing();

	mapInfo->minCoords = minCoords.GetBtVector3();
	mapInfo->maxCoords = maxCoords.GetBtVector3();

	mapInfo->width = (int)(maxCoords.X - minCoords.X);
	mapInfo->length = (int)(maxCoords.Y - minCoords.Y);

	mapInfo->minHeight = btScalar(minCoords.Z);
	mapInfo->maxHeight = btScalar(maxCoords.Z);
	if (mapInfo->minHeight == mapInfo->maxHeight)
		mapInfo->minHeight = mapInfo->maxHeight - 1.0;

	int numEntries = mapInfo->width * mapInfo->length;

	float* heightMap = new float[numEntries];
	mapInfo->heightMap = heightMap;

	for (int xx = 0; xx < numEntries; xx++)
		heightMap[xx] = initialValue;

	return mapInfo;
}

EXTERN_C DLL_EXPORT bool ReleaseHeightMapThing(HeightMapThing* mapInfo)
{
	delete mapInfo->heightMap;
	delete mapInfo;
	return true;
}

// Given a previously allocated heightmap array and a new array of values, copy
//    the new values into the array being used by Bullet.
EXTERN_C DLL_EXPORT void UpdateHeightmap2(BulletSim* sim, HeightMapThing* mapInfo, float* newHeightMap)
{
	int size = sizeof(mapInfo->heightMap) / sizeof(float);
	for (int xx = 0; xx < size; xx++)
		mapInfo->heightMap[xx] = newHeightMap[xx];
	return;
}

// =====================================================================
/**
 * Add a generic 6 degree of freedom constraint between two previously created objects
 * @param sim pointer to BulletSim instance this creation is to be in
 * @param id1 first object to constrain
 * @param id2 other object to constrain
 * @param lowLinear low bounds of linear constraint
 * @param hiLinear hi bounds of linear constraint
 * @param lowAngular low bounds of angular constraint
 * @param hiAngular hi bounds of angular constraint
 * @param 'true' if to use FrameA as reference for the constraint action
 * @param 'true' if disable collsions between the constrained objects
 */
EXTERN_C DLL_EXPORT btTypedConstraint* Create6DofConstraint2(BulletSim* sim, btCollisionObject* obj1, btCollisionObject* obj2,
				Vector3 frame1loc, Quaternion frame1rot,
				Vector3 frame2loc, Quaternion frame2rot,
				bool useLinearReferenceFrameA, bool disableCollisionsBetweenLinkedBodies)
{
	btTransform frame1t, frame2t;
	frame1t.setIdentity();
	frame1t.setOrigin(frame1loc.GetBtVector3());
	frame1t.setRotation(frame1rot.GetBtQuaternion());
	frame2t.setIdentity();
	frame2t.setOrigin(frame2loc.GetBtVector3());
	frame2t.setRotation(frame2rot.GetBtQuaternion());

	btRigidBody* rb1 = btRigidBody::upcast(obj1);
	btRigidBody* rb2 = btRigidBody::upcast(obj2);

	btGeneric6DofConstraint* constrain = NULL;
	if (rb1 != NULL && rb2 != NULL)
	{
		constrain = new btGeneric6DofConstraint(*rb1, *rb2, frame1t, frame2t, useLinearReferenceFrameA);

		constrain->calculateTransforms();
		sim->getDynamicsWorld()->addConstraint(constrain, disableCollisionsBetweenLinkedBodies);
	}

	// BSLog("CreateConstraint2: loc=%x, body1=%u, body2=%u", constrain,
	// 					CONVLOCALID(obj1->getCollisionShape()->getUserPointer()),
	// 					CONVLOCALID(obj2->getCollisionShape()->getUserPointer()));
	// BSLog("          f1=<%f,%f,%f>, f1r=<%f,%f,%f,%f>, f2=<%f,%f,%f>, f2r=<%f,%f,%f,%f>",
	// 					frame1loc.X, frame1loc.Y, frame1loc.Z, frame1rot.X, frame1rot.Y, frame1rot.Z, frame1rot.W,
	// 					frame2loc.X, frame2loc.Y, frame2loc.Z, frame2rot.X, frame2rot.Y, frame2rot.Z, frame2rot.W);
	return constrain;
}

// Create a 6Dof constraint between two objects and around the given world point.
EXTERN_C DLL_EXPORT btTypedConstraint* Create6DofConstraintToPoint2(BulletSim* sim, btCollisionObject* obj1, btCollisionObject* obj2,
				Vector3 joinPoint,
				Vector3 frame2loc, Quaternion frame2rot,
				bool useLinearReferenceFrameA, bool disableCollisionsBetweenLinkedBodies)
{
	btGeneric6DofConstraint* constrain = NULL;

	btRigidBody* rb1 = btRigidBody::upcast(obj1);
	btRigidBody* rb2 = btRigidBody::upcast(obj2);

	if (rb1 != NULL && rb2 != NULL)
	{
		// following example at http://bulletphysics.org/Bullet/phpBB3/viewtopic.php?t=5851
		btTransform joinPointt, frame1t, frame2t;
		joinPointt.setIdentity();
		joinPointt.setOrigin(joinPoint.GetBtVector3());
		frame1t = rb1->getWorldTransform().inverse() * joinPointt;
		frame2t = rb2->getWorldTransform().inverse() * joinPointt;

		constrain = new btGeneric6DofConstraint(*rb1, *rb2, frame1t, frame2t, useLinearReferenceFrameA);

		sim->getDynamicsWorld()->addConstraint(constrain, disableCollisionsBetweenLinkedBodies);

		// BSLog("CreateConstraint2: loc=%x, body1=%u, body2=%u", constrain,
		// 					CONVLOCALID(obj1->getCollisionShape()->getUserPointer()),
		// 					CONVLOCALID(obj2->getCollisionShape()->getUserPointer()));
		// BSLog("          f1=<%f,%f,%f>, f1r=<%f,%f,%f,%f>, f2=<%f,%f,%f>, f2r=<%f,%f,%f,%f>",
		// 					frame1loc.X, frame1loc.Y, frame1loc.Z, frame1rot.X, frame1rot.Y, frame1rot.Z, frame1rot.W,
		// 					frame2loc.X, frame2loc.Y, frame2loc.Z, frame2rot.X, frame2rot.Y, frame2rot.Z, frame2rot.W);
	}

	return constrain;
}

EXTERN_C DLL_EXPORT btTypedConstraint* CreateHingeConstraint2(BulletSim* sim, btCollisionObject* obj1, btCollisionObject* obj2,
						Vector3 pivotInA, Vector3 pivotInB,
						Vector3 axisInA, Vector3 axisInB,
						bool useReferenceFrameA,
						bool disableCollisionsBetweenLinkedBodies
						)
{
	btHingeConstraint* constrain = NULL;

	btRigidBody* rb1 = btRigidBody::upcast(obj1);
	btRigidBody* rb2 = btRigidBody::upcast(obj2);

	if (rb1 != NULL && rb2 != NULL)
	{
		constrain = new btHingeConstraint(*rb1, *rb2, 
									pivotInA.GetBtVector3(), pivotInB.GetBtVector3(), 
									axisInA.GetBtVector3(), axisInB.GetBtVector3(), 
									useReferenceFrameA);

		sim->getDynamicsWorld()->addConstraint(constrain, disableCollisionsBetweenLinkedBodies);
	}

	return constrain;
}

EXTERN_C DLL_EXPORT bool SetFrames2(btTypedConstraint* constrain, 
			Vector3 frameA, Quaternion frameArot, Vector3 frameB, Quaternion frameBrot)
{
	bool ret = false;
	switch (constrain->getConstraintType())
	{
	case D6_CONSTRAINT_TYPE:
	{
		btGeneric6DofConstraint* cc = (btGeneric6DofConstraint*)constrain;
		btTransform transA;
		transA.setIdentity();
		transA.setOrigin(frameA.GetBtVector3());
		transA.setRotation(frameArot.GetBtQuaternion());
		btTransform transB;
		transB.setIdentity();
		transB.setOrigin(frameB.GetBtVector3());
		transB.setRotation(frameBrot.GetBtQuaternion());
		cc->setFrames(transA, transB);
		ret = true;
		break;
	}
	default:
		break;
	}
	return ret;
}

EXTERN_C DLL_EXPORT void SetConstraintEnable2(btTypedConstraint* constrain, float trueFalse)
{
	constrain->setEnabled(trueFalse == ParamTrue ? true : false);
	return;
}

EXTERN_C DLL_EXPORT void SetConstraintNumSolverIterations2(btTypedConstraint* constrain, float iterations)
{
	constrain->setOverrideNumSolverIterations((int)iterations);
	return;
}

EXTERN_C DLL_EXPORT bool SetLinearLimits2(btTypedConstraint* constrain, Vector3 low, Vector3 high)
{
	// BSLog("SetLinearLimits2: loc=%x, low=<%f,%f,%f>, high=<%f,%f,%f>", constrain,
	// 							low.X, low.Y, low.Z, high.X, high.Y, high.Z );
	bool ret = false;
	switch (constrain->getConstraintType())
	{
	case D6_CONSTRAINT_TYPE:
	{
		btGeneric6DofConstraint* cc = (btGeneric6DofConstraint*)constrain;
		cc->setLinearLowerLimit(low.GetBtVector3());
		cc->setLinearUpperLimit(high.GetBtVector3());
		ret = true;
		break;
	}
	default:
		break;
	}
	return ret;
}

EXTERN_C DLL_EXPORT bool SetAngularLimits2(btTypedConstraint* constrain, Vector3 low, Vector3 high)
{
	// BSLog("SetAngularLimits2: loc=%x, low=<%f,%f,%f>, high=<%f,%f,%f>", constrain,
	// 							low.X, low.Y, low.Z, high.X, high.Y, high.Z );
	bool ret = false;
	switch (constrain->getConstraintType())
	{
	case D6_CONSTRAINT_TYPE:
	{
		btGeneric6DofConstraint* cc = (btGeneric6DofConstraint*)constrain;
		cc->setAngularLowerLimit(low.GetBtVector3());
		cc->setAngularUpperLimit(high.GetBtVector3());
		ret = true;
		break;
	}
	default:
		break;
	}
	return ret;
}

EXTERN_C DLL_EXPORT bool UseFrameOffset2(btTypedConstraint* constrain, float enable)
{
	// BSLog("UseFrameOffset2: loc=%x, enable=%f", constrain, enable);
	bool ret = false;
	bool onOff = (enable == ParamTrue);
	switch (constrain->getConstraintType())
	{
	case HINGE_CONSTRAINT_TYPE:
	{
		btHingeConstraint* hc = (btHingeConstraint*)constrain;
		hc->setUseFrameOffset(onOff);
		ret = true;
		break;
	}
	case D6_CONSTRAINT_TYPE:
	{
		btGeneric6DofConstraint* cc = (btGeneric6DofConstraint*)constrain;
		cc->setUseFrameOffset(onOff);
		ret = true;
		break;
	}
	default:
		break;
	}
	return ret;
}

EXTERN_C DLL_EXPORT bool TranslationalLimitMotor2(btTypedConstraint* constrain, 
				float enable, float targetVelocity, float maxMotorForce)
{
	// BSLog("TranslationalLimitMotor2: loc=%x, enable=%f, targetVel=%f, maxMotorForce=%f", constrain, enable, targetVelocity, maxMotorForce);
	bool ret = false;
	bool onOff = (enable == ParamTrue);
	switch (constrain->getConstraintType())
	{
	case D6_CONSTRAINT_TYPE:
	{
		btGeneric6DofConstraint* cc = (btGeneric6DofConstraint*)constrain;
		cc->getTranslationalLimitMotor()->m_enableMotor[0] = onOff;
		cc->getTranslationalLimitMotor()->m_targetVelocity[0] = targetVelocity;
		cc->getTranslationalLimitMotor()->m_maxMotorForce[0] = maxMotorForce;
		ret = true;
		break;
	}
	default:
		break;
	}

	return ret;
}

EXTERN_C DLL_EXPORT bool SetBreakingImpulseThreshold2(btTypedConstraint* constrain, float thresh)
{
	// BSLog("SetBreakingImpulseThreshold: loc=%x, threshold=%f", constrain, thresh);
	bool ret = false;
	switch (constrain->getConstraintType())
	{
	case D6_CONSTRAINT_TYPE:
	{
		btGeneric6DofConstraint* cc = (btGeneric6DofConstraint*)constrain;
		cc->setBreakingImpulseThreshold(btScalar(thresh));
		ret = true;
		break;
	}
	default:
		break;
	}

	return ret;
}

EXTERN_C DLL_EXPORT bool CalculateTransforms2(btTypedConstraint* constrain)
{
	bool ret = false;
	switch (constrain->getConstraintType())
	{
	case D6_CONSTRAINT_TYPE:
	{
		btGeneric6DofConstraint* cc = (btGeneric6DofConstraint*)constrain;
		cc->calculateTransforms();
		ret = true;
		break;
	}
	default:
		break;
	}
	return ret;
}

EXTERN_C DLL_EXPORT bool SetConstraintParam2(btTypedConstraint* constrain, int paramIndex, float value, int axis)
{
	if (axis == COLLISION_AXIS_LINEAR_ALL || axis == COLLISION_AXIS_ALL)
	{
		constrain->setParam(paramIndex, btScalar(value), 0);
		constrain->setParam(paramIndex, btScalar(value), 1);
		constrain->setParam(paramIndex, btScalar(value), 2);
	}
	if (axis == COLLISION_AXIS_ANGULAR_ALL || axis == COLLISION_AXIS_ALL)
	{
		constrain->setParam(paramIndex, btScalar(value), 3);
		constrain->setParam(paramIndex, btScalar(value), 4);
		constrain->setParam(paramIndex, btScalar(value), 5);
	}
	if (axis < COLLISION_AXIS_LINEAR_ALL)
	{
		constrain->setParam(paramIndex, btScalar(value), axis);
	}
	return true;
}

EXTERN_C DLL_EXPORT bool DestroyConstraint2(BulletSim* sim, btTypedConstraint* constrain)
{
	// BSLog("DestroyConstraint2: loc=%x", constrain);
	sim->getDynamicsWorld()->removeConstraint(constrain);
	delete constrain;
	return true;
}

// =====================================================================
// Remember to restore any constraints
EXTERN_C DLL_EXPORT bool AddObjectToWorld2(BulletSim* sim, btCollisionObject* obj)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb == NULL)
		sim->getDynamicsWorld()->addCollisionObject(obj);
	else
		sim->getDynamicsWorld()->addRigidBody(rb);
	return true;
}

// Remember to remove any constraints
EXTERN_C DLL_EXPORT bool RemoveObjectFromWorld2(BulletSim* sim, btCollisionObject* obj)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb == NULL)
		sim->getWorldData()->dynamicsWorld->removeCollisionObject(obj);
	else
		sim->getDynamicsWorld()->removeRigidBody(rb);
	return true;
}

// =====================================================================
EXTERN_C DLL_EXPORT Vector3 GetPosition2(btCollisionObject* obj)
{
	btTransform xform = obj->getWorldTransform();
	btVector3 p = xform.getOrigin();
	return Vector3(p.getX(), p.getY(), p.getZ());
}

EXTERN_C DLL_EXPORT Quaternion GetOrientation2(btCollisionObject* obj)
{
	btTransform xform = obj->getWorldTransform();
	btQuaternion p = xform.getRotation();
	return Quaternion(p.getX(), p.getY(), p.getZ(), p.getW());
}

EXTERN_C DLL_EXPORT bool SetTranslation2(btCollisionObject* obj, Vector3 position, Quaternion rotation)
{
	btVector3 pos = position.GetBtVector3();
	btQuaternion rot = rotation.GetBtQuaternion();
	// Build a transform containing the new position and rotation
	btTransform transform;
	transform.setIdentity();
	transform.setOrigin(pos);
	transform.setRotation(rot);

	// Set the new transform for the rigid body and the motion state
	obj->setWorldTransform(transform);
	return true;
}

EXTERN_C DLL_EXPORT bool SetVelocity2(btCollisionObject* obj, Vector3 velocity)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb == NULL) return false;

	rb->setLinearVelocity(velocity.GetBtVector3());
	return true;
}

EXTERN_C DLL_EXPORT bool SetAngularVelocity2(btCollisionObject* obj, Vector3 angularVelocity)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb == NULL) return false;

	rb->setAngularVelocity(angularVelocity.GetBtVector3());
	return true;
}

EXTERN_C DLL_EXPORT bool SetObjectForce2(btCollisionObject* obj, Vector3 force)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb == NULL) return false;

	// Oddly, Bullet doesn't have a way to directly set the force so this
	//    subtracts the total force (making force zero) and then adds our new force.
	rb->applyCentralForce(force.GetBtVector3() - rb->getTotalForce());
	return true;
}

// Adding a force is different than adding an impulse
EXTERN_C DLL_EXPORT bool AddObjectForce2(btCollisionObject* obj, Vector3 force)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb == NULL) return false;

	rb->applyCentralForce(force.GetBtVector3());
	return true;
}

EXTERN_C DLL_EXPORT bool SetCcdMotionThreshold2(btCollisionObject* obj, float val)
{
	obj->setCcdMotionThreshold(btScalar(val));
	return true;
}

EXTERN_C DLL_EXPORT bool SetCcdSweepSphereRadius2(btCollisionObject* obj, float val)
{
	obj->setCcdSweptSphereRadius(btScalar(val));
	return true;
}

EXTERN_C DLL_EXPORT bool SetDamping2(btCollisionObject* obj, float lin_damping, float ang_damping)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb == NULL) return false;

	rb->setDamping(btScalar(lin_damping), btScalar(ang_damping));
	return true;

}

EXTERN_C DLL_EXPORT bool SetDeactivationTime2(btCollisionObject* obj, float val)
{
	obj->setDeactivationTime(btScalar(val));
	return true;
}

EXTERN_C DLL_EXPORT bool SetSleepingThresholds2(btCollisionObject* obj, float lin_threshold, float ang_threshold)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb == NULL) return false;

	rb->setSleepingThresholds(btScalar(lin_threshold), btScalar(ang_threshold));
	return true;
}

EXTERN_C DLL_EXPORT bool SetContactProcessingThreshold2(btCollisionObject* obj, float val)
{
	obj->setContactProcessingThreshold(btScalar(val));
	return true;
}

EXTERN_C DLL_EXPORT bool SetFriction2(btCollisionObject* obj, float val)
{
	obj->setFriction(btScalar(val));
	return true;
}

EXTERN_C DLL_EXPORT bool SetRestitution2(btCollisionObject* obj, float val)
{
	obj->setRestitution(btScalar(val));
	return true;
}

EXTERN_C DLL_EXPORT bool SetLinearVelocity2(btCollisionObject* obj, Vector3 val)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb == NULL) return false;

	rb->setLinearVelocity(val.GetBtVector3());
	return true;
}

// (sets both linear and angular interpolation velocity)
EXTERN_C DLL_EXPORT bool SetInterpolation2(btCollisionObject* obj, Vector3 lin, Vector3 ang)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb == NULL) return false;

	rb->setInterpolationLinearVelocity(lin.GetBtVector3());
	rb->setInterpolationAngularVelocity(ang.GetBtVector3());
	rb->setInterpolationWorldTransform(rb->getWorldTransform());
	return true;
}

EXTERN_C DLL_EXPORT int GetCollisionFlags2(btCollisionObject* obj)
{
	return obj->getCollisionFlags();
}

EXTERN_C DLL_EXPORT bool SetCollisionFlags2(btCollisionObject* obj, uint32_t flags)
{
	obj->setCollisionFlags(flags);
	return true;
}

EXTERN_C DLL_EXPORT bool AddToCollisionFlags2(btCollisionObject* obj, uint32_t flags)
{
	obj->setCollisionFlags(obj->getCollisionFlags() | flags);
	return true;
}

EXTERN_C DLL_EXPORT bool RemoveFromCollisionFlags2(btCollisionObject* obj, uint32_t flags)
{
	obj->setCollisionFlags(obj->getCollisionFlags() & ~flags);
	return true;
}

EXTERN_C DLL_EXPORT bool SetMassProps2(btCollisionObject* obj, float mass, Vector3 inertia)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb == NULL) return false;

	rb->setMassProps(btScalar(mass), inertia.GetBtVector3());
	return true;
}

EXTERN_C DLL_EXPORT bool UpdateInertiaTensor2(btCollisionObject* obj)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb == NULL) return false;

	rb->updateInertiaTensor();
	return true;
}

EXTERN_C DLL_EXPORT bool SetGravity2(btCollisionObject* obj, Vector3 val)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb == NULL) return false;

	rb->setGravity(val.GetBtVector3());
	return true;
}

EXTERN_C DLL_EXPORT bool ClearForces2(btCollisionObject* obj)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb == NULL) return false;

	rb->clearForces();
	return true;
}

// Zero out all forces and bring the object to a dead stop
EXTERN_C DLL_EXPORT bool ClearAllForces2(btCollisionObject* obj)
{
	Vector3 zeroVector = Vector3();

	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb == NULL) return false;

	SetLinearVelocity2(rb, zeroVector);
	SetAngularVelocity2(rb, zeroVector);
	SetObjectForce2(rb, zeroVector);
	SetInterpolation2(rb, zeroVector, zeroVector);
	rb->clearForces();
	return true;
}

EXTERN_C DLL_EXPORT bool SetMargin2(btCollisionObject* obj, float val)
{
	obj->getCollisionShape()->setMargin(btScalar(val));
	return true;
}

EXTERN_C DLL_EXPORT bool UpdateSingleAabb2(BulletSim* world, btCollisionObject* obj)
{
	world->getDynamicsWorld()->updateSingleAabb(obj);
	return true;
}

/**
 * Stop simulation for a character or rigid body and free all memory allocated by it.
 * @param worldID ID of the world to modify.
 * @param id Object ID.
 * @return True on success, false if the object was not found.
 */
EXTERN_C DLL_EXPORT bool DestroyObject2(BulletSim* world, unsigned int id)
{
	return world->DestroyObject(id);
}

// =====================================================================
/**
 * Perform a sweep test by moving a convex shape through space and testing for collisions. 
 * Starting and ending rotations are not currently supported since this was designed for
 * character sweep tests, which use capsules.
 * @param worldID ID of the world to access.
 * @param id Object ID of a convex object.
 * @param from Starting position of the sweep.
 * @param to Destination position of the sweep.
 * @param extraMargin Extra collision margin to add to the convex shape during the sweep.
 * @return Sweep results. If there were no collisions, SweepHit.ID will be ID_INVALID_HIT (0xFFFFFFFF)
 */
EXTERN_C DLL_EXPORT SweepHit ConvexSweepTest2(BulletSim* world, unsigned int id, Vector3 from, Vector3 to, float extraMargin)
{
	btVector3 f = from.GetBtVector3();
	btVector3 t = to.GetBtVector3();
	return world->ConvexSweepTest(id, f, t, extraMargin);
}

/**
 * Perform a raycast test by drawing a line from a and testing for collisions.
 * @param worldID ID of the world to access.
 * @param id Object ID to ignore during the raycast.
 * @param from Start of the ray.
 * @param to End of the ray.
 * @return Raycast results. If there were no collisions, RaycastHit.ID will be ID_INVALID_HIT (0xFFFFFFFF)
 */
EXTERN_C DLL_EXPORT RaycastHit RayTest2(BulletSim* world, unsigned int id, Vector3 from, Vector3 to)
{
	btVector3 f = from.GetBtVector3();
	btVector3 t = to.GetBtVector3();
	return world->RayTest(id, f, t);
}

/**
 * Returns the position offset required to bring a character out of a penetrating collision.
 * @param worldID ID of the world to access.
 * @param id Character ID.
 * @return A position offset to apply to the character to resolve a penetration.
 */
EXTERN_C DLL_EXPORT Vector3 RecoverFromPenetration2(BulletSim* world, unsigned int id)
{
	btVector3 v = world->RecoverFromPenetration(id);
	return Vector3(v.getX(), v.getY(), v.getZ());
}

// =====================================================================
/**
 * Causes the detailed physics performance statistics to be logged.
 */
EXTERN_C DLL_EXPORT void DumpPhysicsStatistics2(BulletSim* world)
{
	if (debugLogCallback == NULL) return;
	world->DumpPhysicsStats();
	return;
}

/*	RESTORE WHEN THE OLD API.CPP IS REMOVED
DebugLogCallback* debugLogCallback;
EXTERN_C DLL_EXPORT void SetDebugLogCallback(DebugLogCallback* dlc) {
	debugLogCallback = dlc;
}
// Call back into the managed world to output a log message with formatting
void BSLog(const char* msg, ...) {
	char buff[2048];
	if (debugLogCallback != NULL) {
		va_list args;
		va_start(args, msg);
		vsprintf(buff, msg, args);
		va_end(args);
		(*debugLogCallback)(buff);
	}
}
*/
