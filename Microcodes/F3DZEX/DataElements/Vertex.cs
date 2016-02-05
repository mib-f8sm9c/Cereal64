using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;
using Cereal64.Common.DataElements;
using System.Xml.Linq;

namespace Cereal64.Microcodes.F3DZEX.DataElements
{
    public class Vertex : N64DataElement
    {
        public short X, Y, Z;
        public short S, T;
        public sbyte R, G, B; //Can represent X, Y, and Z for vertex normals
        public byte A;

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
