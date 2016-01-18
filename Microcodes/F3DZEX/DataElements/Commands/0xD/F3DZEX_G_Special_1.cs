using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_Special_1 : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_SPECIAL_1; } }

        public string CommandName
        { get { return "G_SPECIAL_1"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Explicitly reserved opcode... or is it?"; } }

        public byte Unknown1;
        public ushort Unknown2;
        public uint Unknown3;

        public bool IsValid { get; private set; }

        public F3DZEX_G_Special_1(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, Unknown1, Unknown2, Unknown3);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;
                Unknown1 = ByteHelper.ReadByte(value, 1);
                Unknown2 = ByteHelper.ReadUShort(value, 2);
                Unknown3 = ByteHelper.ReadUInt(value, 4);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
