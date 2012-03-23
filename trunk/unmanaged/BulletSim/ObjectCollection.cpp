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
#include "ObjectCollection.h"

ObjectCollection::ObjectCollection(void)
{
}

ObjectCollection::~ObjectCollection(void)
{
	// Clean out the collection if it already hasn't been cleaned out
	Clear();
}

void ObjectCollection::Clear()
{
	// Delete all the objects in the object list
	for (ObjectMapType::const_iterator it = m_objects.begin(); it != m_objects.end(); ++it)
    {
		IPhysObject* obj = it->second;
		delete obj;
	}

	m_objects.clear();
}

bool ObjectCollection::AddObject(IDTYPE id, IPhysObject* obj)
{
	m_objects[id] = obj;
	return true;
}

bool ObjectCollection::TryGetObject(IDTYPE id, IPhysObject** objp)
{
	bool ret = false;
	ObjectMapType::iterator it = m_objects.find(id);
	if (it != m_objects.end())
    {
		*objp = it->second;
		ret = true;
	}
	return ret;
}

bool ObjectCollection::HasObject(IDTYPE id)
{
	IPhysObject* obj;
	return TryGetObject(id, &obj);
}

IPhysObject* ObjectCollection::RemoveObject(IDTYPE id)
{
	IPhysObject* obj = NULL;
	ObjectMapType::iterator it = m_objects.find(id);
	if (it != m_objects.end())
    {
		obj = it->second;
		m_objects.erase(it);
	}
	return obj;
}

bool ObjectCollection::RemoveAndDestroyObject(IDTYPE id)
{
	bool ret = false;
	IPhysObject* obj = RemoveObject(id);
	if (obj != NULL)
	{
		delete obj;
		ret = true;
	}
	return ret;
}