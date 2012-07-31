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

#include "PrimObject.h"
#include "BulletSim.h"

PrimObject::PrimObject(WorldData* world, ShapeData* data) {

	m_worldData = world;
	m_id = data->ID;
	
	// TODO: correct collision shape creation
	btCollisionShape* shape = CreateShape(data);

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
	bool isStatic = (data->Static == 1.0);
	bool isCollidable = (data->Collidable == 1.0);

	// Save the ID for this shape in the user settable variable (used to know what is colliding)
	shape->setUserPointer(PACKLOCALID(id));
	
	// Create a starting transform
	btTransform startTransform;
	startTransform.setIdentity();
	startTransform.setOrigin(position);
	startTransform.setRotation(rotation);

	// Building a rigid body
	btVector3 localInertia(0, 0, 0);
	if (mass != btScalar(0.0))
	{
		shape->calculateLocalInertia(mass, localInertia);
	}

	// Create the motion state and rigid body
	SimMotionState* motionState = new SimMotionState(data->ID, startTransform, &(m_worldData->updatesThisFrame));
	btRigidBody::btRigidBodyConstructionInfo cInfo(mass, motionState, shape, localInertia);
	btRigidBody* body = new btRigidBody(cInfo);
	motionState->RigidBody = body;
	m_body = body;

	UpdatePhysicalParameters(friction, restitution, velocity, false);

	// Set the dynamic and collision flags (for static and phantom objects)
	SetObjectProperties(isStatic, isCollidable, false, mass, false);

	world->dynamicsWorld->addRigidBody(body);

	btVector3 Dvel = m_body->getLinearVelocity();
	btVector3 Dgrav = m_body->getGravity();
	// BSLog("PrimObject::constructor: id=%u, vel=<%f,%f,%f>, grav=<%f,%f,%f>", 
	// 		m_id, Dvel.x(), Dvel.y(), Dvel.z(), Dgrav.x(), Dgrav.y(), Dgrav.z());
}

PrimObject::~PrimObject(void) {
	// BSLog("PrimObject::destructor: id=%u", m_id);
	if (m_body)
	{
		// Remove the object from the world
		m_worldData->dynamicsWorld->removeCollisionObject(m_body);

		// If we added a motionState to the object, delete that
		btMotionState* motionState = m_body->getMotionState();
		if (motionState)
			delete motionState;
		
		// Delete the rest of the memory allocated to this object
		btCollisionShape* shape = m_body->getCollisionShape();
		if (shape) 
			delete shape;

		// finally make the object itself go away
		delete m_body;

		m_body = NULL;
	}
}

bool PrimObject::SetObjectProperties(bool isStatic, bool isSolid, bool genCollisions, float mass)
{
	return SetObjectProperties(isStatic, isSolid, genCollisions, mass, true);
}

// TODO: generalize these parameters so we can model the non-physical/phantom/collidable objects of OpenSimulator
bool PrimObject::SetObjectProperties(bool isStatic, bool isSolid, bool genCollisions, float mass, bool removeIt)
{
	// BSLog("PrimObject::SetObjectProperties: id=%u, rem=%s, isStatic=%d, isSolid=%d, genCollisions=%d, mass=%f", 
	// 				m_id, removeIt?"true":"false", isStatic, isSolid, genCollisions, mass);
	if (removeIt)
	{
		// NOTE: From the author of Bullet: "If you want to change important data 
		// from objects, you have to remove them from the world first, then make the 
		// change, and re-add them to the world." It's my understanding that some of
		// the following changes, including mass, constitute "important data"
		m_worldData->dynamicsWorld->removeRigidBody(m_body);
	}
	SetObjectDynamic(!isStatic, mass, false);		// this handles the static part
	SetCollidable(isSolid);
	if (genCollisions)
	{
		// for the moment, everything generates collisions
		// TODO: Add a flag to CollisionFlags that is checked in StepSimulation on whether to pass up or not
	}
	if (removeIt)
	{
		m_worldData->dynamicsWorld->addRigidBody(m_body);
	}
	return true;
}

void PrimObject::UpdatePhysicalParameters(btScalar frict, btScalar resti, const btVector3& velo)
{
	UpdatePhysicalParameters(frict, resti, velo, true);
}

