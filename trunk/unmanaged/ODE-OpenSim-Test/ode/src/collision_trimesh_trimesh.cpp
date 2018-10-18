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

 // OPCODE TriMesh/TriMesh collision code
 // Written at 2006-10-28 by Francisco León (http://gimpact.sourceforge.net)

#ifdef _MSC_VER
#pragma warning(disable:4244 4305)  // for VC++, no precision loss complaints
#endif

#include <ode/collision.h>
#include <ode/rotation.h>
#include "config.h"
#include "matrix.h"
#include "odemath.h"


#include "collision_util.h"
#include "collision_trimesh_internal.h"


#if !dTLS_ENABLED
// Have collider cache instance unconditionally of OPCODE or GIMPACT selection
/*extern */TrimeshCollidersCache g_ccTrimeshCollidersCache;
#endif


#define SMALL_ELT           REAL(2.5e-4)
#define EXPANDED_ELT_THRESH REAL(1.0e-3)
#define DISTANCE_EPSILON    REAL(1.0e-8)
#define VELOCITY_EPSILON    REAL(1.0e-5)
#define TINY_PENETRATION    REAL(5.0e-6)

// static void GetTriangleGeometryCallback(udword, VertexPointers&, udword); -- not used
inline void dMakeMatrix4(const dVector3 Position, const dMatrix3 Rotation, dMatrix4 &B);

dReal TriTriOverlap(const dVector3 tri1[3], const dVector3 edges1[3], const dVector4 tri1plane,
    const dVector3 tri2[3], const dVector3 edges2[3], const dVector4 tri2plane,
    dVector3 separating_normal, dVector3 deep_points[], int &ndeep_points);

/* some math macros */
#define IS_ZERO(v) (!(v)[0] && !(v)[1] && !(v)[2])

static inline dReal dMin(const dReal x, const dReal y)
{
    return x < y ? x : y;
}

inline void
SwapNormals(dVector3 *&pen_v, dVector3 *&col_v, dVector3* v1, dVector3* v2,
    dVector3 *&pen_elt, dVector3 *elt_f1, dVector3 *elt_f2,
    dVector3 n, dVector3 n1, dVector3 n2)
{
    if (pen_v == v1) {
        pen_v = v2;
        pen_elt = elt_f2;
        col_v = v1;
        dCopyVector3r4(n, n1);
    }
    else {
        pen_v = v1;
        pen_elt = elt_f1;
        col_v = v2;
        dCopyVector3r4(n, n2);
    }
}

///////////////////////MECHANISM FOR AVOID CONTACT REDUNDANCE///////////////////////////////
////* Written by Francisco León (http://gimpact.sourceforge.net) *///
#define CONTACT_DIFF_EPSILON REAL(0.00001)
#if defined(dDOUBLE)
#define CONTACT_NORMAL_ZERO REAL(0.0000001)
#else // if defined(dSINGLE)
#define CONTACT_NORMAL_ZERO REAL(0.00001)
#endif
#define CONTACT_POS_HASH_QUOTIENT REAL(10000.0)
#define dSQRT3	REAL(1.7320508075688773)

void UpdateContactKey(CONTACT_KEY & key, dContactGeom * contact)
{
    key.m_contact = contact;

    unsigned int hash = 0;

    int i = 0;

    while (true)
    {
        dReal coord = contact->pos[i];
        coord = dFloor(coord * CONTACT_POS_HASH_QUOTIENT);

        const int sz = sizeof(coord) / sizeof(unsigned);
        dIASSERT(sizeof(coord) % sizeof(unsigned) == 0);

        unsigned hash_v[sz];
        memcpy(hash_v, &coord, sizeof(coord));

        unsigned int hash_input = hash_v[0];
        for (int i = 1; i < sz; ++i)
            hash_input ^= hash_v[i];

        hash = ((hash << 4) + (hash_input >> 24)) ^ (hash >> 28);
        hash = ((hash << 4) + ((hash_input >> 16) & 0xFF)) ^ (hash >> 28);
        hash = ((hash << 4) + ((hash_input >> 8) & 0xFF)) ^ (hash >> 28);
        hash = ((hash << 4) + (hash_input & 0xFF)) ^ (hash >> 28);

        if (++i == 3)
        {
            break;
        }

        hash = (hash << 11) | (hash >> 21);
    }

    key.m_key = hash;
}


static inline unsigned int MakeContactIndex(unsigned int key)
{
    dIASSERT(CONTACTS_HASHSIZE == 256);

    unsigned int index = key ^ (key >> 16);
    index = (index ^ (index >> 8)) & 0xFF;

    return index;
}

dContactGeom *AddContactToNode(const CONTACT_KEY * contactkey, CONTACT_KEY_HASH_NODE * node)
{
    for (int i = 0; i < node->m_keycount; i++)
    {
        if (node->m_keyarray[i].m_key == contactkey->m_key)
        {
            dContactGeom *contactfound = node->m_keyarray[i].m_contact;
            if (dCalcPointsDistanceSquare3(contactfound->pos, contactkey->m_contact->pos) < REAL(1.00001) /*for comp. errors*/ * dSQRT3 / CONTACT_POS_HASH_QUOTIENT /*cube diagonal*/)
            {
                return contactfound;
            }
        }
    }

    if (node->m_keycount < MAXCONTACT_X_NODE)
    {
        node->m_keyarray[node->m_keycount].m_contact = contactkey->m_contact;
        node->m_keyarray[node->m_keycount].m_key = contactkey->m_key;
        node->m_keycount++;
    }
    else
    {
        dDEBUGMSG("Trimesh-trimesh contach hash table bucket overflow - close contacts might not be culled");
    }

    return contactkey->m_contact;
}

