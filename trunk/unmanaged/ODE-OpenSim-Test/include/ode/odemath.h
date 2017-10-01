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

#ifndef _ODE_ODEMATH_H_
#define _ODE_ODEMATH_H_

#include <ode/common.h>

/*
 * macro to access elements i,j in an NxM matrix A, independent of the
 * matrix storage convention.
 */
#define dACCESS33(A,i,j) ((A)[(i)*4+(j)])

/*
 * Macro to test for valid floating point values
 */
#define dVALIDVEC3(v) (!(dIsNan(v[0]) || dIsNan(v[1]) || dIsNan(v[2])))
#define dVALIDVEC4(v) (!(dIsNan(v[0]) || dIsNan(v[1]) || dIsNan(v[2]) || dIsNan(v[3])))
#define dVALIDMAT3(m) (!(dIsNan(m[0]) || dIsNan(m[1]) || dIsNan(m[2]) || dIsNan(m[3]) || dIsNan(m[4]) || dIsNan(m[5]) || dIsNan(m[6]) || dIsNan(m[7]) || dIsNan(m[8]) || dIsNan(m[9]) || dIsNan(m[10]) || dIsNan(m[11])))
#define dVALIDMAT4(m) (!(dIsNan(m[0]) || dIsNan(m[1]) || dIsNan(m[2]) || dIsNan(m[3]) || dIsNan(m[4]) || dIsNan(m[5]) || dIsNan(m[6]) || dIsNan(m[7]) || dIsNan(m[8]) || dIsNan(m[9]) || dIsNan(m[10]) || dIsNan(m[11]) || dIsNan(m[12]) || dIsNan(m[13]) || dIsNan(m[14]) || dIsNan(m[15]) ))

/* Some vector math */
#if defined(__AVX__)
ODE_PURE_INLINE void dAddVector3(dReal *res, const dReal *a)
{
    dReal restmp[4];
    __m128 ma, mb;
    ma = _mm_loadu_ps(a);
    mb = _mm_loadu_ps(res);
    ma = _mm_add_ps(ma, mb);

    _mm_storeu_ps(restmp, ma);
    res[0] = restmp[0];
    res[1] = restmp[1];
    res[2] = restmp[2];
}
#else
ODE_PURE_INLINE void dAddVector3(dReal *res, const dReal *a)
{
    res[0] += a[0];
    res[1] += a[1];
    res[2] += a[2];
}
#endif

#if defined(__AVX__)
ODE_PURE_INLINE void dAddVectors3(dReal *res, const dReal *a, const dReal *b)
{
    dReal restmp[4];
    __m128 ma, mb;
    ma = _mm_loadu_ps(a);
    mb = _mm_loadu_ps(b);
    ma = _mm_add_ps(ma, mb);

    _mm_storeu_ps(restmp, ma);
    res[0] = restmp[0];
    res[1] = restmp[1];
    res[2] = restmp[2];
}
#else
ODE_PURE_INLINE void dAddVectors3(dReal *res, const dReal *a, const dReal *b)
{
    const dReal res_0 = a[0] + b[0];
    const dReal res_1 = a[1] + b[1];
    const dReal res_2 = a[2] + b[2];
    /* Only assign after all the calculations are over to avoid incurring memory aliasing*/
    res[0] = res_0;
    res[1] = res_1;
    res[2] = res_2;
}
#endif

#if defined(__AVX__)
ODE_PURE_INLINE void dSubtractVectors3(dReal *res, const dReal *a, const dReal *b)
{
    dReal restmp[4];
    __m128 ma, mb;
    ma = _mm_loadu_ps(a);
    mb = _mm_loadu_ps(b);
    ma = _mm_sub_ps(ma, mb);

    _mm_storeu_ps(restmp, ma);
    res[0] = restmp[0];
    res[1] = restmp[1];
    res[2] = restmp[2];
}
#else
ODE_PURE_INLINE void dSubtractVectors3(dReal *res, const dReal *a, const dReal *b)
{
  const dReal res_0 = a[0] - b[0];
  const dReal res_1 = a[1] - b[1];
  const dReal res_2 = a[2] - b[2];
  /* Only assign after all the calculations are over to avoid incurring memory aliasing*/
  res[0] = res_0;
  res[1] = res_1;
  res[2] = res_2;
}
#endif
#if defined(__AVX__)
ODE_PURE_INLINE void dSubNoAliaseVectors3(dReal *res, const dReal *a, const dReal *b)
{
    dSubtractVectors3(res, a, b);
}
#else
ODE_PURE_INLINE void dSubNoAliaseVectors3(dReal *res, const dReal *a, const dReal *b)
{
  res[0] = a[0] - b[0];
  res[1] = a[1] - b[1];
  res[2] = a[2] - b[2];
}
#endif

#if defined(__AVX__)
ODE_PURE_INLINE void dAddScaledVector3(dReal *res, const dReal *a, const dReal scale)
{
    dReal restmp[4];
    __m128 ma, mb, mc;

    ma = _mm_loadu_ps(a);
    mc = _mm_set1_ps(scale);
    mb = _mm_loadu_ps(res);

    ma = _mm_mul_ps(ma, mc);
    ma = _mm_add_ps(ma, mb);

    _mm_storeu_ps(restmp, ma);
    res[0] = restmp[0];
    res[1] = restmp[1];
    res[2] = restmp[2];
}
#else
ODE_PURE_INLINE void dAddScaledVector3(dReal *res, const dReal *a, const dReal scale)
{
    res[0] += scale * a[0];
    res[1] += scale * a[1];
    res[2] += scale * a[2];
}
#endif

#if defined(__AVX__)
ODE_PURE_INLINE void dAddScaledVector4(dReal *res, const dReal *a, const dReal scale)
{
    __m128 ma, mb, mc;

    ma = _mm_loadu_ps(a);
    mc = _mm_set1_ps(scale);
    mb = _mm_loadu_ps(res);

    ma = _mm_mul_ps(ma, mc);
    ma = _mm_add_ps(ma, mb);

    _mm_storeu_ps(res, ma);
}
#else
ODE_PURE_INLINE void dAddScaledVector4(dReal *res, const dReal *a, const dReal scale)
{
    res[0] += scale * a[0];
    res[1] += scale * a[1];
    res[2] += scale * a[2];
    res[3] += scale * a[3];
}
#endif


