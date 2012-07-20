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

#ifndef OBJECT_COLLECTION_H
#define OBJECT_COLLECTION_H

#include "ArchStuff.h"
#include "IPhysObject.h"

#include <map>

class ObjectCollection
{
public:
	ObjectCollection(void);
	~ObjectCollection(void);

	// Remove and destroy all objects in the collection
	void Clear();

	// Add an object to collection.
	// Return 'true' if successfully added.
	bool AddObject(IDTYPE id, IPhysObject* obj);

	// Fetch the object
	// Return null if the object isn't there
	IPhysObject* GetObject(IDTYPE id);

	// Fetch the object.
	// Return true of the object was found.
	bool TryGetObject(IDTYPE id, IPhysObject** obj);

	// Return 'true' if the object is in the collection
	bool HasObject(IDTYPE id);

	// Remove the object from the collection and return the object.
	IPhysObject* RemoveObject(IDTYPE id);

	// Remove the obejct from the collection and delete the object.
	// Return 'true' if successfully removed and deleted.
	bool RemoveAndDestroyObject(IDTYPE id);

private:
	typedef std::map<IDTYPE, IPhysObject*> ObjectMapType;
	ObjectMapType m_objects;
};

#endif     // OBJECT_COLLECTION_H