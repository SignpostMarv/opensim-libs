#include "main.h"


extern "C"
{
    EXPORT IntPtr BulletAPI_CreateBtAxisSweep3(IntPtr worldAabbMin, IntPtr worldAabbMax, int MaxProxies);
    EXPORT IntPtr BulletAPI_BtAxisSweep3Get_getOverlappingPairCache(IntPtr obj);
    EXPORT void BulletAPI_BtAxisSweep3_delete(IntPtr ax);
}