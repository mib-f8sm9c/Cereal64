using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;
using System.ComponentModel;
using System.Drawing;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_SetPrimColor : N64DataElement, IF3DZEXCommand
    {
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_SETPRIMCOLOR; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public string CommandName
        { get { return "G_SETPRIMCOLOR"; } }
        
        [BrowsableAttribute(false)]
        public string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Set primitive color register";
            
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Minimum possible LOD value (clamped to this at minimum)"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte MinLevel { get; set; }
        
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Primitive LOD fraction for mipmap filtering"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte LodFrac { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Primitive color to be set")]
        public Color PrimitiveColor { get; set; }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public bool IsValid { get; private set; }

        public F3DZEX_G_SetPrimColor(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)0x00, MinLevel, LodFrac,
                    PrimitiveColor.R, PrimitiveColor.G, PrimitiveColor.B, PrimitiveColor.A);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;
                MinLevel = ByteHelper.ReadByte(value, 2);
                LodFrac = ByteHelper.ReadByte(value, 3);
                byte R = ByteHelper.ReadByte(value, 4);
                byte G = ByteHelper.ReadByte(value, 5);
                byte B = ByteHelper.ReadByte(value, 6);
                byte A = ByteHelper.ReadByte(value, 7);

                PrimitiveColor = Color.FromArgb(A, R, G, B);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