void RemoveNewContactFromNode(const CONTACT_KEY * contactkey, CONTACT_KEY_HASH_NODE * node)
{
    dIASSERT(node->m_keycount > 0);

    if (node->m_keyarray[node->m_keycount - 1].m_contact == contactkey->m_contact)
    {
        node->m_keycount -= 1;
    }
    else
    {
        dIASSERT(node->m_keycount == MAXCONTACT_X_NODE);
    }
}

void RemoveArbitraryContactFromNode(const CONTACT_KEY *contactkey, CONTACT_KEY_HASH_NODE *node)
{
    dIASSERT(node->m_keycount > 0);

    int keyindex, lastkeyindex = node->m_keycount - 1;

    // Do not check the last contact
    for (keyindex = 0; keyindex < lastkeyindex; keyindex++)
    {
        if (node->m_keyarray[keyindex].m_contact == contactkey->m_contact)
        {
            node->m_keyarray[keyindex] = node->m_keyarray[lastkeyindex];
            break;
        }
    }

    dIASSERT(keyindex < lastkeyindex ||
        node->m_keyarray[keyindex].m_contact == contactkey->m_contact); // It has been either the break from loop or last element should match

    node->m_keycount = lastkeyindex;
}

void UpdateArbitraryContactInNode(const CONTACT_KEY *contactkey, CONTACT_KEY_HASH_NODE *node,
    dContactGeom *pwithcontact)
{
    dIASSERT(node->m_keycount > 0);

    int keyindex, lastkeyindex = node->m_keycount - 1;

    // Do not check the last contact
    for (keyindex = 0; keyindex < lastkeyindex; keyindex++)
    {
        if (node->m_keyarray[keyindex].m_contact == contactkey->m_contact)
        {
            break;
        }
    }

    dIASSERT(keyindex < lastkeyindex ||
        node->m_keyarray[keyindex].m_contact == contactkey->m_contact); // It has been either the break from loop or last element should match

    node->m_keyarray[keyindex].m_contact = pwithcontact;
}

void ClearContactSet(CONTACT_KEY_HASH_TABLE &hashcontactset)
{
    memset(&hashcontactset, 0, sizeof(CONTACT_KEY_HASH_TABLE));
}

//return true if found
dContactGeom *InsertContactInSet(CONTACT_KEY_HASH_TABLE &hashcontactset, const CONTACT_KEY &newkey)
{
    unsigned int index = MakeContactIndex(newkey.m_key);

    return AddContactToNode(&newkey, &hashcontactset[index]);
}

void RemoveNewContactFromSet(CONTACT_KEY_HASH_TABLE &hashcontactset, const CONTACT_KEY &contactkey)
{
    unsigned int index = MakeContactIndex(contactkey.m_key);

    RemoveNewContactFromNode(&contactkey, &hashcontactset[index]);
}

void RemoveArbitraryContactFromSet(CONTACT_KEY_HASH_TABLE &hashcontactset, const CONTACT_KEY &contactkey)
{
    unsigned int index = MakeContactIndex(contactkey.m_key);

    RemoveArbitraryContactFromNode(&contactkey, &hashcontactset[index]);
}

void UpdateArbitraryContactInSet(CONTACT_KEY_HASH_TABLE &hashcontactset, const CONTACT_KEY &contactkey,
    dContactGeom *pwithcontact)
{
    unsigned int index = MakeContactIndex(contactkey.m_key);

    UpdateArbitraryContactInNode(&contactkey, &hashcontactset[index], pwithcontact);
}

bool AllocNewContact(const dVector3 newpoint, dContactGeom *& out_pcontact,
    int Flags, CONTACT_KEY_HASH_TABLE &hashcontactset,
    dContactGeom* Contacts, int Stride, int &contactcount)
{
    dContactGeom dLocalContact;
    dContactGeom *pcontact;
    dContactGeom *pcontactfound;
    bool allocated_new = false;

    pcontact = contactcount != (Flags & NUMC_MASK) ?
        SAFECONTACT(Flags, Contacts, contactcount, Stride) : &dLocalContact;

    dCopyVector3r4(pcontact->pos, newpoint);

    CONTACT_KEY newkey;
    UpdateContactKey(newkey, pcontact);

    pcontactfound = InsertContactInSet(hashcontactset, newkey);
    if (pcontactfound == pcontact)
    {
        if (pcontactfound != &dLocalContact)
        {
            contactcount++;
        }
        else
        {
            RemoveNewContactFromSet(hashcontactset, newkey);
            pcontactfound = NULL;
        }
        allocated_new = true;
    }

    out_pcontact = pcontactfound;
    return allocated_new;
}

