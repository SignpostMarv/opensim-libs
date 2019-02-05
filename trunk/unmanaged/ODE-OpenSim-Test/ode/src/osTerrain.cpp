// dOSTerrain Collider
// Leal Duarte.
// adapted from HeighField:
//  Paul Cheyrou-Lagreze aka Tuan Kuranes 2006 Speed enhancements http://www.pop-3d.com
//  Martijn Buijs 2006 http://home.planet.nl/~buijs512/
// Based on Terrain & Cone contrib by:
//  Benoit CHAPEROT 2003-2004 http://www.jstarlab.com
//  Some code inspired by Magic Software


// square 1m side x y grid


#include <ode/common.h>
#include <ode/collision.h>
#include <ode/matrix.h>
#include <ode/rotation.h>
#include <ode/odemath.h>
#include "config.h"
#include "collision_kernel.h"
#include "collision_std.h"
#include "collision_util.h"
#include "osTerrain.h"
#include "ode/timer.h"

#include "collision_trimesh_colliders.h"

#define dMIN(A,B)  ((A)>(B) ? (B) : (A))
#define dMAX(A,B)  ((A)>(B) ? (A) : (B))

ODE_PURE_INLINE dReal dV2Dot(const dReal *a, const dReal *b)
{
    return a[0] * b[0] + a[1] * b[1];
}

ODE_PURE_INLINE dReal dV2DistSQ(const dReal *a, const dReal *b)
{
    dReal c = a[0] - b[0];
    c *= c;
    dReal d = a[1] - b[1];
    c = c + d*d;
    return  c;
}


ODE_PURE_INLINE dReal dV3CrossTerrainInvLen(dReal *norm, dReal dx, dReal dy)
{
    dReal dinvlength = REAL(1.0) / dSqrt(dx * dx + dy * dy + REAL(1.0));

    norm[0] = dx * dinvlength;
    norm[1] = dy * dinvlength;
    norm[2] = dinvlength;
    return dinvlength;
}

ODE_PURE_INLINE dReal dV3CrossTerrainLen(dReal *norm, dReal dx, dReal dy)
{
    dReal length = dSqrt(dx * dx + dy * dy + REAL(1.0));
    dReal dinvlength = REAL(1.0) / length;

    norm[0] = dx * dinvlength;
    norm[1] = dy * dinvlength;
    norm[2] = dinvlength;
    return length;
}
// only does cross  -1,0,dx with 0,-1,dy ( D->C x C->A )
// or               1,0,-dx  with 0,1,-dy ( A->B x B->D)
ODE_PURE_INLINE void dV3CrossTerrain(dReal *norm, dReal dx, dReal dy)
{
    dReal length = dSqrt(dx * dx + dy * dy + REAL(1.0));
    dReal dinvlength = REAL(1.0) / length;

    norm[0] = dx * dinvlength;
    norm[1] = dy * dinvlength;
    norm[2] = dinvlength;
}

//////// dxOSTerrainData /////////////////////////////////////////////////////////////

// dxOSTerrainData constructor
dxOSTerrainData::dxOSTerrainData() :	m_fWidth( 0 ),
											m_fDepth( 0 ),
											
											m_fHalfWidth( 0 ),
											m_fHalfDepth( 0 ),

											m_fMinHeight( 0 ),
											m_fMaxHeight( 0 ),
											m_fThickness( 0 ),
											
											m_nWidthSamples( 0 ),
											m_nDepthSamples( 0 ),
											m_bCopyHeightData( 0 ),
											
											m_pHeightData( NULL )
{
	memset( m_contacts, 0, sizeof( m_contacts ) );
}

// build OSTerrain data
void dxOSTerrainData::SetData( int nWidthSamples, int nDepthSamples,
                                float fSampleSize,
                                float fThickness, int bWrapMode)
{
    dIASSERT( nWidthSamples > 0 );
    dIASSERT( nDepthSamples > 0 );
    dIASSERT( fSampleSize > REAL( 0.0 ) );

    // x,z bounds
    m_fWidth = nWidthSamples - REAL(1.0);
    m_fDepth = nDepthSamples - REAL(1.0);

    // cache half x,z bounds
    m_fHalfWidth = m_fWidth * REAL( 0.5 );
    m_fHalfDepth = m_fDepth * REAL( 0.5 );

    // infinite min height bounds
    m_fThickness = fThickness;

    // number of vertices per side
    m_nWidthSamples = nWidthSamples;
    m_nDepthSamples = nDepthSamples;

}


// recomputes heights bounds
void dxOSTerrainData::ComputeHeightBounds()
{
    int i;
    dReal h;

    m_fMinHeight = dInfinity;
    m_fMaxHeight = -dInfinity;
    int tot = m_nWidthSamples*m_nDepthSamples;
    for (i=0; i < tot; i++)
    {
        h = m_pHeightData[i];
        if (h < m_fMinHeight)	m_fMinHeight = h;
        if (h > m_fMaxHeight)	m_fMaxHeight = h;
    }
}

// returns whether point is over terrain Cell triangle?
bool dxOSTerrainData::IsOnOSTerrain2(const OSTerrainVertex * const CellCorner,
    const dReal *const pos, const bool isFirst) const
{
    // WARNING!!!
    // This function must be written in the way to make sure that every point on
    // XZ plane falls in one and only one triangle. Keep that in mind if you 
    // intend to change the code.
    // Also remember about computational errors and possible mismatches in 
    // values if they are calculated differently in different places in the code.
    // Currently both the implementation has been optimized and effects of 
    // computational errors have been eliminated.

    dReal dX;
    dReal dY;

    if (isFirst)
    {
        // point C
        dX = pos[0] - CellCorner->vertex[0];
        if (dX < dReal(0.0) || dX > dReal(1.0))
            return false;

        dY = CellCorner->vertex[1] - pos[1];
        if (dY < dReal(0.0) || dY > dReal(1.0))
            return false;
        dY = dReal(1.0) + dY;
        return (dY > dX);
    }
    else
    {
        // point B
        dX = CellCorner->vertex[0] - pos[0];
        if (dX < dReal(0.0) || dX > dReal(1.0))
            return false;

        dY = pos[1] - CellCorner->vertex[1];
        if (dY < dReal(0.0) || dY > dReal(1.0))
            return false;

        dX = dReal(1.0) - dX;
        return (dY < dX);
    }
}


// returns height at given sample coordinates
dReal dxOSTerrainData::GetHeight( int x, int y )
{
        // Finite
    if ( x < 0 )
        x = 0;
    else if ( x >= m_nWidthSamples)
        x = m_nWidthSamples - 1;

    if ( y < 0 )
        y = 0;
    else if ( y >= m_nDepthSamples)
        y = m_nDepthSamples - 1;

    return m_pHeightData[x+(y * m_nWidthSamples)];
}


