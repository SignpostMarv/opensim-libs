#include "main.h"

extern "C"
{
    //rbA,rbB - Rigid Bodies
    // frameInA FramInB - btTransform
    EXPORT IntPtr BulletAPI_CreateBtGeneric6DofConstraint(IntPtr rbA, IntPtr rbB,IntPtr  frameInA, IntPtr frameInB, bool useLinearReferenceFrameA);
/*
    EXPORT int BulletAPI_BtGeneric6DofConstraint_setAngularLimits(IntPtr handle,IntPtr info, int row_offset); // btConstraintInfo2
    EXPORT int BulletAPI_BtGeneric6DofConstraint_setLinearLimits(IntPtr handle,IntPtr info, int row_offset); // btConstraintInfo2
*/
    EXPORT void BulletAPI_BtGeneric6DofConstraint_setLimits(IntPtr handle, int axis, float lo, float hi);
    EXPORT bool BulletAPI_BtGeneric6DofConstraint_isLimited(IntPtr handle, int limitIndex);
    EXPORT void BulletAPI_BtGeneric6DofConstraint_setAngularLowerLimit(IntPtr handle, IntPtr angularLower);// btVector3
    EXPORT void BulletAPI_BtGeneric6DofConstraint_setAngularUpperLimit(IntPtr handle, IntPtr angularUpper);// btVector3
    EXPORT void BulletAPI_BtGeneric6DofConstraint_setLinearLowerLimit(IntPtr handle, IntPtr linearLower);// btVector3
    EXPORT void BulletAPI_BtGeneric6DofConstraint_setLinearUpperLimit(IntPtr handle, IntPtr linearUpper);// btVector3
    EXPORT void BulletAPI_BtGeneric6DofConstraint_delete(IntPtr handle);

}