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
#include "Constraint.h"

#include "BulletCollision/CollisionDispatch/btSimulationIslandManager.h"

#include <set>

BulletSim::BulletSim(btScalar maxX, btScalar maxY, btScalar maxZ)
{
	// Make sure structures that will be created in initPhysics are marked as not created
	m_worldData.dynamicsWorld = NULL;
	m_worldData.objects = NULL;
	m_worldData.constraints = NULL;
	m_terrainObject = NULL;

	m_worldData.MinPosition = btVector3(0, 0, 0);
	m_worldData.MaxPosition = btVector3(maxX, maxY, maxZ);
	// start the terrain as flat at height 25
	m_worldData.heightMap = new HeightMapData(maxX, maxY, 25.0);
}

void BulletSim::initPhysics(ParamBlock* parms, 
							int maxCollisions, CollisionDesc* collisionArray, 
							int maxUpdates, EntityProperties* updateArray)
{
	// remember the pointers to pinned memory for returning collisions and property updates
	m_maxCollisionsPerFrame = maxCollisions;
	m_collidersThisFrameArray = collisionArray;
	m_maxUpdatesPerFrame = maxUpdates;
	m_updatesThisFrameArray = updateArray;

	// Parameters are in a block of pinned memory
	m_worldData.params = parms;
	// the collection of all the objects that are passed to the physics engine
	m_worldData.objects = new ObjectCollection();
	// the collection of the constraints that are used to create linkset
	m_worldData.constraints = new ConstraintCollection(&m_worldData);

	// create the functional parts of the physics simulation
	btDefaultCollisionConstructionInfo cci;
	// cci.m_defaultMaxPersistentManifoldPoolSize = 32768;
	m_collisionConfiguration = new btDefaultCollisionConfiguration(cci);
	m_dispatcher = new btCollisionDispatcher(m_collisionConfiguration);
	// m_dispatcher->setDispatcherFlags(btCollisionDispatcher::CD_DISABLE_CONTACTPOOL_DYNAMIC_ALLOCATION);
	
	m_broadphase = new btDbvtBroadphase();

	// the following is needed to enable GhostObjects
	// m_broadphase->getOverlappingPairCache()->setInternalGhostPairCallback(new btGhostPairCallback());
	
	m_solver = new btSequentialImpulseConstraintSolver();

	// Create the world
	btDiscreteDynamicsWorld* dynamicsWorld = new btDiscreteDynamicsWorld(m_dispatcher, m_broadphase, m_solver, m_collisionConfiguration);
	m_worldData.dynamicsWorld = dynamicsWorld;
	
	// disable the continuious recalculation of the static AABBs
	// http://www.bulletphysics.org/Bullet/phpBB3/viewtopic.php?f=9&t=4991
	// Note that movement or changes to a static object will not update the AABB. Do it explicitly.
	dynamicsWorld->setForceUpdateAllAabbs(false);
	
	// Randomizing the solver order makes object stacking more stable at a slight performance cost
	dynamicsWorld->getSolverInfo().m_solverMode |= SOLVER_RANDMIZE_ORDER;

	// setting to false means the islands are not reordered and split up for individual processing
	dynamicsWorld->getSimulationIslandManager()->setSplitIslands(false);

	// Performance speedup: http://bulletphysics.org/Bullet/phpBB3/viewtopic.php?p=14367
	// Actually a NOOP unless Bullet is compiled with USE_SEPDISTANCE_UTIL2 set.
	dynamicsWorld->getDispatchInfo().m_useConvexConservativeDistanceUtil = true;
	dynamicsWorld->getDispatchInfo().m_convexConservativeDistanceThreshold = btScalar(0.01);

	// Performance speedup: from BenchmarkDemo.cpp, ln 381
	// m_worldData.dynamicsWorld->getSolverInfo().m_solverMode |= SOLVER_ENABLE_FRICTION_DIRECTION_CACHING; //don't recalculate friction values each frame
	// m_worldData.dynamicsWorld->getSolverInfo().m_numIterations = 5; //few solver iterations 

	// Earth-like gravity
	dynamicsWorld->setGravity(btVector3(0.f, 0.f, m_worldData.params->gravity));

	// Information on creating a custom collision computation routine and a pointer to the computation
	// of friction and restitution at:
	// http://bulletphysics.org/Bullet/phpBB3/viewtopic.php?f=9&t=7922
	// foreach body that you want the callback, enable it with:
	// body->setCollisionFlags(body->getCollisionFlags() | btCollisionObject::CF_CUSTOM_MATERIAL_CALLBACK);

	// Start with a ground plane and a flat terrain
	CreateGroundPlane();
	CreateTerrain();
}

