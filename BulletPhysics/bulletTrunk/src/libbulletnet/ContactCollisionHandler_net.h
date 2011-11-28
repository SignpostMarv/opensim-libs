#include "main.h"

class ContactCollisionHandler;
extern "C"
{
    EXPORT IntPtr BulletHelper_CreateContactCollector(btDiscreteDynamicsWorld* world, int* pinned);
    EXPORT int BulletHelper_FetchDebugValue();
    EXPORT void BulletHelper_FetchContact(IntPtr cch);
    EXPORT void BulletHelper_ClearContacts(IntPtr cch);
    EXPORT void BulletHelper_DestroyContactCollector();
}
