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

	// BSLog("AvatarObject::AvatarObject: Creating an avatar");
	m_worldData = world;
	m_id = data->ID;
	m_currentFriction = 0.0;

	// Unpack ShapeData
	btVector3 position = data->Position.GetBtVector3();
	btQuaternion rotation = data->Rotation.GetBtQuaternion();
	btVector3 scale = data->Scale.GetBtVector3();
	btVector3 velocity = data->Velocity.GetBtVector3();
	btScalar maxScale = scale.m_floats[scale.maxAxis()];
	btScalar mass = btScalar(data->Mass);

	// btScalar friction = btScalar(data->Friction);
	// btScalar restitution = btScalar(data->Restitution);
	// force friction and bouncyness to zero. 
	// Movement will be managed by this module.
	btScalar friction = btScalar(m_currentFriction);
	btScalar restitution = btScalar(0);

	// bool isStatic = (data->Static == 1);
	// bool isCollidable = (data->Collidable == 1);
	bool isStatic = false;
	bool isCollidable = true;

	// Create the default capsule for the avatar
	btCollisionShape* shape = new btCapsuleShapeZ(m_worldData->params->avatarCapsuleRadius,
												m_worldData->params->avatarCapsuleHeight);
	shape->setMargin(m_worldData->params->collisionMargin);
	// TODO: adjust capsule size for the height of the avatar

	// Save the ID for this shape in the user settable variable (used to know what is colliding)
	shape->setUserPointer(PACKLOCALID(m_id));
	
	// Create a starting transform
	btTransform startTransform;
	startTransform.setIdentity();
	startTransform.setOrigin(position);
	startTransform.setRotation(rotation);

	// Avatars are created as rigid objects so they collide and have gravity

	// Inertia calculation for physical objects (non-zero mass)
	// TODO: compute avatar mass based on the size of the capsule (change with scaling)
	btVector3 localInertia(0, 0, 0);
	shape->calculateLocalInertia(mass, localInertia);

	// Create the motion state and rigid body
	SimMotionState* motionState = new SimMotionState(m_id, startTransform, &(m_worldData->updatesThisFrame));
	btRigidBody::btRigidBodyConstructionInfo cInfo(mass, motionState, shape, localInertia);
	m_body = new btRigidBody(cInfo);
	motionState->RigidBody = m_body;

	// Characters can have special collision operations.
	m_body->setCollisionFlags(m_body->getCollisionFlags() | btCollisionObject::CF_CHARACTER_OBJECT);
	// The following makes it so the avatar doesn't respond to things collided with.

	UpdatePhysicalParameters(friction, restitution, velocity);

	m_worldData->dynamicsWorld->addRigidBody(m_body);

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

bool AvatarObject::SetObjectProperties(const bool isStatic, const bool isCollidable, const bool genCollisions, const float mass) {
	// This is a NOP for an avatar
	return true;
}

// Note that this does not really set the friction for the avatar
void AvatarObject::UpdatePhysicalParameters(btScalar frict, btScalar resti, const btVector3& velo)
{
	// Tweak continuous collision detection parameters
	// Only perform continuious collision detection (CCD) if movement last frame was more than threshold
	if (m_worldData->params->ccdMotionThreshold > 0.0f)
	{
		m_body->setCcdMotionThreshold(btScalar(m_worldData->params->ccdMotionThreshold));
		m_body->setCcdSweptSphereRadius(btScalar(m_worldData->params->ccdSweptSphereRadius));
	}

	m_body->setFriction(m_currentFriction);
	m_body->setRestitution(resti);
	m_body->setActivationState(DISABLE_DEACTIVATION);
	m_body->setContactProcessingThreshold(m_worldData->params->avatarContactProcessingThreshold);

	m_body->setAngularFactor(btVector3(0, 0, 0));	// makes the capsule not fall over
	m_body->setLinearVelocity(velo);
	m_body->setInterpolationLinearVelocity(btVector3(0, 0, 0));	// turns off unexpected interpolation
	m_body->setInterpolationAngularVelocity(btVector3(0, 0, 0));
	m_body->setInterpolationWorldTransform(m_body->getWorldTransform());
}

