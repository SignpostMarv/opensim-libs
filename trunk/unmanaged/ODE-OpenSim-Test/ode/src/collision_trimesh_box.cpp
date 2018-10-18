/*************************************************************************
 *                                                                       *
 * Open Dynamics Engine, Copyright (C) 2001-2003 Russell L. Smith.       *
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


 /*************************************************************************
  *                                                                       *
  * Triangle-box collider by Alen Ladavac and Vedran Klanac.              *
  * Ported to ODE by Oskari Nyman.                                        *
  *                                                                       *
  *************************************************************************/

#include <ode/collision.h>
#include <ode/rotation.h>
#include "config.h"
#include "matrix.h"
#include "odemath.h"
#include "collision_util.h"
#include "collision_trimesh_internal.h"

static void
GenerateContact(int in_Flags, dContactGeom* in_Contacts, int in_Stride,
    dxGeom* in_g1, dxGeom* in_g2, int TriIndex,
    const dVector3 in_ContactPos, const dVector3 in_Normal, dReal in_Depth,
    int& OutTriCount);


// largest number, double or float
#if defined(dSINGLE)
#define MAXVALUE FLT_MAX
#else
#define MAXVALUE DBL_MAX
#endif

struct sTrimeshBoxColliderData
{
    sTrimeshBoxColliderData() : m_iBestAxis(0), m_iExitAxis(0), m_ctContacts(0) {}

    void SetupInitialContext(dxTriMesh *TriMesh, dxGeom *BoxGeom,
        int Flags, dContactGeom* Contacts, int Stride);
    int TestCollisionForSingleTriangle(int ctContacts0, int Triint,
        dVector3 dv[3], bool &bOutFinishSearching);

    bool _cldTestNormal(dReal fp0, dReal fR, dVector3 vNormal, int iAxis);
    bool _cldTestFace(dReal fp0, dReal fp1, dReal fp2, dReal fR, dReal fD,
        dVector3 vNormal, int iAxis);
    bool _cldTestEdge(dReal fp0, dReal fp1, dReal fR, dReal fD,
        dVector3 vNormal, int iAxis);
    bool _cldTestSeparatingAxes(const dVector3 &v0, const dVector3 &v1, const dVector3 &v2);
    void _cldClipping(const dVector3 &v0, const dVector3 &v1, const dVector3 &v2, int TriIndex);
    void _cldTestOneTriangle(const dVector3 &v0, const dVector3 &v1, const dVector3 &v2, int TriIndex);

    // box data
    dMatrix3 m_mHullBoxRot;
    dVector3 m_vHullBoxPos;
    dVector3 m_vBoxHalfSize;

    // mesh data
    dVector3   m_vHullDstPos;

    // global collider data
    dVector3 m_vBestNormal;
    dReal    m_fBestDepth;
    int    m_iBestAxis;
    int    m_iExitAxis;
    dVector3 m_vE0, m_vE1, m_vE2, m_vN;

    // global info for contact creation
    int m_iFlags;
    dContactGeom *m_ContactGeoms;
    int m_iStride;
    dxGeom *m_Geom1;
    dxGeom *m_Geom2;
    int m_ctContacts;
};

// Test normal of mesh face as separating axis for intersection
bool sTrimeshBoxColliderData::_cldTestNormal(dReal fp0, dReal fR, dVector3 vNormal, int iAxis)
{
    // calculate overlapping interval of box and triangle
    dReal fDepth = fR + fp0;

    // if we do not overlap
    if (fDepth < 0) {
        // do nothing
        return false;
    }

    // calculate normal's length
    dReal fLength = dCalcVectorLengthSquare3(vNormal);
    // if long enough
    if (fLength > 0.0f) {

        dReal fOneOverLength = dRecipSqrt(fLength);
        // normalize depth
        fDepth = fDepth * fOneOverLength;

        // get minimum depth
        if (fDepth < m_fBestDepth)
        {
            dCopyScaledVector3r4(m_vBestNormal, vNormal, -fOneOverLength);
            m_iBestAxis = iAxis;
            //dAASSERT(fDepth>=0);
            m_fBestDepth = fDepth;
        }
    }

    return true;
}

// Test box axis as separating axis
bool sTrimeshBoxColliderData::_cldTestFace(dReal fp0, dReal fp1, dReal fp2, dReal fR, dReal fD,
    dVector3 vNormal, int iAxis)
{
    dReal fMin, fMax;

    // find min of triangle interval
    fMin = fp0 < fp1 ? (fp0 < fp2 ? fp0 : fp2) : (fp1 < fp2 ? fp1 : fp2);

    // find max of triangle interval
    fMax = fp0 > fp1 ? (fp0 > fp2 ? fp0 : fp2) : (fp1 > fp2 ? fp1 : fp2);

    // calculate minimum and maximum depth
    dReal fDepthMin = fR - fMin;
    dReal fDepthMax = fMax + fR;

    // if we dont't have overlapping interval
    if (fDepthMin < 0 || fDepthMax < 0) {
        // do nothing
        return false;
    }

    dReal fDepth = 0;

    // if greater depth is on negative side
    if (fDepthMin > fDepthMax)
    {
        // use smaller depth (one from positive side)
        fDepth = fDepthMax;
        // flip normal direction
        dNegateVector3r4(vNormal);
        fD = -fD;
        // if greater depth is on positive side
    }
    else
    {
        // use smaller depth (one from negative side)
        fDepth = fDepthMin;
    }

    // if lower depth than best found so far
    if (fDepth < m_fBestDepth)
    {
        // remember current axis as best axis
        dCopyVector3r4(m_vBestNormal, vNormal);
        m_iBestAxis = iAxis;
        //dAASSERT(fDepth>=0);
        m_fBestDepth = fDepth;
    }

    return true;
}