#if defined(__AVX__)
ODE_PURE_INLINE void dAddScaledVectors3(dReal *res, const dReal *a, const dReal *b, dReal a_scale, dReal b_scale)
{
    dReal restmp[4];
    __m128 ma, mb, mc;

    ma = _mm_loadu_ps(a);
    mc = _mm_set1_ps(a_scale);
    ma = _mm_mul_ps(ma, mc);

    mb = _mm_loadu_ps(b);
    mc = _mm_set1_ps(b_scale);
    mb = _mm_mul_ps(mb, mc);

    ma = _mm_add_ps(ma, mb);

    _mm_storeu_ps(restmp, ma);
    res[0] = restmp[0];
    res[1] = restmp[1];
    res[2] = restmp[2];
}
#else
ODE_PURE_INLINE void dAddScaledVectors3(dReal *res, const dReal *a, const dReal *b, dReal a_scale, dReal b_scale)
{
  const dReal res_0 = a_scale * a[0] + b_scale * b[0];
  const dReal res_1 = a_scale * a[1] + b_scale * b[1];
  const dReal res_2 = a_scale * a[2] + b_scale * b[2];
  /* Only assign after all the calculations are over to avoid incurring memory aliasing*/
  res[0] = res_0; res[1] = res_1; res[2] = res_2;
}
#endif

#if defined(__AVX__)
ODE_PURE_INLINE void dScaleVector3(dReal *res, dReal nScale)
{
    dReal restmp[4];
    __m128 ma, mc;
    ma = _mm_loadu_ps(res);
    mc = _mm_set1_ps(nScale);
    ma = _mm_mul_ps(ma, mc);

    _mm_storeu_ps(restmp, ma);
    res[0] = restmp[0];
    res[1] = restmp[1];
    res[2] = restmp[2];
}
#else
ODE_PURE_INLINE void dScaleVector3(dReal *res, dReal nScale)
{
    res[0] *= nScale;
    res[1] *= nScale;
    res[2] *= nScale;
}
#endif

#if defined(__AVX__)
ODE_PURE_INLINE void dScaleVector4(dReal *res, dReal nScale)
{
    __m128 ma, mc;
    ma = _mm_loadu_ps(res);
    mc = _mm_set1_ps(nScale);
    ma = _mm_mul_ps(ma, mc);

    _mm_storeu_ps(res, ma);
}
#else
ODE_PURE_INLINE void dScaleVector4(dReal *res, dReal nScale)
{
    res[0] *= nScale;
    res[1] *= nScale;
    res[2] *= nScale;
    res[3] *= nScale;
}
#endif

#if defined(__AVX__)
ODE_PURE_INLINE void dCopyVector3(dReal *res, const dReal *a)
{
    dReal restmp[4];
    __m128 ma;

    ma = _mm_loadu_ps(a);
    _mm_storeu_ps(restmp, ma);
    res[0] = restmp[0];
    res[1] = restmp[1];
    res[2] = restmp[2];
}
#else
ODE_PURE_INLINE void dCopyVector3(dReal *res, const dReal *a)
{
  const dReal res_0 = a[0];
  const dReal res_1 = a[1];
  const dReal res_2 = a[2];
  /* Only assign after all the calculations are over to avoid incurring memory aliasing*/
  res[0] = res_0; res[1] = res_1; res[2] = res_2;
}
#endif

#if defined(__AVX__)
ODE_PURE_INLINE void dCopyFabsVector3(dReal *res, const dReal *a)
{
    dReal restmp[4];
    __m128 ma, sign;

    ma = _mm_loadu_ps(a);
    sign = _mm_set1_ps(-0.0f);
    ma = _mm_andnot_ps(sign, ma);

    _mm_storeu_ps(restmp, ma);
    res[0] = restmp[0];
    res[1] = restmp[1];
    res[2] = restmp[2];
}
#else
ODE_PURE_INLINE void dCopyFabsVector3(dReal *res, const dReal *a)
{
    const dReal res_0 = dFabs(a[0]);
    const dReal res_1 = dFabs(a[1]);
    const dReal res_2 = dFabs(a[2]);
    /* Only assign after all the calculations are over to avoid incurring memory aliasing*/
    res[0] = res_0; res[1] = res_1; res[2] = res_2;
}
#endif

#if defined(__AVX__)
ODE_PURE_INLINE void dCopyScaledVector3(dReal *res, const dReal *a, dReal nScale)
{
    dReal restmp[4];
    __m128 ma, mc;
    ma = _mm_loadu_ps(a);
    mc = _mm_set1_ps(nScale);
    ma = _mm_mul_ps(ma, mc);

    _mm_storeu_ps(restmp, ma);
    res[0] = restmp[0];
    res[1] = restmp[1];
    res[2] = restmp[2];
}
#else
ODE_PURE_INLINE void dCopyScaledVector3(dReal *res, const dReal *a, dReal nScale)
{
  const dReal res_0 = a[0] * nScale;
  const dReal res_1 = a[1] * nScale;
  const dReal res_2 = a[2] * nScale;
  /* Only assign after all the calculations are over to avoid incurring memory aliasing*/
  res[0] = res_0; res[1] = res_1; res[2] = res_2;
}
#endif

#if defined(__AVX__)
ODE_PURE_INLINE void  dCopyNegatedVector3(dReal *res, const dReal *a)
{
    dReal restmp[4];
    __m128 ma, mc;

    ma = _mm_loadu_ps(a);
    mc = _mm_setzero_ps();
    ma = _mm_sub_ps(mc, ma);

    _mm_storeu_ps(restmp, ma);
    res[0] = restmp[0];
    res[1] = restmp[1];
    res[2] = restmp[2];
}
#else
ODE_PURE_INLINE void dCopyNegatedVector3(dReal *res, const dReal *a)
{
    const dReal res_0 = -a[0];
    const dReal res_1 = -a[1];
    const dReal res_2 = -a[2];
    /* Only assign after all the calculations are over to avoid incurring memory aliasing*/
    res[0] = res_0; res[1] = res_1; res[2] = res_2;
}
#endif

