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

// There are several different types of constraints. This is the one we're using.
#define BTCONSTRAINTTYPE btGeneric6DofConstraint

#pragma warning( disable: 4190 ) // Warning about returning Vector3 that we can safely ignore

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
 * Update the simulator terrain.
 * @param worldID ID of the world to modify.
 * @param heightmap Array of terrain heights the width and depth (X and Y of maxPosition) of the simulator.
 */
EXTERN_C DLL_EXPORT void SetHeightmap2(BulletSim* sim, float* heightmap)
{
	sim->SetHeightmap(heightmap);
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

/*
EXTERN_C DLL_EXPORT btCollisionShape* CreateMesh2(BulletSim* sim, 
									int indicesCount, int* indices, int verticesCount, float* vertices )
{
	return sim->CreateMesh2(indicesCount, indices, verticesCount, vertices);
}

EXTERN_C DLL_EXPORT bool BuildHull2(BulletSim* sim, btCollisionShape* mesh) {
	return mesh->BuildHull(sim);
}

EXTERN_C DLL_EXPORT bool ReleaseHull2(BulletSim* sim, btCollisionShape* mesh) {
	return mesh->ReleaseHull(sim);
}
EXTERN_C DLL_EXPORT bool DestroyMesh2(BulletSim* sim, btCollisionShape* mesh)
{
	return sim->DestroyMesh2(mesh);
}
EXTERN_C DLL_EXPORT IPhysObject* CreateObject2(BulletSim* sim, ShapeData shapeData)
{
	return sim->CreateObject2(&shapeData);
}
*/

/**
 * Add a generic 6 degree of freedom constraint between two previously created objects
 * @param worldID ID of the world to modify.
 * @param id1 first object to constrain
 * @param id2 other object to constrain
 * @param lowLinear low bounds of linear constraint
 * @param hiLinear hi bounds of linear constraint
 * @param lowAngular low bounds of angular constraint
 * @param hiAngular hi bounds of angular constraint
 */
EXTERN_C DLL_EXPORT BTCONSTRAINTTYPE* CreateConstraint2(BulletSim* sim, btCollisionObject* obj1, btCollisionObject* obj2,
	Vector3 frame1loc, Quaternion frame1rot,
	Vector3 frame2loc, Quaternion frame2rot,
	Vector3 lowLinear, Vector3 hiLinear, Vector3 lowAngular, Vector3 hiAngular)
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

	BTCONSTRAINTTYPE* constrain = NULL;
	if (rb1 != NULL && rb2 != NULL)
	{
		constrain = new BTCONSTRAINTTYPE(*rb1, *rb2, frame1t, frame1t, true);
		constrain->setLinearLowerLimit(lowLinear.GetBtVector3());
		constrain->setLinearUpperLimit(hiLinear.GetBtVector3());
		constrain->setAngularLowerLimit(lowAngular.GetBtVector3());
		constrain->setAngularUpperLimit(hiAngular.GetBtVector3());

		constrain->calculateTransforms();
		sim->getDynamicsWorld()->addConstraint(constrain, false);
	}

	return constrain;
}

EXTERN_C DLL_EXPORT bool DestroyConstraint2(BulletSim* sim, BTCONSTRAINTTYPE* constrain)
{
	sim->getDynamicsWorld()->removeConstraint(constrain);
	delete constrain;
	return true;
}

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

EXTERN_C DLL_EXPORT bool SetInterpolation2(btCollisionObject* obj, Vector3 lin, Vector3 ang, Vector3 pos, Quaternion rot)
	// (sets both linear and angular interpolation velocity)
{
	btTransform transf;
	transf.setIdentity();
	transf.setOrigin(pos.GetBtVector3());
	transf.setRotation(rot.GetBtQuaternion());

	obj->setInterpolationLinearVelocity(lin.GetBtVector3());
	obj->setInterpolationAngularVelocity(ang.GetBtVector3());
	obj->setInterpolationWorldTransform(transf);

	return true;
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

EXTERN_C DLL_EXPORT bool AddObjectToWorld2(BulletSim* world, btCollisionObject* obj)
{
	world->getDynamicsWorld()->addCollisionObject(obj);
	return true;
}

EXTERN_C DLL_EXPORT bool RemoveObjectFromWorld2(BulletSim* world, btCollisionObject* obj)
{
	world->getDynamicsWorld()->removeCollisionObject(obj);
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
