using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_SetTileSize : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_SETTILESIZE; } }

        public string CommandName
        { get { return "G_SETTILESIZE"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Set dimensions of texture"; } }

        public ushort ULS, ULT;
        public byte Tile;
        public ushort LRS, LRT;

        public bool IsValid { get; private set; }

        public F3DZEX_G_SetTileSize(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((((uint)ULS) & 0x0FFF) << 12) |
                                ((uint)ULT) & 0x0FFF);
                uint secondHalf = (uint)(((((uint)Tile) & 0x0F) << 24) |
                                ((((uint)LRS) & 0x0FFF) << 12) |
                                ((uint)LRT) & 0x0FFF);
                return ByteHelper.CombineIntoBytes(firstHalf, secondHalf);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                ULS = (ushort)(ByteHelper.ReadUShort(value, 1) >> 4);
                ULT = (ushort)(ByteHelper.ReadUShort(value, 2) & 0x0FFF);
                Tile = (byte)(ByteHelper.ReadByte(value, 4) & 0x0F);
                LRS = (ushort)(ByteHelper.ReadUShort(value, 5) >> 4);
                LRT = (ushort)(ByteHelper.ReadUShort(value, 6) & 0x0FFF);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