#if defined(__AVX__)
ODE_PURE_INLINE void  dNegateVector3(dReal *res)
{
    dReal restmp[4];
    __m128 ma, mc;

    ma = _mm_loadu_ps(res);
    mc = _mm_setzero_ps();
    ma = _mm_sub_ps(mc, ma);

    _mm_storeu_ps(restmp, ma);
    res[0] = restmp[0];
    res[1] = restmp[1];
    res[2] = restmp[2];
}
#else
ODE_PURE_INLINE void dNegateVector3(dReal *res)
{
    const dReal res_0 = -res[0];
    const dReal res_1 = -res[1];
    const dReal res_2 = -res[2];
    /* Only assign after all the calculations are over to avoid incurring memory aliasing*/
    res[0] = res_0; res[1] = res_1; res[2] = res_2;
}
#endif


#if defined(__AVX__)
ODE_PURE_INLINE void dCopyVector4(dReal *res, const dReal *a)
{
    __m128 ma;
    ma = _mm_loadu_ps(a);
    _mm_storeu_ps(res, ma);
}
#else
ODE_PURE_INLINE void dCopyVector4(dReal *res, const dReal *a)
{
  const dReal res_0 = a[0];
  const dReal res_1 = a[1];
  const dReal res_2 = a[2];
  const dReal res_3 = a[3];
  /* Only assign after all the calculations are over to avoid incurring memory aliasing*/
  res[0] = res_0; res[1] = res_1; res[2] = res_2; res[3] = res_3;
}
#endif

#if defined(__AVX__)
ODE_PURE_INLINE void dCopyMatrix4x4(dReal *res, const dReal *a)
{
    __m256 ab;
    ab = _mm256_loadu_ps(a);
    _mm256_storeu_ps(res, ab);
    ab = _mm256_loadu_ps(a + 8);
    _mm256_storeu_ps(res + 8, ab);
}
#else
ODE_PURE_INLINE void dCopyMatrix4x4(dReal *res, const dReal *a)
{
  dCopyVector4(res + 0, a + 0);
  dCopyVector4(res + 4, a + 4);
  dCopyVector4(res + 8, a + 8);
}
#endif

#if defined(__AVX__)
ODE_PURE_INLINE void dCopyMatrix4x3(dReal *res, const dReal *a)
{
    __m128 ma,mb,mc;
    ma = _mm_loadu_ps(a);
    mb = _mm_loadu_ps(a + 4);
    mc = _mm_loadu_ps(a + 8);
    _mm_storeu_ps(res, ma);
    _mm_storeu_ps(res + 4, mb);
    _mm_storeu_ps(res + 8, mc);
}
#else
ODE_PURE_INLINE void dCopyMatrix4x3(dReal *res, const dReal *a)
{
  dCopyVector3(res + 0, a + 0);
  dCopyVector3(res + 4, a + 4);
  dCopyVector3(res + 8, a + 8);
}
#endif
ODE_PURE_INLINE void dGetMatrixColumn3(dReal *res, const dReal *a, unsigned n)
{
  const dReal res_0 = a[n + 0];
  const dReal res_1 = a[n + 4];
  const dReal res_2 = a[n + 8];
  /* Only assign after all the calculations are over to avoid incurring memory aliasing*/
  res[0] = res_0; res[1] = res_1; res[2] = res_2;
}

#if defined(__AVX__)
ODE_PURE_INLINE dReal dCalcVectorLength3(const dReal *a)
{
    __m128 ma;
    ma = _mm_loadu_ps(a);
    ma = _mm_dp_ps(ma, ma, 0x71);
    dReal l = (dReal)_mm_cvtss_f32(ma);
    return dSqrt(l);
}
#else
ODE_PURE_INLINE dReal dCalcVectorLength3(const dReal *a)
{
    return dSqrt(a[0] * a[0] + a[1] * a[1] + a[2] * a[2]);
}
#endif

#if defined(__AVX__)
ODE_PURE_INLINE dReal dCalcVectorLength4(const dReal *a)
{
    __m128 ma;
    ma = _mm_loadu_ps(a);
    ma = _mm_dp_ps(ma, ma, 0xf1);
    dReal l = (dReal)_mm_cvtss_f32(ma);
    return dSqrt(l);
}
#else
ODE_PURE_INLINE dReal dCalcVectorLength4(const dReal *a)
{
    return dSqrt(a[0] * a[0] + a[1] * a[1] + a[2] * a[2] + a[3] * a[3]);
}
#endif

#if defined(__AVX__)
ODE_PURE_INLINE dReal dCalcVectorLengthSquare3(const dReal *a)
{
    __m128 ma;
    ma = _mm_loadu_ps(a);
    ma = _mm_dp_ps(ma, ma, 0x71);
    return (dReal)_mm_cvtss_f32(ma);
}
#else
ODE_PURE_INLINE dReal dCalcVectorLengthSquare3(const dReal *a)
{
    return (a[0] * a[0] + a[1] * a[1] + a[2] * a[2]);
}
#endif

#if defined(__AVX__)
ODE_PURE_INLINE dReal dCalcVectorLengthSquare4(const dReal *a)
{
    __m128 ma;
    ma = _mm_loadu_ps(a);
    ma = _mm_dp_ps(ma, ma, 0xf1);
    return (dReal)_mm_cvtss_f32(ma);
}
#else
ODE_PURE_INLINE dReal dCalcVectorLengthSquare4(const dReal *a)
{
    return (a[0] * a[0] + a[1] * a[1] + a[2] * a[2] + a[3] * a[3]);
}
#endif


ODE_PURE_INLINE dReal dCalcVectorLengthSquare3(const dReal x, const dReal y, const dReal z)
{
  return (x * x + y * y + z * z);
}

#if defined(__AVX__)
ODE_PURE_INLINE dReal  dCalcPointDepth3(const dReal *test_p, const dReal *plane_p, const dReal *plane_n)
{
    __m128 mt, mp, mn;
    mp = _mm_loadu_ps(plane_p);
    mt = _mm_loadu_ps(test_p);
    mn = _mm_loadu_ps(plane_n);

    mp = _mm_sub_ps(mp, mt);
    mt = _mm_mul_ps(mp, mn);

    mp = _mm_dp_ps(mt, mt, 0x71);
    return (dReal)_mm_cvtss_f32(mp);
}
#else
ODE_PURE_INLINE dReal dCalcPointDepth3(const dReal *test_p, const dReal *plane_p, const dReal *plane_n)
{
  return (plane_p[0] - test_p[0]) * plane_n[0] + (plane_p[1] - test_p[1]) * plane_n[1] + (plane_p[2] - test_p[2]) * plane_n[2];
}
#endif
#if defined(__AVX__)
ODE_PURE_INLINE dReal dCalcPointPlaneDistance(const dVector3& point, const dVector4& plane)
{
    __m128 mp, mn;
    mp = _mm_loadu_ps(plane);
    mn = _mm_loadu_ps(point);

    mp = _mm_dp_ps(mp, mn, 0x71);
    return (dReal)_mm_cvtss_f32(mp) + plane[3];
}
#else
ODE_PURE_INLINE dReal dCalcPointPlaneDistance(const dVector3& point, const dVector4& plane)
{
    return (plane[0] * point[0] + plane[1] * point[1] + plane[2] * point[2] + plane[3]);
}
#endif

