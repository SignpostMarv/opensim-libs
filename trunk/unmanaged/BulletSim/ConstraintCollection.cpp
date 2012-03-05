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
}

void ConstraintCollection::Clear(void)
{
}

bool ConstraintCollection::AddConstraint(IDTYPE id1, IDTYPE id2, btTypedConstraint* constraint)
{
    m_worldData->dynamicsWorld->addConstraint(constraint, true);
	return false;
}

bool ConstraintCollection::RemoveConstraints(IDTYPE id1)
{
	/*
	bool removedSomething = false;
	bool doAgain = true;
	while (doAgain)
	{
		doAgain = false;	// start out thinking one time through is enough
		ConstraintMapType::iterator it = m_constraints.begin();
		while (it != m_constraints.end())
		{
			unsigned long long constraintID = it->first;
			// if this constraint contains the passed localID, delete the constraint
			if ((((IDTYPE)(constraintID & 0xffffffff)) == id1)
				|| (((IDTYPE)(constraintID >> 32) & 0xffffffff) == id1))
			{
				btGeneric6DofConstraint* constraint = it->second;
		 		m_worldData.dynamicsWorld->removeConstraint(constraint);
				m_constraints.erase(it);
				delete constraint;
				removedSomething = true;
				doAgain = true;	// if we deleted, we scan the list again for another match
				break;

			}
			it++;
		}
	}
	return removedSomething;	// return 'true' if we actually deleted a constraint
	*/
	return false;
}

bool ConstraintCollection::RemoveConstraint(IDTYPE id1, IDTYPE id2)
{
	/*
	unsigned long long constraintID = GenConstraintID(id1, id2);
	ConstraintMapType::iterator it = m_constraints.find(constraintID);
	if (it != m_constraints.end())
	{
		btGeneric6DofConstraint* constraint = m_constraints[constraintID];
 		m_worldData.dynamicsWorld->removeConstraint(constraint);
		m_constraints.erase(it);
		delete constraint;
		return true;
	}
	return false;
	*/
	return false;
}

// one or more objects changed shape/mass. Recompute the constraint transforms.
bool ConstraintCollection::RecalculateAllConstraints(IDTYPE id1)
{
	/*
	bool recalcuatedSomething = false;
	ConstraintMapType::iterator it = m_constraints.begin();
	while (it != m_constraints.end())
	{
		unsigned long long constraintID = it->first;
		// if this constraint contains the passed localID, recalcuate its transforms
		if ((((IDTYPE)(constraintID & 0xffffffff)) == id1)
			|| (((IDTYPE)(constraintID >> 32) & 0xffffffff) == id1))
		{
			btGeneric6DofConstraint* constraint = it->second;
			constraint->calculateTransforms();
			recalcuatedSomething = true;
		}
		it++;
	}
	return recalcuatedSomething;	// return 'true' if we actually recalcuated a constraint
	*/
	return false;
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

