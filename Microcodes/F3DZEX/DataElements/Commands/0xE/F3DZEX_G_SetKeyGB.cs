using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_SetKeyGB : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_SETKEYGB; } }

        public string CommandName
        { get { return "G_SETKEYGB"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Set green and blue components of chroma key"; } }

        public ushort WidthG;
        public ushort WidthB;
        public byte CenterG;
        public byte ScaleG;
        public byte CenterB;
        public byte ScaleB;

        public bool IsValid { get; private set; }

        public F3DZEX_G_SetKeyGB(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((((uint)WidthG) & 0x0FFF) << 12) |
                                (((uint)WidthB) & 0x0FFF));
                return ByteHelper.CombineIntoBytes(firstHalf, 
                    CenterG, ScaleG, CenterB, ScaleB);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                WidthG = (ushort)((ByteHelper.ReadUShort(value, 1) >> 4) & 0x0FFF);
                WidthB = (ushort)(ByteHelper.ReadUShort(value, 2) & 0x0FFF);

                CenterG = ByteHelper.ReadByte(value, 4);
                ScaleG = ByteHelper.ReadByte(value, 5);
                CenterB = ByteHelper.ReadByte(value, 6);
                ScaleB = ByteHelper.ReadByte(value, 7);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
