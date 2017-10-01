///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/**
 *    Contains code for 3D vectors.
 *    \file        IcePoint.h
 *    \author        Pierre Terdiman
 *    \date        April, 4, 2000
 */
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Include Guard
#ifndef __ICEPOINT_H__
#define __ICEPOINT_H__

#if defined(__AVX__)
#include <immintrin.h>
#endif

// Forward declarations
    class HPoint;
    class Plane;
    class Matrix3x3;
    class Matrix4x4;

    #define CROSS2D(a, b) (a.x*b.y - b.x*a.y)

    const float EPSILON2 = 1.0e-20f;

    class ICEMATHS_API Point
    {
        public:

        //! Empty constructor
        inline_ Point() {}
        //! Constructor from a single float
//        inline_                    Point(float val) : x(val), y(val), z(val)                    {}
// Removed since it introduced the nasty "Point T = *Matrix4x4.GetTrans();" bug.......
        //! Constructor from floats
        inline_ Point(float xx, float yy, float zz) : x(xx), y(yy), z(zz) {}
        //! Constructor from array
        inline_ Point(const float f[3]) : x(f[0]), y(f[1]), z(f[2]) {}
        //! Copy constructor
        inline_ Point(const Point& p) : x(p.x), y(p.y), z(p.z) {}
        //! Destructor
        inline_ ~Point() {}

        //! Clears the vector
        inline_ Point& Zero() { x = y = z = 0.0f; return *this; }

        //! + infinity
        inline_ Point& SetPlusInfinity() { x = y = z = MAX_FLOAT; return *this; }
        //! - infinity
        inline_ Point& SetMinusInfinity() { x = y = z = MIN_FLOAT; return *this; }

        //! Sets positive unit random vector
        Point& PositiveUnitRandomVector();
        //! Sets unit random vector
        Point& UnitRandomVector();

        //! Assignment from values
        inline_ Point& Set(float xx, float yy, float zz) { x = xx; y = yy; z = zz; return *this; }
        //! Assignment from array
        inline_ Point& Set(const float f[3]) { x = f[0]; y = f[1]; z = f[2]; return *this; }
        //! Assignment from another point
        inline_ Point& Set(const Point& src) { x = src.x; y = src.y; z  = src.z; return *this; }

        //! Adds a vector
#if defined(__AVX__)
        inline_ Point& Add(const Point& p)
        {
            float restmp[4];
            __m128 ma, mb;

            ma = _mm_loadu_ps(&x);
            mb = _mm_loadu_ps(p);
            ma = _mm_add_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            x = restmp[0];
            y = restmp[1];
            z = restmp[2];
            return *this;
        }
#else
        inline_ Point& Add(const Point& p) { x += p.x; y += p.y; z += p.z; return *this; }
#endif
        //! Adds a vector
        inline_ Point& Add(float xx, float yy, float zz) { x += xx; y += yy; z += zz; return *this; }
        //! Adds a vector
#if defined(__AVX__)
        inline_ Point& Add(const float f[3])
        {
            float restmp[4];
            __m128 ma, mb;

            ma = _mm_loadu_ps(&x);
            mb = _mm_loadu_ps(f);
            ma = _mm_add_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            x = restmp[0];
            y = restmp[1];
            z = restmp[2];
            return *this;
        }
#else
        inline_ Point& Add(const float f[3]) { x += f[0]; y += f[1]; z += f[2]; return *this; }
#endif
        //! Adds vectors
#if defined(__AVX__)
        inline_ Point& Add(const Point& p, const Point& q)
        {
            float restmp[4];
            __m128 ma, mb;

            ma = _mm_loadu_ps(p);
            mb = _mm_loadu_ps(q);
            ma = _mm_add_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            x = restmp[0];
            y = restmp[1];
            z = restmp[2];
            return *this;
        }
#else
        inline_ Point& Add(const Point& p, const Point& q)  { x = p.x + q.x; y = p.y + q.y; z = p.z + q.z; return *this; }
#endif

        //! Subtracts a vector
#if defined(__AVX__)
        inline_ Point& Sub(const Point& p)
        {
            float restmp[4];
            __m128 ma, mb;

            ma = _mm_loadu_ps(&x);
            mb = _mm_loadu_ps(p);
            ma = _mm_sub_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            x = restmp[0];
            y = restmp[1];
            z = restmp[2];
            return *this;
        }
#else
        inline_ Point& Sub(const Point& p) { x -= p.x; y -= p.y; z -= p.z; return *this; }
#endif
        //! Subtracts a vector
        inline_ Point& Sub(float xx, float yy, float zz) { x -= xx; y -= yy; z -= zz; return *this; }

        //! Subtracts a vector
#if defined(__AVX__)
        inline_ Point& Sub(const float f[3])
        {
            float restmp[4];
            __m128 ma, mb;

            ma = _mm_loadu_ps(&x);
            mb = _mm_loadu_ps(f);
            ma = _mm_sub_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            x = restmp[0];
            y = restmp[1];
            z = restmp[2];
            return *this;
        }
#else
        inline_ Point& Sub(const float f[3]) { x -= f[0]; y -= f[1]; z -= f[2]; return *this; }
#endif

        //! Subtracts vectors
#if defined(__AVX__)
        inline_ Point& Sub(const Point& p, const Point& q)
        {
            float restmp[4];
            __m128 ma, mb;

            ma = _mm_loadu_ps(p);
            mb = _mm_loadu_ps(q);
            ma = _mm_sub_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            x = restmp[0];
            y = restmp[1];
            z = restmp[2];
            return *this;
        }
#else
        inline_ Point& Sub(const Point& p, const Point& q) { x = p.x - q.x; y = p.y - q.y; z = p.z-q.z; return *this; }
#endif
        //! this = -this
        inline_ Point& Neg() { x = -x; y = -y; z = -z; return *this; }
        //! this = -a
        inline_ Point& Neg(const Point& a) { x = -a.x; y = -a.y; z = -a.z; return *this; }

        //! Multiplies by a scalar
#if defined(__AVX__)
        inline_ Point& Mult(float s)
        {
            float restmp[4];
            __m128 ma, mc;

            ma = _mm_loadu_ps(&x);
            mc = _mm_set1_ps(s);
            ma = _mm_mul_ps(ma, mc);

            _mm_storeu_ps(restmp, ma);
            x = restmp[0];
            y = restmp[1];
            z = restmp[2];
            return *this;
        }
#else
        inline_ Point& Mult(float s) { x *= s; y *= s; z *= s; return *this; }
#endif
        //! this = a * scalar
#if defined(__AVX__)
        inline_ Point& Mult(const Point& a, float scalar)
        {
            float restmp[4];
            __m128 ma, mc;

            ma = _mm_loadu_ps(a);
            mc = _mm_set1_ps(scalar);
            ma = _mm_mul_ps(ma, mc);

            _mm_storeu_ps(restmp, ma);
            x = restmp[0];
            y = restmp[1];
            z = restmp[2];
            return *this;
        }
#else
        inline_ Point& Mult(const Point& a, float scalar)
        {
            x = a.x * scalar;
            y = a.y * scalar;
            z = a.z * scalar;
            return *this;
        }
#endif
        //! this = a + b * scalar
#if defined(__AVX__)
        inline_ Point& Mac(const Point& a, const Point& b, float scalar)
        {
            float restmp[4];
            __m128 ma, mb, mc;

            mb = _mm_loadu_ps(b);
            mc = _mm_set1_ps(scalar);
            mb = _mm_mul_ps(mb, mc);

            ma = _mm_loadu_ps(a);
            ma = _mm_add_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            x = restmp[0];
            y = restmp[1];
            z = restmp[2];
            return *this;
        }
#else
        inline_ Point& Mac(const Point& a, const Point& b, float scalar)
        {
            x = a.x + b.x * scalar;
            y = a.y + b.y * scalar;
            z = a.z + b.z * scalar;
            return *this;
        }
#endif
        //! this = this + a * scalar
#if defined(__AVX__)
        inline_ Point& Mac(const Point& a, float scalar)
        {
            float restmp[4];
            __m128 ma, mb, mc;

            mb = _mm_loadu_ps(a);
            mc = _mm_set1_ps(scalar);
            mb = _mm_mul_ps(mb, mc);

            ma = _mm_loadu_ps(&x);
            ma = _mm_add_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            x = restmp[0];
            y = restmp[1];
            z = restmp[2];
            return *this;
        }
#else
        inline_ Point& Mac(const Point& a, float scalar)
        {
            x += a.x * scalar;
            y += a.y * scalar;
            z += a.z * scalar;
            return *this;
        }
#endif
        //! this = a - b * scalar
#if defined(__AVX__)
        inline_ Point& Msc(const Point& a, const Point& b, float scalar)
        {
            float restmp[4];
            __m128 ma, mb, mc;

            mb = _mm_loadu_ps(b);
            mc = _mm_set1_ps(scalar);
            mb = _mm_mul_ps(mb, mc);

            ma = _mm_loadu_ps(a);
            ma = _mm_sub_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            x = restmp[0];
            y = restmp[1];
            z = restmp[2];
            return *this;
        }
#else
        inline_ Point& Msc(const Point& a, const Point& b, float scalar)
        {
            x = a.x - b.x * scalar;
            y = a.y - b.y * scalar;
            z = a.z - b.z * scalar;
            return *this;
        }
#endif
        //! this = this - a * scalar
#if defined(__AVX__)
        inline_ Point& Msc(const Point& a, float scalar)
        {
            float restmp[4];
            __m128 ma, mb, mc;

            mb = _mm_loadu_ps(a);
            mc = _mm_set1_ps(scalar);
            mb = _mm_mul_ps(mb, mc);

            ma = _mm_loadu_ps(&x);
            ma = _mm_sub_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            x = restmp[0];
            y = restmp[1];
            z = restmp[2];
            return *this;
        }
#else
        inline_ Point& Msc(const Point& a, float scalar)
        {
            x -= a.x * scalar;
            y -= a.y * scalar;
            z -= a.z * scalar;
            return *this;
        }
#endif
        //! this = a + b * scalarb + c * scalarc
#if defined(__AVX__)
        inline_ Point& Mac2(const Point& a, const Point& b, float scalarb, const Point& c, float scalarc)
        {
            float restmp[4];
            __m128 ma, mb, mc, ms;

            mb = _mm_loadu_ps(b);
            ms = _mm_set1_ps(scalarb);
            mb = _mm_mul_ps(mb, ms);

            mc = _mm_loadu_ps(c);
            ms = _mm_set1_ps(scalarc);
            mc = _mm_mul_ps(mc, ms);
            mb = _mm_add_ps(mb, mc);

            ma = _mm_loadu_ps(a);
            ma = _mm_add_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            x = restmp[0];
            y = restmp[1];
            z = restmp[2];
            return *this;
        }
#else
    inline_ Point& Mac2(const Point& a, const Point& b, float scalarb, const Point& c, float scalarc)
        {
            x = a.x + b.x * scalarb + c.x * scalarc;
            y = a.y + b.y * scalarb + c.y * scalarc;
            z = a.z + b.z * scalarb + c.z * scalarc;
            return *this;
        }
#endif
        //! this = a - b * scalarb - c * scalarc
#if defined(__AVX__)
    inline_ Point& Msc2(const Point& a, const Point& b, float scalarb, const Point& c, float scalarc)
    {
        float restmp[4];
        __m128 ma, mb, mc, ms;

        mb = _mm_loadu_ps(b);
        ms = _mm_set1_ps(scalarb);
        mb = _mm_mul_ps(mb, ms);

        mc = _mm_loadu_ps(c);
        ms = _mm_set1_ps(scalarc);
        mc = _mm_mul_ps(mc, ms);
        mb = _mm_add_ps(mb, mc);

        ma = _mm_loadu_ps(a);
        ma = _mm_sub_ps(ma, mb);

        _mm_storeu_ps(restmp, ma);
        x = restmp[0];
        y = restmp[1];
        z = restmp[2];
        return *this;
    }
#else
        inline_ Point& Msc2(const Point& a, const Point& b, float scalarb, const Point& c, float scalarc)
        {
            x = a.x - b.x * scalarb - c.x * scalarc;
            y = a.y - b.y * scalarb - c.y * scalarc;
            z = a.z - b.z * scalarb - c.z * scalarc;
            return *this;
        }
#endif
        //! this = mat * a
        inline_ Point& Mult(const Matrix3x3& mat, const Point& a);

        //! this = mat1 * a1 + mat2 * a2
        inline_ Point& Mult2(const Matrix3x3& mat1, const Point& a1, const Matrix3x3& mat2, const Point& a2);

        //! this = this + mat * a
        inline_ Point& Mac(const Matrix3x3& mat, const Point& a);

        //! this = transpose(mat) * a
        inline_ Point& TransMult(const Matrix3x3& mat, const Point& a);

        //! Linear interpolate between two vectors: this = a + t * (b - a)
#if defined(__AVX__)
        inline_ Point& Lerp(const Point& a, const Point& b, float t)
        {
            float restmp[4];
            __m128 ma, mb, mt;

            ma = _mm_loadu_ps(a);
            mb = _mm_loadu_ps(b);
            mt = _mm_set1_ps(t);

            mb = _mm_sub_ps(mb, ma);
            mb = _mm_mul_ps(mb, mt);
            ma = _mm_add_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            x = restmp[0];
            y = restmp[1];
            z = restmp[2];
            return *this;
        }
#else
        inline_ Point& Lerp(const Point& a, const Point& b, float t)
        {
            x = a.x + t * (b.x - a.x);
            y = a.y + t * (b.y - a.y);
            z = a.z + t * (b.z - a.z);
            return *this;
        }
#endif
        //! Hermite interpolate between p1 and p2. p0 and p3 are used for finding gradient at p1 and p2.
        //! this =    p0 * (2t^2 - t^3 - t)/2
        //!            + p1 * (3t^3 - 5t^2 + 2)/2
        //!            + p2 * (4t^2 - 3t^3 + t)/2
        //!            + p3 * (t^3 - t^2)/2
        inline_ Point& Herp(const Point& p0, const Point& p1, const Point& p2, const Point& p3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;
            float kp0 = (2.0f * t2 - t3 - t) * 0.5f;
            float kp1 = (3.0f * t3 - 5.0f * t2 + 2.0f) * 0.5f;
            float kp2 = (4.0f * t2 - 3.0f * t3 + t) * 0.5f;
            float kp3 = (t3 - t2) * 0.5f;
            x = p0.x * kp0 + p1.x * kp1 + p2.x * kp2 + p3.x * kp3;
            y = p0.y * kp0 + p1.y * kp1 + p2.y * kp2 + p3.y * kp3;
            z = p0.z * kp0 + p1.z * kp1 + p2.z * kp2 + p3.z * kp3;
            return *this;
        }

        //! this = rotpos * r + linpos
        inline_ Point& Transform(const Point& r, const Matrix3x3& rotpos, const Point& linpos);

        //! this = trans(rotpos) * (r - linpos)
        inline_ Point& InvTransform(const Point& r, const Matrix3x3& rotpos, const Point& linpos);

        //! Returns MIN(x, y, z);
        inline_ float Min()                const        { return MIN(x, MIN(y, z));                                                }
        //! Returns MAX(x, y, z);
        inline_ float Max()                const        { return MAX(x, MAX(y, z));                                                }
        //! Sets each element to be componentwise minimum
        inline_ Point& Min(const Point& p)                { x = MIN(x, p.x); y = MIN(y, p.y); z = MIN(z, p.z);    return *this;    }
        //! Sets each element to be componentwise maximum
        inline_ Point& Max(const Point& p)                { x = MAX(x, p.x); y = MAX(y, p.y); z = MAX(z, p.z);    return *this;    }

        //! Clamps each element
        inline_ Point& Clamp(float min, float max)
        {
            if(x<min)    x=min;    if(x>max)    x=max;
            if(y<min)    y=min;    if(y>max)    y=max;
            if(z<min)    z=min;    if(z>max)    z=max;
            return *this;
        }

        //! Computes square magnitude
#if defined(__AVX__)
        inline_    float SquareMagnitude()    const
        {
            __m128 ma;
            ma = _mm_loadu_ps(&x);
            ma = _mm_dp_ps(ma, ma, 0x71);
            return _mm_cvtss_f32(ma);
        }
#else
        inline_ float SquareMagnitude()    const { return x*x + y*y + z*z;    }
#endif

        //! Computes magnitude
#if defined(__AVX__)
        inline_ float Magnitude()    const
        {
            __m128 ma;
            ma = _mm_loadu_ps(&x);
            ma = _mm_dp_ps(ma, ma, 0x71);
            return sqrtf(_mm_cvtss_f32(ma));
        }
#else
        inline_ float Magnitude() const    { return sqrtf(x*x + y*y + z*z); }
#endif

        //! Computes volume
        inline_ float Volume() const { return x * y * z; }

        //! Checks the point is near zero
        inline_ bool ApproxZero() const { return SquareMagnitude() < EPSILON2; }

        //! Tests for exact zero vector
        inline_ BOOL IsZero()const
        {
            if(IR(x) || IR(y) || IR(z))    
                return FALSE;
            return TRUE;
        }

        //! Checks point validity
        inline_ BOOL IsValid() const
        {
            if(!IsValidFloat(x))    return FALSE;
            if(!IsValidFloat(y))    return FALSE;
            if(!IsValidFloat(z))    return FALSE;
            return TRUE;
        }

        //! Slighty moves the point
        void Tweak(udword coord_mask, udword tweak_mask)
        {
            if(coord_mask&1)    { udword Dummy = IR(x);    Dummy^=tweak_mask;    x = FR(Dummy); }
            if(coord_mask&2)    { udword Dummy = IR(y);    Dummy^=tweak_mask;    y = FR(Dummy); }
            if(coord_mask&4)    { udword Dummy = IR(z);    Dummy^=tweak_mask;    z = FR(Dummy); }
        }

        #define TWEAKMASK        0x3fffff
        #define TWEAKNOTMASK    ~TWEAKMASK
        //! Slighty moves the point out
        inline_ void TweakBigger()
        {
            udword    Dummy;
            Dummy = (IR(x)&TWEAKNOTMASK);    if (!IS_NEGATIVE_FLOAT(x))    Dummy += TWEAKMASK + 1;    x = FR(Dummy);
            Dummy = (IR(y)&TWEAKNOTMASK);    if(!IS_NEGATIVE_FLOAT(y))    Dummy+=TWEAKMASK+1;    y = FR(Dummy);
            Dummy = (IR(z)&TWEAKNOTMASK);    if(!IS_NEGATIVE_FLOAT(z))    Dummy+=TWEAKMASK+1;    z = FR(Dummy);
        }

        //! Slighty moves the point in
        inline_ void TweakSmaller()
        {
            udword    Dummy;
            Dummy = (IR(x)&TWEAKNOTMASK);    if (IS_NEGATIVE_FLOAT(x))    Dummy += TWEAKMASK + 1;    x = FR(Dummy);
            Dummy = (IR(y)&TWEAKNOTMASK);    if(IS_NEGATIVE_FLOAT(y))    Dummy+=TWEAKMASK+1;    y = FR(Dummy);
            Dummy = (IR(z)&TWEAKNOTMASK);    if(IS_NEGATIVE_FLOAT(z))    Dummy+=TWEAKMASK+1;    z = FR(Dummy);
        }

        //! Normalizes the vector
#if defined(__AVX__)
        inline_ Point& Normalize()
        {
            __m128 ma, mc;
            float restmp[4];

            ma = _mm_loadu_ps(&x);
            mc = _mm_dp_ps(ma, ma, 0x71);
            float m = _mm_cvtss_f32(mc);
            if(m)
            {
                m = 1.0f / sqrtf(m);
                mc = _mm_set1_ps(m);
                ma = _mm_mul_ps(ma, mc);

                _mm_storeu_ps(restmp, ma);
                x = restmp[0];
                y = restmp[1];
                z = restmp[2];
            }
            return *this;
        }
#else
        inline_ Point& Normalize()
        {
            float M = SquareMagnitude();
            if(M)
            {
                M = 1.0f / sqrtf(M);
                x *= M;
                y *= M;
                z *= M;
            }
            return *this;
        }
#endif

        //! Sets vector length
        inline_ Point& SetLength(float length)
        {
            float mag = SquareMagnitude();
            if (mag > 1e-20f)
            {
                float NewLength = length / sqrtf(mag);
                x *= NewLength;
                y *= NewLength;
                z *= NewLength;
            }
            return *this;
        }

        //! Clamps vector length
        inline_ Point&  ClampLength(float limit_length)
        {
            if(limit_length>=0.0f)    // Magnitude must be positive
            {
                float CurrentSquareLength = SquareMagnitude();

                if(CurrentSquareLength > limit_length * limit_length)
                {
                    float Coeff = limit_length / sqrtf(CurrentSquareLength);
                    x *= Coeff;
                    y *= Coeff;
                    z *= Coeff;
                }
            }
            return *this;
        }

        //! Computes distance to another point
#if defined(__AVX__)
        inline_ float Distance(const Point& b) const
        {
            __m128 ma, mb;
            ma = _mm_loadu_ps(&x);
            mb = _mm_loadu_ps(&b.x);
            ma = _mm_sub_ps(ma, mb);
            ma = _mm_dp_ps(ma, ma, 0x71);
            return sqrtf(_mm_cvtss_f32(ma));
        }
#else
        inline_ float Distance(const Point& b) const
        {
            return sqrtf((x - b.x)*(x - b.x) + (y - b.y)*(y - b.y) + (z - b.z)*(z - b.z));
        }
#endif

        //! Computes square distance to another point
#if defined(__AVX__)
        inline_ float SquareDistance(const Point& b) const
        {
            __m128 ma, mb;
            ma = _mm_loadu_ps(&x);
            mb = _mm_loadu_ps(&b.x);
            ma = _mm_sub_ps(ma, mb);
            ma = _mm_dp_ps(ma, ma, 0x71);
            return _mm_cvtss_f32(ma);
        }
#else
        inline_ float SquareDistance(const Point& b) const
        {
            return ((x - b.x)*(x - b.x) + (y - b.y)*(y - b.y) + (z - b.z)*(z - b.z));
        }
#endif

        //! Dot product dp = this|a
#if defined(__AVX__)
        inline_    float Dot(const Point& p) const
        {
            __m128 ma, mb;
            ma = _mm_loadu_ps(&x);
            mb = _mm_loadu_ps(&p.x);
            ma = _mm_dp_ps(ma, mb, 0x71);

            return _mm_cvtss_f32(ma);
        }
#else
        inline_ float Dot(const Point& p) const    { return p.x * x + p.y * y + p.z * z; }
#endif

        //! Cross product this = a x b
#if defined(__AVX__)
        inline_ Point& Cross(const Point& a, const Point& b)
        {
            __m128 ma, mb, t1, t2, t3, t4;
            ma = _mm_loadu_ps(a);
            mb = _mm_loadu_ps(b);
            float restmp[4];

            t1 = _mm_shuffle_ps(ma, ma, _MM_SHUFFLE(3, 0, 2, 1));
            t2 = _mm_shuffle_ps(mb, mb, _MM_SHUFFLE(3, 1, 0, 2));

            t3 = _mm_mul_ps(t1, t2);

            t1 = _mm_shuffle_ps(t1, t1, _MM_SHUFFLE(3, 0, 2, 1));
            t2 = _mm_shuffle_ps(t2, t2, _MM_SHUFFLE(3, 1, 0, 2));
            t4 = _mm_mul_ps(t1, t2);

            ma = _mm_sub_ps(t3, t4);
            _mm_storeu_ps(restmp, ma);
            x = restmp[0];
            y = restmp[1];
            x = restmp[2];
        }
#else
        inline_ Point& Cross(const Point& a, const Point& b)
        {
            x = a.y * b.z - a.z * b.y;
            y = a.z * b.x - a.x * b.z;
            z = a.x * b.y - a.y * b.x;
            return *this;
        }
#endif
        //! Vector code ( bitmask = sign(z) | sign(y) | sign(x) )
        inline_ udword VectorCode() const
        {
            return (IR(x)>>31) | ((IR(y)&SIGN_BITMASK)>>30) | ((IR(z)&SIGN_BITMASK)>>29);
        }

        //! Returns largest axis
        inline_ PointComponent LargestAxis() const
        {
            const float* Vals = &x;
            PointComponent m = X;
            if(Vals[Y] > Vals[m]) m = Y;
            if(Vals[Z] > Vals[m]) m = Z;
            return m;
        }

        //! Returns closest axis
        inline_ PointComponent ClosestAxis() const
        {
            const float* Vals = &x;
            PointComponent m = X;
            if(AIR(Vals[Y]) > AIR(Vals[m])) m = Y;
            if(AIR(Vals[Z]) > AIR(Vals[m])) m = Z;
            return m;
        }

        //! Returns smallest axis
        inline_ PointComponent SmallestAxis() const
        {
            const float* Vals = &x;
            PointComponent m = X;
            if(Vals[Y] < Vals[m]) m = Y;
            if(Vals[Z] < Vals[m]) m = Z;
            return m;
        }

        //! Refracts the point
        Point& Refract(const Point& eye, const Point& n, float refractindex, Point& refracted);

        //! Projects the point onto a plane
        Point& ProjectToPlane(const Plane& p);

        //! Projects the point onto the screen
        void ProjectToScreen(float halfrenderwidth, float halfrenderheight, const Matrix4x4& mat, HPoint& projected) const;

        //! Unfolds the point onto a plane according to edge(a,b)
        Point& Unfold(Plane& p, Point& a, Point& b);

        //! Hash function from Ville Miettinen
        inline_ udword GetHashValue() const
        {
            const udword* h = (const udword*)(this);
            udword f = (h[0]+h[1]*11-(h[2]*17)) & 0x7fffffff;    // avoid problems with +-0
            return (f>>22)^(f>>12)^(f);
        }

        //! Stuff magic values in the point, marking it as explicitely not used.
       void SetNotUsed();
        //! Checks the point is marked as not used
       BOOL IsNotUsed() const;

        // Arithmetic operators

        //! Unary operator for Point Negate = - Point
        inline_ Point operator-() const { return Point(-x, -y, -z); }

        //! Operator for Point Plus = Point + Point.
#if defined(__AVX__)
        inline_ Point operator+(const Point& p) const
        {
            float restmp[4];
            __m128 ma, mb;

            ma = _mm_loadu_ps(&x);
            mb = _mm_loadu_ps(p);
            ma = _mm_add_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            return Point(restmp[0], restmp[1], restmp[2]);
        }
#else
        inline_ Point operator+(const Point& p) const { return Point(x + p.x, y + p.y, z + p.z); }
#endif
        //! Operator for Point Minus = Point - Point.
#if defined(__AVX__)
        inline_ Point operator-(const Point& p) const
        {
            float restmp[4];
            __m128 ma, mb;

            ma = _mm_loadu_ps(&x);
            mb = _mm_loadu_ps(p);
            ma = _mm_sub_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            return Point(restmp[0], restmp[1], restmp[2]);
        }
#else
        inline_ Point operator-(const Point& p) const { return Point(x - p.x, y - p.y, z - p.z); }
#endif
        //! Operator for Point Mul   = Point * Point.
#if defined(__AVX__)
        inline_ Point operator*(const Point& p) const
        {
            float restmp[4];
            __m128 ma, mb;

            ma = _mm_loadu_ps(&x);
            mb = _mm_loadu_ps(p);
            ma = _mm_mul_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            return Point(restmp[0], restmp[1], restmp[2]);
        }
#else
        inline_ Point operator*(const Point& p) const { return Point(x * p.x, y * p.y, z * p.z); }
#endif
        //! Operator for Point Scale = Point * float.
#if defined(__AVX__)
        inline_ Point operator*(float s) const
        {
            float restmp[4];
            __m128 ma, mb;

            ma = _mm_loadu_ps(&x);
            mb = _mm_set1_ps(s);
            ma = _mm_mul_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            return Point(restmp[0], restmp[1], restmp[2]);
        }
#else
        inline_ Point operator*(float s) const { return Point(x * s,   y * s,   z * s ); }
#endif
        //! Operator for Point Scale = float * Point.
#if defined(__AVX__)
        inline_ friend Point operator*(float s, const Point& p)
        {
            float restmp[4];
            __m128 ma, mb;

            ma = _mm_loadu_ps(p);
            mb = _mm_set1_ps(s);
            ma = _mm_mul_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            return Point(restmp[0], restmp[1], restmp[2]);
        }
#else
        inline_ friend Point operator*(float s, const Point& p) { return Point(s * p.x, s * p.y, s * p.z); }
#endif
        //! Operator for Point Div   = Point / Point.
#if defined(__AVX__)
        inline_ Point operator/(const Point& p) const
        {
            float restmp[4];
            __m128 ma, mb;

            ma = _mm_loadu_ps(&x);
            mb = _mm_loadu_ps(p);
            ma = _mm_div_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            return Point(restmp[0], restmp[1], restmp[2]);
        }
#else
        inline_ Point operator/(const Point& p) const { return Point(x / p.x, y / p.y, z / p.z); }
#endif
        //! Operator for Point Scale = Point / float.
#if defined(__AVX__)
        inline_ Point operator/(float s) const
        {
            float restmp[4];
            __m128 ma, mb;

            s = 1.0f / s;
            ma = _mm_loadu_ps(&x);
            mb = _mm_set1_ps(s);
            ma = _mm_mul_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            return Point(restmp[0], restmp[1], restmp[2]);
        }
#else
        inline_ Point operator/(float s) const { s = 1.0f / s; return Point(x * s, y * s, z * s); }
#endif
        //! Operator for Point Scale = float / Point.
#if defined(__AVX__)
        inline_ friend Point operator/(float s, const Point& p)
        {
            float restmp[4];
            __m128 ma, mb;

            ma = _mm_loadu_ps(p);
            mb = _mm_set1_ps(s);
            ma = _mm_div_ps(mb, ma);

            _mm_storeu_ps(restmp, ma);
            return Point(restmp[0], restmp[1], restmp[2]);
        }
#else
        inline_ friend Point operator/(float s, const Point& p) { return Point(s / p.x, s / p.y, s / p.z); }
#endif
        //! Operator for float DotProd = Point | Point.
#if defined(__AVX__)
        inline_ float operator|(const Point& p) const
        {
            __m128 ma, mb;
            ma = _mm_loadu_ps(&x);
            mb = _mm_loadu_ps(&p.x);
            ma = _mm_dp_ps(ma, mb, 0x71);

            return _mm_cvtss_f32(ma);
        }
#else
        inline_ float operator|(const Point& p)    const { return x*p.x + y*p.y + z*p.z; }
#endif

        //! Operator for Point VecProd = Point ^ Point.
#if defined(__AVX__)
        inline_ Point operator^(const Point& p) const
        {
            __m128 ma, mb, t1, t2, t3, t4;
            ma = _mm_loadu_ps(&x);
            mb = _mm_loadu_ps(&p.x);
            float restmp[4];

            t1 = _mm_shuffle_ps(ma, ma, _MM_SHUFFLE(3, 0, 2, 1));
            t2 = _mm_shuffle_ps(mb, mb, _MM_SHUFFLE(3, 1, 0, 2));
            t3 = _mm_mul_ps(t1, t2);

            t1 = _mm_shuffle_ps(t1, t1, _MM_SHUFFLE(3, 0, 2, 1));
            t2 = _mm_shuffle_ps(t2, t2, _MM_SHUFFLE(3, 1, 0, 2));
            t4 = _mm_mul_ps(t1, t2);

            ma = _mm_sub_ps(t3, t4);
            _mm_storeu_ps(restmp, ma);
            return Point(restmp[0], restmp[1], restmp[2]);
        }
#else
        inline_ Point operator^(const Point& p)    const
        {
            return Point(
                y * p.z - z * p.y,
                z * p.x - x * p.z,
                x * p.y - y * p.x );
        }
#endif

        //! Operator for Point += Point.
#if defined(__AVX__)
        inline_ Point& operator+=(const Point& p)
        {
            float restmp[4];
            __m128 ma, mb;

            ma = _mm_loadu_ps(&x);
            mb = _mm_loadu_ps(p);
            ma = _mm_add_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            x = restmp[0];
            y = restmp[1];
            z = restmp[2];
            return *this;
        }
#else
        inline_ Point& operator+=(const Point& p) { x += p.x; y += p.y; z += p.z; return *this; }
#endif
        //! Operator for Point += float.
#if defined(__AVX__)
        inline_ Point& operator+=(float s)
        {
            float restmp[4];
            __m128 ma, mb;

            ma = _mm_loadu_ps(&x);
            mb = _mm_set1_ps(s);
            ma = _mm_add_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            x = restmp[0];
            y = restmp[1];
            z = restmp[2];
            return *this;
        }
#else
        inline_ Point& operator+=(float s) { x += s; y += s; z += s; return *this; }
#endif
        //! Operator for Point -= Point.
#if defined(__AVX__)
        inline_ Point& operator-=(const Point& p)
        {
            float restmp[4];
            __m128 ma, mb;

            ma = _mm_loadu_ps(&x);
            mb = _mm_loadu_ps(p);
            ma = _mm_sub_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            x = restmp[0];
            y = restmp[1];
            z = restmp[2];
            return *this;
        }
#else
        inline_ Point& operator-=(const Point& p) { x -= p.x; y -= p.y; z -= p.z; return *this; }
#endif
        //! Operator for Point -= float.
        inline_ Point& operator-=(float s) { x -= s; y -= s; z -= s; return *this; }

        //! Operator for Point *= Point.
#if defined(__AVX__)
        inline_ Point& operator*=(const Point& p)
        {
            float restmp[4];
            __m128 ma, mb;

            ma = _mm_loadu_ps(&x);
            mb = _mm_loadu_ps(p);
            ma = _mm_mul_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            x = restmp[0];
            y = restmp[1];
            z = restmp[2];
            return *this;
        }
#else
        inline_ Point& operator*=(const Point& p)  { x *= p.x; y *= p.y; z *= p.z; return *this; }
#endif
        //! Operator for Point *= float.
        inline_ Point& operator*=(float s) { x *= s; y *= s; z *= s; return *this; }

        //! Operator for Point /= Point.
#if defined(__AVX__)
        inline_ Point& operator/=(const Point& p)
        {
            float restmp[4];
            __m128 ma, mb;

            ma = _mm_loadu_ps(&x);
            mb = _mm_loadu_ps(p);
            ma = _mm_div_ps(ma, mb);

            _mm_storeu_ps(restmp, ma);
            x = restmp[0];
            y = restmp[1];
            z = restmp[2];
            return *this;
        }
#else
        inline_ Point& operator/=(const Point& p)                        { x /= p.x; y /= p.y; z /= p.z;    return *this;        }
#endif
        //! Operator for Point /= float.
        inline_ Point& operator/=(float s)                                { s = 1.0f/s; x *= s; y *= s; z *= s; return *this; }

        // Logical operators

        //! Operator for "if(Point==Point)"
        inline_ bool operator==(const Point& p)            const        { return ( (IR(x)==IR(p.x))&&(IR(y)==IR(p.y))&&(IR(z)==IR(p.z)));    }
        //! Operator for "if(Point!=Point)"
        inline_ bool operator!=(const Point& p)            const        { return ( (IR(x)!=IR(p.x))||(IR(y)!=IR(p.y))||(IR(z)!=IR(p.z)));    }

        // Arithmetic operators

        //! Operator for Point Mul = Point * Matrix3x3.
#if defined(__AVX__)
        inline_ Point operator*(const Matrix3x3& mat) const
        {
            __m128 ma, t0, t1, t2, m0, m1, m2, m3;
            float xx, yy, zz;

            class ShadowMatrix3x3 { public: float m[3][3]; };    // To allow inlining
            const ShadowMatrix3x3* Mat = (const ShadowMatrix3x3*)&mat;

            t0 = _mm_loadu_ps(Mat->m[0]);
            t1 = _mm_loadu_ps(Mat->m[1]);
            t2 = _mm_loadu_ps(Mat->m[2]);

            m0 = _mm_shuffle_ps(t0, t1, _MM_SHUFFLE(1, 0, 1, 0)); // x0 y0 x1 y1
            m2 = _mm_shuffle_ps(t0, t1, _MM_SHUFFLE(3, 2, 3, 2)); // z0 w0 z1 w1
            m1 = _mm_shuffle_ps(t1, t2, _MM_SHUFFLE(1, 0, 1, 0)); // x1 y1 x2 y2
            m3 = _mm_shuffle_ps(t1, t2, _MM_SHUFFLE(3, 2, 3, 2)); // z1 w1 z2 w2

            t0 = _mm_shuffle_ps(m0, m1, _MM_SHUFFLE(2, 2, 2, 0)); //x0 x1 x2 x2
            t1 = _mm_shuffle_ps(m0, m1, _MM_SHUFFLE(3, 3, 3, 1)); //y0 y1 y2 y2
            t2 = _mm_shuffle_ps(m2, m3, _MM_SHUFFLE(2, 2, 2, 0)); //z0 z1 z2 z2

            ma = _mm_loadu_ps(&x);

            m0 = _mm_dp_ps(ma, t0, 0x71);
            xx = _mm_cvtss_f32(m0);
            m1 = _mm_dp_ps(ma, t1, 0x71);
            yy = _mm_cvtss_f32(m1);
            m2 = _mm_dp_ps(ma, t2, 0x71);
            zz = _mm_cvtss_f32(m2);
            return Point(xx, yy, zz);
        }
#else
        inline_ Point operator*(const Matrix3x3& mat) const
        {
            class ShadowMatrix3x3 { public: float m[3][3]; };    // To allow inlining
            const ShadowMatrix3x3* Mat = (const ShadowMatrix3x3*)&mat;

            return Point(
                x * Mat->m[0][0] + y * Mat->m[1][0] + z * Mat->m[2][0],
                x * Mat->m[0][1] + y * Mat->m[1][1] + z * Mat->m[2][1],
                x * Mat->m[0][2] + y * Mat->m[1][2] + z * Mat->m[2][2] );
        }
#endif

        //! Operator for Point Mul = Point * Matrix4x4.

#if defined(__AVX__)
        inline_ Point operator*(const Matrix4x4& mat) const
        {
            __m128 ma, t0, t1, t2, m0, m1, m2, m3;
            float xx, yy, zz;

            class ShadowMatrix4x4 { public: float m[4][4]; };    // To allow inlining
            const ShadowMatrix4x4* Mat = (const ShadowMatrix4x4*)&mat;

            t0 = _mm_loadu_ps(Mat->m[0]);
            t1 = _mm_loadu_ps(Mat->m[1]);
            t2 = _mm_loadu_ps(Mat->m[2]);

            m0 = _mm_shuffle_ps(t0, t1, _MM_SHUFFLE(1, 0, 1, 0)); // x0 y0 x1 y1
            m2 = _mm_shuffle_ps(t0, t1, _MM_SHUFFLE(3, 2, 3, 2)); // z0 w0 z1 w1
            m1 = _mm_shuffle_ps(t1, t2, _MM_SHUFFLE(1, 0, 1, 0)); // x1 y1 x2 y2
            m3 = _mm_shuffle_ps(t1, t2, _MM_SHUFFLE(3, 2, 3, 2)); // z1 w1 z2 w2

            t0 = _mm_shuffle_ps(m0, m1, _MM_SHUFFLE(2, 2, 2, 0)); //x0 x1 x2 x2
            t1 = _mm_shuffle_ps(m0, m1, _MM_SHUFFLE(3, 3, 3, 1)); //y0 y1 y2 y2
            t2 = _mm_shuffle_ps(m2, m3, _MM_SHUFFLE(2, 2, 2, 0)); //z0 z1 z2 z2

            ma = _mm_loadu_ps(&x);

            m0 = _mm_dp_ps(ma, t0, 0x71);
            xx = _mm_cvtss_f32(m0) + Mat->m[3][0];
            m1 = _mm_dp_ps(ma, t1, 0x71);
            yy = _mm_cvtss_f32(m1) + Mat->m[3][1];
            m2 = _mm_dp_ps(ma, t2, 0x71);
            zz = _mm_cvtss_f32(m2) + Mat->m[3][2];
            return Point(xx, yy, zz);
        }
#else
        inline_ Point operator*(const Matrix4x4& mat) const
        {
            class ShadowMatrix4x4 { public: float m[4][4]; };    // To allow inlining
            const ShadowMatrix4x4* Mat = (const ShadowMatrix4x4*)&mat;

            return Point(
                x * Mat->m[0][0] + y * Mat->m[1][0] + z * Mat->m[2][0] + Mat->m[3][0],
                x * Mat->m[0][1] + y * Mat->m[1][1] + z * Mat->m[2][1] + Mat->m[3][1],
                x * Mat->m[0][2] + y * Mat->m[1][2] + z * Mat->m[2][2] + Mat->m[3][2]);
        }
#endif

        //! Operator for Point *= Matrix3x3.

#if defined(__AVX__)
        inline_ Point& operator*=(const Matrix3x3& mat)
        {
            __m128 ma, t0, t1, t2, m0, m1, m2, m3;
            float xx, yy, zz;

            class ShadowMatrix3x3 { public: float m[3][3]; };    // To allow inlining
            const ShadowMatrix3x3* Mat = (const ShadowMatrix3x3*)&mat;

            t0 = _mm_loadu_ps(Mat->m[0]);
            t1 = _mm_loadu_ps(Mat->m[1]);
            t2 = _mm_loadu_ps(Mat->m[2]);

            m0 = _mm_shuffle_ps(t0, t1, _MM_SHUFFLE(1, 0, 1, 0)); // x0 y0 x1 y1
            m2 = _mm_shuffle_ps(t0, t1, _MM_SHUFFLE(3, 2, 3, 2)); // z0 w0 z1 w1
            m1 = _mm_shuffle_ps(t1, t2, _MM_SHUFFLE(1, 0, 1, 0)); // x1 y1 x2 y2
            m3 = _mm_shuffle_ps(t1, t2, _MM_SHUFFLE(3, 2, 3, 2)); // z1 w1 z2 w2

            t0 = _mm_shuffle_ps(m0, m1, _MM_SHUFFLE(2, 2, 2, 0)); //x0 x1 x2 x2
            t1 = _mm_shuffle_ps(m0, m1, _MM_SHUFFLE(3, 3, 3, 1)); //y0 y1 y2 y2
            t2 = _mm_shuffle_ps(m2, m3, _MM_SHUFFLE(2, 2, 2, 0)); //z0 z1 z2 z2

            ma = _mm_loadu_ps(&x);
            m0 = _mm_dp_ps(ma, t0, 0x71);
            xx = _mm_cvtss_f32(m0);
            m1 = _mm_dp_ps(ma, t1, 0x71);
            yy = _mm_cvtss_f32(m1);
            m2 = _mm_dp_ps(ma, t2, 0x71);
            zz = _mm_cvtss_f32(m2);
            x = xx;    y = yy;    z = zz;
            return *this;
        }
#else
        inline_ Point& operator*=(const Matrix3x3& mat)
        {
            class ShadowMatrix3x3 { public: float m[3][3]; };    // To allow inlining
            const ShadowMatrix3x3* Mat = (const ShadowMatrix3x3*)&mat;

            float xp = x * Mat->m[0][0] + y * Mat->m[1][0] + z * Mat->m[2][0];
            float yp = x * Mat->m[0][1] + y * Mat->m[1][1] + z * Mat->m[2][1];
            float zp = x * Mat->m[0][2] + y * Mat->m[1][2] + z * Mat->m[2][2];

            x = xp;    y = yp;    z = zp;

            return *this;
        }
#endif
        //! Operator for Point *= Matrix4x4.

#if defined(__AVX__)
        inline_ Point& operator*=(const Matrix4x4& mat)
        {
            __m128 ma, t0, t1, t2, m0, m1, m2, m3;
            float xx, yy, zz;

            class ShadowMatrix4x4 { public: float m[4][4]; };    // To allow inlining
            const ShadowMatrix4x4* Mat = (const ShadowMatrix4x4*)&mat;
            t0 = _mm_loadu_ps(Mat->m[0]);
            t1 = _mm_loadu_ps(Mat->m[1]);
            t2 = _mm_loadu_ps(Mat->m[2]);

            m0 = _mm_shuffle_ps(t0, t1, _MM_SHUFFLE(1, 0, 1, 0)); // x0 y0 x1 y1
            m2 = _mm_shuffle_ps(t0, t1, _MM_SHUFFLE(3, 2, 3, 2)); // z0 w0 z1 w1
            m1 = _mm_shuffle_ps(t1, t2, _MM_SHUFFLE(1, 0, 1, 0)); // x1 y1 x2 y2
            m3 = _mm_shuffle_ps(t1, t2, _MM_SHUFFLE(3, 2, 3, 2)); // z1 w1 z2 w2

            t0 = _mm_shuffle_ps(m0, m1, _MM_SHUFFLE(2, 2, 2, 0)); //x0 x1 x2 x2
            t1 = _mm_shuffle_ps(m0, m1, _MM_SHUFFLE(3, 3, 3, 1)); //y0 y1 y2 y2
            t2 = _mm_shuffle_ps(m2, m3, _MM_SHUFFLE(2, 2, 2, 0)); //z0 z1 z2 z2

            ma = _mm_loadu_ps(&x);

            m0 = _mm_dp_ps(ma, t0, 0x71);
            xx = _mm_cvtss_f32(m0) + Mat->m[3][0];
            m1 = _mm_dp_ps(ma, t1, 0x71);
            yy = _mm_cvtss_f32(m1) + Mat->m[3][1];
            m2 = _mm_dp_ps(ma, t2, 0x71);
            zz = _mm_cvtss_f32(m2) + Mat->m[3][2];
            x = xx;    y = yy;    z = zz;
            return *this;
        }
#else
        inline_ Point& operator*=(const Matrix4x4& mat)
        {
            class ShadowMatrix4x4{ public: float m[4][4]; };    // To allow inlining
            const ShadowMatrix4x4* Mat = (const ShadowMatrix4x4*)&mat;

            float xp = x * Mat->m[0][0] + y * Mat->m[1][0] + z * Mat->m[2][0] + Mat->m[3][0];
            float yp = x * Mat->m[0][1] + y * Mat->m[1][1] + z * Mat->m[2][1] + Mat->m[3][1];
            float zp = x * Mat->m[0][2] + y * Mat->m[1][2] + z * Mat->m[2][2] + Mat->m[3][2];

            x = xp;    y = yp;    z = zp;

            return *this;
        }
#endif
        // Cast operators

        //! Cast a Point to a HPoint. w is set to zero.
        operator HPoint() const;
        inline_ operator const    float*() const    { return &x; }
        inline_ operator float*()    { return &x; }

        public:
           float x, y, z;
    };

    FUNCTION ICEMATHS_API void Normalize1(Point& a);
    FUNCTION ICEMATHS_API void Normalize2(Point& a);

#endif //__ICEPOINT_H__
