using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;
using System.ComponentModel;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_MoveWord : N64DataElement, IF3DZEXCommand
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

        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_MOVEWORD; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public string CommandName
        { get { return "G_MOVEWORD"; } }
        
        [BrowsableAttribute(false)]
        public string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Change \"word\" (32 bits) of data in DMEM";

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Index into DMEM pointer table(?)")]
        public G_MW_INDEX WordIndex { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Offset from the indexed base address(?)"),
        TypeConverter(typeof(UInt16HexTypeConverter))]
        public ushort Offset { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("New 32-bit value"),
        TypeConverter(typeof(UInt32HexTypeConverter))]
        public uint Data { get; set; }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public bool IsValid { get; private set; }

        public F3DZEX_G_MoveWord(int index, byte[] rawBytes)
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
