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
struct HeightMapInfo {
	int sizeX;
	int sizeY;
	btScalar minHeight;
	btScalar maxHeight;
	btVector3 minCoords;
	btVector3 maxCoords;
	float collisionMargin;
	float* heightMap;
	IDTYPE id;
};
// The minimum thickness for terrain. If less than this, we correct
#define TERRAIN_MIN_THICKNESS (0.2)

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

// =====================================================================
// Mesh, hull, shape and body creation helper routines
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

EXTERN_C DLL_EXPORT btCollisionShape* BuildHullShapeFromMesh2(BulletSim* sim, btCollisionShape* mesh) {
	return sim->BuildHullShapeFromMesh2(mesh);
}

EXTERN_C DLL_EXPORT btCollisionShape* CreateCompoundShape2(BulletSim* sim)
{
	btCompoundShape* cShape = new btCompoundShape();
	return cShape;
}

EXTERN_C DLL_EXPORT void AddChildShapeToCompoundShape2(btCompoundShape* cShape, 
				btCollisionShape* addShape, Vector3 relativePosition, Quaternion relativeRotation)
{
	btTransform relativeTransform;
	relativeTransform.setIdentity();
	relativeTransform.setOrigin(relativePosition.GetBtVector3());
	relativeTransform.setRotation(relativeRotation.GetBtQuaternion());

	cShape->addChildShape(relativeTransform, addShape);
}

EXTERN_C DLL_EXPORT void RemoveChildShapeFromCompoundShape2(btCompoundShape* cShape, btCollisionShape* removeShape)
{
	cShape->removeChildShape(removeShape);
}

EXTERN_C DLL_EXPORT void RecalculatecompoundShapeLocalAabb2(btCompoundShape* cShape)
{
	cShape->recalculateLocalAabb();
}

EXTERN_C DLL_EXPORT btCollisionShape* BuildNativeShape2(BulletSim* sim, ShapeData* shapeData)
{
	btCollisionShape* shape = NULL;
	switch ((int)shapeData->Type)
	{
		case ShapeData::SHAPE_BOX:
			// btBoxShape subtracts the collision margin from the half extents, so no 
			// fiddling with scale necessary
			// boxes are defined by their half extents
			shape = new btBoxShape(btVector3(0.5, 0.5, 0.5));	// this is really a unit box
			break;
		case ShapeData::SHAPE_CONE:	// TODO:
			shape = new btConeShapeZ(0.5, 1.0);
			break;
		case ShapeData::SHAPE_CYLINDER:	// TODO:
			shape = new btCylinderShapeZ(btVector3(0.5f, 0.5f, 0.5f));
			break;
		case ShapeData::SHAPE_SPHERE:
			shape = new btSphereShape(0.5);		// this is really a unit sphere
			break;
	}
	if (shape != NULL)
	{
		shape->setMargin(btScalar(sim->getWorldData()->params->collisionMargin));
		shape->setLocalScaling(shapeData->Scale.GetBtVector3());
		shape->setUserPointer(PACKLOCALID(shapeData->ID));
	}

	return shape;
}

// Return 'true' if this shape is a Bullet implemented native shape
EXTERN_C DLL_EXPORT bool IsNativeShape2(btCollisionShape* shape)
{
	bool ret = false;
	switch (shape->getShapeType())
	{
	case ShapeData::SHAPE_BOX:
		ret = true;
		break;
	case ShapeData::SHAPE_CONE:
		ret = true;
		break;
	case ShapeData::SHAPE_CYLINDER:
		ret = true;
		break;
	case ShapeData::SHAPE_SPHERE:
		ret = true;
		break;
	}
	return ret;
}

// Note: this does not do a deep deletion.
EXTERN_C DLL_EXPORT bool DeleteCollisionShape2(BulletSim* sim, btCollisionShape* shape)
{
	delete shape;
	return true;
}

EXTERN_C DLL_EXPORT btCollisionShape* DuplicateCollisionShape2(BulletSim* sim, btCollisionShape* src, unsigned int id)
{
	btCollisionShape* newShape = NULL;

	int shapeType = src->getShapeType();
	switch (shapeType)
	{
		case BroadphaseNativeTypes::TRIANGLE_MESH_SHAPE_PROXYTYPE:
		{
			btBvhTriangleMeshShape* srcTriShape = (btBvhTriangleMeshShape*)src;
			newShape = new btBvhTriangleMeshShape(srcTriShape->getMeshInterface(), true, true);
			break;
		}
		/*
		case BroadphaseNativeTypes::SCALED_TRIANGLE_MESH_SHAPE_PROXYTYPE:
		{
			btScaledBvhTriangleMeshShape* srcTriShape = (btScaledBvhTriangleMeshShape*)src;
			newShape = new btScaledBvhTriangleMeshShape(srcTriShape, src->getLocalScaling());
			break;
		}
		*/
		case BroadphaseNativeTypes::COMPOUND_SHAPE_PROXYTYPE:
		{
			btCompoundShape* srcCompShape = (btCompoundShape*)src;

			btCompoundShape* newCompoundShape = new btCompoundShape(false);
			int childCount = srcCompShape->getNumChildShapes();
			btCompoundShapeChild* children = srcCompShape->getChildList();

			for (int i = 0; i < childCount; i++)
			{
				btCollisionShape* childShape = children[i].m_childShape;
				btTransform childTransform = children[i].m_transform;

				newCompoundShape->addChildShape(childTransform, childShape);
			}
			newShape = newCompoundShape;
			break;
		}
		default:
			break;
	}
	if (newShape != NULL)
	{
		newShape->setUserPointer(PACKLOCALID(id));
	}
	return newShape;
}

// Returns a btCollisionObject::CollisionObjectTypes
EXTERN_C DLL_EXPORT int GetBodyType2(btCollisionObject* obj)
{
	return obj->getInternalType();
}

// Create aa btRigidBody with our MotionState structure so we can track updates to this body.
EXTERN_C DLL_EXPORT btCollisionObject* CreateBodyFromShape2(BulletSim* sim, btCollisionShape* shape, 
						IDTYPE id, Vector3 pos, Quaternion rot)
{
	btTransform bodyTransform;
	bodyTransform.setIdentity();
	bodyTransform.setOrigin(pos.GetBtVector3());
	bodyTransform.setRotation(rot.GetBtQuaternion());

	// Use the BulletSim motion state so motion updates will be sent up
	SimMotionState* motionState = new SimMotionState(id, bodyTransform, &(sim->getWorldData()->updatesThisFrame));
	btRigidBody::btRigidBodyConstructionInfo cInfo(0.0, motionState, shape);
	btRigidBody* body = new btRigidBody(cInfo);
	motionState->RigidBody = body;

	body->setUserPointer(PACKLOCALID(id));

	return body;
}

