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

some useful collision utility stuff. this includes some API utility
functions that are defined in the public header files.

*/

#include <ode/common.h>
#include <ode/collision.h>
#include "config.h"
#include "odemath.h"
#include "collision_util.h"

//****************************************************************************

int dCollideSpheres (dVector3 p1, dReal r1,
                     dVector3 p2, dReal r2, dContactGeom *c)
{
    // printf ("d=%.2f  (%.2f %.2f %.2f) (%.2f %.2f %.2f) r1=%.2f r2=%.2f\n",
    //	  d,p1[0],p1[1],p1[2],p2[0],p2[1],p2[2],r1,r2);

    dVector3 delta;
    dSubtractVectors3(delta, p1, p2);
    float d = dCalcVectorLengthSquare3(delta);

    dReal rsum = r1 + r2;
    if( d < dEpsilon)
    {
        dCopyVector3(c->pos, p1);
        c->normal[0] = 1;
        c->normal[1] = 0;
        c->normal[2] = 0;
        c->depth = rsum;
        return 1;
    }

    if (d > rsum * rsum)
        return 0;

    d = dSqrt(d);
    c->depth = rsum - d;

    d = dRecip(d);
    dScaleVector3(delta, d);

    dCopyVector3(c->normal, delta);

    dCopyVector3(c->pos, p1);
    dReal k = REAL(0.5) * (r2 - r1 - d);
    dAddScaledVector4(c->pos, delta, k);
    return 1;
}


void dLineClosestApproach (const dVector3 pa, const dVector3 ua,
                           const dVector3 pb, const dVector3 ub,
                           dReal *alpha, dReal *beta)
{
    dVector3 p;
    dReal uaub = dCalcVectorDot3(ua, ub);
    dReal d = 1 - uaub * uaub;
    if (d <= REAL(0.0001))
    {
        // @@@ this needs to be made more robust
        *alpha = 0;
        *beta  = 0;
    }
    else
    {
        dSubtractVectors3(p, pb, pa);
        dReal q1 = dCalcVectorDot3(ua, p);
        dReal q2 = -dCalcVectorDot3(ub, p);
        d = dRecip(d);
        *alpha = (q1 + uaub * q2) * d;
        *beta  = (uaub * q1 + q2) * d;
    }
}


// given two line segments A and B with endpoints a1-a2 and b1-b2, return the
// points on A and B that are closest to each other (in cp1 and cp2).
// in the case of parallel lines where there are multiple solutions, a
// solution involving the endpoint of at least one line will be returned.
// this will work correctly for zero length lines, e.g. if a1==a2 and/or
// b1==b2.
//
// the algorithm works by applying the voronoi clipping rule to the features
// of the line segments. the three features of each line segment are the two
// endpoints and the line between them. the voronoi clipping rule states that,
// for feature X on line A and feature Y on line B, the closest points PA and
// PB between X and Y are globally the closest points if PA is in V(Y) and
// PB is in V(X), where V(X) is the voronoi region of X.

