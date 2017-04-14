// dOSTerrain Collider
// Leal Duarte.
// adapted from HeighField:
//  Martijn Buijs 2006 http://home.planet.nl/~buijs512/
// Based on Terrain & Cone contrib by:
//  Benoit CHAPEROT 2003-2004 http://www.jstarlab.com

#ifndef _DOSTERRAIN_H_
#define _DOSTERRAIN_H_
//------------------------------------------------------------------------------

#include <ode/common.h>
#include "collision_kernel.h"

#define OSTERRAINPLAINEPSILON 1e-02F

#define OSTERRAINMAXCONTACTPERCELL 10

class OSTerrainVertex;
class OSTerrainEdge;
class OSTerrainTriangle;

//
// dxOSTerrainData
//
// OSTerrain Data structure
//
struct dxOSTerrainData
{
    dReal m_fWidth;				// World space dimension on X axis
    dReal m_fDepth;				// World space dimension on Y axis


    dReal m_fHalfWidth;			// Cache of half of m_fWidth
    dReal m_fHalfDepth;			// Cache of half of m_fDepth

    dReal m_fMinHeight;        // Min sample height value (scaled and offset)
    dReal m_fMaxHeight;        // Max sample height value (scaled and offset)
    dReal m_fThickness;        // Surface thickness (added to bottom AABB)

    int	m_nWidthSamples;       // Vertex count on X axis edge (number of samples)
    int	m_nDepthSamples;       // Vertex count on Z axis edge (number of samples)
    int m_bCopyHeightData;     // Do we own the sample data?

    const float* m_pHeightData; // Sample data array
    
    dContactGeom            m_contacts[OSTERRAINMAXCONTACTPERCELL];

    dxOSTerrainData();
    ~dxOSTerrainData();

    void SetData( int nWidthSamples, int nDepthSamples,
        float fSampleSize,
        float fThickness, int bWrapMode);

    void ComputeHeightBounds();

    bool IsOnOSTerrain2  ( const OSTerrainVertex * const CellCorner, 
        const dReal * const pos,  const bool isABC) const;

    int GetTriIndex(int x, int y, bool second);

    dReal GetHeight(int x, int z);
    dReal GetHeightSafe(int x, int z);
    dReal GetHeight(dReal x, dReal z);
    void GetNormal( dReal x, dReal y ,dReal *normal);
};

class OSTerrainVertex
{
public:
    OSTerrainVertex(){};

    dVector3 vertex;
    bool state;
};

class OSTerrainEdge
{
public:
    OSTerrainEdge(){};

    OSTerrainVertex   *vertices[2];
};

class OSTerrainTriangle
{
public:
    OSTerrainTriangle(){};

    inline void setMinMax()
    {
        maxAAAB = vertices[0]->vertex[1] > vertices[1]->vertex[1] ? vertices[0]->vertex[1] : vertices[1]->vertex[1];
        maxAAAB = vertices[2]->vertex[1] > maxAAAB  ? vertices[2]->vertex[1] : maxAAAB;
    };

    OSTerrainVertex   *vertices[3];
    dReal               planeDef[4];
    dReal               maxAAAB;

    bool                isFirst;
    bool                state;
};

class OSTerrainPlane
{
public:
    OSTerrainPlane() {};
    ~OSTerrainPlane() {};

    dReal   planeDef[4];
};

//
// dxOSTerrain
//
// OSTerrain geom structure
//
struct dxOSTerrain : public dxGeom
{
    dxOSTerrainData* m_p_data;

    dxOSTerrain( dSpaceID space, dOSTerrainDataID data, int bPlaceable );
    ~dxOSTerrain();

    void computeAABB();
 
    int dCollideOSTerrainSphere( const int minX, const int maxX, const int minZ, const int maxZ,
        dxGeom *o2, const int numMaxContacts,
        int flags, dContactGeom *contact, int skip );
    int dCollideOSTerrainZone( const int minX, const int maxX, const int minZ, const int maxZ,  
        dxGeom *o2, const int numMaxContacts,
        int flags, dContactGeom *contact, int skip );

	enum
	{
		TEMP_PLANE_BUFFER_ELEMENT_COUNT_ALIGNMENT = 4,
		TEMP_HEIGHT_BUFFER_ELEMENT_COUNT_ALIGNMENT_X = 4,
		TEMP_HEIGHT_BUFFER_ELEMENT_COUNT_ALIGNMENT_Y = 4,
		TEMP_TRIANGLE_BUFFER_ELEMENT_COUNT_ALIGNMENT = 1, // Triangles are easy to reallocate and hard to predict
	};

	static inline size_t AlignBufferSize(size_t value, size_t alignment) { dIASSERT((alignment & (alignment - 1)) == 0); return (value + (alignment - 1)) & ~(alignment - 1); }

	void  allocateTriangleBuffer(size_t numTri);
	void  resetTriangleBuffer();
	void  allocatePlaneBuffer(size_t numTri);
	void  resetPlaneBuffer();
	void  allocateHeightBuffer(size_t numX, size_t numZ);
    void  resetHeightBuffer();

    OSTerrainPlane    **tempPlaneBuffer;
    OSTerrainPlane    *tempPlaneInstances;
    size_t              tempPlaneBufferSize;

    OSTerrainTriangle *tempTriangleBuffer;
    size_t              tempTriangleBufferSize;

    OSTerrainVertex   **tempHeightBuffer;
	OSTerrainVertex   *tempHeightInstances;
    size_t              tempHeightBufferSizeX;
    size_t              tempHeightBufferSizeY;

};


//------------------------------------------------------------------------------
#endif //_DOSTERRAIN_H_
