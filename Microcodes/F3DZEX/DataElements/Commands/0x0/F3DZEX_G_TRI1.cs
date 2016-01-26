using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_Tri1 : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_TRI1; } }

        public string CommandName
        { get { return "G_TRI1"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Draw a triangle"; } }

        public byte Vertex1;
        public byte Vertex2;
        public byte Vertex3;

        public bool IsValid { get; private set; }

        public F3DZEX_G_Tri1(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)(Vertex1 * 2), 
                    (byte)(Vertex2 * 2), (byte)(Vertex3 * 2),
                    (uint)0x00000000);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;
                Vertex1 = (byte)(ByteHelper.ReadByte(value, 1) / 2);
                Vertex2 = (byte)(ByteHelper.ReadByte(value, 2) / 2);
                Vertex3 = (byte)(ByteHelper.ReadByte(value, 3) / 2);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