void PrimObject::UpdatePhysicalParameters(btScalar frict, btScalar resti, const btVector3& velo, bool removeIt)
{
	// Tweak continuous collision detection parameters
	if (removeIt)
	{
		m_worldData->dynamicsWorld->removeRigidBody(m_body);
	}
	if (m_worldData->params->ccdMotionThreshold > 0.0f)
	{
		m_body->setCcdMotionThreshold(btScalar(m_worldData->params->ccdMotionThreshold));
		m_body->setCcdSweptSphereRadius(btScalar(m_worldData->params->ccdSweptSphereRadius));
	}
	m_body->setDamping(m_worldData->params->linearDamping, m_worldData->params->angularDamping);
	m_body->setDeactivationTime(m_worldData->params->deactivationTime);
	m_body->setSleepingThresholds(m_worldData->params->linearSleepingThreshold, m_worldData->params->angularSleepingThreshold);
	m_body->setContactProcessingThreshold(m_worldData->params->contactProcessingThreshold);

	m_body->setFriction(frict);
	m_body->setRestitution(resti);
	m_body->setLinearVelocity(velo);
	// per http://www.bulletphysics.org/Bullet/phpBB3/viewtopic.php?t=3382
	m_body->setInterpolationLinearVelocity(btVector3(0, 0, 0));
	m_body->setInterpolationAngularVelocity(btVector3(0, 0, 0));
	m_body->setInterpolationWorldTransform(m_body->getWorldTransform());
	if (removeIt)
	{
		m_worldData->dynamicsWorld->addRigidBody(m_body);
	}
}

bool PrimObject::SetObjectDynamic(bool isDynamic, float mass)
{
	return SetObjectDynamic(isDynamic, mass, true);
}

// The 'removeIt' flag is true if the body should be removed and reinserted
//    since insertion causes a bunch of stuff to be initialized.
bool PrimObject::SetObjectDynamic(bool isDynamic, float mass, bool removeIt)
{
	const btVector3 ZERO_VECTOR(0.0, 0.0, 0.0);
	btVector3 localInertia(0, 0, 0);

	// BSLog("PrimObject::SetObjectDynamic: id=%u, rem=%s, isDynamic=%d, mass=%f", m_id, removeIt?"true":"false", isDynamic, mass);
	if (removeIt)
	{
		m_worldData->dynamicsWorld->removeRigidBody(m_body);
	}
	if (isDynamic)
	{
		m_body->setCollisionFlags(m_body->getCollisionFlags() & ~btCollisionObject::CF_STATIC_OBJECT);
		// We can't deactivate a physical object because Bullet does not tell us when
		// the object goes idle. The lack of this event means small velocity values are
		// left on the object and the viewer displays the object floating off.
		// m_body->setActivationState(DISABLE_DEACTIVATION);
		// Change for if Bullet has been modified to call MotionState when body goes inactive

		// Recalculate local inertia based on the new mass
		m_body->getCollisionShape()->calculateLocalInertia(mass, localInertia);

		// Set the new mass
		m_body->setMassProps(mass, localInertia);
		m_body->updateInertiaTensor();

		btVector3 Dvel = m_body->getLinearVelocity();
		btVector3 Dgrav = m_body->getGravity();
		// BSLog("PrimObject::SetObjectDynamic: dynamic. ID=%u, Mass = %f, vel=<%f,%f,%f>, grav=<%f,%f,%f>", 
		// 		m_id, mass, Dvel.x(), Dvel.y(), Dvel.z(), Dgrav.x(), Dgrav.y(), Dgrav.z());
	}
	else
	{
		m_body->setCollisionFlags(m_body->getCollisionFlags() | btCollisionObject::CF_STATIC_OBJECT);
		// m_body->setActivationState(WANTS_DEACTIVATION);
		// force the static object to be inactive
		m_body->forceActivationState(ISLAND_SLEEPING);

		// Clear all forces for this object
		m_body->setLinearVelocity(ZERO_VECTOR);
		m_body->setAngularVelocity(ZERO_VECTOR);
		m_body->clearForces();

		// Set the new mass (the caller should be passing zero)
		// mass MUST be zero for Bullet to handle it as a static object
		m_body->setMassProps(0.0, ZERO_VECTOR);
		m_body->updateInertiaTensor();

		// BSLog("PrimObject::SetObjectDynamic: static. ID=%u", m_id);
	}
	if (removeIt)
	{
		m_worldData->dynamicsWorld->addRigidBody(m_body);
	}
	// don't force activation so we don't undo the "ISLAND_SLEEPING" for static objects
	m_body->activate(false);
	return true;
}

void PrimObject::SetCollidable(bool collidable)
{
	// Toggle the CF_NO_CONTACT_RESPONSE on or off for the object to enable/disable phantom behavior
	if (collidable)
		m_body->setCollisionFlags(m_body->getCollisionFlags() & ~btCollisionObject::CF_NO_CONTACT_RESPONSE);
	else
		m_body->setCollisionFlags(m_body->getCollisionFlags() | btCollisionObject::CF_NO_CONTACT_RESPONSE);

	m_body->activate(false);
	return;
}

