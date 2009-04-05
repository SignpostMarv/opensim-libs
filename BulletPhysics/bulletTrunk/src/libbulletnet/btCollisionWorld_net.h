#include "main.h"

extern "C"
{
    //typedef bool (STDCALL EVENTCALLBACK)(const IntPtr);

    EXPORT void BulletAPI_btCollisionWorld_addCollisionObject(IntPtr handle, IntPtr collisionObject, int collisionFilterGroup, int collisionFilterMask);
    EXPORT IntPtr BulletAPI_btCollisionWorld_getBroadphase(IntPtr handle); //btBroadphaseInterface*
    EXPORT IntPtr BulletAPI_btCollisionWorld_getPairCache(IntPtr handle); //btOverlappingPairCache*
    EXPORT IntPtr BulletAPI_btCollisionWorld_getDispatcher(IntPtr handle); //btDispatcher*	
    EXPORT int BulletAPI_btCollisionWorld_getNumCollisionObjects(IntPtr handle);
    EXPORT void BulletAPI_btCollisionWorld_rayTest(IntPtr handle, IntPtr rayFromWorld, IntPtr rayToWorld, IntPtr resultCallback);
    EXPORT void BulletAPI_btCollisionWorld_removeCollisionObject(IntPtr handle, IntPtr collisionObject);
}