// Create a RigidBody from passed shape and construction info.
// NOTE: it is presumed that a previous RigidBody was saved into the construction info
//     and that, in particular, the motionState is a SimMotionState from the saved RigidBody.
//     This WILL NOT WORK for terrain bodies.
// This does not restore collisionFlags.
EXTERN_C DLL_EXPORT btCollisionObject* CreateBodyFromShapeAndInfo2(BulletSim* sim, btCollisionShape* shape, 
						IDTYPE id, btRigidBody::btRigidBodyConstructionInfo* consInfo)
{
	consInfo->m_collisionShape = shape;
	btRigidBody* body = new btRigidBody(*consInfo);

	// The saved motion state was the SimMotionState saved from before.
	((SimMotionState*)consInfo->m_motionState)->RigidBody = body;
	body->setUserPointer(PACKLOCALID(id));

	return body;
}

// Create a btRigidBody with the default MotionState. We will not get any movement updates from this body.
EXTERN_C DLL_EXPORT btCollisionObject* CreateBodyWithDefaultMotionState2(btCollisionShape* shape, 
						IDTYPE id, Vector3 pos, Quaternion rot)
{
	btTransform heightfieldTr;
	heightfieldTr.setIdentity();
	heightfieldTr.setOrigin(pos.GetBtVector3());
	heightfieldTr.setRotation(rot.GetBtQuaternion());

	// Use the default motion state since we are not interested in these
	//   objects reporting collisions. Other objects will report their
	//   collisions with the terrain.
	btDefaultMotionState* motionState = new btDefaultMotionState(heightfieldTr);
	btRigidBody::btRigidBodyConstructionInfo cInfo(0.0, motionState, shape);
	btRigidBody* body = new btRigidBody(cInfo);

	body->setUserPointer(PACKLOCALID(id));

	return body;
}

// Create a btGhostObject with the passed shape
EXTERN_C DLL_EXPORT btCollisionObject* CreateGhostFromShape2(BulletSim* sim, btCollisionShape* shape, 
						IDTYPE id, Vector3 pos, Quaternion rot)
{
	btTransform bodyTransform;
	bodyTransform.setIdentity();
	bodyTransform.setOrigin(pos.GetBtVector3());
	bodyTransform.setRotation(rot.GetBtQuaternion());

	btGhostObject* gObj = new btPairCachingGhostObject();
	gObj->setWorldTransform(bodyTransform);
	gObj->setCollisionShape(shape);

	gObj->setUserPointer(PACKLOCALID(id));

	// place the ghost object in the list to be scanned for colliions at step time
	sim->getWorldData()->specialCollisionObjects[id] = gObj;
	
	return gObj;
}

// Build a RigidBody construction info from an existing RigidBody.
// Can be used later to recreate the rigid body.
EXTERN_C DLL_EXPORT btRigidBody::btRigidBodyConstructionInfo* AllocateBodyInfo2(btCollisionObject* obj)
{

	btRigidBody::btRigidBodyConstructionInfo* consInfo = NULL;

	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb)
	{
		consInfo = new btRigidBody::btRigidBodyConstructionInfo(
			1.0 / rb->getInvMass(), rb->getMotionState(), rb->getCollisionShape() );
		consInfo->m_localInertia = btVector3(0.0, 0.0, 0.0);
		consInfo->m_linearDamping = rb->getLinearDamping();
		consInfo->m_angularDamping = rb->getAngularDamping();
		consInfo->m_friction = rb->getFriction();
		consInfo->m_restitution = rb->getRestitution();
		consInfo->m_linearSleepingThreshold = rb->getLinearSleepingThreshold();
		consInfo->m_angularSleepingThreshold = rb->getAngularSleepingThreshold();
		// The following are unaccessable but they are usually the default anyway.
		// If these are used, they must be later set on the restored RigidBody.
		// consInfo->m_additionalDamping = rb->m_additionalDamping;
		// consInfo->m_additionalDampingFactor = rb->m_additionalDampingFactor;
		// consInfo->m_additionalLinearDampingThresholdSqr = rb->m_additionalLinearDampingThresholdSqr;
		// consInfo->m_additionalAngularDampingThresholdSqr = rb->m_additionalAngularDampingThresholdSqr;
		// consInfo->m_additionalAngularDampingFactor = rb->m_additionalAngularDampingFactor;
	}
	return consInfo;
}

// Release the btRigidBodyConstructionInfo created in the above routine.
EXTERN_C DLL_EXPORT void ReleaseBodyInfo2(btRigidBody::btRigidBodyConstructionInfo* consInfo)
{
	delete consInfo;
	return;
}

/**
 * Free all memory allocated to an object. The caller must have previously removed
 * the object from the dynamicsWorld.
 * @param worldID ID of the world to modify.
 * @param id Object ID.
 * @return True on success, false if the object was not found.
 */
EXTERN_C DLL_EXPORT void DestroyObject2(BulletSim* sim, btCollisionObject* obj)
{

	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb)
	{
		// If we added a motionState to the object, delete that
		btMotionState* motionState = rb->getMotionState();
		if (motionState)
			delete motionState;
	}
	
	// Delete the rest of the memory allocated to this object
	btCollisionShape* shape = obj->getCollisionShape();
	if (shape) 
		delete shape;

	// Remove from special collision objects. A NOOP if not in the list.
	IDTYPE id = CONVLOCALID(obj->getUserPointer());
	sim->getWorldData()->stepObjectCallbacks.erase(id);

	// finally make the object itself go away
	delete obj;
}

// =====================================================================
// Terrain creation and helper routines
EXTERN_C DLL_EXPORT void DumpMapInfo2(BulletSim* sim, HeightMapInfo* mapInfo)
{
	sim->getWorldData()->BSLog("HeightMapInfo: sizeX=%d, sizeY=%d, collMargin=%f, id=%u, minH=%f, maxH=%f", 
		mapInfo->sizeX, mapInfo->sizeY, mapInfo->collisionMargin, mapInfo->id, mapInfo->minHeight, mapInfo->maxHeight );
	sim->getWorldData()->BSLog("HeightMapInfo: minCoords=<%f,%f,%f> maxCoords=<%f,%f,%f>", 
		mapInfo->minCoords.getX(), mapInfo->minCoords.getY(), mapInfo->minCoords.getZ(),
		mapInfo->maxCoords.getX(), mapInfo->maxCoords.getY(), mapInfo->maxCoords.getZ() );
}

