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

/*
standard ODE geometry primitives: public API and pairwise collision functions.
the rule is that only the low level primitive collision functions should set
dContactGeom::g1 and dContactGeom::g2.
*/

#include <ode/common.h>
#include <ode/collision.h>
#include <ode/rotation.h>
#include "config.h"
#include "matrix.h"
#include "odemath.h"
#include "collision_kernel.h"
#include "collision_std.h"
#include "collision_util.h"

#ifdef _MSC_VER
#pragma warning(disable:4291)  // for VC++, no complaints about "no matching operator delete found"
#endif

//****************************************************************************
// capped cylinder public API

dxCapsule::dxCapsule (dSpaceID space, dReal _radius, dReal _length) :
dxGeom (space,1)
{
    dAASSERT (_radius >= 0 && _length >= 0);
    type = dCapsuleClass;
    radius = _radius;
    lz = _length;
    updateZeroSizedFlag(!_radius/* || !_length -- zero length capsule is not a zero sized capsule*/);
}


void dxCapsule::computeAABB()
{
    const dMatrix3& R = final_posr->R;
    const dVector3& pos = final_posr->pos;
    dReal hlz = lz * REAL(0.5);

    dReal xrange = dFabs(R[2]  * hlz) + radius;
    dReal yrange = dFabs(R[6]  * hlz) + radius;
    dReal zrange = dFabs(R[10] * hlz) + radius;
    aabb[0] = pos[0] - xrange;
    aabb[1] = pos[0] + xrange;
    aabb[2] = pos[1] - yrange;
    aabb[3] = pos[1] + yrange;
    aabb[4] = pos[2] - zrange;
    aabb[5] = pos[2] + zrange;
}


dGeomID dCreateCapsule (dSpaceID space, dReal radius, dReal length)
{
    return new dxCapsule (space,radius,length);
}


void dGeomCapsuleSetParams (dGeomID g, dReal radius, dReal length)
{
    dUASSERT (g && g->type == dCapsuleClass,"argument not a ccylinder");
    dAASSERT (radius >= 0 && length >= 0);
    dxCapsule *c = (dxCapsule*) g;
    c->radius = radius;
    c->lz = length;
    c->updateZeroSizedFlag(!radius/* || !length -- zero length capsule is not a zero sized capsule*/);
    dGeomMoved (g);
}


void dGeomCapsuleGetParams (dGeomID g, dReal *radius, dReal *length)
{
    dUASSERT (g && g->type == dCapsuleClass,"argument not a ccylinder");
    dxCapsule *c = (dxCapsule*) g;
    *radius = c->radius;
    *length = c->lz;
}


dReal dGeomCapsulePointDepth (dGeomID g, dReal x, dReal y, dReal z)
{
    dUASSERT (g && g->type == dCapsuleClass,"argument not a ccylinder");
    g->recomputePosr();
    dxCapsule *c = (dxCapsule*) g;

    const dReal* R = g->final_posr->R;
    const dReal* pos = g->final_posr->pos;

    dVector3 a;
    a[0] = x - pos[0];
    a[1] = y - pos[1];
    a[2] = z - pos[2];
    dReal beta = dCalcVectorDot3_14(a, R + 2);
    dReal lz2 = c->lz * REAL(0.5);
    if (beta < -lz2)
        beta = -lz2;
    else if (beta > lz2)
        beta = lz2;
    a[0] -= beta*R[2];
    a[1] -= beta*R[6];
    a[2] -= beta*R[10];
    return c->radius - dCalcVectorLength3(a);
}



