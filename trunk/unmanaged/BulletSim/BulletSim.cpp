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
#include "GroundPlaneObject.h"
#include "TerrainObject.h"
#include "Util.h"

#include "BulletCollision/CollisionDispatch/btSimulationIslandManager.h"

#include <set>

BulletSim::BulletSim(btScalar maxX, btScalar maxY, btScalar maxZ)
{
	// Make sure structures that will be created in initPhysics are marked as not created
	m_worldData.dynamicsWorld = NULL;
	m_worldData.objects = NULL;
	m_worldData.Terrain = NULL;

	m_worldData.sim = this;

	m_worldData.MinPosition = btVector3(0, 0, 0);
	m_worldData.MaxPosition = btVector3(maxX, maxY, maxZ);
}

void BulletSim::initPhysics(ParamBlock* parms, 
							int maxCollisions, CollisionDesc* collisionArray, 
							int maxUpdates, EntityProperties* updateArray)
{
	// Tell the world we're initializing and output size of types so we can
	//    debug mis-alignments when changing architecture.
	// m_worldData.BSLog("InitPhysics: sizeof(int)=%d, sizeof(long)=%d, sizeof(long long)=%d, sizeof(float)=%d",
	// 	sizeof(int), sizeof(long), sizeof(long long), sizeof(float));

	// remember the pointers to pinned memory for returning collisions and property updates
	m_maxCollisionsPerFrame = maxCollisions;
	m_collidersThisFrameArray = collisionArray;
	m_maxUpdatesPerFrame = maxUpdates;
	m_updatesThisFrameArray = updateArray;

	// Parameters are in a block of pinned memory
	m_worldData.params = parms;
	// the collection of all the objects that are passed to the physics engine
	m_worldData.objects = new ObjectCollection();

	// create the functional parts of the physics simulation
	btDefaultCollisionConstructionInfo cci;
	m_collisionConfiguration = new btDefaultCollisionConfiguration(cci);
	m_dispatcher = new btCollisionDispatcher(m_collisionConfiguration);

	// if you are setting a pool size, you should disable dynamic allocation
	if (m_worldData.params->maxPersistantManifoldPoolSize > 0)
		cci.m_defaultMaxPersistentManifoldPoolSize = (int)m_worldData.params->maxPersistantManifoldPoolSize;
	if (m_worldData.params->maxCollisionAlgorithmPoolSize > 0)
	if (m_worldData.params->shouldDisableContactPoolDynamicAllocation != ParamFalse)
		m_dispatcher->setDispatcherFlags(btCollisionDispatcher::CD_DISABLE_CONTACTPOOL_DYNAMIC_ALLOCATION);
	
	m_broadphase = new btDbvtBroadphase();

	// the following is needed to enable GhostObjects
	m_broadphase->getOverlappingPairCache()->setInternalGhostPairCallback(new btGhostPairCallback());
	
	m_solver = new btSequentialImpulseConstraintSolver();

	// Create the world
	btDiscreteDynamicsWorld* dynamicsWorld = new btDiscreteDynamicsWorld(m_dispatcher, m_broadphase, m_solver, m_collisionConfiguration);
	m_worldData.dynamicsWorld = dynamicsWorld;
	
	// disable or enable the continuious recalculation of the static AABBs
	// http://www.bulletphysics.org/Bullet/phpBB3/viewtopic.php?f=9&t=4991
	// Note that if disabled, movement or changes to a static object will not update the AABB. Must do it explicitly.
	dynamicsWorld->setForceUpdateAllAabbs(m_worldData.params->shouldForceUpdateAllAabbs != ParamFalse);
	
	// Randomizing the solver order makes object stacking more stable at a slight performance cost
	if (m_worldData.params->shouldRandomizeSolverOrder != ParamFalse)
		dynamicsWorld->getSolverInfo().m_solverMode |= SOLVER_RANDMIZE_ORDER;

	// setting to false means the islands are not reordered and split up for individual processing
	dynamicsWorld->getSimulationIslandManager()->setSplitIslands(m_worldData.params->shouldSplitSimulationIslands != ParamFalse);

	// Performance speedup: http://bulletphysics.org/Bullet/phpBB3/viewtopic.php?p=14367
	// Actually a NOOP unless Bullet is compiled with USE_SEPDISTANCE_UTIL2 set.
	dynamicsWorld->getDispatchInfo().m_useConvexConservativeDistanceUtil = true;
	dynamicsWorld->getDispatchInfo().m_convexConservativeDistanceThreshold = btScalar(0.01);

	// Performance speedup: from BenchmarkDemo.cpp, ln 381
	if (m_worldData.params->shouldEnableFrictionCaching != ParamFalse)
		m_worldData.dynamicsWorld->getSolverInfo().m_solverMode |= SOLVER_ENABLE_FRICTION_DIRECTION_CACHING; //don't recalculate friction values each frame
	if (m_worldData.params->numberOfSolverIterations > 0)
		m_worldData.dynamicsWorld->getSolverInfo().m_numIterations = (int)m_worldData.params->numberOfSolverIterations;

	// Earth-like gravity
	dynamicsWorld->setGravity(btVector3(0.f, 0.f, m_worldData.params->gravity));

	// Information on creating a custom collision computation routine and a pointer to the computation
	// of friction and restitution at:
	// http://bulletphysics.org/Bullet/phpBB3/viewtopic.php?f=9&t=7922

	// foreach body that you want the callback, enable it with:
	// body->setCollisionFlags(body->getCollisionFlags() | btCollisionObject::CF_CUSTOM_MATERIAL_CALLBACK);

}