// Given a previously allocated HeightMapInfo, fill with the information about the heightmap
EXTERN_C DLL_EXPORT void FillHeightMapInfo2(BulletSim* sim, HeightMapInfo* mapInfo, IDTYPE id, 
				Vector3 minCoords, Vector3 maxCoords, float* heightMap, float collisionMargin)
{
	mapInfo->minCoords = minCoords.GetBtVector3();
	mapInfo->maxCoords = maxCoords.GetBtVector3();

	mapInfo->sizeX = (int)(maxCoords.X - minCoords.X);
	mapInfo->sizeY = (int)(maxCoords.Y - minCoords.Y);

	mapInfo->collisionMargin = collisionMargin;
	mapInfo->id = id;

	mapInfo->minHeight = btScalar(minCoords.Z);
	mapInfo->maxHeight = btScalar(maxCoords.Z);

	// Make sure top and bottom are different so a bounding box can be built.
	// This is not really necessary (it should be done in the calling code)
	//    but a sanity check doesn't hurt.
	if (mapInfo->minHeight == mapInfo->maxHeight)
		mapInfo->minHeight -= TERRAIN_MIN_THICKNESS;

	int numEntries = mapInfo->sizeX * mapInfo->sizeY;

	// Copy the heightmap local because the passed in array will be freed on return.
	float* localHeightMap = new float[numEntries];
	bsMemcpy(localHeightMap, heightMap, numEntries * sizeof(float));

	// Free any existing heightmap memory
	if (mapInfo->heightMap != NULL)
		delete mapInfo->heightMap;
	mapInfo->heightMap = localHeightMap;

	// DumpMapInfo2(sim, mapInfo);

	return;
}

// Bullet requires us to manage the heightmap array so these methods create
//    and release the memory for the heightmap.
EXTERN_C DLL_EXPORT HeightMapInfo* CreateHeightMapInfo2(BulletSim* sim, IDTYPE id,
				Vector3 minCoords, Vector3 maxCoords, float* heightMap, float collisionMargin)
{
	HeightMapInfo* mapInfo = new HeightMapInfo();
	mapInfo->heightMap = NULL;	// make sure there is no memory to deallocate
	FillHeightMapInfo2(sim, mapInfo, id, minCoords, maxCoords, heightMap, collisionMargin);
	return mapInfo;
}

EXTERN_C DLL_EXPORT bool ReleaseHeightMapInfo2(HeightMapInfo* mapInfo)
{
	if (mapInfo->heightMap)
		delete mapInfo->heightMap;
	delete mapInfo;
	return true;
}

// Build and return a btCollisionShape for the terrain
EXTERN_C DLL_EXPORT btCollisionShape* CreateTerrainShape2(HeightMapInfo* mapInfo)
{
	const int upAxis = 2;
	const btScalar scaleFactor = 1.0;
	btHeightfieldTerrainShape* terrainShape = new btHeightfieldTerrainShape(
			mapInfo->sizeX, mapInfo->sizeY, 
			mapInfo->heightMap, scaleFactor, 
			mapInfo->minHeight, mapInfo->maxHeight, upAxis, PHY_FLOAT, false);

	terrainShape->setMargin(btScalar(mapInfo->collisionMargin));
	terrainShape->setUseDiamondSubdivision(true);

	// Add the localID to the object so we know about collisions
	terrainShape->setUserPointer(PACKLOCALID(mapInfo->id));

	return terrainShape;
}

EXTERN_C DLL_EXPORT btCollisionShape* CreateGroundPlaneShape2(
	IDTYPE id,
	float height,	// usually 1
	float collisionMargin)
{
	// Initialize the ground plane
	btVector3 groundPlaneNormal = btVector3(0, 0, 1);	// Z up
	btStaticPlaneShape* m_planeShape = new btStaticPlaneShape(groundPlaneNormal, (int)height);
	m_planeShape->setMargin(collisionMargin);

	m_planeShape->setUserPointer(PACKLOCALID(id));

	return m_planeShape;
}

// =====================================================================
// Constraint creation and helper routines
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
 * @param 'true' if disable collisions between the constrained objects
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

	// sim->getWorldData()->BSLog("CreateConstraint2: loc=%x, body1=%u, body2=%u", constrain,
	// 					CONVLOCALID(obj1->getCollisionShape()->getUserPointer()),
	// 					CONVLOCALID(obj2->getCollisionShape()->getUserPointer()));
	// sim->getWorldData()->BSLog("          f1=<%f,%f,%f>, f1r=<%f,%f,%f,%f>, f2=<%f,%f,%f>, f2r=<%f,%f,%f,%f>",
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

		// sim->getWorldData()->BSLog("CreateConstraint2: loc=%x, body1=%u, body2=%u", constrain,
		// 					CONVLOCALID(obj1->getCollisionShape()->getUserPointer()),
		// 					CONVLOCALID(obj2->getCollisionShape()->getUserPointer()));
		// sim->getWorldData()->BSLog("          f1=<%f,%f,%f>, f1r=<%f,%f,%f,%f>, f2=<%f,%f,%f>, f2r=<%f,%f,%f,%f>",
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
}

EXTERN_C DLL_EXPORT void SetConstraintNumSolverIterations2(btTypedConstraint* constrain, float iterations)
{
	constrain->setOverrideNumSolverIterations((int)iterations);
}

EXTERN_C DLL_EXPORT bool SetLinearLimits2(btTypedConstraint* constrain, Vector3 low, Vector3 high)
{
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
	sim->getDynamicsWorld()->removeConstraint(constrain);
	delete constrain;
	return true;
}

// =====================================================================
// btCollisionWorld entries
EXTERN_C DLL_EXPORT void UpdateSingleAabb2(BulletSim* world, btCollisionObject* obj)
{
	world->getDynamicsWorld()->updateSingleAabb(obj);
}

EXTERN_C DLL_EXPORT void UpdateAabbs2(BulletSim* world)
{
	world->getDynamicsWorld()->updateAabbs();
}

EXTERN_C DLL_EXPORT bool GetForceUpdateAllAabbs2(BulletSim* world)
{
	return world->getDynamicsWorld()->getForceUpdateAllAabbs();
}

EXTERN_C DLL_EXPORT void SetForceUpdateAllAabbs2(BulletSim* world, bool forceUpdateAllAabbs)
{
	world->getDynamicsWorld()->setForceUpdateAllAabbs(forceUpdateAllAabbs);
}

// =====================================================================
// btDynamicWorld entries
// TODO: Remember to restore any constraints
EXTERN_C DLL_EXPORT bool AddObjectToWorld2(BulletSim* sim, btCollisionObject* obj)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb)
		sim->getDynamicsWorld()->addRigidBody(rb);
	else
		sim->getDynamicsWorld()->addCollisionObject(obj);
	return true;
}

