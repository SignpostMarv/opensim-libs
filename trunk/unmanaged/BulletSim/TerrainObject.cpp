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
#include "TerrainObject.h"

#include "btBulletDynamicsCommon.h"
#include "BulletCollision/CollisionShapes/btHeightfieldTerrainShape.h"

TerrainObject::TerrainObject(WorldData* world, IDTYPE theID)
{
	m_worldData = world;
	m_id = theID;

	// start the terrain as flat
	m_heightMap = new HeightMapData(world->MaxPosition.getX(), world->MaxPosition.getY(), 24.1);

	CreateTerrainBody();
}

TerrainObject::TerrainObject(WorldData* world, IDTYPE theID, float* newMap)
{
	m_worldData = world;
	m_id = theID;

	if (m_heightMap == NULL)
	{
		m_heightMap = new HeightMapData(world->MaxPosition.getX(), world->MaxPosition.getY(), 24.2);
	}
	m_heightMap->UpdateHeightMap(newMap, m_heightMap->MaxX, m_heightMap->MaxY);

	CreateTerrainBody();
}

TerrainObject::~TerrainObject(void)
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
	if (m_heightMap)
	{
		delete m_heightMap;
		m_heightMap = NULL;
	}
}

void TerrainObject::CreateTerrainBody()
{
	btVector3 zeroVector = btVector3(0, 0, 0);

	if (m_heightMap == NULL)
		return;

	// Initialize the terrain that spans from 0,0,0 to m_maxPosition
	// TODO: Use the maxHeight from m_maxPosition.getZ()
	int heightMapWidth = (int)m_heightMap->MaxX;
	int heightMapLength = (int)m_heightMap->MaxY;

	float minHeight = 99999;
	float maxHeight = 0;
	// find the minimum and maximum height
	for (int yy = 0; yy<heightMapWidth; yy++)
	{
		for (int xx = 0; xx<heightMapLength; xx++)
		{
			float heightHere = m_heightMap->GetHeightAtXY(xx, yy);
			if (heightHere < minHeight) minHeight = heightHere;
			if (heightHere > maxHeight) maxHeight = heightHere;
		}
	}
	if (minHeight == maxHeight)
	{
		// make different so the terrain gets a bounding box
		minHeight = maxHeight - 1.0f;
	}
	const int upAxis = 2;
	const btScalar scaleFactor(1.0);
	btHeightfieldTerrainShape* m_heightfieldShape = new btHeightfieldTerrainShape(
			heightMapWidth, heightMapLength, 
			m_heightMap->HeightMap, scaleFactor, 
			(btScalar)minHeight, (btScalar)maxHeight, upAxis, PHY_FLOAT, false);
	// there is no room between the terrain and an object
	m_heightfieldShape->setMargin(0.0f);
	// m_heightfieldShape->setMargin(gCollisionMargin);
	m_heightfieldShape->setUseDiamondSubdivision(true);

	// Add the localID to the object so we know about collisions
	m_heightfieldShape->setUserPointer(PACKLOCALID(m_id));

	// Set the heightfield origin
	btTransform heightfieldTr;
	heightfieldTr.setIdentity();
	heightfieldTr.setOrigin(btVector3(
			((float)heightMapWidth) * 0.5f,
			((float)heightMapLength) * 0.5f,
			minHeight + (maxHeight - minHeight) * 0.5f));

	btVector3 theOrigin = heightfieldTr.getOrigin();

	// Use the default motion state since we are not interested in the
	//   terrain reporting its collisions. Other objects will report their
	//   collisions with the terrain.
	btDefaultMotionState* motionState = new btDefaultMotionState(heightfieldTr);
	btRigidBody::btRigidBodyConstructionInfo cInfo(0.0, motionState, m_heightfieldShape);
	m_body = new btRigidBody(cInfo);

	m_body->setCollisionFlags(btCollisionObject::CF_STATIC_OBJECT);
	UpdatePhysicalParameters( m_worldData->params->terrainFriction,
				m_worldData->params->terrainRestitution,
				zeroVector);

	m_worldData->dynamicsWorld->addRigidBody(m_body);
	m_worldData->dynamicsWorld->updateSingleAabb(m_body);
}

void TerrainObject::UpdateHeightMap(float* newMap)
{
	if (m_heightMap != NULL)
	{
		m_heightMap->UpdateHeightMap(newMap, m_heightMap->MaxX, m_heightMap->MaxY);
	}
}

void TerrainObject::UpdatePhysicalParameters(float friction, float restitution, btVector3& velo)
{
	m_body->setFriction(btScalar(friction));
	m_body->setHitFraction(btScalar(m_worldData->params->terrainHitFraction));
	m_body->setRestitution(btScalar(restitution));
	// body->setActivationState(DISABLE_DEACTIVATION);
	m_body->activate(true);
}

float TerrainObject::GetHeightAtXYZ(btVector3& pos)
{
	return m_heightMap->GetHeightAtXYZ(pos);
}

