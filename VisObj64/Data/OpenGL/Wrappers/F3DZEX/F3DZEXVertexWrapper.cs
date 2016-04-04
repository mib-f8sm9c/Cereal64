using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Microcodes.F3DZEX.DataElements;

namespace Cereal64.VisObj64.Data.OpenGL.Wrappers.F3DZEX
{
    public class F3DZEXVertexWrapper : IVO64Vertex
    {
        //NOTE: THIS WILL ALL BREAK IF VERTEX IS NULL

        public Vertex Vertex { get; set; }

        public float X { get { return Vertex.X / 100; } set { Vertex.X = (short)Math.Round(value); _updated = true; } }
        public float Y { get { return Vertex.Y / 100; } set { Vertex.Y = (short)Math.Round(value); _updated = true; } }
        public float Z { get { return Vertex.Z / 100; } set { Vertex.Z = (short)Math.Round(value); _updated = true; } }

        public float U { get { return Vertex.S; } set { Vertex.S = (short)Math.Round(value); _updated = true; } }
        public float V { get { return Vertex.T; } set { Vertex.T = (short)Math.Round(value); _updated = true; } }

        public float NX { get { return Vertex.R; } set { Vertex.R = (sbyte)Math.Round(value); _updated = true; } }
        public float NY { get { return Vertex.G; } set { Vertex.G = (sbyte)Math.Round(value); _updated = true; } }
        public float NZ { get { return Vertex.B; } set { Vertex.B = (sbyte)Math.Round(value); _updated = true; } }

        private bool _updated = true;
        private VO64SimpleVertex _simpleVertex;

        public F3DZEXVertexWrapper(Vertex vertex)
        {
            Vertex = vertex;
        }

        public VO64SimpleVertex GetAsSimpleVertex()
        {
            if (_updated)
            {
                _simpleVertex = new VO64SimpleVertex(X, Y, Z, U, V, NX, NY, NZ);
                _updated = false;
            }
            return _simpleVertex;
        }
    }
}
