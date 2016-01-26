using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_Quad : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_QUAD; } }

        public string CommandName
        { get { return "G_QUAD"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Draw quadrilateral (deprecated)"; } }

        public byte Vertex1;
        public byte Vertex2;
        public byte Vertex3;
        public byte Vertex4;

        public bool IsValid { get; private set; }

        public F3DZEX_G_Quad(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)(Vertex1 * 2),
                    (byte)(Vertex2 * 2), (byte)(Vertex3 * 2),
                    (byte)0x00, (byte)(Vertex1 * 2), (byte)(Vertex3 * 2),
                    (byte)(Vertex4 * 2));
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;
                Vertex1 = (byte)(ByteHelper.ReadByte(value, 1) / 2);
                Vertex2 = (byte)(ByteHelper.ReadByte(value, 2) / 2);
                Vertex3 = (byte)(ByteHelper.ReadByte(value, 3) / 2);
                //0x00
                if (Vertex1 != (byte)(ByteHelper.ReadByte(value, 5) / 2)) return;
                if (Vertex3 != (byte)(ByteHelper.ReadByte(value, 6) / 2)) return;
                Vertex4 = (byte)(ByteHelper.ReadByte(value, 7) / 2);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
