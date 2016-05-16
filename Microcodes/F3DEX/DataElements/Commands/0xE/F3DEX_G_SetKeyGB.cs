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
    public class F3DEX_G_SetKeyGB : F3DEXCommand
    {
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DEXIDTypeConverter))]
        public override F3DEXCommandID CommandID
        { get { return F3DEXCommandID.F3DEX_G_SETKEYGB; } }
        
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_SETKEYGB"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Set green and blue components of chroma key";

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Scaled width of half the key window for green")]
        public qushort WidthG { get { return _widthG; } set { _widthG = value; Updated(); } } //Actually 4.8 float
        private qushort _widthG;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Scaled width of half the key window for blue")]
        public qushort WidthB { get { return _widthB; } set { _widthB = value; Updated(); } } //Actually 4.8 float
        private qushort _widthB;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Intensity of active key for green"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte CenterG { get { return _centerG; } set { _centerG = value; Updated(); } }
        private byte _centerG;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Reciprocal of size of soft edge, normalized to 0..0xFF, for green"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte ScaleG { get { return _scaleG; } set { _scaleG = value; Updated(); } }
        private byte _scaleG;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Intensity of active key for blue"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte CenterB { get { return _centerB; } set { _centerB = value; Updated(); } }
        private byte _centerB;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Reciprocal of size of soft edge, normalized to 0..0xFF, for blue"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte ScaleB { get { return _scaleB; } set { _scaleB = value; Updated(); } }
        private byte _scaleB;
        
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DEX_G_SetKeyGB(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((((uint)WidthG.RawValue) & 0x0FFF) << 12) |
                                (((uint)WidthB.RawValue) & 0x0FFF));
                return ByteHelper.CombineIntoBytes(firstHalf, 
                    CenterG, ScaleG, CenterB, ScaleB);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                WidthG = new qushort("4.8", (ushort)((ByteHelper.ReadUShort(value, 1) >> 4) & 0x0FFF));
                WidthB = new qushort("4.8", (ushort)(ByteHelper.ReadUShort(value, 2) & 0x0FFF));

                CenterG = ByteHelper.ReadByte(value, 4);
                ScaleG = ByteHelper.ReadByte(value, 5);
                CenterB = ByteHelper.ReadByte(value, 6);
                ScaleB = ByteHelper.ReadByte(value, 7);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