void FreeExistingContact(dContactGeom *pcontact,
    int Flags, CONTACT_KEY_HASH_TABLE &hashcontactset,
    dContactGeom *Contacts, int Stride, int &contactcount)
{
    dContactGeom *plastContact;
    CONTACT_KEY contactKey;
    UpdateContactKey(contactKey, pcontact);

    RemoveArbitraryContactFromSet(hashcontactset, contactKey);

    int lastContactIndex = contactcount - 1;
    plastContact = SAFECONTACT(Flags, Contacts, lastContactIndex, Stride);

    if (pcontact != plastContact)
    {
        *pcontact = *plastContact;

        CONTACT_KEY lastContactKey;
        UpdateContactKey(lastContactKey, plastContact);

        UpdateArbitraryContactInSet(hashcontactset, lastContactKey, pcontact);
    }

    contactcount = lastContactIndex;
}


dContactGeom *  PushNewContact(dxGeom* g1, dxGeom* g2, int TriIndex1, int TriIndex2,
    const dVector3 point, dVector3 normal, dReal  depth,
    int Flags, CONTACT_KEY_HASH_TABLE &hashcontactset,
    dContactGeom* Contacts, int Stride, int &contactcount)
{
    dContactGeom *pcontact;

    if (!AllocNewContact(point, pcontact, Flags, hashcontactset, Contacts, Stride, contactcount))
    {
        const dReal depthDifference = depth - pcontact->depth;
        if (depthDifference > CONTACT_DIFF_EPSILON)
        {
            dCopyVector3r4(pcontact->normal, normal);
            pcontact->depth = depth;

            pcontact->g1 = g1;
            pcontact->g2 = g2;
            pcontact->side1 = TriIndex1;
            pcontact->side2 = TriIndex2;
        }
    }
    // Contact can be not available if buffer is full
    else if (pcontact)
    {
        dCopyVector3r4(pcontact->normal, normal);
        pcontact->depth = depth;
        pcontact->g1 = g1;
        pcontact->g2 = g2;
        pcontact->side1 = TriIndex1;
        pcontact->side2 = TriIndex2;
    }

    return pcontact;
}

////////////////////////////////////////////////////////////////////////////////////////////

bool BuildPlane(dVector4 plane, const dVector3 s0, const dVector3 s1, const dVector3 s2)
{
    dVector3 e0, e1;
    dSubtractVectors3r4(e0, s1, s0);
    dSubtractVectors3r4(e1, s2, s0);

    dCalcVectorCross3(plane, e0, e1);

    if (!dSafeNormalize3(plane))
    {
        return false;
    }

    plane[3] = -dCalcVectorDot3(plane, s0);
    return true;
}

bool BuildPlaneFromEdges(dVector4 plane, const dVector3 e0, const dVector3 e1, const dVector3 s0)
{
    dCalcVectorCross3r4(plane, e0, e1);
    if (!dSafeNormalize3(plane))
    {
        return false;
    }
    plane[3] = -dCalcVectorDot3(plane, s0);
    return true;
}

bool BuildEdgePlane(const dVector3 s0, const dVector3 s1,
    const dVector3 normal, dVector4 plane_normal)
{
    dVector3 e0;
    dSubtractVectors3r4(e0, s1, s0);
    dCalcVectorCross3r4(plane_normal, e0, normal);
    if (!dSafeNormalize3(plane_normal))
    {
        return false;
    }
    plane_normal[3] = -dCalcVectorDot3(plane_normal, s0);
    return true;
}

bool BuildEdgePlaneFromEdge(dVector4 plane_normal, const dVector3 s0, const dVector3 e0, const dVector3 normal)
{
    dCalcVectorCross3r4(plane_normal, e0, normal);
    if (!dSafeNormalize3(plane_normal))
    {
        return false;
    }
    plane_normal[3] = -dCalcVectorDot3(plane_normal, s0);
    return true;
}

#define B11   B[0]
#define B12   B[1]
#define B13   B[2]
#define B14   B[3]
#define B21   B[4]
#define B22   B[5]
#define B23   B[6]
#define B24   B[7]
#define B31   B[8]
#define B32   B[9]
#define B33   B[10]
#define B34   B[11]
#define B41   B[12]
#define B42   B[13]
#define B43   B[14]
#define B44   B[15]

#define Binv11   Binv[0]
#define Binv12   Binv[1]
#define Binv13   Binv[2]
#define Binv14   Binv[3]
#define Binv21   Binv[4]
#define Binv22   Binv[5]
#define Binv23   Binv[6]
#define Binv24   Binv[7]
#define Binv31   Binv[8]
#define Binv32   Binv[9]
#define Binv33   Binv[10]
#define Binv34   Binv[11]
#define Binv41   Binv[12]
#define Binv42   Binv[13]
#define Binv43   Binv[14]
#define Binv44   Binv[15]

inline void
dMakeMatrix4(const dVector3 Position, const dMatrix3 Rotation, dMatrix4 &B)
{
    B11 = Rotation[0]; B21 = Rotation[1]; B31 = Rotation[2];    B41 = Position[0];
    B12 = Rotation[4]; B22 = Rotation[5]; B32 = Rotation[6];    B42 = Position[1];
    B13 = Rotation[8]; B23 = Rotation[9]; B33 = Rotation[10];   B43 = Position[2];

    B14 = 0.0;         B24 = 0.0;         B34 = 0.0;            B44 = 1.0;
}