void BulletSim::exitPhysics()
{
	if (!m_worldData.dynamicsWorld)
		return;

	if (m_worldData.constraints)
	{
		m_worldData.constraints->Clear();
		delete m_worldData.constraints;
		m_worldData.constraints = NULL;
	}

	if (m_worldData.objects)
	{
		m_worldData.objects->Clear();
		delete m_worldData.objects;
		m_worldData.objects = NULL;
	}

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

	// The ground plane and terrain are deleted when the object list is cleared
	m_terrainObject = NULL;

	// Delete solver
	delete m_solver;
	m_solver = NULL;

	// Delete broadphase
	delete m_broadphase;
	m_broadphase = NULL;

	// Delete dispatcher
	delete m_dispatcher;
	m_dispatcher = NULL;

	// Delete collision config
	delete m_collisionConfiguration;
	m_collisionConfiguration = NULL;

	// Finally, end the world
	delete m_worldData.dynamicsWorld;
	m_worldData.dynamicsWorld = NULL;
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
		numSimSteps = m_worldData.dynamicsWorld->stepSimulation(timeStep, maxSubSteps, fixedTimeStep);

		// Put all of the updates this frame into m_updatesThisFrameArray
		int updates = 0;
		if (m_worldData.updatesThisFrame.size() > 0)
		{
			for (WorldData::UpdatesThisFrameMapType::const_iterator it = m_worldData.updatesThisFrame.begin(); 
										it != m_worldData.updatesThisFrame.end(); ++it)
			{
				m_updatesThisFrameArray[updates] = *(it->second);
				updates++;
				if (updates >= m_maxUpdatesPerFrame) break;
			}
			m_worldData.updatesThisFrame.clear();
		}

		// Update the values passed by reference into this function
		*updatedEntityCount = updates;
		*updatedEntities = m_updatesThisFrameArray;

		// Put all of the colliders this frame into m_collidersThisFrameArray
		std::set<unsigned long long> collidersThisFrame;
		int collisions = 0;
		int numManifolds = m_worldData.dynamicsWorld->getDispatcher()->getNumManifolds();
		for (int j = 0; j < numManifolds; j++)
		{
			btPersistentManifold* contactManifold = m_worldData.dynamicsWorld->getDispatcher()->getManifoldByIndexInternal(j);
			int numContacts = contactManifold->getNumContacts();
			if (numContacts == 0) continue;

			btCollisionObject* objA = static_cast<btCollisionObject*>(contactManifold->getBody0());
			btCollisionObject* objB = static_cast<btCollisionObject*>(contactManifold->getBody1());

			// when two objects collide, we only report one contact point
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
			unsigned long long collisionID = ((unsigned long long)idA << 32) | idB;

			// If this collision has not been seen yet, record it
			if (collidersThisFrame.find(collisionID) == collidersThisFrame.end())
			{
				collidersThisFrame.insert(collisionID);
				m_collidersThisFrameArray[collisions].aID = idA;
				m_collidersThisFrameArray[collisions].bID = idB;
				m_collidersThisFrameArray[collisions].point = contactPoint;
				m_collidersThisFrameArray[collisions].normal = contactNormal;
				collisions++;
			}

			if (collisions >= m_maxCollisionsPerFrame) break;
		}

		*collidersCount = collisions;
		*colliders = m_collidersThisFrameArray;
	}

	return numSimSteps;
}

// Copy the passed heightmap into the memory block used by Bullet
void BulletSim::SetHeightmap(float* heightmap)
{
	if (m_worldData.heightMap)
	{
		m_worldData.heightMap->UpdateHeightMap(heightmap, m_worldData.MaxPosition.getX(), m_worldData.MaxPosition.getY());
		if (m_terrainObject)
		{
			m_terrainObject->UpdateTerrain();
		}
	}
}

