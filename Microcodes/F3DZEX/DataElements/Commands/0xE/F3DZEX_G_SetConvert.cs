using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_SetConvert : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_SETCONVERT; } }

        public string CommandName
        { get { return "G_SETCONVERT"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Set conversion factors for YUV to RGB conversion"; } }

        public ushort K0, K1, K2, K3, K4, K5;

        public bool IsValid { get; private set; }

        public F3DZEX_G_SetConvert(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((((uint)K0) & 0x01FF) << 13) |
                                ((((uint)K1) & 0x01FF) << 4) |
                                ((((uint)K2) >> 5) & 0x000F));
                uint secondHalf = (uint)(((uint)K2) << 27 |
                                ((((uint)K3) & 0x01FF) << 18) |
                                ((((uint)K4) & 0x01FF) << 9) |
                                (((uint)K5) & 0x01FF));

                return ByteHelper.CombineIntoBytes(firstHalf, secondHalf);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                K0 = (ushort)((ByteHelper.ReadUShort(value, 1) >> 5) & 0x01FF);
                K1 = (ushort)((ByteHelper.ReadUShort(value, 2) >> 4) & 0x01FF);
                K2 = (ushort)((ByteHelper.ReadUShort(value, 3) >> 3) & 0x01FF);
                K3 = (ushort)((ByteHelper.ReadUShort(value, 4) >> 2) & 0x01FF);
                K4 = (ushort)((ByteHelper.ReadUShort(value, 5) >> 1) & 0x01FF);
                K5 = (ushort)(ByteHelper.ReadUShort(value, 6) & 0x01FF);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
