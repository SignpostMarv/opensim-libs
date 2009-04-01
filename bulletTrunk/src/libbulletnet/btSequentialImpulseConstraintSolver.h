#include "main.h"


extern "C"
{
    EXPORT IntPtr BulletAPI_CreateBtSequentialImpulseConstraintSolver();
    EXPORT void BulletAPI_BtSequentialImpulseConstraintSolver_delete(IntPtr obj);
}