int dCollideCapsuleSphere (dxGeom *o1, dxGeom *o2, int flags,
                           dContactGeom *contact, int skip)
{
    dIASSERT (skip >= (int)sizeof(dContactGeom));
    dIASSERT (o1->type == dCapsuleClass);
    dIASSERT (o2->type == dSphereClass);
    dIASSERT ((flags & NUMC_MASK) >= 1);

    dxCapsule *ccyl = (dxCapsule*) o1;
    dxSphere *sphere = (dxSphere*) o2;

    contact->g1 = o1;
    contact->g2 = o2;
    contact->side1 = -1;
    contact->side2 = -1;

    dReal *pos = o1->final_posr->pos;
    dReal *cR = o1->final_posr->R;

    dReal *spherepos = o2->final_posr->pos;

    // find the point on the cylinder axis that is closest to the sphere
    dReal alpha =   cR[2]  * (spherepos[0] - pos[0]) +
                    cR[6]  * (spherepos[1] - pos[1]) +
                    cR[10] * (spherepos[2] - pos[2]);
    dReal lz2 = ccyl->lz * REAL(0.5);
    if (alpha > lz2)
        alpha = lz2;
    if (alpha < -lz2)
        alpha = -lz2;

    // collide the spheres
    dVector3 p;
    p[0] = pos[0] + alpha * cR[2];
    p[1] = pos[1] + alpha * cR[6];
    p[2] = pos[2] + alpha * cR[10];
    return dCollideSpheres (p, ccyl->radius, spherepos, sphere->radius, contact);
}


int dCollideCapsuleBox (dxGeom *o1, dxGeom *o2, int flags,
                        dContactGeom *contact, int skip)
{
    dIASSERT (skip >= (int)sizeof(dContactGeom));
    dIASSERT (o1->type == dCapsuleClass);
    dIASSERT (o2->type == dBoxClass);
    dIASSERT ((flags & NUMC_MASK) >= 1);

    dxCapsule *cyl = (dxCapsule*) o1;
    dxBox *box = (dxBox*) o2;

    contact->g1 = o1;
    contact->g2 = o2;
    contact->side1 = -1;
    contact->side2 = -1;

    // get p1,p2 = cylinder axis endpoints, get radius
    dVector3 p1,p2;
    dReal clen = cyl->lz * REAL(0.5);

    dReal *pos = o1->final_posr->pos;
    dReal *cR = o1->final_posr->R;

    dReal radius = clen * cR[2];
    p1[0] = pos[0] + radius;
    p2[0] = pos[0] - radius;
    radius = clen * cR[6];
    p1[1] = pos[1] + radius;
    p2[1] = pos[1] - radius;
    radius = clen * cR[10];
    p1[2] = pos[2] + radius;
    p2[2] = pos[2] - radius;
    radius = cyl->radius;

    // copy out box center, rotation matrix, and side array
    dReal *c = o2->final_posr->pos;
    dReal *R = o2->final_posr->R;
    const dReal *side = box->halfside;

    // get the closest point between the cylinder axis and the box
    dVector3 pl,pb;
    dClosestLineBoxPoints(p1, p2, c, R, side, pl, pb);

    // if the capsule is penetrated further than radius 
    //  then pl and pb are equal (up to mindist) -> unknown normal
    // use normal vector of closest box surface
#ifdef dSINGLE
    dReal mindist = REAL(1e-6);
#else
    dReal mindist = REAL(1e-15);
#endif

    dSubtractVectors3(p2, pl, pb);   
    if (dCalcVectorLengthSquare3(p2) < mindist)
    {
        // consider capsule as box
        dVector3 normal;
        dReal depth;
        const dVector3 capboxside = {radius, radius, clen + radius};
        int num = dBoxBox (c, R, side, 
            pos, cR, capboxside,
            normal, &depth, flags, contact, skip);

        for (int i=0; i<num; i++)
        {
            dContactGeom *currContact = CONTACT(contact,i*skip);
            dCopyVector3(currContact->normal, normal);
            currContact->g1 = o1;
            currContact->g2 = o2;
            currContact->side1 = -1;
            currContact->side2 = -1;
        }
        return num;
    }
    else
    {
        // generate contact point
        return dCollideSpheres (pl, radius, pb, 0, contact);
    }
}

