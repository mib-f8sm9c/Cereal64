using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;
using Cereal64.Common.DataElements;
using System.Xml.Linq;
using System.ComponentModel;

namespace Cereal64.Microcodes.F3DEX.DataElements
{
    public class ReversedVertex : Vertex
    {
        public ReversedVertex(XElement xml, byte[] fileData)
            : base(xml, fileData)
        {
        }

        public ReversedVertex(int index, byte[] rawData)
            : base(index, rawData)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes(X, Y, Z, S, T,
                    (ushort)0x0000, R, G, B, A);
            }
            set
            {
                X = ByteHelper.ReadShort(value, 0);
                Y = ByteHelper.ReadShort(value, 2);
                Z = ByteHelper.ReadShort(value, 4);
                S = ByteHelper.ReadShort(value, 6);
                T = ByteHelper.ReadShort(value, 8);
                //10 is 0x0000
                R = ByteHelper.ReadSByte(value, 12);
                G = ByteHelper.ReadSByte(value, 13);
                B = ByteHelper.ReadSByte(value, 14);
                A = ByteHelper.ReadByte(value, 15);
            }
        }
    }
}
