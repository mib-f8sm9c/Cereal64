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
    //NOTE: Not sure, but the ushort values might be 4x what's in the raw data
    public class F3DEX_G_FillRect : F3DEXCommand
    {
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DEXIDTypeConverter))]
        public override F3DEXCommandID CommandID
        { get { return F3DEXCommandID.F3DEX_G_FILLRECT; } }
        
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_FILLRECT"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Draw solid color rectangle to screen";
            
        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Lower-right corner X coordinate ")]
        public qushort LRX { get { return _lrx; } set { _lrx = value; Updated(); } }//10.2 fixed point
        private qushort _lrx;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Lower-right corner Y coordinate ")]
        public qushort LRY { get { return _lry; } set { _lry = value; Updated(); } }//10.2 fixed point
        private qushort _lry;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Upper-left corner X coordinate ")]
        public qushort ULX { get { return _ulx; } set { _ulx = value; Updated(); } }//10.2 fixed point
        private qushort _ulx;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Upper-left corner Y coordinate ")]
        public qushort ULY { get { return _uly; } set { _uly = value; Updated(); } }//10.2 fixed point
        private qushort _uly;

        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DEX_G_FillRect(int index, byte[] rawBytes)
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
