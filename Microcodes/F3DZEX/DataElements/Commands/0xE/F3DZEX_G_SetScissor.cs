using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;
using System.ComponentModel;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_SetScissor : N64DataElement, IF3DZEXCommand
    {
        public enum ScissorInterlace
        {
            G_SC_NON_INTERLACE = 0x00,
            G_SC_EVEN_INTERLACE = 0x02,
            G_SC_ODD_INTERLACE = 0x03
        }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_SETSCISSOR; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public string CommandName
        { get { return "G_SETSCISSOR"; } }
        
        [BrowsableAttribute(false)]
        public string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Set scissoring rectangle";

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Upper-left corner X coordinate")]
        public ushort ULX { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Upper-left corner Y coordinate")]
        public ushort ULY { get; set; }
        
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Interpolation mode setting")]
        public ScissorInterlace Mode { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Lower-right corner X coordinate")]
        public ushort LRX { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Lower-right corner Y coordinate")]
        public ushort LRY { get; set; }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public bool IsValid { get; private set; }

        public F3DZEX_G_SetScissor(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {

                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((((uint)ULX) & 0x0FFF) << 12) |
                                (((uint)ULY) & 0x0FFF));
                uint secondHalf = (uint)((((uint)Mode) & 0x0F) << 28 |
                                ((((uint)LRX) & 0x0FFF) << 12) |
                                (((uint)LRY) & 0x0FFF));
                return ByteHelper.CombineIntoBytes(firstHalf, secondHalf);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                ULX = (ushort)((ByteHelper.ReadUShort(value, 1) >> 4) & 0x0FFF);
                ULY = (ushort)(ByteHelper.ReadUShort(value, 2) & 0x0FFF);

                Mode = (ScissorInterlace)((ByteHelper.ReadByte(value, 4) >> 4) & 0x0F);
                LRX = (ushort)((ByteHelper.ReadUShort(value, 5) >> 4) & 0x0FFF);
                LRY = (ushort)(ByteHelper.ReadUShort(value, 6) & 0x0FFF);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