inline int dxOSTerrainData::GetTriIndex(int x, int y, bool second)
{
    int i = y * m_nWidthSamples + x;
    i *= 2;
    if (second)
        i++;
    return i;
}

// returns height at given sample coordinates
inline dReal dxOSTerrainData::GetHeightSafe(int x, int y)
{
    return m_pHeightData[x + (y * m_nWidthSamples)];
}

// returns height at given coordinates
dReal dxOSTerrainData::GetHeight(dReal x, dReal y)
{
    dReal dnX = dFloor(x);
    dReal dnY = dFloor(y);

    dReal dx = x - dnX;
    dReal dy = y - dnY;

    int nX = int(dnX);
    int nY = int(dnY);

    nY *= m_nWidthSamples;
    nY += nX; // index of 0,0

    dReal z, z0, z1;

    z0 = m_pHeightData[nY];
    z = z0;

    if ( dy > dx )
    {
        nY += m_nWidthSamples;                  // move to x,1
        z1 = m_pHeightData[nY];                 // 0,1
        z += (z1 - z0) * dy;                    // 0,1 - 0,0
        z += (m_pHeightData[nY+1] - z1) * dx;  // 1,1 - 0,1
    }
    else
    {
        nY++;                                   // move to 1,y
        z1 = m_pHeightData[nY];                 // 1,0
        z += (z1 - z0) *dx;                     // 1,0 - 0,0
        z += (m_pHeightData[nY + m_nWidthSamples] - z1) * dy; // 1,1 - 1,0
    }

    return z;
}

void dxOSTerrainData::GetNormal( dReal x, dReal y ,dReal *normal)
{
    dReal dnX = dFloor( x );
	dReal dnY = dFloor( y );

    dReal dx = x - dnX;
    dReal dy = y - dnY;

    int nX = int( dnX );
    int nY = int( dnY );

    nY *= m_nWidthSamples;
    nY += nX; // index of 0,0

    dReal z, z0, z1;
    dReal nx,ny;
    
    z0 = m_pHeightData[nY]; // 0,0

    if (dy > dx)
    {
        nY += m_nWidthSamples;
        z = m_pHeightData[nY]; // 0,1
        z1 = m_pHeightData[nY + 1]; // 1,1
        nx = z - z1;
        ny = z0 - z;
    }
    else
    {
        nY++;
        z = m_pHeightData[nY]; // 1,0
        z1 = m_pHeightData[nY + 1]; // 1,1
        nx = z0 - z;
        ny = z - z1;
    }

    dV3CrossTerrain(normal, nx, ny);
}


// dxOSTerrainData destructor
dxOSTerrainData::~dxOSTerrainData()
{
    if ( m_bCopyHeightData )
    {
        dIASSERT( m_pHeightData );
        delete [] m_pHeightData;
    }
}


//////// dxOSTerrain /////////////////////////////////////////////////////////////////


// dxOSTerrain constructor
dxOSTerrain::dxOSTerrain( dSpaceID space,
                             dOSTerrainDataID data,
                             int bPlaceable )			:
    dxGeom( space, bPlaceable ),
    tempPlaneBuffer(0),
	tempPlaneInstances(0),
    tempPlaneBufferSize(0),
    tempTriangleBuffer(0),
    tempTriangleBufferSize(0),
    tempHeightBuffer(0),
	tempHeightInstances(0),
    tempHeightBufferSizeX(0),
    tempHeightBufferSizeY(0)
{
    type = dOSTerrainClass;
    this->m_p_data = data;
}


// compute axis aligned bounding box
void dxOSTerrain::computeAABB()
{
    const dxOSTerrainData *d = m_p_data;

    // Finite
    aabb[0] = final_posr->pos[0] - d->m_fHalfWidth;
    aabb[1] = final_posr->pos[0] + d->m_fHalfWidth;

    aabb[2] = final_posr->pos[1] - d->m_fHalfDepth;
    aabb[3] = final_posr->pos[1] + d->m_fHalfDepth;

//    aabb[4] = final_posr->pos[2] + (d->m_fMinHeight - d->m_fThickness);
//    aabb[5] = final_posr->pos[2] + d->m_fMaxHeight;

    aabb[4] = d->m_fMinHeight - d->m_fThickness;
    aabb[5] = d->m_fMaxHeight;
}


// dxOSTerrain destructor
dxOSTerrain::~dxOSTerrain()
{
	resetTriangleBuffer();
	resetPlaneBuffer();
	resetHeightBuffer();
}

void dxOSTerrain::allocateTriangleBuffer(size_t numTri)
{
	size_t alignedNumTri = AlignBufferSize(numTri, TEMP_TRIANGLE_BUFFER_ELEMENT_COUNT_ALIGNMENT);
	tempTriangleBufferSize = alignedNumTri;
	tempTriangleBuffer = new OSTerrainTriangle[alignedNumTri];
}

void dxOSTerrain::resetTriangleBuffer()
{
	delete[] tempTriangleBuffer;
}

void dxOSTerrain::allocatePlaneBuffer(size_t numTri)
{
	size_t alignedNumTri = AlignBufferSize(numTri, TEMP_PLANE_BUFFER_ELEMENT_COUNT_ALIGNMENT);
	tempPlaneBufferSize = alignedNumTri;
	tempPlaneBuffer = new OSTerrainPlane *[alignedNumTri];
	tempPlaneInstances = new OSTerrainPlane[alignedNumTri];

	OSTerrainPlane *ptrPlaneMatrix = tempPlaneInstances;
	for (size_t indexTri = 0; indexTri != alignedNumTri; indexTri++)
	{
		tempPlaneBuffer[indexTri] = ptrPlaneMatrix;
		ptrPlaneMatrix += 1;
	}
}

void dxOSTerrain::resetPlaneBuffer()
{
	delete[] tempPlaneInstances;
    delete[] tempPlaneBuffer;
}

void dxOSTerrain::allocateHeightBuffer(size_t numX, size_t numY)
{
	size_t alignedNumX = AlignBufferSize(numX, TEMP_HEIGHT_BUFFER_ELEMENT_COUNT_ALIGNMENT_X);
	size_t alignedNumY = AlignBufferSize(numY, TEMP_HEIGHT_BUFFER_ELEMENT_COUNT_ALIGNMENT_Y);
	tempHeightBufferSizeX = alignedNumX;
	tempHeightBufferSizeY = alignedNumY;
	tempHeightBuffer = new OSTerrainVertex *[alignedNumY];
	size_t numCells = alignedNumX * alignedNumY;
	tempHeightInstances = new OSTerrainVertex [numCells];
	
	OSTerrainVertex *ptrHeightMatrix = tempHeightInstances;
	for (size_t indexY = 0; indexY != alignedNumY; indexY++)
	{
		tempHeightBuffer[indexY] = ptrHeightMatrix;
		ptrHeightMatrix += alignedNumX;
	}
}

void dxOSTerrain::resetHeightBuffer()
{
	delete[] tempHeightInstances;
    delete[] tempHeightBuffer;
}