void dClosestLineSegmentPoints (const dVector3 a1, const dVector3 a2,
                                const dVector3 b1, const dVector3 b2,
                                dVector3 cp1, dVector3 cp2)
{
    dVector3 a1a2,b1b2,a1b1,a1b2,a2b1,a2b2,n;
    dReal la,lb,k,da1,da2,da3,da4,db1,db2,db3,db4,det;

#define SET2(a, b) a[0] = b[0]; a[1] = b[1]; a[2] = b[2];
#define SET3(a, b, op, c) a[0] = b[0] op c[0]; a[1] = b[1] op c[1]; a[2] = b[2] op c[2];

    // check vertex-vertex features
    dSubtractVectors3(a1a2, a2, a1);
    dSubtractVectors3(a1b1, b1, a1);
    da1 = dCalcVectorDot3(a1a2, a1b1);

    dSubtractVectorCross3(b1b2, b2, b1);
    db1 = dCalcVectorDot3(b1b2, a1b1);
    if (da1 <= 0 && db1 >= 0)
    {
        SET2 (cp1, a1);
        SET2 (cp2, b1);
        return;
    }

    dSubtractVectors3(a1b2, b2, a1);
    da2 = dCalcVectorDot3(a1a2, a1b2);
    db2 = dCalcVectorDot3(b1b2, a1b2);
    if (da2 <= 0 && db2 <= 0)
    {
        SET2 (cp1, a1);
        SET2 (cp2, b2);
        return;
    }

    dSubtractVectors3(a2b1, b1, a2);
    da3 = dCalcVectorDot3(a1a2, a2b1);
    db3 = dCalcVectorDot3(b1b2, a2b1);
    if (da3 >= 0 && db3 >= 0)
    {
        SET2 (cp1, a2);
        SET2 (cp2, b1);
        return;
    }

    dSubtractVectors3(a2b2, b2, a2);
    da4 = dCalcVectorDot3(a1a2, a2b2);
    db4 = dCalcVectorDot3(b1b2, a2b2);
    if (da4 >= 0 && db4 <= 0)
    {
        SET2 (cp1, a2);
        SET2 (cp2, b2);
        return;
    }

    // check edge-vertex features.
    // if one or both of the lines has zero length, we will never get to here,
    // so we do not have to worry about the following divisions by zero.

    la = dCalcVectorLengthSquare3(a1a2);
    if (da1 >= 0 && da3 <= 0)
    {
        k = da1 / la;
        SET3 (n, a1b1, -, k * a1a2);
        if (dCalcVectorDot3(b1b2, n) >= 0)
        {
            SET3 (cp1,a1,+,k*a1a2);
            SET2 (cp2,b1);
            return;
        }
    }

    if (da2 >= 0 && da4 <= 0)
    {
        k = da2 / la;
        SET3 (n, a1b2, -, k * a1a2);
        if (dCalcVectorDot3(b1b2, n) <= 0)
        {
            SET3 (cp1, a1, +, k * a1a2);
            SET2 (cp2, b2);
            return;
        }
    }

    lb = dCalcVectorLengthSquare3(b1b2);
    if (db1 <= 0 && db2 >= 0)
    {
        k = -db1 / lb;
        SET3 (n, -a1b1, -, k * b1b2);
        if (dCalcVectorDot3(a1a2, n) >= 0)
        {
            SET2 (cp1, a1);
            SET3 (cp2, b1, +, k * b1b2);
            return;
        }
    }

    if (db3 <= 0 && db4 >= 0)
    {
        k = -db3 / lb;
        SET3 (n, -a2b1, -, k * b1b2);
        if (dCalcVectorDot3(a1a2, n) <= 0)
        {
            SET2 (cp1, a2);
            SET3 (cp2, b1, +, k * b1b2);
            return;
        }
    }

    // it must be edge-edge

    k = dCalcVectorDot3(a1a2,b1b2);
    det = la * lb - k * k;
    if (det <= 0)
    {
        // this should never happen, but just in case...
        SET2(cp1, a1);
        SET2(cp2, b1);
        return;
    }
    det = dRecip (det);
    dReal alpha = (lb * da1 -  k * db1) * det;
    dReal beta  = ( k *da1 - la * db1) * det;
    SET3 (cp1, a1, +, alpha * a1a2);
    SET3 (cp2, b1, +, beta * b1b2);

# undef SET2
# undef SET3
}


// a simple root finding algorithm is used to find the value of 't' that
// satisfies:
//		d|D(t)|^2/dt = 0
// where:
//		|D(t)| = |p(t)-b(t)|
// where p(t) is a point on the line parameterized by t:
//		p(t) = p1 + t*(p2-p1)
// and b(t) is that same point clipped to the boundary of the box. in box-
// relative coordinates d|D(t)|^2/dt is the sum of three x,y,z components
// each of which looks like this:
//
//	    t_lo     /
//	      ______/    -->t
//	     /     t_hi
//	    /
//
// t_lo and t_hi are the t values where the line passes through the planes
// corresponding to the sides of the box. the algorithm computes d|D(t)|^2/dt
// in a piecewise fashion from t=0 to t=1, stopping at the point where
// d|D(t)|^2/dt crosses from negative to positive.