// Test cross products of box axis and triangle edges as separating axis
bool sTrimeshBoxColliderData::_cldTestEdge(dReal fp0, dReal fp1, dReal fR, dReal fD,
    dVector3 vNormal, int iAxis)
{
    dReal fMin, fMax;

    dReal fLength = dCalcVectorLengthSquare3(vNormal);
    if (fLength <= dEpsilon) /// THIS NORMAL WOULD BE DANGEROUS
        return true;

    // calculate min and max interval values
    if (fp0 < fp1)
    {
        fMin = fp0;
        fMax = fp1;
    }
    else
    {
        fMin = fp1;
        fMax = fp0;
    }

    // check if we overlapp
    dReal fDepthMin = fR - fMin;
    dReal fDepthMax = fMax + fR;

    // if we don't overlapp
    if (fDepthMin < 0 || fDepthMax < 0) {
        // do nothing
        return false;
    }

    dReal fDepth;

    // if greater depth is on negative side
    if (fDepthMin > fDepthMax) {
        // use smaller depth (one from positive side)
        fDepth = fDepthMax;
        // flip normal direction
        dNegateVector3r4(vNormal);
        fD = -fD;
        // if greater depth is on positive side
    }
    else
    {
        // use smaller depth (one from negative side)
        fDepth = fDepthMin;
    }

    // calculate normal's length

    // normalize depth
    dReal fOneOverLength = dRecipSqrt(fLength);
    fDepth = fDepth * fOneOverLength;
    fD *= fOneOverLength;

    // if lower depth than best found so far (favor face over edges)
    if (fDepth * 1.5f < m_fBestDepth)
    {
        // remember current axis as best axis
        dCopyScaledVector3r4(m_vBestNormal, vNormal, fOneOverLength);
        m_iBestAxis = iAxis;
        //dAASSERT(fDepth>=0);
        m_fBestDepth = fDepth;
    }
    return true;
}


// clip polygon with plane and generate new polygon points
static void _cldClipPolyToPlane(dVector3 avArrayIn[], int ctIn,
    dVector3 avArrayOut[], int &ctOut,
    const dVector4 plPlane)
{

    dVector3 *v0;
    dVector3 *v1;
    dReal fDistance0;
    dReal fDistance1;
    // start with no output points
    ctOut = 0;

    v0 = &avArrayIn[ctIn - 1];
    fDistance0 = dCalcPointPlaneDistance(*v0, plPlane);

    // for each edge in input polygon
    for (int i1 = 0; i1 < ctIn; i1++)
    {
        v1 = &avArrayIn[i1];
        // calculate distance of edge points to plane
        fDistance1 = dCalcPointPlaneDistance(*v1, plPlane);

        // if first point is in front of plane
        if (fDistance0 == 0)
        {
            // emit point
            dCopyVector3r4(avArrayOut[ctOut], *v0);
            ctOut++;
        }
        else if (fDistance0 > 0)
        {
            // emit point
            dCopyVector3r4(avArrayOut[ctOut], *v0);
            ctOut++;

            if (fDistance1 < 0)
            {
                dReal fd = fDistance0 / (fDistance0 - fDistance1);
                dCalcLerpVectors3r4(avArrayOut[ctOut], *v0, *v1, fd);
                ctOut++;
            }
        }
        else if (fDistance1 > 0)
        {
            // find intersection point of edge and plane
            dReal fd = fDistance0 / (fDistance0 - fDistance1);
            dCalcLerpVectors3r4(avArrayOut[ctOut], *v0, *v1, fd);
            ctOut++;
        }
        v0 = v1;
        fDistance0 = fDistance1;
    }
}

