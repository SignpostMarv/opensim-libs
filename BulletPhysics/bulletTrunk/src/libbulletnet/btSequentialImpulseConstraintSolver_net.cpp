#include "btSequentialImpulseConstraintSolver_net.h"

btSequentialImpulseConstraintSolver *GetBtSequentialImpulseConstraintSolverFromIntPtr(IntPtr object)
{
    return (btSequentialImpulseConstraintSolver*) object;
}

IntPtr BulletAPI_CreateBtSequentialImpulseConstraintSolver()
{
    return new btSequentialImpulseConstraintSolver();
}

void BulletAPI_BtSequentialImpulseConstraintSolver_delete(IntPtr obj)
{
    delete GetBtSequentialImpulseConstraintSolverFromIntPtr(obj);
}