void BulletSim::CreateInitialGroundPlaneAndTerrain()
{
	CreateGroundPlane();
	CreateTerrain();
}

void BulletSim::exitPhysics()
{
	if (m_worldData.dynamicsWorld == NULL)
		return;

	if (m_worldData.objects)
	{
		m_worldData.objects->Clear();	// remove and delete all objects
		delete m_worldData.objects;		// get rid of the object collection itself
		m_worldData.objects = NULL;
	}

	// The terrain and ground plane objects are deleted when the object list is cleared
	m_worldData.Terrain = NULL;
	m_worldData.GroundPlane = NULL;

	// Delete collision meshes
	for (WorldData::HullsMapType::const_iterator it = m_worldData.Hulls.begin(); it != m_worldData.Hulls.end(); ++it)
    {
		btCollisionShape* collisionShape = it->second;
		delete collisionShape;
	}
	m_worldData.Hulls.clear();

	// Delete collision meshes
	for (WorldData::MeshesMapType::const_iterator it = m_worldData.Meshes.begin(); it != m_worldData.Meshes.end(); ++it)
    {
		btCollisionShape* collisionShape = it->second;
		delete collisionShape;
	}
	m_worldData.Meshes.clear();

	// Must delete the dynamics world before deleting it's solver, broadphase, ...
	if (m_worldData.dynamicsWorld != NULL)
	{
		delete m_worldData.dynamicsWorld;
		m_worldData.dynamicsWorld = NULL;
	}

	// Delete solver
	if (m_solver != NULL)
	{
		delete m_solver;
		m_solver = NULL;
	}

	// Delete broadphase
	if (m_broadphase != NULL)
	{
		delete m_broadphase;
		m_broadphase = NULL;
	}

	// Delete dispatcher
	if (m_dispatcher != NULL)
	{
		delete m_dispatcher;
		m_dispatcher = NULL;
	}

	// Delete collision config
	if (m_collisionConfiguration != NULL)
	{
		delete m_collisionConfiguration;
		m_collisionConfiguration = NULL;
	}
}