// Remember to remove any constraints
EXTERN_C DLL_EXPORT bool RemoveObjectFromWorld2(BulletSim* sim, btCollisionObject* obj)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb)
		sim->getDynamicsWorld()->removeRigidBody(rb);
	else
		sim->getDynamicsWorld()->removeCollisionObject(obj);
	return true;
}

EXTERN_C DLL_EXPORT bool AddConstraintToWorld2(BulletSim* sim, btTypedConstraint* constrain, bool disableCollisionsBetweenLinkedBodies)
{
	sim->getDynamicsWorld()->addConstraint(constrain, disableCollisionsBetweenLinkedBodies);
	return true;
}

EXTERN_C DLL_EXPORT bool RemoveConstraintToWorld2(BulletSim* sim, btTypedConstraint* constrain)
{
	sim->getDynamicsWorld()->removeConstraint(constrain);
	return true;
}

// =====================================================================
// btCollisionObject entries and helpers
// These are in the order they are defined in btCollisionObject.h.
EXTERN_C DLL_EXPORT Vector3 GetAnisotropicFriction2(btCollisionObject* obj)
{
	btVector3 aFrict = obj->getAnisotropicFriction();
	return Vector3(aFrict.getX(), aFrict.getY(), aFrict.getZ());
}

EXTERN_C DLL_EXPORT void SetAnisotropicFriction2(btCollisionObject* obj, Vector3 aFrict)
{
	obj->setAnisotropicFriction(aFrict.GetBtVector3());
}

EXTERN_C DLL_EXPORT bool HasAnisotropicFriction2(btCollisionObject* obj)
{
	return obj->hasAnisotropicFriction();
}

EXTERN_C DLL_EXPORT void SetContactProcessingThreshold2(btCollisionObject* obj, float threshold)
{
	obj->setContactProcessingThreshold(btScalar(threshold));
}

EXTERN_C DLL_EXPORT float GetContactProcessingThreshold2(btCollisionObject* obj)
{
	return obj->getContactProcessingThreshold();
}

EXTERN_C DLL_EXPORT bool IsStaticObject2(btCollisionObject* obj)
{
	return obj->isStaticObject();
}

EXTERN_C DLL_EXPORT bool IsKinematicObject2(btCollisionObject* obj)
{
	return obj->isKinematicObject();
}

EXTERN_C DLL_EXPORT bool IsStaticOrKinematicObject2(btCollisionObject* obj)
{
	return obj->isStaticOrKinematicObject();
}

EXTERN_C DLL_EXPORT bool HasContactResponse2(btCollisionObject* obj)
{
	return obj->hasContactResponse();
}

// Given a previously allocated collision object and a new collision shape,
//    replace the shape on the collision object with the new shape.
EXTERN_C DLL_EXPORT void SetCollisionShape2(BulletSim* sim, btCollisionObject* obj, btCollisionShape* shape)
{
	obj->setCollisionShape(shape);

	// test
	// These are attempts to make Bullet accept the new shape that has been stuffed into the RigidBody.
	btOverlappingPairCache* opp = sim->getDynamicsWorld()->getBroadphase()->getOverlappingPairCache();
	opp->cleanProxyFromPairs(obj->getBroadphaseHandle(), sim->getDynamicsWorld()->getDispatcher());

	// Don't free the old shape here since it could be shared with other
	//    bodies. The managed code should be keeping track of the shapes
	//    and their use counts and creating and deleting them as needed.
}

EXTERN_C DLL_EXPORT btCollisionShape* GetCollisionShape2(btCollisionObject* obj)
{
	return obj->getCollisionShape();
}

EXTERN_C DLL_EXPORT int GetActivationState2(btCollisionObject* obj)
{
	return obj->getActivationState();
}

EXTERN_C DLL_EXPORT void SetActivationState2(btCollisionObject* obj, int state)
{
	obj->setActivationState(state);
}

EXTERN_C DLL_EXPORT void SetDeactivationTime2(btCollisionObject* obj, float dtime)
{
	obj->setDeactivationTime(btScalar(dtime));
}

EXTERN_C DLL_EXPORT float GetDeactivationTime2(btCollisionObject* obj)
{
	return obj->getDeactivationTime();
}

EXTERN_C DLL_EXPORT void ForceActivationState2(btCollisionObject* obj, int newState)
{
	obj->forceActivationState(newState);
}

EXTERN_C DLL_EXPORT void Activate2(btCollisionObject* obj, bool forceActivation)
{
	obj->activate(forceActivation);
}

EXTERN_C DLL_EXPORT bool IsActive2(btCollisionObject* obj)
{
	return obj->isActive();
}

EXTERN_C DLL_EXPORT void SetRestitution2(btCollisionObject* obj, float val)
{
	obj->setRestitution(btScalar(val));
}

EXTERN_C DLL_EXPORT float GetRestitution2(btCollisionObject* obj)
{
	return obj->getRestitution();
}

EXTERN_C DLL_EXPORT void SetFriction2(btCollisionObject* obj, float val)
{
	obj->setFriction(btScalar(val));
}

EXTERN_C DLL_EXPORT float GetFriction2(btCollisionObject* obj)
{
	return obj->getFriction();
}

EXTERN_C DLL_EXPORT void SetWorldTransform2(btCollisionObject* obj, Transform& trans)
{
	obj->setWorldTransform(trans.GetBtTransform());
}

EXTERN_C DLL_EXPORT Transform GetWorldTransform2(btCollisionObject* obj)
{
	btTransform xform;

	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb)
		xform = rb->getWorldTransform();
	else
		xform = obj->getWorldTransform();

	return xform;
}

// Helper function to get the position from the world transform.
EXTERN_C DLL_EXPORT Vector3 GetPosition2(btCollisionObject* obj)
{
	btTransform xform;

	// getWorldTransform() on a collisionObject adds interpolation to the position.
	// Getting the transform directly from the rigidBody gets the real value.
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb)
		xform = rb->getWorldTransform();
	else
		xform = obj->getWorldTransform();

	btVector3 p = xform.getOrigin();
	return Vector3(p.getX(), p.getY(), p.getZ());
}

// Helper function to get the rotation from the world transform.
EXTERN_C DLL_EXPORT Quaternion GetOrientation2(btCollisionObject* obj)
{
	Quaternion ret = Quaternion();

	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) 
		ret = rb->getOrientation();
	else
		ret = obj->getWorldTransform().getRotation();

	return ret;
}

// Helper routine that sets the world transform based on the passed position and rotation.
EXTERN_C DLL_EXPORT void SetTranslation2(btCollisionObject* obj, Vector3 position, Quaternion rotation)
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
	return;
}

