#include "main.h"

extern "C"
{
    EXPORT IntPtr BulletAPI_CreateBtKinematicCharacterController();
    EXPORT int BulletAPI_BtKinematicCharacterController_getNumOverlappingObjects(IntPtr handle);
    EXPORT IntPtr BulletAPI_BtKinematicCharacterController_getOverlappingObject(IntPtr handle);
    EXPORT void BulletAPI_BtKinematicCharacterController_delete(IntPtr handle);
}