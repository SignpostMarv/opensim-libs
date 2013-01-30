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

#include "ArchStuff.h"
#include "APIData.h"
#include "btBulletDynamicsCommon.h"

#include <stdarg.h>
#include <map>

// Forward references
class BulletSim;
class ObjectCollection;
class HeightMapData;
class IPhysObject;
class TerrainObject;
class GroundPlaneObject;

// template for debugging call
typedef void DebugLogCallback(const char*);

// Structure to hold the world data that is common to all the objects in the world
struct WorldData
{
	BulletSim* sim;

	ParamBlock* params;
	
	// pointer to the containing world control structure
	btDynamicsWorld* dynamicsWorld;

	// The minimum and maximum points in the defined physical space
	btVector3 MinPosition;
	btVector3 MaxPosition;

	// Used to expose updates from Bullet to the BulletSim API
	typedef std::map<IDTYPE, EntityProperties*> UpdatesThisFrameMapType;
	UpdatesThisFrameMapType updatesThisFrame;

	// Some collisionObjects can set themselves up for special collision processing.
	// This is used for ghost objects to be handed in the simulation step.
	typedef std::map<IDTYPE, btCollisionObject*> SpecialCollisionObjectMapType;
	SpecialCollisionObjectMapType specialCollisionObjects;

	// DEBUGGGING
	// ============================================================================================
	// Callback to managed code for logging
	// This is a callback into the managed code that writes a text message to the log.
	// This callback is only initialized if the simulator is running in DEBUG mode.
	DebugLogCallback* debugLogCallback;

	/*
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
	// Call back into the managed world to output a log message with formatting
	void BSLog(const char* msg, ...) {
		if (debugLogCallback != NULL) {
			va_list args;
			va_start(args, msg);
			BSLog2(msg, args);
			va_end(args);
		}
	}
	void BSLog2(const char* msg, va_list argp)
	{
		char buff[2048];
		if (debugLogCallback != NULL) {
			vsprintf(buff, msg, argp);
			(*debugLogCallback)(buff);
		}
	}

	void	dumpAll()
	{
		BSLog("PROFILE LOGGING IS NOT ENABLED IN BULLET");
	}

	/*	Uncomment this if profiling is turned on in Bullet.
	// The following code was borrowed directly from Bullet.
	// The Bullet dumper uses printf to output stats and BulletSim needs it to call BSLog.]
	// Just copying the code here turned out to be the quickest and easiest solution.
	void dumpRecursive(CProfileIterator* profileIterator, int spacing)
	{
		profileIterator->First();
		if (profileIterator->Is_Done())
			return;

		char dots[256];
		for (int ii=0; ii<spacing; ii++)
			dots[ii] = '.';
		dots[spacing] = '\0';

		float accumulated_time=0,parent_time = profileIterator->Is_Root() ? CProfileManager::Get_Time_Since_Reset() : profileIterator->Get_Current_Parent_Total_Time();
		int i;
		int frames_since_reset = CProfileManager::Get_Frame_Count_Since_Reset();
		BSLog("%s----------------------------------", dots);
		BSLog("%sProfiling: %s (total running time: %.3f ms) ---",dots,profileIterator->Get_Current_Parent_Name(), parent_time );
		float totalTime = 0.f;

		
		int numChildren = 0;
		
		for (i = 0; !profileIterator->Is_Done(); i++,profileIterator->Next())
		{
			numChildren++;
			float current_total_time = profileIterator->Get_Current_Total_Time();
			accumulated_time += current_total_time;
			float fraction = parent_time > SIMD_EPSILON ? (current_total_time / parent_time) * 100 : 0.f;
			BSLog("%s%d -- %s (%.2f %%) :: %.3f ms / frame (%d calls)",dots,i, profileIterator->Get_Current_Name(), fraction,(current_total_time / (double)frames_since_reset),profileIterator->Get_Current_Total_Calls());
			totalTime += current_total_time;
			//recurse into children
		}

		if (parent_time < accumulated_time)
		{
			BSLog("what's wrong");
		}
		BSLog("%s%s (%.3f %%) :: %.3f ms", dots,"Unaccounted:",parent_time > SIMD_EPSILON ? ((parent_time - accumulated_time) / parent_time) * 100 : 0.f, parent_time - accumulated_time);
		
		for (i=0;i<numChildren;i++)
		{
			profileIterator->Enter_Child(i);
			dumpRecursive(profileIterator,spacing+3);
			profileIterator->Enter_Parent();
		}
	}

	void	dumpAll()
	{
		CProfileIterator* profileIterator = 0;
		profileIterator = CProfileManager::Get_Iterator();

		dumpRecursive(profileIterator,0);

		CProfileManager::Release_Iterator(profileIterator);
	}
	*/

};


#endif // WORLD_DATA_H