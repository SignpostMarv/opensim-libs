#include "main.h"

extern "C"
{
    /* abstract!
        EXPORT IntPtr BulletAPI_CreateBtTypedConstraint(int type);
        EXPORT IntPtr BulletAPI_CreateBtTypedConstraintA(int type, IntPtr rbA);
        EXPORT IntPtr BulletAPI_CreateBtTypedConstraintAB(int type, IntPtr rbA, IntPtr rbB);
    */
    EXPORT IntPtr BulletAPI_BtTypedConstraint_CreateBtConstraintInfo1(int numConstraintRows, int nub);
    EXPORT IntPtr BulletAPI_BtTypedConstraint_CreateBtConstraintInfo2(float fps, float erp, 
        float m_J1linearAxis,float m_J1angularAxis,float m_J2linearAxis,float m_J2angularAxis,
        int rowskip, float constraintError, float cfm, float lowerLimit, float upperLimit, float index);
    EXPORT void BulletAPI_BtTypedConstraint_getInfo1(IntPtr handle, IntPtr b);
    EXPORT void BulletAPI_BtTypedConstraint_getInfo2(IntPtr handle, IntPtr b);
    EXPORT void BulletAPI_BtTypedConstraint_delete(IntPtr handle);
    EXPORT void BulletAPI_BtTypedConstraint_BtConstraintInfo1_delete(IntPtr handle);
    EXPORT void BulletAPI_BtTypedConstraint_BtConstraintInfo2_delete(IntPtr handle);

}