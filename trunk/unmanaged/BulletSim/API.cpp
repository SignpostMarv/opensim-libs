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

static std::map<unsigned int, BulletSim*> m_simulations;

/**
 * Returns a string that identifies the version of the BulletSim.dll
 * @return static string of version information
 */
EXTERN_C DLL_EXPORT char* GetVersion()
{
	return &BulletSimVersionString[0];
}

// Return a pointer to the BulletSim instance that has this worldID
// This is here because of the reference to m_simulations
EXTERN_C DLL_EXPORT BulletSim* GetSimHandle2(unsigned int worldID)
{
	return m_simulations[worldID];
}

// Return a pointer to the Bullet object itself
// This is here because of the reference to m_simulations
EXTERN_C DLL_EXPORT btCollisionObject* GetBodyHandleWorldID2(unsigned int worldID, unsigned int localID)
{
	return m_simulations[worldID]->getWorldData()->objects->GetObject(localID)->GetBody();
}

// Return a pointer to the Bullet object itself
// This is here because of the reference to m_simulations
EXTERN_C DLL_EXPORT btCollisionObject* GetBodyHandle2(BulletSim* sim, unsigned int localID)
{
	return sim->getWorldData()->objects->GetObject(localID)->GetBody();
}

/**
 * Initializes the physical simulation.
 * @param maxPosition Top north-east corner of the simulation, with Z being up. The bottom south-west corner is 0,0,0.
 * @param maxCollisions maximum number of collisions that can be reported each tick
 * @param updateArray pointer to pinned memory to return the collision info
 * @param maxUpdates maximum number of property updates that can be reported each tick
 * @param maxCollisions pointer to pinned memory to return the update information
 * @return worldID for the newly created simulation.
 */
EXTERN_C DLL_EXPORT unsigned int Initialize(Vector3 maxPosition, ParamBlock* parms,
											int maxCollisions, CollisionDesc* collisionArray,
											int maxUpdates, EntityProperties* updateArray)
{
	BulletSim* sim = new BulletSim(maxPosition.X, maxPosition.Y, maxPosition.Z);
	sim->initPhysics(parms, maxCollisions, collisionArray, maxUpdates, updateArray);

	unsigned int worldID = (unsigned int)m_simulations.size();
	m_simulations[worldID] = sim;

	return worldID;
}

/**
 * Update the internal value of a parameter. Some parameters require changing world state.
 * @param worldID ID of the world to change the paramter in
 * @param localID ID of the object to change the paramter on or special values for NONE or ALL
 * @param parm the name of the parameter to change (must be passed in as lower case)
 * @param value the value to change the parameter to
 */
EXTERN_C DLL_EXPORT bool UpdateParameter(unsigned int worldID, unsigned int localID, const char* parm, float value)
{
	return m_simulations[worldID]->UpdateParameter(localID, parm, value);
}

/**
 * Update the simulator terrain.
 * @param worldID ID of the world to modify.
 * @param heightmap Array of terrain heights the width and depth (X and Y of maxPosition) of the simulator.
 */
EXTERN_C DLL_EXPORT void SetHeightmap(unsigned int worldID, float* heightmap)
{
	m_simulations[worldID]->SetHeightmap(heightmap);
}

/**
 * Shuts down the physical simulation.
 * @param worldID ID of the world to shut down.
 */
