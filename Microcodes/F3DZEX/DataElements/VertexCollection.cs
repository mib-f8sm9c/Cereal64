using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements
{
    public class VertexCollection : N64DataElement
    {
        private List<Vertex> _vertices = new List<Vertex>();

        public ReadOnlyCollection<Vertex> Vertices { get { return _vertices.AsReadOnly(); } }

        public VertexCollection(int index, byte[] bytes)
            : base(index, bytes)
        {
        }


        public override byte[] RawData
        {
            get
            {
                byte[] rawBytes = new byte[RawDataSize];
                for (int i = 0; i < rawBytes.Length; i++)
                {
                    Array.Copy(_vertices[i].RawData, 0, rawBytes, i * 0x10, 0x10);
                }
                return rawBytes;
            }
            set
            {
                byte[] bytes = new byte[0x10];
                for (int i = 0; i < value.Length - 0xF; i += 0x10)
                {
                    Array.Copy(value, i, bytes, 0, 0x10);
                    Vertex vtx = new Vertex(Address.Offset + i, bytes);
                    _vertices.Add(vtx);
                }
            }
        }

        public override int RawDataSize
        {
            get
            {
                return _vertices.Count * 0x10;
            }
        }

    }
}
