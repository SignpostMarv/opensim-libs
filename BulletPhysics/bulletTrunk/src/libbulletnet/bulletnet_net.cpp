#include "bulletnet_net.h"
#include "LinearMath/btDefaultMotionState.h"
#include "LinearMath/btVector3.h"
#include "LinearMath/btMatrix3x3.h"
#include "LinearMath/btTransform.h"
#include "LinearMath/btQuaternion.h"
#include <iostream>

IntPtr BulletAPI()
{
    return (int)0;
}

#ifdef BT_USE_DOUBLE_PRECISION
btScalar BulletAPI_btScalar(double val)
{
    return btScalar(val);
}
#else
btScalar BulletAPI_btScalar(float val)
{
    return btScalar(val);
}
#endif


IntPtr BulletAPI_btDefaultMotionState(float x,float y, float z, float w, float x2, float y2, float z2)
{
    btDefaultMotionState* ms = new btDefaultMotionState(btTransform(btQuaternion(x,y,z,w),btVector3(x2,y2,z2)));
    return ms;
}