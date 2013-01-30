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

#ifndef DEBUG_LOGIC_H
#define DEBUG_LOGIC_H

// Conditional compiled system for tracking the creation, reference and destruction
//    of Bullet objects.
// TODO: Implement if needed.
#ifndef BSDEBUG

#define bsDebug_Initialize()
#define bsDebug_AllDone()

#define bsDebug_RememberCollisionObject(obj)
#define bsDebug_ForgetCollisionObject(obj)
#define bsDebug_AssertIsKnownCollisionObject(obj,msg)
#define bsDebug_AssertCollisionObjectIsInWorld(w,obj,msg)
#define bsDebug_AssertCollisionObjectIsNotInWorld(world,obj,msg)

#define bsDebug_RememberCollisionShape(shape)
#define bsDebug_ForgetCollisionShape(shape)
#define bsDebug_AssertIsKnownCollisionShape(shape,msg)

#define bsDebug_RememberConstraint(constrain)
#define bsDebug_ForgetConstraint(constrain)
#define bsDebug_AssertIsKnownConstraint(constrain,msg)
#define bsDebug_AssertNoExistingConstraint(o1, o2, msg)
#define bsDebug_AssertConstraintIsInWorld(world,constrain,msg)
#define bsDebug_AssertConstraintIsNotInWorld(world,constrain,msg)


#else	// BSDEBUG

#include <set>

std::set<btCollisionObject*> bsDebug_collisionObjects;

#endif	// BSDEBUG

#endif	// DEBUG_LOGIC_H
