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
#include "BulletCollision/CollisionDispatch/btSimulationIslandManager.h"

#include <set>

BulletSim::BulletSim(btScalar maxX, btScalar maxY, btScalar maxZ)
{
	int i_maxX = (int)maxX;
	int i_maxY = (int)maxY;

	m_minPosition = btVector3(0, 0, 0);
	m_maxPosition = btVector3(maxX, maxY, maxZ);
	m_heightmapData = new float[i_maxX * i_maxY];

	for (int y = 0; y < i_maxY; y++)
	{
		for (int x = 0; x < i_maxX; x++)
		{
			m_heightmapData[y * i_maxX + x] = 0.0;
		}
	}
}

void BulletSim::initPhysics(ParamBlock* parms, 
							int maxCollisions, CollisionDesc* collisionArray, 
							int maxUpdates, EntityProperties* updateArray)
{
	// all our parameters in a block of pinned memory
	m_params = parms;

	// remember the pointers to pinned memory for returning collisions and property updates
	m_maxCollisionsPerFrame = maxCollisions;
	m_collidersThisFrameArray = collisionArray;
	m_maxUpdatesPerFrame = maxUpdates;
	m_updatesThisFrameArray = updateArray;

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
	m_dynamicsWorld = new btDiscreteDynamicsWorld(m_dispatcher, m_broadphase, m_solver, m_collisionConfiguration);
	
	// disable the continuious recalculation of the static AABBs
	// http://www.bulletphysics.org/Bullet/phpBB3/viewtopic.php?f=9&t=4991
	// Note that movement or changes to a static object will not update the AABB. Do it explicitly.
	m_dynamicsWorld->setForceUpdateAllAabbs(false);
	
	// Randomizing the solver order makes object stacking more stable at a slight performance cost
	m_dynamicsWorld->getSolverInfo().m_solverMode |= SOLVER_RANDMIZE_ORDER;

	// setting to false means the islands are not reordered and split up for individual processing
	m_dynamicsWorld->getSimulationIslandManager()->setSplitIslands(false);

	// Performance speedup: http://bulletphysics.org/Bullet/phpBB3/viewtopic.php?p=14367
	// Actually a NOOP unless Bullet is compiled with USE_SEPDISTANCE_UTIL2 set.
	m_dynamicsWorld->getDispatchInfo().m_useConvexConservativeDistanceUtil = true;
	m_dynamicsWorld->getDispatchInfo().m_convexConservativeDistanceThreshold = btScalar(0.01);

	// Performance speedup: from BenchmarkDemo.cpp, ln 381
	// m_dynamicsWorld->getSolverInfo().m_solverMode |= SOLVER_ENABLE_FRICTION_DIRECTION_CACHING; //don't recalculate friction values each frame
	// m_dynamicsWorld->getSolverInfo().m_numIterations = 5; //few solver iterations 

	// Earth-like gravity
	m_dynamicsWorld->setGravity(btVector3(0.f, 0.f, m_params->gravity));

	// Start with a ground plane and a flat terrain
	CreateGroundPlane();
	CreateTerrain();
}

void BulletSim::exitPhysics()
{
	if (!m_dynamicsWorld)
		return;

	// Clean up in the reverse order of creation/initialization

	// Remove the rigidbodies from the dynamics world and delete them
	for (int i = m_dynamicsWorld->getNumCollisionObjects() - 1; i >= 0; i--)
	{
		btCollisionObject* obj = m_dynamicsWorld->getCollisionObjectArray()[i];
		
		// Delete motion states attached to rigid bodies
		btRigidBody* body = btRigidBody::upcast(obj);
		if (body && body->getMotionState())
			delete body->getMotionState();
		
		// Remove the collision object from the scene
		m_dynamicsWorld->removeCollisionObject(obj);
		
		// Delete the collision shape
		btCollisionShape* shape = obj->getCollisionShape();
		if (shape)
			delete shape;

		// Delete the collision object
		delete obj;
	}
	m_bodies.clear();
	m_characters.clear();

	// Delete collision meshes
	for (HullsMapType::const_iterator it = m_hulls.begin(); it != m_hulls.end(); ++it)
    {
		btCollisionShape* collisionShape = it->second;
		delete collisionShape;
	}

	m_hulls.clear();

	// Delete collision meshes
	for (MeshesMapType::const_iterator it = m_meshes.begin(); it != m_meshes.end(); ++it)
    {
		btCollisionShape* collisionShape = it->second;
		delete collisionShape;
	}
	m_meshes.clear();

	// Ground plane and terrain shapes were deleted above
	m_planeShape = NULL;
	m_heightfieldShape = NULL;

	// Delete dynamics world
	delete m_dynamicsWorld;
	m_dynamicsWorld = NULL;

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

	delete m_dynamicsWorld;
	m_dynamicsWorld = NULL;
}