EXTERN_C DLL_EXPORT btBroadphaseProxy* GetBroadphaseHandle2(btCollisionObject* obj)
{
	return obj->getBroadphaseHandle();
}

EXTERN_C DLL_EXPORT void SetBroadphaseHandle2(btCollisionObject* obj, btBroadphaseProxy* proxy)
{
	obj->setBroadphaseHandle(proxy);
}

EXTERN_C DLL_EXPORT Transform GetInterpolationWorldTransform2(btCollisionObject* obj)
{
	Transform ret = obj->getInterpolationWorldTransform();
	return ret;
}

EXTERN_C DLL_EXPORT void SetInterpolationWorldTransform2(btCollisionObject* obj, Transform& trans)
{
	obj->setInterpolationWorldTransform(trans.GetBtTransform());
}

EXTERN_C DLL_EXPORT void SetInterpolationLinearVelocity2(btCollisionObject* obj, Vector3& vel)
{
	obj->setInterpolationLinearVelocity(vel.GetBtVector3());
}

EXTERN_C DLL_EXPORT void SetInterpolationAngularVelocity2(btCollisionObject* obj, Vector3& vel)
{
	obj->setInterpolationLinearVelocity(vel.GetBtVector3());
}

// Helper function that sets both linear and angular interpolation velocity
EXTERN_C DLL_EXPORT void SetInterpolationVelocity2(btCollisionObject* obj, Vector3 lin, Vector3 ang)
{
	obj->setInterpolationLinearVelocity(lin.GetBtVector3());
	obj->setInterpolationAngularVelocity(ang.GetBtVector3());
}

EXTERN_C DLL_EXPORT Vector3 GetInterpolationLinearVelocity2(btCollisionObject* obj)
{
	return Vector3(obj->getInterpolationLinearVelocity());
}

EXTERN_C DLL_EXPORT Vector3 GetInterpolationAngularVelocity2(btCollisionObject* obj)
{
	return Vector3(obj->getInterpolationLinearVelocity());
}

EXTERN_C DLL_EXPORT float GetHitFraction2(btCollisionObject* obj)
{
	return obj->getHitFraction();
}

EXTERN_C DLL_EXPORT void SetHitFraction2(btCollisionObject* obj, float val)
{
	obj->setHitFraction(btScalar(val));
}

EXTERN_C DLL_EXPORT int GetCollisionFlags2(btCollisionObject* obj)
{
	return obj->getCollisionFlags();
}

EXTERN_C DLL_EXPORT uint32_t SetCollisionFlags2(btCollisionObject* obj, uint32_t flags)
{
	obj->setCollisionFlags(flags);
	return obj->getCollisionFlags();
}

EXTERN_C DLL_EXPORT uint32_t AddToCollisionFlags2(btCollisionObject* obj, uint32_t flags)
{
	obj->setCollisionFlags(obj->getCollisionFlags() | flags);
	return obj->getCollisionFlags();
}

EXTERN_C DLL_EXPORT uint32_t RemoveFromCollisionFlags2(btCollisionObject* obj, uint32_t flags)
{
	obj->setCollisionFlags(obj->getCollisionFlags() & ~flags);
	return obj->getCollisionFlags();
}

EXTERN_C DLL_EXPORT float GetCcdSweptSphereRadius2(btCollisionObject* obj)
{
	return obj->getCcdSweptSphereRadius();
}

EXTERN_C DLL_EXPORT void SetCcdsweptSphereRadius2(btCollisionObject* obj, float val)
{
	obj->setCcdSweptSphereRadius(btScalar(val));
}

EXTERN_C DLL_EXPORT float GetCcdMotionThreshold2(btCollisionObject* obj)
{
	return obj->getCcdMotionThreshold();
}

EXTERN_C DLL_EXPORT float GetSquareCcdMotionThreshold2(btCollisionObject* obj)
{
	return obj->getCcdSquareMotionThreshold();
}

EXTERN_C DLL_EXPORT void SetCcdMotionThreshold2(btCollisionObject* obj, float val)
{
	obj->setCcdMotionThreshold(btScalar(val));
}

EXTERN_C DLL_EXPORT void* GetUserPointer2(btCollisionObject* obj)
{
	return obj->getUserPointer();
}

EXTERN_C DLL_EXPORT void SetUserPointer2(btCollisionObject* obj, void* ptr)
{
	obj->setUserPointer(ptr);
}

// ==================================================================================
// btRigidBody methods
// These are in the order found in btRigidBody.h

EXTERN_C DLL_EXPORT void ApplyGravity2(btCollisionObject* obj)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->applyGravity();
}
EXTERN_C DLL_EXPORT void SetGravity2(btCollisionObject* obj, Vector3 grav)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->setGravity(grav.GetBtVector3());
}
EXTERN_C DLL_EXPORT Vector3 GetGravity2(btCollisionObject* obj)
{
	Vector3 ret = Vector3();
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->getGravity();
	return ret;
}

EXTERN_C DLL_EXPORT void SetDamping2(btCollisionObject* obj, float lin_damping, float ang_damping)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->setDamping(btScalar(lin_damping), btScalar(ang_damping));
}

EXTERN_C DLL_EXPORT float GetLinearDamping2(btCollisionObject* obj)
{
	float ret = 0.0;
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->getLinearDamping();
	return ret;
}

EXTERN_C DLL_EXPORT float GetAngularDamping2(btCollisionObject* obj)
{
	float ret = 0.0;
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->getAngularDamping();
	return ret;
}

EXTERN_C DLL_EXPORT float GetLinearSleepingThreshold2(btCollisionObject* obj)
{
	float ret = 0.0;
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->getLinearSleepingThreshold();
	return ret;
}

EXTERN_C DLL_EXPORT float GetAngularSleepingThreshold2(btCollisionObject* obj)
{
	float ret = 0.0;
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->getAngularSleepingThreshold();
	return ret;
}

EXTERN_C DLL_EXPORT void ApplyDamping2(btCollisionObject* obj, float timeStep)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->applyDamping(btScalar(timeStep));
}

EXTERN_C DLL_EXPORT void SetMassProps2(btCollisionObject* obj, float mass, Vector3 inertia)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->setMassProps(btScalar(mass), inertia.GetBtVector3());
}

EXTERN_C DLL_EXPORT Vector3 GetLinearFactor2(btCollisionObject* obj)
{
	Vector3 ret = Vector3();
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->getLinearFactor();
	return ret;
}

EXTERN_C DLL_EXPORT void SetLinearFactor2(btCollisionObject* obj, Vector3 fact)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->setLinearFactor(fact.GetBtVector3());
}

