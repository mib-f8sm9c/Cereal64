using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_SPNoOp : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_SPNOOP; } }

        public string CommandName
        { get { return "G_SPNOOP"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Stall RSP for a few cycles"; } }

        public bool IsValid { get; private set; }

        public F3DZEX_G_SPNoOp(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)0x00, (ushort)0x0000,
                    (uint)0x00000000);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
