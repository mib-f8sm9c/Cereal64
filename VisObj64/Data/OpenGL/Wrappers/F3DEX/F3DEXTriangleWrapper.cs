using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Microcodes.F3DEX.DataElements.Commands;

namespace Cereal64.VisObj64.Data.OpenGL.Wrappers.F3DEX
{
    public class F3DEXTriangleWrapper : IVO64Triangle
    {
       //NOTE: THIS CLASS DOES NOT WORK!!
        public enum TriangleMode
        {
            Single,
            Double
        }

        public F3DEX_G_Tri1 SingleTriangle;
        public F3DEX_G_Tri2 DoubleTriangle;
        public int DoubleIndex { get; private set; } //Base 1

        public ushort VertexOffset = 0;

        public TriangleMode Mode { get; private set; }

        public ushort T1
        {
            get
            {
                switch (Mode)
                {
                    case TriangleMode.Double:
                        if(DoubleIndex == 1)
                            return (ushort)(DoubleTriangle.Vertex1 + VertexOffset);
                        else
                            return (ushort)(DoubleTriangle.Vertex4 + VertexOffset);
                    case TriangleMode.Single:
                    default:
                        return (ushort)(SingleTriangle.Vertex1 + VertexOffset);
                }
            }
            set
            {
                switch (Mode)
                {
                    case TriangleMode.Double:
                        if (DoubleIndex == 1)
                            DoubleTriangle.Vertex1 = (byte)(value - VertexOffset);
                        else
                            DoubleTriangle.Vertex4 = (byte)(value - VertexOffset);
                        break;
                    case TriangleMode.Single:
                    default:
                        SingleTriangle.Vertex1 = (byte)(value - VertexOffset);
                        break;
                }
                _updated = true;
            }
        }

        public ushort T2
        {
            get
            {
                switch (Mode)
                {
                    case TriangleMode.Double:
                        if (DoubleIndex == 1)
                            return (ushort)(DoubleTriangle.Vertex2 + VertexOffset);
                        else
                            return (ushort)(DoubleTriangle.Vertex5 + VertexOffset);
                    case TriangleMode.Single:
                    default:
                        return (ushort)(SingleTriangle.Vertex2 + VertexOffset);
                }
            }
            set
            {
                switch (Mode)
                {
                    case TriangleMode.Double:
                        if (DoubleIndex == 1)
                            DoubleTriangle.Vertex2 = (byte)(value- VertexOffset);
                        else
                            DoubleTriangle.Vertex5 = (byte)(value - VertexOffset);
                        break;
                    case TriangleMode.Single:
                    default:
                        SingleTriangle.Vertex2 = (byte)(value - VertexOffset);
                        break;
                }
                _updated = true;
            }
        }

        public ushort T3
        {
            get
            {
                switch (Mode)
                {
                    case TriangleMode.Double:
                        if (DoubleIndex == 1)
                            return (ushort)(DoubleTriangle.Vertex3 + VertexOffset);
                        else
                            return (ushort)(DoubleTriangle.Vertex6 + VertexOffset);
                    case TriangleMode.Single:
                    default:
                        return (ushort)(SingleTriangle.Vertex3 + VertexOffset);
                }
            }
            set
            {
                switch (Mode)
                {
                    case TriangleMode.Double:
                        if (DoubleIndex == 1)
                            DoubleTriangle.Vertex3 = (byte)(value - VertexOffset);
                        else
                            DoubleTriangle.Vertex6 = (byte)(value - VertexOffset);
                        break;
                    case TriangleMode.Single:
                    default:
                        SingleTriangle.Vertex3 = (byte)(value - VertexOffset);
                        break;
                }
                _updated = true;
            }
        }

        private bool _updated = true;
        private VO64SimpleTriangle _simpleTriangle;

        public F3DEXTriangleWrapper(F3DEX_G_Tri1 triangle)
        {
            Mode = TriangleMode.Single;
            SingleTriangle = triangle;
        }

        public F3DEXTriangleWrapper(F3DEX_G_Tri2 triangle, int index)
        {
            Mode = TriangleMode.Double;
            DoubleIndex = index % 2; //Handle stupid user input
            DoubleTriangle = triangle;
        }

        public VO64SimpleTriangle GetAsSimpleTriangle()
        {
            if (_updated)
            {
                _simpleTriangle = new VO64SimpleTriangle(T1, T2, T3);
                _updated = false;
            }
            return _simpleTriangle;
        }
    }
}