// Step the simulation forward by one full step and potentially some number of substeps
int BulletSim::PhysicsStep(btScalar timeStep, int maxSubSteps, btScalar fixedTimeStep, 
						   int* updatedEntityCount, EntityProperties** updatedEntities, 
						   int* collidersCount, CollisionDesc** colliders)
{
	int numSimSteps = 0;

	if (m_worldData.dynamicsWorld)
	{
		// The simulation calls the SimMotionState to put object updates into updatesThisFrame.
		// m_worldData.BSLog("BulletSim::PhysicsStep: ts=%f, maxSteps=%d, fixedTS=%f", timeStep, maxSubSteps, fixedTimeStep);
		numSimSteps = m_worldData.dynamicsWorld->stepSimulation(timeStep, maxSubSteps, fixedTimeStep);

		/*
		// BEGIN constraint debug==============================================================
		// Log the state of all the constraints.
		int numConstraints = m_worldData.dynamicsWorld->getNumConstraints();
		for (int kk = 0; kk < numConstraints; kk++)
		{
			btTypedConstraint* constrain = m_worldData.dynamicsWorld->getConstraint(kk);
			if (constrain->getConstraintType() == D6_CONSTRAINT_TYPE)
			{
				btGeneric6DofConstraint* constrain6 = (btGeneric6DofConstraint*)constrain;
				m_worldData.BSLog("constrain[%d]: A=%u, B=%u, enable=%s, limited=%s%s%s%s%s%s, impulse=%f",
					kk,
					CONVLOCALID(constrain6->getRigidBodyA().getCollisionShape()->getUserPointer()),
					CONVLOCALID(constrain6->getRigidBodyB().getCollisionShape()->getUserPointer()),
					(constrain6->isEnabled()) ? "true" : "false",
					(constrain6->isLimited(0)) ? "t" : "f",
					(constrain6->isLimited(1)) ? "t" : "f",
					(constrain6->isLimited(2)) ? "t" : "f",
					(constrain6->isLimited(3)) ? "t" : "f",
					(constrain6->isLimited(4)) ? "t" : "f",
					(constrain6->isLimited(5)) ? "t" : "f",
					constrain6->getAppliedImpulse()
				);
			}
		}
		// END constraint debug================================================================
		*/

		// Objects can register to be called after each step.
		// This allows objects to do per-step modification of returned values.
		if (m_worldData.stepObjectCallbacks.size() > 0)
		{
			WorldData::StepObjectCallbacksMapType::const_iterator it;
			for (it = m_worldData.stepObjectCallbacks.begin(); it != m_worldData.stepObjectCallbacks.end(); ++it)
			{
				(it->second)->StepCallback(it->first, &m_worldData);
			}
		}

		// Put all of the updates this frame into m_updatesThisFrameArray
		int updates = 0;
		if (m_worldData.updatesThisFrame.size() > 0)
		{
			for (WorldData::UpdatesThisFrameMapType::const_iterator it = m_worldData.updatesThisFrame.begin(); 
										it != m_worldData.updatesThisFrame.end(); ++it)
			{
				m_updatesThisFrameArray[updates] = *(it->second);
				updates++;
				if (updates >= m_maxUpdatesPerFrame) 
					break;
			}
			m_worldData.updatesThisFrame.clear();
		}

		// Update the values passed by reference into this function
		*updatedEntityCount = updates;
		*updatedEntities = m_updatesThisFrameArray;

		// Put all of the colliders this frame into m_collidersThisFrameArray
		std::set<COLLIDERKEYTYPE> collidersThisFrame;
		int collisions = 0;
		int numManifolds = m_worldData.dynamicsWorld->getDispatcher()->getNumManifolds();
		for (int j = 0; j < numManifolds; j++)
		{
			btPersistentManifold* contactManifold = m_worldData.dynamicsWorld->getDispatcher()->getManifoldByIndexInternal(j);
			int numContacts = contactManifold->getNumContacts();
			if (numContacts == 0)
				continue;

			btCollisionObject* objA = static_cast<btCollisionObject*>(contactManifold->getBody0());
			btCollisionObject* objB = static_cast<btCollisionObject*>(contactManifold->getBody1());

			// DEBUG BEGIN
			// IDTYPE idAx = CONVLOCALID(objA->getCollisionShape()->getUserPointer());
			// IDTYPE idBx = CONVLOCALID(objB->getCollisionShape()->getUserPointer());
			// m_worldData.BSLog("Collision: A=%u/%x, B=%u/%x", idAx, objA->getCollisionFlags(), idBx, objB->getCollisionFlags());
			// DEBUG END

			// One of the objects has to want to hear about collisions
			if ((objA->getCollisionFlags() & BS_SUBSCRIBE_COLLISION_EVENTS) == 0
					&& (objB->getCollisionFlags() & BS_SUBSCRIBE_COLLISION_EVENTS) == 0)
				continue;

			// When two objects collide, we only report one contact point
			const btManifoldPoint& manifoldPoint = contactManifold->getContactPoint(0);
			const btVector3& contactPoint = manifoldPoint.getPositionWorldOnB();
			btVector3 contactNormal = -manifoldPoint.m_normalWorldOnB;	// make relative to A

			// Get the IDs of colliding objects (stored in the one user definable field)
			IDTYPE idA = CONVLOCALID(objA->getCollisionShape()->getUserPointer());
			IDTYPE idB = CONVLOCALID(objB->getCollisionShape()->getUserPointer());

			// Make sure idA is the lower ID so we don't record both 'A hit B' and 'B hit A'
			if (idA > idB)
			{
				IDTYPE temp = idA;
				idA = idB;
				idB = temp;
				contactNormal = -contactNormal;
			}

			// Create a unique ID for this collision from the two colliding object IDs
			// We check for duplicate collisions between the two objects because
			//    there may be multiple hulls involved and thus multiple collisions.
			// TODO: decide if this is really a problem -- can this checking be removed?
			COLLIDERKEYTYPE collisionID = ((COLLIDERKEYTYPE)idA << 32) | idB;

			// If this collision has not been seen yet, record it
			if (collidersThisFrame.find(collisionID) == collidersThisFrame.end())
			{
				collidersThisFrame.insert(collisionID);

				CollisionDesc cDesc;
				cDesc.aID = idA;
				cDesc.bID = idB;
				cDesc.point = contactPoint;
				cDesc.normal = contactNormal;
				m_collidersThisFrameArray[collisions] = cDesc;
				collisions++;
			}

			if (collisions >= m_maxCollisionsPerFrame) 
				break;
		}

		*collidersCount = collisions;
		*colliders = m_collidersThisFrameArray;
	}

	return numSimSteps;
}

