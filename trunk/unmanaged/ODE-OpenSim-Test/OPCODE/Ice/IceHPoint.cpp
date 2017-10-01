///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/**
 *	Contains code for homogeneous points.
 *	\file		IceHPoint.cpp
 *	\author		Pierre Terdiman
 *	\date		April, 4, 2000
 */
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/**
 *	Homogeneous point.
 *
 *	Use it:
 *	- for clipping in homogeneous space (standard way)
 *	- to differentiate between points (w=1) and vectors (w=0).
 *	- in some cases you can also use it instead of Point for padding reasons.
 *
 *	\class		HPoint
 *	\author		Pierre Terdiman
 *	\version	1.0
 *	\warning	No cross-product in 4D.
 *	\warning	HPoint *= Matrix3x3 doesn't exist, the matrix is first casted to a 4x4
 */
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Precompiled Header
#include "Stdafx.h"

using namespace IceMaths;

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Point Mul = HPoint * Matrix3x3;
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#if defined(__AVX__)
Point HPoint::operator*(const Matrix3x3& mat) const
{
    __m128 ma, t0, t1, t2, m0, m1, m2, m3;
    float xx, yy, zz;

    ma = _mm_loadu_ps(&x);
    t0 = _mm_loadu_ps(mat.m[0]);
    t1 = _mm_loadu_ps(mat.m[1]);
    t2 = _mm_loadu_ps(mat.m[2]);

    m0 = _mm_shuffle_ps(t0, t1, _MM_SHUFFLE(1, 0, 1, 0)); // x0 y0 x1 y1
    m2 = _mm_shuffle_ps(t0, t1, _MM_SHUFFLE(3, 2, 3, 2)); // z0 w0 z1 w1
    m1 = _mm_shuffle_ps(t1, t2, _MM_SHUFFLE(1, 0, 1, 0)); // x1 y1 x2 y2
    m3 = _mm_shuffle_ps(t1, t2, _MM_SHUFFLE(3, 2, 3, 2)); // z1 w1 z2 w2

    t0 = _mm_shuffle_ps(m0, m1, _MM_SHUFFLE(2, 2, 2, 0)); //x0 x1 x2 x2
    t1 = _mm_shuffle_ps(m0, m1, _MM_SHUFFLE(3, 3, 3, 1)); //y0 y1 y2 y2
    t2 = _mm_shuffle_ps(m2, m3, _MM_SHUFFLE(2, 2, 2, 0)); //z0 z1 z2 z2

    m0 = _mm_dp_ps(ma, t0, 0x71);
    xx = _mm_cvtss_f32(m0);
    m1 = _mm_dp_ps(ma, t1, 0x71);
    yy = _mm_cvtss_f32(m1);
    m2 = _mm_dp_ps(ma, t2, 0x71);
    zz = _mm_cvtss_f32(m2);
    return Point(xx, yy, zz);
}
#else
Point HPoint::operator*(const Matrix3x3& mat) const
{
	return Point(
	x * mat.m[0][0] + y * mat.m[1][0] + z * mat.m[2][0],
	x * mat.m[0][1] + y * mat.m[1][1] + z * mat.m[2][1],
	x * mat.m[0][2] + y * mat.m[1][2] + z * mat.m[2][2] );
}
#endif
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// HPoint Mul = HPoint * Matrix4x4;
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#if defined(__AVX__)
HPoint HPoint::operator*(const Matrix4x4& mat) const
{
    __m128 ma, t0, t1, t2, t3, m0, m1, m2, m3;
    float xx, yy, zz, ww;

    ma = _mm_loadu_ps(&x);
    t0 = _mm_loadu_ps(mat.m[0]);
    t1 = _mm_loadu_ps(mat.m[1]);
    t2 = _mm_loadu_ps(mat.m[2]);
    t3 = _mm_loadu_ps(mat.m[3]);

    m0 = _mm_shuffle_ps(t0, t1, _MM_SHUFFLE(1, 0, 1, 0)); // x0 y0 x1 y1
    m2 = _mm_shuffle_ps(t0, t1, _MM_SHUFFLE(3, 2, 3, 2)); // z0 w0 z1 w1
    m1 = _mm_shuffle_ps(t2, t3, _MM_SHUFFLE(1, 0, 1, 0)); // x2 y2 x3 y3
    m3 = _mm_shuffle_ps(t2, t3, _MM_SHUFFLE(3, 2, 3, 2)); // z2 w2 z3 w3

    t0 = _mm_shuffle_ps(m0, m1, _MM_SHUFFLE(2, 0, 2, 0)); //x0 x1 x2 x3
    t1 = _mm_shuffle_ps(m0, m1, _MM_SHUFFLE(3, 1, 3, 1)); //y0 y1 y2 y3
    t2 = _mm_shuffle_ps(m2, m3, _MM_SHUFFLE(2, 0, 2, 0)); //z0 z1 z2 z3
    t3 = _mm_shuffle_ps(m2, m3, _MM_SHUFFLE(3, 1, 2, 1)); //W0 W1 W2 W3

    m0 = _mm_dp_ps(ma, t0, 0xf1);
    xx = _mm_cvtss_f32(m0);
    m1 = _mm_dp_ps(ma, t1, 0xf1);
    yy = _mm_cvtss_f32(m1);
    m2 = _mm_dp_ps(ma, t2, 0xf1);
    zz = _mm_cvtss_f32(m2);
    m3 = _mm_dp_ps(ma, t3, 0xf1);
    ww = _mm_cvtss_f32(m2);
    return HPoint(xx, yy, zz, ww);
}
#else
HPoint HPoint::operator*(const Matrix4x4& mat) const
{
	return HPoint(
	x * mat.m[0][0] + y * mat.m[1][0] + z * mat.m[2][0] + w * mat.m[3][0],
	x * mat.m[0][1] + y * mat.m[1][1] + z * mat.m[2][1] + w * mat.m[3][1],
	x * mat.m[0][2] + y * mat.m[1][2] + z * mat.m[2][2] + w * mat.m[3][2],
	x * mat.m[0][3] + y * mat.m[1][3] + z * mat.m[2][3] + w * mat.m[3][3]);
}
#endif
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// HPoint *= Matrix4x4
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#if defined(__AVX__)
HPoint& HPoint::operator*=(const Matrix4x4& mat)
{
    __m128 ma, t0, t1, t2, t3, m0, m1, m2, m3;
    float xx, yy, zz, ww;

    ma = _mm_loadu_ps(&x);
    t0 = _mm_loadu_ps(mat.m[0]);
    t1 = _mm_loadu_ps(mat.m[1]);
    t2 = _mm_loadu_ps(mat.m[2]);
    t3 = _mm_loadu_ps(mat.m[3]);

    m0 = _mm_shuffle_ps(t0, t1, _MM_SHUFFLE(1, 0, 1, 0)); // x0 y0 x1 y1
    m2 = _mm_shuffle_ps(t0, t1, _MM_SHUFFLE(3, 2, 3, 2)); // z0 w0 z1 w1
    m1 = _mm_shuffle_ps(t2, t3, _MM_SHUFFLE(1, 0, 1, 0)); // x2 y2 x3 y3
    m3 = _mm_shuffle_ps(t2, t3, _MM_SHUFFLE(3, 2, 3, 2)); // z2 w2 z3 w3

    t0 = _mm_shuffle_ps(m0, m1, _MM_SHUFFLE(2, 0, 2, 0)); //x0 x1 x2 x2
    t1 = _mm_shuffle_ps(m0, m1, _MM_SHUFFLE(3, 1, 3, 1)); //y0 y1 y2 y3
    t2 = _mm_shuffle_ps(m2, m3, _MM_SHUFFLE(2, 0, 2, 0)); //z0 z1 z2 z3
    t3 = _mm_shuffle_ps(m2, m3, _MM_SHUFFLE(3, 1, 2, 1)); //W0 W1 W2 W3

    m0 = _mm_dp_ps(ma, t0, 0xf1);
    xx = _mm_cvtss_f32(m0);
    m1 = _mm_dp_ps(ma, t1, 0xf1);
    yy = _mm_cvtss_f32(m1);
    m2 = _mm_dp_ps(ma, t2, 0xf1);
    zz = _mm_cvtss_f32(m2);
    m3 = _mm_dp_ps(ma, t3, 0xf1);
    ww = _mm_cvtss_f32(m2);
    x = xx; y = yy; z = zz; w = ww;

    return	*this;
    }
#else
HPoint& HPoint::operator*=(const Matrix4x4& mat)
{
	float xp = x * mat.m[0][0] + y * mat.m[1][0] + z * mat.m[2][0] + w * mat.m[3][0];
	float yp = x * mat.m[0][1] + y * mat.m[1][1] + z * mat.m[2][1] + w * mat.m[3][1];
	float zp = x * mat.m[0][2] + y * mat.m[1][2] + z * mat.m[2][2] + w * mat.m[3][2];
	float wp = x * mat.m[0][3] + y * mat.m[1][3] + z * mat.m[2][3] + w * mat.m[3][3];

	x = xp; y = yp; z = zp; w = wp;

	return	*this;
}
#endif