EXTERN_C DLL_EXPORT void SetCenterOfMassTransform2(btCollisionObject* obj, Transform trans)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->setCenterOfMassTransform(trans.GetBtTransform());
}

EXTERN_C DLL_EXPORT void SetCenterOfMassByPosRot2(btCollisionObject* obj, Vector3 pos, Quaternion rot)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb)
	{
		btTransform trans;
		trans.setIdentity();
		trans.setOrigin(pos.GetBtVector3());
		trans.setRotation(rot.GetBtQuaternion());

		rb->setCenterOfMassTransform(trans);
	}
}

EXTERN_C DLL_EXPORT void ApplyCentralForce2(btCollisionObject* obj, Vector3 force)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->applyCentralForce(force.GetBtVector3());
}

EXTERN_C DLL_EXPORT void SetObjectForce2(btCollisionObject* obj, Vector3 force)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	// Oddly, Bullet doesn't have a way to directly set the force so this
	//    subtracts the total force (making force zero) and then adds our new force.
	if (rb) rb->applyCentralForce(force.GetBtVector3() - rb->getTotalForce());
}

EXTERN_C DLL_EXPORT Vector3 GetTotalForce2(btCollisionObject* obj)
{
	Vector3 ret = Vector3();
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->getTotalForce();
	return ret;
}

EXTERN_C DLL_EXPORT Vector3 GetTotalTorque2(btCollisionObject* obj)
{
	Vector3 ret = Vector3();
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->getTotalTorque();
	return ret;
}

EXTERN_C DLL_EXPORT Vector3 GetInvInertiaDiagLocal2(btCollisionObject* obj)
{
	Vector3 ret = Vector3();
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->getInvInertiaDiagLocal();
	return ret;
}

EXTERN_C DLL_EXPORT void SetInvInertiaDiagLocal2(btCollisionObject* obj, Vector3 inert)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->setInvInertiaDiagLocal(inert.GetBtVector3());
}

EXTERN_C DLL_EXPORT void SetSleepingThresholds2(btCollisionObject* obj, float lin_threshold, float ang_threshold)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->setSleepingThresholds(btScalar(lin_threshold), btScalar(ang_threshold));
}

EXTERN_C DLL_EXPORT void ApplyTorque2(btCollisionObject* obj, Vector3 force)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->applyTorque(force.GetBtVector3());
}

EXTERN_C DLL_EXPORT void ApplyForce2(btCollisionObject* obj, Vector3 force, Vector3 pos)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->applyForce(force.GetBtVector3(), pos.GetBtVector3());
}

EXTERN_C DLL_EXPORT void ApplyCentralImpulse2(btCollisionObject* obj, Vector3 force)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->applyCentralImpulse(force.GetBtVector3());
}

EXTERN_C DLL_EXPORT void ApplyTorqueImpulse2(btCollisionObject* obj, Vector3 force)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->applyTorqueImpulse(force.GetBtVector3());
}

EXTERN_C DLL_EXPORT void ApplyImpulse2(btCollisionObject* obj, Vector3 force, Vector3 pos)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->applyImpulse(force.GetBtVector3(), pos.GetBtVector3());
}

EXTERN_C DLL_EXPORT void ClearForces2(btCollisionObject* obj)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->clearForces();
}

// Zero out all forces and bring the object to a dead stop
EXTERN_C DLL_EXPORT void ClearAllForces2(btCollisionObject* obj)
{
	btVector3 zeroVector = btVector3(0.0, 0.0, 0.0);

	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb)
	{
		rb->setLinearVelocity(zeroVector);
		rb->setAngularVelocity(zeroVector);
		rb->setInterpolationLinearVelocity(zeroVector);
		rb->setInterpolationAngularVelocity(zeroVector);
		rb->clearForces();
	}
}

EXTERN_C DLL_EXPORT void UpdateInertiaTensor2(btCollisionObject* obj)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->updateInertiaTensor();
}

EXTERN_C DLL_EXPORT Vector3 GetCenterOfMassPosition2(btCollisionObject* obj)
{
	Vector3 ret = Vector3();
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->getCenterOfMassPosition();
	return ret;
}

/* A helper function is above that works generally for btCollisionObject's
EXTERN_C DLL_EXPORT Quaternion GetOrientation2(btCollisionObject* obj)
{
	Quaternion ret = Quaternion();
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->getOrientation();
	return ret;
}
*/

EXTERN_C DLL_EXPORT Transform GetCenterOfMassTransform2(btCollisionObject* obj)
{
	Transform ret = Transform();
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->getCenterOfMassTransform();
	return ret;
}

EXTERN_C DLL_EXPORT Vector3 GetLinearVelocity2(btCollisionObject* obj)
{
	Vector3 ret = Vector3();
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->getLinearVelocity();
	return ret;
}

EXTERN_C DLL_EXPORT Vector3 GetAngularVelocity2(btCollisionObject* obj)
{
	Vector3 ret = Vector3();
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->getAngularVelocity();
	return ret;
}

EXTERN_C DLL_EXPORT void SetLinearVelocity2(btCollisionObject* obj, Vector3 velocity)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->setLinearVelocity(velocity.GetBtVector3());
}

EXTERN_C DLL_EXPORT void SetAngularVelocity2(btCollisionObject* obj, Vector3 angularVelocity)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->setAngularVelocity(angularVelocity.GetBtVector3());
}

EXTERN_C DLL_EXPORT Vector3 GetVelocityInLocalPoint2(btCollisionObject* obj, Vector3 pos)
{
	Vector3 ret = Vector3();
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->getVelocityInLocalPoint(pos.GetBtVector3());
	return ret;
}

EXTERN_C DLL_EXPORT void Translate2(btCollisionObject* obj, Vector3 trans)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->translate(trans.GetBtVector3());
}

EXTERN_C DLL_EXPORT void UpdateDeactivation2(btCollisionObject* obj, float timeStep)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->updateDeactivation(btScalar(timeStep));
}

EXTERN_C DLL_EXPORT bool WantsSleeping2(btCollisionObject* obj)
{
	bool ret = false;
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->wantsSleeping();
	return ret;
}

EXTERN_C DLL_EXPORT void SetAngularFactor2(btCollisionObject* obj, float fact)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->setAngularFactor(btScalar(fact));
}

EXTERN_C DLL_EXPORT void SetAngularFactorV2(btCollisionObject* obj, Vector3 fact)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->setAngularFactor(fact.GetBtVector3());
}

EXTERN_C DLL_EXPORT Vector3 GetAngularFactor2(btCollisionObject* obj)
{
	Vector3 ret = Vector3();
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->getAngularFactor();
	return ret;
}

