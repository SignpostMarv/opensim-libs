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

#ifndef API_DATA_H
#define API_DATA_H

#include "ArchStuff.h"
#include "btBulletDynamicsCommon.h"

// Fixed object ID codes used by OpenSimulator
#define ID_TERRAIN 0	// OpenSimulator identifies collisions with terrain by localID of zero
#define ID_GROUND_PLANE 1
#define ID_INVALID_HIT 0xFFFFFFFF

// API-exposed structure for a 3D vector
struct Vector3
{
	float X;
	float Y;
	float Z;

	Vector3()
	{
		X = 0.0;
		Y = 0.0;
		Z = 0.0;
	}

	Vector3(float x, float y, float z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	Vector3(const btVector3& v)
	{
		X = v.getX();
		Y = v.getY();
		Z = v.getZ();
	}

	bool AlmostEqual(const Vector3& v, const float nEpsilon)
	{
		return
			(((v.X - nEpsilon) < X) && (X < (v.X + nEpsilon))) &&
			(((v.Y - nEpsilon) < Y) && (Y < (v.Y + nEpsilon))) &&
			(((v.Z - nEpsilon) < Z) && (Z < (v.Z + nEpsilon)));
	}

	btVector3 GetBtVector3()
	{
		return btVector3(X, Y, Z);
	}

	void operator= (const btVector3& v)
	{
		X = v.getX();
		Y = v.getY();
		Z = v.getZ();
	}

	bool operator!= (const Vector3& o)
	{
		return (
			   X != o.X
			|| Y != o.Y
			|| Z != o.Z
		);
	}

	bool operator==(const Vector3& b)
	{
		return (X == b.X && Y == b.Y && Z == b.Z);
	}
};

// API-exposed structure for a rotation
struct Quaternion
{
	float X;
	float Y;
	float Z;
	float W;

	Quaternion()
	{
		X = 0.0;
		Y = 0.0;
		Z = 0.0;
		W = 1.0;
	}

	Quaternion(float xx, float yy, float zz, float ww)
	{
		X = xx;
		Y = yy;
		Z = zz;
		W = ww;
	}

	Quaternion(const btQuaternion& btq)
	{
		X = btq.getX();
		Y = btq.getY();
		Z = btq.getZ();
		W = btq.getW();
	}

	bool AlmostEqual(const Quaternion& q, float nEpsilon)
	{
		return
			(((q.X - nEpsilon) < X) && (X < (q.X + nEpsilon))) &&
			(((q.Y - nEpsilon) < Y) && (Y < (q.Y + nEpsilon))) &&
			(((q.Z - nEpsilon) < Z) && (Z < (q.Z + nEpsilon))) &&
			(((q.W - nEpsilon) < W) && (W < (q.W + nEpsilon)));
	}

	btQuaternion GetBtQuaternion()
	{
		return btQuaternion(X, Y, Z, W);
	}

	void operator= (const btQuaternion& q)
	{
		X = q.getX();
		Y = q.getY();
		Z = q.getZ();
		W = q.getW();
	}
};

struct Matrix3x3
{
	Vector3 m_el[3];
public:
	Matrix3x3()
	{
		m_el[0] = Vector3(1.0, 0.0, 0.0);
		m_el[1] = Vector3(0.0, 1.0, 0.0);
		m_el[2] = Vector3(0.0, 0.0, 1.0);
	}
	void operator= (const btMatrix3x3& o)
	{
		m_el[0] = o.getRow(0);
		m_el[1] = o.getRow(1);
		m_el[2] = o.getRow(2);
	}
	btMatrix3x3 GetBtMatrix3x3()
	{
		return btMatrix3x3(
			m_el[0].X, m_el[0].Y, m_el[0].Z,
			m_el[1].X, m_el[1].Y, m_el[1].Z,
			m_el[2].X, m_el[2].Y, m_el[2].Z );
	}
};

struct Transform
{
	// A btTransform is made of a 3x3 array plus a btVector3 copy of the origin.
	// Note that a btVector3 is defined as 4 floats (probably for alignment)
	//    which is why the assignment is done explicitly to get type conversion.
	Matrix3x3 m_basis;
	Vector3 m_origin;
public:
	Transform() { 
		m_basis = Matrix3x3();
		m_origin = Vector3();
	}
	Transform(const btTransform& t)
	{
		m_basis = t.getBasis();
		m_origin = t.getOrigin();
	}
	btTransform GetBtTransform()
	{
		return btTransform(m_basis.GetBtMatrix3x3(), m_origin.GetBtVector3());
	}

};

// API-exposed structure defining an object
struct ShapeData
{
	enum PhysicsShapeType
	{
		SHAPE_UNKNOWN	= 0,
		SHAPE_AVATAR	= 1,
		SHAPE_BOX		= 2,
		SHAPE_CONE		= 3,
		SHAPE_CYLINDER	= 4,
		SHAPE_SPHERE	= 5,
		SHAPE_MESH		= 6,
		SHAPE_HULL		= 7
	};