//////// OSTerrain data interface ////////////////////////////////////////////////////


dOSTerrainDataID dGeomOSTerrainDataCreate()
{
    return new dxOSTerrainData();
}


ODE_API void dGeomOSTerrainDataBuild( dOSTerrainDataID d,
				const float* pHeightData, int bCopyHeightData,
				dReal samplesize, int widthSamples, int depthSamples,
				dReal thickness, int bWrap )
{
    dUASSERT( d, "Argument not OSTerrain data" );
    dIASSERT( pHeightData );
    dIASSERT(samplesize>0);
    dIASSERT( widthSamples >= 2 );	// Ensure we're making something with at least one cell.
    dIASSERT( depthSamples >= 2 );

    // set info
    d->SetData( widthSamples, depthSamples, samplesize, thickness, bWrap );
    d->m_bCopyHeightData = bCopyHeightData;

    if ( d->m_bCopyHeightData == 0 )
    {
        // Data is referenced only.
        d->m_pHeightData = pHeightData;
    }
    else
    {
        // We own the height data, allocate storage
        d->m_pHeightData = new float[ d->m_nWidthSamples * d->m_nDepthSamples ];
        dIASSERT( d->m_pHeightData );

        // Copy data.
        memcpy( (void*)d->m_pHeightData, pHeightData,
            sizeof( float ) * d->m_nWidthSamples * d->m_nDepthSamples );
    }

    // Find height bounds
    d->ComputeHeightBounds();
}


void dGeomOSTerrainDataSetBounds( dOSTerrainDataID d, dReal minHeight, dReal maxHeight )
{
    dUASSERT(d, "Argument not OSTerrain data");
    d->m_fMinHeight = minHeight;
    d->m_fMaxHeight = maxHeight;
}


void dGeomOSTerrainDataDestroy( dOSTerrainDataID d )
{
    dUASSERT(d, "argument not OSTerrain data");
    delete d;
}


//////// OSTerrain geom interface ////////////////////////////////////////////////////


dGeomID dCreateOSTerrain( dSpaceID space, dOSTerrainDataID data, int bPlaceable )
{
    return new dxOSTerrain( space, data, bPlaceable );
}


void dGeomOSTerrainSetData( dGeomID g, dOSTerrainDataID d )
{
    dxOSTerrain* geom = (dxOSTerrain*) g;
    geom->data = d;
}


dOSTerrainDataID dGeomOSTerrainGetData( dGeomID g )
{
    dxOSTerrain* geom = (dxOSTerrain*) g;
    return geom->m_p_data;
}

//////// dxOSTerrain /////////////////////////////////////////////////////////////////


// Typedef for generic 'get point depth' function
typedef dReal dGetDepthFn( dGeomID g, dReal x, dReal y, dReal z );


#define DMESS(A)	\
    dMessage(0,"Contact Plane (%d %d %d) %.5e %.5e (%.5e %.5e %.5e)(%.5e %.5e %.5e)).",	\
    x,z,(A),	\
    pContact->depth,	\
    dGeomSphereGetRadius(o2),		\
    pContact->pos[0],	\
    pContact->pos[1],	\
    pContact->pos[2],	\
    pContact->normal[0],	\
    pContact->normal[1],	\
    pContact->normal[2]);

static inline bool DescendingTriangleSort(const OSTerrainTriangle * const A, const OSTerrainTriangle * const B)
{
    return ((A->maxAAAB - B->maxAAAB) > dEpsilon);
}

void SortPlaneContacts(dContactGeom* PlaneContact,int numPlaneContacts)
    {
        dContactGeom t;
        int i,j;

        for(i = 0; i < numPlaneContacts - 1; i++)
        {   
            for (j = i + 1 ; j< numPlaneContacts; j++)
            {
                if(PlaneContact[j].depth > PlaneContact[i].depth)
                {
                    t = PlaneContact[i];
                    PlaneContact[i] = PlaneContact[j];
                    PlaneContact[j] = t;
                }
            }
        }
    }