#if defined(__AVX__)
ODE_PURE_INLINE dReal dCalcVectorDot3(const dReal *a, const dReal *b)
{
    __m128 ma, mb;
    ma = _mm_loadu_ps(a);
    mb = _mm_loadu_ps(b);
    ma = _mm_dp_ps(ma, mb, 0x71);

    return (dReal)_mm_cvtss_f32(ma);
}
#else
ODE_PURE_INLINE dReal dCalcVectorDot3(const dReal *a, const dReal *b)
{
  return a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
}
#endif

#if defined(__AVX__)
ODE_PURE_INLINE dReal dCalcVectorDot4(const dReal *a, const dReal *b)
{
    __m128 ma, mb;
    ma = _mm_loadu_ps(a);
    mb = _mm_loadu_ps(b);
    ma = _mm_dp_ps(ma, mb, 0xf1);

    return (dReal)_mm_cvtss_f32(ma);
}
#else
ODE_PURE_INLINE dReal dCalcVectorDot4(const dReal *a, const dReal *b)
{
    return a[0] * b[0] + a[1] * b[1] + a[2] * b[2] + a[3] * b[3];
}
#endif

/*
* 3-way dot product. _dCalcVectorDot3 means that elements of `a' and `b' are spaced
* step_a and step_b indexes apart respectively.
*/
ODE_PURE_INLINE dReal _dCalcVectorDot3(const dReal *a, const dReal *b, unsigned step_a, unsigned step_b)
{
  return a[0] * b[0] + a[step_a] * b[step_b] + a[2 * step_a] * b[2 * step_b];
}

ODE_PURE_INLINE dReal dCalcVectorDot3_13 (const dReal *a, const dReal *b) { return _dCalcVectorDot3(a,b,1,3); }
ODE_PURE_INLINE dReal dCalcVectorDot3_31 (const dReal *a, const dReal *b) { return _dCalcVectorDot3(a,b,3,1); }
ODE_PURE_INLINE dReal dCalcVectorDot3_33 (const dReal *a, const dReal *b) { return _dCalcVectorDot3(a,b,3,3); }
ODE_PURE_INLINE dReal dCalcVectorDot3_14 (const dReal *a, const dReal *b) { return _dCalcVectorDot3(a,b,1,4); }
ODE_PURE_INLINE dReal dCalcVectorDot3_41 (const dReal *a, const dReal *b) { return _dCalcVectorDot3(a,b,4,1); }
ODE_PURE_INLINE dReal dCalcVectorDot3_44 (const dReal *a, const dReal *b) { return _dCalcVectorDot3(a,b,4,4); }

#if defined(__AVX__)
ODE_PURE_INLINE void dCalcVectorCross3(dReal *res, const dReal *a, const dReal *b)
{
    __m128 ma, mb, t1, t2, t3, t4;
    ma = _mm_loadu_ps(a);
    mb = _mm_loadu_ps(b);
    dReal restmp[4];

    t1 = _mm_shuffle_ps(ma, ma, _MM_SHUFFLE(3, 0, 2, 1)); // a1 a2 a0 a3
    t2 = _mm_shuffle_ps(mb, mb, _MM_SHUFFLE(3, 1, 0, 2)); // b2 b0 b1 b2
    
    t3 = _mm_mul_ps(t1, t2); //a1b2 a2b0 a0b1 a3b2

    t1 = _mm_shuffle_ps(t1, t1, _MM_SHUFFLE(3, 0, 2, 1));
    t2 = _mm_shuffle_ps(t2, t2, _MM_SHUFFLE(3, 1, 0, 2));

    t4 = _mm_mul_ps(t1, t2);
    ma = _mm_sub_ps(t3, t4);
    _mm_storeu_ps(restmp, ma);
    res[0] = restmp[0];
    res[1] = restmp[1];
    res[2] = restmp[2];
}
#else
ODE_PURE_INLINE void dCalcVectorCross3(dReal *res, const dReal *a, const dReal *b)
{
  const dReal res_0 = a[1]*b[2] - a[2]*b[1];
  const dReal res_1 = a[2]*b[0] - a[0]*b[2];
  const dReal res_2 = a[0]*b[1] - a[1]*b[0];
  /* Only assign after all the calculations are over to avoid incurring memory aliasing*/
  res[0] = res_0;
  res[1] = res_1;
  res[2] = res_2;
}
#endif
/*
 * cross product, set res = a x b. _dCalcVectorCross3 means that elements of `res', `a'
 * and `b' are spaced step_res, step_a and step_b indexes apart respectively.
 */

ODE_PURE_INLINE void _dCalcVectorCross3(dReal *res, const dReal *a, const dReal *b, unsigned step_res, unsigned step_a, unsigned step_b)
{
  const dReal res_0 = a[  step_a]*b[2*step_b] - a[2*step_a]*b[  step_b];
  const dReal res_1 = a[2*step_a]*b[       0] - a[       0]*b[2*step_b];
  const dReal res_2 = a[       0]*b[  step_b] - a[  step_a]*b[       0];
  /* Only assign after all the calculations are over to avoid incurring memory aliasing*/
  res[         0] = res_0;
  res[  step_res] = res_1;
  res[2*step_res] = res_2;
}

