using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_Tri2 : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_TRI2; } }

        public string CommandName
        { get { return "G_TRI2"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Draw two triangles"; } }

        public byte Vertex1;
        public byte Vertex2;
        public byte Vertex3;

        public byte Vertex4;
        public byte Vertex5;
        public byte Vertex6;

        public bool IsValid { get; private set; }

        public F3DZEX_G_Tri2(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)(Vertex1 * 2),
                    (byte)(Vertex2 * 2), (byte)(Vertex3 * 2),
                    (byte)0x00, (byte)(Vertex4 * 2), (byte)(Vertex5 * 2), 
                    (byte)(Vertex6 * 2));
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;
                Vertex1 = (byte)(ByteHelper.ReadByte(value, 1) / 2);
                Vertex2 = (byte)(ByteHelper.ReadByte(value, 2) / 2);
                Vertex3 = (byte)(ByteHelper.ReadByte(value, 3) / 2);
                //0x00
                Vertex4 = (byte)(ByteHelper.ReadByte(value, 5) / 2);
                Vertex5 = (byte)(ByteHelper.ReadByte(value, 6) / 2);
                Vertex6 = (byte)(ByteHelper.ReadByte(value, 7) / 2);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
