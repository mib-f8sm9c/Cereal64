using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_LoadBlock : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_LOADBLOCK; } }

        public string CommandName
        { get { return "G_LOADBLOCK"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Load texture into TMEM as a continuous stream"; } }

        public ushort ULS, ULT;
        public byte Tile;
        public ushort Texels, DLT;

        public bool IsValid { get; private set; }

        public F3DZEX_G_LoadBlock(int index, byte[] rawBytes)
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
                                ((((uint)Texels) & 0x0FFF) << 12) |
                                ((uint)DLT) & 0x0FFF);
                return ByteHelper.CombineIntoBytes(firstHalf, secondHalf);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                ULS = (ushort)(ByteHelper.ReadUShort(value, 1) >> 4);
                ULT = (ushort)(ByteHelper.ReadUShort(value, 2) & 0x0FFF);
                Tile = (byte)(ByteHelper.ReadByte(value, 4) & 0x0F);
                Texels = (ushort)(ByteHelper.ReadUShort(value, 5) >> 4);
                DLT = (ushort)(ByteHelper.ReadUShort(value, 6) & 0x0FFF);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
