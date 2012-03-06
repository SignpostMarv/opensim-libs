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

#include "ConstraintCollection.h"

ConstraintCollection::ConstraintCollection(WorldData* world)
{
	m_worldData = world;
}

ConstraintCollection::~ConstraintCollection(void)
{
	// clean out our collection
	Clear();
}

void ConstraintCollection::Clear(void)
{
	for (ConstraintMapType::iterator it = m_constraints.begin(); it != m_constraints.end(); it++)
	{
		Constraint* constraint = it->second;
		delete constraint;
	}
	m_constraints.clear();
}

bool ConstraintCollection::AddConstraint(Constraint* constraint)
{
	// add the constraint to our collection
	CONSTRAINTIDTYPE constID = GenConstraintID(constraint->GetID1(), constraint->GetID2());
	m_constraints[constID] = constraint;
	return true;
}

bool ConstraintCollection::RemoveAndDestroyConstraints(IDTYPE id1)
{
	bool removedSomething = false;
	for (ConstraintMapType::iterator it = m_constraints.begin(); it != m_constraints.end(); it++)
	{
		Constraint* constraint = it->second;
		if (id1 == constraint->GetID1() || id1 == constraint->GetID2())
		{
			Constraint* constraint = it->second;
			m_constraints.erase(it);
			delete constraint;
			removedSomething = true;
		}
	}
	return removedSomething;	// 'true' if we actually deleted a constraint
}

bool ConstraintCollection::RemoveAndDestroyConstraint(IDTYPE id1, IDTYPE id2)
{
	CONSTRAINTIDTYPE constID = GenConstraintID(id1, id2);
	bool removedSomething = false;
	for (ConstraintMapType::iterator it = m_constraints.begin(); it != m_constraints.end(); it++)
	{
		CONSTRAINTIDTYPE checkID = it->first;
		if (constID == checkID)
		{
			Constraint* constraint = it->second;
			m_constraints.erase(it);
			delete constraint;
			removedSomething = true;
			break;	// there should only be one constraint for any pair of IDs
		}
	}
	return removedSomething;	// return 'true' if we actually deleted a constraint
}

// One or more objects changed shape/mass. Recompute the constraint transforms.
// Find all the constraints with this object included and recalcuate it
bool ConstraintCollection::RecalculateAllConstraints(IDTYPE id1)
{
	bool recalcuatedSomething = false;
	for (ConstraintMapType::iterator it = m_constraints.begin(); it != m_constraints.end(); it++)
	{
		Constraint* constraint = it->second;
		if (id1 == constraint->GetID1() || id1 == constraint->GetID2())
		{
			it->second->GetBtConstraint()->calculateTransforms();
			recalcuatedSomething = true;
		}
	}
	return recalcuatedSomething;	// return 'true' if we actually recalcuated a constraint
}

// There can be only one constraint between objects. Always use the lowest id as the first part
// of the key so the same constraint will be found no matter what order the caller passes them.
CONSTRAINTIDTYPE ConstraintCollection::GenConstraintID(IDTYPE id1, IDTYPE id2)
{
	CONSTRAINTIDTYPE newID = 0;
	if (id1 < id2)
		newID = ((CONSTRAINTIDTYPE)id1 << 32) | id2;
	else
		newID = ((CONSTRAINTIDTYPE)id2 << 32) | id1;

	return newID;
}

