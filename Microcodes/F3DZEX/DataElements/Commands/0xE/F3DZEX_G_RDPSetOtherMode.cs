using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_RDPSetOtherMode : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_RDPSETOTHERMODE; } }

        public string CommandName
        { get { return "G_RDPSETOTHERMODE"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Set state of the RDP Other Modes bits, both halves (combined E2 and E3)."; } }

        public uint OModeH;
        public uint OModeL;

        public bool IsValid { get; private set; }

        public F3DZEX_G_RDPSetOtherMode(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((((uint)CommandID) << 24) | (OModeH & 0x00FFFFFF),
                    OModeL);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                OModeH = ByteHelper.ReadUInt(value, 0) & 0x00FFFFFF;
                OModeL = ByteHelper.ReadUInt(value, 4);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
