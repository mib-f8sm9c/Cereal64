using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_Vtx : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_VTX; } }

        public string CommandName
        { get { return "G_VTX"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Loads vertices to RSP"; } }

        public byte VertexCount;
        public byte TargetBufferIndex; //Cloud Modding suggests subtracting VertexCount from this
        public uint VertexSourceAddress;

        public bool IsValid { get; private set; }

        public F3DZEX_G_Vtx(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((uint)VertexCount) << 12 |
                                (byte)(((TargetBufferIndex) & 0x7F) * 2));
                return ByteHelper.CombineIntoBytes(firstHalf, VertexSourceAddress);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;
                VertexCount = (byte)(ByteHelper.ReadUShort(value, 1) >> 4);
                TargetBufferIndex = (byte)(ByteHelper.ReadByte(value, 3) / 2);
                VertexSourceAddress = ByteHelper.ReadUInt(value, 4);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
