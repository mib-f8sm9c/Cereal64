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
    public class F3DEX_G_LoadBlock : F3DEXCommand
    {
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DEXIDTypeConverter))]
        public override F3DEXCommandID CommandID
        { get { return F3DEXCommandID.F3DEX_G_LOADBLOCK; } }
        
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_LOADBLOCK"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Load texture into TMEM as a continuous stream";

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Upper-left corner of texture to load, S-axis")]
        public qushort ULS { get { return _uls; } set { _uls = value; Updated(); } } //10.2 fixed point
        private qushort _uls;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Upper-left corner of texture to load, T-axis")]
        public qushort ULT { get { return _ult; } set { _ult = value; Updated(); } } //10.2 fixed point
        private qushort _ult;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Tile descriptor to load into"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Tile { get { return _tile; } set { _tile = value; Updated(); } }
        private byte _tile;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Number of texels to load to TMEM")]
        public ushort Texels { get { return _texels; } set { _texels = value; Updated(); } }
        private ushort _texels;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Change in T-axis per scanline")]
        public qushort DXT { get { return _dxt; } set { _dxt = value; Updated(); } } //1.11 fixed point
        private qushort _dxt;

        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DEX_G_LoadBlock(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((((uint)ULS.RawValue) & 0x0FFF) << 12) |
                                ((uint)ULT.RawValue) & 0x0FFF);
                uint secondHalf = (uint)(((((uint)Tile) & 0x0F) << 24) |
                                ((((uint)Texels - 1) & 0x0FFF) << 12) |
                                ((uint)DXT.RawValue) & 0x0FFF);
                return ByteHelper.CombineIntoBytes(firstHalf, secondHalf);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                ULS = new qushort("10.2", (ushort)(ByteHelper.ReadUShort(value, 1) >> 4));
                ULT = new qushort("10.2", (ushort)(ByteHelper.ReadUShort(value, 2) & 0x0FFF));
                Tile = (byte)(ByteHelper.ReadByte(value, 4) & 0x0F);
                Texels = (ushort)((ByteHelper.ReadUShort(value, 5) >> 4) + 1);
                DXT = new qushort("1.11", (ushort)(ByteHelper.ReadUShort(value, 6) & 0x0FFF));

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