bool sTrimeshBoxColliderData::_cldTestSeparatingAxes(const dVector3 &v0, const dVector3 &v1, const dVector3 &v2) {
    // reset best axis
    m_iBestAxis = 0;
    m_iExitAxis = -1;
    m_fBestDepth = MAXVALUE;

    // calculate edges
    dSubtractVectors3r4(m_vE0, v1, v0);
    dSubtractVectors3r4(m_vE1, v2, v0);
    dSubtractVectors3r4(m_vE2, m_vE1, m_vE0);

    // calculate poly normal
    dCalcVectorCross3r4(m_vN, m_vE0, m_vE1);

    // calculate length of face normal
    dReal fNLen = dCalcVectorLengthSquare3(m_vN);

    // Even though all triangles might be initially valid, 
    // a triangle may degenerate into a segment after applying 
    // space transformation.
    if (!fNLen) {
        return false;
    }

    fNLen = dSqrt(fNLen);

    // extract box axes as vectors
    dVector3 vA0, vA1, vA2;
    dGetMatrixColumn3(vA0, m_mHullBoxRot, 0);
    dGetMatrixColumn3(vA1, m_mHullBoxRot, 1);
    dGetMatrixColumn3(vA2, m_mHullBoxRot, 2);

    // box halfsizes
    dReal fa0 = m_vBoxHalfSize[0];
    dReal fa1 = m_vBoxHalfSize[1];
    dReal fa2 = m_vBoxHalfSize[2];

    // calculate relative position between box and triangle
    dVector3 vD;
    dSubtractVectors3r4(vD, v0, m_vHullBoxPos);

    dVector3 vL;
    dReal fp0, fp1, fp2, fR, fD;

    // Test separating axes for intersection
    // ************************************************
    fp0 = dCalcVectorDot3(m_vN, vD);
    fp1 = fp0;
    fp2 = fp0;
    fR = fa0 * dFabs(dCalcVectorDot3(m_vN, vA0)) + fa1 * dFabs(dCalcVectorDot3(m_vN, vA1)) + fa2 * dFabs(dCalcVectorDot3(m_vN, vA2));

    if (!_cldTestNormal(fp0, fR, m_vN, 1))
    {
        m_iExitAxis = 1;
        return false;
    }

    dReal invfNLen = dRecip(fNLen);
    dVector3 m_vN_norm;

    dCopyScaledVector3r4(m_vN_norm, m_vN, invfNLen);

    // ************************************************

    // Test Faces
    // ************************************************
    // Axis 2 - Box X-Axis
    fD = dCalcVectorDot3(vA0, m_vN_norm);
    fp0 = dCalcVectorDot3(vA0, vD);
    fp1 = fp0 + dCalcVectorDot3(vA0, m_vE0);
    fp2 = fp0 + dCalcVectorDot3(vA0, m_vE1);
    fR = fa0;

    if (!_cldTestFace(fp0, fp1, fp2, fR, fD, vA0, 2))
    {
        m_iExitAxis = 2;
        return false;
    }
    // ************************************************

    // ************************************************
    // Axis 3 - Box Y-Axis
    fD = dCalcVectorDot3(vA1, m_vN_norm);
    fp0 = dCalcVectorDot3(vA1, vD);
    fp1 = fp0 + dCalcVectorDot3(vA1, m_vE0);
    fp2 = fp0 + dCalcVectorDot3(vA1, m_vE1);
    fR = fa1;

    if (!_cldTestFace(fp0, fp1, fp2, fR, fD, vA1, 3))
    {
        m_iExitAxis = 3;
        return false;
    }

    // ************************************************
    // Axis 4 - Box Z-Axis
    fD = dCalcVectorDot3(vA2, m_vN_norm);
    fp0 = dCalcVectorDot3(vA2, vD);
    fp1 = fp0 + dCalcVectorDot3(vA2, m_vE0);
    fp2 = fp0 + dCalcVectorDot3(vA2, m_vE1);
    fR = fa2;

    if (!_cldTestFace(fp0, fp1, fp2, fR, fD, vA2, 4))
    {
        m_iExitAxis = 4;
        return false;
    }

    // ************************************************

    // Test Edges
    // ************************************************
    // Axis 5 - Box X-Axis cross Edge0
    dCalcVectorCross3r4(vL, vA0, m_vE0);
    fD = dCalcVectorDot3(vL, m_vN_norm);
    fp0 = dCalcVectorDot3(vL, vD);
    fp1 = fp0;
    fp2 = fp0 + dCalcVectorDot3(vA0, m_vN);
    fR = fa1 * dFabs(dCalcVectorDot3(vA2, m_vE0)) + fa2 * dFabs(dCalcVectorDot3(vA1, m_vE0));

    if (!_cldTestEdge(fp1, fp2, fR, fD, vL, 5))
    {
        m_iExitAxis = 5;
        return false;
    }

    // ************************************************
    // Axis 6 - Box X-Axis cross Edge1
    dCalcVectorCross3r4(vL, vA0, m_vE1);
    fD = dCalcVectorDot3(vL, m_vN_norm);
    fp0 = dCalcVectorDot3(vL, vD);
    fp1 = fp0 - dCalcVectorDot3(vA0, m_vN);
    fp2 = fp0;
    fR = fa1 * dFabs(dCalcVectorDot3(vA2, m_vE1)) + fa2 * dFabs(dCalcVectorDot3(vA1, m_vE1));

    if (!_cldTestEdge(fp0, fp1, fR, fD, vL, 6))
    {
        m_iExitAxis = 6;
        return false;
    }
    // ************************************************

    // ************************************************
    // Axis 7 - Box X-Axis cross Edge2
    dCalcVectorCross3r4(vL, vA0, m_vE2);
    fD = dCalcVectorDot3(vL, m_vN_norm);
    fp0 = dCalcVectorDot3(vL, vD);
    fp1 = fp0 - dCalcVectorDot3(vA0, m_vN);
    fp2 = fp0 - dCalcVectorDot3(vA0, m_vN);
    fR = fa1 * dFabs(dCalcVectorDot3(vA2, m_vE2)) + fa2 * dFabs(dCalcVectorDot3(vA1, m_vE2));

    if (!_cldTestEdge(fp0, fp1, fR, fD, vL, 7))
    {
        m_iExitAxis = 7;
        return false;
}

    // ************************************************
    // Axis 8 - Box Y-Axis cross Edge0
    dCalcVectorCross3r4(vL, vA1, m_vE0);
    fD = dCalcVectorDot3(vL, m_vN_norm);
    fp0 = dCalcVectorDot3(vL, vD);
    fp1 = fp0;
    fp2 = fp0 + dCalcVectorDot3(vA1, m_vN);
    fR = fa0 * dFabs(dCalcVectorDot3(vA2, m_vE0)) + fa2 * dFabs(dCalcVectorDot3(vA0, m_vE0));

    if (!_cldTestEdge(fp0, fp2, fR, fD, vL, 8))
    {
        m_iExitAxis = 8;
        return false;
    }

    // ************************************************

    // ************************************************
    // Axis 9 - Box Y-Axis cross Edge1
    dCalcVectorCross3r4(vL, vA1, m_vE1);
    fD = dCalcVectorDot3(vL, m_vN_norm);
    fp0 = dCalcVectorDot3(vL, vD);
    fp1 = fp0 - dCalcVectorDot3(vA1, m_vN);
    fp2 = fp0;
    fR = fa0 * dFabs(dCalcVectorDot3(vA2, m_vE1)) + fa2 * dFabs(dCalcVectorDot3(vA0, m_vE1));

    if (!_cldTestEdge(fp0, fp1, fR, fD, vL, 9))
    {
        m_iExitAxis = 9;
        return false;
    }

    // ************************************************
    // Axis 10 - Box Y-Axis cross Edge2
    dCalcVectorCross3r4(vL, vA1, m_vE2);
    fD = dCalcVectorDot3(vL, m_vN_norm);
    fp0 = dCalcVectorDot3(vL, vD);
    fp1 = fp0 - dCalcVectorDot3(vA1, m_vN);
    fp2 = fp0 - dCalcVectorDot3(vA1, m_vN);
    fR = fa0 * dFabs(dCalcVectorDot3(vA2, m_vE2)) + fa2 * dFabs(dCalcVectorDot3(vA0, m_vE2));

    if (!_cldTestEdge(fp0, fp1, fR, fD, vL, 10))
    {
        m_iExitAxis = 10;
        return false;
    }

    // ************************************************
    // Axis 11 - Box Z-Axis cross Edge0
    dCalcVectorCross3r4(vL, vA2, m_vE0);
    fD = dCalcVectorDot3(vL, m_vN_norm);
    fp0 = dCalcVectorDot3(vL, vD);
    fp1 = fp0;
    fp2 = fp0 + dCalcVectorDot3(vA2, m_vN);
    fR = fa0 * dFabs(dCalcVectorDot3(vA1, m_vE0)) + fa1 * dFabs(dCalcVectorDot3(vA0, m_vE0));

    if (!_cldTestEdge(fp0, fp2, fR, fD, vL, 11))
    {
        m_iExitAxis = 11;
        return false;
    }
    // ************************************************

    // ************************************************
    // Axis 12 - Box Z-Axis cross Edge1
    dCalcVectorCross3r4(vL, vA2, m_vE1);
    fD = dCalcVectorDot3(vL, m_vN_norm);
    fp0 = dCalcVectorDot3(vL, vD);
    fp1 = fp0 - dCalcVectorDot3(vA2, m_vN);
    fp2 = fp0;
    fR = fa0 * dFabs(dCalcVectorDot3(vA1, m_vE1)) + fa1 * dFabs(dCalcVectorDot3(vA0, m_vE1));

    if (!_cldTestEdge(fp0, fp1, fR, fD, vL, 12))
    {
        m_iExitAxis = 12;
        return false;
    }
    // ************************************************

    // ************************************************
    // Axis 13 - Box Z-Axis cross Edge2
    dCalcVectorCross3r4(vL, vA2, m_vE2);
    fD = dCalcVectorDot3(vL, m_vN_norm);
    fp0 = dCalcVectorDot3(vL, vD);
    fp1 = fp0 - dCalcVectorDot3(vA2, m_vN);
    fp2 = fp0 - dCalcVectorDot3(vA2, m_vN);
    fR = fa0 * dFabs(dCalcVectorDot3(vA1, m_vE2)) + fa1 * dFabs(dCalcVectorDot3(vA0, m_vE2));

    if (!_cldTestEdge(fp0, fp1, fR, fD, vL, 13))
    {
        m_iExitAxis = 13;
        return false;
    }


    // ************************************************
    return true;
}