int dxOSTerrain::dCollideOSTerrainZone( const int minX, const int maxX, const int minY, const int maxY, 
                                           dxGeom* o2, const int numMaxContactsPossible,
                                           int flags, dContactGeom* contact, 
                                           int skip )
{
	dContactGeom *pContact = 0;
/*   
    double tstart = getClockTicksMs();
    double t1;
    double t2;
    double t3;
    double t4;
*/
    dxPlane myplane(0,0,0,0,0);
    dxPlane* sliding_plane = &myplane;
    dReal triplane[4];
    int i;

    int numTerrainContacts = 0;

    const unsigned int numX = (maxX - minX) + 1;
    const unsigned int numY = (maxY - minY) + 1;

    int  x, y;

    unsigned int x_local, y_local;

    dReal offsetX;
    dReal offsetY;
    
    if (tempHeightBufferSizeX < numX || tempHeightBufferSizeY < numY)
    {
        resetHeightBuffer();
        allocateHeightBuffer(numX, numY);
    }

    offsetX = final_posr->pos[0] - m_p_data->m_fHalfWidth;
    offsetY = final_posr->pos[1] - m_p_data->m_fHalfDepth;

    dReal Xpos;
    dReal Ypos = minY + offsetY;

    OSTerrainVertex *OSTerrainRow;

    dReal maxZ = - dInfinity;
    dReal minZ = dInfinity;

    for ( y = minY, y_local = 0; y_local < numY; y++, y_local++)
    {
        OSTerrainRow = tempHeightBuffer[y_local];

        Xpos = minX + offsetX;

        for ( x = minX ; x < maxX + 1; x++)
        {
            const dReal h = m_p_data->GetHeightSafe(x, y);
            OSTerrainRow->vertex[0] = Xpos;
            OSTerrainRow->vertex[1] = Ypos;
            OSTerrainRow->vertex[2] = h;
            OSTerrainRow++;

            if(h > maxZ)
                maxZ = h;
            if(h < minZ)
                minZ = h;

            Xpos += REAL(1.0);
        }
        Ypos += REAL(1.0);
    }

    const dReal minO2Height = o2->aabb[4];
    const dReal maxO2Height = o2->aabb[5];

    if (minO2Height - maxZ > -dEpsilon )
    {
        //totally above heightfield
        return 0;
    }

    if (minZ - maxO2Height > -dEpsilon)
    {
        // totally under heightfield
        // meshs aren't all centered around position
        dReal Xpos = (o2->aabb[1] + o2->aabb[0]) * REAL(0.5);
        dReal Ypos = (o2->aabb[3] + o2->aabb[2]) * REAL(0.5);
        const dReal h = m_p_data->GetHeight(Xpos - offsetX, Ypos - offsetY);

        pContact = CONTACT(contact,0);

        pContact->pos[0] = Xpos;
        pContact->pos[1] = Ypos;
        pContact->pos[2] = minO2Height;
        pContact->normal[0] = 0;
        pContact->normal[1] = 0;
        pContact->normal[2] = -1;

        pContact->depth = minZ - minO2Height;

        pContact->side1 = -1;
        pContact->side2 = -1;

        return 1;
    }

    // get All Planes that could collide against.
    dColliderFn *geomRayNCollider=0;
    dColliderFn *geomNPlaneCollider=0;
    dGetDepthFn *geomNDepthGetter=0;

    // int max_collisionContact = numMaxContactsPossible; -- not used
    switch (o2->type)
    {
    case dRayClass:
        geomRayNCollider		= NULL;
        geomNPlaneCollider	    = dCollideRayPlane;
        geomNDepthGetter		= NULL;
        //max_collisionContact    = 1;
        break;

    case dSphereClass:
        geomRayNCollider		= dCollideRaySphere;
        geomNPlaneCollider  	= dCollideSpherePlane;
        geomNDepthGetter		= dGeomSpherePointDepth;
        //max_collisionContact    = 3;
        break;

    case dBoxClass:
        geomRayNCollider		= dCollideRayBox;
        geomNPlaneCollider	    = dCollideBoxPlane;
        geomNDepthGetter		= dGeomBoxPointDepth;
        //max_collisionContact    = 8;
        break;

    case dCapsuleClass:
        geomRayNCollider		= dCollideRayCapsule;
        geomNPlaneCollider  	= dCollideCapsulePlane;
        geomNDepthGetter		= dGeomCapsulePointDepth;
        // max_collisionContact    = 3;
        break;
/*
    case dCylinderClass:
        geomRayNCollider		= dCollideRayCylinder;
        geomNPlaneCollider	    = dCollideCylinderPlane;
        geomNDepthGetter		= NULL;// TODO: dGeomCCylinderPointDepth
        //max_collisionContact    = 3;
        break;

    case dConvexClass:
        geomRayNCollider		= dCollideRayConvex;
        geomNPlaneCollider  	= dCollideConvexPlane;
        geomNDepthGetter		= NULL;// TODO: dGeomConvexPointDepth;
        //max_collisionContact    = 3;
        break;
*/
    case dTriMeshClass:
        geomRayNCollider		= dCollideRayTrimesh;
        geomNPlaneCollider	    = dCollideTrimeshPlane;
        geomNDepthGetter		= NULL;// TODO: dGeomTrimeshPointDepth;
        //max_collisionContact    = 3;
        break;

    default:
        dIASSERT(0);	// Shouldn't ever get here.
        break;

    }

   if (maxZ - minZ < OSTERRAINPLAINEPSILON) // flat terrain
    {
        triplane[0] = 0;
        triplane[1] = 0;
        triplane[2] = 1;
        triplane[3] =  minZ;

        dCopyVector4((sliding_plane)->p, triplane);
        dGeomMoved(sliding_plane);

        // find collision and compute contact points
        numTerrainContacts = geomNPlaneCollider (o2, sliding_plane, flags, contact, skip);
        dIASSERT(numTerrainContacts <= numMaxContactsPossible);

        for (i = 0; i < numTerrainContacts; i++)
        {
            pContact = CONTACT(contact, i*skip);
            dCopyNegatedVector3r4(pContact->normal, triplane);
        }
        return numTerrainContacts;
    }
 

    dContactGeom *PlaneContact = m_p_data->m_contacts;

    const unsigned int numTriMax = (maxX - minX) * (maxY - minY) * 2;
    if (tempTriangleBufferSize < numTriMax)
    {
        resetTriangleBuffer();
        allocateTriangleBuffer(numTriMax);
    }
    
    // Sorting triangle/plane  resulting from heightfield zone
    // Perhaps that would be necessary in case of too much limited
    // maximum contact point...
    // or in complex mesh case (trimesh and convex)
    // need some test or insights on this before enabling this.
    const bool isContactNumPointsLimited = 
        true;

    bool needFurtherPasses = (o2->type == dTriMeshClass);
    //compute Ratio between Triangle size and O2 aabb size
    // no FurtherPasses are needed in ray class
    if (o2->type != dRayClass  && needFurtherPasses == false)
    {
        const dReal xratio = (o2->aabb[1] - o2->aabb[0]);
        if (xratio > REAL(1.5))
            needFurtherPasses = true;
        else
        {
            const dReal zratio = (o2->aabb[3] - o2->aabb[2]);
            if (zratio > REAL(1.5))
                needFurtherPasses = true;
        }
    }

    unsigned int numTri = 0;
    OSTerrainVertex *A, *B, *C, *D;
    /*    (z is up)
         y
         .
         .
         .
         C--------D   
         |       /|
         |      / |
         |     /  |
         |    /   |
         |   /    |
         |  /     |
         | /      |
         |/       |
         A--------B-...x
    */   
  
    // keep only triangle that does intersect geom
    
    const unsigned int maxX_local = maxX - minX;
    const unsigned int maxY_local = maxY - minY;

    bool isACollide;
    bool isBCollide;
    bool isCCollide;
    bool isDCollide;

    dReal dz1;
    dReal dz2;

    dReal AHeight;
    dReal BHeight;
    dReal CHeight;
    dReal DHeight;

    dReal *plane;

    OSTerrainTriangle *CurrTri;

    for ( y_local = 0; y_local < maxY_local; y_local++)
    {
        OSTerrainVertex *OSTerrainRow      = tempHeightBuffer[y_local];
        OSTerrainVertex *OSTerrainNextRow  = tempHeightBuffer[y_local + 1];

        // First A
        B = &OSTerrainRow[0];
        BHeight = B->vertex[2];
        isBCollide = BHeight > minO2Height;
        B->state = !(isBCollide);

        // First C
        D = &OSTerrainNextRow[0];
        DHeight = D->vertex[2];
        isDCollide = DHeight > minO2Height;
        D->state = !(isDCollide);

        for ( x_local = 0; x_local < maxX_local; x_local++)
        {
            A = B;
            AHeight = BHeight;
            isACollide = isBCollide;

            C = D;
            CHeight = DHeight;
            isCCollide = isDCollide;

            B = &OSTerrainRow    [x_local + 1];
            D = &OSTerrainNextRow[x_local + 1];
 
            BHeight = B->vertex[2];
            DHeight = D->vertex[2];

            const bool isBCollide = BHeight > minO2Height;
            const bool isDCollide = DHeight > minO2Height;

            B->state = !(isBCollide);
            D->state = !(isDCollide);

            if (isACollide || isCCollide || isDCollide)
            {
                CurrTri = &tempTriangleBuffer[numTri++];
                CurrTri->state = false;

                // changing point order here implies to change it in isOnOSTerrain
                CurrTri->vertices[0] = C;
                CurrTri->vertices[1] = A;
                CurrTri->vertices[2] = D;

                if (isContactNumPointsLimited)
                    CurrTri->setMinMax();
                CurrTri->isFirst = true;

                plane =  CurrTri->planeDef;

                dz1 = CHeight - DHeight;
                dz2 = AHeight - CHeight;

                dV3CrossTerrain(plane, dz1, dz2);

                plane[3] = dCalcVectorDot3(CurrTri->planeDef, C->vertex);
            }

            if (isACollide || isBCollide || isDCollide)
            {
                CurrTri = &tempTriangleBuffer[numTri++];

                CurrTri->state = false;
                // changing point order here implies to change it in isOnOSTerrain

                CurrTri->vertices[0] = B;
                CurrTri->vertices[1] = D;
                CurrTri->vertices[2] = A;

                if (isContactNumPointsLimited)
                    CurrTri->setMinMax();

                CurrTri->isFirst = false;

                plane =  CurrTri->planeDef;

                dz1 = AHeight - BHeight;
                dz2 = BHeight - DHeight;

                dV3CrossTerrain(plane, dz1, dz2);

                plane[3] = dCalcVectorDot3(CurrTri->planeDef, B->vertex);
            }
        }
    }

    // at least on triangle should intersect geom
    dIASSERT (numTri != 0);


//    t1 = getClockTicksMs() - tstart;

    unsigned int numPlanes = 0;
    int numPlaneContacts;

    int numMaxContactsPerPlane = dMIN(numMaxContactsPossible - numTerrainContacts, OSTERRAINMAXCONTACTPERCELL);
    int planeTestFlags = (flags & ~NUMC_MASK) | numMaxContactsPerPlane;
    dIASSERT((OSTERRAINMAXCONTACTPERCELL & ~NUMC_MASK) == 0);     

    OSTerrainTriangle* tri_base;
    OSTerrainTriangle* tri_test;
    dReal *itPlane;
    dContactGeom *planeCurrContact;


    for (unsigned int k = 0; k < numTri; k++)
    {
        tri_base = &tempTriangleBuffer[k];

        if (tri_base->state == true)
            continue;// already tested

        tri_base->state = true;
        itPlane = tri_base->planeDef;

        //set plane Geom
        dCopyVector4((sliding_plane)->p, itPlane);
        dGeomMoved(sliding_plane);

        numPlaneContacts = geomNPlaneCollider (o2, sliding_plane, planeTestFlags, PlaneContact, sizeof(dContactGeom));
        if(numPlaneContacts > 0)
        {
            for (i = 0; i < numPlaneContacts; i++)
            {
                planeCurrContact = PlaneContact + i;

                if (m_p_data->IsOnOSTerrain2 (tri_base->vertices[0], 
                    planeCurrContact->pos,
                    tri_base->isFirst))
                {
                    pContact = CONTACT(contact, numTerrainContacts*skip);
                    dCopyVector3r4( pContact->pos, planeCurrContact->pos);
                    dCopyNegatedVector3r4(pContact->normal, itPlane);
                    pContact->depth = planeCurrContact->depth;
                    pContact->side1 = planeCurrContact->side1;
                    pContact->side2 = planeCurrContact->side2;
                    numTerrainContacts++;
                    if ( numTerrainContacts == numMaxContactsPossible )
                        return numTerrainContacts;
                    if (needFurtherPasses)
                    {
                        tri_base->vertices[0]->state = true;
                        tri_base->vertices[1]->state = true;
                        tri_base->vertices[2]->state = true;
                    }
                }
            }
        }

        // check if there are identical planes
        const dReal normx = itPlane[0];
        const dReal normy = itPlane[1];
        const dReal normz = itPlane[2];
        const dReal dist = itPlane[3];

        for (unsigned int m = k + 1; m < numTri; m++)
        {
            tri_test = &tempTriangleBuffer[m];
            if (tri_test->state == true)
                continue;// already tested or added to plane list.

            // check if identical plane
            if (
                dFabs(normx - tri_test->planeDef[0]) > 0.001 ||
                dFabs(normy - tri_test->planeDef[1]) > 0.001 ||
                dFabs(normz - tri_test->planeDef[2]) > 0.001 ||
                dFabs(dist - tri_test->planeDef[3]) > 0.01
                )
                continue;

            // identical plane, process it
            tri_test->state = true;

            if(numPlaneContacts == 0)
                continue;

            for (i = 0; i < numPlaneContacts; i++)
            {
                planeCurrContact = PlaneContact + i;
                const dVector3 &pCPos = planeCurrContact->pos;

                if (m_p_data->IsOnOSTerrain2 (tri_test->vertices[0], 
                    planeCurrContact->pos,
                    tri_test->isFirst))
                {
                    pContact = CONTACT(contact, numTerrainContacts*skip);
                    dCopyVector3r4(pContact->pos, planeCurrContact->pos);
                    dCopyNegatedVector3r4(pContact->normal, itPlane);
                    pContact->depth = planeCurrContact->depth;
                    pContact->side1 = planeCurrContact->side1;
                    pContact->side2 = planeCurrContact->side2;
                    numTerrainContacts++;
                    if ( numTerrainContacts == numMaxContactsPossible )
                        return numTerrainContacts;
                    if (needFurtherPasses)
                    {
                        tri_test->vertices[0]->state = true;
                        tri_test->vertices[1]->state = true;
                        tri_test->vertices[2]->state = true;
                    }
                }
            }
        }
    }

//    t2 = getClockTicksMs() - tstart;

    // pass2: VS triangle vertices
    if (needFurtherPasses)
    {
        dxRay tempRay(0, 1); 
        dReal depth;
        bool vertexCollided;

        // Only one contact is necessary for ray test
        int rayTestFlags = (flags & ~NUMC_MASK) | 1;
        dIASSERT((1 & ~NUMC_MASK) == 0);
        //
        // Find Contact Penetration Depth of each vertices
        //
        for (unsigned int k = 0; k < numTri; k++)
        {
            const OSTerrainTriangle * const itTriangle = &tempTriangleBuffer[k];

            for (size_t i = 0; i < 3; i++)
            {
                OSTerrainVertex *vertex = itTriangle->vertices[i];
                if (vertex->state == true)
                    continue;// vertice did already collide.

                vertex->state = true;

                vertexCollided = false;
                const dVector3 &triVertex = vertex->vertex;
                if ( geomNDepthGetter )
                {
                    depth = geomNDepthGetter( o2,
                        triVertex[0], triVertex[1], triVertex[2] );
                    if (depth > dEpsilon)
                        vertexCollided = true;
                }
                else
                {
                    // We don't have a GetDepth function, so do a ray cast instead.
                    // NOTE: This isn't ideal, and a GetDepth function should be
                    // written for all geom classes.
                    tempRay.length = (triVertex[2] - minO2Height) * REAL(100.0);
                    if(tempRay.length < 0)
                        continue;

                    dGeomRaySet( &tempRay, triVertex[0], triVertex[1], triVertex[2],
                        -itTriangle->planeDef[0], -itTriangle->planeDef[1], -itTriangle->planeDef[2] );
                    dGeomRaySetClosestHit(&tempRay,1);

                    if ( geomRayNCollider( &tempRay, o2, rayTestFlags, PlaneContact, sizeof( dContactGeom ) ) )
                    {
                        depth = PlaneContact[0].depth;
                        vertexCollided = true;
                    }
                }
                if (vertexCollided)
                {
                    pContact = CONTACT(contact, numTerrainContacts*skip);
                    //create contact using vertices
                    dCopyVector3r4(pContact->pos, triVertex);
                    //create contact using Plane Normal
                    dCopyNegatedVector3r4(pContact->normal, itTriangle->planeDef);
                    pContact->depth = depth;
                    pContact->side1 = -1;
                    pContact->side2 = -1;

                    numTerrainContacts++;
                    if ( numTerrainContacts == numMaxContactsPossible ) 
                        return numTerrainContacts;
                }
            }
        }
    }
//    t3 = getClockTicksMs() - tstart;
    return numTerrainContacts;
}

