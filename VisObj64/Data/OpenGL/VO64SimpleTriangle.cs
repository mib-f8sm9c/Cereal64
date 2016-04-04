using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace Cereal64.VisObj64.Data.OpenGL
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VO64SimpleTriangle : IVO64Triangle

    {
        public ushort T1 { get; set; }
        public ushort T2 { get; set; }
        public ushort T3 { get; set; }

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

        public VO64SimpleTriangle GetAsSimpleTriangle()
        {
            return this;
        }
    }
}
