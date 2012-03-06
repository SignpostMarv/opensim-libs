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

#ifndef CONSTRAINT_COLLECTION_H
#define CONSTRAINT_COLLECTION_H

#include "ArchStuff.h"
#include "WorldData.h"
#include "Constraint.h"

#include "btBulletDynamicsCommon.h"

#include <map>

#define CONSTRAINTIDTYPE unsigned long long

class ConstraintCollection
{
public:
	ConstraintCollection(WorldData*);
	~ConstraintCollection(void);

	void Clear(void);

	bool AddConstraint(Constraint*);

	// Remove all constraints that reference this ID
	bool RemoveAndDestroyConstraints(IDTYPE);

	// Remove the constraint between these two objects
	bool RemoveAndDestroyConstraint(IDTYPE, IDTYPE);

	// An object has changed shape/mass. Recompute the constraint transforms.
	bool RecalculateAllConstraints(IDTYPE);

private:
	typedef std::map<CONSTRAINTIDTYPE, Constraint*> ConstraintMapType;
	ConstraintMapType m_constraints;

	WorldData* m_worldData;

	CONSTRAINTIDTYPE GenConstraintID(IDTYPE id1, IDTYPE id2);
};

#endif   // CONSTRAINT_COLLECTION_H