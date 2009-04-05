#include "btAxisSweep3_net.h"

btVector3 *GetBtVector3FromIntPtr_AxSweep(IntPtr object)
{
    return (btVector3*) object;
}

btAxisSweep3 *GetBtAxisSweep3FromIntPtr(IntPtr object)
{
    return (btAxisSweep3*) object;
}

IntPtr BulletAPI_CreateBtAxisSweep3(IntPtr worldAabbMin, IntPtr worldAabbMax, int MaxProxies)
{
    return new btAxisSweep3(*GetBtVector3FromIntPtr_AxSweep(worldAabbMin),*GetBtVector3FromIntPtr_AxSweep(worldAabbMax),(unsigned short)MaxProxies);
}

IntPtr BulletAPI_BtAxisSweep3Get_getOverlappingPairCache(IntPtr obj)
{
    // pointer
    return (GetBtAxisSweep3FromIntPtr(obj))->getOverlappingPairCache();;
}

void BulletAPI_BtAxisSweep3_delete(IntPtr ax)
{
    //btAxisSweep3* obj = GetBtAxisSweep3FromIntPtr(ax);
    //obj->
    delete (GetBtAxisSweep3FromIntPtr(ax));
}