using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cereal64.VisObj64.Data.OpenGL
{
    public interface IVO64Vertex
    {
        float X { get; set; }
        float Y { get; set; }
        float Z { get; set; }

        float U { get; set; }
        float V { get; set; }

        float NX { get; set; }
        float NY { get; set; }
        float NZ { get; set; }

        void SetVBOReference(uint vbo, int vboOffset);

        VO64SimpleVertex GetAsSimpleVertex();
    }
}