// find two closest points on two lines
static bool _cldClosestPointOnTwoLines(
    dVector3 vPoint1, dVector3 vLenVec1, dVector3 vPoint2, dVector3 vLenVec2,
    dReal &fvalue1, dReal &fvalue2)
{
    // calculate denominator
    dVector3 vp;
    dSubtractVectors3r4(vp, vPoint2, vPoint1);
    dReal fuaub = dCalcVectorDot3(vLenVec1, vLenVec2);
    dReal fq1 = dCalcVectorDot3(vLenVec1, vp);
    dReal fq2 = -dCalcVectorDot3(vLenVec2, vp);
    dReal fd = 1.0f - fuaub * fuaub;

    // if denominator is positive
    if (fd > 0.0f) {
        // calculate points of closest approach
        fd = 1.0f / fd;
        fvalue1 = (fq1 + fuaub * fq2) * fd;
        fvalue2 = (fuaub * fq1 + fq2) * fd;
        return true;
        // otherwise
    }
    else {
        // lines are parallel
        fvalue1 = 0.0f;
        fvalue2 = 0.0f;
        return false;
    }
}

// clip and generate contacts
void sTrimeshBoxColliderData::_cldClipping(const dVector3 &v0, const dVector3 &v1, const dVector3 &v2, int TriIndex) {
    dIASSERT(!(m_iFlags & CONTACTS_UNIMPORTANT) || m_ctContacts < (m_iFlags & NUMC_MASK)); // Do not call the function if there is no room to store results

    // if we have edge/edge intersection
    if (m_iBestAxis > 4)
    {
        dVector3 vub, vPb, vPa;

        dCopyVector3r4(vPa, m_vHullBoxPos);

        // calculate point on box edge
        for (int i = 0; i < 3; i++)
        {
            dVector3 vRotCol;
            dGetMatrixColumn3(vRotCol, m_mHullBoxRot, i);
            dReal fSign = dCalcVectorDot3(m_vBestNormal, vRotCol) > 0 ? m_vBoxHalfSize[i] : -m_vBoxHalfSize[i];
            dAddScaledVector3r4(vPa, vRotCol, fSign);
        }

        int iEdge = (m_iBestAxis - 5) % 3;

        // decide which edge is on triangle
        if (iEdge == 0)
        {
            dCopyVector3r4(vPb, v0);
            dCopyVector3r4(vub, m_vE0);
        }
        else if (iEdge == 1)
        {
            dCopyVector3r4(vPb, v2);
            dCopyVector3r4(vub, m_vE1);
        }
        else
        {
            dCopyVector3r4(vPb, v1);
            dCopyVector3r4(vub, m_vE2);
        }

        // setup direction parameter for face edge
        dNormalize3(vub);

        dReal fParam1, fParam2;

        // setup direction parameter for box edge
        dVector3 vua;
        int col = (m_iBestAxis - 5) / 3;
        dGetMatrixColumn3(vua, m_mHullBoxRot, col);

        // find two closest points on both edges
        _cldClosestPointOnTwoLines(vPa, vua, vPb, vub, fParam1, fParam2);
        dAddScaledVector3r4(vPa, vua, fParam1);
        dAddScaledVector3r4(vPb, vub, fParam2);

        // calculate collision point
        dVector3 vPntTmp;
        dAddVectors3r4(vPntTmp, vPa, vPb);

        dScaleVector3r4(vPntTmp, 0.5f);

        GenerateContact(m_iFlags, m_ContactGeoms, m_iStride, m_Geom1, m_Geom2, TriIndex,
            vPntTmp, m_vBestNormal, m_fBestDepth, m_ctContacts);

        // if triangle is the referent face then clip box to triangle face
    }
    else if (m_iBestAxis == 1)
    {
        dVector3 vNormal2;
        dCopyNegatedVector3r4(vNormal2, m_vBestNormal);

        // vNr is normal in box frame, pointing from triangle to box
        dMatrix3 mTransposed;
        dTransposetMatrix34(mTransposed, m_mHullBoxRot);

        dVector3 vNr;
        vNr[0] = dCalcVectorDot3(mTransposed, vNormal2);
        vNr[1] = dCalcVectorDot3(mTransposed + 4, vNormal2);
        vNr[2] = dCalcVectorDot3(mTransposed + 8, vNormal2);

        dVector3 vAbsNormal;
        dFabsVector3r4(vAbsNormal, vNr);

        // get closest face from box
        int iB0, iB1, iB2;
        if (vAbsNormal[1] > vAbsNormal[0])
        {
            if (vAbsNormal[1] > vAbsNormal[2])
            {
                iB1 = 0;
                iB0 = 1;
                iB2 = 2;
            }
            else
            {
                iB1 = 0;
                iB2 = 1;
                iB0 = 2;
            }
        }
        else
        {
            if (vAbsNormal[0] > vAbsNormal[2])
            {
                iB0 = 0;
                iB1 = 1;
                iB2 = 2;
            }
            else
            {
                iB1 = 0;
                iB2 = 1;
                iB0 = 2;
            }
        }

        // Here find center of box face we are going to project
        dVector3 vCenter;
        dVector3 vRotCol;
        dGetMatrixColumn3(vRotCol, m_mHullBoxRot, iB0);

        dSubtractVectors3r4(vCenter, m_vHullBoxPos, v0);

        if (vNr[iB0] > 0)
            dAddScaledVector3r4(vCenter, vRotCol, -m_vBoxHalfSize[iB0]);
        else
            dAddScaledVector3r4(vCenter, vRotCol, m_vBoxHalfSize[iB0]);

        // Here find 4 corner points of box
        dVector3 avPoints[4];

        dVector3 vRotCol2;
        dGetMatrixColumn3(vRotCol, m_mHullBoxRot, iB1);
        dGetMatrixColumn3(vRotCol2, m_mHullBoxRot, iB2);

        dReal m_vBoxHalfSizeiB1 = m_vBoxHalfSize[iB1];
        dReal m_vBoxHalfSizeiB2 = m_vBoxHalfSize[iB2];

        dAddScaledVectors3r4(avPoints[0], vRotCol, vRotCol2, m_vBoxHalfSizeiB1, -m_vBoxHalfSizeiB2);
        dAddVector3r4(avPoints[0], vCenter);

        dAddScaledVectors3r4(avPoints[1], vRotCol, vRotCol2, -m_vBoxHalfSizeiB1, -m_vBoxHalfSizeiB2);
        dAddVector3r4(avPoints[1], vCenter);

        dAddScaledVectors3r4(avPoints[2], vRotCol, vRotCol2, -m_vBoxHalfSizeiB1, m_vBoxHalfSizeiB2);
        dAddVector3r4(avPoints[2], vCenter);

        dAddScaledVectors3r4(avPoints[3], vRotCol, vRotCol2, m_vBoxHalfSizeiB1, m_vBoxHalfSizeiB2);
        dAddVector3r4(avPoints[3], vCenter);

        // clip Box face with 4 planes of triangle (1 face plane, 3 egde planes)
        dVector3 avTempArray1[9];
        dVector3 avTempArray2[9];
        dVector4 plPlane;

        int iTempCnt1 = 0;
        int iTempCnt2 = 0;
        /*
                // zeroify vectors - necessary?
                for(int i=0; i<9; i++) {
                    avTempArray1[i][0]=0;
                    avTempArray1[i][1]=0;
                    avTempArray1[i][2]=0;

                    avTempArray2[i][0]=0;
                    avTempArray2[i][1]=0;
                    avTempArray2[i][2]=0;
                }
        */

        // Normal plane
        dVector3 vTemp;
        dCopyNegatedVector3r4(vTemp, m_vN);
        dNormalize3(vTemp);
        dConstructPlane(plPlane, vTemp, 0);

        _cldClipPolyToPlane(avPoints, 4, avTempArray1, iTempCnt1, plPlane);

        // Plane p0
        dVector3 vTemp2;
        dSubtractVectors3r4(vTemp2, v1, v0);
        dCalcVectorCross3r4(vTemp, m_vN, vTemp2);
        dNormalize3(vTemp);
        dConstructPlane(plPlane, vTemp, 0);

        _cldClipPolyToPlane(avTempArray1, iTempCnt1, avTempArray2, iTempCnt2, plPlane);

        // Plane p1
        dSubtractVectors3r4(vTemp2, v2, v1);
        dCalcVectorCross3r4(vTemp, m_vN, vTemp2);
        dNormalize3(vTemp);
        dSubtractVectors3r4(vTemp2, v0, v2);
        dConstructPlane(plPlane, vTemp, dCalcVectorDot3(vTemp2, vTemp));

        _cldClipPolyToPlane(avTempArray2, iTempCnt2, avTempArray1, iTempCnt1, plPlane);

        // Plane p2
        dSubtractVectors3r4(vTemp2, v0, v2);
        dCalcVectorCross3r4(vTemp, m_vN, vTemp2);
        dNormalize3(vTemp);
        dConstructPlane(plPlane, vTemp, 0);

        _cldClipPolyToPlane(avTempArray1, iTempCnt1, avTempArray2, iTempCnt2, plPlane);

        // END of clipping polygons

        // for each generated contact point
        for (int i = 0; i < iTempCnt2; i++)
        {
            // calculate depth
            dReal fTempDepth = dCalcVectorDot3(vNormal2, avTempArray2[i]);

            // clamp depth to zero
            if (fTempDepth > 0)
            {
                fTempDepth = 0;
            }

            dVector3 vPntTmp;
            dAddVectors3r4(vPntTmp, avTempArray2[i], v0);

            GenerateContact(m_iFlags, m_ContactGeoms, m_iStride, m_Geom1, m_Geom2, TriIndex,
                vPntTmp, m_vBestNormal, -fTempDepth, m_ctContacts);

            if ((m_ctContacts | CONTACTS_UNIMPORTANT) == (m_iFlags & (NUMC_MASK | CONTACTS_UNIMPORTANT))) {
                break;
            }
        }

        //dAASSERT(m_ctContacts>0);

        // if box face is the referent face, then clip triangle on box face
    }
    else
    { // 2 <= if iBestAxis <= 4

        // get normal of box face
        dVector3 vNormal2;
        dCopyVector3r4(vNormal2, m_vBestNormal);

        // get indices of box axes in correct order
        int iA0, iA1, iA2;
        iA0 = m_iBestAxis - 2;
        if (iA0 == 0)
        {
            iA1 = 1;
            iA2 = 2;
        }
        else if (iA0 == 1)
        {
            iA1 = 0;
            iA2 = 2;
        }
        else
        {
            iA1 = 0;
            iA2 = 1;
        }

        dVector3 avPoints[3];
        // calculate triangle vertices in box frame
        dSubtractVectors3r4(avPoints[0], v0, m_vHullBoxPos);
        dSubtractVectors3r4(avPoints[1], v1, m_vHullBoxPos);
        dSubtractVectors3r4(avPoints[2], v2, m_vHullBoxPos);

        // CLIP Polygons
        // define temp data for clipping
        dVector3 avTempArray1[9];
        dVector3 avTempArray2[9];

        int iTempCnt1, iTempCnt2;
        /*
                // zeroify vectors - necessary?
                for(int i=0; i<9; i++) {
                    avTempArray1[i][0]=0;
                    avTempArray1[i][1]=0;
                    avTempArray1[i][2]=0;

                    avTempArray2[i][0]=0;
                    avTempArray2[i][1]=0;
                    avTempArray2[i][2]=0;
                }
        */
        // clip triangle with 5 box planes (1 face plane, 4 edge planes)

        dVector4 plPlane;

        // Normal plane
        dVector3 vTemp;
        dCopyNegatedVector3r4(vTemp, vNormal2);
        dConstructPlane(plPlane, vTemp, m_vBoxHalfSize[iA0]);

        _cldClipPolyToPlane(avPoints, 3, avTempArray1, iTempCnt1, plPlane);

        // Plane p0
        dGetMatrixColumn3(vTemp, m_mHullBoxRot, iA1);
        dConstructPlane(plPlane, vTemp, m_vBoxHalfSize[iA1]);

        _cldClipPolyToPlane(avTempArray1, iTempCnt1, avTempArray2, iTempCnt2, plPlane);

        // Plane p1
        dGetMatrixColumn3(vTemp, m_mHullBoxRot, iA1);
        dCopyNegatedVector3r4(vTemp, vTemp);
        dConstructPlane(plPlane, vTemp, m_vBoxHalfSize[iA1]);

        _cldClipPolyToPlane(avTempArray2, iTempCnt2, avTempArray1, iTempCnt1, plPlane);

        // Plane p2
        dGetMatrixColumn3(vTemp, m_mHullBoxRot, iA2);
        dConstructPlane(plPlane, vTemp, m_vBoxHalfSize[iA2]);

        _cldClipPolyToPlane(avTempArray1, iTempCnt1, avTempArray2, iTempCnt2, plPlane);

        // Plane p3
        dGetMatrixColumn3(vTemp, m_mHullBoxRot, iA2);
        dCopyNegatedVector3r4(vTemp, vTemp);
        dConstructPlane(plPlane, vTemp, m_vBoxHalfSize[iA2]);

        _cldClipPolyToPlane(avTempArray2, iTempCnt2, avTempArray1, iTempCnt1, plPlane);


        // for each generated contact point
        for (int i = 0; i < iTempCnt1; i++)
        {
            // calculate depth
            dReal fTempDepth = dCalcVectorDot3(vNormal2, avTempArray1[i]) - m_vBoxHalfSize[iA0];

            // clamp depth to zero
            if (fTempDepth > 0)
            {
                fTempDepth = 0;
            }

            // generate contact data
            dVector3 vPntTmp;
            dAddVectors3r4(vPntTmp, avTempArray1[i], m_vHullBoxPos);

            GenerateContact(m_iFlags, m_ContactGeoms, m_iStride, m_Geom1, m_Geom2, TriIndex,
                vPntTmp, m_vBestNormal, -fTempDepth, m_ctContacts);

            if ((m_ctContacts | CONTACTS_UNIMPORTANT) == (m_iFlags & (NUMC_MASK | CONTACTS_UNIMPORTANT))) {
                break;
            }
        }

        //dAASSERT(m_ctContacts>0);
    }
}

