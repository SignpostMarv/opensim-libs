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

#ifndef AVATAROBJECT_H
#define AVATAROBJECT_H

#include "IPhysObject.h"
#include "APIData.h"
#include "WorldData.h"

class AvatarObject :
	public IPhysObject
{
public:
	AvatarObject(WorldData*, ShapeData*);
	~AvatarObject(void);

	bool SetProperties(const bool isStatic, const bool isCollidable, const bool genCollisions, const float mass);

	btVector3 GetObjectPosition(void);

	bool SetDynamic(const bool isPhysical, const float mass);
	bool SetScaleMass(const float scale, const float mass);
	bool SetObjectTranslation(btVector3& position, btQuaternion& rotation);
	bool SetObjectVelocity(btVector3& velocity);
	bool SetObjectAngularVelocity(btVector3& angularVelocity);
	bool SetObjectForce(btVector3& force);
	bool SetObjectScaleMass(btVector3& scale, float mass, bool isDynamic);
	bool SetObjectCollidable(bool collidable);
	bool SetObjectBuoyancy(float buoy);

	void UpdateParameter(const char* parm, const float val);
	void UpdatePhysicalParameters(float friction, float restitution, const btVector3& velo);

private:
};

#endif // AVATAROBJECT_H
