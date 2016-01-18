using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_MoveMem : N64DataElement, IF3DZEXCommand
    {
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.G_MOVEMEM; } }

        public string CommandName
        { get { return "G_MOVEMEM"; } }

        public string CommandDesc //Copied from CloudModding
        { get { return "Change block of memory in DMEM"; } }

        public enum G_MV_INDEX
        {
            G_MV_MMTX = 0x02,
            G_MV_PMTX = 0x06,
            G_MV_VIEWPORT = 0x08,
            G_MV_LIGHT = 0x0A,
            G_MV_POINT = 0x0C,
            G_MV_MATRIX = 0x0E
        }

        public byte Size;
        public byte Offset;
        public G_MV_INDEX MemoryIndex;
        public uint MemAddress;

        public bool IsValid { get; private set; }

        public F3DZEX_G_MoveMem(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)((((Size - 1) / 8) & 0x1F) << 3),
                    Offset, (byte)MemoryIndex, MemAddress);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                Size = (byte)((ByteHelper.ReadByte(value, 1) >> 3) * 8 + 1);
                Offset = ByteHelper.ReadByte(value, 2);
                MemoryIndex = (G_MV_INDEX)ByteHelper.ReadByte(value, 3);
                MemAddress = ByteHelper.ReadUInt(value, 4);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
