#include "main.h"


extern "C"
{
    EXPORT IntPtr BulletAPI_CreateBtDefaultCollisionConfiguration();
    EXPORT void BulletAPI_BtDefaultCollisionConfiguration_delete(IntPtr obj);
}