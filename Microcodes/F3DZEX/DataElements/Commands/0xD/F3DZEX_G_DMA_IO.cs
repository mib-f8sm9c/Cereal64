using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_DMA_IO : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_DMA_IO; } }

        public string CommandName
        { get { return "G_DMA_IO"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "DMA transfer between main RAM and RCP DMEM (or IMEM)"; } }

        public byte Flag;
        public ushort DMem;
        public ushort Size;
        public uint DRam;

        public bool IsValid { get; private set; }

        public F3DZEX_G_DMA_IO(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((uint)Flag) << 23 |
                                (((uint)DMem / 8) & 0x3FF) << 13 |
                                (uint)(Size - 1));
                return ByteHelper.CombineIntoBytes(firstHalf, DRam);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;
                Flag = (byte)((ByteHelper.ReadByte(value, 1) >> 7) & 0x01);
                DMem = (ushort)(((ByteHelper.ReadUShort(value, 1) >> 5) & 0x03FF) * 8);
                Size = (ushort)((ByteHelper.ReadUShort(value, 2) & 0xFFF) + 1);
                DRam = ByteHelper.ReadUInt(value, 4);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
