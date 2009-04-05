#include "btGeneric6DofConstraint_net.h"
#include "BulletDynamics/ConstraintSolver/btGeneric6DofConstraint.h"
#include "BulletDynamics/Dynamics/btRigidBody.h"
#include "LinearMath/btTransformUtil.h"
#include "BulletDynamics/ConstraintSolver/btTypedConstraint.h"

btGeneric6DofConstraint *GetBtGeneric6DofConstraintFromIntPtr(IntPtr handle)
{
    return (btGeneric6DofConstraint *) handle;
}

IntPtr BulletAPI_CreateBtGeneric6DofConstraint(IntPtr rbA, IntPtr rbB,IntPtr frameInA, IntPtr frameInB, bool useLinearReferenceFrameA)
{
    return new btGeneric6DofConstraint(*(btRigidBody *)rbA,*(btRigidBody*)rbB, *(btTransform *)frameInA, *(btTransform *)frameInB, useLinearReferenceFrameA);
}

/*
int BulletAPI_BtGeneric6DofConstraint_setAngularLimits(IntPtr handle,IntPtr info, int row_offset) // btConstraintInfo2
{
    return 0;// protected! GetBtGeneric6DofConstraintFromIntPtr(handle)->setAngularLimits((btTypedConstraint::btConstraintInfo2 *) info, row_offset);
}


int BulletAPI_BtGeneric6DofConstraint_setLinearLimits(IntPtr handle,IntPtr info, int row_offset) // btConstraintInfo2
{
    return 0;// protected! GetBtGeneric6DofConstraintFromIntPtr(handle)->setLinearLimits((btTypedConstraint::btConstraintInfo2 *) info, row_offset);
}
*/

void BulletAPI_BtGeneric6DofConstraint_setLimit(IntPtr handle, int axis, float lo, float hi)
{
    GetBtGeneric6DofConstraintFromIntPtr(handle)->setLimit(axis,lo,hi);
}

bool BulletAPI_BtGeneric6DofConstraint_isLimited(IntPtr handle, int limitIndex)
{
    _FIX_BOOL_MARSHAL_BUG(GetBtGeneric6DofConstraintFromIntPtr(handle)->isLimited(limitIndex));
}

void BulletAPI_BtGeneric6DofConstraint_setAngularLowerLimit(IntPtr handle, IntPtr angularLower)// btVector3
{
    GetBtGeneric6DofConstraintFromIntPtr(handle)->setAngularLowerLimit(*(btVector3 *)angularLower);
}

void BulletAPI_BtGeneric6DofConstraint_setAngularUpperLimit(IntPtr handle, IntPtr angularUpper)// btVector3
{
    GetBtGeneric6DofConstraintFromIntPtr(handle)->setAngularUpperLimit(*(btVector3 *)angularUpper);
}

void BulletAPI_BtGeneric6DofConstraint_setLinearLowerLimit(IntPtr handle, IntPtr linearLower)// btVector3
{
    GetBtGeneric6DofConstraintFromIntPtr(handle)->setLinearLowerLimit(*(btVector3 *)linearLower);
}

void BulletAPI_BtGeneric6DofConstraint_setLinearUpperLimit(IntPtr handle, IntPtr linearUpper)// btVector3
{
    GetBtGeneric6DofConstraintFromIntPtr(handle)->setLinearUpperLimit(*(btVector3 *)linearUpper);
}

void BulletAPI_BtGeneric6DofConstraint_delete(IntPtr handle)
{
    delete  GetBtGeneric6DofConstraintFromIntPtr(handle);
}

