#include "btDefaultCollisionConfiguration_net.h"

btDefaultCollisionConfiguration *GetBtDefaultCollisionConfigurationFromIntPtr(IntPtr object)
{
    return (btDefaultCollisionConfiguration*) object;
}

IntPtr BulletAPI_CreateBtDefaultCollisionConfiguration()
{
    return new btDefaultCollisionConfiguration();
}

void BulletAPI_BtDefaultCollisionConfiguration_delete(IntPtr obj)
{
    delete GetBtDefaultCollisionConfigurationFromIntPtr(obj);
}