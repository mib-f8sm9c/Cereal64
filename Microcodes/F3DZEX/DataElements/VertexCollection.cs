using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Cereal64.Common;
using Cereal64.Common.DataElements;
using System.Xml.Linq;
using System.ComponentModel;

namespace Cereal64.Microcodes.F3DZEX.DataElements
{
    public class VertexCollection : N64DataElement
    {
        private List<Vertex> _vertices = new List<Vertex>();
        
        [CategoryAttribute("Vertex Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("Collection of consecutive vertices"),
        TypeConverter(typeof(CollectionConverter))]
        public ReadOnlyCollection<Vertex> Vertices { get { return _vertices.AsReadOnly(); } }

        public VertexCollection(XElement xml, byte[] fileData)
            : base(xml, fileData)
        {
        }

        public VertexCollection(int index, byte[] bytes)
            : base(index, bytes)
        {
        }

        public VertexCollection(int index, List<Vertex> vertices)
            : base (index, null)
        {
            _vertices = vertices;
        }

        public override byte[] RawData
        {
            get
            {
                byte[] rawBytes = new byte[RawDataSize];
                for (int i = 0; i < _vertices.Count; i++)
                {
                    Array.Copy(_vertices[i].RawData, 0, rawBytes, i * 0x10, 0x10);
                }
                return rawBytes;
            }
            set
            {
                if (value == null)
                    return;

                byte[] bytes = new byte[0x10];
                for (int i = 0; i < value.Length - 0xF; i += 0x10)
                {
                    Array.Copy(value, i, bytes, 0, 0x10);
                    Vertex vtx = new Vertex(FileOffset + i, bytes);
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
