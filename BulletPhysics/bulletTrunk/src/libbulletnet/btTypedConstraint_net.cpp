#include "btTypedConstraint_net.h"
#include "BulletDynamics/ConstraintSolver/btTypedConstraint.h"

btTypedConstraint *GetBtTypedConstraintFromIntPtr(IntPtr handle)
{
    return (btTypedConstraint *)handle;
}

btTypedConstraint::btConstraintInfo1 *GetBtTypedConstraint_btConstraintInfo1FromIntPtr(IntPtr handle)
{
    return (btTypedConstraint::btConstraintInfo1 *) handle;
}

btTypedConstraint::btConstraintInfo2 *GetBtTypedConstraint_btConstraintInfo2FromIntPtr(IntPtr handle)
{
    return (btTypedConstraint::btConstraintInfo2 *) handle;
}

/*
IntPtr BulletAPI_CreateBtTypedConstraint(int type)
{
    return new btTypedConstraint((btTypedConstraintType) type);
}

IntPtr BulletAPI_CreateBtTypedConstraintA(int type, IntPtr rbA)
{
    return new btTypedConstraint((btTypedConstraintType) type, *(btRigidBody *) rbA);
}

IntPtr BulletAPI_CreateBtTypedConstraintAB(int type, IntPtr rbA, IntPtr rbB)
{
    return new btTypedConstraint((btTypedConstraintType) type, *(btRigidBody *) rbA, *(btRigidBody *) rbB);
}

*/

IntPtr BulletAPI_BtTypedConstraint_CreateBtConstraintInfo1(int numConstraintRows, int nub)
{
    btTypedConstraint::btConstraintInfo1* ret = new btTypedConstraint::btConstraintInfo1();
    ret->m_numConstraintRows=numConstraintRows;
    ret->nub = nub;
    return ret;
}

IntPtr BulletAPI_BtTypedConstraint_CreateBtConstraintInfo1(float fps, float erp, 
        float m_J1linearAxis,float m_J1angularAxis,float m_J2linearAxis,float m_J2angularAxis,
        int rowskip, float constraintError, float cfm, float lowerLimit, float upperLimit, float index)
{
    btTypedConstraint::btConstraintInfo2* ret = new btTypedConstraint::btConstraintInfo2();
    ret->cfm = &cfm;
    ret->fps = fps;
    ret->erp = erp;
    ret->m_J1linearAxis = &m_J1linearAxis;
    ret->m_J1angularAxis = &m_J1angularAxis;
    ret->m_J2linearAxis = &m_J2linearAxis;
    ret->m_J2angularAxis = &m_J2angularAxis;
    ret->m_lowerLimit = &lowerLimit;
    ret->m_upperLimit = &upperLimit;
    ret->m_constraintError = &constraintError;
    ret->rowskip = rowskip;

    return ret;
}

void BulletAPI_BtTypedConstraint_getInfo1(IntPtr handle, IntPtr b)
{
    GetBtTypedConstraintFromIntPtr(handle)->getInfo1((btTypedConstraint::btConstraintInfo1*) b);
}

void BulletAPI_BtTypedConstraint_getInfo2(IntPtr handle, IntPtr b)
{
    GetBtTypedConstraintFromIntPtr(handle)->getInfo2((btTypedConstraint::btConstraintInfo2*) b);
}

void BulletAPI_BtTypedConstraint_delete(IntPtr handle)
{
    delete GetBtTypedConstraintFromIntPtr(handle);
}

void BulletAPI_BtTypedConstraint_BtConstraintInfo1_delete(IntPtr handle)
{
    delete GetBtTypedConstraint_btConstraintInfo1FromIntPtr(handle);
}


void BulletAPI_BtTypedConstraint_BtConstraintInfo2_delete(IntPtr handle)
{
    delete GetBtTypedConstraint_btConstraintInfo2FromIntPtr(handle);
}