void dClosestLineBoxPoints (const dVector3 p1, const dVector3 p2,
                            const dVector3 c, const dMatrix3 R,
                            const dVector3 h,
                            dVector3 lret, dVector3 bret)
{
    int i;

    // compute the start and delta of the line p1-p2 relative to the box.
    // we will do all subsequent computations in this box-relative coordinate
    // system. we have to do a translation and rotation for each point.
    dVector3 tmp,s,v;
    dSubtractVectors3(tmp, p1, c);
    dMultiply1_331 (s,R,tmp);
    dSubtractVectors3(tmp, p2, p1);
    dMultiply1_331 (v,R,tmp);

    dVector3 sign;

    // region is -1,0,+1 depending on which side of the box planes each
    // coordinate is on. tanchor is the next t value at which there is a
    // transition, or the last one if there are no more.

    int region[3];
    dReal t = 0;
    dReal dd2dt = 0;
    dReal tanchor[3];
    dVector3 v2;

#if defined( dSINGLE )
    const dReal tanchor_eps = REAL(1e-6);
#else
    const dReal tanchor_eps = REAL(1e-15);
#endif

    // mirror the line so that v has all components >= 0
    // find the region and tanchor values for p1
    for (i = 0; i < 3; i++)
    {
        if (v[i] < 0)
        {
            v[i] = -v[i];
            s[i] = -s[i];
            sign[i] = -1;
        }
        else sign[i] = 1;

        v2[i] = v[i] * v[i];

        if (v[i] > tanchor_eps)
        {
            if (s[i] < -h[i])
            {
                region[i] = -1;
                tanchor[i] = (-h[i] - s[i]) / v[i];
                dd2dt -= v2[i] * tanchor[i];
            }
            else
            {
                tanchor[i] = (h[i] - s[i]) / v[i];
                if(s[i] > h[i])
                {
                    region[i] = 1;
                    dd2dt -= v2[i] * tanchor[i];
                }
                else
                    region[i] = 0;
            }
        }
        else
        {
            region[i] = 0;
            tanchor[i] = 2;		// this will never be a valid tanchor
        }
    }
    // compute d|d|^2/dt for t=0. if it's >= 0 then p1 is the closest point
    if (dd2dt >= 0)
        goto got_answer;

    do {
        // find the point on the line that is at the next clip plane boundary
        dReal next_t = 1.0;
        for (i=0; i<3; i++)
        {
            if (tanchor[i] > t && tanchor[i] < next_t)
                next_t = tanchor[i];
        }

        // compute d|d|^2/dt for the next t
        dReal next_dd2dt = 0;
        for (i=0; i<3; i++)
        {
            if(region[i] != 0)
                next_dd2dt += v2[i] * (next_t - tanchor[i]);
        }

        // if the sign of d|d|^2/dt has changed, solution = the crossover point
        if (next_dd2dt >= 0)
        {
            dReal m = (next_dd2dt - dd2dt) / (next_t - t);
            t -= dd2dt / m;
            goto got_answer;
        }

        // advance to the next anchor point / region
        for (i=0; i<3; i++)
        {
            if (tanchor[i] == next_t)
            {
                tanchor[i] = (h[i] - s[i]) / v[i];
                region[i]++;
            }
        }
        t = next_t;
        dd2dt = next_dd2dt;
    }
    while (t < 1);
    t = 1;

got_answer:
    float ftmp;
    if(t >= (1 - tanchor_eps))
    {
        for (i=0; i<3; i++)
        {
            // compute closest point on the line
            lret[i] = p2[i];

            ftmp = sign[i] * (s[i] + v[i]);
            if (ftmp < -h[i])
                ftmp = -h[i];
            else if (ftmp > h[i])
                ftmp = h[i];
            tmp[i] = ftmp;
        }
    }
    else if (t <= tanchor_eps)
    {
        for (i=0; i<3; i++)
        {
            lret[i] = p1[i];

            ftmp = sign[i] * s[i];
            if (ftmp < -h[i])
                ftmp = -h[i];
            else if (ftmp > h[i])
                ftmp = h[i];
            tmp[i] = ftmp;
        }
    }
    else
    {
        for (i=0; i<3; i++)
        {
            lret[i] = p1[i] + t*tmp[i];

            ftmp = sign[i] * (s[i] + t*v[i]);
            if (ftmp < -h[i])
                ftmp = -h[i];
            else if (ftmp > h[i])
                ftmp = h[i];
            tmp[i] = ftmp;
        }       
    }

    // compute closest point on the box
    dMultiply0_331(s,R,tmp);
    dAddVectors3(bret, s, c);
}


