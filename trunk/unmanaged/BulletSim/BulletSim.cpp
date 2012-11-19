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
#include "Util.h"

#include "BulletCollision/CollisionDispatch/btSimulationIslandManager.h"

extern "C" void DumpPhysicsStatistics2(BulletSim* sim);

BulletSim::BulletSim(btScalar maxX, btScalar maxY, btScalar maxZ)
{
	bsDebug_Initialize();

	// Make sure structures that will be created in initPhysics are marked as not created
	m_worldData.dynamicsWorld = NULL;

	m_worldData.sim = this;

	m_worldData.MinPosition = btVector3(0, 0, 0);
	m_worldData.MaxPosition = btVector3(maxX, maxY, maxZ);
}

void BulletSim::initPhysics2(ParamBlock* parms, 
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

	// create the functional parts of the physics simulation
	btDefaultCollisionConstructionInfo cci;
	// if you are setting a pool size, you should disable dynamic allocation
	if (m_worldData.params->maxPersistantManifoldPoolSize > 0)
		cci.m_defaultMaxPersistentManifoldPoolSize = (int)m_worldData.params->maxPersistantManifoldPoolSize;
	if (m_worldData.params->shouldDisableContactPoolDynamicAllocation != ParamFalse)
		m_dispatcher->setDispatcherFlags(btCollisionDispatcher::CD_DISABLE_CONTACTPOOL_DYNAMIC_ALLOCATION);
	if (m_worldData.params->maxCollisionAlgorithmPoolSize > 0)
		cci.m_defaultMaxCollisionAlgorithmPoolSize = m_worldData.params->maxCollisionAlgorithmPoolSize;
	
	m_collisionConfiguration = new btDefaultCollisionConfiguration(cci);
	m_dispatcher = new btCollisionDispatcher(m_collisionConfiguration);

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

	// Increasing solver interations can increase stability.
	if (m_worldData.params->numberOfSolverIterations > 0)
		m_worldData.dynamicsWorld->getSolverInfo().m_numIterations = (int)m_worldData.params->numberOfSolverIterations;

	// Earth-like gravity
	dynamicsWorld->setGravity(btVector3(0.f, 0.f, m_worldData.params->gravity));

	m_dumpStatsCount = 0;
	if (m_worldData.debugLogCallback != NULL)
	{
		m_dumpStatsCount = (int)m_worldData.params->physicsLoggingFrames;
		if (m_dumpStatsCount != 0)
			m_worldData.BSLog("Logging detailed physics stats every %d frames", m_dumpStatsCount);
	}

	// Information on creating a custom collision computation routine and a pointer to the computation
	// of friction and restitution at:
	// http://bulletphysics.org/Bullet/phpBB3/viewtopic.php?f=9&t=7922

	// foreach body that you want the callback, enable it with:
	// body->setCollisionFlags(body->getCollisionFlags() | btCollisionObject::CF_CUSTOM_MATERIAL_CALLBACK);

}

