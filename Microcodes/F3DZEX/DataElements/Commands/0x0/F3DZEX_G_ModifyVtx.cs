using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_ModifyVtx : N64DataElement, IF3DZEXCommand
    {
        public enum OverwriteType
        {
            G_MWO_POINT_RGBA = 0x10,
            G_MWO_POINT_ST = 0x14,
            G_MWO_POINT_XYSCREEN = 0x18,
            G_MWO_POINT_ZSCREEN = 0x1C
        }

        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_MODIFYVTX; } }

        public string CommandName
        { get { return "G_MODIFYVTX"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Modifies a portion of vertex attributes in RSP "; } }

        public OverwriteType Type;
        public ushort TargetBufferIndex;
        public uint NewValue;

        public bool IsValid { get; private set; }

        public F3DZEX_G_ModifyVtx(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((uint)Type) << 16 |
                                (uint)(TargetBufferIndex * 2));
                return ByteHelper.CombineIntoBytes(firstHalf, NewValue);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                byte type = ByteHelper.ReadByte(value, 1);
                if (!Enum.IsDefined(typeof(OverwriteType), (int)type))
                    return;

                Type = (OverwriteType)type;
                TargetBufferIndex = (ushort)(ByteHelper.ReadUShort(value, 2) / 2);
                NewValue = ByteHelper.ReadUInt(value, 4);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
