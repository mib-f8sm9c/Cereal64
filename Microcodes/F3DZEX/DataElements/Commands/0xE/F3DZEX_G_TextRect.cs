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
    public class F3DZEX_G_TexRect : F3DZEXCommand
    {
        //THIS COMMAND IS FOLLOWED BY 0xE1 AND 0xF1 COMMANDS THAT PROVIDE EXTRA DATA
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public override F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_TEXRECT; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_TEXRECT"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Draw textured rectangle onto screen";
            
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Lower-right corner X coordinate")]
        public qushort LRX { get { return _lrx; } set { _lrx = value; Updated(); } }
        private qushort _lrx;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Lower-right corner Y coordinate")]
        public qushort LRY { get { return _lry; } set { _lry = value; Updated(); } }
        private qushort _lry;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Tile descriptor to use for rectangle"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Tile { get { return _tile; } set { _tile = value; Updated(); } }
        private byte _tile;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Upper-left corner X coordinate")]
        public qushort ULX { get { return _ulx; } set { _ulx = value; Updated(); } }
        private qushort _ulx;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Upper-left corner Y coordinate")]
        public qushort ULY { get { return _uly; } set { _uly = value; Updated(); } }
        private qushort _uly;
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DZEX_G_TexRect(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {

                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((((uint)LRX.RawValue) & 0x0FFF) << 12) |
                                (((uint)LRY.RawValue) & 0x0FFF));
                uint secondHalf = (uint)((uint)(Tile & 0x0F) << 24 |
                                ((((uint)ULX.RawValue) & 0x0FFF) << 12) |
                                (((uint)ULY.RawValue) & 0x0FFF));
                return ByteHelper.CombineIntoBytes(firstHalf, secondHalf);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                LRX = new qushort("10.2", (ushort)((ByteHelper.ReadUShort(value, 1) >> 4) & 0x0FFF));
                LRY = new qushort("10.2", (ushort)(ByteHelper.ReadUShort(value, 2) & 0x0FFF));

                Tile = (byte)(ByteHelper.ReadByte(value, 4) & 0x0F);
                ULX = new qushort("10.2", (ushort)((ByteHelper.ReadUShort(value, 5) >> 4) & 0x0FFF));
                ULY = new qushort("10.2", (ushort)(ByteHelper.ReadUShort(value, 6) & 0x0FFF));

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