void BulletSim::exitPhysics2()
{
	if (m_worldData.dynamicsWorld == NULL)
		return;

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
int BulletSim::PhysicsStep2(btScalar timeStep, int maxSubSteps, btScalar fixedTimeStep, 
						   int* updatedEntityCount, EntityProperties** updatedEntities, 
						   int* collidersCount, CollisionDesc** colliders)
{
	int numSimSteps = 0;

	if (m_worldData.dynamicsWorld)
	{
		// The simulation calls the SimMotionState to put object updates into updatesThisFrame.
		numSimSteps = m_worldData.dynamicsWorld->stepSimulation(timeStep, maxSubSteps, fixedTimeStep);

		if (m_dumpStatsCount != 0)
		{
			if (--m_dumpStatsCount <= 0)
			{
				m_dumpStatsCount = (int)m_worldData.params->physicsLoggingFrames;
				DumpPhysicsStatistics2(this);
			}
		}

		// OBJECT UPDATES =================================================================
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

		// COLLISIONS =================================================================
		// Put all of the colliders this frame into m_collidersThisFrameArray
		m_collidersThisFrame.clear();
		m_collisionsThisFrame = 0;
		int numManifolds = m_worldData.dynamicsWorld->getDispatcher()->getNumManifolds();
		for (int j = 0; j < numManifolds; j++)
		{
			btPersistentManifold* contactManifold = m_worldData.dynamicsWorld->getDispatcher()->getManifoldByIndexInternal(j);
			int numContacts = contactManifold->getNumContacts();
			if (numContacts == 0)
				continue;

			const btCollisionObject* objA = static_cast<const btCollisionObject*>(contactManifold->getBody0());
			const btCollisionObject* objB = static_cast<const btCollisionObject*>(contactManifold->getBody1());

			// DEBUG BEGIN
			// IDTYPE idAx = CONVLOCALID(objA->getCollisionShape()->getUserPointer());
			// IDTYPE idBx = CONVLOCALID(objB->getCollisionShape()->getUserPointer());
			// m_worldData.BSLog("Collision: A=%u/%x, B=%u/%x", idAx, objA->getCollisionFlags(), idBx, objB->getCollisionFlags());
			// DEBUG END

			// When two objects collide, we only report one contact point
			const btManifoldPoint& manifoldPoint = contactManifold->getContactPoint(0);
			const btVector3& contactPoint = manifoldPoint.getPositionWorldOnB();
			btVector3 contactNormal = -manifoldPoint.m_normalWorldOnB;	// make relative to A

			RecordCollision(objA, objB, contactPoint, contactNormal);

			if (m_collisionsThisFrame >= m_maxCollisionsPerFrame) 
				break;
		}

		// Any ghost objects must be relieved of their collisions.
		WorldData::SpecialCollisionObjectMapType::iterator it = m_worldData.specialCollisionObjects.begin();
		for (; it != m_worldData.specialCollisionObjects.end(); it++)
		{
			btCollisionObject* collObj = it->second;
			btPairCachingGhostObject* obj = (btPairCachingGhostObject*)btGhostObject::upcast(collObj);
			if (obj)
			{
				RecordGhostCollisions(obj);
			}

			if (m_collisionsThisFrame >= m_maxCollisionsPerFrame) 
				break;
		}


		*collidersCount = m_collisionsThisFrame;
		*colliders = m_collidersThisFrameArray;
	}

	return numSimSteps;
}

void BulletSim::RecordCollision(const btCollisionObject* objA, const btCollisionObject* objB, const btVector3& contact, const btVector3& norm)
{
	btVector3 contactNormal = norm;

	// One of the objects has to want to hear about collisions
	if ((objA->getCollisionFlags() & BS_SUBSCRIBE_COLLISION_EVENTS) == 0
						&& (objB->getCollisionFlags() & BS_SUBSCRIBE_COLLISION_EVENTS) == 0)
	{
		return;
	}

	// Get the IDs of colliding objects (stored in the one user definable field)
	IDTYPE idA = CONVLOCALID(objA->getUserPointer());
	IDTYPE idB = CONVLOCALID(objB->getUserPointer());

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
	//    How many duplicate manifolds are there?
	// Also, using obj->getCollisionFlags() we can pass up only the collisions
	//    for one object if it's the only one requesting. Wouldn't have to do
	//    the "Collide(a,b);Collide(b,a)" in BSScene.
	COLLIDERKEYTYPE collisionID = ((COLLIDERKEYTYPE)idA << 32) | idB;

	// If this collision has not been seen yet, record it
	if (m_collidersThisFrame.find(collisionID) == m_collidersThisFrame.end())
	{
		m_collidersThisFrame.insert(collisionID);

		CollisionDesc cDesc;
		cDesc.aID = idA;
		cDesc.bID = idB;
		cDesc.point = contact;
		cDesc.normal = contactNormal;
		m_collidersThisFrameArray[m_collisionsThisFrame] = cDesc;
		m_collisionsThisFrame++;
	}
}

void BulletSim::RecordGhostCollisions(btPairCachingGhostObject* obj)
{
	btManifoldArray   manifoldArray;
	btBroadphasePairArray& pairArray = obj->getOverlappingPairCache()->getOverlappingPairArray();
	int numPairs = pairArray.size();

	// For all the pairs of sets of contact points
	for (int i=0; i < numPairs; i++)
	{
		if (m_collisionsThisFrame >= m_maxCollisionsPerFrame) 
			break;

		manifoldArray.clear();
		const btBroadphasePair& pair = pairArray[i];

		// The real representation is over in the world pair cache
		btBroadphasePair* collisionPair = m_worldData.dynamicsWorld->getPairCache()->findPair(pair.m_pProxy0,pair.m_pProxy1);
		if (!collisionPair)
			continue;

		if (collisionPair->m_algorithm)
			collisionPair->m_algorithm->getAllContactManifolds(manifoldArray);

		// The collision pair has sets of collision points (manifolds)
		for (int j=0; j < manifoldArray.size(); j++)
		{
			btPersistentManifold* contactManifold = manifoldArray[j];
			int numContacts = contactManifold->getNumContacts();

			const btCollisionObject* objA = static_cast<const btCollisionObject*>(contactManifold->getBody0());
			const btCollisionObject* objB = static_cast<const btCollisionObject*>(contactManifold->getBody1());

			// TODO: this is a more thurough check than the regular collision code --
			//     here we find the penetrating contact in the manifold but for regular
			//     collisions we assume the first point in the manifold is good enough.
			//     Decide of this extra checking is required or if first point is good enough.
			for (int p=0; p < numContacts; p++)
			{
				const btManifoldPoint& pt = contactManifold->getContactPoint(p);
				// If a penetrating contact, this is a hit
				if (pt.getDistance()<0.f)
				{
					const btVector3& contactPoint = pt.getPositionWorldOnA();
					const btVector3& normalOnA = -pt.m_normalWorldOnB;
					RecordCollision(objA, objB, contactPoint, normalOnA);
					// Only one contact point for each set of colliding objects
					break;
				}
			}
		}
	}
}

// If using Bullet' convex hull code, refer to following link for parameter setting
// http://kmamou.blogspot.com/2011/11/hacd-parameters.html
// Another useful reference for ConvexDecomp
// http://www.bulletphysics.org/Bullet/phpBB3/viewtopic.php?t=7159

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
btCollisionShape* BulletSim::BuildHullShapeFromMesh2(btCollisionShape* mesh)
{
	// TODO: write the code to use the Bullet HACD routine
	return NULL;
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

bool BulletSim::UpdateParameter2(IDTYPE localID, const char* parm, float val)
{
	btScalar btVal = btScalar(val);
	btVector3 btZeroVector3 = btVector3(0, 0, 0);

	// changes to the environment
	if (strcmp(parm, "gravity") == 0)
	{
		m_worldData.dynamicsWorld->setGravity(btVector3(0.f, 0.f, val));
		return true;
	}
	return false;
}

// #include "LinearMath/btQuickprof.h"
// Call Bullet to dump its performance stats
// Bullet must be patched to make this work. See BulletDetailLogging.patch
void BulletSim::DumpPhysicsStats()
{
	// CProfileManager::dumpAll();
	return;
}