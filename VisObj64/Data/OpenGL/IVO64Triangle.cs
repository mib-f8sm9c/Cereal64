using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cereal64.VisObj64.Data.OpenGL
{
    public interface IVO64Triangle
    {
        ushort T1 { get; set; }
        ushort T2 { get; set; }
        ushort T3 { get; set; }

        VO64SimpleTriangle GetAsSimpleTriangle();
    }
}
