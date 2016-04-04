using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace Cereal64.VisObj64.Data.OpenGL
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VO64SimpleVertex : IVO64Vertex
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public float U { get; set; }
        public float V { get; set; }

        public float NX { get; set; }
        public float NY { get; set; }
        public float NZ { get; set; }

        public VO64SimpleVertex(float x, float y, float z, float u, float v, float nx, float ny, float nz)
            : this()
        {
            X = x;
            Y = y;
            Z = z;
            U = u;
            V = v;
            NX = nx;
            NY = ny;
            NZ = nz;
        }

        public static void SetOpenGLVertexFormat()
        {
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 8 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            
            GL.NormalPointer(NormalPointerType.Float, 8 * sizeof(float), (IntPtr)(5 * sizeof(float)));
            GL.EnableClientState(ArrayCap.NormalArray);
        }

        public static byte Size
        {
            get
            {
                return sizeof(float) * 8;
            }
        }

        public VO64SimpleVertex GetAsSimpleVertex()
        {
            return this;
        }
    }
}