ODE_PURE_INLINE void dCalcVectorCross3_114(dReal *res, const dReal *a, const dReal *b) { _dCalcVectorCross3(res, a, b, 1, 1, 4); }
ODE_PURE_INLINE void dCalcVectorCross3_141(dReal *res, const dReal *a, const dReal *b) { _dCalcVectorCross3(res, a, b, 1, 4, 1); }
ODE_PURE_INLINE void dCalcVectorCross3_144(dReal *res, const dReal *a, const dReal *b) { _dCalcVectorCross3(res, a, b, 1, 4, 4); }
ODE_PURE_INLINE void dCalcVectorCross3_411(dReal *res, const dReal *a, const dReal *b) { _dCalcVectorCross3(res, a, b, 4, 1, 1); }
ODE_PURE_INLINE void dCalcVectorCross3_414(dReal *res, const dReal *a, const dReal *b) { _dCalcVectorCross3(res, a, b, 4, 1, 4); }
ODE_PURE_INLINE void dCalcVectorCross3_441(dReal *res, const dReal *a, const dReal *b) { _dCalcVectorCross3(res, a, b, 4, 4, 1); }
ODE_PURE_INLINE void dCalcVectorCross3_444(dReal *res, const dReal *a, const dReal *b) { _dCalcVectorCross3(res, a, b, 4, 4, 4); }

#if defined(__AVX__)
ODE_PURE_INLINE void dAddVectorCross3(dReal *res, const dReal *a, const dReal *b)
{
    __m128 ma, mb, mc, t1, t2, t3;
    dReal restmp[4];

    ma = _mm_loadu_ps(a);
    mb = _mm_loadu_ps(b);

    t1 = _mm_shuffle_ps(ma, ma, _MM_SHUFFLE(3, 0, 2, 1)); // a1 a2 a0 a3
    t2 = _mm_shuffle_ps(mb, mb, _MM_SHUFFLE(3, 1, 0, 2)); // b2 b0 b1 b2

    t3 = _mm_mul_ps(t1, t2); //a1b2 a2b0 a0b1 a3b2

    t1 = _mm_shuffle_ps(t1, t1, _MM_SHUFFLE(3, 0, 2, 1));
    t2 = _mm_shuffle_ps(t2, t2, _MM_SHUFFLE(3, 1, 0, 2));

    mc = _mm_loadu_ps(res);

    mb = _mm_mul_ps(t1, t2);
    ma = _mm_sub_ps(t3, mb);

    ma = _mm_add_ps(ma, mc);

    _mm_storeu_ps(restmp, ma);
    res[0] = restmp[0];
    res[1] = restmp[1];
    res[2] = restmp[2];
}
#else
ODE_PURE_INLINE void dAddVectorCross3(dReal *res, const dReal *a, const dReal *b)
{
  dReal tmp[3];
  dCalcVectorCross3(tmp, a, b);
  dAddVectors3(res, res, tmp);
}
#endif

#if defined(__AVX__)
ODE_PURE_INLINE void dSubtractVectorCross3(dReal *res, const dReal *a, const dReal *b)
{
    __m128 ma, mb, mc, t1, t2, t3;
    dReal restmp[4];

    ma = _mm_loadu_ps(a);
    mb = _mm_loadu_ps(b);

    t1 = _mm_shuffle_ps(ma, ma, _MM_SHUFFLE(3, 0, 2, 1)); // a1 a2 a0 a3
    t2 = _mm_shuffle_ps(mb, mb, _MM_SHUFFLE(3, 1, 0, 2)); // b2 b0 b1 b2

    t3 = _mm_mul_ps(t1, t2); //a1b2 a2b0 a0b1 a3b2

    t1 = _mm_shuffle_ps(t1, t1, _MM_SHUFFLE(3, 0, 2, 1));
    t2 = _mm_shuffle_ps(t2, t2, _MM_SHUFFLE(3, 1, 0, 2));

    mc = _mm_loadu_ps(res);

    mb = _mm_mul_ps(t1, t2);
    ma = _mm_sub_ps(t3, mb);

    ma = _mm_sub_ps(mc, ma);

    _mm_storeu_ps(restmp, ma);
    res[0] = restmp[0];
    res[1] = restmp[1];
    res[2] = restmp[2];
}
#else
ODE_PURE_INLINE void dSubtractVectorCross3(dReal *res, const dReal *a, const dReal *b)
{
  dReal tmp[3];
  dCalcVectorCross3(tmp, a, b);
  dSubtractVectors3(res, res, tmp);
}
#endif

/*
 * set a 3x3 submatrix of A to a matrix such that submatrix(A)*b = a x b.
 * A is stored by rows, and has `skip' elements per row. the matrix is
 * assumed to be already zero, so this does not write zero elements!
 * if (plus,minus) is (+,-) then a positive version will be written.
 * if (plus,minus) is (-,+) then a negative version will be written.
 */

ODE_PURE_INLINE void dSetCrossMatrixPlus(dReal *res, const dReal *a, unsigned skip)
{
  const dReal a_0 = a[0], a_1 = a[1], a_2 = a[2];
  res[1] = -a_2;
  res[2] = +a_1;
  res[skip+0] = +a_2;
  res[skip+2] = -a_0;
  res[2*skip+0] = -a_1;
  res[2*skip+1] = +a_0;
}

ODE_PURE_INLINE void dSetCrossMatrixMinus(dReal *res, const dReal *a, unsigned skip)
{
  const dReal a_0 = a[0], a_1 = a[1], a_2 = a[2];
  res[1] = +a_2;
  res[2] = -a_1;
  res[skip+0] = -a_2;
  res[skip+2] = +a_0;
  res[2*skip+0] = +a_1;
  res[2*skip+1] = -a_0;
}


/*
 * compute the distance between two 3D-vectors
 */
#if defined(__AVX__)
ODE_PURE_INLINE dReal dCalcPointsDistance3(const dReal *a, const dReal *b)
{
    __m128 ma, mb;
    ma = _mm_loadu_ps(a);
    mb = _mm_loadu_ps(b);
    ma = _mm_sub_ps(ma,mb);
    ma = _mm_dp_ps(ma, mb, 0x71);
    return dSqrt((dReal)_mm_cvtss_f32(ma));
}
#else
ODE_PURE_INLINE dReal dCalcPointsDistance3(const dReal *a, const dReal *b)
{
  dReal res;
  dReal tmp[3];
  dSubtractVectors3(tmp, a, b);
  res = dCalcVectorLength3(tmp);
  return res;
}
#endif

#if defined(__AVX__)
ODE_PURE_INLINE dReal dCalcPointsDistanceSquare3(const dReal *a, const dReal *b)
{
    __m128 ma, mb;
    ma = _mm_loadu_ps(a);
    mb = _mm_loadu_ps(b);
    ma = _mm_sub_ps(ma, mb);
    ma = _mm_dp_ps(ma, mb, 0x71);
    return (dReal)_mm_cvtss_f32(ma);
}
#else
ODE_PURE_INLINE dReal dCalcPointsDistanceSquare3(const dReal *a, const dReal *b)
{
    dReal res;
    dReal tmp[3];
    dSubtractVectors3(tmp, a, b);
    res = dCalcVectorLengthSquare3(tmp);
    return res;
}
#endif

