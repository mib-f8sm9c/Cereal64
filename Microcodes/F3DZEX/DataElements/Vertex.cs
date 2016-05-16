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
        public delegate void UpdateEvent();
        public UpdateEvent Updated = delegate { };

        [CategoryAttribute("Vertex Settings"),
        DescriptionAttribute("X coordinate of the vertex")]
        public short X { get { return _x; } set { _x = value; Updated(); } }
        private short _x;

        [CategoryAttribute("Vertex Settings"),
        DescriptionAttribute("Y coordinate of the vertex")]
        public short Y { get { return _y; } set { _y = value; Updated(); } }
        private short _y;

        [CategoryAttribute("Vertex Settings"),
        DescriptionAttribute("Z coordinate of the vertex")]
        public short Z { get { return _z; } set { _z = value; Updated(); } }
        private short _z;

        [CategoryAttribute("Vertex Settings"),
        DescriptionAttribute("X coordinate of the texture")]
        public short S { get { return _s; } set { _s = value; Updated(); } }
        private short _s;

        [CategoryAttribute("Vertex Settings"),
        DescriptionAttribute("Y coordinate of the vertex")]
        public short T { get { return _t; } set { _t = value; Updated(); } }
        private short _t;

        [CategoryAttribute("Vertex Settings"),
        DescriptionAttribute("Red color component of the vertex (also can be X coordinate of the vertex normal)")]
        public sbyte R { get { return _r; } set { _r = value; Updated(); } }
        private sbyte _r;

        [CategoryAttribute("Vertex Settings"),
        DescriptionAttribute("Green color component of the vertex (also can be Y coordinate of the vertex normal)")]
        public sbyte G { get { return _g; } set { _g = value; Updated(); } }
        private sbyte _g;

        [CategoryAttribute("Vertex Settings"),
        DescriptionAttribute("Blue color component of the vertex (also can be Z coordinate of the vertex normal)")]
        public sbyte B { get { return _b; } set { _b = value; Updated(); } }
        private sbyte _b;

        [CategoryAttribute("Vertex Settings"),
        DescriptionAttribute("Alpha color component of the vertex")]
        public byte A { get { return _a; } set { _a = value; Updated(); } }
        private byte _a;

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

        public void CallUpdate()
        {
            Updated();
        }
    }
}