/* ClipConvexPolygonAgainstPlane - Clip a a convex polygon, described by
CONTACTS, with a plane (described by N and C distance from origin).
Note:  the input vertices are assumed to be in invcounterclockwise order.
changed by Francisco Leon (http://gimpact.sourceforge.net) */
static void ClipConvexPolygonAgainstPlane(const dVector3 inpoints[], const int ninpoints, const dVector4 N, dVector3 clipped[], int& nclipped)
{
    const dVector3 *v0;
    const dVector3 *v1;
    dReal d0;
    dReal d1;
    dReal factor;
    int  i;

    nclipped = 0;

    v0 = &inpoints[ninpoints - 1];
    d0 = dCalcPointPlaneDistance(*v0, N);

    for (i = 0; i < ninpoints; i++)
    {
        v1 = &inpoints[i];
        d1 = dCalcPointPlaneDistance(*v1, N);

        if (d0 <= dEpsilon) //back
        {
            dCopyVector3r4(clipped[nclipped], *v0);
            nclipped++;

            if (d1 > dEpsilon)// front
            {
                ///add clipped point
                factor = d0 / (d0 - d1);
                dCalcLerpVectors3r4(clipped[nclipped], *v0, *v1, factor);
                nclipped++;
            }
        }
        else if (d1 <= dEpsilon)
        {
            factor = d0 / (d0 - d1);
            dCalcLerpVectors3r4(clipped[nclipped], *v0, *v1, factor);
            nclipped++;
        }
        d0 = d1;
        v0 = v1;
    }
    return;
}

///returns the penetration depth
dReal MostDeepPoints(const dVector3 inpoints[], int& ninpoints, const dVector4 plane_normal, dVector3 deep[], int& ndeep)
{
    int i;
    int max_candidates[9];
    dReal maxdeep = -dInfinity;
    dReal dist;

    ndeep = 0;
    for (i = 0; i < ninpoints; i++)
    {
        dist = -dCalcPointPlaneDistance(inpoints[i], plane_normal);
        if (dist > -dEpsilon)
        {
            if (dist > maxdeep)
            {
                maxdeep = dist;
                max_candidates[0] = i;
                ndeep = 1;
            }
            else if (dist + dEpsilon >= maxdeep)
            {
                max_candidates[ndeep] = i;
                ndeep++;
            }
        }
    }

    for (i = 0; i < ndeep; i++)
    {
        dCopyVector3r4(deep[i], inpoints[max_candidates[i]]);
    }

    return maxdeep;
}

void ClipPointsByTri(const dVector3 points[], int pointcount, const dVector3 tri[3], const dVector3 edges[3],
    const dVector4 triplanenormal, dVector3 clipped_points[], int& nclipped_points)
{
    ///build edges planes
    dVector4 plane;
    dVector3 clippedA[9];
    dVector3 clippedB[9];
    int nclippedA = 0;
    int nclippedB = 0;
    nclipped_points = 0;
    if (BuildEdgePlaneFromEdge(plane, tri[0], edges[0], triplanenormal))
        ClipConvexPolygonAgainstPlane(points, pointcount, plane, clippedA, nclippedA);

    if (nclippedA == 0)
        return;

    if (BuildEdgePlaneFromEdge(plane, tri[1], edges[1], triplanenormal))
        ClipConvexPolygonAgainstPlane(clippedA, nclippedA, plane, clippedB, nclippedB);

    if (nclippedB == 0)
        return;

    if (BuildEdgePlaneFromEdge(plane, tri[2], edges[2], triplanenormal))
        ClipConvexPolygonAgainstPlane(clippedB, nclippedB, plane, clipped_points, nclipped_points);
}

///returns the penetration depth
dReal FindTriangleTriangleCollision(const dVector3 tri1[3], const dVector3 edges1[3], const dVector4 tri1plane,
    const dVector3 tri2[3], const dVector3 edges2[3], const dVector4 tri2plane,
    dVector3 separating_normal, dVector3 deep_points[], int &ndeep_points)
{
    dVector3 clipped_points[9];
    dVector3 deep_points1[9];
    dVector3 deep_points2[9];

    int nclipped_points;
    int ndeep_points1;
    int ndeep_points2;

    dReal maxdeep = dInfinity;
    dReal dist;

    ////find interval face1
    ClipPointsByTri(tri2, 3, tri1, edges1, tri1plane, clipped_points, nclipped_points);
    if (nclipped_points == 0)
        return -1;

    maxdeep = MostDeepPoints(clipped_points, nclipped_points, tri1plane, deep_points1, ndeep_points1);
    if (ndeep_points1 == 0)
        return -1;

    ////find interval face2
    ClipPointsByTri(tri1, 3, tri2, edges2, tri2plane, clipped_points, nclipped_points);
    if (nclipped_points == 0)
        return -1;

    dist = MostDeepPoints(clipped_points, nclipped_points, tri2plane, deep_points2, ndeep_points2);
    if (ndeep_points2 == 0)
        return -1;

    if (dist < maxdeep)
    {
        ndeep_points = ndeep_points2;
        memcpy(&deep_points[0], &deep_points2[0], ndeep_points2 * sizeof(dVector3));
        dCopyVector3r4(separating_normal, tri2plane);
        return dist;
    }

    if (ndeep_points1 == 0)
        return -1;

    ndeep_points = ndeep_points1;
    memcpy(&deep_points[0], &deep_points1[0], ndeep_points1 * sizeof(dVector3));
    dCopyNegatedVector3r4(separating_normal, tri1plane);

    return maxdeep;
}