// test one mesh triangle on intersection with given box
void sTrimeshBoxColliderData::_cldTestOneTriangle(const dVector3 &v0, const dVector3 &v1, const dVector3 &v2, int TriIndex)//, void *pvUser)
{
    // do intersection test and find best separating axis
    if (!_cldTestSeparatingAxes(v0, v1, v2))
    {
        // if not found do nothing
        return;
    }

    // if best separation axis is not found
    if (m_iBestAxis == 0) {
        // this should not happen (we should already exit in that case)
        //dMessage (0, "best separation axis not found");
        // do nothing
        return;
    }

    _cldClipping(v0, v1, v2, TriIndex);
}


void sTrimeshBoxColliderData::SetupInitialContext(dxTriMesh *TriMesh, dxGeom *BoxGeom,
    int Flags, dContactGeom* Contacts, int Stride)
{
    // get source hull position, orientation and half size
    const dMatrix3& mRotBox = *(const dMatrix3*)dGeomGetRotation(BoxGeom);
    const dVector3& vPosBox = *(const dVector3*)dGeomGetPosition(BoxGeom);

    // to global
    dCopyMatrix4x3(m_mHullBoxRot, mRotBox);
    dCopyVector3r4(m_vHullBoxPos, vPosBox);

    dGeomBoxGetLengths(BoxGeom, m_vBoxHalfSize);
    dScaleVector3r4(m_vBoxHalfSize, 0.5f);

    // get destination hull position and orientation
    const dVector3& vPosMesh = *(const dVector3*)dGeomGetPosition(TriMesh);

    // to global
    dCopyVector3r4(m_vHullDstPos, vPosMesh);

    // global info for contact creation
    m_ctContacts = 0;
    m_iStride = Stride;
    m_iFlags = Flags;
    m_ContactGeoms = Contacts;
    m_Geom1 = TriMesh;
    m_Geom2 = BoxGeom;

    // reset stuff
    m_fBestDepth = MAXVALUE;
    m_vBestNormal[0] = 0;
    m_vBestNormal[1] = 0;
    m_vBestNormal[2] = 0;
}

