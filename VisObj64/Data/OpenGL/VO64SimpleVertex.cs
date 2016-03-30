using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace Cereal64.VisObj64.Data.OpenGL
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VO64SimpleVertex
    {
        float V1, V2, V3;
        float T1, T2;
        float N1, N2, N3;

        public VO64SimpleVertex(float v1, float v2, float v3, float t1, float t2, float n1, float n2, float n3)
            : this()
        {
            V1 = v1;
            V2 = v2;
            V3 = v3;
            T1 = t1;
            T2 = t2;
            N1 = n1;
            N2 = n2;
            N3 = n3;
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
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VO64SimpleTriangle
    {
        private ushort T1, T2, T3;

        public VO64SimpleTriangle(ushort t1, ushort t2, ushort t3)
            : this()
        {
            T1 = t1;
            T2 = t2;
            T3 = t3;
        }

        public static byte Size
        {
            get
            {
                return sizeof(ushort) * 3;
            }
        }
    }
}