// Register to be called just after the simulation step of Bullet.
bool BulletSim::RegisterStepCallback(IDTYPE id, IPhysObject* target)
{
	UnregisterStepCallback(id);
	m_worldData.stepObjectCallbacks[id] = target;
	return true;
}

// Remove a registration for step callback.
// Safe to call if not registered.
// Returns 'true' if something was actually unregistered.
bool BulletSim::UnregisterStepCallback(IDTYPE id)
{
	size_type cnt = m_worldData.stepObjectCallbacks.erase(id);
	return (cnt > 0);
}

// Copy the passed heightmap into the memory block used by Bullet
void BulletSim::SetHeightmap(float* heightmap)
{
	if (m_worldData.Terrain)
	{
		m_worldData.Terrain->UpdateHeightMap(heightmap);
	}
}

// Create a collision plane at height zero to stop things falling to oblivion
void BulletSim::CreateGroundPlane()
{
	m_worldData.objects->RemoveAndDestroyObject(ID_GROUND_PLANE);
	IPhysObject* groundPlane = new GroundPlaneObject(&m_worldData, ID_GROUND_PLANE);
	m_worldData.objects->AddObject(ID_GROUND_PLANE, groundPlane);
}

// Based on the heightmap, create a mesh for the terrain and put it in the world
void BulletSim::CreateTerrain()
{
	// get rid of any old terrains lying around
	m_worldData.objects->RemoveAndDestroyObject(ID_TERRAIN);
	m_worldData.Terrain = NULL;

	// Create the new terrain based on the heightmap in m_worldData
	m_worldData.Terrain = new TerrainObject(&m_worldData, ID_TERRAIN);
	m_worldData.objects->AddObject(ID_TERRAIN, m_worldData.Terrain);
}

// If using Bullet' convex hull code, refer to following link for parameter setting
// http://kmamou.blogspot.com/2011/11/hacd-parameters.html
// Another useful reference for ConvexDecomp
// http://www.bulletphysics.org/Bullet/phpBB3/viewtopic.php?t=7159

// Create a hull based on passed convex hull information
bool BulletSim::CreateHull(MESHKEYTYPE meshKey, int hullCount, float* hulls)
{
	// m_worldData.BSLog("CreateHull: key=%ld, hullCount=%d", meshKey, hullCount);
	bool ret = false;

	WorldData::HullsMapType::iterator it = m_worldData.Hulls.find(meshKey);
	if (it == m_worldData.Hulls.end())
	{
		// Create a compound shape that will wrap the set of convex hulls
		btCompoundShape* compoundShape = new btCompoundShape(false);

		btTransform childTrans;
		childTrans.setIdentity();
		compoundShape->setMargin(m_worldData.params->collisionMargin);
		
		// Loop through all of the convex hulls and add them to our compound shape
		int ii = 1;
		for (int i = 0; i < hullCount; i++)
		{
			int vertexCount = (int)hulls[ii];

			// Offset this child hull by its calculated centroid
			btVector3 centroid = btVector3((btScalar)hulls[ii+1], (btScalar)hulls[ii+2], (btScalar)hulls[ii+3]);
			childTrans.setOrigin(centroid);

			// Create the child hull and add it to our compound shape
			btScalar* hullVertices = (btScalar*)&hulls[ii+4];
			btConvexHullShape* convexShape = new btConvexHullShape(hullVertices, vertexCount, sizeof(Vector3));
			convexShape->setMargin(m_worldData.params->collisionMargin);
			compoundShape->addChildShape(childTrans, convexShape);

			ii += (vertexCount * 3 + 4);
		}

		// Track this mesh
		m_worldData.Hulls[meshKey] = compoundShape;
		ret = true;
	}
	return ret;
}