int sTrimeshBoxColliderData::TestCollisionForSingleTriangle(int ctContacts0, int Triint,
    dVector3 dv[3], bool &bOutFinishSearching)
{
    // test this triangle
    _cldTestOneTriangle(dv[0], dv[1], dv[2], Triint);

    // fill-in tri index for generated contacts
    for (; ctContacts0 < m_ctContacts; ctContacts0++) {
        dContactGeom* pContact = SAFECONTACT(m_iFlags, m_ContactGeoms, ctContacts0, m_iStride);
        pContact->side1 = Triint;
        pContact->side2 = -1;
    }

    /*
    NOTE by Oleh_Derevenko:
    The function continues checking triangles after maximal number
    of contacts is reached because it selects maximal penetration depths.
    See also comments in GenerateContact()
    */
    bOutFinishSearching = ((m_ctContacts | CONTACTS_UNIMPORTANT) == (m_iFlags & (NUMC_MASK | CONTACTS_UNIMPORTANT)));

    return ctContacts0;
}

// OPCODE version of box to mesh collider
static void dQueryBTLPotentialCollisionTriangles(OBBCollider &Collider,
    const sTrimeshBoxColliderData &cData, dxTriMesh *TriMesh, dxGeom *BoxGeom,
    OBBCache &BoxCache)
{
    // get source hull position, orientation and half size
    const dMatrix3& mRotBox = *(const dMatrix3*)dGeomGetRotation(BoxGeom);
    const dVector3& vPosBox = *(const dVector3*)dGeomGetPosition(BoxGeom);

    // Make OBB
    OBB Box;
    Box.mCenter.x = vPosBox[0];
    Box.mCenter.y = vPosBox[1];
    Box.mCenter.z = vPosBox[2];

    // It is a potential issue to explicitly cast to float 
    // if custom width floating point type is introduced in OPCODE.
    // It is necessary to make a typedef and cast to it
    // (e.g. typedef float opc_float;)
    // However I'm not sure in what header it should be added.

    Box.mExtents.x = /*(float)*/cData.m_vBoxHalfSize[0];
    Box.mExtents.y = /*(float)*/cData.m_vBoxHalfSize[1];
    Box.mExtents.z = /*(float)*/cData.m_vBoxHalfSize[2];

    Box.mRot.m[0][0] = /*(float)*/mRotBox[0];
    Box.mRot.m[1][0] = /*(float)*/mRotBox[1];
    Box.mRot.m[2][0] = /*(float)*/mRotBox[2];

    Box.mRot.m[0][1] = /*(float)*/mRotBox[4];
    Box.mRot.m[1][1] = /*(float)*/mRotBox[5];
    Box.mRot.m[2][1] = /*(float)*/mRotBox[6];

    Box.mRot.m[0][2] = /*(float)*/mRotBox[8];
    Box.mRot.m[1][2] = /*(float)*/mRotBox[9];
    Box.mRot.m[2][2] = /*(float)*/mRotBox[10];

    Matrix4x4 amatrix;
    Matrix4x4 BoxMatrix = MakeMatrix(vPosBox, mRotBox, amatrix);

    Matrix4x4 InvBoxMatrix;
    InvertPRMatrix(InvBoxMatrix, BoxMatrix);

    // get destination hull position and orientation
    const dMatrix3& mRotMesh = *(const dMatrix3*)dGeomGetRotation(TriMesh);
    const dVector3& vPosMesh = *(const dVector3*)dGeomGetPosition(TriMesh);

    // TC results
    if (TriMesh->doBoxTC)
    {
        dxTriMesh::BoxTC* BoxTC = 0;
        for (int i = 0; i < TriMesh->BoxTCCache.size(); i++)
        {
            if (TriMesh->BoxTCCache[i].Geom == BoxGeom)
            {
                BoxTC = &TriMesh->BoxTCCache[i];
                break;
            }
        }
        if (!BoxTC)
        {
            TriMesh->BoxTCCache.push(dxTriMesh::BoxTC());

            BoxTC = &TriMesh->BoxTCCache[TriMesh->BoxTCCache.size() - 1];
            BoxTC->Geom = BoxGeom;
            BoxTC->FatCoeff = 1.1f; // Pierre recommends this, instead of 1.0
        }

        // Intersect
        Collider.SetTemporalCoherence(true);
        Collider.Collide(*BoxTC, Box, TriMesh->Data->BVTree, null, &MakeMatrix(vPosMesh, mRotMesh, amatrix));
    }
    else
    {
        Collider.SetTemporalCoherence(false);
        Collider.Collide(BoxCache, Box, TriMesh->Data->BVTree, null,
            &MakeMatrix(vPosMesh, mRotMesh, amatrix));
    }
}