// Step the simulation forward by one full step and potentially some number of substeps
int BulletSim::PhysicsStep(btScalar timeStep, int maxSubSteps, btScalar fixedTimeStep, 
						   int* updatedEntityCount, EntityProperties** updatedEntities, 
						   int* collidersCount, CollisionDesc** colliders)
{
	int numSimSteps = 0;

	if (m_dynamicsWorld)
	{
		// The simulation calls the SimMotionState to put object updates into m_updatesThisFrame.
		numSimSteps = m_dynamicsWorld->stepSimulation(timeStep, maxSubSteps, fixedTimeStep);

		// Put all of the updates this frame into m_updatesThisFrameArray
		int updates = 0;
		if (m_updatesThisFrame.size() > 0)
		{
			for (UpdatesThisFrameMapType::const_iterator it = m_updatesThisFrame.begin(); it != m_updatesThisFrame.end(); ++it)
			{
				m_updatesThisFrameArray[updates] = *(it->second);
				updates++;
				if (updates >= m_maxUpdatesPerFrame) break;
			}
			m_updatesThisFrame.clear();
		}

		// Update the values passed by reference into this function
		*updatedEntityCount = updates;
		*updatedEntities = m_updatesThisFrameArray;

		// Put all of the colliders this frame into m_collidersThisFrameArray
		std::set<unsigned long long> collidersThisFrame;
		int collisions = 0;
		int numManifolds = m_dynamicsWorld->getDispatcher()->getNumManifolds();
		for (int j = 0; j < numManifolds; j++)
		{
			btPersistentManifold* contactManifold = m_dynamicsWorld->getDispatcher()->getManifoldByIndexInternal(j);
			int numContacts = contactManifold->getNumContacts();
			if (numContacts == 0) continue;

			btCollisionObject* objA = static_cast<btCollisionObject*>(contactManifold->getBody0());
			btCollisionObject* objB = static_cast<btCollisionObject*>(contactManifold->getBody1());

			// when two objects collide, we only report one contact point
			const btManifoldPoint& manifoldPoint = contactManifold->getContactPoint(0);
			const btVector3& contactPoint = manifoldPoint.getPositionWorldOnB();
			btVector3 contactNormal = -manifoldPoint.m_normalWorldOnB;	// make relative to A

			// Get the IDs of colliding objects (stored in the one user definable field)
			unsigned int idA = CONVLOCALID(objA->getCollisionShape()->getUserPointer());
			unsigned int idB = CONVLOCALID(objB->getCollisionShape()->getUserPointer());

			// Make sure idA is the lower ID so we don't record both 'A hit B' and 'B hit A'
			if (idA > idB)
			{
				unsigned int temp = idA;
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
	// Find the dimensions of our heightmap
	int maxX = (int)m_maxPosition.getX();
	int maxY = (int)m_maxPosition.getY();
	// Overwrite terrain data
	memcpy(m_heightmapData, heightmap, maxY * maxX * sizeof(float));
	CreateTerrain();
}

// Create a collision plane at height zero to stop things falling to oblivion
void BulletSim::CreateGroundPlane()
{
	// Initialize the ground plane at height 0 (Z-up)
	m_planeShape = new btStaticPlaneShape(btVector3(0, 0, 1), 1);
	m_planeShape->setMargin(m_params->collisionMargin);

	m_planeShape->setUserPointer((void*)ID_GROUND_PLANE);

	btDefaultMotionState* motionState = new btDefaultMotionState();
	btRigidBody::btRigidBodyConstructionInfo cInfo(0.0, motionState, m_planeShape);
	btRigidBody* body = new btRigidBody(cInfo);

	body->setCollisionFlags(btCollisionObject::CF_STATIC_OBJECT);
	
	m_dynamicsWorld->addRigidBody(body);
	m_bodies[ID_GROUND_PLANE] = body;
}

// Based on the heightmap, create a mesh for the terrain and put it in the world
void BulletSim::CreateTerrain()
{
	// Initialize the terrain that spans from 0,0,0 to m_maxPosition
	// TODO: Use the maxHeight from m_maxPosition.getZ()
	int heightStickWidth = (int)m_maxPosition.getX();
	int heightStickLength = (int)m_maxPosition.getY();

	const btScalar scaleFactor(1.0);
	float minHeight = 99999;
	float maxHeight = 0;
	// find the minimum and maximum height
	for (int yy = 0; yy<heightStickWidth; yy++)
	{
		for (int xx = 0; xx<heightStickLength; xx++)
		{
			float here = m_heightmapData[yy * heightStickWidth + xx];
			if (here < minHeight) minHeight = here;
			if (here > maxHeight) maxHeight = here;
		}
	}
	if (minHeight == maxHeight)
	{
		// make different so the terrain gets a bounding box
		minHeight = maxHeight - 1.0f;
	}
	const int upAxis = 2;
	m_heightfieldShape = new btHeightfieldTerrainShape(heightStickWidth, heightStickLength, m_heightmapData,
		scaleFactor, (btScalar)minHeight, (btScalar)maxHeight, upAxis, PHY_FLOAT, false);
	// there is no room between the terrain and an object
	m_heightfieldShape->setMargin(0.0f);
	// m_heightfieldShape->setMargin(gCollisionMargin);
	m_heightfieldShape->setUseDiamondSubdivision(true);

	m_heightfieldShape->setUserPointer((void*)ID_TERRAIN);

	// Set the heightfield origin
	btTransform heightfieldTr;
	heightfieldTr.setIdentity();
	heightfieldTr.setOrigin(btVector3(
		((float)heightStickWidth) * 0.5f,
		((float)heightStickLength) * 0.5f,
		minHeight + (maxHeight - minHeight) * 0.5f));

	btVector3 theOrigin = heightfieldTr.getOrigin();

	btDefaultMotionState* motionState = new btDefaultMotionState(heightfieldTr);
	btRigidBody::btRigidBodyConstructionInfo cInfo(0.0, motionState, m_heightfieldShape);
	btRigidBody* body = new btRigidBody(cInfo);

	body->setCollisionFlags(btCollisionObject::CF_STATIC_OBJECT);
	SetTerrainPhysicalParameters(body);

	// if there is a previous terrain, remove it
	DestroyObject(ID_TERRAIN);

	m_dynamicsWorld->addRigidBody(body);
	m_bodies[ID_TERRAIN] = body;
	m_dynamicsWorld->updateSingleAabb(body);
}

void BulletSim::SetTerrainPhysicalParameters(btRigidBody* body)
{
	body->setFriction(btScalar(m_params->terrainFriction));
	body->setHitFraction(btScalar(m_params->terrainHitFraction));
	body->setRestitution(btScalar(m_params->terrainRestitution));
	// body->setActivationState(DISABLE_DEACTIVATION);
	body->activate(true);
}

// If using Bullet' convex hull code, refer to following link for parameter setting
// http://kmamou.blogspot.com/2011/11/hacd-parameters.html
// Another useful reference for ConvexDecomp
// http://www.bulletphysics.org/Bullet/phpBB3/viewtopic.php?t=7159

// Create a hull based on convex hull information
bool BulletSim::CreateHull(unsigned long long meshKey, int hullCount, float* hulls)
{
	// BSLog("CreateHull: hullCount=%d", hullCount);
	HullsMapType::iterator it = m_hulls.find(meshKey);
	if (it == m_hulls.end())
	{
		// Create a compound shape that will wrap the set of convex hulls
		btCompoundShape* compoundShape = new btCompoundShape(false);

		btTransform childTrans;
		childTrans.setIdentity();
		compoundShape->setMargin(m_params->collisionMargin);
		
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
			convexShape->setMargin(m_params->collisionMargin);
			compoundShape->addChildShape(childTrans, convexShape);

			ii += (vertexCount * 3 + 4);
		}

		// Track this mesh
		m_hulls[meshKey] = compoundShape;
		return true;
	}
	return false;
}

// Delete a hull
bool BulletSim::DestroyHull(unsigned long long meshKey)
{
	// BSLog("DeleteHull:");
	HullsMapType::iterator it = m_hulls.find(meshKey);
	if (it != m_hulls.end())
	{
		btCompoundShape* compoundShape = m_hulls[meshKey];
		delete compoundShape;
		m_hulls.erase(it);
		return true;
	}
	return false;
}

// Create a mesh structure to be used for static objects
bool BulletSim::CreateMesh(unsigned long long meshKey, int indicesCount, int* indices, int verticesCount, float* vertices)
{
	// BSLog("CreateMesh: nIndices=%d, nVertices=%d, key=%ld", indicesCount, verticesCount, meshKey);
	MeshesMapType::iterator it = m_meshes.find(meshKey);
	if (it == m_meshes.end())
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
		
		m_meshes[meshKey] = meshShape;
	}
	return false;
}

// Delete a mesh
bool BulletSim::DestroyMesh(unsigned long long meshKey)
{
	// BSLog("DeleteMesh:");
	MeshesMapType::iterator it = m_meshes.find(meshKey);
	if (it != m_meshes.end())
	{
		btBvhTriangleMeshShape* tms = m_meshes[meshKey];
		/* This causes memory corruption.
		 * TODO: figure out when to properly release the memory allocated in CreateMesh.
		btIndexedMesh* smi = (btIndexedMesh*)tms->getMeshInterface();
		delete smi->m_triangleIndexBase;
		delete smi->m_vertexBase;
		*/
		delete tms;
		m_meshes.erase(it);
		return true;
	}
	return false;
}

// Create and return the collision shape specified by the ShapeData.
btCollisionShape* BulletSim::CreateShape(ShapeData* data)
{
	ShapeData::PhysicsShapeType type = data->Type;
	Vector3 scale = data->Scale;
	btVector3 scaleBt = scale.GetBtVector3();
	MeshesMapType::const_iterator mt;
	HullsMapType::const_iterator ht;

	btCollisionShape* shape = NULL;

	switch (type)
	{
		case ShapeData::SHAPE_AVATAR:
			shape = new btCapsuleShapeZ(m_params->avatarCapsuleRadius, m_params->avatarCapsuleHeight);
			shape->setMargin(m_params->collisionMargin);
			break;
		case ShapeData::SHAPE_BOX:
			// btBoxShape subtracts the collision margin from the half extents, so no 
			// fiddling with scale necessary
			// boxes are defined by their half extents
			shape = new btBoxShape(btVector3(0.5, 0.5, 0.5));	// this is really a unit box
			shape->setMargin(m_params->collisionMargin);
			AdjustScaleForCollisionMargin(shape, scaleBt);
			break;
		case ShapeData::SHAPE_CONE:	// TODO:
			shape = new btConeShapeZ(0.5, 1.0);
			shape->setMargin(m_params->collisionMargin);
			break;
		case ShapeData::SHAPE_CYLINDER:	// TODO:
			shape = new btCylinderShapeZ(btVector3(0.5f, 0.5f, 0.5f));
			shape->setMargin(m_params->collisionMargin);
			break;
		case ShapeData::SHAPE_MESH:
			mt = m_meshes.find(data->MeshKey);
			if (mt != m_meshes.end())
			{
				// BSLog("CreateShape: SHAPE_MESH. localID=%d", data->ID);
				btBvhTriangleMeshShape* origionalMeshShape = mt->second;
				// we have to copy the mesh shape because we don't keep use counters
				shape = DuplicateMeshShape(origionalMeshShape);
				shape->setMargin(m_params->collisionMargin);
				AdjustScaleForCollisionMargin(shape, scaleBt);
			}
			break;
		case ShapeData::SHAPE_HULL:
			ht = m_hulls.find(data->HullKey);
			if (ht != m_hulls.end())
			{
				// The compound shape stored in m_hulls is really just a storage container for
				// the the individual convex hulls and their offsets. Here we copy each child
				// convex hull and its offset to the new compound shape which will actually be
				// inserted into the physics simulation
				// BSLog("CreateShape: SHAPE_HULL. localID=%d", data->ID);
				btCompoundShape* originalCompoundShape = ht->second;
				shape = DuplicateCompoundShape(originalCompoundShape);
				shape->setMargin(m_params->collisionMargin);
				AdjustScaleForCollisionMargin(shape, scaleBt);
			}
			break;
		case ShapeData::SHAPE_SPHERE:
			shape = new btSphereShape(0.5);		// this is really a unit sphere
			shape->setMargin(m_params->collisionMargin);
			AdjustScaleForCollisionMargin(shape, scaleBt);
			break;
	}

	return shape;
}

// create a new compound shape that contains the hulls of the passed compound shape
btCompoundShape* BulletSim::DuplicateCompoundShape(btCompoundShape* originalCompoundShape)
{
	btCompoundShape* newCompoundShape = new btCompoundShape(false);

	int childCount = originalCompoundShape->getNumChildShapes();
	btCompoundShapeChild* children = originalCompoundShape->getChildList();

	for (int i = 0; i < childCount; i++)
	{
		btCollisionShape* childShape = children[i].m_childShape;
		btTransform childTransform = children[i].m_transform;

		newCompoundShape->addChildShape(childTransform, childShape);
	}

	return newCompoundShape;
}

btCollisionShape* BulletSim::DuplicateMeshShape(btBvhTriangleMeshShape* mShape)
{
	btBvhTriangleMeshShape* newTMS = new btBvhTriangleMeshShape(mShape->getMeshInterface(), true, true);
	return newTMS;
}


// Using the shape data, create the RigidObject and put it in the world
bool BulletSim::CreateObject(ShapeData* data)
{
	// If the object already exists, destroy it
	DestroyObject(data->ID);

	// Create the appropriate collision shape that will go into the body
	btCollisionShape* shape = CreateShape(data);

	if (!shape || shape->getShapeType() == INVALID_SHAPE_PROXYTYPE)
		return false;

	// Unpack ShapeData
	unsigned int id = data->ID;
	btVector3 position = data->Position.GetBtVector3();
	btQuaternion rotation = data->Rotation.GetBtQuaternion();
	btVector3 scale = data->Scale.GetBtVector3();
	btVector3 velocity = data->Velocity.GetBtVector3();
	btScalar maxScale = scale.m_floats[scale.maxAxis()];
	btScalar mass = btScalar(data->Mass);
	btScalar friction = btScalar(data->Friction);
	btScalar restitution = btScalar(data->Restitution);
	bool isStatic = (data->Static == 1);
	bool isCollidable = (data->Collidable == 1);

	// Save the ID for this shape in the user settable variable (used to know what is colliding)
	shape->setUserPointer((void*)id);
	
	// Create a starting transform
	btTransform startTransform;
	startTransform.setIdentity();
	startTransform.setOrigin(position);
	startTransform.setRotation(rotation);

	if (data->Type == ShapeData::SHAPE_AVATAR)
	{
		// Building an avatar
		// Avatars are created as rigid objects so they collide and have gravity

		// Inertia calculation for physical objects (non-zero mass)
		btVector3 localInertia(0, 0, 0);
		if (mass != 0.0f)
			shape->calculateLocalInertia(mass, localInertia);

		// Create the motion state and rigid body
		SimMotionState* motionState = new SimMotionState(data->ID, startTransform, &m_updatesThisFrame);
		btRigidBody::btRigidBodyConstructionInfo cInfo(mass, motionState, shape, localInertia);
		btRigidBody* character = new btRigidBody(cInfo);
		motionState->RigidBody = character;

		character->setCollisionFlags(character->getCollisionFlags() | btCollisionObject::CF_CHARACTER_OBJECT);

		SetAvatarPhysicalParameters(character, friction, restitution, velocity);

		m_dynamicsWorld->addRigidBody(character);
		m_characters[id] = character;

		/*
		// NOTE: Old code kept for reference
		// Building a kinematic character controller
		btPairCachingGhostObject* character = new btPairCachingGhostObject();
		character->setWorldTransform(startTransform);
		character->setCollisionShape(shape);
		character->setCollisionFlags(btCollisionObject::CF_CHARACTER_OBJECT | btCollisionObject::CF_NO_CONTACT_RESPONSE);
		character->setActivationState(DISABLE_DEACTIVATION);
		character->setContactProcessingThreshold(0.0);

		m_dynamicsWorld->addCollisionObject(character, btBroadphaseProxy::CharacterFilter);
		m_characters[id] = character;
		*/
	}
	else
	{
		// Building a rigid body

		btVector3 localInertia(0, 0, 0);
		shape->calculateLocalInertia(mass, localInertia);

		// Create the motion state and rigid body
		SimMotionState* motionState = new SimMotionState(data->ID, startTransform, &m_updatesThisFrame);
		btRigidBody::btRigidBodyConstructionInfo cInfo(mass, motionState, shape, localInertia);
		btRigidBody* body = new btRigidBody(cInfo);
		motionState->RigidBody = body;

		SetObjectPhysicalParameters(body, friction, restitution, velocity);

		// Set the dynamic and collision flags (for static and phantom objects)
		SetObjectProperties(body, isStatic, isCollidable, false, mass);

		m_dynamicsWorld->addRigidBody(body);
		m_bodies[id] = body;
	}

	return true;
}

void BulletSim::SetAvatarPhysicalParameters(btRigidBody* character, btScalar frict, btScalar resti, const btVector3& velo)
{
	// Tweak continuous collision detection parameters
	// Only perform continuious collision detection (CCD) if movement last frame was more than threshold
	if (m_params->ccdMotionThreshold > 0.0f)
	{
		character->setCcdMotionThreshold(btScalar(m_params->ccdMotionThreshold));
		character->setCcdSweptSphereRadius(btScalar(m_params->ccdSweptSphereRadius));
	}

	character->setFriction(frict);
	character->setRestitution(resti);
	character->setActivationState(DISABLE_DEACTIVATION);
	character->setContactProcessingThreshold(0.0);

	character->setAngularFactor(btVector3(0, 0, 0));	// makes the capsule not fall over
	character->setLinearVelocity(velo);
	character->setInterpolationLinearVelocity(btVector3(0, 0, 0));	// turns off unexpected interpolation
	character->setInterpolationAngularVelocity(btVector3(0, 0, 0));
	character->setInterpolationWorldTransform(character->getWorldTransform());
}

void BulletSim::SetObjectPhysicalParameters(btRigidBody* body, btScalar frict, btScalar resti, const btVector3& velo)
{
	// Tweak continuous collision detection parameters
	if (m_params->ccdMotionThreshold > 0.0f)
	{
		body->setCcdMotionThreshold(btScalar(m_params->ccdMotionThreshold));
		body->setCcdSweptSphereRadius(btScalar(m_params->ccdSweptSphereRadius));
	}
	body->setDamping(m_params->linearDamping, m_params->angularDamping);
	body->setDeactivationTime(m_params->deactivationTime);
	body->setSleepingThresholds(m_params->linearSleepingThreshold, m_params->angularSleepingThreshold);
	body->setContactProcessingThreshold(m_params->contactProcessingThreshold);

	body->setFriction(frict);
	body->setRestitution(resti);
	body->setLinearVelocity(velo);
	// per http://www.bulletphysics.org/Bullet/phpBB3/viewtopic.php?t=3382
	body->setInterpolationLinearVelocity(btVector3(0, 0, 0));
	body->setInterpolationAngularVelocity(btVector3(0, 0, 0));
	body->setInterpolationWorldTransform(body->getWorldTransform());
}

// A linkset is a compound collision object that is made up of the hulls
// of all the parts of the linkset.
// We are passed an array of shape data for all of the pieces. Their
// hulls should have already been created.
// NOTE: this way does not work and the code should be removed
void BulletSim::CreateLinkset(int objectCount, ShapeData* shapes)
{
	// BSLog("CreateLinkset: total prims = %d", objectCount);
	// the first shape is the base shape that we will replace with the linkset
	int32_t baseID = shapes[0].ID;

	// the base shape is forced to always be a compound shape by the mesh creation code
	btCollisionShape* collisionShape = CreateShape(&shapes[0]);
	if (!collisionShape->isCompound())
	{
		// BSLog("CreateLinkset: base shape is not a compound shape");
		return;
	}
	btCompoundShape* baseShape = (btCompoundShape*)collisionShape;

	// loop through all of the children adding their hulls to the base hull
	for (int ii = 1; ii < objectCount; ii++)
	{
		btCollisionShape* childShape = CreateShape(&shapes[ii]);
		btTransform childTransform;
		childTransform.setIdentity();
		// we're passed rotation in world coordinates. Change rotation to be relative to parent
		btQuaternion parentWorldRotation = shapes[0].Rotation.GetBtQuaternion();
		btQuaternion childWorldRotation = shapes[ii].Rotation.GetBtQuaternion();
		btQuaternion childRelativeRotation = parentWorldRotation * childWorldRotation.inverse();

		// the child prim is offset and rotated from the base
		btVector3 parentWorldPosition = shapes[0].Position.GetBtVector3();
		btVector3 childWorldPosition = shapes[ii].Position.GetBtVector3();
		btVector3 childRelativePosition = quatRotate(parentWorldRotation.inverse(), (childWorldPosition - parentWorldPosition));

		childTransform.setOrigin(childRelativePosition);
		childTransform.setRotation(childRelativeRotation);
		baseShape->addChildShape(childTransform, childShape);
	}

	// find the current root rigid body and replace its shape with the whole linkset shape
	BodiesMapType::iterator bit = m_bodies.find(baseID);
	if (bit != m_bodies.end())
	{
		btRigidBody* baseBody = bit->second;
		btCollisionShape* oldCollisionShape = baseBody->getCollisionShape();
		delete oldCollisionShape;
		baseBody->setCollisionShape(baseShape);
	}
	return;
}

void BulletSim::AddConstraint(unsigned int id1, unsigned int id2, 
							  btVector3& frame1, btQuaternion& frame1rot,
							  btVector3& frame2, btQuaternion& frame2rot,
	btVector3& lowLinear, btVector3& hiLinear, btVector3& lowAngular, btVector3& hiAngular)
{
	RemoveConstraint(id1, id2);		// remove any existing constraint
	BodiesMapType::iterator bit1 = m_bodies.find(id1);
	if (bit1 != m_bodies.end())
	{
		btRigidBody* body1 = bit1->second;
		BodiesMapType::iterator bit2 = m_bodies.find(id2);
		if (bit2 != m_bodies.end())
		{
            // BSLog("AddConstraint: found body1=%d, body2=%d", id1, id2);
			btRigidBody* body2 = bit2->second;
			btTransform frame1t, frame2t;
			frame1t.setIdentity();
			frame1t.setOrigin(frame1);
			frame1t.setRotation(frame1rot);
			frame2t.setIdentity();
			frame2t.setOrigin(frame2);
			frame2t.setRotation(frame2rot);
			btGeneric6DofConstraint* constraint = new btGeneric6DofConstraint(*body1, *body2, frame1t, frame2t, true);
            m_dynamicsWorld->addConstraint(constraint, true);
			constraint->setLinearLowerLimit(lowLinear);
			constraint->setLinearUpperLimit(hiLinear);
			constraint->setAngularLowerLimit(lowAngular);
			constraint->setAngularUpperLimit(hiAngular);
			constraint->setUseFrameOffset(false);
			constraint->getTranslationalLimitMotor()->m_enableMotor[0] = true;
			constraint->getTranslationalLimitMotor()->m_targetVelocity[0] = 5.0f;
			constraint->getTranslationalLimitMotor()->m_maxMotorForce[0] = 0.1f;

			// Create a unique ID for this constraint
			unsigned long long constraintID = GenConstraintID(id1, id2);

			// this constraint should not be in the list (deleted before)
			if (m_constraints.find(constraintID) == m_constraints.end())
			{
				m_constraints[constraintID] = constraint;
			}
		}
	}
	return;
}

// When we are deleting and object, we need to make sure there are no constraints
// associated with it.
bool BulletSim::RemoveConstraintByID(unsigned int id1)
{
	bool removedSomething = false;
	bool doAgain = true;
	while (doAgain)
	{
		doAgain = false;	// start out thinking one time through is enough
		ConstraintMapType::iterator it = m_constraints.begin();
		while (it != m_constraints.end())
		{
			unsigned long long constraintID = it->first;
			// if this constraint contains the passed localID, delete the constraint
			if ((((unsigned int)(constraintID & 0xffffffff)) == id1)
				|| (((unsigned int)(constraintID >> 32) & 0xffffffff) == id1))
			{
				btGeneric6DofConstraint* constraint = it->second;
		 		m_dynamicsWorld->removeConstraint(constraint);
				m_constraints.erase(it);
				delete constraint;
				removedSomething = true;
				doAgain = true;	// if we deleted, we scan the list again for another match
				break;

			}
			it++;
		}
	}
	return removedSomething;	// return 'true' if we actually deleted a constraint
}

// When properties of the object change, the base transforms in the constraint should be recomputed
bool BulletSim::RecalculateAllConstraintsByID(unsigned int id1)
{
	bool recalcuatedSomething = false;
	ConstraintMapType::iterator it = m_constraints.begin();
	while (it != m_constraints.end())
	{
		unsigned long long constraintID = it->first;
		// if this constraint contains the passed localID, recalcuate its transforms
		if ((((unsigned int)(constraintID & 0xffffffff)) == id1)
			|| (((unsigned int)(constraintID >> 32) & 0xffffffff) == id1))
		{
			btGeneric6DofConstraint* constraint = it->second;
			constraint->calculateTransforms();
			recalcuatedSomething = true;
		}
		it++;
	}
	return recalcuatedSomething;	// return 'true' if we actually recalcuated a constraint
}

bool BulletSim::RemoveConstraint(unsigned int id1, unsigned int id2)
{
	unsigned long long constraintID = GenConstraintID(id1, id2);
	ConstraintMapType::iterator it = m_constraints.find(constraintID);
	if (it != m_constraints.end())
	{
		btGeneric6DofConstraint* constraint = m_constraints[constraintID];
 		m_dynamicsWorld->removeConstraint(constraint);
		m_constraints.erase(it);
		delete constraint;
		return true;
	}
	return false;
}

// There can be only one constraint between objects. Always use the lowest id as the first part
// of the key so the same constraint will be found no matter what order the caller passes them.
unsigned long long BulletSim::GenConstraintID(unsigned int id1, unsigned int id2)
{
	if (id1 < id2)
		return ((unsigned long long)id1 << 32) | id2;
	else
		return ((unsigned long long)id2 << 32) | id1;
	return 0;
}

btVector3 BulletSim::GetObjectPosition(unsigned int id)
{
	// Look for a character
	CharactersMapType::iterator cit = m_characters.find(id);
	if (cit != m_characters.end())
	{
		btRigidBody* character = cit->second;

		btTransform xform = character->getWorldTransform();
		return xform.getOrigin();
	}

	// Look for a rigid body
	BodiesMapType::iterator bit = m_bodies.find(id);
	if (bit != m_bodies.end())
	{
		btRigidBody* body = bit->second;

		btTransform xform = body->getWorldTransform();
		return xform.getOrigin();
	}

	return btVector3(0.0, 0.0, 0.0);
}

bool BulletSim::SetObjectTranslation(unsigned int id, btVector3& position, btQuaternion& rotation)
{
	const btVector3 ZERO_VECTOR(0.0, 0.0, 0.0);

	// Build a transform containing the new position and rotation
	btTransform transform;
	transform.setIdentity();
	transform.setOrigin(position);
	transform.setRotation(rotation);

	// Look for a character
	CharactersMapType::iterator cit = m_characters.find(id);
	if (cit != m_characters.end())
	{
		btRigidBody* character = cit->second;

		// Set the new transform for this character controller
		character->setWorldTransform(transform);
		return true;
	}

	// Look for a rigid body
	BodiesMapType::iterator bit = m_bodies.find(id);
	if (bit != m_bodies.end())
	{
		btRigidBody* body = bit->second;

		// Clear all forces for this object
		body->setLinearVelocity(ZERO_VECTOR);
		body->setAngularVelocity(ZERO_VECTOR);
		body->clearForces();

		// Set the new transform for the rigid body and the motion state
		body->setWorldTransform(transform);
		body->getMotionState()->setWorldTransform(transform);

		body->activate(false);
		return true;
	}

	return false;
}

bool BulletSim::SetObjectVelocity(unsigned int id, btVector3& velocity)
{
	// Look for a character
	CharactersMapType::iterator cit = m_characters.find(id);
	if (cit != m_characters.end())
	{
		btRigidBody* character = cit->second;

		character->setLinearVelocity(velocity);
		character->activate(true);
		return true;
	}

	// Look for a rigid body
	BodiesMapType::iterator it = m_bodies.find(id);
	if (it != m_bodies.end())
	{
		btRigidBody* body = it->second;

		// Set the linear velocity for this object
		body->setLinearVelocity(velocity);
		body->activate(false);
		return true;
	}
	return false;
}

bool BulletSim::SetObjectAngularVelocity(unsigned int id, btVector3& angularVelocity)
{
	// Look for a rigid body
	BodiesMapType::iterator it = m_bodies.find(id);
	if (it != m_bodies.end())
	{
		btRigidBody* body = it->second;

		// Set the linear velocity for this object
		body->setAngularVelocity(angularVelocity);
		body->activate(true);
		return true;
	}
	return false;
}

bool BulletSim::SetObjectForce(unsigned int id, btVector3& force)
{
	// Look for a rigid body
	BodiesMapType::iterator it = m_bodies.find(id);
	if (it != m_bodies.end())
	{
		btRigidBody* body = it->second;

		// Apply the force to this object
		body->applyCentralForce(force);
		body->activate(false);
		return true;
	}

	return false;
}

bool BulletSim::SetObjectScaleMass(unsigned int id, btVector3& scale, float mass, bool isDynamic)
{
	// BSLog("SetObjectScaleMass: mass=%f", mass);
	const btVector3 ZERO_VECTOR(0.0, 0.0, 0.0);

	// Look for a character
	CharactersMapType::iterator cit = m_characters.find(id);
	if (cit != m_characters.end())
	{
		btRigidBody* character = cit->second;
		btCollisionShape* shape = character->getCollisionShape();

		// Set the new scale
		shape->setLocalScaling(scale);

		return true;
	}

	// Look for a rigid body
	BodiesMapType::iterator bit = m_bodies.find(id);
	if (bit != m_bodies.end())
	{
		btRigidBody* body = bit->second;
		btCollisionShape* shape = body->getCollisionShape();

		// NOTE: From the author of Bullet: "If you want to change important data 
		// from objects, you have to remove them from the world first, then make the 
		// change, and re-add them to the world." It's my understanding that some of
		// the following changes, including mass, constitute "important data"
		m_dynamicsWorld->removeRigidBody(body);

		// Clear all forces for this object
		body->setLinearVelocity(ZERO_VECTOR);
		body->setAngularVelocity(ZERO_VECTOR);
		body->clearForces();

		// Set the new scale
		AdjustScaleForCollisionMargin(shape, scale);

		// apply the mass and dynamicness
		SetObjectDynamic(body, isDynamic, mass);

		// Add the rigid body back to the simulation
		m_dynamicsWorld->addRigidBody(body);

		// Calculate a new AABB for this object
		m_dynamicsWorld->updateSingleAabb(body);

		body->activate(false);
		return true;
	}

	return false;
}

bool BulletSim::SetObjectCollidable(unsigned int id, bool collidable)
{
	// Look for a rigid body
	BodiesMapType::iterator it = m_bodies.find(id);
	if (it != m_bodies.end())
	{
		btRigidBody* body = it->second;
		SetObjectCollidable(body, collidable);
		return true;
	}
	return false;
}

void BulletSim::SetObjectCollidable(btRigidBody* body, bool collidable)
{
	// Toggle the CF_NO_CONTACT_RESPONSE on or off for the object to enable/disable phantom behavior
	if (collidable)
		body->setCollisionFlags(body->getCollisionFlags() & ~btCollisionObject::CF_NO_CONTACT_RESPONSE);
	else
		body->setCollisionFlags(body->getCollisionFlags() | btCollisionObject::CF_NO_CONTACT_RESPONSE);

	body->activate(false);
	// BSLog("SetObjectCollidable: after activate: id=%u, collidable=%d",
	// 	CONVLOCALID(body->getCollisionShape()->getUserPointer()), collidable);
	return;
}

bool BulletSim::SetObjectDynamic(unsigned int id, bool isDynamic, float mass)
{
	// Look for a rigid body
	BodiesMapType::iterator it = m_bodies.find(id);
	if (it != m_bodies.end())
	{
		btRigidBody* body = it->second;
		m_dynamicsWorld->removeRigidBody(body);
		SetObjectDynamic(body, isDynamic, mass);
		m_dynamicsWorld->addRigidBody(body);
		m_dynamicsWorld->updateSingleAabb(body);
		body->activate(false);
		return true;
	}
	return false;

}

void BulletSim::SetObjectDynamic(btRigidBody* body, bool isDynamic, float mass)
{
	const btVector3 ZERO_VECTOR(0.0, 0.0, 0.0);

	// BSLog("SetObjectDynamic: isDynamic=%d, mass=%f", isDynamic, mass);
	if (isDynamic)
	{
		body->setCollisionFlags(body->getCollisionFlags() & ~btCollisionObject::CF_STATIC_OBJECT);
		// We can't deactivate a physical object because Bullet does not tell us when
		// the object goes idle. The lack of this event means small velocity values are
		// left on the object and the viewer displays the object floating off.
		// body->setActivationState(DISABLE_DEACTIVATION);
		// Change for if Bullet has been modified to call MotionState when body goes inactive

		// Recalculate local inertia based on the new mass
		btVector3 localInertia(0, 0, 0);
		body->getCollisionShape()->calculateLocalInertia(mass, localInertia);

		// Set the new mass
		body->setMassProps(mass, localInertia);
		// BSLog("SetObjectDynamic: dynamic. ID=%d, Mass = %f", CONVLOCALID(body->getCollisionShape()->getUserPointer()), mass);
		body->updateInertiaTensor();

		// NOTE: Workaround for issue http://code.google.com/p/bullet/issues/detail?id=364
		// when setting mass
		body->setGravity(body->getGravity());

		// if there are any constraints on this object, recalcuate transforms for new mass
		this->RecalculateAllConstraintsByID(CONVLOCALID(body->getCollisionShape()->getUserPointer()));
	}
	else
	{
		body->setCollisionFlags(body->getCollisionFlags() | btCollisionObject::CF_STATIC_OBJECT);
		// body->setActivationState(WANTS_DEACTIVATION);
		// force the static object to be inactive
		body->forceActivationState(ISLAND_SLEEPING);

		// Clear all forces for this object
		body->setLinearVelocity(ZERO_VECTOR);
		body->setAngularVelocity(ZERO_VECTOR);
		body->clearForces();

		// Set the new mass (the caller should be passing zero)
		body->setMassProps(mass, ZERO_VECTOR);
		// BSLog("SetObjectDynamic: not dynamic. ID=%d, Mass = %f", CONVLOCALID(body->getCollisionShape()->getUserPointer()), mass);
		body->updateInertiaTensor();
		body->setGravity(body->getGravity());
	}
	// don't force activation so we don't undo the "ISLAND_SLEEPING" for static objects
	body->activate(false);
	return;
}

// Adjust how gravity effects the object
// neg=fall quickly, 0=1g, 1=0g, pos=float up
bool BulletSim::SetObjectBuoyancy(unsigned int id, float buoy)
{
	float grav = m_params->gravity * (1.0f - buoy);

	CharactersMapType::iterator it = m_characters.find(id);
	if (it != m_characters.end())
	{
		btRigidBody* body = it->second;

		body->setGravity(btVector3(0, 0, grav));

		body->activate(true);
		return true;
	}
	// Look for a rigid body
	BodiesMapType::iterator bit = m_bodies.find(id);
	if (bit != m_bodies.end())
	{
		btRigidBody* body = bit->second;

		body->setGravity(btVector3(0, 0, grav));

		body->activate(false);
		return true;
	}
	return false;
}

bool BulletSim::SetObjectProperties(unsigned int id, bool isStatic, bool isSolid, bool genCollisions, float mass)
{
	// Look for a rigid body
	BodiesMapType::iterator it = m_bodies.find(id);
	if (it != m_bodies.end())
	{
		btRigidBody* body = it->second;
		m_dynamicsWorld->removeRigidBody(body);
		SetObjectProperties(body, isStatic, isSolid, genCollisions, mass);
		m_dynamicsWorld->addRigidBody(body);
		// Why is this commented out?
		// m_dynamicsWorld->updateSingleAabb(body);
		body->activate(false);
		return true;
	}
	return false;

}

void BulletSim::SetObjectProperties(btRigidBody* body, bool isStatic, bool isSolid, bool genCollisions, float mass)
{
	// BSLog("SetObjectProperties: id=%u, isStatic=%d, isSolid=%d, genCollisions=%d, mass=%f", 
	// 	CONVLOCALID(body->getCollisionShape()->getUserPointer()), isStatic, isSolid, genCollisions, mass);
	SetObjectDynamic(body, !isStatic, mass);		// this handles the static part
	SetObjectCollidable(body, isSolid);
	if (genCollisions)
	{
		// for the moment, everything generates collisions
		// TODO: Add a flag to CollisionFlags that is checked in StepSimulation on whether to pass up or not
	}
}

// Bullet has a 'collisionMargin' that makes the mesh a little bigger. This routine
// reduces the scale of the underlying mesh to make the mesh plus margin the
// same size as the original mesh.
// TODO: figure out of this works correctly. For the moment set gCollisionMargin to zero.
//    Bullet tries to use scale only on the shape and does not include the margin in the scale
//    calculation. This is true in most cases but there seem to be some shapes that get
//    this wrong. 
void BulletSim::AdjustScaleForCollisionMargin(btCollisionShape* shape, btVector3& scale)
{
	btVector3 aabbMin;
	btVector3 aabbMax;
	btTransform transform;
	transform.setIdentity();
	
	// we use the constant margin because SPHERE getMargin does not return the real margin
	// btScalar margin = shape->getMargin();
	btScalar margin = m_params->collisionMargin;
	// the margin adjustment only has to happen to our compound shape. Internal shapes don't need it.
	// if (shape->isCompound() && margin > 0.01)
	// looks like internal shapes need it after all
	if (margin > 0.01)
	{
		shape->getAabb(transform, aabbMin, aabbMax);
		// the AABB contains the margin already
		btScalar xExtent = aabbMax.x() - aabbMin.x();
		btScalar xAdjustment = (xExtent - margin - margin) / xExtent;
		btScalar yExtent = aabbMax.y() - aabbMin.y();
		btScalar yAdjustment = (yExtent - margin - margin) / yExtent;
		btScalar zExtent = aabbMax.z() - aabbMin.z();
		btScalar zAdjustment = (zExtent - margin - margin) / zExtent;
		// BSLog("Adjust: m=%f, xE=%f, xA=%f, yE=%f, yA=%f, zE=%f, zA=%f, sX=%f, sY=%f, sZ=%f", margin,
		// 	xExtent, xAdjustment, yExtent, yAdjustment, zExtent, zAdjustment,
		// 	scale.x(), scale.y(), scale.z());
		shape->setLocalScaling(btVector3(scale.x()*xAdjustment, scale.y()*yAdjustment, scale.z()*zAdjustment));
	}
	else
	{
		// margin is small enough we won't fiddle with the adjustment
		shape->setLocalScaling(btVector3(scale.x(), scale.y(), scale.z()));
	}
	return;
}

bool BulletSim::HasObject(unsigned int id)
{
	// Look for a character
	CharactersMapType::iterator cit = m_characters.find(id);
	if (cit != m_characters.end())
		return true;

	// Look for a rigid body
	BodiesMapType::iterator bit = m_bodies.find(id);
	return (bit != m_bodies.end());
}

bool BulletSim::DestroyObject(unsigned int id)
{
	// Remove any constraints associated with this object
	RemoveConstraintByID(id);

	// Look for a character
	CharactersMapType::iterator cit = m_characters.find(id);
	if (cit != m_characters.end())
	{
		btRigidBody* character = cit->second;
		btCollisionShape* shape = character->getCollisionShape();

		// Remove the character from the map of characters
		m_characters.erase(cit);

		// Remove the character from the scene
		m_dynamicsWorld->removeCollisionObject(character);

		// Delete the character and character shape
		delete character;
		delete shape;

		return true;
	}

	// Look for a rigid body
	BodiesMapType::iterator bit = m_bodies.find(id);
	if (bit != m_bodies.end())
	{
		btRigidBody* body = bit->second;
		btCollisionShape* shape = body->getCollisionShape();

		// Remove the rigid body from the map of rigid bodies
		m_bodies.erase(bit);

		// Remove the rigid body from the scene
		m_dynamicsWorld->removeRigidBody(body);

		// Delete the MotionState
		btMotionState* motionState = body->getMotionState();
		if (motionState)
			delete motionState;
		
		// Delete the body and shape
		delete body;
		delete shape;

		return true;
	}

	return false;
}

SweepHit BulletSim::ConvexSweepTest(unsigned int id, btVector3& fromPos, btVector3& targetPos, btScalar extraMargin)
{
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
			m_dynamicsWorld->convexSweepTest(convex, from, to, callback, m_dynamicsWorld->getDispatchInfo().m_allowedCcdPenetration);

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
}

RaycastHit BulletSim::RayTest(unsigned int id, btVector3& from, btVector3& to)
{
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
		m_dynamicsWorld->rayTest(from, to, callback);

		if (callback.hasHit())
		{
			hit.ID = CONVLOCALID(callback.m_collisionObject->getUserPointer());
			hit.Fraction = callback.m_closestHitFraction;
			hit.Normal = callback.m_hitNormalWorld;
			//hit.Point = callback.m_hitPointWorld; // TODO: Is this useful?
		}
	}

	return hit;
}