// Create a collision plane at height zero to stop things falling to oblivion
void BulletSim::CreateGroundPlane()
{
	m_worldData.objects->RemoveAndDestroyObject(ID_GROUND_PLANE);
	IPhysObject* groundPlane = new GroundPlaneObject(&m_worldData);
	m_worldData.objects->AddObject(ID_GROUND_PLANE, groundPlane);
}

// Based on the heightmap, create a mesh for the terrain and put it in the world
void BulletSim::CreateTerrain()
{
	// get rid of any old terrains lying around
	m_worldData.objects->RemoveAndDestroyObject(ID_TERRAIN);
	m_terrainObject = NULL;

	// Create the new terrain based on the heightmap in m_worldData
	m_terrainObject = new TerrainObject(&m_worldData);
	m_worldData.objects->AddObject(ID_TERRAIN, m_terrainObject);
}

// If using Bullet' convex hull code, refer to following link for parameter setting
// http://kmamou.blogspot.com/2011/11/hacd-parameters.html
// Another useful reference for ConvexDecomp
// http://www.bulletphysics.org/Bullet/phpBB3/viewtopic.php?t=7159

// Create a hull based on convex hull information
bool BulletSim::CreateHull(unsigned long long meshKey, int hullCount, float* hulls)
{
	// BSLog("CreateHull: hullCount=%d", hullCount);
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
		return true;
	}
	return false;
}

// Delete a hull
bool BulletSim::DestroyHull(unsigned long long meshKey)
{
	// BSLog("DeleteHull:");
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

// Create a mesh structure to be used for static objects
bool BulletSim::CreateMesh(unsigned long long meshKey, int indicesCount, int* indices, int verticesCount, float* vertices)
{
	// BSLog("CreateMesh: nIndices=%d, nVertices=%d, key=%ld", indicesCount, verticesCount, meshKey);
	WorldData::MeshesMapType::iterator it = m_worldData.Meshes.find(meshKey);
	if (it == m_worldData.Meshes.end())
	{
		// We must copy the indices and vertices since the passed memory is released when this call returns.
		btIndexedMesh indexedMesh;
		int* copiedIndices = new int[indicesCount];
		memcpy(copiedIndices, indices, indicesCount * sizeof(int));
		float* copiedVertices = new float[verticesCount * 3];
		memcpy(copiedVertices, vertices, verticesCount * 3 * sizeof(float));

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
	}
	return false;
}

// Delete a mesh
bool BulletSim::DestroyMesh(unsigned long long meshKey)
{
	// BSLog("DeleteMesh:");
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
		return true;
	}
	return false;
}


// Using the shape data, create the RigidObject and put it in the world
bool BulletSim::CreateObject(ShapeData* data)
{
	bool ret = false;

	// If the object already exists, destroy it
	m_worldData.objects->RemoveAndDestroyObject(data->ID);

	// Create and add the new physical object
	IPhysObject* newObject = IPhysObject::PhysObjectFactory(&m_worldData, data);
	if (newObject != NULL)
	{
		m_worldData.objects->AddObject(data->ID, newObject);
		ret = true;
	}

	return ret;
}

