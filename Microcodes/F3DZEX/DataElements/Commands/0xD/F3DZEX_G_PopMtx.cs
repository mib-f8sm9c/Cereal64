using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_PopMtx : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_POPMTX; } }

        public string CommandName
        { get { return "G_POPMTX"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Pops n modelview matrices from stack"; } }

        public uint Num;

        public bool IsValid { get; private set; }

        public F3DZEX_G_PopMtx(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)0x38, (byte)0x00, (byte)0x02,
                    Num * 64);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                Num = ByteHelper.ReadUInt(value, 4) / 64;

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
