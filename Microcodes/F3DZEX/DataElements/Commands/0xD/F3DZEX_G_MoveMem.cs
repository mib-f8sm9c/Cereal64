using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;
using System.ComponentModel;
using Cereal64.Common.DataElements;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_MoveMem : F3DZEXCommand
    {
        public enum G_MV_INDEX
        {
            G_MV_MMTX = 0x02,
            G_MV_PMTX = 0x06,
            G_MV_VIEWPORT = 0x08,
            G_MV_LIGHT = 0x0A,
            G_MV_POINT = 0x0C,
            G_MV_MATRIX = 0x0E
        }

        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public override F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_MOVEMEM; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_MOVEMEM"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Change block of memory in DMEM";

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Size in bytes of memory to be moved"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Size { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Offset from indexed base address"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Offset { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Index into table of DMEM addresses")]
        public G_MV_INDEX MemoryIndex { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("RAM address of memory"),
        TypeConverter(typeof(UInt32HexTypeConverter))]
        public uint MemAddress { get; set; }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

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