EXTERN_C DLL_EXPORT bool IsInWorld2(btCollisionObject* obj)
{
	bool ret = false;
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->isInWorld();
	return ret;
}

EXTERN_C DLL_EXPORT void AddConstraintRef2(btCollisionObject* obj, btTypedConstraint* constrain)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->addConstraintRef(constrain);
}

EXTERN_C DLL_EXPORT void RemoveConstraintRef2(btCollisionObject* obj, btTypedConstraint* constrain)
{
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) rb->removeConstraintRef(constrain);
}

EXTERN_C DLL_EXPORT btTypedConstraint* GetConstraintRef2(btCollisionObject* obj, int index)
{
	btTypedConstraint* ret = 0;
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->getConstraintRef(index);
	return ret;
}

EXTERN_C DLL_EXPORT int GetNumConstraintRefs2(btCollisionObject* obj)
{
	int ret = 0;
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->getNumConstraintRefs();
	return ret;
}

EXTERN_C DLL_EXPORT Vector3 GetDeltaLinearVelocity2(btCollisionObject* obj)
{
	Vector3 ret = Vector3();
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->getDeltaLinearVelocity();
	return ret;
}

EXTERN_C DLL_EXPORT Vector3 GetDeltaAngularVelocity2(btCollisionObject* obj)
{
	Vector3 ret = Vector3();
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->getDeltaAngularVelocity();
	return ret;
}

EXTERN_C DLL_EXPORT Vector3 GetPushVelocity2(btCollisionObject* obj)
{
	Vector3 ret = Vector3();
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->getPushVelocity();
	return ret;
}

EXTERN_C DLL_EXPORT Vector3 GetTurnVelocity2(btCollisionObject* obj)
{
	Vector3 ret = Vector3();
	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb) ret = rb->getTurnVelocity();
	return ret;
}

// ========================================
// btCollisionShape methods and related

// Would be nice to have these functions but have not decided how to return the values
// EXTERN_C DLL_EXPORT void GetAabb2(btCollisionShape* shape)
// EXTERN_C DLL_EXPORT void GetBoundingSphere2(btCollisionShape* shape)
// EXTERN_C DLL_EXPORT void CalculateTemporalAabb2(btCollisionShape* shape)

EXTERN_C DLL_EXPORT float GetAngularMotionDisc2(btCollisionShape* shape)
{
	return shape->getAngularMotionDisc();
}

EXTERN_C DLL_EXPORT float GetContactBreakingThreshold2(btCollisionShape* shape, float defaultFactor)
{
	return shape->getContactBreakingThreshold(btScalar(defaultFactor));
}

EXTERN_C DLL_EXPORT bool IsPloyhedral2(btCollisionShape* shape)
{
	return shape->isPolyhedral();
}

EXTERN_C DLL_EXPORT bool IsConvex2d2(btCollisionShape* shape)
{
	return shape->isConvex2d();
}

EXTERN_C DLL_EXPORT bool IsConvex2(btCollisionShape* shape)
{
	return shape->isConvex();
}

EXTERN_C DLL_EXPORT bool IsNonMoving2(btCollisionShape* shape)
{
	return shape->isNonMoving();
}

EXTERN_C DLL_EXPORT bool IsConcave2(btCollisionShape* shape)
{
	return shape->isConcave();
}

EXTERN_C DLL_EXPORT bool IsCompound2(btCollisionShape* shape)
{
	return shape->isCompound();
}

EXTERN_C DLL_EXPORT bool IsSoftBody2(btCollisionShape* shape)
{
	return shape->isSoftBody();
}

EXTERN_C DLL_EXPORT bool IsInfinite2(btCollisionShape* shape)
{
	return shape->isInfinite();
}

EXTERN_C DLL_EXPORT void SetLocalScaling2(btCollisionShape* shape, Vector3 scale)
{
	shape->setLocalScaling(scale.GetBtVector3());
}

EXTERN_C DLL_EXPORT Vector3 GetLocalScaling2(btCollisionShape* shape)
{
	return shape->getLocalScaling();
}

EXTERN_C DLL_EXPORT Vector3 CalculateLocalInertia2(btCollisionShape* shape, float mass)
{
	btVector3 btInertia;
	shape->calculateLocalInertia(btScalar(mass), btInertia);
	return Vector3(btInertia);
}

EXTERN_C DLL_EXPORT int GetShapeType2(btCollisionShape* shape)
{
	return shape->getShapeType();
}

EXTERN_C DLL_EXPORT void SetMargin2(btCollisionShape* shape, float val)
{
	shape->setMargin(btScalar(val));
}

EXTERN_C DLL_EXPORT float GetMargin2(btCollisionShape* shape)
{
	return shape->getMargin();
}

EXTERN_C DLL_EXPORT void SetCollisionFilterMask2(btCollisionObject* obj, unsigned int filter, unsigned int mask)
{
	btBroadphaseProxy* proxy = obj->getBroadphaseHandle();
	// If the object is not in the world, there won't be a proxy.
	if (proxy)
	{
		proxy->m_collisionFilterGroup = (short)filter;
		proxy->m_collisionFilterMask = (short)mask;
	}
}

// =====================================================================


