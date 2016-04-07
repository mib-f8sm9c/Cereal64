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
    public class F3DZEX_G_SetTileSize : F3DZEXCommand
    {
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public override F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_SETTILESIZE; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_SETTILESIZE"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Set dimensions of texture";
            
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Upper-left corner of texture to load, S-axis")]
        public qushort ULS { get { return _uls; } set { _uls = value; Updated(); } } //10.2 Format
        private qushort _uls;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Upper-left corner of texture to load, T-axis")]
        public qushort ULT { get { return _ult; } set { _ult = value; Updated(); } } //10.2 Format
        private qushort _ult;
        
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Tile descriptor to modify"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Tile { get { return _tile; } set { _tile = value; Updated(); } }
        private byte _tile;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Lower-right corner of texture to load, S-axis")]
        public qushort LRS { get { return _lrs; } set { _lrs = value; Updated(); } }  //10.2 Format
        private qushort _lrs;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Lower-right corner of texture to load, T-axis")]
        public qushort LRT { get { return _lrt; } set { _lrt = value; Updated(); } } //10.2 Format
        private qushort _lrt;
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DZEX_G_SetTileSize(int index, byte[] rawBytes)
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
                                ((((uint)LRS.RawValue) & 0x0FFF) << 12) |
                                ((uint)LRT.RawValue) & 0x0FFF);
                return ByteHelper.CombineIntoBytes(firstHalf, secondHalf);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                ULS = new qushort("10.2", (ushort)(ByteHelper.ReadUShort(value, 1) >> 4));
                ULT = new qushort("10.2", (ushort)(ByteHelper.ReadUShort(value, 2) & 0x0FFF));
                Tile = (byte)(ByteHelper.ReadByte(value, 4) & 0x0F);
                LRS = new qushort("10.2", (ushort)(ByteHelper.ReadUShort(value, 5) >> 4));
                LRT = new qushort("10.2", (ushort)(ByteHelper.ReadUShort(value, 6) & 0x0FFF));

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