EXTERN_C DLL_EXPORT void Shutdown(unsigned int worldID)
{
	BulletSim* sim = m_simulations[worldID];
	m_simulations.erase(worldID);
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
EXTERN_C DLL_EXPORT int PhysicsStep(unsigned int worldID, float timeStep, int maxSubSteps, float fixedTimeStep, 
			int* updatedEntityCount, EntityProperties** updatedEntities, int* collidersCount, CollisionDesc** colliders)
{
	return m_simulations[worldID]->PhysicsStep(timeStep, maxSubSteps, fixedTimeStep, updatedEntityCount, updatedEntities, collidersCount, colliders);
}

/**
 * Creates a Bullet representation of a series of convex hulls describing an object.
 * The resulting hull representation is stored in the m_hulls structure
 * @param meshKey Unique identifier for this object.
 * @param hullCount Number of convex hulls that make up this object.
 * @param hulls Convex hull array.
 * @return True if the object was created, false if an object with meshKey already existed.
 */
EXTERN_C DLL_EXPORT bool CreateHull(unsigned int worldID, unsigned long long meshKey, int hullCount, float* hulls)
{
	return m_simulations[worldID]->CreateHull(meshKey, hullCount, hulls);
}

/**
 * Creates a Bullet representation of a mesh of an object.
 * The resulting mesh representation is stored in the m_meshes structure
 * @param meshKey Unique identifier for this object.
 * @param hullCount Number of convex hulls that make up this object.
 * @param hulls Convex hull array.
 * @return True if the object was created, false if an object with meshKey already existed.
 */
EXTERN_C DLL_EXPORT bool CreateMesh(unsigned int worldID, unsigned long long meshKey, 
									int indicesCount, int* indices, int verticesCount, float* vertices )
{
	return m_simulations[worldID]->CreateMesh(meshKey, indicesCount, indices, verticesCount, vertices);
}

/**
 * Deletes the bullet representation of a hull
 * @param meshKey Unique identifier for this object.
 * @return True if the object was deleted, false if we didn't have the hull
 */
EXTERN_C DLL_EXPORT bool DestroyHull(unsigned int worldID, unsigned long long hullKey)
{
	return m_simulations[worldID]->DestroyHull(hullKey);
}

/**
 * Deletes the bullet representation of a mesh
 * @param meshKey Unique identifier for this object.
 * @return True if the object was deleted, false if we didn't have the mesh
 */
EXTERN_C DLL_EXPORT bool DestroyMesh(unsigned int worldID, unsigned long long meshKey)
{
	return m_simulations[worldID]->DestroyMesh(meshKey);
}

/**
 * Creates a single object in the simulation. This can either be a character or a rigid body.
 * @param worldID ID of the world to modify.
 * @param shapeData Structure describing the object to start simulating.
 * @return True if the object was created or replaced, false if the object could not be created.
 */
EXTERN_C DLL_EXPORT bool CreateObject(unsigned int worldID, ShapeData shapeData)
{
	return m_simulations[worldID]->CreateObject(&shapeData);
}

/**
 * Get the position of a character or rigid body.
 * @param worldID ID of the world to access.
 * @param id Object ID.
 * @return Position of the object if found, otherwise Vector3.Zero
 */
EXTERN_C DLL_EXPORT Vector3 GetObjectPosition(unsigned int worldID, unsigned int id)
{
	btVector3 v = m_simulations[worldID]->GetObjectPosition(id);
	return Vector3(v.getX(), v.getY(), v.getZ());
}

/**
 * Get the orientation of a character or rigid body.
 * @param worldID ID of the world to access.
 * @param id Object ID.
 * @return Position of the object if found, otherwise Vector3.Zero
 */
EXTERN_C DLL_EXPORT Quaternion GetObjectOrientation(unsigned int worldID, unsigned int id)
{
	btQuaternion v = m_simulations[worldID]->GetObjectOrientation(id);
	return Quaternion(v.getX(), v.getY(), v.getZ(), v.getW());
}

/**
 * Set the position and rotation of a character or rigid body.
 * @param worldID ID of the world to modify.
 * @param id Object ID.
 * @param position New position.
 * @param rotation New rotation.
 * @return True on success, false if the object was not found.
 */
EXTERN_C DLL_EXPORT bool SetObjectTranslation(unsigned int worldID, unsigned int id, Vector3 position, Quaternion rotation)
{
	btVector3 pos = position.GetBtVector3();
	btQuaternion rot = rotation.GetBtQuaternion();

	return m_simulations[worldID]->SetObjectTranslation(id, pos, rot);
}

/**
 * Set the velocity of a rigid body.
 * @param worldID ID of the world to modify.
 * @param id Object ID.
 * @param velocity New velocity.
 * @return True on success, false if the rigid body was not found.
 */
EXTERN_C DLL_EXPORT bool SetObjectVelocity(unsigned int worldID, unsigned int id, Vector3 velocity)
{
	btVector3 v = velocity.GetBtVector3();
	return m_simulations[worldID]->SetObjectVelocity(id, v);
}

/**
 * Set the angular velocity of a rigid body.
 * @param worldID ID of the world to modify.
 * @param id Object ID.
 * @param angularVelocity New angular velocity.
 * @return True on success, false if the rigid body was not found.
 */
EXTERN_C DLL_EXPORT bool SetObjectAngularVelocity(unsigned int worldID, unsigned int id, Vector3 angularVelocity)
{
	btVector3 v = angularVelocity.GetBtVector3();
	return m_simulations[worldID]->SetObjectAngularVelocity(id, v);
}

/**
 * Apply a force to a rigid body.
 * @param worldID ID of the world to modify.
 * @param id Object ID.
 * @param force Force to apply.
 * @return True on success, false if the rigid body was not found.
 */
EXTERN_C DLL_EXPORT bool SetObjectForce(unsigned int worldID, unsigned int id, Vector3 force)
{
	btVector3 v = force.GetBtVector3();
	return m_simulations[worldID]->SetObjectForce(id, v);
}

/**
 * Set the scale and mass of a rigid body.
 * @param worldID ID of the world to modify.
 * @param id Object ID.
 * @param scale New scale.
 * @param mass New mass, in kilograms. A mass of 0.0 will make the object static.
 * @return True on success, false if the rigid body was not found.
 */
EXTERN_C DLL_EXPORT bool SetObjectScaleMass(unsigned int worldID, unsigned int id, Vector3 scale, float mass, bool isPhysical)
{
	btVector3 s = scale.GetBtVector3();
	return m_simulations[worldID]->SetObjectScaleMass(id, s, mass, isPhysical);
}

/**
 * Toggle collisions on and off for a rigid body.
 * @param worldID ID of the world to modify.
 * @param id Object ID.
 * @param collidable True to disable collisions, false to enable collisions.
 * @return True on success, false if the rigid body was not found.
 */
EXTERN_C DLL_EXPORT bool SetObjectCollidable(unsigned int worldID, unsigned int id, bool collidable)
{
	return m_simulations[worldID]->SetObjectCollidable(id, collidable);
}

/**
 * Set the rigid body to respond to physics or not
 * @param worldID ID of the world to modify.
 * @param id Object ID.
 * @param phantom True to disable collisions, false to enable collisions.
 * @return True on success, false if the rigid body was not found.
 */
EXTERN_C DLL_EXPORT bool SetObjectDynamic(unsigned int worldID, unsigned int id, bool isPhysical, float mass)
{
	return m_simulations[worldID]->SetObjectDynamic(id, isPhysical, mass);
}

/**
 * Set the physical and interactive properties of the object
 * @param worldID ID of the world to modify
 * @param id Object ID.
 * @param isStatic true of the object is static and does not move. False if gravity, etc has an effect
 * @param isSolid true if other objects cannot pass through this object
 * @param genCollisions true if this object should generate collisions with other objects
 * @param mass the mass of the object (needed if it's not static)
 */
EXTERN_C DLL_EXPORT bool SetObjectProperties(unsigned int worldID, unsigned int id, bool isStatic, bool isSolid, bool genCollisions, float mass)
{
	return m_simulations[worldID]->SetObjectProperties(id, isStatic, isSolid, genCollisions, mass);
}

/**
 * Set the buoyancy of an object (set the degree that gravity affects the object)
 * @param worldID ID of the world to modify.
 * @param id Object ID.
 * @param gravity effect (1.0 = normal gravity, 0.0 = no gravity)
 * @return True on success, false if the rigid body was not found.
 */
EXTERN_C DLL_EXPORT bool SetObjectBuoyancy(unsigned int worldID, unsigned int id, float buoyancy)
{
	return m_simulations[worldID]->SetObjectBuoyancy(id, buoyancy);
}

/**
 * Searches for a character or rigid body with the given id.
 * @param worldID ID of the world to access.
 * @param id Object ID.
 * @return True if a character or rigid body was found, otherwise false.
 */
EXTERN_C DLL_EXPORT bool HasObject(unsigned int worldID, unsigned int id)
{
	return m_simulations[worldID]->HasObject(id);
}

/**
 * Stop simulation for a character or rigid body and free all memory allocated by it.
 * @param worldID ID of the world to modify.
 * @param id Object ID.
 * @return True on success, false if the object was not found.
 */
EXTERN_C DLL_EXPORT bool DestroyObject(unsigned int worldID, unsigned int id)
{
	return m_simulations[worldID]->DestroyObject(id);
}

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
EXTERN_C DLL_EXPORT SweepHit ConvexSweepTest(unsigned int worldID, unsigned int id, Vector3 from, Vector3 to, float extraMargin)
{
	btVector3 f = from.GetBtVector3();
	btVector3 t = to.GetBtVector3();
	return m_simulations[worldID]->ConvexSweepTest(id, f, t, extraMargin);
}

/**
 * Perform a raycast test by drawing a line from a and testing for collisions.
 * @param worldID ID of the world to access.
 * @param id Object ID to ignore during the raycast.
 * @param from Start of the ray.
 * @param to End of the ray.
 * @return Raycast results. If there were no collisions, RaycastHit.ID will be ID_INVALID_HIT (0xFFFFFFFF)
 */
EXTERN_C DLL_EXPORT RaycastHit RayTest(unsigned int worldID, unsigned int id, Vector3 from, Vector3 to)
{
	btVector3 f = from.GetBtVector3();
	btVector3 t = to.GetBtVector3();
	return m_simulations[worldID]->RayTest(id, f, t);
}

/**
 * Returns the position offset required to bring a character out of a penetrating collision.
 * @param worldID ID of the world to access.
 * @param id Character ID.
 * @return A position offset to apply to the character to resolve a penetration.
 */
EXTERN_C DLL_EXPORT Vector3 RecoverFromPenetration(unsigned int worldID, unsigned int id)
{
	btVector3 v = m_simulations[worldID]->RecoverFromPenetration(id);
	return Vector3(v.getX(), v.getY(), v.getZ());
}

/**
 * Causes the detailed physics performance statistics to be logged.
 */
EXTERN_C DLL_EXPORT void DumpPhysicsStatistics(unsigned int worldID)
{
	if (debugLogCallback == NULL) return;
	m_simulations[worldID]->DumpPhysicsStats();
	return;
}

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
