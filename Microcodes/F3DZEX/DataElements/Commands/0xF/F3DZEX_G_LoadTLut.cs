using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_LoadTLut : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_LOADTLUT; } }

        public string CommandName
        { get { return "G_LOADTLUT"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Load palette into TMEM."; } }

        public byte Tile;
        public ushort Count;

        public bool IsValid { get; private set; }

        public F3DZEX_G_LoadTLut(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)0x00, (byte)0x00, (byte)0x00,
                    (byte)(Tile & 0x0F), (ushort)(((Count & 0x3FF) * 4) << 4), (byte)0x00);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;
                Tile = (byte)(ByteHelper.ReadByte(value, 4) & 0x0F);
                Count = (ushort)((ByteHelper.ReadUShort(value, 5) >> 4) / 4);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
