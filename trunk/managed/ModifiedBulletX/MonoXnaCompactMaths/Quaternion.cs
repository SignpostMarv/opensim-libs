#region License
/*
MIT License
Copyright © 2006 The Mono.Xna Team

All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion License

using System;
using System.ComponentModel;

namespace MonoXnaCompactMaths
{
    [Serializable]
    //[TypeConverter(typeof(QuaternionConverter))]
    public struct Quaternion : IEquatable<Quaternion>
    {
        public float X;
        public float Y;
        public float Z;
        public float W;
        static Quaternion identity = new Quaternion(0, 0, 0, 1);

        
        public Quaternion(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }
        
        
        public Quaternion(Vector3 vectorPart, float scalarPart)
        {
            this.X = vectorPart.X;
            this.Y = vectorPart.Y;
            this.Z = vectorPart.Z;
            this.W = scalarPart;
        }

        public static Quaternion Identity
        {
            get{ return identity; }
        }


        public static Quaternion Add(Quaternion quaternion1, Quaternion quaternion2)
        {
            throw new NotImplementedException();
        }


        public static void Add(ref Quaternion quaternion1, ref Quaternion quaternion2, out Quaternion result)
        {
            throw new NotImplementedException();
        }


        public static Quaternion CreateFromAxisAngle(Vector3 axis, float angle)
        {
            throw new NotImplementedException();
        }


        public static void CreateFromAxisAngle(ref Vector3 axis, float angle, out Quaternion result)
        {
            throw new NotImplementedException();
        }


        public static Quaternion CreateFromRotationMatrix(Matrix matrix)
        {
            /* Q = (x,y,z,w)*/
            Quaternion q = new Quaternion();
            /*      | 1 - (2y^2 + 2z^2)         2xy + 2zw           2xz - 2yw           0   |
             * M =  |   2xy - 2zw           1 - (2x^2 + 2z^2)       2yz + 2xw           0   |
             *      |   2xz + 2yw               2yz - 2xw       1 - (2x^2 + 2y^2)       0   |
             *      |       0                       0                     0             1   |
             * 
             * w^2 + x^2 + y^2 + z^2 = 1
             * 
             * +/- 1/2*Sqrt(M11 + M22 + M33 + 1) = +/- 1/2*Sqrt(4 - 4x^2 - 4y^2 + 4z^2) =
             * = +/- Sqrt(1 - x^2 - y^2 - z^2) = w */
            q.W = 0.5f * (float)Math.Sqrt(matrix.M11 + matrix.M22 + matrix.M33 + 1);
            ///* case w!=0 
            // *      M12 - M21 = +/- 4z||w||     z = (M21 - M12)/4w
            // *      M31 - M13 = +/- 4y||w||     y = (M13 - M31)/4w
            // *      M23 - M32 = +/- 4x||w||     x = (M32 - M23)/4w */
            if (q.W != 0)
            {
                q.X = (matrix.M32 - matrix.M23) / (4 * q.W);
                q.Y = (matrix.M13 - matrix.M31) / (4 * q.W);
                q.Z = (matrix.M21 - matrix.M12) / (4 * q.W);
            }
            else
            {
                float alpha2 = matrix.M12 * matrix.M12 * matrix.M13 * matrix.M13;
                float beta2 = matrix.M12 * matrix.M12 * matrix.M23 * matrix.M23;
                float gamma2 = matrix.M13 * matrix.M13 * matrix.M23 * matrix.M23;
                float omega = (float)Math.Sqrt(alpha2 + beta2 + gamma2);
                q.X = matrix.M13 * matrix.M12 / omega;
                q.Y = matrix.M12 * matrix.M23 / omega;
                q.Z = matrix.M13 * matrix.M23 / omega;
            }
            return q;
        }

        public static void CreateFromRotationMatrix(ref Matrix matrix, out Quaternion result)
        {
            throw new NotImplementedException();
        }


        public static Quaternion Divide(Quaternion quaternion1, Quaternion quaternion2)
        {
            throw new NotImplementedException();
        }


        public static void Divide(ref Quaternion quaternion1, ref Quaternion quaternion2, out Quaternion result)
        {
            throw new NotImplementedException();
        }


        public static float Dot(Quaternion quaternion1, Quaternion quaternion2)
        {
            throw new NotImplementedException();
        }


        public static void Dot(ref Quaternion quaternion1, ref Quaternion quaternion2, out float result)
        {
            throw new NotImplementedException();
        }


        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }


        public bool Equals(Quaternion other)
        {
            throw new NotImplementedException();
        }


        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }


        public static Quaternion Inverse(Quaternion quaternion)
        {
            throw new NotImplementedException();
        }


        public static void Inverse(ref Quaternion quaternion, out Quaternion result)
        {
            throw new NotImplementedException();
        }


        public float Length()
        {
            //---
            return (float)Math.Sqrt(Math.Pow(this.W, 2.0) + Math.Pow(this.X, 2.0) + Math.Pow(this.Y, 2.0) + Math.Pow(this.Z, 2.0));
            //---
            //throw new NotImplementedException();
        }


        public float LengthSquared()
        {
            //---
            return (float)(Math.Pow(this.W, 2.0) + Math.Pow(this.X, 2.0) + Math.Pow(this.Y, 2.0) + Math.Pow(this.Z, 2.0));
            //---
            //throw new NotImplementedException();
        }


        public static Quaternion Lerp(Quaternion quaternion1, Quaternion quaternion2, float amount)
        {
            throw new NotImplementedException();
        }


        public static void Lerp(ref Quaternion quaternion1, ref Quaternion quaternion2, float amount, out Quaternion result)
        {
            throw new NotImplementedException();
        }


        public static Quaternion Slerp(Quaternion quaternion1, Quaternion quaternion2, float amount)
        {
            throw new NotImplementedException();
        }


        public static void Slerp(ref Quaternion quaternion1, ref Quaternion quaternion2, float amount, out Quaternion result)
        {
            throw new NotImplementedException();
        }


        public static Quaternion Subtract(Quaternion quaternion1, Quaternion quaternion2)
        {
            throw new NotImplementedException();
        }


        public static void Subtract(ref Quaternion quaternion1, ref Quaternion quaternion2, out Quaternion result)
        {
            throw new NotImplementedException();
        }


        public static Quaternion Multiply(Quaternion quaternion1, Quaternion quaternion2)
        {
            throw new NotImplementedException();
        }


        public static Quaternion Multiply(Quaternion quaternion1, float scaleFactor)
        {
            throw new NotImplementedException();
        }


        public static void Multiply(ref Quaternion quaternion1, float scaleFactor, out Quaternion result)
        {
            throw new NotImplementedException();
        }


        public static void Multiply(ref Quaternion quaternion1, ref Quaternion quaternion2, out Quaternion result)
        {
            throw new NotImplementedException();
        }


        public static Quaternion Negate(Quaternion quaternion)
        {
            throw new NotImplementedException();
        }


        public static void Negate(ref Quaternion quaternion, out Quaternion result)
        {
            throw new NotImplementedException();
        }


        public void Normalize()
        {
            //---
            this = Normalize(this);
            //---
            //throw new NotImplementedException();
        }


        public static Quaternion Normalize(Quaternion quaternion)
        {
            //---
            return quaternion / quaternion.Length();
            //---
            //throw new NotImplementedException();
        }


        public static void Normalize(ref Quaternion quaternion, out Quaternion result)
        {
            throw new NotImplementedException();
        }


        public static Quaternion operator +(Quaternion quaternion1, Quaternion quaternion2)
        {
            throw new NotImplementedException();
        }


        public static Quaternion operator /(Quaternion quaternion1, Quaternion quaternion2)
        {
            throw new NotImplementedException();
        }
        public static Quaternion operator /(Quaternion quaternion, float factor)
        {
            quaternion.W /= factor;
            quaternion.X /= factor;
            quaternion.Y /= factor;
            quaternion.Z /= factor;
            return quaternion;
        }

        public static bool operator ==(Quaternion quaternion1, Quaternion quaternion2)
        {
            throw new NotImplementedException();
        }


        public static bool operator !=(Quaternion quaternion1, Quaternion quaternion2)
        {
            throw new NotImplementedException();
        }


        public static Quaternion operator *(Quaternion quaternion1, Quaternion quaternion2)
        {
            //---
            //Grassmann product
            Quaternion quaternionProduct = new Quaternion();

            quaternionProduct.W = quaternion1.W * quaternion2.W - quaternion1.X * quaternion2.X - quaternion1.Y * quaternion2.Y - quaternion1.Z * quaternion2.Z;
            quaternionProduct.X = quaternion1.W * quaternion2.X + quaternion1.X * quaternion2.W + quaternion1.Y * quaternion2.Z - quaternion1.Z * quaternion2.Y;
            quaternionProduct.Y = quaternion1.W * quaternion2.Y - quaternion1.X * quaternion2.Z + quaternion1.Y * quaternion2.W + quaternion1.Z * quaternion2.X;
            quaternionProduct.Z = quaternion1.W * quaternion2.Z + quaternion1.X * quaternion2.Y - quaternion1.Y * quaternion2.X + quaternion1.Z * quaternion2.W;
            return quaternionProduct;
            //---
            //throw new NotImplementedException();
        }


        public static Quaternion operator *(Quaternion quaternion1, float scaleFactor)
        {
            return new Quaternion(quaternion1.X / scaleFactor, quaternion1.Y / scaleFactor,
                quaternion1.Z / scaleFactor, quaternion1.W / scaleFactor);
        }


        public static Quaternion operator -(Quaternion quaternion1, Quaternion quaternion2)
        {
            throw new NotImplementedException();
        }


        public static Quaternion operator -(Quaternion quaternion)
        {
            throw new NotImplementedException();
        }


        public override string ToString()
        {
            return "(" + this.X + ", " + this.Y + ", " + this.Z + ", " + this.W + ")";
        }

        private static void Conjugate(ref Quaternion quaternion, out Quaternion result)
        {
            throw new NotImplementedException();
        }
    }
}