// Delete a hull
bool BulletSim::DestroyHull(MESHKEYTYPE meshKey)
{
	// m_worldData.BSLog("BulletSim::DestroyHull: key=%ld", meshKey);
	// m_worldData.BSLog("DeleteHull:");
	WorldData::HullsMapType::iterator it = m_worldData.Hulls.find(meshKey);
	if (it != m_worldData.Hulls.end())
	{
		btCompoundShape* compoundShape = m_worldData.Hulls[meshKey];
		delete compoundShape;
		m_worldData.Hulls.erase(it);
		return true;
	}
	return false;
}

// Quote from http://bulletphysics.org/Bullet/phpBB3/viewtopic.php?t=7997
//       Yes, btBvhTriangleMeshShape can be used for static/kinematic objects only. 
//       You can use btGImpactMeshShapes (or btCompoundShapes plus HACD) for concave 
//       dynamic rigidbodies, or you can just throw away the indices and create a 
//       btConvexHullShape with the vertices of your mesh, but this would result in your 
//       shape becoming convex.
//       Create a mesh structure to be used for static objects
bool BulletSim::CreateMesh(MESHKEYTYPE meshKey, int indicesCount, int* indices, int verticesCount, float* vertices)
{
	// m_worldData.BSLog("BulletSim::CreateMesh: key=$ld, nIndices=%d, nVertices=%d", meshKey, indicesCount, verticesCount);
	bool ret = false;

	WorldData::MeshesMapType::iterator it = m_worldData.Meshes.find(meshKey);
	if (it == m_worldData.Meshes.end())
	{
		// We must copy the indices and vertices since the passed memory is released when this call returns.
		btIndexedMesh indexedMesh;
		int* copiedIndices = new int[indicesCount];
		bsMemcpy(copiedIndices, indices, indicesCount * sizeof(int));
		int numVertices = verticesCount * 3;
		float* copiedVertices = new float[numVertices];
		bsMemcpy(copiedVertices, vertices, numVertices * sizeof(float));

		indexedMesh.m_indexType = PHY_INTEGER;
		indexedMesh.m_triangleIndexBase = (const unsigned char*)copiedIndices;
		indexedMesh.m_triangleIndexStride = sizeof(int) * 3;
		indexedMesh.m_numTriangles = indicesCount / 3;
		indexedMesh.m_vertexType = PHY_FLOAT;
		indexedMesh.m_numVertices = verticesCount;
		indexedMesh.m_vertexBase = (const unsigned char*)copiedVertices;
		indexedMesh.m_vertexStride = sizeof(float) * 3;

		btTriangleIndexVertexArray* vertexArray = new btTriangleIndexVertexArray();
		vertexArray->addIndexedMesh(indexedMesh, PHY_INTEGER);

		btBvhTriangleMeshShape* meshShape = new btBvhTriangleMeshShape(vertexArray, true, true);
		
		m_worldData.Meshes[meshKey] = meshShape;
		ret = true;
	}
	return ret;
}

// Delete a mesh
bool BulletSim::DestroyMesh(MESHKEYTYPE meshKey)
{
	// m_worldData.BSLog("BulletSim::DeleteMesh: key=%ld", meshKey);
	bool ret = false;
	WorldData::MeshesMapType::iterator it = m_worldData.Meshes.find(meshKey);
	if (it != m_worldData.Meshes.end())
	{
		btBvhTriangleMeshShape* tms = m_worldData.Meshes[meshKey];
		/* This causes memory corruption.
		 * TODO: figure out when to properly release the memory allocated in CreateMesh.
		btIndexedMesh* smi = (btIndexedMesh*)tms->getMeshInterface();
		delete smi->m_triangleIndexBase;
		delete smi->m_vertexBase;
		*/
		delete tms;
		m_worldData.Meshes.erase(it);
		ret = true;
	}
	return ret;
}

