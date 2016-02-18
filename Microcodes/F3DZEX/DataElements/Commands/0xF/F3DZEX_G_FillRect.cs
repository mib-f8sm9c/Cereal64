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
    //NOTE: Not sure, but the ushort values might be 4x what's in the raw data
    public class F3DZEX_G_FillRect : F3DZEXCommand
    {
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public override F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_FILLRECT; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_FILLRECT"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Draw solid color rectangle to screen";
            
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Lower-right corner X coordinate ")]
        public qushort LRX { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Lower-right corner Y coordinate ")]
        public qushort LRY { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Upper-left corner X coordinate ")]
        public qushort ULX { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Upper-left corner Y coordinate ")]
        public qushort ULY{ get; set; }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DZEX_G_FillRect(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((((uint)LRX.RawValue) & 0x0FFF) << 12) |
                                ((uint)LRY.RawValue) & 0x0FFF);
                uint secondHalf = (uint)(((((uint)ULX.RawValue) & 0x0FFF) << 12) |
                                ((uint)ULY.RawValue) & 0x0FFF);
                return ByteHelper.CombineIntoBytes(firstHalf, secondHalf);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                LRX = new qushort("10.2", (ushort)(ByteHelper.ReadUShort(value, 1) >> 4));
                LRY = new qushort("10.2", (ushort)(ByteHelper.ReadUShort(value, 2) & 0x0FFF));

                ULX = new qushort("10.2", (ushort)(ByteHelper.ReadUShort(value, 5) >> 4));
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
