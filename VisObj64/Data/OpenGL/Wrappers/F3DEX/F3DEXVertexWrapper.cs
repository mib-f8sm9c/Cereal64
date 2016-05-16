using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Microcodes.F3DEX.DataElements;
using OpenTK.Graphics.OpenGL;

namespace Cereal64.VisObj64.Data.OpenGL.Wrappers.F3DEX
{
    public class F3DEXVertexWrapper : IVO64Vertex
    {
        //NOTE: THIS WILL ALL BREAK IF VERTEX IS NULL

        public Vertex Vertex { get; set; }

        public float X { get { return Vertex.X / 100.0f; } set { Vertex.X = (short)Math.Round(value * 100.0f); _updated = true; } }
        public float Y { get { return Vertex.Y / 100.0f; } set { Vertex.Y = (short)Math.Round(value * 100.0f); _updated = true; } }
        public float Z { get { return Vertex.Z / 100.0f; } set { Vertex.Z = (short)Math.Round(value * 100.0f); _updated = true; } }

        //public float U { get { return Vertex.S / 500.0f; } set { Vertex.S = (short)Math.Round(value * 500.0f); _updated = true; } }
        //public float V { get { return Vertex.T / 500.0f; } set { Vertex.T = (short)Math.Round(value * 500.0f); _updated = true; } }

        public float U
        {
            get { if (_texture == null) return Vertex.S / 500.0f; return (float)(Vertex.S * _texture.ScaleS * _texture.ShiftScaleS / 32.0f / _texture.TexWidth); }
            set { Vertex.S = (short)Math.Round(value / _texture.ScaleS / _texture.ShiftScaleS * 32.0f * _texture.TexWidth); }
        }

        public float V
        {
            get { if (_texture == null) return Vertex.T / 500.0f; return (float)(Vertex.T * _texture.ScaleT * _texture.ShiftScaleT / 32.0f / _texture.TexHeight); }
            set { Vertex.T = (short)Math.Round(value / _texture.ScaleT / _texture.ShiftScaleT * 32.0f * _texture.TexHeight); }
        }

        public float NX { get { return Vertex.R / 127.0f; } set { Vertex.R = (sbyte)Math.Round(value * 127.0f); _updated = true; } }
        public float NY { get { return Vertex.G / 127.0f; } set { Vertex.G = (sbyte)Math.Round(value * 127.0f); _updated = true; } }
        public float NZ { get { return Vertex.B / 127.0f; } set { Vertex.B = (sbyte)Math.Round(value * 127.0f); _updated = true; } }

        private bool _updated = true;
        private VO64SimpleVertex _simpleVertex;

        public F3DEXVertexWrapper(Vertex vertex)
        {
            Vertex = vertex;
            Vertex.Updated += vertexUpdated;
        }

        private void vertexUpdated()
        {
            _updated = true;

            if (_vbo != 0)
            {
                //Update data automatically
                VO64SimpleVertex vert = GetAsSimpleVertex();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)_vboOffset, (IntPtr)VO64SimpleVertex.Size, ref vert);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }
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

        private F3DEXTextureWrapper _texture;

        public void SetTextureProperties(F3DEXTextureWrapper textureWrap)
        {
            _texture = textureWrap;
        }

        //NOTE: CAN A VERTEX BE IN MORE THAN ONE VERTEX BUFFER OBJECT?
        private uint _vbo;
        private int _vboOffset;

        public void SetVBOReference(uint vbo, int vboOffset)
        {
            _vbo = vbo;
            _vboOffset = vboOffset;
        }
    }
}
