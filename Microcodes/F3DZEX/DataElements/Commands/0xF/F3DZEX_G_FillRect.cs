using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    //NOTE: Not sure, but the ushort values might be 4x what's in the raw data
    public class F3DZEX_G_FillRect
        : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_FILLRECT; } }

        public string CommandName
        { get { return "G_FILLRECT"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Draw solid color rectangle to screen"; } }

        public ushort LRX, LRY;
        public ushort ULX, ULY;

        public bool IsValid { get; private set; }

        public F3DZEX_G_FillRect(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((((uint)LRX) & 0x0FFF) << 12) |
                                ((uint)LRY) & 0x0FFF);
                uint secondHalf = (uint)(((((uint)ULX) & 0x0FFF) << 12) |
                                ((uint)ULY) & 0x0FFF);
                return ByteHelper.CombineIntoBytes(firstHalf, secondHalf);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                LRX = (ushort)(ByteHelper.ReadUShort(value, 1) >> 4);
                LRY = (ushort)(ByteHelper.ReadUShort(value, 2) & 0x0FFF);

                ULX = (ushort)(ByteHelper.ReadUShort(value, 5) >> 4);
                ULY = (ushort)(ByteHelper.ReadUShort(value, 6) & 0x0FFF);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
