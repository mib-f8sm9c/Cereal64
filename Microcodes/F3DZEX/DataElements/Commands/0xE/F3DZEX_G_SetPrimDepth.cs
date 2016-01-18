using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_SetPrimDepth : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_SETPRIMDEPTH; } }

        public string CommandName
        { get { return "G_SETPRIMDEPTH"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Set depth value of whole primitive (used when enabled)"; } }

        public short Z;
        public short DeltaZ;

        public bool IsValid { get; private set; }

        public F3DZEX_G_SetPrimDepth(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)0x00, (byte)0x00, (byte)0x00,
                    Z, DeltaZ);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;
                Z = ByteHelper.ReadShort(value, 4);
                DeltaZ = ByteHelper.ReadShort(value, 6);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