int dCollideTTL(dxGeom* g1, dxGeom* g2, int Flags, dContactGeom* Contacts, int Stride)
{
    dIASSERT(Stride >= (int)sizeof(dContactGeom));
    dIASSERT(g1->type == dTriMeshClass);
    dIASSERT(g2->type == dTriMeshClass);
    dIASSERT((Flags & NUMC_MASK) >= 1);

    dVector3 contactpoints[9];
    dVector4 normal;
    dVector4 tri1plane;
    dVector4 tri2plane;
    dVector3 v1[3], v2[3];
    dVector3 v1e[3], v2e[3];

    dxTriMesh* TriMesh1 = (dxTriMesh*)g1;
    dxTriMesh* TriMesh2 = (dxTriMesh*)g2;

    const dVector3& TLPosition1 = *(const dVector3*)dGeomGetPosition(TriMesh1);
    // TLRotation1 = column-major order
    const dMatrix3& TLRotation1 = *(const dMatrix3*)dGeomGetRotation(TriMesh1);

    const dVector3& TLPosition2 = *(const dVector3*)dGeomGetPosition(TriMesh2);
    // TLRotation2 = column-major order
    const dMatrix3& TLRotation2 = *(const dMatrix3*)dGeomGetRotation(TriMesh2);
    Matrix4x4 amatrix, bmatrix;

    const unsigned uiTLSKind = TriMesh1->getParentSpaceTLSKind();
    dIASSERT(uiTLSKind == TriMesh2->getParentSpaceTLSKind()); // The colliding spaces must use matching cleanup method
    TrimeshCollidersCache *pccColliderCache = GetTrimeshCollidersCache(uiTLSKind);
    AABBTreeCollider& Collider = pccColliderCache->_AABBTreeCollider;
    BVTCache &ColCache = pccColliderCache->ColCache;
    CONTACT_KEY_HASH_TABLE &hashcontactset = pccColliderCache->_hashcontactset;

    ColCache.Model0 = &TriMesh1->Data->BVTree;
    ColCache.Model1 = &TriMesh2->Data->BVTree;

    const bool contacts_unimportant = (Flags & CONTACTS_UNIMPORTANT) != 0;
    const int max_contacts = (Flags & NUMC_MASK);

    ////Prepare contact list
    ClearContactSet(hashcontactset);

    // Collision query
    BOOL IsOk = Collider.Collide(ColCache,
        &MakeMatrix(TLPosition1, TLRotation1, amatrix),
        &MakeMatrix(TLPosition2, TLRotation2, bmatrix));

    if (IsOk && Collider.GetContactStatus())
    {
        // Number of colliding pairs and list of pairs
        int TriCount = Collider.GetNbPairs();

        if (TriCount == 0)
            return 0;

        const Pair* CollidingPairs = Collider.GetPairs();

        // step through the pairs, adding contacts
        int lastid1 = -11111;
        int lastid2 = -11111;
        int OutContactsCount = 0;

        for (int i = 0; i < TriCount; i++)
        {
            int id1 = CollidingPairs[i].id0;
            int id2 = CollidingPairs[i].id1;
            int ccount;
            int ncontactpoints;
            dReal depth;

            // grab the colliding triangles
            if (lastid1 != id1)
            {
                FetchTriangle((dxTriMesh*)g1, id1, TLPosition1, TLRotation1, v1);
                dSubtractVectors3r4(v1e[0], v1[1], v1[0]);
                dSubtractVectors3r4(v1e[1], v1[2], v1[1]);
                if (!BuildPlaneFromEdges(tri1plane, v1e[0], v1e[1], v1[0]))
                    continue;
                dSubtractVectors3r4(v1e[2], v1[0], v1[2]);
                lastid1 = id1;
            }
            else if (lastid2 == id2)
                continue;

            if (lastid2 != id2)
            {
                FetchTriangle((dxTriMesh*)g2, id2, TLPosition2, TLRotation2, v2);
                dSubtractVectors3r4(v2e[0], v2[1], v2[0]);
                dSubtractVectors3r4(v2e[1], v2[2], v2[1]);
                if (!BuildPlaneFromEdges(tri2plane, v2e[0], v2e[1], v2[0]))
                    continue;
                dSubtractVectors3r4(v2e[2], v2[0], v2[2]);
                lastid2 = id2;
            }

            ncontactpoints = 0;

            ///find best direction
            depth = TriTriOverlap(v1, v1e, tri1plane, v2, v2e, tri2plane, normal, contactpoints, ncontactpoints);

            if (depth < dEpsilon)
                continue;

            depth = FindTriangleTriangleCollision(v1, v1e, tri1plane, v2, v2e, tri2plane, normal, contactpoints, ncontactpoints);

            if (depth < dEpsilon)
                continue;

            for (ccount = 0; ccount < ncontactpoints; ccount++)
            {
                PushNewContact(g1, g2, id1, id1,
                    contactpoints[ccount], normal, depth, Flags, hashcontactset,
                    Contacts, Stride, OutContactsCount);

                if (contacts_unimportant)
                    break;
                if (OutContactsCount >= max_contacts)
                    break;
            }

            if (contacts_unimportant)
                break;
            if (OutContactsCount >= max_contacts)
                break;
        }

        // Return the number of contacts
        return OutContactsCount;
    }

    // There was some kind of failure during the Collide call or
    // there are no faces overlapping
    return 0;
}