void inline dOSTerrainAddSphereContact(const dContactGeom* contact, int skip, 
            dVector3& pos, dReal depth, int& numTerrainContacts)
{
    dContactGeom* pContact;

    if(depth < dEpsilon )
        return;

    while(true)
    {
        pContact = CONTACT(contact, 0);
        if(depth > pContact->depth)
            break;

        pContact = CONTACT(contact, skip);
        if(pos[0] > pContact->pos[0])
            break;

        pContact = CONTACT(contact, 2 * skip);
        if(pos[0] < pContact->pos[0])
            break;

        pContact = CONTACT(contact, 3 * skip);
        if(pos[1] > pContact->pos[1])
            break;

        pContact = CONTACT(contact, 4 * skip);
        if(pos[1] < pContact->pos[1])
            break;
        return;
    }

    dCopyVector3r4(pContact->pos, pos);
    pContact->depth = depth;
    numTerrainContacts++;
}


int dxOSTerrain::dCollideOSTerrainSphere(const int minX, const int maxX, const int minY, const int maxY,
    dxGeom* o2, const int numMaxContactsPossible,
    int flags, dContactGeom* contact,
    int skip)
{
    dIASSERT(o2->type == dSphereClass);

/*
    double tstart = getClockTicksMs();
    double t1;
    double t2;
    double t3;
    double t4;
*/
    int numTerrainContacts = 0;

    dReal radius;
    dReal radiussq;

    dReal depth;

    dVector3 center;

    dReal offsetX = final_posr->pos[0] - m_p_data->m_fHalfWidth;
    dReal offsetY = final_posr->pos[1] - m_p_data->m_fHalfDepth;

    dxSphere *sphere = (dxSphere*)o2;
    radius = sphere->radius;

    dCopyVector3r4(center, sphere->final_posr->pos);

    const dReal minO2Height = center[2] - radius;

    dReal h = m_p_data->GetHeight(center[0] - offsetX, center[1] - offsetY);

    dContactGeom *pContact = 0;

    dReal tf = m_p_data->m_fMaxHeight - m_p_data->m_fMinHeight;

    if (h > center[2] || tf < REAL(.05))
    {
        depth = h - minO2Height;
        if(depth < dEpsilon)
            return 0;

        pContact = CONTACT(contact, 0);
    
        pContact->depth = depth;
        pContact->pos[0] = center[0];
        pContact->pos[1] = center[1];
        pContact->pos[2] = minO2Height;
        pContact->normal[0] = 0;
        pContact->normal[1] = 0;
        pContact->normal[2] = -1;
        pContact->side1 = -1;
        pContact->side2 = -1;

        return 1;
    }

    const dContactGeom *ContactBuffer = m_p_data->m_contacts;
 
    pContact = CONTACT(ContactBuffer, 0);
    pContact->depth = -1e30;

    pContact = CONTACT(ContactBuffer, skip);
    pContact->pos[0] = -1e30;
    pContact->depth = 0;

    pContact = CONTACT(ContactBuffer, 2* skip);
    pContact->pos[0] = 1e30;
    pContact->depth = 0;

    pContact = CONTACT(ContactBuffer, 3 * skip);
    pContact->pos[1] = -1e30;
    pContact->depth = 0;

    pContact = CONTACT(ContactBuffer, 4 * skip);
    pContact->pos[1] = 1e30;
    pContact->depth = 0;

    dVector3 dist;
    dVector3 tdist;
    dVector3 pdist;
    dVector3 normA;
    dVector3 normB;


    /*    (z is up)
    y
    .
    .
    .
    C--------D
    |       /|
    |      / |
    |     /  |
    |    /   |
    |   /    |
    |  /     |
    | /      |
    |/       |
    A--------B-...x
    */

    bool *VtopCollideA = new bool[maxX - minX + 3];
    bool *VtopCollideB = new bool[maxX - minX + 3];

    bool *lastVtopCollide;
    bool *curVtopCollide;
    int VtopColliedPtr;

    bool isACollide;
    bool isBCollide;
    bool isCCollide;
    bool isDCollide;
    bool isAorDCollide;

    bool topColide = false;
    bool botColide = false;
    bool lastHtopColide;
    bool lastHbotColide;

    int i;
    int  x, y;

    dReal dz1;
    dReal dz2;

    dReal AHeight;
    dReal BHeight;
    dReal CHeight;
    dReal DHeight;

    dReal px, py, k;

    radiussq = radius * radius;

    dReal tdistStartX = center[0] - minX - offsetX;
    tdist[1] = center[1] - minY - offsetY;

    i=0;
    for (x = minX; x <= maxX; x++)
        VtopCollideA[i++] = true;
    VtopCollideB[0] = true;

    curVtopCollide = VtopCollideA;
    for (y = minY; y < maxY; y++, tdist[1] -= REAL(1.0))
    {
        if(curVtopCollide == VtopCollideA)
        {
            lastVtopCollide = VtopCollideA;
            curVtopCollide = VtopCollideB;
        }
        else
        {
            lastVtopCollide = VtopCollideB;
            curVtopCollide = VtopCollideA;
        }

        tdist[0] = tdistStartX;

        // First A
        BHeight = m_p_data->GetHeightSafe(minX, y);
        isBCollide = BHeight > minO2Height;

        // First C
        DHeight = m_p_data->GetHeightSafe(minX, y + 1);
        isDCollide = DHeight > minO2Height;

        topColide = true;
        botColide = true;

        for (x = minX + 1, VtopColliedPtr = 1; x <= maxX;
            x++, VtopColliedPtr++, tdist[0] -= REAL(1.0))
        {
            AHeight = BHeight;
            isACollide = isBCollide;

            CHeight = DHeight;
            isCCollide = isDCollide;

            BHeight = m_p_data->GetHeightSafe(x, y);
            isBCollide = BHeight > minO2Height;

            DHeight = m_p_data->GetHeightSafe(x, y + 1);
            isDCollide = DHeight > minO2Height;

            lastHtopColide = topColide;
            lastHbotColide = botColide;
            topColide = false;
            botColide = false;
            curVtopCollide[VtopColliedPtr] = false;

            isAorDCollide = isACollide || isDCollide;
            if (isAorDCollide || isBCollide || isCCollide)
            {
                tdist[2] = center[2] - AHeight;

                if(dFabs(CHeight - BHeight) < REAL(0.01) )
                {
                    if(dFabs(DHeight - AHeight) < REAL(0.01) )
                    {
                        // near horizontal flat
                        if (tdist[0] >= REAL(0.0) && tdist[0] < REAL(1.0) &&
                            tdist[1] >= REAL(0.0) && tdist[1] < REAL(1.0))
                        {
                            depth = radius - tdist[2];
                            if (depth > dEpsilon)
                            {
                                dist[0] = 0;
                                dist[1] = 0;
                                dist[2] = tdist[2];
                                dOSTerrainAddSphereContact(ContactBuffer, skip, dist, depth, numTerrainContacts);
                                curVtopCollide[VtopColliedPtr] = true;
                                topColide = true;
                                botColide = true;
                            }
                            continue;
                        }
                    }
                    else
                    {
                        dz1 = CHeight - DHeight;
                        dz2 = AHeight - CHeight;
                        dV3CrossTerrain(normA, dz1, dz2);

                        k = dCalcVectorDot3(tdist, normA);
                        depth = radius - k;
                        if (depth > dEpsilon)
                        {
                            px = tdist[0] - radius*normA[0];

                            if (px >= REAL(0.0) && px < REAL(1.0))
                            {
                                py = tdist[1] - radius*normA[1];

                                if(py >= REAL(0.0) && py < REAL(1.0))
                                {
                                    dist[0] = tdist[0] - px;
                                    dist[1] = tdist[1] - py;
                                    dist[2] = k;
                                    dOSTerrainAddSphereContact(ContactBuffer, skip, dist, depth, numTerrainContacts);
                                    curVtopCollide[VtopColliedPtr] = true;
                                    topColide = true;
                                    botColide = true;
                                    if(normA[2] > REAL(0.9))
                                        continue;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (isAorDCollide || isCCollide)
                    {
                        dz1 = CHeight - DHeight;
                        dz2 = AHeight - CHeight;
                        dV3CrossTerrain(normA, dz1, dz2);

                        k = dCalcVectorDot3(tdist, normA);
                        depth = radius - k;
                        if (depth > dEpsilon)
                        {
                            px = tdist[0] - radius*normA[0];

                            if (px >= REAL(0.0) && px < REAL(1.0))
                            {
                                py = tdist[1] - radius*normA[1];

                                if(py > px && py >= REAL(0.0) && py < REAL(1.0))
                                {
                                    dist[0] = tdist[0] - px;
                                    dist[1] = tdist[1] - py;
                                    dist[2] = k;
                                    dOSTerrainAddSphereContact(ContactBuffer, skip, dist, depth, numTerrainContacts);
                                    topColide = true;
                                    curVtopCollide[VtopColliedPtr] = true;
                                }
                            }
                        }
                    }

                    if (isAorDCollide || isBCollide)
                    {
                        dz1 = AHeight - BHeight;
                        dz2 = BHeight - DHeight;
                        dV3CrossTerrain(normB, dz1, dz2);

                        k = dCalcVectorDot3(tdist, normB);
                        depth = radius - k;
                        if (depth > dEpsilon)
                        {
                            px = tdist[0] - radius*normB[0];
                            if (px >= REAL(0.0) && px < REAL(1.0))
                            {
                                py = tdist[1] - radius*normB[1];
                                if (px > py && py >= REAL(0.0) && py < REAL(1.0))
                                {
                                    dist[0] = tdist[0] - px;
                                    dist[1] = tdist[1] - py;
                                    dist[2] = k;
                                    dOSTerrainAddSphereContact(ContactBuffer, skip, dist, depth, numTerrainContacts);
                                    botColide = true;
                                }
                            }
                        }
                    }
                }
  
                depth = 1e30;
                
                if (!topColide)
                {
                    if (!lastHbotColide)
                    {
                        dz2 = CHeight - AHeight;

                        dReal t = (tdist[1] + tdist[2] * dz2);
                        if (t > dEpsilon)
                        {
                            dReal ln = REAL(1.0) + dz2 * dz2;
                            if (t < ln)
                            {
                                t = t / ln;
                                dist[2] = tdist[2] - dz2 * t;

                                if (dist[2] > dEpsilon)
                                {
                                    dist[0] = tdist[0];
                                    dist[1] = tdist[1] - t;

                                    k = dCalcVectorLengthSquare3(dist);
                                    if (k < depth)
                                    {
                                        depth = k;
                                        dCopyVector3r4(pdist, dist);
                                    }
                                }
                            }
                        }
                    }
                    if (!botColide && isAorDCollide)
                    {
                        //AD
                        dReal dz = DHeight - AHeight;
                        // dist dot dir
                        dReal t = (tdist[0] + tdist[1] + tdist[2] * dz);
                        if (t > dEpsilon)
                        {
                            dReal ln = REAL(2.0) + dz * dz;                          
                            if (t < ln)
                            {
                                t = t / ln;
                                dist[2] = tdist[2] - dz * t;
                                if (dist[2] > dEpsilon)
                                {
                                    dist[0] = tdist[0] - t;
                                    dist[1] = tdist[1] - t;
                                    k = dCalcVectorLengthSquare3(dist);
                                    if (k < depth)
                                    {
                                        depth = k;
                                        dCopyVector3r4(pdist, dist);
                                    }
                                }
                            }
                        }
                    }
                }

                if (!botColide && !lastVtopCollide[VtopColliedPtr])
                {
                    //AB
                    dz1 = BHeight - AHeight;
    
                    dReal t = (tdist[0] + tdist[2] * dz1);
                    if (t > dEpsilon)
                    {
                        dReal ln = REAL(1.0) + dz1 * dz1;
                        if(t < ln)
                        {
                           t = t / ln;                               
                           dist[2] = tdist[2] - dz1 * t;
                           if (tdist[2] > dEpsilon)
                           {
                               dist[0] = tdist[0] - t;
                               dist[1] = tdist[1];
                               k = dCalcVectorLengthSquare3(dist);
                               if (k < depth)
                               {
                                   depth = k;
                                   dCopyVector3r4(pdist, dist);
                               }
                           }
                        }
                    }
                    else if (!topColide && !lastHbotColide && tdist[2] > dEpsilon && !lastVtopCollide[VtopColliedPtr -1])
                    {
                       k = dCalcVectorLengthSquare3(tdist);
                       if (k < depth)
                       {
                           depth = k;
                           dCopyVector3r4(pdist, tdist);
                       }
                    }
                }

                if (depth < radiussq && depth > dEpsilon)
                {
                    depth = radius - dSqrt(depth);
                    dOSTerrainAddSphereContact(ContactBuffer, skip, pdist, depth, numTerrainContacts);
                }
            }
        } // for x
        
    } // for y

//    t1 = getClockTicksMs() - tstart;

    delete[] VtopCollideA;
    delete[] VtopCollideB;

    if (numTerrainContacts > 0)
    {
        dContactGeom *pContactB;
        numTerrainContacts = 1;

        pContactB = CONTACT(ContactBuffer, 0);
        pContact = CONTACT(contact, 0);

        pContact->side1 = -1;
        pContact->side2 = -1;
        pContact->depth = pContactB->depth;
        dCopyNegatedVector3r4(pContact->normal, pContactB->pos);
        dNormalize3(pContact->normal);
        dSubtractVectors3r4(pContact->pos, center, pContactB->pos);

        pContactB = CONTACT(ContactBuffer, skip);
        if(pContactB->depth > 0)
        {
            pContact = CONTACT(contact, skip);
            pContact->side1 = -1;
            pContact->side2 = -1;
            pContact->depth = pContactB->depth;
            dCopyNegatedVector3r4(pContact->normal, pContactB->pos);
            dNormalize3(pContact->normal);
            dSubtractVectors3r4(pContact->pos, center, pContactB->pos);
            numTerrainContacts++;
        }

        pContactB = CONTACT(ContactBuffer, 2 * skip);
        if(pContactB->depth > 0)
        {
            pContact = CONTACT(contact, numTerrainContacts * skip);
            pContact->side1 = -1;
            pContact->side2 = -1;
            pContact->depth = pContactB->depth;
            dCopyNegatedVector3r4(pContact->normal, pContactB->pos);
            dNormalize3(pContact->normal);
            dSubtractVectors3r4(pContact->pos, center, pContactB->pos);
            numTerrainContacts++;
        }

        pContactB = CONTACT(ContactBuffer,3 * skip);
        if(pContactB->depth > 0)
        {
            pContact = CONTACT(contact, numTerrainContacts * skip);
            pContact->side1 = -1;
            pContact->side2 = -1;
            pContact->depth = pContactB->depth;
            dCopyNegatedVector3r4(pContact->normal, pContactB->pos);
            dNormalize3(pContact->normal);
            dSubtractVectors3r4(pContact->pos, center, pContactB->pos);
            numTerrainContacts++;
        }

        pContactB = CONTACT(ContactBuffer, 4 * skip);
        if(pContactB->depth > 0)
        {
            pContact = CONTACT(contact, numTerrainContacts * skip);
            pContact->side1 = -1;
            pContact->side2 = -1;
            pContact->depth = pContactB->depth;
            dCopyNegatedVector3r4(pContact->normal, pContactB->pos);
            dNormalize3(pContact->normal);
            dSubtractVectors3r4(pContact->pos, center, pContactB->pos);
            numTerrainContacts++;
        }
        return numTerrainContacts;
    }
    return 0;
}


int dCollideOSTerrain( dxGeom *o1, dxGeom *o2, int flags, dContactGeom* contact, int skip )
{
    dIASSERT( skip >= (int)sizeof(dContactGeom) );
    dIASSERT( o1->type == dOSTerrainClass );
    dIASSERT((flags & NUMC_MASK) >= 1);

    int i;
	int nMinX;
	int nMaxX;
	
	int nMinY;
	int nMaxY;

    int numMaxTerrainContacts;

    // if ((flags & NUMC_MASK) == 0) -- An assertion check is made on entry
	//	{ flags = (flags & ~NUMC_MASK) | 1; dIASSERT((1 & ~NUMC_MASK) == 0); }

    dxOSTerrain *terrain = (dxOSTerrain*) o1;
    dxOSTerrainData *tdata = terrain->m_p_data;

    if(o2->aabb[4] > tdata-> m_fMaxHeight)
        return 0;


    int numTerrainContacts = 0;

    //
    // Collide
    //

    //check if inside boundaries
    // using O2 aabb
    //  aabb[6] is (minx, maxx, miny, maxy, minz, maxz)

    dReal offsetX = terrain->final_posr->pos[0] - tdata->m_fHalfWidth;
    dReal offsetY = terrain->final_posr->pos[1] - tdata->m_fHalfDepth;
    dReal o2minx = o2->aabb[0] - offsetX;
    dReal o2maxx = o2->aabb[1] - offsetX;
    dReal o2miny = o2->aabb[2] - offsetY;
    dReal o2maxy = o2->aabb[3] - offsetY;

    if ( o2minx > tdata->m_fWidth //MinX
         ||  o2miny > tdata->m_fDepth)//MinY
       return 0;

    if ( o2maxx < 0 //MaxX
         ||  o2maxy < 0)//MaxY
       return 0;

	// To narrow scope of following variables
	nMinX = (int)dFloor(dNextAfter(o2minx, -dInfinity));
    nMinX = dMAX( nMinX, 0 );
	nMaxX = (int)dCeil(dNextAfter(o2maxx, dInfinity));
	nMaxX = dMIN( nMaxX, tdata->m_nWidthSamples - 1 );
	
	nMinY = (int)dFloor(dNextAfter(o2miny, -dInfinity));
	nMinY = dMAX( nMinY, 0 );
	nMaxY = (int)dCeil(dNextAfter(o2maxy, dInfinity));
	nMaxY = dMIN( nMaxY, tdata->m_nDepthSamples - 1 );
    dIASSERT ((nMinX < nMaxX) && (nMinY < nMaxY));

    dContactGeom *pContact;

    numMaxTerrainContacts = (flags & NUMC_MASK);

    if(o2->type == dSphereClass)
    {
        numTerrainContacts = terrain->dCollideOSTerrainSphere(
            nMinX,nMaxX,nMinY,nMaxY,o2,numMaxTerrainContacts - numTerrainContacts,
            flags,CONTACT(contact,numTerrainContacts*skip),skip	);
    }
    else    
    {
        numTerrainContacts = terrain->dCollideOSTerrainZone(
            nMinX,nMaxX,nMinY,nMaxY,o2,numMaxTerrainContacts - numTerrainContacts,
            flags,CONTACT(contact,numTerrainContacts*skip),skip	);
    }
    dIASSERT( numTerrainContacts <= numMaxTerrainContacts );

    for ( i = 0; i < numTerrainContacts; ++i )
    {
        pContact = CONTACT(contact,i*skip);
        pContact->g1 = o1;
        pContact->g2 = o2;
		// pContact->side1 = -1; -- Oleh_Derevenko: sides must not be erased here as they are set by respective colliders during ray/plane tests 
		// pContact->side2 = -1;
    }


    //------------------------------------------------------------------------------

//dCollideOSTerrainExit:

    return numTerrainContacts;
}

