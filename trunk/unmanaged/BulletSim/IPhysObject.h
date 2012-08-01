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
#pragma once

#ifndef IPHYSOBJECT_H
#define IPHYSOBJECT_H

#include "APIData.h"
#include "WorldData.h"
#include "btBulletDynamicsCommon.h"

class IPhysObject
{
public:
	IPhysObject(void);
	virtual ~IPhysObject(void);

	// Passed a parameter block, create a new instance of the proper object
	static IPhysObject* PhysObjectFactory(WorldData*, ShapeData*);

	// Objects can register to be called each step. This is the method called.
	virtual bool StepCallback(IDTYPE id, WorldData* worldData) { return false; };

	// These functions have a default null implementation so all sub-classes don't need
	//   to define the method if it is not used by that object type.

	virtual btVector3 GetObjectPosition(void) { return btVector3(0.0, 0.0, 0.0); };
	virtual btQuaternion GetObjectOrientation(void) { return btQuaternion::getIdentity(); };

	virtual bool SetObjectTranslation(btVector3& position, btQuaternion& rotation) { return false; };
	virtual bool SetObjectVelocity(btVector3& velocity) { return false; };
	virtual bool SetObjectAngularVelocity(btVector3& angularVelocity) { return false; };
	virtual bool SetObjectForce(btVector3& force) { return false; };
	virtual bool SetObjectScaleMass(btVector3& scale, float mass, bool isDynamic) { return false; };
	virtual bool SetObjectProperties(const bool isStatic, const bool isCollidable, const bool genCollisions, const float mass) { return false; };
	virtual bool SetObjectCollidable(bool collidable) { return false; };
	virtual bool SetObjectDynamic(bool isDynamic, float mass) { return false; };
	virtual bool SetObjectBuoyancy(float buoy) { return false; };

	virtual bool UpdateParameter(const char* parm, const float val) { return false; };
	virtual void UpdatePhysicalParameters(float friction, float restitution, btVector3& velo) { };

	virtual const char* GetType() { return "Unknown"; }
	
	// no need to over-ride these since they return a varialble we hold for our children
	btRigidBody* GetBody() { return m_body; };
	IDTYPE GetID() { return m_id; }

protected:
	IDTYPE m_id;			// the ID used to identify this object

	btRigidBody* m_body;	// pointer to the physics instance

	WorldData* m_worldData;	// pointer to the physics context for this object
};

#endif	// IPHYSOBJECT_H