//*****************************************************************************************

#define LOCAL_EPSILON 0.000001f

//! sort so that a<=b
#define SORT(a,b)			\
	if(a>b)					\
	{						\
		const float c=a;	\
		a=b;				\
		b=c;				\
	}

//! Edge to edge test based on Franlin Antonio's gem: "Faster Line Segment Intersection", in Graphics Gems III, pp. 199-202
#define EDGE_EDGE_TEST(V0, U0, U1)						\
	Bx = U0[i0] - U1[i0];								\
	By = U0[i1] - U1[i1];								\
	Cx = V0[i0] - U0[i0];								\
	Cy = V0[i1] - U0[i1];								\
	f  = Ay * Bx - Ax * By;								\
	d  = By * Cx - Bx * Cy;								\
	if((f>0.0f && d>=0.0f && d<=f) || (f<0.0f && d<=0.0f && d>=f))	\
	{													\
		const float e = Ax * Cy - Ay * Cx;				\
		if(f > 0.0f)									\
		{												\
			if(e >= 0.0f && e <= f) return 1;     		\
		}												\
		else											\
		{												\
			if(e <= 0.0f && e >= f) return 1;	    	\
		}												\
	}

//! TO BE DOCUMENTED
#define EDGE_AGAINST_TRI_EDGES(V0, V1, U0, U1, U2)		\
{														\
	float Bx,By,Cx,Cy,d,f;								\
	const float Ax = V1[i0] - V0[i0];					\
	const float Ay = V1[i1] - V0[i1];					\
	/* test edge U0,U1 against V0,V1 */					\
	EDGE_EDGE_TEST(V0, U0, U1);							\
	/* test edge U1,U2 against V0,V1 */					\
	EDGE_EDGE_TEST(V0, U1, U2);							\
	/* test edge U2,U1 against V0,V1 */					\
	EDGE_EDGE_TEST(V0, U2, U0);							\
}

//! TO BE DOCUMENTED
#define POINT_IN_TRI(V0, U0, U1, U2)					\
{														\
	/* is T1 completly inside T2? */					\
	/* check if V0 is inside tri(U0,U1,U2) */			\
	float a  = U1[i1] - U0[i1];							\
	float b  = -(U1[i0] - U0[i0]);						\
	float c  = -a*U0[i0] - b*U0[i1];					\
	float d0 = a * V0[i0] + b * V0[i1] + c;				\
														\
	a  = U2[i1] - U1[i1];								\
	b  = -(U2[i0] - U1[i0]);							\
	c  = -a*U1[i0] - b*U1[i1];							\
	const float d1 = a * V0[i0] + b * V0[i1] + c;		\
														\
	a  = U0[i1] - U2[i1];								\
	b  = -(U0[i0] - U2[i0]);							\
	c  = -a*U2[i0] - b*U2[i1];							\
	const float d2 = a * V0[i0] + b *V0[i1] + c;		\
	if(d0 * d1 > 0.0f)									\
	{													\
		if(d0 * d2 > 0.0f) return 1;  					\
	}													\
}

//! TO BE DOCUMENTED
dReal CoplanarTriTri(const dVector3 n, const dVector3 v0, const dVector3 v1, const dVector3 v2, const dVector3 u0, const dVector3 u1, const dVector3 u2)
{
    float A[3];
    short i0, i1;
    /* first project onto an axis-aligned plane, that maximizes the area */
    /* of the triangles, compute indices: i0,i1. */
    A[0] = fabsf(n[0]);
    A[1] = fabsf(n[1]);
    A[2] = fabsf(n[2]);
    if (A[0] > A[1])
    {
        if (A[0] > A[2])
        {
            i0 = 1;      /* A[0] is greatest */
            i1 = 2;
        }
        else
        {
            i0 = 0;      /* A[2] is greatest */
            i1 = 1;
        }
    }
    else   /* A[0]<=A[1] */
    {
        if (A[2] > A[1])
        {
            i0 = 0;      /* A[2] is greatest */
            i1 = 1;
        }
        else
        {
            i0 = 0;      /* A[1] is greatest */
            i1 = 2;
        }
    }

    /* test all edges of triangle 1 against the edges of triangle 2 */
    EDGE_AGAINST_TRI_EDGES(v0, v1, u0, u1, u2);
    EDGE_AGAINST_TRI_EDGES(v1, v2, u0, u1, u2);
    EDGE_AGAINST_TRI_EDGES(v2, v0, u0, u1, u2);

    /* finally, test if tri1 is totally contained in tri2 or vice versa */
    POINT_IN_TRI(v0, u0, u1, u2);
    POINT_IN_TRI(u0, v0, v1, v2);

    return 0;
}

