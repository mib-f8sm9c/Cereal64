using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_DL : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_DL; } }

        public string CommandName
        { get { return "G_DL"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Jump or \"call\" another display list"; } }

        public byte ForceJump;
        public uint DLAddress;

        public bool IsValid { get; private set; }

        public F3DZEX_G_DL(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, ForceJump, (byte)0x00, (byte)0x00,
                    DLAddress);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                ForceJump = ByteHelper.ReadByte(value, 1);
                DLAddress = ByteHelper.ReadUInt(value, 4);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
