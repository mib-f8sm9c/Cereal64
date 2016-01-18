using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_SetKeyR : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_SETKEYR; } }

        public string CommandName
        { get { return "G_SETKEYR"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Set red component of chroma key"; } }

        public ushort WidthR; //THIS IS AN UNSIGNED FIXED POINT 4.8 NUBMER
        public byte CenterR;
        public byte ScaleR;

        public bool IsValid { get; private set; }

        public F3DZEX_G_SetKeyR(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)0x00, (byte)0x00, (byte)0x00,
                    (ushort)(WidthR & 0x0FFF), CenterR, ScaleR);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                WidthR = (ushort)(ByteHelper.ReadUShort(value, 4) & 0x0FFF);
                CenterR = ByteHelper.ReadByte(value, 6);
                ScaleR = ByteHelper.ReadByte(value, 7);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