btVector3 PrimObject::GetObjectPosition()
{
	btTransform xform = m_body->getWorldTransform();
	return xform.getOrigin();
}

btQuaternion PrimObject::GetObjectOrientation()
{
	btTransform xform = m_body->getWorldTransform();
	return xform.getRotation();
}

bool PrimObject::SetObjectTranslation(btVector3& position, btQuaternion& rotation)
{
	const btVector3 ZERO_VECTOR(0.0, 0.0, 0.0);
	// BSLog("PrimObject::SetObjectTranslation: id=%u, pos=<%f,%f,%f>, rot=<%f,%f,%f,%f>",
	// 	m_id, position.x(), position.y(), position.z(), rotation.x(), rotation.y(), rotation.z(), rotation.w());

	// Build a transform containing the new position and rotation
	btTransform transform;
	transform.setIdentity();
	transform.setOrigin(position);
	transform.setRotation(rotation);

	// Clear all forces for this object
	m_body->setLinearVelocity(ZERO_VECTOR);
	m_body->setAngularVelocity(ZERO_VECTOR);
	m_body->clearForces();

	// Set the new transform for the rigid body and the motion state
	m_body->setWorldTransform(transform);
	// Force an update of the position/rotation on the next tick
	m_body->getMotionState()->setWorldTransform(transform);

	m_body->activate(false);
	return true;
}

bool PrimObject::SetObjectVelocity(btVector3& velocity)
{
	// BSLog("PrimObject::SetObjectVelocity: id=%u, vel=<%f,%f,%f>", m_id, velocity.x(), velocity.y(), velocity.z());
	m_body->setLinearVelocity(velocity);
	m_body->activate(false);
	return true;
}

bool PrimObject::SetObjectAngularVelocity(btVector3& angularVelocity)
{
	// BSLog("PrimObject::SetAngularVelocity: id=%u, avel=<%f,%f,%f>", m_id, angularVelocity.x(), angularVelocity.y(), angularVelocity.z());
	m_body->setAngularVelocity(angularVelocity);
	m_body->activate(true);
	return true;
}

bool PrimObject::SetObjectForce(btVector3& force)
{
	// BSLog("PrimObject::SetObjectForce: id=%u, vel=<%f,%f,%f>", m_id, force.x(), force.y(), force.z());
	m_body->applyCentralForce(force);
	m_body->activate(false);
	return true;
}

bool PrimObject::SetObjectScaleMass(btVector3& scale, float mass, bool isDynamic)
{
	// BSLog("PrimObject::SetObjectScaleMass: id=%u, scale=<%f,%f,%f>, mass=%f, isDyn=%s", m_id, scale.x(), scale.y(), scale.z(), mass, isDynamic?"true":"false");
	const btVector3 ZERO_VECTOR(0.0, 0.0, 0.0);

	btCollisionShape* shape = m_body->getCollisionShape();

	// NOTE: From the author of Bullet: "If you want to change important data 
	// from objects, you have to remove them from the world first, then make the 
	// change, and re-add them to the world." It's my understanding that some of
	// the following changes, including mass, constitute "important data"
	m_worldData->dynamicsWorld->removeRigidBody(m_body);

	// Clear all forces for this object
	m_body->setLinearVelocity(ZERO_VECTOR);
	m_body->setAngularVelocity(ZERO_VECTOR);
	m_body->clearForces();

	// Set the new scale
	AdjustScaleForCollisionMargin(shape, scale);

	// apply the mass and dynamicness
	SetObjectDynamic(isDynamic, mass, false);

	// Add the rigid body back to the simulation
	m_worldData->dynamicsWorld->addRigidBody(m_body);

	// Calculate a new AABB for this object
	m_worldData->dynamicsWorld->updateSingleAabb(m_body);

	m_body->activate(false);
	return true;
}

bool PrimObject::SetObjectCollidable(bool collidable)
{
	// BSLog("PrimObject::SetObjectCollidable: id=%u, collidable=%s", m_id, collidable?"true":"false");
	SetCollidable(collidable);
	return true;
}

// Adjust how gravity effects the object
// neg=fall quickly, 0=1g, 1=0g, pos=float up
bool PrimObject::SetObjectBuoyancy(float buoy)
{
	float grav = m_worldData->params->gravity * (1.0f - buoy);
	// BSLog("PrimObject::SetObjectBuoyancy: id=%u, buoy=%f, grav=%s", m_id, buoy, grav);

	m_body->setGravity(btVector3(0, 0, grav));

	m_body->activate(false);

	return true;
}