//! TO BE DOCUMENTED
#define NEWCOMPUTE_INTERVALS(VV0, VV1, VV2, D0, D1, D2, D0D1, D0D2, A, B, C, X0, X1)	\
{																						\
	if(D0D1 > 0.0f)																		\
	{																					\
		/* here we know that D0D2<=0.0 */												\
		/* that is D0, D1 are on the same side, D2 on the other or on the plane */		\
		A=VV2; B=(VV0 - VV2)*D2; C=(VV1 - VV2)*D2; X0=D2 - D0; X1=D2 - D1;				\
	}																					\
	else if(D0D2 > 0.0f)																\
	{																					\
		/* here we know that d0d1<=0.0 */												\
		A=VV1; B=(VV0 - VV1)*D1; C=(VV2 - VV1)*D1; X0=D1 - D0; X1=D1 - D2;				\
	}																					\
	else if(D0 != 0.0f || D1*D2 > 0.0f)													\
	{																					\
		/* here we know that d0d1<=0.0 or that D0!=0.0 */								\
		A=VV0; B=(VV1 - VV0)*D0; C=(VV2 - VV0)*D0; X0=D0 - D1; X1=D0 - D2;				\
	}																					\
	else if(D1 != 0.0f)																	\
	{																					\
		A=VV1; B=(VV0 - VV1)*D1; C=(VV2 - VV1)*D1; X0=D1 - D0; X1=D1 - D2;				\
	}																					\
	else if(D2 != 0.0f)																	\
	{																					\
		A=VV2; B=(VV0 - VV2)*D2; C=(VV1 - VV2)*D2; X0=D2 - D0; X1=D2 - D1;				\
	}																					\
	else																				\
	{																					\
		/* triangles are coplanar */													\
		return CoplanarTriTri(tri1plane, tri1[0], tri1[1], tri1[2], tri2[0], tri2[1], tri2[2]);	\
	}																					\
}

#define SORT2(a,b,smallest)       \
             if(a>b)       \
             {             \
               float c;    \
               c=a;        \
               a=b;        \
               b=c;        \
               smallest=1; \
             }             \
             else smallest=0;

inline void isect2(const dVector3 VTX0, const dVector3 VTX1, const dVector3 VTX2, float VV0, float VV1, float VV2,
    float D0, float D1, float D2, float *isect0, float *isect1, dVector3 isectpoint0, dVector3 isectpoint1)
{
    float tmp = D0 / (D0 - D1);
    *isect0 = VV0 + (VV1 - VV0) * tmp;
    dCalcLerpVectors3r4(isectpoint0, VTX0, VTX1, tmp);
    tmp = D0 / (D0 - D2);
    *isect1 = VV0 + (VV2 - VV0)*tmp;
    dCalcLerpVectors3r4(isectpoint1, VTX0, VTX2, tmp);
}

