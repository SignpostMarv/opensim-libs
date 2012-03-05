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
#include "GroundPlaneObject.h"

GroundPlaneObject::GroundPlaneObject(WorldData* world)
{
	m_worldData = world;

	// Initialize the ground plane at height 0 (Z-up)
	m_planeShape = new btStaticPlaneShape(btVector3(0, 0, 1), 1);
	m_planeShape->setMargin(m_worldData->params->collisionMargin);

	m_planeShape->setUserPointer((void*)ID_GROUND_PLANE);

	btDefaultMotionState* motionState = new btDefaultMotionState();
	btRigidBody::btRigidBodyConstructionInfo cInfo(0.0, motionState, m_planeShape);
	m_body = new btRigidBody(cInfo);

	m_body->setCollisionFlags(btCollisionObject::CF_STATIC_OBJECT);
	
	m_worldData->dynamicsWorld->addRigidBody(m_body);
}

GroundPlaneObject::~GroundPlaneObject(void)
{
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