// =====================================================================
// =====================================================================
// =====================================================================
// Raycasting.
// NOTE DONE YET. NOT EVEN LOOKED AT.
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
// Debugging
// Dump a btCollisionObject and even more if it's a btRigidBody.
EXTERN_C DLL_EXPORT void DumpRigidBody2(BulletSim* sim, btCollisionObject* obj)
{
	sim->getWorldData()->BSLog("DumpRigidBody: id=%u, pos=<%f,%f,%f>, orient=<%f,%f,%f,%f>",
				CONVLOCALID(obj->getCollisionShape()->getUserPointer()),
				(float)obj->getWorldTransform().getOrigin().getX(),
				(float)obj->getWorldTransform().getOrigin().getY(),
				(float)obj->getWorldTransform().getOrigin().getZ(),
				(float)obj->getWorldTransform().getRotation().getX(),
				(float)obj->getWorldTransform().getRotation().getY(),
				(float)obj->getWorldTransform().getRotation().getZ(),
				(float)obj->getWorldTransform().getRotation().getW()
		);

	sim->getWorldData()->BSLog("DumpRigidBody: actState=%d, active=%s, static=%s, mergesIslnd=%s, contactResp=%s, cFlag=%d, deactTime=%f",
				obj->getActivationState(),
				obj->isActive() ? "true" : "false",
				obj->isStaticObject() ? "true" : "false",
				obj->mergesSimulationIslands() ? "true" : "false",
				obj->hasContactResponse() ? "true" : "false",
				obj->getCollisionFlags(),
				(float)obj->getDeactivationTime()
		);

	sim->getWorldData()->BSLog("DumpRigidBody: ccdTrsh=%f, ccdSweep=%f, contProc=%f, frict=%f, hitFrict=%f, restit=%f, internTyp=%f",
				(float)obj->getCcdMotionThreshold(),
				(float)obj->getCcdSweptSphereRadius(),
				(float)obj->getContactProcessingThreshold(),
				(float)obj->getFriction(),
				(float)obj->getHitFraction(),
				(float)obj->getRestitution(),
				(float)obj->getInternalType()
		);

	btBroadphaseProxy* proxy = obj->getBroadphaseHandle();
	if (proxy)
	{
		sim->getWorldData()->BSLog("DumpRigidBody: collisionFilterGroup=%X, mask=%X",
									proxy->m_collisionFilterGroup,
									proxy->m_collisionFilterMask);
	}

	btTransform interpTrans = obj->getInterpolationWorldTransform();
	btVector3 interpPos = interpTrans.getOrigin();
	btQuaternion interpRot = interpTrans.getRotation();
	sim->getWorldData()->BSLog("DumpRigidBody: interpPos=<%f,%f,%f>, interpRot=<%f,%f,%f,%f>, interpLVel=<%f,%f,%f>, interpAVel=<%f,%f,%f>",
				(float)interpPos.getX(),
				(float)interpPos.getY(),
				(float)interpPos.getZ(),
				(float)interpRot.getX(),
				(float)interpRot.getY(),
				(float)interpRot.getZ(),
				(float)interpRot.getW(),
				(float)obj->getInterpolationLinearVelocity().getX(),
				(float)obj->getInterpolationLinearVelocity().getY(),
				(float)obj->getInterpolationLinearVelocity().getZ(),
				(float)obj->getInterpolationAngularVelocity().getX(),
				(float)obj->getInterpolationAngularVelocity().getY(),
				(float)obj->getInterpolationAngularVelocity().getZ()
		);



	btRigidBody* rb = btRigidBody::upcast(obj);
	if (rb)
	{
		sim->getWorldData()->BSLog("DumpRigidBody: lVel=<%f,%f,%f>, deltaLVel=<%f,%f,%f>, pushVel=<%f,%f,%f> turnVel=<%f,%f,%f>",
					(float)rb->getLinearVelocity().getX(),
					(float)rb->getLinearVelocity().getY(),
					(float)rb->getLinearVelocity().getZ(),
					(float)rb->getDeltaLinearVelocity().getX(),
					(float)rb->getDeltaLinearVelocity().getY(),
					(float)rb->getDeltaLinearVelocity().getZ(),
					(float)rb->getPushVelocity().getX(),
					(float)rb->getPushVelocity().getY(),
					(float)rb->getPushVelocity().getZ(),
					(float)rb->getTurnVelocity().getX(),
					(float)rb->getTurnVelocity().getY(),
					(float)rb->getTurnVelocity().getZ()
			);

		sim->getWorldData()->BSLog("DumpRigidBody: aVel=<%f,%f,%f>, deltaAVel=<%f,%f,%f>, aFactor=<%f,%f,%f> sleepThresh=%f, aDamp=%f",
					(float)rb->getAngularVelocity().getX(),
					(float)rb->getAngularVelocity().getY(),
					(float)rb->getAngularVelocity().getZ(),
					(float)rb->getDeltaAngularVelocity().getX(),
					(float)rb->getDeltaAngularVelocity().getY(),
					(float)rb->getDeltaAngularVelocity().getZ(),
					(float)rb->getAngularFactor().getX(),
					(float)rb->getAngularFactor().getY(),
					(float)rb->getAngularFactor().getZ(),
					(float)rb->getAngularSleepingThreshold(),
					(float)rb->getAngularDamping()
			);

		sim->getWorldData()->BSLog("DumpRigidBody: totForce=<%f,%f,%f>, totTorque=<%f,%f,%f>",
					(float)rb->getTotalForce().getX(),
					(float)rb->getTotalForce().getY(),
					(float)rb->getTotalForce().getZ(),
					(float)rb->getTotalTorque().getX(),
					(float)rb->getTotalTorque().getY(),
					(float)rb->getTotalTorque().getZ()
			);

		sim->getWorldData()->BSLog("DumpRigidBody: grav=<%f,%f,%f>, COMPos=<%f,%f,%f>, invMass=%f",
					(float)rb->getGravity().getX(),
					(float)rb->getGravity().getY(),
					(float)rb->getGravity().getZ(),
					(float)rb->getCenterOfMassPosition().getX(),
					(float)rb->getCenterOfMassPosition().getY(),
					(float)rb->getCenterOfMassPosition().getZ(),
					(float)rb->getInvMass()
			);

		btScalar inertiaTensorYaw, inertiaTensorPitch, inertiaTensorRoll;
		rb->getInvInertiaTensorWorld().getEulerYPR(inertiaTensorYaw, inertiaTensorPitch, inertiaTensorRoll);
		sim->getWorldData()->BSLog("DumpRigidBody: invInertDiag=<%f,%f,%f>, invInertiaTensorW: yaw=%f, pitch=%f, roll=%f",
					(float)rb->getInvInertiaDiagLocal().getX(),
					(float)rb->getInvInertiaDiagLocal().getY(),
					(float)rb->getInvInertiaDiagLocal().getZ(),
					(float)inertiaTensorYaw,
					(float)inertiaTensorPitch,
					(float)inertiaTensorRoll
			);
	}
}

EXTERN_C DLL_EXPORT void DumpCollisionShape2(BulletSim* sim, btCollisionShape* shape)
{
		sim->getWorldData()->BSLog("DumpCollisionShape: type=%d, id=%u, margin=%f, isMoving=%s, isConvex=%s",
				shape->getShapeType(),
				CONVLOCALID(shape->getUserPointer()),
				(float)shape->getMargin(),
				shape->isNonMoving() ? "true" : "false",
				shape->isConvex() ? "true" : "false"
			);
		sim->getWorldData()->BSLog("DumpCollisionShape:   localScaling=<%f,%f,%f>",
				(float)shape->getLocalScaling().getX(),
				(float)shape->getLocalScaling().getY(),
				(float)shape->getLocalScaling().getZ()
			);
}

// Causes detailed physics performance statistics to be logged.
EXTERN_C DLL_EXPORT void DumpPhysicsStatistics2(BulletSim* world)
{
	if (world->getWorldData()->debugLogCallback)
	{
		world->DumpPhysicsStats();
	}
	return;
}