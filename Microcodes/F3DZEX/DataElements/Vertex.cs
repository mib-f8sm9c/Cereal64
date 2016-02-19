using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;
using Cereal64.Common.DataElements;
using System.Xml.Linq;
using System.ComponentModel;

namespace Cereal64.Microcodes.F3DZEX.DataElements
{
    public class Vertex : N64DataElement
    {
        [CategoryAttribute("Vertex Settings"),
        DescriptionAttribute("X coordinate of the vertex")]
        public short X { get; set; }
        [CategoryAttribute("Vertex Settings"),
        DescriptionAttribute("Y coordinate of the vertex")]
        public short Y { get; set; }
        [CategoryAttribute("Vertex Settings"),
        DescriptionAttribute("Z coordinate of the vertex")]
        public short Z { get; set; }
        [CategoryAttribute("Vertex Settings"),
        DescriptionAttribute("X coordinate of the texture")]
        public short S { get; set; }
        [CategoryAttribute("Vertex Settings"),
        DescriptionAttribute("Y coordinate of the vertex")]
        public short T { get; set; }
        [CategoryAttribute("Vertex Settings"),
        DescriptionAttribute("Red color component of the vertex (also can be X coordinate of the vertex normal)")]
        public sbyte R { get; set; }
        [CategoryAttribute("Vertex Settings"),
        DescriptionAttribute("Green color component of the vertex (also can be Y coordinate of the vertex normal)")]
        public sbyte G { get; set; }
        [CategoryAttribute("Vertex Settings"),
        DescriptionAttribute("Blue color component of the vertex (also can be Z coordinate of the vertex normal)")]
        public sbyte B { get; set; }
        [CategoryAttribute("Vertex Settings"),
        DescriptionAttribute("Alpha color component of the vertex")]
        public byte A { get; set; }

        public Vertex(XElement xml, byte[] fileData)
            : base(xml, fileData)
        {
        }

        public Vertex(int index, byte[] rawData)
            : base(index, rawData)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes(X, Y, Z, (ushort)0x0000,
                    S, T, R, G, B, A);
            }
            set
            {
                X = ByteHelper.ReadShort(value, 0);
                Y = ByteHelper.ReadShort(value, 2);
                Z = ByteHelper.ReadShort(value, 4);
                //6 is 0x00
                S = ByteHelper.ReadShort(value, 8);
                T = ByteHelper.ReadShort(value, 10);
                R = ByteHelper.ReadSByte(value, 12);
                G = ByteHelper.ReadSByte(value, 13);
                B = ByteHelper.ReadSByte(value, 14);
                A = ByteHelper.ReadByte(value, 15);
            }
        }

        public override int RawDataSize { get { return 0x10; } }
    }
}
