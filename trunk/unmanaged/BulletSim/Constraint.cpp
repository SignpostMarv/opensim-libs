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
#include "Constraint.h"
#include "IPhysObject.h"
#include "ObjectCollection.h"
#include "BSLogger.h"

Constraint::Constraint(WorldData* world, IDTYPE id1, IDTYPE id2, btTransform& frame1t, btTransform& frame2t)
{
	m_constraint = NULL;
	m_worldData = world;
	m_id1 = id1;
	m_id2 = id2;

	IPhysObject* obj1;
	IPhysObject* obj2;
	if (m_worldData->objects != NULL && m_worldData->objects->TryGetObject(id1, &obj1))
	{
		if (m_worldData->objects->TryGetObject(id2, &obj2))
		{
            // BSLog("AddConstraint: found body1=%d, body2=%d", id1, id2);
			btRigidBody* body1 = obj1->GetBody();
			btRigidBody* body2 = obj2->GetBody();

			m_constraint = new BTCONSTRAINTTYPE(*body1, *body2, frame1t, frame2t, true);

			m_worldData->dynamicsWorld->addConstraint(m_constraint, false);
			m_constraint->calculateTransforms();
			// BSLog("Constraint::Constructor: id1=%u, id2=%u", obj1->GetID(), obj2->GetID());
		}
	}
}

Constraint::~Constraint(void)
{
	// BSLog("Constraint::Destructor: m_constraint=%x, id1=%u, id2=%u", m_constraint, m_id1, m_id2);
	if (m_constraint)
	{
		m_worldData->dynamicsWorld->removeConstraint(m_constraint);
		delete m_constraint;
	}
	m_constraint = NULL;
}

void Constraint::SetLinear(const btVector3& low, const btVector3& high)
{
	if (m_constraint)
	{
		m_constraint->setLinearLowerLimit(low);
		m_constraint->setLinearUpperLimit(high);
	}
}

void Constraint::SetAngular(const btVector3& low, const btVector3& high)
{
	if (m_constraint)
	{
		m_constraint->setAngularLowerLimit(low);
		m_constraint->setAngularUpperLimit(high);
	}
}

void Constraint::UseFrameOffset(bool flag)
{
	if (m_constraint)
	{
		m_constraint->setUseFrameOffset(flag);
	}
}

void Constraint::TranslationalLimitMotor(bool flag, float vel, float force)
{
	if (m_constraint)
	{
		m_constraint->getTranslationalLimitMotor()->m_enableMotor[0] = flag;
		if (flag)
		{
			m_constraint->getTranslationalLimitMotor()->m_targetVelocity[0] = vel;
			m_constraint->getTranslationalLimitMotor()->m_maxMotorForce[0] = force;
		}
	}
}

