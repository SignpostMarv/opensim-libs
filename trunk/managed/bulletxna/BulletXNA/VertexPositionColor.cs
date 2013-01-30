using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using BulletXNA.LinearMath;

namespace BulletXNA
{
    public class VertexPositionColor
    {
        public Color color;
        public IndexedVector3 vec;

        public float X { get { return vec.X; } }
        public float Y { get { return vec.Y; } }
        public float Z { get { return vec.Z; } }

       public VertexPositionColor (IndexedVector3 pVec, Color pCol)
       {
           color = pCol;
           vec = pVec;
       }
    }
}