// given boxes (p1,R1,side1) and (p1,R1,side1), return 1 if they intersect
// or 0 if not.

int dBoxTouchesBox (const dVector3 p1, const dMatrix3 R1,
                    const dVector3 side1, const dVector3 p2,
                    const dMatrix3 R2, const dVector3 side2)
{
    // two boxes are disjoint if (and only if) there is a separating axis
    // perpendicular to a face from one box or perpendicular to an edge from
    // either box. the following tests are derived from:
    //    "OBB Tree: A Hierarchical Structure for Rapid Interference Detection",
    //    S.Gottschalk, M.C.Lin, D.Manocha., Proc of ACM Siggraph 1996.

    // Rij is R1'*R2, i.e. the relative rotation between R1 and R2.
    // Qij is abs(Rij)
    dVector3 p,pp;
    dReal A1,A2,A3,B1,B2,B3,R11,R12,R13,R21,R22,R23,R31,R32,R33,
        Q11,Q12,Q13,Q21,Q22,Q23,Q31,Q32,Q33;

    // get vector from centers of box 1 to box 2, relative to box 1
    dSubtractVectors3(p, p2, p1);
    dMultiply1_331 (pp, R1 ,p);		// get pp = p relative to body 1

    // get side lengths / 2
    A1 = side1[0]*REAL(0.5);
    A2 = side1[1]*REAL(0.5);
    A3 = side1[2]*REAL(0.5);

    B1 = side2[0]*REAL(0.5);
    B2 = side2[1]*REAL(0.5);
    B3 = side2[2]*REAL(0.5);

    // for the following tests, excluding computation of Rij, in the worst case,
    // 15 compares, 60 adds, 81 multiplies, and 24 absolutes.
    // notation: R1=[u1 u2 u3], R2=[v1 v2 v3]

    // separating axis = u1,u2,u3
    R11 = dCalcVectorDot3_44(R1+0,R2+0);
    R12 = dCalcVectorDot3_44(R1+0,R2+1);
    R13 = dCalcVectorDot3_44(R1+0,R2+2);
    Q11 = dFabs(R11);
    Q12 = dFabs(R12);
    Q13 = dFabs(R13);
    if (dFabs(pp[0]) > (A1 + B1  * Q11 + B2 * Q12 + B3 * Q13))
        return 0;
    R21 = dCalcVectorDot3_44(R1+1,R2+0);
    R22 = dCalcVectorDot3_44(R1+1,R2+1);
    R23 = dCalcVectorDot3_44(R1+1,R2+2);
    Q21 = dFabs(R21);
    Q22 = dFabs(R22);
    Q23 = dFabs(R23);
    if (dFabs(pp[1]) > (A2 + B1 * Q21 + B2 * Q22 + B3 * Q23))
        return 0;
    R31 = dCalcVectorDot3_44(R1+2,R2+0);
    R32 = dCalcVectorDot3_44(R1+2,R2+1);
    R33 = dCalcVectorDot3_44(R1+2,R2+2);
    Q31 = dFabs(R31);
    Q32 = dFabs(R32);
    Q33 = dFabs(R33);
    if (dFabs(pp[2]) > (A3 + B1 * Q31 + B2 * Q32 + B3 * Q33))
        return 0;

    // separating axis = v1,v2,v3
    if (dFabs(dCalcVectorDot3_41(R2+0, p)) > (A1 * Q11 + A2 * Q21 + A3 * Q31 + B1)) return 0;
    if (dFabs(dCalcVectorDot3_41(R2+1 ,p)) > (A1 * Q12 + A2 * Q22 + A3 * Q32 + B2)) return 0;
    if (dFabs(dCalcVectorDot3_41(R2+2, p)) > (A1 * Q13 + A2 * Q23 + A3 * Q33 + B3)) return 0;

    // separating axis = u1 x (v1,v2,v3)
    if (dFabs(pp[2]*R21-pp[1]*R31) > A2 * Q31 + A3 * Q21 + B2 * Q13 + B3 * Q12) return 0;
    if (dFabs(pp[2]*R22-pp[1]*R32) > A2 * Q32 + A3 * Q22 + B1 * Q13 + B3 * Q11) return 0;
    if (dFabs(pp[2]*R23-pp[1]*R33) > A2 * Q33 + A3 * Q23 + B1 * Q12 + B2 * Q11) return 0;

    // separating axis = u2 x (v1,v2,v3)
    if (dFabs(pp[0]*R31-pp[2]*R11) > A1 * Q31 + A3 * Q11 + B2 * Q23 + B3 * Q22) return 0;
    if (dFabs(pp[0]*R32-pp[2]*R12) > A1 * Q32 + A3 * Q12 + B1 * Q23 + B3 * Q21) return 0;
    if (dFabs(pp[0]*R33-pp[2]*R13) > A1 * Q33 + A3 * Q13 + B1 * Q22 + B2 * Q21) return 0;

    // separating axis = u3 x (v1,v2,v3)
    if (dFabs(pp[1]*R11-pp[0]*R21) > A1 * Q21 + A2 * Q11 + B2 * Q33 + B3 * Q32) return 0;
    if (dFabs(pp[1]*R12-pp[0]*R22) > A1 * Q22 + A2 * Q12 + B1 * Q33 + B3 * Q31) return 0;
    if (dFabs(pp[1]*R13-pp[0]*R23) > A1 * Q23 + A2 * Q13 + B1 * Q32 + B2 * Q31) return 0;

    return 1;
}

