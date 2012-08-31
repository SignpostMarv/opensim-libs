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

#ifndef TERRAIN_OBJECT_H
#define TERRAIN_OBJECT_H

#include "APIData.h"
#include "IPhysObject.h"
#include "WorldData.h"
#include "Util.h"

// A class used by TerrainObject to hold the height map
class HeightMapData
{
public:
	HeightMapData(float maxX, float maxY, float height) {
		MaxX = maxX;
		MaxY = maxY;
		int imaxX = (int)maxX;
		int imaxY = (int)maxY;

		HeightMap = new float[imaxX * imaxY];

		for (int y = 0; y < imaxY; y++)
		{
			for (int x = 0; x < imaxX; x++)
			{
				HeightMap[y * imaxX + x] = height;
			}
		}
	}
	~HeightMapData(void) {
		delete HeightMap;
		HeightMap = NULL;
	}

	bool UpdateHeightMap(float* heightMap, float maxX, float maxY)
	{
		bool ret = false;
		// Cannot reallocate memory if the size changes since a pointer
		//   to the heightmap data has been given to Bullet.
		if (MaxX == maxX && MaxY == maxY)
		{
			int imaxX = (int)maxX;
			int imaxY = (int)maxY;

			// copy the passed data into our new heightmap
			bsMemcpy(HeightMap, heightMap, imaxY * imaxX * sizeof(float));
			ret = true;
		}

		return ret;
	}

	float GetHeightAtXYZ(btVector3& pos)
	{
		return GetHeightAtXY(pos.getX(), pos.getY());
	}

	float GetHeightAtXY(float xx, float yy)
	{
		float ret = 0.0;
		if (xx >= 0 && xx < MaxX && yy >= 0 && yy < MaxY)
		{
			ret = HeightMap[((int)xx) + (((int)yy) * ((int)MaxY))];
		}
		return ret;
	}

	float* HeightMap;
	float MaxX;
	float MaxY;
private:
};

class TerrainObject :
	public IPhysObject
{
public:
	TerrainObject(WorldData*, IDTYPE);
	TerrainObject(WorldData*, IDTYPE, float* newMap);
	~TerrainObject(void);

	void CreateTerrainBody(void);
	void UpdateHeightMap(float* newMap);

	void UpdatePhysicalParameters(float friction, float restitution, btVector3& velo);

	float GetHeightAtXYZ(btVector3& pos);

	const char* GetType() { return "Terrain"; }

private:
	HeightMapData* m_heightMap;
	btRigidBody* m_body;		// the physical body that is the terrain
};

#endif // TERRAIN_OBJECT_H