int dCollideCapsuleCapsule (dxGeom *o1, dxGeom *o2,
                            int flags, dContactGeom *contact, int skip)
{
    dIASSERT (skip >= (int)sizeof(dContactGeom));
    dIASSERT (o1->type == dCapsuleClass);
    dIASSERT (o2->type == dCapsuleClass);
    dIASSERT ((flags & NUMC_MASK) >= 1);

    int i;
    const dReal tolerance = REAL(1e-5);

    dxCapsule *cyl1 = (dxCapsule*) o1;
    dxCapsule *cyl2 = (dxCapsule*) o2;

    contact->g1 = o1;
    contact->g2 = o2;
    contact->side1 = -1;
    contact->side2 = -1;

    // copy out some variables, for convenience
    dReal lz1 = cyl1->lz * REAL(0.5);
    dReal lz2 = cyl2->lz * REAL(0.5);
    dReal *pos1 = o1->final_posr->pos;
    dReal *pos2 = o2->final_posr->pos;
    dReal axis1[3],axis2[3];
    axis1[0] = o1->final_posr->R[2];
    axis1[1] = o1->final_posr->R[6];
    axis1[2] = o1->final_posr->R[10];
    axis2[0] = o2->final_posr->R[2];
    axis2[1] = o2->final_posr->R[6];
    axis2[2] = o2->final_posr->R[10];

    // if the cylinder axes are close to parallel, we'll try to detect up to
    // two contact points along the body of the cylinder. if we can't find any
    // points then we'll fall back to the closest-points algorithm. note that
    // we are not treating this special case for reasons of degeneracy, but
    // because we want two contact points in some situations. the closet-points
    // algorithm is robust in all casts, but it can return only one contact.

    dVector3 sphere1, sphere2;
    dReal a1a2 = dCalcVectorDot3 (axis1, axis2);
    dReal det = REAL(1.0) - a1a2 * a1a2;
    if (det < tolerance) {
        // the cylinder axes (almost) parallel, so we will generate up to two
        // contacts. alpha1 and alpha2 (line position parameters) are related by:
        //       alpha2 =   alpha1 + (pos1-pos2)'*axis1   (if axis1==axis2)
        //    or alpha2 = -(alpha1 + (pos1-pos2)'*axis1)  (if axis1==-axis2)
        // first compute where the two cylinders overlap in alpha1 space:
        if (a1a2 < 0)
        {
            axis2[0] = -axis2[0];
            axis2[1] = -axis2[1];
            axis2[2] = -axis2[2];
        }
        dReal q[3];
        for (i=0; i<3; i++)
            q[i] = pos1[i] - pos2[i];
        dReal k = dCalcVectorDot3 (axis1,q);
        dReal a1lo = -lz1;
        dReal a1hi = lz1;
        dReal a2lo = -lz2 - k;
        dReal a2hi = lz2 - k;
        dReal lo = (a1lo > a2lo) ? a1lo : a2lo;
        dReal hi = (a1hi < a2hi) ? a1hi : a2hi;
        if (lo <= hi) {
            int num_contacts = flags & NUMC_MASK;
            if (num_contacts >= 2 && lo < hi) {
                // generate up to two contacts. if one of those contacts is
                // not made, fall back on the one-contact strategy.
                for (i=0; i<3; i++)
                    sphere1[i] = pos1[i] + lo * axis1[i];
                for (i=0; i<3; i++)
                    sphere2[i] = pos2[i] + (lo+k) * axis2[i];
                int n1 = dCollideSpheres(sphere1, cyl1->radius,
                    sphere2, cyl2->radius, contact);
                if (n1)
                {
                    for (i=0; i<3; i++)
                        sphere1[i] = pos1[i] + hi*axis1[i];
                    for (i=0; i<3; i++)
                        sphere2[i] = pos2[i] + (hi+k)*axis2[i];
                    dContactGeom *c2 = CONTACT(contact,skip);
                    int n2 = dCollideSpheres(sphere1, cyl1->radius,
                        sphere2, cyl2->radius, c2);
                    if (n2)
                    {
                        c2->g1 = o1;
                        c2->g2 = o2;
                        c2->side1 = -1;
                        c2->side2 = -1;
                        return 2;
                    }
                }
            }

            // just one contact to generate, so put it in the middle of
            // the range
            dReal alpha1 = (lo + hi) * REAL(0.5);
            dReal alpha2 = alpha1 + k;
            for (i=0; i<3; i++)
                sphere1[i] = pos1[i] + alpha1*axis1[i];
            for (i=0; i<3; i++)
                sphere2[i] = pos2[i] + alpha2*axis2[i];
            return dCollideSpheres (sphere1, cyl1->radius,
                sphere2, cyl2->radius, contact);
        }
    }

    // use the closest point algorithm
    dVector3 a1, a2, b1, b2;

    dReal tt = axis1[0] * lz1;
    a1[0] = pos1[0] + tt;
    a2[0] = pos1[0] - tt;

    tt = axis1[1] * lz1;
    a1[1] = pos1[1] + tt;
    a2[1] = pos1[1] - tt;

    tt = axis1[2] * lz1;
    a1[2] = pos1[2] + tt;
    a2[2] = pos1[2] - tt;

    tt = axis2[0] * lz2;
    b1[0] = pos2[0] + tt;
    b2[0] = pos2[0] - tt;

    tt = axis2[1] * lz2;
    b1[1] = pos2[1] + tt;
    b2[1] = pos2[1] - tt;

    tt = axis2[2] * lz2;
    b1[2] = pos2[2] + tt;
    b2[2] = pos2[2] - tt;

    dClosestLineSegmentPoints(a1, a2, b1, b2, sphere1, sphere2);
    return dCollideSpheres(sphere1, cyl1->radius, sphere2, cyl2->radius, contact);
}


