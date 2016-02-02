using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;
using System.ComponentModel;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_SetKeyR : N64DataElement, IF3DZEXCommand
    {
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_SETKEYR; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public string CommandName
        { get { return "G_SETKEYR"; } }
        
        [BrowsableAttribute(false)]
        public string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Set red component of chroma key";
        
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Scaled width of half the key window for blue"),
        TypeConverter(typeof(UInt16HexTypeConverter))]
        public ushort WidthR { get; set; } //THIS IS AN UNSIGNED FIXED POINT 4.8 NUBMER
        
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Intensity of active key for red"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte CenterR { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Reciprocal of size of soft edge, normalized to 0..0xFF, for red"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte ScaleR { get; set; }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public bool IsValid { get; private set; }

        public F3DZEX_G_SetKeyR(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)0x00, (byte)0x00, (byte)0x00,
                    (ushort)(WidthR & 0x0FFF), CenterR, ScaleR);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                WidthR = (ushort)(ByteHelper.ReadUShort(value, 4) & 0x0FFF);
                CenterR = ByteHelper.ReadByte(value, 6);
                ScaleR = ByteHelper.ReadByte(value, 7);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