int dCollideBTL(dxGeom* g1, dxGeom* BoxGeom, int Flags, dContactGeom* Contacts, int Stride) {
    dIASSERT(Stride >= (int)sizeof(dContactGeom));
    dIASSERT(g1->type == dTriMeshClass);
    dIASSERT(BoxGeom->type == dBoxClass);
    dIASSERT((Flags & NUMC_MASK) >= 1);

    dxTriMesh* TriMesh = (dxTriMesh*)g1;

    sTrimeshBoxColliderData cData;
    cData.SetupInitialContext(TriMesh, BoxGeom, Flags, Contacts, Stride);

    const unsigned uiTLSKind = TriMesh->getParentSpaceTLSKind();
    dIASSERT(uiTLSKind == BoxGeom->getParentSpaceTLSKind()); // The colliding spaces must use matching cleanup method
    TrimeshCollidersCache *pccColliderCache = GetTrimeshCollidersCache(uiTLSKind);
    OBBCollider& Collider = pccColliderCache->_OBBCollider;

    dQueryBTLPotentialCollisionTriangles(Collider, cData, TriMesh, BoxGeom,
        pccColliderCache->defaultBoxCache);

    if (!Collider.GetContactStatus()) {
        // no collision occurred
        return 0;
    }

    // Retrieve data
    int TriCount = Collider.GetNbTouchedPrimitives();
    const int* Triangles = (const int*)Collider.GetTouchedPrimitives();

    if (TriCount != 0)
    {
        if (TriMesh->ArrayCallback != null)
        {
            TriMesh->ArrayCallback(TriMesh, BoxGeom, Triangles, TriCount);
        }

        // get destination hull position and orientation
        const dMatrix3& mRotMesh = *(const dMatrix3*)dGeomGetRotation(TriMesh);
        const dVector3& vPosMesh = *(const dVector3*)dGeomGetPosition(TriMesh);

        int ctContacts0 = 0;

        // loop through all intersecting triangles
        for (int i = 0; i < TriCount; i++)
        {
            const int Triint = Triangles[i];
            if (!Callback(TriMesh, BoxGeom, Triint))
                continue;

            dVector3 dv[3];
            FetchTriangle(TriMesh, Triint, vPosMesh, mRotMesh, dv);

            bool bFinishSearching;
            ctContacts0 = cData.TestCollisionForSingleTriangle(ctContacts0, Triint, dv, bFinishSearching);

            if (bFinishSearching) {
                break;
            }
        }
    }

    return cData.m_ctContacts;
}

