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
    public class F3DZEX_G_DMA_IO : F3DZEXCommand
    {
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_DMA_IO; } }

        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_DMA_IO"; } }

        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "DMA transfer between main RAM and RCP DMEM (or IMEM)";

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("If false, DMEM/IMEM -> DRAM. If true, DRAM -> DMEM/IMEM.")]
        public bool Flag { get { return _flag; } set { _flag = value; Updated(); } }
        private bool _flag;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Address in DMEM/IMEM(?)")]
        public ushort DMem { get { return _dmem; } set { _dmem = value; Updated(); } }
        private ushort _dmem;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("(Presumably) size of data to transfer"),
        TypeConverter(typeof(UInt16HexTypeConverter))]
        public ushort Size { get { return _size; } set { _size = value; Updated(); } }
        private ushort _size;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("DRAM address "),
        TypeConverter(typeof(UInt32HexTypeConverter))]
        public uint DRam { get { return _dram; } set { _dram = value; Updated(); } }
        private uint _dram;

        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DZEX_G_DMA_IO(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((uint)(Flag ? 1 : 0)) << 23 |
                                (((uint)DMem / 8) & 0x3FF) << 13 |
                                (uint)(Size - 1));
                return ByteHelper.CombineIntoBytes(firstHalf, DRam);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;
                Flag = ((ByteHelper.ReadByte(value, 1) >> 7) & 0x01) == 0x01;
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