// Explanation of cfm (constraint force mixing) and erp(error reduction parameter) at:
//       http://bulletphysics.org/Bullet/phpBB3/viewtopic.php?t=6792
// Information on stabilty in constraint solver (see last comment from Erwin) at:
//       http://bulletphysics.org/Bullet/phpBB3/viewtopic.php?t=7686
void BulletSim::AddConstraint(IDTYPE id1, IDTYPE id2, 
							  btVector3& frame1, btQuaternion& frame1rot,
							  btVector3& frame2, btQuaternion& frame2rot,
	btVector3& lowLinear, btVector3& hiLinear, btVector3& lowAngular, btVector3& hiAngular)
{
	m_worldData.constraints->RemoveAndDestroyConstraint(id1, id2);		// remove any existing constraint

	btTransform frame1t, frame2t;
	frame1t.setIdentity();
	frame1t.setOrigin(frame1);
	frame1t.setRotation(frame1rot);
	frame2t.setIdentity();
	frame2t.setOrigin(frame2);
	frame2t.setRotation(frame2rot);
	Constraint* constraint = new Constraint(&m_worldData, id1, id2, frame1t, frame2t);
	constraint->SetLinear(lowLinear, hiLinear);
	constraint->SetAngular(lowAngular, hiAngular);
	constraint->UseFrameOffset(false);
	constraint->TranslationalLimitMotor(true, 5.0f, 0.1f);

	m_worldData.constraints->AddConstraint(constraint);

	/*
	IPhysObject* obj1;
	IPhysObject* obj2;
	if (m_worldData.objects->TryGetObject(id1, &obj1))
	{
		if (m_worldData.objects->TryGetObject(id2, &obj2))
		{
            // BSLog("AddConstraint: found body1=%d, body2=%d", id1, id2);
			btRigidBody* body1 = obj1->GetBody();
			btRigidBody* body2 = obj2->GetBody();

			btTransform frame1t, frame2t;
			frame1t.setIdentity();
			frame1t.setOrigin(frame1);
			frame1t.setRotation(frame1rot);
			frame2t.setIdentity();
			frame2t.setOrigin(frame2);
			frame2t.setRotation(frame2rot);
			btGeneric6DofConstraint* constraint = new btGeneric6DofConstraint(*body1, *body2, frame1t, frame2t, true);
			constraint->setLinearLowerLimit(lowLinear);
			constraint->setLinearUpperLimit(hiLinear);
			constraint->setAngularLowerLimit(lowAngular);
			constraint->setAngularUpperLimit(hiAngular);
			constraint->setUseFrameOffset(false);
			constraint->getTranslationalLimitMotor()->m_enableMotor[0] = true;
			constraint->getTranslationalLimitMotor()->m_targetVelocity[0] = 5.0f;
			constraint->getTranslationalLimitMotor()->m_maxMotorForce[0] = 0.1f;

			m_worldData.constraints->AddConstraint(id1, id2, constraint);
		}
	}
	*/
	return;
}

// When we are deleting and object, we need to make sure there are no constraints
// associated with it.
bool BulletSim::RemoveConstraintByID(IDTYPE id1)
{
	return m_worldData.constraints->RemoveAndDestroyConstraints(id1);
}

bool BulletSim::RemoveConstraint(IDTYPE id1, IDTYPE id2)
{
	return m_worldData.constraints->RemoveAndDestroyConstraint(id1, id2);
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
		obj->SetProperties(isStatic, isSolid, genCollisions, mass);
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
	// Remove any constraints associated with this object
	m_worldData.constraints->RemoveAndDestroyConstraints(id);

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

void BulletSim::UpdateParameter(IDTYPE localID, const char* parm, float val)
{
	btScalar btVal = btScalar(val);
	btVector3 btZeroVector3 = btVector3(0, 0, 0);

	// changes to the environment
	if (strcmp(parm, "gravity") == 0)
	{
		m_worldData.dynamicsWorld->setGravity(btVector3(0.f, 0.f, val));
		return;
	}

	// something changed in the terrain so reset all the terrain parameters to values from m_worldData.params
	if (strcmp(parm, "terrain") == 0)
	{
		// some terrain physical parameter changed. Reset the terrain.
		if (m_terrainObject)
		{
			m_terrainObject->UpdatePhysicalParameters(
							m_worldData.params->terrainFriction,
							m_worldData.params->terrainRestitution,
							btZeroVector3);
		}
		return;
	}

	IPhysObject* obj;
	if (!m_worldData.objects->TryGetObject(localID, &obj))
	{
		return;
	}

	// something changed in the avatar so reset all the terrain parameters to values from m_worldData.params
	if (strcmp(parm, "avatar") == 0)
	{
		obj->UpdatePhysicalParameters(
				m_worldData.params->avatarFriction, 
				m_worldData.params->avatarRestitution,
				btZeroVector3);
		return;
	}

	// something changed in an object so reset all the terrain parameters to values from m_worldData.params
	if (strcmp(parm, "object") == 0)
	{
		obj->UpdatePhysicalParameters(
				m_worldData.params->defaultFriction, 
				m_worldData.params->defaultRestitution,
				btZeroVector3);
		return;
	}

	// changes to an object
	obj->UpdateParameter(parm, val);
	return;
}

// #include "LinearMath/btQuickprof.h"
void BulletSim::DumpPhysicsStats()
{
	// call Bullet to dump its performance stats
	// CProfileManager::dumpAll();
	return;
}