btCollisionShape* BulletSim::CreateMeshShape2(int indicesCount, int* indices, int verticesCount, float* vertices )
{
	// We must copy the indices and vertices since the passed memory is released when this call returns.
	btIndexedMesh indexedMesh;
	int* copiedIndices = new int[indicesCount];
	bsMemcpy(copiedIndices, indices, indicesCount * sizeof(int));
	int numVertices = verticesCount * 3;
	float* copiedVertices = new float[numVertices];
	bsMemcpy(copiedVertices, vertices, numVertices * sizeof(float));

	indexedMesh.m_indexType = PHY_INTEGER;
	indexedMesh.m_triangleIndexBase = (const unsigned char*)copiedIndices;
	indexedMesh.m_triangleIndexStride = sizeof(int) * 3;
	indexedMesh.m_numTriangles = indicesCount / 3;
	indexedMesh.m_vertexType = PHY_FLOAT;
	indexedMesh.m_numVertices = verticesCount;
	indexedMesh.m_vertexBase = (const unsigned char*)copiedVertices;
	indexedMesh.m_vertexStride = sizeof(float) * 3;

	btTriangleIndexVertexArray* vertexArray = new btTriangleIndexVertexArray();
	vertexArray->addIndexedMesh(indexedMesh, PHY_INTEGER);

	btBvhTriangleMeshShape* meshShape = new btBvhTriangleMeshShape(vertexArray, true, true);

	return meshShape;
}

btCollisionShape* BulletSim::CreateHullShape2(int hullCount, float* hulls )
{
	// Create a compound shape that will wrap the set of convex hulls
	btCompoundShape* compoundShape = new btCompoundShape(false);

	btTransform childTrans;
	childTrans.setIdentity();
	compoundShape->setMargin(m_worldData.params->collisionMargin);
	
	// Loop through all of the convex hulls and add them to our compound shape
	int ii = 1;
	for (int i = 0; i < hullCount; i++)
	{
		int vertexCount = (int)hulls[ii];

		// Offset this child hull by its calculated centroid
		btVector3 centroid = btVector3((btScalar)hulls[ii+1], (btScalar)hulls[ii+2], (btScalar)hulls[ii+3]);
		childTrans.setOrigin(centroid);

		// Create the child hull and add it to our compound shape
		btScalar* hullVertices = (btScalar*)&hulls[ii+4];
		btConvexHullShape* convexShape = new btConvexHullShape(hullVertices, vertexCount, sizeof(Vector3));
		convexShape->setMargin(m_worldData.params->collisionMargin);
		compoundShape->addChildShape(childTrans, convexShape);

		ii += (vertexCount * 3 + 4);
	}

	return compoundShape;
}

// From a previously created mesh shape, create a convex hull using the Bullet
//   HACD hull creation code. The created hull will go into the hull collection
//   so remember to delete it later.
// Returns the created collision shape or NULL if couldn't create
btCollisionShape* BulletSim::BuildHullShape2(btCollisionShape* mesh)
{
	return NULL;
}

// From a previously created mesh shape, create a convex hull using the Bullet
//   HACD hull creation code. The created hull will go into the hull collection
//   so remember to delete it later.
// Returns 'true' if the hull was successfully created.
bool BulletSim::CreateHullFromMesh(MESHKEYTYPE hullkey, MESHKEYTYPE meshkey)
{
	// TODO: well, you know, like, write the code.
	return false;
}

// Using the shape data, create the RigidObject and put it in the world
bool BulletSim::CreateObject(ShapeData* data)
{
	// m_worldData.BSLog("BulletSim::CreateObject: id=%d", data->ID);

	bool ret = false;

	// If the object already exists, destroy it
	m_worldData.objects->RemoveAndDestroyObject(data->ID);

	// Create and add the new physical object
	IPhysObject* newObject = IPhysObject::PhysObjectFactory(&m_worldData, data);
	if (newObject != NULL)
	{
		// m_worldData.BSLog("CreateObject: created object of type= '%s'", newObject->GetType());
		m_worldData.objects->AddObject(data->ID, newObject);
		ret = true;
	}

	return ret;
}

btVector3 BulletSim::GetObjectPosition(IDTYPE id)
{
	btVector3 ret = btVector3(0.0, 0.0, 0.0);

	IPhysObject* obj;
	if (m_worldData.objects->TryGetObject(id, &obj))
	{
		ret = obj->GetObjectPosition();
	}
	return ret;
}

btQuaternion BulletSim::GetObjectOrientation(IDTYPE id)
{
	btQuaternion ret = btQuaternion::getIdentity();

	IPhysObject* obj;
	if (m_worldData.objects->TryGetObject(id, &obj))
	{
		ret = obj->GetObjectOrientation();
	}
	return ret;
}

