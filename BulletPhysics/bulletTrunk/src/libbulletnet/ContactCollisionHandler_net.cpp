#include "ContactCollisionHandler_net.h"
#include <hash_map>

struct ContactInfo {
	IntPtr contact;
	IntPtr contactWith;
	float pX;	// contact position
	float pY;
	float pZ;
	float nX;	// contact normal
	float nY;
	float nZ;
	float depth;	// contact depth
};

// Each region invocation of the physics engine will need it's own collision
// data structures. The BulletDotNET routine takes the pointer to an instance
// of this class and passed it back for our use.
class ContactCollisionHandler {
public:
	// list built during sub-ticks of collisions that occured
	stdext::hash_map<btCollisionObject*, ContactInfo> m_collisionsList;
	// pointer to pinned memory used to pass the collisions back to managed code
	int* m_collisionsPinned;
};

ContactCollisionHandler* getContactCollisionHandlerFromIntPtr(IntPtr object) 
{
	return (ContactCollisionHandler*) object;
}

void interTickCallback(btDynamicsWorld* world, btScalar timeStep);

IntPtr BulletHelper_CreateContactCollector(btDiscreteDynamicsWorld* world, int* pinnedMem) {
	ContactCollisionHandler* m_cch;
	// insert ourself into simulation tick
	world->setInternalTickCallback(interTickCallback);
	// create an instance for our local variables. This is passed back in most functions.
	m_cch = new ContactCollisionHandler();
	m_cch->m_collisionsPinned = pinnedMem;
	m_cch->m_collisionsList.clear();
	// hide pointer in world structure so interTickCallback can find our data
	world->setWorldUserInfo((void*)m_cch);
	return m_cch;
}

static void interTickCallback(btDynamicsWorld* world, btScalar timeStep) {
	ContactCollisionHandler* cch = (ContactCollisionHandler*)world->getWorldUserInfo();
	int numManifolds = world->getDispatcher()->getNumManifolds();
	for (int ii=0; ii < numManifolds; ii++) {
		btPersistentManifold* contactManifold = world->getDispatcher()->getManifoldByIndexInternal(ii);
		btCollisionObject* obA = static_cast<btCollisionObject*>(contactManifold->getBody0());
		btCollisionObject* obB = static_cast<btCollisionObject*>(contactManifold->getBody1());

		int numContacts = contactManifold->getNumContacts();
		if (numContacts > 0) {
			// if this object is not in our list of contacted objects, add this contact
			if (cch->m_collisionsList.find(obA) == cch->m_collisionsList.end()) {
				btVector3 contact(0.f, 0.f, 0.f);
				btVector3 normal(1.f, 0.f, 0.f);
				btScalar depth = 0.f;
				for (int jj=0; jj < numContacts; jj++) {
					btManifoldPoint& pt = contactManifold->getContactPoint(jj);
					if (pt.getDistance() < 0.f) {
						contact = pt.getPositionWorldOnA();
						normal = pt.m_normalWorldOnB;
						depth = pt.getDistance();
						break;
					}
				}
				ContactInfo ci;
				ci.contact = obA->getUserPointer();
				ci.contactWith = obB->getUserPointer();
				ci.pX = contact.getX();
				ci.pY = contact.getY();
				ci.pZ = contact.getZ();
				ci.nX = normal.getX();
				ci.nY = normal.getY();
				ci.nZ = normal.getZ();
				ci.depth = (float)depth;
				cch->m_collisionsList.insert(std::pair<btCollisionObject*, ContactInfo>(obA, ci));
			}
		}
	}
	return;
}

int BulletHelper_FetchDebugValue() {
	return 0;
}

void BulletHelper_FetchContact(IntPtr ip_cch) {
	ContactCollisionHandler* cch = getContactCollisionHandlerFromIntPtr(ip_cch);
	int max = cch->m_collisionsPinned[0];		// max num of entries in the pinned array
	cch->m_collisionsPinned[1] = 0;
	int stride = 9;
	int offset = 3;
	int cnt = 0;								// counts number passed back
	stdext::hash_map<btCollisionObject*, ContactInfo>::const_iterator iter = cch->m_collisionsList.begin();
	while ((max > offset) && iter != cch->m_collisionsList.end()) {
		ContactInfo ci = iter->second;
		cch->m_collisionsPinned[offset + 0] = (int)ci.contact;
		cch->m_collisionsPinned[offset + 1] = (int)ci.contactWith;
		cch->m_collisionsPinned[offset + 2] = (int)(ci.pX * 1000);
		cch->m_collisionsPinned[offset + 3] = (int)(ci.pY * 1000);
		cch->m_collisionsPinned[offset + 4] = (int)(ci.pZ * 1000);
		cch->m_collisionsPinned[offset + 5] = (int)(ci.nX * 1000);
		cch->m_collisionsPinned[offset + 6] = (int)(ci.nY * 1000);
		cch->m_collisionsPinned[offset + 7] = (int)(ci.nZ * 1000);
		cch->m_collisionsPinned[offset + 8] = (int)(ci.depth * 1000);
		cnt++;
		offset += stride;
		iter++;
	}
	cch->m_collisionsPinned[1] = cnt;
	cch->m_collisionsPinned[2] = stride;
	return;
}

void BulletHelper_ClearContacts(IntPtr ip_cch) {
	ContactCollisionHandler* cch = getContactCollisionHandlerFromIntPtr(ip_cch);
	cch->m_collisionsList.clear();
	return;
}

void BulletHelper_DestroyContactCollector() {
	return;
}
