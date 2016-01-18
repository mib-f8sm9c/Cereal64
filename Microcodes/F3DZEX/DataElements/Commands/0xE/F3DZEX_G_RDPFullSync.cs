using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_RDPFullSync : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_RDPFULLSYNC; } }

        public string CommandName
        { get { return "G_RDPFULLSYNC"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Indicates end of RDP processing; interrupts CPU when RDP has nothing more to do"; } }

        public bool IsValid { get; private set; }

        public F3DZEX_G_RDPFullSync(int index, byte[] rawBytes)
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