//****************************************************************************
// other utility functions

/*ODE_API */void dInfiniteAABB (dxGeom *geom, dReal aabb[6])
{
    aabb[0] = -dInfinity;
    aabb[1] = dInfinity;
    aabb[2] = -dInfinity;
    aabb[3] = dInfinity;
    aabb[4] = -dInfinity;
    aabb[5] = dInfinity;
}


//****************************************************************************
// Helpers for Croteam's collider - by Nguyen Binh

int dClipEdgeToPlane( dVector3 &vEpnt0, dVector3 &vEpnt1, const dVector4& plPlane)
{
    // calculate distance of edge points to plane
    dReal fDistance0 = dCalcPointPlaneDistance(  vEpnt0 ,plPlane );
    dReal fDistance1 = dCalcPointPlaneDistance(  vEpnt1 ,plPlane );

    // if both points are behind the plane
    if ( fDistance0 < 0 && fDistance1 < 0 ) 
    {
        // do nothing
        return 0;
        // if both points in front of the plane
    } 
    else if ( fDistance0 > 0 && fDistance1 > 0 ) 
    {
        // accept them
        return 1;
        // if we have edge/plane intersection
    } else if ((fDistance0 > 0 && fDistance1 < 0) || ( fDistance0 < 0 && fDistance1 > 0)) 
    {

        // find intersection point of edge and plane
        dVector3 vIntersectionPoint;
        vIntersectionPoint[0]= vEpnt0[0]-(vEpnt0[0]-vEpnt1[0])*fDistance0/(fDistance0-fDistance1);
        vIntersectionPoint[1]= vEpnt0[1]-(vEpnt0[1]-vEpnt1[1])*fDistance0/(fDistance0-fDistance1);
        vIntersectionPoint[2]= vEpnt0[2]-(vEpnt0[2]-vEpnt1[2])*fDistance0/(fDistance0-fDistance1);

        // clamp correct edge to intersection point
        if ( fDistance0 < 0 ) 
        {
            dCopyVector3(vEpnt0, vIntersectionPoint);
        } else 
        {
            dCopyVector3(vEpnt1, vIntersectionPoint);
        }
        return 1;
    }
    return 1;
}

