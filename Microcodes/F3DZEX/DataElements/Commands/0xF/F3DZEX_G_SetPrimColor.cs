using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_SetPrimColor : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_SETPRIMCOLOR; } }

        public string CommandName
        { get { return "G_SETPRIMCOLOR"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Set primitive color register"; } }

        public byte MinLevel, LodFrac;
        public byte R, G, B, A;

        public bool IsValid { get; private set; }

        public F3DZEX_G_SetPrimColor(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)0x00, MinLevel, LodFrac,
                    R, G, B, A);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;
                MinLevel = ByteHelper.ReadByte(value, 2);
                LodFrac = ByteHelper.ReadByte(value, 3);
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