	// note that bool's are passed as floats's since bool size changes by language
	IDTYPE ID;
	PhysicsShapeType Type;
	Vector3 Position;
	Quaternion Rotation;
	Vector3 Velocity;
	Vector3 Scale;
	float Mass;
	float Buoyancy;		// gravity effect on the object
	MESHKEYTYPE HullKey;
	MESHKEYTYPE MeshKey;
	float Friction;
	float Restitution;
	float Collidable;	// things can collide with this object
	float Static;	// object is non-moving. Otherwise gravity, etc
};

// API-exposed structure for reporting a collision
struct CollisionDesc
{
	IDTYPE aID;
	IDTYPE bID;
	Vector3 point;
	Vector3 normal;
	float penetration;
};

// BulletSim extends the definition of the collision flags
//   so we can control when collisions are desired.
#define BS_SUBSCRIBE_COLLISION_EVENTS    (0x0400)
#define BS_FLOATS_ON_WATER               (0x0800)
#define BS_VEHICLE_COLLISIONS            (0x1000)
#define BS_RETURN_ROOT_COMPOUND_SHAPE    (0x2000)

// Combination of above bits for all settings that want collisions reported
#define BS_WANTS_COLLISIONS              (0x1400)

// API-exposed structure to input a convex hull
struct ConvexHull
{
	Vector3 Offset;
	uint32_t VertexCount;
	Vector3* Vertices;
};

// API-exposed structured to return a raycast result
struct RaycastHit
{
	IDTYPE ID;
	float Fraction;
	Vector3 Normal;
};

// API-exposed structure to return a convex sweep result
struct SweepHit
{
	IDTYPE ID;
	float Fraction;
	Vector3 Normal;
	Vector3 Point;
};

// API-exposed structure to return physics updates from Bullet
struct EntityProperties
{
	IDTYPE ID;
	Vector3 Position;
	Quaternion Rotation;
	Vector3 Velocity;
	Vector3 Acceleration;
	Vector3 AngularVelocity;

	EntityProperties(IDTYPE id, const btTransform& startTransform)
	{
		ID = id;
		Position = startTransform.getOrigin();
		Rotation = startTransform.getRotation();
	}

	void operator= (const EntityProperties& e)
	{
		ID = e.ID;
		Position = e.Position;
		Rotation = e.Rotation;
		Velocity = e.Velocity;
		Acceleration = e.Acceleration;
		AngularVelocity = e.AngularVelocity;
	}
};

// added values for collision CFM and ERP setting so we can set all axis at once
#define COLLISION_AXIS_LINEAR_ALL (20)
#define COLLISION_AXIS_ANGULAR_ALL (21)
#define COLLISION_AXIS_ALL (22)

// Block of parameters passed from the managed code.
// The memory layout MUST MATCH the layout in the managed code.
// Rely on the fact that 'float' is always 32 bits in both C# and C++
#define ParamTrue (1.0)
#define ParamFalse (0.0)
struct ParamBlock
{
    float defaultFriction;
    float defaultDensity;
	float defaultRestitution;
    float collisionMargin;
    float gravity;

	float maxPersistantManifoldPoolSize;
	float maxCollisionAlgorithmPoolSize;
	float shouldDisableContactPoolDynamicAllocation;
	float shouldForceUpdateAllAabbs;
	float shouldRandomizeSolverOrder;
	float shouldSplitSimulationIslands;
	float shouldEnableFrictionCaching;
	float numberOfSolverIterations;
    float useSingleSidedMeshes;
	float globalContactBreakingThreshold;

	float physicsLoggingFrames;
};


// Block of parameters for HACD algorithm
struct HACDParams
{
	float maxVerticesPerHull;		// 100
	float minClusters;				// 2
	float compacityWeight;			// 0.1
	float volumeWeight;				// 0.0
	float concavity;				// 100
	float addExtraDistPoints;		// false
	float addNeighboursDistPoints;	// false
	float addFacesPoints;			// false
	float shouldAdjustCollisionMargin;	// false
};

#define CONSTRAINT_NOT_SPECIFIED (-1)
#define CONSTRAINT_NOT_SPECIFIEDF (-1.0)


#endif // API_DATA_H