int dCollideCapsulePlane(dxGeom *o1, dxGeom *o2, int flags,
                          dContactGeom *contact, int skip)
{
    dIASSERT (skip >= (int)sizeof(dContactGeom));
    dIASSERT (o1->type == dCapsuleClass);
    dIASSERT (o2->type == dPlaneClass);
    dIASSERT ((flags & NUMC_MASK) >= 1);

    dxCapsule *ccyl = (dxCapsule*) o1;
    dxPlane *plane = (dxPlane*) o2;

    dReal *pos = o1->final_posr->pos;
    dReal *cR = o1->final_posr->R;

    // collide the deepest capping sphere with the plane
    dReal sign = (dCalcVectorDot3_14 (plane->p, cR + 2) > 0) ? REAL(-1.0) : REAL(1.0);
    
    dVector3 p;
    dReal lzsign = ccyl->lz * REAL(0.5) * sign;
    p[0] = pos[0] + cR[2]  * lzsign;
    p[1] = pos[1] + cR[6]  * lzsign;
    p[2] = pos[2] + cR[10] * lzsign;

    dReal k = dCalcVectorDot3 (p,plane->p);
    dReal depth = plane->p[3] - k + ccyl->radius;
    if (depth < 0) return 0;
    dCopyVector3(contact->normal, plane->p);
    contact->pos[0] = p[0] - plane->p[0] * ccyl->radius;
    contact->pos[1] = p[1] - plane->p[1] * ccyl->radius;
    contact->pos[2] = p[2] - plane->p[2] * ccyl->radius;
    contact->depth = depth;

    int ncontacts = 1;
    if ((flags & NUMC_MASK) >= 2) {
        // collide the other capping sphere with the plane
        p[0] = pos[0] - cR[2]  * lzsign;
        p[1] = pos[1] - cR[6]  * lzsign;
        p[2] = pos[2] - cR[10] * lzsign;

        k = dCalcVectorDot3 (p,plane->p);
        depth = plane->p[3] - k + ccyl->radius;
        if (depth >= 0) {
            dContactGeom *c2 = CONTACT(contact,skip);
            dCopyVector3(c2->normal, plane->p);
            c2->pos[0] = p[0] - plane->p[0] * ccyl->radius;
            c2->pos[1] = p[1] - plane->p[1] * ccyl->radius;
            c2->pos[2] = p[2] - plane->p[2] * ccyl->radius;
            c2->depth = depth;
            ncontacts = 2;
        }
    }

    for (int i=0; i < ncontacts; i++) {
        dContactGeom *currContact = CONTACT(contact,i*skip);
        currContact->g1 = o1;
        currContact->g2 = o2;
        currContact->side1 = -1;
        currContact->side2 = -1;
    }
    return ncontacts;
}