const btVector3 BulletSim::RecoverFromPenetration(unsigned int id)
{
	// Look for a character
	CharactersMapType::iterator cit = m_characters.find(id);
	if (cit != m_characters.end())
	{
		btCollisionObject* character = cit->second;

		ContactSensorCallback contactCallback(character);
		m_dynamicsWorld->contactTest(character, contactCallback);

		return contactCallback.mOffset;
	}

	return btVector3(0.0, 0.0, 0.0);
}

void BulletSim::UpdateParameter(unsigned int localID, const char* parm, float val)
{
	btScalar btVal = btScalar(val);

	// changes to the environment
	if (strcmp(parm, "gravity") == 0)
	{
		m_dynamicsWorld->setGravity(btVector3(0.f, 0.f, val));
		return;
	}

	// something changed in the terrain so reset all the terrain parameters to values from m_params
	if (strcmp(parm, "terrain") == 0)
	{
		// some terrain physical parameter changed. Reset the terrain.
		BodiesMapType::iterator bit = m_bodies.find(ID_TERRAIN);
		if (bit != m_bodies.end())
		{
			btRigidBody* body = bit->second;
			SetTerrainPhysicalParameters(body);
		}
		return;
	}

	// something changed in the avatar so reset all the terrain parameters to values from m_params
	if (strcmp(parm, "avatar") == 0)
	{
		CharactersMapType::iterator cit = m_characters.find(localID);
		if (cit != m_characters.end())
		{
			btRigidBody* character = cit->second;
			SetAvatarPhysicalParameters(character, 
					m_params->avatarFriction, 
					m_params->avatarRestitution,
					btVector3(0, 0, 0));
		}
		return;
	}

	// something changed in an object so reset all the terrain parameters to values from m_params
	if (strcmp(parm, "object") == 0)
	{
		BodiesMapType::iterator bit = m_bodies.find(localID);
		if (bit != m_bodies.end())
		{
			btRigidBody* body = bit->second;
			SetObjectPhysicalParameters(body, 
					m_params->defaultFriction, 
					m_params->defaultRestitution,
					btVector3(0, 0, 0));
		}
		return;
	}

	// changes to an object
	btRigidBody* body = NULL;

	CharactersMapType::iterator cit = m_characters.find(localID);
	if (cit != m_characters.end())
	{
		body = cit->second;
	}
	if (body == NULL)
	{
		BodiesMapType::iterator bit = m_bodies.find(localID);
		if (bit != m_bodies.end())
		{
			body = bit->second;
		}
	}
	if (body == NULL)
		return;

	if (strcmp(parm, "lineardamping") == 0)
	{
		body->setDamping(btVal, m_params->angularDamping);
		return;
	}
	if (strcmp(parm, "angulardamping") == 0)
	{
		body->setDamping(m_params->linearDamping, btVal);
		return;
	}
	if (strcmp(parm, "deactivationtime") == 0)
	{
		body->setDeactivationTime(btVal);
		return;
	}
	if (strcmp(parm, "linearsleepingthreshold") == 0)
	{
		body->setSleepingThresholds(btVal, m_params->angularSleepingThreshold);
	}
	if (strcmp(parm, "angularsleepingthreshold") == 0)
	{
		body->setSleepingThresholds(m_params->linearSleepingThreshold, btVal);
	}
	if (strcmp(parm, "ccdmotionthreshold") == 0)
	{
		body->setCcdMotionThreshold(btVal);
	}
	if (strcmp(parm, "ccdsweptsphereradius") == 0)
	{
		body->setCcdSweptSphereRadius(btVal);
	}
	if (strcmp(parm, "avatarfriction") == 0)
	{
		body->setFriction(btVal);
	}
	if (strcmp(parm, "avatarmass") == 0)
	{
		body->setMassProps(btVal, btVector3(0, 0, 0));
	}
	if (strcmp(parm, "avatarrestitution") == 0)
	{
		body->setRestitution(btVal);
	}
	if (strcmp(parm, "avatarcapsuleradius") == 0)
	{
		// can't change this without rebuilding the collision shape
		// TODO: rebuild the capsule (remember to take scale into account)
	}
	if (strcmp(parm, "avatarcapsuleheight") == 0)
	{
		// can't change this without rebuilding the collision shape
		// TODO: rebuild the capsule (remember to take scale into account)
	}

	return;
}

// #include "LinearMath/btQuickprof.h"
void BulletSim::DumpPhysicsStats()
{
	// call Bullet to dump its performance stats
	// CProfileManager::dumpAll();
	return;
}



