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
#include "AvatarObject.h"
#include "btBulletDynamicsCommon.h"
#include "BulletSim.h"

AvatarObject::AvatarObject(WorldData* world, ShapeData* data) {

	m_worldData = world;
	m_id = data->ID;

	btCollisionShape* shape = NULL;

	// Unpack ShapeData
	IDTYPE id = data->ID;
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

	// Building an avatar
	// Avatars are created as rigid objects so they collide and have gravity

	// Inertia calculation for physical objects (non-zero mass)
	btVector3 localInertia(0, 0, 0);
	if (mass != 0.0f)
		shape->calculateLocalInertia(mass, localInertia);

	// Create the motion state and rigid body
	SimMotionState* motionState = new SimMotionState(data->ID, startTransform, &(m_worldData->updatesThisFrame));
	btRigidBody::btRigidBodyConstructionInfo cInfo(mass, motionState, shape, localInertia);
	btRigidBody* character = new btRigidBody(cInfo);
	motionState->RigidBody = character;

	character->setCollisionFlags(character->getCollisionFlags() | btCollisionObject::CF_CHARACTER_OBJECT);

	this->SetPhysicalProperties(friction, restitution, velocity);

	m_body = character;
	m_worldData->dynamicsWorld->addRigidBody(character);

	/*
	// NOTE: Old code kept for reference
	// Building a kinematic character controller
	btPairCachingGhostObject* character = new btPairCachingGhostObject();
	character->setWorldTransform(startTransform);
	character->setCollisionShape(shape);
	character->setCollisionFlags(btCollisionObject::CF_CHARACTER_OBJECT | btCollisionObject::CF_NO_CONTACT_RESPONSE);
	character->setActivationState(DISABLE_DEACTIVATION);
	character->setContactProcessingThreshold(0.0);

	m_worldData->dynamicsWorld->addCollisionObject(character, btBroadphaseProxy::CharacterFilter);
	m_characters[id] = character;
	*/
}

AvatarObject::~AvatarObject(void) {
	if (m_body)
	{
		btCollisionShape* shape = m_body->getCollisionShape();

		// Remove the object from the world
		m_worldData->dynamicsWorld->removeCollisionObject(m_body);

		// If we added a motionState to the object, delete that
		btMotionState* motionState = m_body->getMotionState();
		if (motionState)
			delete motionState;
		
		// Delete the rest of the memory allocated to this object
		if (shape) 
			delete shape;
		delete m_body;

		m_body = NULL;
	}
}

bool AvatarObject::SetProperties(const bool isStatic, const bool isCollidable, const bool genCollisions, const float mass) {
	return false;
}

bool AvatarObject::SetPhysicalProperties(const btScalar friction, const btScalar restitution, const btVector3& velocity) {
	return false;
}

btVector3 AvatarObject::GetObjectPosition()
{
	btTransform xform = m_body->getWorldTransform();
	return xform.getOrigin();
}

bool AvatarObject::SetObjectTranslation(btVector3& position, btQuaternion& rotation)
{
	// Build a transform containing the new position and rotation
	btTransform transform;
	transform.setIdentity();
	transform.setOrigin(position);
	transform.setRotation(rotation);

	// Set the new transform for this character controller
	m_body->setWorldTransform(transform);
	return true;
}

bool AvatarObject::SetObjectVelocity(btVector3& velocity)
{
	m_body->setLinearVelocity(velocity);
	m_body->activate(true);
	return true;
}

bool AvatarObject::SetObjectAngularVelocity(btVector3& angularVelocity)
{
	// Don't do anything for an avatar
	return true;
}

bool AvatarObject::SetObjectForce(btVector3& force)
{
	// Don't do anything for an avatar
	return true;
}

bool AvatarObject::SetObjectScaleMass(btVector3& scale, float mass, bool isDynamic)
{
	// BSLog("AvatarObject::SetObjectScaleMass: mass=%f", mass);
	btCollisionShape* shape = m_body->getCollisionShape();
	shape->setLocalScaling(scale);

	return true;
}

bool AvatarObject::SetObjectCollidable(bool collidable)
{
	// do nothing for an avatar (they don't go phantom)
	return true;
}

// Adjust how gravity effects the object
// neg=fall quickly, 0=1g, 1=0g, pos=float up
bool AvatarObject::SetObjectBuoyancy(float buoy)
{
	float grav = m_worldData->params->gravity * (1.0f - buoy);
	m_body->setGravity(btVector3(0, 0, grav));
	m_body->activate(true);

	return true;
}

void AvatarObject::UpdateParameter(const char* parm, const float val)
{
	btScalar btVal = btScalar(val);

	if (strcmp(parm, "lineardamping") == 0)
	{
		m_body->setDamping(btVal, m_worldData->params->angularDamping);
		return;
	}
	if (strcmp(parm, "angulardamping") == 0)
	{
		m_body->setDamping(m_worldData->params->linearDamping, btVal);
		return;
	}
	if (strcmp(parm, "deactivationtime") == 0)
	{
		m_body->setDeactivationTime(btVal);
		return;
	}
	if (strcmp(parm, "linearsleepingthreshold") == 0)
	{
		m_body->setSleepingThresholds(btVal, m_worldData->params->angularSleepingThreshold);
	}
	if (strcmp(parm, "angularsleepingthreshold") == 0)
	{
		m_body->setSleepingThresholds(m_worldData->params->linearSleepingThreshold, btVal);
	}
	if (strcmp(parm, "ccdmotionthreshold") == 0)
	{
		m_body->setCcdMotionThreshold(btVal);
	}
	if (strcmp(parm, "ccdsweptsphereradius") == 0)
	{
		m_body->setCcdSweptSphereRadius(btVal);
	}
	if (strcmp(parm, "avatarfriction") == 0)
	{
		m_body->setFriction(btVal);
	}
	if (strcmp(parm, "avatarmass") == 0)
	{
		m_body->setMassProps(btVal, btVector3(0, 0, 0));
	}
	if (strcmp(parm, "avatarrestitution") == 0)
	{
		m_body->setRestitution(btVal);
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
void AvatarObject::UpdatePhysicalParameters(btScalar frict, btScalar resti, const btVector3& velo)
{
	// Tweak continuous collision detection parameters
	// Only perform continuious collision detection (CCD) if movement last frame was more than threshold
	if (m_worldData->params->ccdMotionThreshold > 0.0f)
	{
		m_body->setCcdMotionThreshold(btScalar(m_worldData->params->ccdMotionThreshold));
		m_body->setCcdSweptSphereRadius(btScalar(m_worldData->params->ccdSweptSphereRadius));
	}

	m_body->setFriction(frict);
	m_body->setRestitution(resti);
	m_body->setActivationState(DISABLE_DEACTIVATION);
	m_body->setContactProcessingThreshold(0.0);

	m_body->setAngularFactor(btVector3(0, 0, 0));	// makes the capsule not fall over
	m_body->setLinearVelocity(velo);
	m_body->setInterpolationLinearVelocity(btVector3(0, 0, 0));	// turns off unexpected interpolation
	m_body->setInterpolationAngularVelocity(btVector3(0, 0, 0));
	m_body->setInterpolationWorldTransform(m_body->getWorldTransform());
}
