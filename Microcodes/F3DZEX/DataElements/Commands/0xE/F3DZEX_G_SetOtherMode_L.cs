using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_SetOtherMode_L : N64DataElement, IF3DZEXCommand
    {
        public enum LMode
        {
            G_MDSFT_ALPHACOMPARE = 0X00,
            G_MDSFT_ZSRCSEL = 0X02,
            G_MDSFT_RENDERMODE = 0X03
        }

        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_SETOTHERMODE_L; } }

        public string CommandName
        { get { return "G_SETOTHERMODE_L"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Configure lower half of RDP Other Modes"; } }

        public LMode Shift;
        public ushort Length;
        public uint Data;  //Enumeration available here

        public bool IsValid { get; private set; }

        public F3DZEX_G_SetOtherMode_L(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)0x00, (byte)Shift, (byte)(Length - 1),
                    Data);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                Shift = (LMode)ByteHelper.ReadByte(value, 2);
                Length = (ushort)((ushort)ByteHelper.ReadByte(value, 3) + 1);
                Data = ByteHelper.ReadUInt(value, 4);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
