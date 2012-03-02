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

AvatarObject::AvatarObject(const ShapeData* data) {

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

AvatarObject::~AvatarObject(void) {
}

bool PrimObject::SetProperties(const bool isStatic, const bool isCollidable, const bool genCollisions, const float mass) {
	return false;
}

bool PrimObject::SetPhysicalProperties(const btScalar friction, const btScalar restitution, const btVector3& velocity) {
	return false;
}