#if defined(__AVX__)
ODE_PURE_INLINE void dCalcLerpVectors3(dReal *res, const dReal *a, const dReal *b, const dReal t)
{
    __m128 ma, mb, mc, t1, t2;
    dReal restmp[4];

    ma = _mm_loadu_ps(a);
    mb = _mm_loadu_ps(b);
    mc = _mm_set1_ps(t);

    t1 = _mm_sub_ps(mb, ma);
    t2 = _mm_mul_ps(t1, mc);
    mb = _mm_add_ps(ma, t2);

    _mm_storeu_ps(restmp, mb);
    res[0] = restmp[0];
    res[1] = restmp[1];
    res[2] = restmp[2];
}
#else
ODE_PURE_INLINE void dCalcLerpVectors3(dReal *res, const dReal *a, const dReal *b, const dReal t)
{
    // res = a + ( b - a ) * t;
    dReal tmp[3];
    tmp[0] = a[0] + (b[0] - a[0]) * t;
    tmp[1] = a[1] + (b[1] - a[1]) * t;
    tmp[0] = a[2] + (b[2] - a[2]) * t;
    res[0] = tmp[0];
    res[1] = tmp[1];
    res[2] = tmp[2];
}
#endif

/*
 * special case matrix multiplication, with operator selection
 */

#if defined(__AVX__)
ODE_PURE_INLINE void dMultiply0_331(dReal *res, const dReal *a, const dReal *b)
{
    __m128 ma, mb,mc;
    dReal restmp[3];
    mb = _mm_loadu_ps(b);
    ma = _mm_loadu_ps(a);
    mc = _mm_dp_ps(ma, mb, 0x71);
    restmp[0] = (dReal)_mm_cvtss_f32(mc);
    ma = _mm_loadu_ps(a + 4);
    mc = _mm_dp_ps(ma, mb, 0x71);
    restmp[1] = (dReal)_mm_cvtss_f32(mc);
    ma = _mm_loadu_ps(a + 8);
    mc = _mm_dp_ps(ma, mb, 0x71);
    restmp[2] = (dReal)_mm_cvtss_f32(mc);

    res[0] = restmp[0];
    res[1] = restmp[1];
    res[2] = restmp[2];
}
#else
ODE_PURE_INLINE void dMultiply0_331(dReal *res, const dReal *a, const dReal *b)
{
  const dReal res_0 = dCalcVectorDot3(a, b);
  const dReal res_1 = dCalcVectorDot3(a + 4, b);
  const dReal res_2 = dCalcVectorDot3(a + 8, b);
  /* Only assign after all the calculations are over to avoid incurring memory aliasing*/
  res[0] = res_0; res[1] = res_1; res[2] = res_2;
}
#endif

#if defined(__AVX__)
ODE_PURE_INLINE void dMultiply1_331(dReal *res, const dReal *a, const dReal *b)
{
    __m128 ma, t0, t1, t2, m0, m1, m2, m3;
    dReal restmp[3];

    t0 = _mm_loadu_ps(a); // a0 a1 a2 a3
    t1 = _mm_loadu_ps(a + 4); // a4 a5 a6 a7
    t2 = _mm_loadu_ps(a + 8); // a8 a9 a10 a11

    m0 = _mm_shuffle_ps(t0, t1, _MM_SHUFFLE(1, 0, 1, 0)); // a0 a1 a4 a5
    m2 = _mm_shuffle_ps(t0, t1, _MM_SHUFFLE(3, 2, 3, 2)); // a2 a3 a6 a7
    m1 = _mm_shuffle_ps(t1, t2, _MM_SHUFFLE(1, 0, 1, 0)); // a4 a5 a8 a9
    m3 = _mm_shuffle_ps(t1, t2, _MM_SHUFFLE(3, 2, 3, 2)); // a6 a7 a10 a11

    ma = _mm_loadu_ps(b);

    t0 = _mm_shuffle_ps(m0, m1, _MM_SHUFFLE(2, 2, 2, 0)); // a0 a4 a8 a8 
    t1 = _mm_shuffle_ps(m0, m1, _MM_SHUFFLE(3, 3, 3, 1)); // a1 a5 a9 a9 
    t2 = _mm_shuffle_ps(m2, m3, _MM_SHUFFLE(2, 2, 2, 0)); // a2 a6 a10 a10

    m0 = _mm_dp_ps(ma, t0, 0x71);
    restmp[0] = _mm_cvtss_f32(m0);
    m1 = _mm_dp_ps(ma, t1, 0x71);
    restmp[1] = _mm_cvtss_f32(m1);
    m2 = _mm_dp_ps(ma, t2, 0x71);
    restmp[2] = _mm_cvtss_f32(m2);

    res[0] = restmp[0];
    res[1] = restmp[1];
    res[2] = restmp[2];
}
#else
ODE_PURE_INLINE void dMultiply1_331(dReal *res, const dReal *a, const dReal *b)
{
  const dReal res_0 = dCalcVectorDot3_41(a, b);
  const dReal res_1 = dCalcVectorDot3_41(a + 1, b);
  const dReal res_2 = dCalcVectorDot3_41(a + 2, b);
  /* Only assign after all the calculations are over to avoid incurring memory aliasing*/
  res[0] = res_0; res[1] = res_1; res[2] = res_2;
}
#endif

ODE_PURE_INLINE void dMultiplyHelper0_133(dReal *res, const dReal *a, const dReal *b)
{
  dMultiply1_331(res, b, a);
}

ODE_PURE_INLINE void dMultiplyHelper1_133(dReal *res, const dReal *a, const dReal *b)
{
  const dReal res_0 = dCalcVectorDot3_44(a, b);
  const dReal res_1 = dCalcVectorDot3_44(a + 1, b);
  const dReal res_2 = dCalcVectorDot3_44(a + 2, b);
  /* Only assign after all the calculations are over to avoid incurring memory aliasing*/
  res[0] = res_0; res[1] = res_1; res[2] = res_2;
}