// Create and return the collision shape specified by the ShapeData.
btCollisionShape* PrimObject::CreateShape(ShapeData* data)
{
	ShapeData::PhysicsShapeType type = data->Type;
	Vector3 scale = data->Scale;
	btVector3 scaleBt = scale.GetBtVector3();
	WorldData::MeshesMapType::const_iterator mt;
	WorldData::HullsMapType::const_iterator ht;

	btCollisionShape* shape = NULL;

	switch (type)
	{
		case ShapeData::SHAPE_AVATAR:
			// should not happen for a prim
			break;
		case ShapeData::SHAPE_BOX:
			// btBoxShape subtracts the collision margin from the half extents, so no 
			// fiddling with scale necessary
			// boxes are defined by their half extents
			shape = new btBoxShape(btVector3(0.5, 0.5, 0.5));	// this is really a unit box
			shape->setMargin(m_worldData->params->collisionMargin);
			AdjustScaleForCollisionMargin(shape, scaleBt);
			break;
		case ShapeData::SHAPE_CONE:	// TODO:
			shape = new btConeShapeZ(0.5, 1.0);
			shape->setMargin(m_worldData->params->collisionMargin);
			break;
		case ShapeData::SHAPE_CYLINDER:	// TODO:
			shape = new btCylinderShapeZ(btVector3(0.5f, 0.5f, 0.5f));
			shape->setMargin(m_worldData->params->collisionMargin);
			break;
		case ShapeData::SHAPE_MESH:
			mt = m_worldData->Meshes.find(data->MeshKey);
			if (mt != m_worldData->Meshes.end())
			{
				// BSLog("CreateShape: SHAPE_MESH. localID=%u", data->ID);
				btBvhTriangleMeshShape* origionalMeshShape = mt->second;
				// we have to copy the mesh shape because we don't keep use counters
				shape = DuplicateMeshShape(origionalMeshShape);
				shape->setMargin(m_worldData->params->collisionMargin);
				AdjustScaleForCollisionMargin(shape, scaleBt);
			}
			break;
		case ShapeData::SHAPE_HULL:
			ht = m_worldData->Hulls.find(data->HullKey);
			if (ht != m_worldData->Hulls.end())
			{
				// The compound shape stored in m_hulls is really just a storage container for
				// the the individual convex hulls and their offsets. Here we copy each child
				// convex hull and its offset to the new compound shape which will actually be
				// inserted into the physics simulation
				// BSLog("CreateShape: SHAPE_HULL. localID=%u", data->ID);
				btCompoundShape* originalCompoundShape = ht->second;
				shape = DuplicateCompoundShape(originalCompoundShape);
				shape->setMargin(m_worldData->params->collisionMargin);
				AdjustScaleForCollisionMargin(shape, scaleBt);
			}
			break;
		case ShapeData::SHAPE_SPHERE:
			shape = new btSphereShape(0.5);		// this is really a unit sphere
			shape->setMargin(m_worldData->params->collisionMargin);
			AdjustScaleForCollisionMargin(shape, scaleBt);
			break;
	}

	return shape;
}

// create a new compound shape that contains the hulls of the passed compound shape
btCompoundShape* PrimObject::DuplicateCompoundShape(btCompoundShape* originalCompoundShape)
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

btCollisionShape* PrimObject::DuplicateMeshShape(btBvhTriangleMeshShape* mShape)
{
	btBvhTriangleMeshShape* newTMS = new btBvhTriangleMeshShape(mShape->getMeshInterface(), true, true);
	return newTMS;
}

// Bullet has a 'collisionMargin' that makes the mesh a little bigger. This routine
// reduces the scale of the underlying mesh to make the mesh plus margin the
// same size as the original mesh.
// TODO: figure out of this works correctly. For the moment set gCollisionMargin to zero.
//    Bullet tries to use scale only on the shape and does not include the margin in the scale
//    calculation. This is true in most cases but there seem to be some shapes that get
//    this wrong. 
void PrimObject::AdjustScaleForCollisionMargin(btCollisionShape* shape, btVector3& scale)
{
	btVector3 aabbMin;
	btVector3 aabbMax;
	btTransform transform;
	transform.setIdentity();
	
	// we use the constant margin because SPHERE getMargin does not return the real margin
	// btScalar margin = shape->getMargin();
	btScalar margin = m_worldData->params->collisionMargin;
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

bool PrimObject::UpdateParameter(const char* parm, const float val)
{
	btScalar btVal = btScalar(val);

	if (strcmp(parm, "friction") == 0)
	{
		m_body->setFriction(btVal);
		return true;
	}
	if (strcmp(parm, "restitution") == 0)
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
	return false;
}