bool BulletSim::SetObjectTranslation(IDTYPE id, btVector3& position, btQuaternion& rotation)
{
	bool ret = false;

	IPhysObject* obj;
	if (m_worldData.objects->TryGetObject(id, &obj))
	{
		obj->SetObjectTranslation(position, rotation);
		ret = true;
	}

	return ret;
}

bool BulletSim::SetObjectVelocity(IDTYPE id, btVector3& velocity)
{
	bool ret = false;

	IPhysObject* obj;
	if (m_worldData.objects->TryGetObject(id, &obj))
	{
		obj->SetObjectVelocity(velocity);
		ret = true;
	}

	return ret;
}

bool BulletSim::SetObjectAngularVelocity(IDTYPE id, btVector3& angularVelocity)
{
	bool ret = false;

	IPhysObject* obj;
	if (m_worldData.objects->TryGetObject(id, &obj))
	{
		obj->SetObjectAngularVelocity(angularVelocity);
		ret = true;
	}

	return ret;
}

bool BulletSim::SetObjectForce(IDTYPE id, btVector3& force)
{
	bool ret = false;

	IPhysObject* obj;
	if (m_worldData.objects->TryGetObject(id, &obj))
	{
		obj->SetObjectForce(force);
		ret = true;
	}

	return ret;
}

bool BulletSim::SetObjectScaleMass(IDTYPE id, btVector3& scale, float mass, bool isDynamic)
{
	bool ret = false;

	IPhysObject* obj;
	if (m_worldData.objects->TryGetObject(id, &obj))
	{
		obj->SetObjectScaleMass(scale, mass, isDynamic);
		ret = true;
	}

	return ret;
}

bool BulletSim::SetObjectCollidable(IDTYPE id, bool collidable)
{
	bool ret = false;

	IPhysObject* obj;
	if (m_worldData.objects->TryGetObject(id, &obj))
	{
		obj->SetObjectCollidable(collidable);
		ret = true;
	}

	return ret;
}

bool BulletSim::SetObjectDynamic(IDTYPE id, bool isDynamic, float mass)
{
	bool ret = false;

	IPhysObject* obj;
	if (m_worldData.objects->TryGetObject(id, &obj))
	{
		obj->SetObjectDynamic(isDynamic, mass);
		ret = true;
	}

	return ret;
}

// Adjust how gravity effects the object
// neg=fall quickly, 0=1g, 1=0g, pos=float up
bool BulletSim::SetObjectBuoyancy(IDTYPE id, float buoy)
{
	bool ret = false;

	IPhysObject* obj;
	if (m_worldData.objects->TryGetObject(id, &obj))
	{
		obj->SetObjectBuoyancy(buoy);
		ret = true;
	}

	return ret;
}

bool BulletSim::SetObjectProperties(IDTYPE id, bool isStatic, bool isSolid, bool genCollisions, float mass)
{
	bool ret = false;
	IPhysObject* obj;
	if (m_worldData.objects->TryGetObject(id, &obj))
	{
		obj->SetObjectProperties(isStatic, isSolid, genCollisions, mass);
		ret = true;
	}
	return ret;
}

bool BulletSim::HasObject(IDTYPE id)
{
	return m_worldData.objects->HasObject(id);
}

bool BulletSim::DestroyObject(IDTYPE id)
{
	// m_worldData.BSLog("BulletSim::DestroyObject: id=%d", id);
	return m_worldData.objects->RemoveAndDestroyObject(id);
}

// TODO: get this code working
SweepHit BulletSim::ConvexSweepTest(IDTYPE id, btVector3& fromPos, btVector3& targetPos, btScalar extraMargin)
{
	return SweepHit();
	/*
	SweepHit hit;
	hit.ID = ID_INVALID_HIT;

	btCollisionObject* castingObject = NULL;

	// Look for a character
	CharactersMapType::iterator cit = m_characters.find(id);
	if (cit != m_characters.end())
		castingObject = cit->second;

	if (!castingObject)
	{
		// Look for a rigid body
		BodiesMapType::iterator bit = m_bodies.find(id);
		if (bit != m_bodies.end())
			castingObject = bit->second;
	}

	if (castingObject)
	{
		btCollisionShape* shape = castingObject->getCollisionShape();

		// Convex sweep test only works with convex objects
		if (shape->isConvex())
		{
			btConvexShape* convex = static_cast<btConvexShape*>(shape);

			// Create transforms to sweep from and to
			btTransform from;
			from.setIdentity();
			from.setOrigin(fromPos);

			btTransform to;
			to.setIdentity();
			to.setOrigin(targetPos);

			btScalar originalMargin = convex->getMargin();
			convex->setMargin(originalMargin + extraMargin);

			// Create a callback for the test
			ClosestNotMeConvexResultCallback callback(castingObject);

			// Do the sweep test
			m_worldData.dynamicsWorld->convexSweepTest(convex, from, to, callback, m_worldData.dynamicsWorld->getDispatchInfo().m_allowedCcdPenetration);

			if (callback.hasHit())
			{
				hit.ID = CONVLOCALID(callback.m_hitCollisionObject->getCollisionShape()->getUserPointer());
				hit.Fraction = callback.m_closestHitFraction;
				hit.Normal = callback.m_hitNormalWorld;
				hit.Point = callback.m_hitPointWorld;
			}

			convex->setMargin(originalMargin);
		}
	}

	return hit;
	*/
}