/* 
Note: NEVER call any of these functions/macros with the same variable for A and C, 
it is not equivalent to A*=B.
*/

#if defined(__AVX__)
ODE_PURE_INLINE void dPointRotateTrans(dReal *res, const dReal *r, const dReal *p, const dReal *t)
{
    __m128 ma, mb, mr, mt;
    dReal restmp[4];

    mb = _mm_loadu_ps(p);
    ma = _mm_loadu_ps(r);
    mr = _mm_dp_ps(ma, mb, 0x7f);

    ma = _mm_loadu_ps(r + 4);
    mt = _mm_dp_ps(ma, mb, 0x7f);
    mr = _mm_blend_ps(mr, mt, 2);

    ma = _mm_loadu_ps(r + 8);
    mt = _mm_dp_ps(ma, mb, 0x7f);
    mr = _mm_blend_ps(mr, mt, 4);

    ma = _mm_loadu_ps(t);
    mr = _mm_add_ps(mr, ma);

    _mm_storeu_ps(restmp, mr);
    res[0] = restmp[0];
    res[1] = restmp[1];
    res[2] = restmp[2];
}
#else
ODE_PURE_INLINE void dPointRotateTrans(dReal *res, const dReal *vec, dReal *rot, const dReal *pos)
{
    dMultiply0_331(res, vec, rot);
    dAddVectors3(res, res, pos);
}
#endif

#if defined(__AVX__)
ODE_PURE_INLINE void dtriangleRotateTrans(dVector3 *res, dVector3 *invec, const dReal *rot, const dVector3 pos)
{
    __m128 mrota, mrotb, mrotc, mpos, ma, mb, mr;
    dReal restmp[4];
    mrota = _mm_loadu_ps(rot);
    mrotb = _mm_loadu_ps(rot + 4);
    mrotc = _mm_loadu_ps(rot + 8);
    mpos = _mm_loadu_ps(pos);
    int i;

    for (i = 0; i < 3; i++)
    {
        ma = _mm_loadu_ps(invec[i]);
        mr = _mm_dp_ps(mrota, ma, 0x7f);
        mb = _mm_dp_ps(mrotb, ma, 0x7f);
        mr = _mm_blend_ps(mr, mb, 2);
        mb = _mm_dp_ps(mrotc, ma, 0x7f);
        mr = _mm_blend_ps(mr, mb, 4);

        mr = _mm_add_ps(mr,mpos);
        _mm_storeu_ps(restmp, mr);
        res[i][0] = restmp[0];
        res[i][1] = restmp[1];
        res[i][2] = restmp[2];
    }
}
#else
ODE_PURE_INLINE void dtriangleRotateTrans(dVector3 *res, dVector3 *invec, const dReal *rot, const dVector3 pos)
{
    int i;
    for (i = 0; i < 3; i++)
    {
        dMultiply0_331(res[i], rot, invec[i]);
        dAddVectors3(res[i], res[i], pos);
    }
}
#endif

ODE_PURE_INLINE void dMultiply0_133(dReal *res, const dReal *a, const dReal *b)
{
  dMultiplyHelper0_133(res, a, b);
}

ODE_PURE_INLINE void dMultiply0_333(dReal *res, const dReal *a, const dReal *b)
{
  dMultiplyHelper0_133(res + 0, a + 0, b);
  dMultiplyHelper0_133(res + 4, a + 4, b);
  dMultiplyHelper0_133(res + 8, a + 8, b);
}

ODE_PURE_INLINE void dMultiply1_333(dReal *res, const dReal *a, const dReal *b)
{
  dMultiplyHelper1_133(res + 0, b, a + 0);
  dMultiplyHelper1_133(res + 4, b, a + 1);
  dMultiplyHelper1_133(res + 8, b, a + 2);
}

ODE_PURE_INLINE void dMultiply2_333(dReal *res, const dReal *a, const dReal *b)
{
  dMultiply0_331(res + 0, b, a + 0);
  dMultiply0_331(res + 4, b, a + 4);
  dMultiply0_331(res + 8, b, a + 8);
}

ODE_PURE_INLINE void dMultiplyAdd0_331(dReal *res, const dReal *a, const dReal *b)
{
  dReal tmp[3];
  dMultiply0_331(tmp, a, b);
  dAddVectors3(res, res, tmp);
}

ODE_PURE_INLINE void dMultiplyAdd1_331(dReal *res, const dReal *a, const dReal *b)
{
  dReal tmp[3];
  dMultiply1_331(tmp, a, b);
  dAddVectors3(res, res, tmp);
}

ODE_PURE_INLINE void dMultiplyAdd0_133(dReal *res, const dReal *a, const dReal *b)
{
  dReal tmp[3];
  dMultiplyHelper0_133(tmp, a, b);
  dAddVectors3(res, res, tmp);
}

ODE_PURE_INLINE void dMultiplyAdd0_333(dReal *res, const dReal *a, const dReal *b)
{
  dReal tmp[3];
  dMultiplyHelper0_133(tmp, a + 0, b);
  dAddVectors3(res + 0, res + 0, tmp);
  dMultiplyHelper0_133(tmp, a + 4, b);
  dAddVectors3(res + 4, res + 4, tmp);
  dMultiplyHelper0_133(tmp, a + 8, b);
  dAddVectors3(res + 8, res + 8, tmp);
}

ODE_PURE_INLINE void dMultiplyAdd1_333(dReal *res, const dReal *a, const dReal *b)
{
  dReal tmp[3];
  dMultiplyHelper1_133(tmp, b, a + 0);
  dAddVectors3(res + 0, res + 0, tmp);
  dMultiplyHelper1_133(tmp, b, a + 1);
  dAddVectors3(res + 4, res + 4, tmp);
  dMultiplyHelper1_133(tmp, b, a + 2);
  dAddVectors3(res + 8, res + 8, tmp);
}

ODE_PURE_INLINE void dMultiplyAdd2_333(dReal *res, const dReal *a, const dReal *b)
{
  dReal tmp[3];
  dMultiply0_331(tmp, b, a + 0);
  dAddVectors3(res + 0, res + 0, tmp);
  dMultiply0_331(tmp, b, a + 4);
  dAddVectors3(res + 4, res + 4, tmp);
  dMultiply0_331(tmp, b, a + 8);
  dAddVectors3(res + 8, res + 8, tmp);
}

