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

#ifndef WORLD_DATA_H
#define WORLD_DATA_H

#include "APIData.h"
#include "ObjectCollection.h"
#include "ConstraintCollection.h"
#include "btBulletDynamicsCommon.h"

#include <map>

// Structure to hold the world data that is common to all the objects in the world
struct WorldData
{
	ParamBlock* params;
	
	// pointer to the containing world control structure
	btDynamicsWorld* dynamicsWorld;

	// Used to expose updates from Bullet to the BulletSim API
	typedef std::map<IDTYPE, EntityProperties*> UpdatesThisFrameMapType;
	UpdatesThisFrameMapType updatesThisFrame;

	// Objects in this world
	// We create a class instance (using IPhysObjectFactory()) for each of the
	// object types kept in the world. This separates the code for handling
	// the physical object from the interface to the simulator.
	// Once collection object is created to hold the objects. This also
	// has the list manipulation functions.
	ObjectCollection* objects;

	// Constraints created in this world
	ConstraintCollection* constraints;
};

#endif // WORLD_DATA_H