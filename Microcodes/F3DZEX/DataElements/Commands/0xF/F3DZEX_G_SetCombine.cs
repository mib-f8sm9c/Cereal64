using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_SetCombine : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_SETCOMBINE; } }

        public string CommandName
        { get { return "G_SETCOMBINE"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Set color combiner parameters"; } }

        public byte a0, c0, Aa0, Ac0, a1, c1, b0, b1, Aa1, Ac1, d0, Ab0, Ad0, d1, Ab1, Ad1;

        public bool IsValid { get; private set; }

        public F3DZEX_G_SetCombine(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((((uint)a0) & 0x0F) << 20) |
                                ((((uint)c0) & 0x1F) << 15) |
                                ((((uint)Aa0) & 0x07) << 12) |
                                ((((uint)Ac0) & 0x07) << 9) |
                                ((((uint)a1) & 0x0F) << 5) |
                                (((uint)c1) & 0x1F));
                uint secondHalf = (uint)(((uint)b0) << 28 |
                                ((((uint)b1) & 0x0F) << 24) |
                                ((((uint)Aa1) & 0x07) << 21) |
                                ((((uint)Ac1) & 0x07) << 18) |
                                ((((uint)d0) & 0x07) << 15) |
                                ((((uint)Ab0) & 0x07) << 12) |
                                ((((uint)Ad0) & 0x07) << 9) |
                                ((((uint)d1) & 0x07) << 6) |
                                ((((uint)Ab1) & 0x07) << 3) |
                                (((uint)Ad1) & 0x07));
                return ByteHelper.CombineIntoBytes(firstHalf, secondHalf);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                a0 = (byte)((ByteHelper.ReadByte(value, 1) >> 4) & 0x0F);
                ushort temp = ByteHelper.ReadUShort(value, 1);
                c0 = (byte)((temp >> 7) & 0x001F);
                Aa0 = (byte)((temp >> 4) & 0x0007);
                Ac0 = (byte)((temp >> 1) & 0x0007);
                temp = ByteHelper.ReadUShort(value, 2);
                a1 = (byte)((temp >> 5) & 0x000F);
                c1 = (byte)(temp & 0x001F);
                byte tempByte = ByteHelper.ReadByte(value, 4);
                b0 = (byte)((tempByte >> 4) & 0x0F);
                b1 = (byte)(tempByte & 0x0F);
                tempByte = ByteHelper.ReadByte(value, 5);
                Aa1 = (byte)((tempByte >> 5) & 0x07);
                Ac1 = (byte)((tempByte >> 2) & 0x07);
                temp = ByteHelper.ReadUShort(value, 5);
                d0 = (byte)((temp >> 7) & 0x07);
                Ab0 = (byte)((temp >> 4) & 0x07);
                Ad0 = (byte)((temp >> 1) & 0x07);
                temp = ByteHelper.ReadUShort(value, 6);
                d1 = (byte)((temp >> 6) & 0x07);
                Ab1 = (byte)((temp >> 3) & 0x07);
                Ad1 = (byte)(temp & 0x07);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