#if defined(__AVX__)
ODE_PURE_INLINE dReal dCalcMatrix3Det(const dReal* mat)
{
    __m128 ma, mb, t1, t2, t3, t4;
    ma = _mm_loadu_ps(mat + 4);
    mb = _mm_loadu_ps(mat + 8);

    t1 = _mm_shuffle_ps(ma, ma, _MM_SHUFFLE(3, 0, 2, 1)); // 5 6 4 7 
    t2 = _mm_shuffle_ps(mb, mb, _MM_SHUFFLE(3, 1, 0, 2)); // 10 8 9 11
    t3 = _mm_mul_ps(t1, t2); //5*10 6*8 4*9 7*11

    t1 = _mm_shuffle_ps(t1, t1, _MM_SHUFFLE(3, 0, 2, 1)); // 6 4 5 7
    t2 = _mm_shuffle_ps(t2, t2, _MM_SHUFFLE(3, 1, 0, 2)); // 9 10 8 7
    t4 = _mm_mul_ps(t1, t2); // 6*9 4*10 5*6 7*7;

    ma = _mm_sub_ps(t3, t4);

    mb = _mm_loadu_ps(mat); // 0 1 2 3
    t1 = _mm_dp_ps(ma, mb, 0x7f);

    return (dReal)_mm_cvtss_f32(t1);
    }
#else
ODE_PURE_INLINE dReal dCalcMatrix3Det(const dReal* mat)
{
    dReal det;

    det = mat[0] * ( mat[5] * mat[10] - mat[9] * mat[6] )
        + mat[1] * ( mat[8] * mat[6]  - mat[4] * mat[10] )
        + mat[2] * ( mat[4] * mat[9]  - mat[8] * mat[5] );

    return( det );
}
#endif

/**
  Closed form matrix inversion, copied from 
  collision_util.h for use in the stepper.

  Returns the determinant.  
  returns 0 and does nothing
  if the matrix is singular.
*/
ODE_PURE_INLINE dReal dInvertMatrix3(dReal *dst, const dReal *ma)
{
    dReal det;  
    dReal detRecip;

    det = dCalcMatrix3Det( ma );
 
    if ( det == 0 )
    {
        return 0;
    }

    detRecip = dRecip(det);    

    dst[0] =  ( ma[5]*ma[10] - ma[6]*ma[9]  ) * detRecip;
    dst[1] =  ( ma[9]*ma[2]  - ma[1]*ma[10] ) * detRecip;
    dst[2] =  ( ma[1]*ma[6]  - ma[5]*ma[2]  ) * detRecip;

    dst[4] =  ( ma[6]*ma[8]  - ma[4]*ma[10] ) * detRecip;
    dst[5] =  ( ma[0]*ma[10] - ma[8]*ma[2]  ) * detRecip;
    dst[6] =  ( ma[4]*ma[2]  - ma[0]*ma[6]  ) * detRecip;

    dst[8] =  ( ma[4]*ma[9]  - ma[8]*ma[5]  ) * detRecip;
    dst[9] =  ( ma[8]*ma[1]  - ma[0]*ma[9]  ) * detRecip;
    dst[10] = ( ma[0]*ma[5]  - ma[1]*ma[4]  ) * detRecip;

    return det;
}

#if defined(__AVX__)
ODE_PURE_INLINE void dTransposetMatrix34(dReal *dst, const dReal *a)
{
    __m128 t0, t1, t2, m0, m1, m2, m3;

    t0 = _mm_loadu_ps(a); // a0 a1 a2 a3
    t1 = _mm_loadu_ps(a + 4); // a4 a5 a6 a7
    t2 = _mm_loadu_ps(a + 8); // a8 a9 a10 a11

    m0 = _mm_shuffle_ps(t0, t1, _MM_SHUFFLE(1, 0, 1, 0)); // a0 a1 a4 a5
    m2 = _mm_shuffle_ps(t0, t1, _MM_SHUFFLE(3, 2, 3, 2)); // a2 a3 a6 a7
    m1 = _mm_shuffle_ps(t1, t2, _MM_SHUFFLE(1, 0, 1, 0)); // a4 a5 a8 a9
    m3 = _mm_shuffle_ps(t1, t2, _MM_SHUFFLE(3, 2, 3, 2)); // a6 a7 a10 a11

    t0 = _mm_shuffle_ps(m0, m1, _MM_SHUFFLE(2, 2, 2, 0)); // a0 a4 a8 a8 
    t1 = _mm_shuffle_ps(m0, m1, _MM_SHUFFLE(3, 3, 3, 1)); // a1 a5 a9 a9 
    t2 = _mm_shuffle_ps(m2, m3, _MM_SHUFFLE(2, 2, 2, 0)); // a2 a6 a10 a10
   
    _mm_storeu_ps(dst, t0);
    _mm_storeu_ps(dst + 4, t1);
    _mm_storeu_ps(dst + 8, t2);
}
#else
ODE_PURE_INLINE void dTransposetMatrix34(dReal *dst, const dReal *a)
{
    dst[0] = a[0];
    dst[1] = a[4];
    dst[2] = a[6];

    dst[4] = a[1];
    dst[5] = a[5];
    dst[6] = a[9];

    dst[8] = a[2];
    dst[9] = a[6];
    dst[10] = a[10];
}
#endif
/* Include legacy macros here */
#include <ode/odemath_legacy.h>


#ifdef __cplusplus
extern "C" {
#endif

/*
 * normalize 3x1 and 4x1 vectors (i.e. scale them to unit length)
 */

/* For DLL export*/
ODE_API int  dSafeNormalize3 (dVector3 a);
ODE_API int  dSafeNormalize4 (dVector4 a);
ODE_API void dNormalize3 (dVector3 a); /* Potentially asserts on zero vec*/
ODE_API void dNormalize4 (dVector4 a); /* Potentially asserts on zero vec*/

/*
 * given a unit length "normal" vector n, generate vectors p and q vectors
 * that are an orthonormal basis for the plane space perpendicular to n.
 * i.e. this makes p,q such that n,p,q are all perpendicular to each other.
 * q will equal n x p. if n is not unit length then p will be unit length but
 * q wont be.
 */

ODE_API void dPlaneSpace (const dVector3 n, dVector3 p, dVector3 q);
/* Makes sure the matrix is a proper rotation */
ODE_API void dOrthogonalizeR(dMatrix3 m);



#ifdef __cplusplus
}
#endif


#endif
