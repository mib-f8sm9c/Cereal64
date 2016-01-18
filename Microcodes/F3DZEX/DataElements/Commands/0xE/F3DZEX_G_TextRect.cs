using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_TexRect : N64DataElement, IF3DZEXCommand
    {
        //THIS COMMAND IS FOLLOWED BY 0xE1 AND 0xF1 COMMANDS THAT PROVIDE EXTRA DATA

        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_TEXRECT; } }

        public string CommandName
        { get { return "G_TEXRECT"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Draw textured rectangle onto screen"; } }

        public ushort LRX;
        public ushort LRY;
        public byte Tile;
        public ushort ULX;
        public ushort ULY;

        public bool IsValid { get; private set; }

        public F3DZEX_G_TexRect(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {

                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((((uint)LRX) & 0x0FFF) << 12) |
                                (((uint)LRY) & 0x0FFF));
                uint secondHalf = (uint)((uint)(Tile & 0x0F) << 24 |
                                ((((uint)ULX) & 0x0FFF) << 12) |
                                (((uint)ULY) & 0x0FFF));
                return ByteHelper.CombineIntoBytes(firstHalf, secondHalf);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                LRX = (ushort)((ByteHelper.ReadUShort(value, 1) >> 4) & 0x0FFF);
                LRY = (ushort)(ByteHelper.ReadUShort(value, 2) & 0x0FFF);

                Tile = (byte)(ByteHelper.ReadByte(value, 4) & 0x0F);
                ULX = (ushort)((ByteHelper.ReadUShort(value, 5) >> 4) & 0x0FFF);
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