// GenerateContact - Written by Jeff Smith (jeff@burri.to)
//   Generate a "unique" contact.  A unique contact has a unique
//   position or normal.  If the potential contact has the same
//   position and normal as an existing contact, but a larger
//   penetration depth, this new depth is used instead
//
static void
GenerateContact(int in_Flags, dContactGeom* in_Contacts, int in_Stride,
    dxGeom* in_g1, dxGeom* in_g2, int TriIndex,
    const dVector3 in_ContactPos, const dVector3 in_Normal, dReal in_Depth,
    int& OutTriCount)
{
    /*
    NOTE by Oleh_Derevenko:
    This function is called after maximal number of contacts has already been
    collected because it has a side effect of replacing penetration depth of
    existing contact with larger penetration depth of another matching normal contact.
    If this logic is not necessary any more, you can bail out on reach of contact
    number maximum immediately in dCollideBTL(). You will also need to correct
    conditional statements after invocations of GenerateContact() in _cldClipping().
    */
    do
    {
        dContactGeom* Contact;
        dVector3 diff;

        if (!(in_Flags & CONTACTS_UNIMPORTANT))
        {
            bool duplicate = false;
            for (int i = 0; i < OutTriCount; i++)
            {
                Contact = SAFECONTACT(in_Flags, in_Contacts, i, in_Stride);

                // same position?
                dSubtractVectors3r4(diff, in_ContactPos, Contact->pos);
                if (dCalcVectorLengthSquare3(diff) < dEpsilon)
                {
                    // same normal?
                    if (REAL(1.0) - dFabs(dCalcVectorDot3(in_Normal, Contact->normal)) < dEpsilon)
                    {
                        if (in_Depth > Contact->depth)
                            Contact->depth = in_Depth;
                        duplicate = true;
                        /*
                        NOTE by Oleh_Derevenko:
                        There may be a case when two normals are close to each other but not duplicate
                        while third normal is detected to be duplicate for both of them.
                        This is the only reason I can think of, there is no "break" statement.
                        Perhaps author considered it to be logical that the third normal would
                        replace the depth in both of initial contacts.
                        However, I consider it a questionable practice which should not
                        be applied without deep understanding of underlaying physics.
                        Even more, is this situation with close normal triplet acceptable at all?
                        Should not be two initial contacts reduced to one (replaced with the latter)?
                        If you know the answers for these questions, you may want to change this code.
                        See the same statement in GenerateContact() of collision_trimesh_trimesh.cpp
                        */
                    }
                }
            }
            if (duplicate || OutTriCount == (in_Flags & NUMC_MASK))
            {
                break;
            }
        }
        else
        {
            dIASSERT(OutTriCount < (in_Flags & NUMC_MASK));
        }

        // Add a new contact
        Contact = SAFECONTACT(in_Flags, in_Contacts, OutTriCount, in_Stride);

        dCopyVector3r4(Contact->pos, in_ContactPos);
        dCopyVector3r4(Contact->normal, in_Normal);

        Contact->depth = in_Depth;

        Contact->g1 = in_g1;
        Contact->g2 = in_g2;

        Contact->side1 = TriIndex;
        Contact->side2 = -1;

        OutTriCount++;
    } while (false);
}
