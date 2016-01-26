using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_SetEnvColor : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_SETENVCOLOR; } }

        public string CommandName
        { get { return "G_SETENVCOLOR"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Set environment color register"; } }

        public byte R, G, B, A;

        public bool IsValid { get; private set; }

        public F3DZEX_G_SetEnvColor(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)0x00, (byte)0x00, (byte)0x00,
                    R, G, B, A);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;
                R = ByteHelper.ReadByte(value, 4);
                G = ByteHelper.ReadByte(value, 5);
                B = ByteHelper.ReadByte(value, 6);
                A = ByteHelper.ReadByte(value, 7);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