// clip polygon with plane and generate new polygon points
void		 dClipPolyToPlane( const dVector3 avArrayIn[], const int ctIn, 
                              dVector3 avArrayOut[], int &ctOut, 
                              const dVector4 &plPlane )
{
    // start with no output points
    ctOut = 0;

    int i0 = ctIn-1;

    // for each edge in input polygon
    for (int i1=0; i1<ctIn; i0=i1, i1++) {


        // calculate distance of edge points to plane
        dReal fDistance0 = dCalcPointPlaneDistance(  avArrayIn[i0],plPlane );
        dReal fDistance1 = dCalcPointPlaneDistance(  avArrayIn[i1],plPlane );

        // if first point is in front of plane
        if( fDistance0 >= 0 ) {
            // emit point
            avArrayOut[ctOut][0] = avArrayIn[i0][0];
            avArrayOut[ctOut][1] = avArrayIn[i0][1];
            avArrayOut[ctOut][2] = avArrayIn[i0][2];
            ctOut++;
        }

        // if points are on different sides
        if( (fDistance0 > 0 && fDistance1 < 0) || ( fDistance0 < 0 && fDistance1 > 0) ) {

            // find intersection point of edge and plane
            dVector3 vIntersectionPoint;
            vIntersectionPoint[0]= avArrayIn[i0][0] - 
                (avArrayIn[i0][0]-avArrayIn[i1][0])*fDistance0/(fDistance0-fDistance1);
            vIntersectionPoint[1]= avArrayIn[i0][1] - 
                (avArrayIn[i0][1]-avArrayIn[i1][1])*fDistance0/(fDistance0-fDistance1);
            vIntersectionPoint[2]= avArrayIn[i0][2] - 
                (avArrayIn[i0][2]-avArrayIn[i1][2])*fDistance0/(fDistance0-fDistance1);

            // emit intersection point
            avArrayOut[ctOut][0] = vIntersectionPoint[0];
            avArrayOut[ctOut][1] = vIntersectionPoint[1];
            avArrayOut[ctOut][2] = vIntersectionPoint[2];
            ctOut++;
        }
    }

}

void		 dClipPolyToCircle(const dVector3 avArrayIn[], const int ctIn, 
                               dVector3 avArrayOut[], int &ctOut, 
                               const dVector4 &plPlane ,dReal fRadius)
{
    // start with no output points
    ctOut = 0;

    int i0 = ctIn-1;

    // for each edge in input polygon
    for (int i1=0; i1<ctIn; i0=i1, i1++) 
    {
        // calculate distance of edge points to plane
        dReal fDistance0 = dCalcPointPlaneDistance(  avArrayIn[i0],plPlane );
        dReal fDistance1 = dCalcPointPlaneDistance(  avArrayIn[i1],plPlane );

        // if first point is in front of plane
        if( fDistance0 >= 0 ) 
        {
            // emit point
            if (dCalcVectorLengthSquare3(avArrayIn[i0]) <= fRadius*fRadius)
            {
                avArrayOut[ctOut][0] = avArrayIn[i0][0];
                avArrayOut[ctOut][1] = avArrayIn[i0][1];
                avArrayOut[ctOut][2] = avArrayIn[i0][2];
                ctOut++;
            }
        }

        // if points are on different sides
        if( (fDistance0 > 0 && fDistance1 < 0) || ( fDistance0 < 0 && fDistance1 > 0) ) 
        {

            // find intersection point of edge and plane
            dVector3 vIntersectionPoint;
            vIntersectionPoint[0]= avArrayIn[i0][0] - 
                (avArrayIn[i0][0]-avArrayIn[i1][0])*fDistance0/(fDistance0-fDistance1);
            vIntersectionPoint[1]= avArrayIn[i0][1] - 
                (avArrayIn[i0][1]-avArrayIn[i1][1])*fDistance0/(fDistance0-fDistance1);
            vIntersectionPoint[2]= avArrayIn[i0][2] - 
                (avArrayIn[i0][2]-avArrayIn[i1][2])*fDistance0/(fDistance0-fDistance1);

            // emit intersection point
            if (dCalcVectorLengthSquare3(avArrayIn[i0]) <= fRadius*fRadius)
            {
                avArrayOut[ctOut][0] = vIntersectionPoint[0];
                avArrayOut[ctOut][1] = vIntersectionPoint[1];
                avArrayOut[ctOut][2] = vIntersectionPoint[2];
                ctOut++;
            }
        }
    }	
}