// TODO: get this code working
RaycastHit BulletSim::RayTest(IDTYPE id, btVector3& from, btVector3& to)
{
	return RaycastHit();
	/*
	RaycastHit hit;
	hit.ID = ID_INVALID_HIT;

	btCollisionObject* castingObject = NULL;

	// Look for a character
	CharactersMapType::iterator cit = m_characters.find(id);
	if (cit != m_characters.end())
		castingObject = cit->second;

	if (!castingObject)
	{
		// Look for a rigid body
		BodiesMapType::iterator bit = m_bodies.find(id);
		if (bit != m_bodies.end())
			castingObject = bit->second;
	}

	if (castingObject)
	{
		// Create a callback for the test
		ClosestNotMeRayResultCallback callback(castingObject);

		// Do the raycast test
		m_worldData.dynamicsWorld->rayTest(from, to, callback);

		if (callback.hasHit())
		{
			hit.ID = CONVLOCALID(callback.m_collisionObject->getUserPointer());
			hit.Fraction = callback.m_closestHitFraction;
			hit.Normal = callback.m_hitNormalWorld;
			//hit.Point = callback.m_hitPointWorld; // TODO: Is this useful?
		}
	}

	return hit;
	*/
}

// TODO: get this code working
const btVector3 BulletSim::RecoverFromPenetration(IDTYPE id)
{
	/*
	// Look for a character
	CharactersMapType::iterator cit = m_characters.find(id);
	if (cit != m_characters.end())
	{
		btCollisionObject* character = cit->second;

		ContactSensorCallback contactCallback(character);
		m_worldData.dynamicsWorld->contactTest(character, contactCallback);

		return contactCallback.mOffset;
	}
	*/
	return btVector3(0.0, 0.0, 0.0);
}

bool BulletSim::UpdateParameter(IDTYPE localID, const char* parm, float val)
{
	btScalar btVal = btScalar(val);
	btVector3 btZeroVector3 = btVector3(0, 0, 0);

	// changes to the environment
	if (strcmp(parm, "gravity") == 0)
	{
		m_worldData.dynamicsWorld->setGravity(btVector3(0.f, 0.f, val));
		return true;
	}

	// something changed in the terrain so reset all the terrain parameters to values from m_worldData.params
	if (strcmp(parm, "terrain") == 0)
	{
		// some terrain physical parameter changed. Reset the terrain.
		if (m_worldData.Terrain)
		{
			m_worldData.Terrain->UpdatePhysicalParameters(
							m_worldData.params->terrainFriction,
							m_worldData.params->terrainRestitution,
							btZeroVector3);
		}
		return true;
	}

	IPhysObject* obj;
	if (!m_worldData.objects->TryGetObject(localID, &obj))
	{
		return false;
	}

	// the friction or restitution changed in the default parameters. Reset same.
	if (strcmp(parm, "avatar") == 0)
	{
		obj->UpdatePhysicalParameters(
				m_worldData.params->avatarFriction, 
				m_worldData.params->avatarRestitution,
				btZeroVector3);
		return true;
	}

	// the friction or restitution changed in the default parameters. Reset same.
	if (strcmp(parm, "object") == 0)
	{
		obj->UpdatePhysicalParameters(
				m_worldData.params->defaultFriction, 
				m_worldData.params->defaultRestitution,
				btZeroVector3);
		return true;
	}

	// changes to an object
	return obj->UpdateParameter(parm, val);
}

// #include "LinearMath/btQuickprof.h"
// Call Bullet to dump its performance stats
// Bullet must be patched to make this work. See BulletDetailLogging.patch
void BulletSim::DumpPhysicsStats()
{
	// CProfileManager::dumpAll();
	return;
}