inline int compute_intervals_isectline(const dVector3 VERT0, const dVector3 VERT1, const dVector3 VERT2,
    float VV0, float VV1, float VV2, float D0, float D1, float D2,
    float D0D1, float D0D2, float *isect0, float *isect1,
    dVector3 isectpoint0, dVector3 isectpoint1)
{
    if (D0D1 > 0.0f)
    {
        /* here we know that D0D2<=0.0 */
        /* that is D0, D1 are on the same side, D2 on the other or on the plane */
        isect2(VERT2, VERT0, VERT1, VV2, VV0, VV1, D2, D0, D1, isect0, isect1, isectpoint0, isectpoint1);
    }
    else if (D0D2 > 0.0f)
    {
        /* here we know that d0d1<=0.0 */
        isect2(VERT1, VERT0, VERT2, VV1, VV0, VV2, D1, D0, D2, isect0, isect1, isectpoint0, isectpoint1);
    }
    else if (D1*D2 > 0.0f || D0 != 0.0f)
    {
        /* here we know that d0d1<=0.0 or that D0!=0.0 */
        isect2(VERT0, VERT1, VERT2, VV0, VV1, VV2, D0, D1, D2, isect0, isect1, isectpoint0, isectpoint1);
    }
    else if (D1 != 0.0f)
    {
        isect2(VERT1, VERT0, VERT2, VV1, VV0, VV2, D1, D0, D2, isect0, isect1, isectpoint0, isectpoint1);
    }
    else if (D2 != 0.0f)
    {
        isect2(VERT2, VERT0, VERT1, VV2, VV0, VV1, D2, D0, D1, isect0, isect1, isectpoint0, isectpoint1);
    }
    else
    {
        /* triangles are coplanar */
        return 1;
    }
    return 0;
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/**
*	Triangle/triangle intersection test routine,
*	by Tomas Moller, 1997.
*	See article "A Fast Triangle-Triangle Intersection Test",
*	Journal of Graphics Tools, 2(2), 1997
*
*	Updated June 1999: removed the divisions -- a little faster now!
*	Updated October 1999: added {} to CROSS and SUB macros
*
*	int NoDivTriTriIsect(float V0[3],float V1[3],float V2[3],
*                      float U0[3],float U1[3],float U2[3])
*
*	\param		V0		[in] triangle 0, vertex 0
*	\param		V1		[in] triangle 0, vertex 1
*	\param		V2		[in] triangle 0, vertex 2
*	\param		U0		[in] triangle 1, vertex 0
*	\param		U1		[in] triangle 1, vertex 1
*	\param		U2		[in] triangle 1, vertex 2
*	\return		true if triangles overlap
*/
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
dReal TriTriOverlap(const dVector3 tri1[3], const dVector3 edges1[3], const dVector4 tri1plane,
    const dVector3 tri2[3], const dVector3 edges2[3], const dVector4 tri2plane,
    dVector3 separating_normal, dVector3 deep_points[], int &ndeep_points)
{

    dVector3 isectpointA1, isectpointA2;
    dVector3 isectpointB1, isectpointB2;
    dVector3 isectpt1, isectpt2;
    int smallest1, smallest2;

    ndeep_points = 0;
    // Put U0,U1,U2 into plane equation 1 to compute signed distances to the plane
    float du0 = dCalcPointPlaneDistance(tri2[0], tri1plane);
    float du1 = dCalcPointPlaneDistance(tri2[1], tri1plane);
    float du2 = dCalcPointPlaneDistance(tri2[2], tri1plane);

    if (fabs(du0) < dEpsilon) du0 = 0.0;
    if (fabs(du1) < dEpsilon) du1 = 0.0;
    if (fabs(du2) < dEpsilon) du2 = 0.0;

    if (fabs(du0) < 0.1f && fabs(du1) < 0.1f && fabs(du2) < 0.1f)
        return CoplanarTriTri(tri1plane, tri1[0], tri1[1], tri1[2], tri2[0], tri2[1], tri2[2]);

    const float du0du1 = du0 * du1;
    const float du0du2 = du0 * du2;

    if (du0du1 > 0 && du0du2 > 0)	// same sign on all of them + not equal 0 ?
        return 0;

    // Compute plane of triangle (U0,U1,U2)
    float dv0 = dCalcPointPlaneDistance(tri1[0], tri2plane);
    float dv1 = dCalcPointPlaneDistance(tri1[1], tri2plane);
    float dv2 = dCalcPointPlaneDistance(tri1[2], tri2plane);

    if (fabs(dv0) < dEpsilon) dv0 = 0.0;
    if (fabs(dv1) < dEpsilon) dv1 = 0.0;
    if (fabs(dv2) < dEpsilon) dv2 = 0.0;

    const float dv0dv1 = dv0 * dv1;
    const float dv0dv2 = dv0 * dv2;

    if (dv0dv1 > 0 && dv0dv2 > 0)	// same sign on all of them + not equal 0 ?
    {
        return 0;
    }

    // Compute direction of intersection line
    dVector3 D;
    dCalcVectorCross3r4(D, tri1plane, tri2plane);

    // Compute and index to the largest component of D
    float max = fabsf(D[0]);
    short index = 0;
    float bb = fabsf(D[1]);
    float cc = fabsf(D[2]);
    if (bb > max)
    {
        max = bb;
        index = 1;
    }
    if (cc > max)
    {
        max = cc;
        index = 2;
    }

    if (max < 1e-4f)
        return CoplanarTriTri(tri1plane, tri1[0], tri1[1], tri1[2], tri2[0], tri2[1], tri2[2]);

    // This is the simplified projection onto L
    const float vp0 = tri1[0][index];
    const float vp1 = tri1[1][index];
    const float vp2 = tri1[2][index];

    const float up0 = tri2[0][index];
    const float up1 = tri2[1][index];
    const float up2 = tri2[2][index];

    float isect1[2], isect2[2];

    // Compute interval for triangle 1
    if (compute_intervals_isectline(tri1[0], tri1[1], tri1[2], vp0, vp1, vp2, dv0, dv1, dv2,
        dv0dv1, dv0dv2, &isect1[0], &isect1[1], isectpointA1, isectpointA2))
    {
        return CoplanarTriTri(tri1plane, tri1[0], tri1[1], tri1[2], tri2[0], tri2[1], tri2[2]);
    }

    // Compute interval for triangle 2
    compute_intervals_isectline(tri2[0], tri2[1], tri2[2], up0, up1, up2, du0, du1, du2,
        du0du1, du0du2, &isect2[0], &isect2[1], isectpointB1, isectpointB2);

    SORT2(isect1[0], isect1[1], smallest1);
    SORT2(isect2[0], isect2[1], smallest2);

    if (isect1[1] < isect2[0] || isect2[1] < isect1[0])
        return 0;

    /* at this point, we know that the triangles intersect */

    if (isect2[0] < isect1[0])
    {
        if (smallest1 == 0)
        {
            dCopyVector3r4(isectpt1, isectpointA1);
        }
        else
        {
            dCopyVector3r4(isectpt1, isectpointA2);
        }

        if (isect2[1] < isect1[1])
        {
            if (smallest2 == 0)
            {
                dCopyVector3r4(isectpt2, isectpointB2);
            }
            else
            {
                dCopyVector3r4(isectpt2, isectpointB1);
            }
        }
        else
        {
            if (smallest1 == 0)
            {
                dCopyVector3r4(isectpt2, isectpointA2);
            }
            else
            {
                dCopyVector3r4(isectpt2, isectpointA1);
            }
        }
    }
    else
    {
        if (smallest2 == 0)
        {
            dCopyVector3r4(isectpt1, isectpointB1);
        }
        else
        {
            dCopyVector3r4(isectpt1, isectpointB2);
        }

        if (isect2[1] > isect1[1])
        {
            if (smallest1 == 0)
            {
                dCopyVector3r4(isectpt2, isectpointA2);
            }
            else
            {
                dCopyVector3r4(isectpt2, isectpointA1);
            }
        }
        else
        {
            if (smallest2 == 0)
            {
                dCopyVector3r4(isectpt2, isectpointB2);
            }
            else
            {
                dCopyVector3r4(isectpt2, isectpointB1);
            }
        }
    }
    return 1;

}
