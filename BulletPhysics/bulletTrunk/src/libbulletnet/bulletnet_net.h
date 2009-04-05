#include "main.h"

extern "C"
{

    EXPORT IntPtr BulletAPI();
    EXPORT IntPtr BulletAPI_btDefaultMotionState(float x,float y, float z, float w, float x2, float y2, float z2);
    EXPORT btScalar BulletAPI_btScalar(float val);
}