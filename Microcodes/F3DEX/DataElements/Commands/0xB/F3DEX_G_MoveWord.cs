using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;
using System.ComponentModel;
using Cereal64.Common.DataElements;

namespace Cereal64.Microcodes.F3DEX.DataElements.Commands
{
    public class F3DEX_G_MoveWord : F3DEXCommand
    {
        public enum G_MW_INDEX
        {
            G_MW_MATRIX = 0x00,
            G_MW_NUMLIGHT = 0x02,
            G_MW_CLIP = 0x04,
            G_MW_SEGMENT = 0x06,
            G_MW_FOG = 0x08,
            G_MW_LIGHTCOL = 0x0A,
            G_MW_FORCEMTX = 0x0C,
            G_MW_PERSPNORM = 0x0E,
        }

        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DEXIDTypeConverter))]
        public override F3DEXCommandID CommandID
        { get { return F3DEXCommandID.F3DEX_G_MOVEWORD; } }
        
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_MOVEWORD"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Change \"word\" (32 bits) of data in DMEM";

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Index into DMEM pointer table(?)")]
        public G_MW_INDEX WordIndex { get { return _wordIndex; } set { _wordIndex = value; Updated(); } }
        private G_MW_INDEX _wordIndex;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Offset from the indexed base address(?)"),
        TypeConverter(typeof(UInt16HexTypeConverter))]
        public ushort Offset { get { return _offset; } set { _offset = value; Updated(); } }
        private ushort _offset;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("New 32-bit value"),
        TypeConverter(typeof(UInt32HexTypeConverter))]
        public uint Data { get { return _data; } set { _data = value; Updated(); } }
        private uint _data;
        
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DEX_G_MoveWord(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)WordIndex, Offset,
                    Data);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                WordIndex = (G_MW_INDEX)ByteHelper.ReadByte(value, 1);
                Offset = ByteHelper.ReadUShort(value, 2);
                Data = ByteHelper.ReadUInt(value, 4);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
