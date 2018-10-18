/*************************************************************************
 *                                                                       *
 * Open Dynamics Engine, Copyright (C) 2001,2002 Russell L. Smith.       *
 * All rights reserved.  Email: russ@q12.org   Web: www.q12.org          *
 *                                                                       *
 * This library is free software; you can redistribute it and/or         *
 * modify it under the terms of EITHER:                                  *
 *   (1) The GNU Lesser General Public License as published by the Free  *
 *       Software Foundation; either version 2.1 of the License, or (at  *
 *       your option) any later version. The text of the GNU Lesser      *
 *       General Public License is included with this library in the     *
 *       file LICENSE.TXT.                                               *
 *   (2) The BSD-style license that is included with this library in     *
 *       the file LICENSE-BSD.TXT.                                       *
 *                                                                       *
 * This library is distributed in the hope that it will be useful,       *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the files    *
 * LICENSE.TXT and LICENSE-BSD.TXT for more details.                     *
 *                                                                       *
 *************************************************************************/

/*

some useful collision utility stuff.

*/

#ifndef _ODE_COLLISION_UTIL_H_
#define _ODE_COLLISION_UTIL_H_

#include <ode/common.h>
#include <ode/contact.h>
#include <ode/rotation.h>
#include "odemath.h"


// given a pointer `p' to a dContactGeom, return the dContactGeom at
// p + skip bytes.
#define CONTACT(p,skip) ((dContactGeom*) (((char*)p) + (skip)))

#if 1
#include "collision_kernel.h"
// Fetches a contact
static inline dContactGeom* SAFECONTACT(int Flags, dContactGeom* Contacts, int Index, int Stride){
    dIASSERT(Index >= 0 && Index < (Flags & NUMC_MASK));
    return ((dContactGeom*)(((char*)Contacts) + (Index * Stride)));
}
#endif


// if the spheres (p1,r1) and (p2,r2) collide, set the contact `c' and
// return 1, else return 0.

int dCollideSpheres (dVector3 p1, dReal r1,
                     dVector3 p2, dReal r2, dContactGeom *c);


// given two lines
//    qa = pa + alpha* ua
//    qb = pb + beta * ub
// where pa,pb are two points, ua,ub are two unit length vectors, and alpha,
// beta go from [-inf,inf], return alpha and beta such that qa and qb are
// as close as possible

void dLineClosestApproach (const dVector3 pa, const dVector3 ua,
                           const dVector3 pb, const dVector3 ub,
                           dReal *alpha, dReal *beta);


// given a line segment p1-p2 and a box (center 'c', rotation 'R', side length
// vector 'side'), compute the points of closest approach between the box
// and the line. return these points in 'lret' (the point on the line) and
// 'bret' (the point on the box). if the line actually penetrates the box
// then the solution is not unique, but only one solution will be returned.
// in this case the solution points will coincide.

void dClosestLineBoxPoints (const dVector3 p1, const dVector3 p2,
                            const dVector3 c, const dMatrix3 R,
                            const dVector3 halfside,
                            dVector3 lret, dVector3 bret);

// 20 Apr 2004
// Start code by Nguyen Binh
int dClipEdgeToPlane(dVector3 &vEpnt0, dVector3 &vEpnt1, const dVector4& plPlane);
// clip polygon with plane and generate new polygon points
void dClipPolyToPlane(const dVector3 avArrayIn[], const int ctIn, dVector3 avArrayOut[], int &ctOut, const dVector4 &plPlane );

void dClipPolyToCircle(const dVector3 avArrayIn[], const int ctIn, dVector3 avArrayOut[], int &ctOut, const dVector4 &plPlane ,dReal fRadius);

static inline void dMat3GetCol(const dMatrix3& m,const int col, dVector3& v)
{
    dGetMatrixColumn3(v, m, col);
}

static inline void dVector3CrossMat3Col(const dMatrix3& m,const int col,const dVector3& v,dVector3& r)
{
    dCalcVectorCross3_114(r, v, m + col);
}

static inline void dMat3ColCrossVector3(const dMatrix3& m,const int col,const dVector3& v,dVector3& r)
{
    dCalcVectorCross3_141(r, m + col, v);
}

static inline void dMultiplyMat3Vec3(const dMatrix3& m,const dVector3& v, dVector3& r)
{
    dMultiply0_331(r, m, v);
}

static inline void dConstructPlane(dVector4& plane, const dVector3& normal,const dReal& distance)
{
    dCopyVector3r4(plane, normal);
    plane[3] = distance;
}

inline void dQuatTransform(const dQuaternion& quat,const dVector3& source,dVector3& dest)
{
    dReal x0 = source[0] * quat[0] + source[2] * quat[2] - source[1] * quat[3];
    dReal x1 = source[1] * quat[0] + source[0] * quat[3] - source[2] * quat[1];
    dReal x2 = source[2] * quat[0] + source[1] * quat[1] - source[0] * quat[2];

    dReal c2_x = quat[2] * x2 - quat[3] * x1;
    dReal c2_y = quat[3] * x0 - quat[1] * x2;
    dReal c2_z = quat[1] * x1 - quat[2] * x0;

    dest[0] = source[0] + REAL(2.0) * c2_x;
    dest[1] = source[1] + REAL(2.0) * c2_y;
    dest[2] = source[2] + REAL(2.0) * c2_z;
}

inline void dQuatInvTransform(const dQuaternion& quat,const dVector3& source,dVector3& dest)
{

    dReal norm = dCalcVectorLengthSquare4(quat);

    if (norm > REAL(0.0))
    {
        dQuaternion invQuat;
        norm = 1.0f / sqrtf(norm);
        invQuat[0] =  quat[0] * norm;
        invQuat[1] = -quat[1] * norm;
        invQuat[2] = -quat[2] * norm;
        invQuat[3] = -quat[3] * norm;
        dQuatTransform(invQuat,source,dest);
    }
    else
    {
        // Singular -> return identity
        dCopyVector3r4(dest, source);
    }
}

inline void dGetEulerAngleFromRot(const dMatrix3& mRot,dReal& rX,dReal& rY,dReal& rZ)
{
    rY = (dReal)asin(mRot[0 * 4 + 2]);
    if (rY < M_PI /2)
    {
        if (rY > -M_PI /2)
        {
            rX = (dReal)atan2(-mRot[1*4 + 2], mRot[2*4 + 2]);
            rZ = (dReal)atan2(-mRot[0*4 + 1], mRot[0*4 + 0]);
        }
        else
        {
            // not unique
            rX = (dReal)-atan2(mRot[1*4 + 0], mRot[1*4 + 1]);
            rZ = REAL(0.0);
        }
    }
    else
    {
        // not unique
        rX = (dReal)atan2(mRot[1*4 + 0], mRot[1*4 + 1]);
        rZ = REAL(0.0);
    }
}

inline void dQuatInv(const dQuaternion& source, dQuaternion& dest)
{
    dReal norm = dCalcVectorLengthSquare4(source);

    if (norm > 0.0f)
    {
        norm = 1.0f / sqrtf(norm);
        dest[0] = source[0] * norm;
        dest[1] = -source[1] * norm;
        dest[2] = -source[2] * norm;
        dest[3] = -source[3] * norm;
    }
    else
    {
        // Singular -> return identity
        dest[0] = REAL(1.0);
        dest[1] = REAL(0.0);
        dest[2] = REAL(0.0);
        dest[3] = REAL(0.0);
    }
}

#endif