btVector3 AvatarObject::GetObjectPosition()
{
	btTransform xform = m_body->getWorldTransform();
	btVector3 pos = xform.getOrigin();
	// BSLog("AvatarObject::GetObjectPosition: pos=<%f,%f,%f>", pos.getX(), pos.getY(), pos.getZ() );
	return pos;
}

bool AvatarObject::SetObjectTranslation(btVector3& position, btQuaternion& rotation)
{
	/* BSLog("AvatarObject::SetObjectTranslation: pos=<%f,%f,%f>, rot=<%f,%f,%f,%f>", 
			position.getX(), position.getY(), position.getZ(), 
			rotation.getW(), rotation.getX(), rotation.getY(), rotation.getZ() );
			*/
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
	// BSLog("AvatarObject::SetObjectVelocity: vel=<%f,%f,%f>", velocity.getX(), velocity.getY(), velocity.getZ() );
	// Manipulate the friction depending on whether the avatar is moving or not.
	// OpenSim moves the avatar by setting a velocity. If the avatar is
	// thus moving, it shouldn't be slowed down by whatever it's walking on.
	m_currentFriction = 0.0;
	if (velocity.length() == 0.0)
	{
		m_currentFriction = 999.0;
	}
	m_body->setFriction(btScalar(m_currentFriction));

	m_body->setLinearVelocity(velocity);
	m_body->activate(true);
	return true;
}

bool AvatarObject::SetObjectAngularVelocity(btVector3& angularVelocity)
{
	// Don't do anything for an avatar
	/* BSLog("AvatarObject::SetObjectAngularVelocity: vel=<%f,%f,%f>", 
			angularVelocity.getX(), angularVelocity.getY(), angularVelocity.getZ() );
			*/
	return true;
}

bool AvatarObject::SetObjectForce(btVector3& force)
{
	// BSLog("AvatarObject::SetObjectForce: force=<%f,%f,%f>", force.getX(), force.getY(), force.getZ() );
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
	// BSLog("AvatarObject::SetObjectBuoyancy: buoy=%f, grav=%f", buoy, grav);
	m_body->setGravity(btVector3(0, 0, grav));
	m_body->activate(true);

	return true;
}

bool AvatarObject::UpdateParameter(const char* parm, const float val)
{
	btScalar btVal = btScalar(val);

	if (strcmp(parm, "friction") == 0 || strcmp(parm, "avatarfriction") == 0)
	{
		m_body->setFriction(btVal);
		return true;
	}
	if (strcmp(parm, "restitution") == 0 || strcmp(parm, "avatarrestitution") == 0)
	{
		m_body->setRestitution(btVal);
		return true;
	}
	if (strcmp(parm, "lineardamping") == 0)
	{
		m_body->setDamping(btVal, m_worldData->params->linearDamping);
		return true;
	}
	if (strcmp(parm, "angulardamping") == 0)
	{
		m_body->setDamping(m_worldData->params->angularDamping, btVal);
		return true;
	}
	if (strcmp(parm, "deactivationtime") == 0)
	{
		m_body->setDeactivationTime(btVal);
		return true;
	}
	if (strcmp(parm, "linearsleepingthreshold") == 0)
	{
		m_body->setSleepingThresholds(btVal, m_worldData->params->angularSleepingThreshold);
		return true;
	}
	if (strcmp(parm, "angularsleepingthreshold") == 0)
	{
		m_body->setSleepingThresholds(m_worldData->params->linearSleepingThreshold, btVal);
		return true;
	}
	if (strcmp(parm, "ccdmotionthreshold") == 0)
	{
		m_body->setCcdMotionThreshold(btVal);
		return true;
	}
	if (strcmp(parm, "ccdsweptsphereradius") == 0)
	{
		m_body->setCcdSweptSphereRadius(btVal);
		return true;
	}
	if (strcmp(parm, "avatarmass") == 0)
	{
		m_body->setMassProps(btVal, btVector3(0, 0, 0));
		return true;
	}
	if (strcmp(parm, "avatarcapsuleradius") == 0)
	{
		// can't change this without rebuilding the collision shape
		// TODO: rebuild the capsule (remember to take scale into account)
		return false;
	}
	if (strcmp(parm, "avatarcapsuleheight") == 0)
	{
		// can't change this without rebuilding the collision shape
		// TODO: rebuild the capsule (remember to take scale into account)
		return false;
	}
	return false;
}