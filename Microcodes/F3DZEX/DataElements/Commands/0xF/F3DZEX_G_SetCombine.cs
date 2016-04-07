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
    public class F3DZEX_G_SetCombine : F3DZEXCommand
    {
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public override F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_SETCOMBINE; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_SETCOMBINE"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Set color combiner parameters";
            
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Color 'a' value, first cycle"),
        TypeConverter(typeof(ByteArrayHexTypeConverter))]
        public byte a0 { get { return _a0; } set { _a0 = value; Updated(); } }
        private byte _a0;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Color 'c' value, first cycle"),
        TypeConverter(typeof(ByteArrayHexTypeConverter))]
        public byte c0 { get { return _c0; } set { _c0 = value; Updated(); } }
        private byte _c0;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Alpha 'a' value, first cycle"),
        TypeConverter(typeof(ByteArrayHexTypeConverter))]
        public byte Aa0 { get { return _aa0; } set { _aa0 = value; Updated(); } }
        private byte _aa0;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Alpha 'c' value, first cycle"),
        TypeConverter(typeof(ByteArrayHexTypeConverter))]
        public byte Ac0 { get { return _ac0; } set { _ac0 = value; Updated(); } }
        private byte _ac0;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Color 'a' value, second cycle"),
        TypeConverter(typeof(ByteArrayHexTypeConverter))]
        public byte a1 { get { return _a1; } set { _a1 = value; Updated(); } }
        private byte _a1;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Color 'c' value, second cycle"),
        TypeConverter(typeof(ByteArrayHexTypeConverter))]
        public byte c1 { get { return _c1; } set { _c1 = value; Updated(); } }
        private byte _c1;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Color 'b' value, first cycle"),
        TypeConverter(typeof(ByteArrayHexTypeConverter))]
        public byte b0 { get { return _b0; } set { _b0 = value; Updated(); } }
        private byte _b0;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Color 'b' value, second cycle"),
        TypeConverter(typeof(ByteArrayHexTypeConverter))]
        public byte b1 { get { return _b1; } set { _b1 = value; Updated(); } }
        private byte _b1;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Alpha 'a' value, second cycle"),
        TypeConverter(typeof(ByteArrayHexTypeConverter))]
        public byte Aa1 { get { return _aa1; } set { _aa1 = value; Updated(); } }
        private byte _aa1;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Alpha 'c' value, second cycle"),
        TypeConverter(typeof(ByteArrayHexTypeConverter))]
        public byte Ac1 { get { return _ac1; } set { _ac1 = value; Updated(); } }
        private byte _ac1;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Color 'd' value, first cycle"),
        TypeConverter(typeof(ByteArrayHexTypeConverter))]
        public byte d0 { get { return _d0; } set { _d0 = value; Updated(); } }
        private byte _d0;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Alpha 'b' value, first cycle"),
        TypeConverter(typeof(ByteArrayHexTypeConverter))]
        public byte Ab0 { get { return _ab0; } set { _ab0 = value; Updated(); } }
        private byte _ab0;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Alpha d' value, first cycle"),
        TypeConverter(typeof(ByteArrayHexTypeConverter))]
        public byte Ad0 { get { return _ad0; } set { _ad0 = value; Updated(); } }
        private byte _ad0;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Color 'd' value, second cycle"),
        TypeConverter(typeof(ByteArrayHexTypeConverter))]
        public byte d1 { get { return _d1; } set { _d1 = value; Updated(); } }
        private byte _d1;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Alpha 'b' value, second cycle"),
        TypeConverter(typeof(ByteArrayHexTypeConverter))]
        public byte Ab1 { get { return _ab1; } set { _ab1 = value; Updated(); } }
        private byte _ab1;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Alpha 'd' value, second cycle"),
        TypeConverter(typeof(ByteArrayHexTypeConverter))]
        public byte Ad1 { get { return _ad1; } set { _ad1 = value; Updated(); } }
        private byte _ad1;
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DZEX_G_SetCombine(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((((uint)a0) & 0x0F) << 20) |
                                ((((uint)c0) & 0x1F) << 15) |
                                ((((uint)Aa0) & 0x07) << 12) |
                                ((((uint)Ac0) & 0x07) << 9) |
                                ((((uint)a1) & 0x0F) << 5) |
                                (((uint)c1) & 0x1F));
                uint secondHalf = (uint)(((uint)b0) << 28 |
                                ((((uint)b1) & 0x0F) << 24) |
                                ((((uint)Aa1) & 0x07) << 21) |
                                ((((uint)Ac1) & 0x07) << 18) |
                                ((((uint)d0) & 0x07) << 15) |
                                ((((uint)Ab0) & 0x07) << 12) |
                                ((((uint)Ad0) & 0x07) << 9) |
                                ((((uint)d1) & 0x07) << 6) |
                                ((((uint)Ab1) & 0x07) << 3) |
                                (((uint)Ad1) & 0x07));
                return ByteHelper.CombineIntoBytes(firstHalf, secondHalf);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                a0 = (byte)((ByteHelper.ReadByte(value, 1) >> 4) & 0x0F);
                ushort temp = ByteHelper.ReadUShort(value, 1);
                c0 = (byte)((temp >> 7) & 0x001F);
                Aa0 = (byte)((temp >> 4) & 0x0007);
                Ac0 = (byte)((temp >> 1) & 0x0007);
                temp = ByteHelper.ReadUShort(value, 2);
                a1 = (byte)((temp >> 5) & 0x000F);
                c1 = (byte)(temp & 0x001F);
                byte tempByte = ByteHelper.ReadByte(value, 4);
                b0 = (byte)((tempByte >> 4) & 0x0F);
                b1 = (byte)(tempByte & 0x0F);
                tempByte = ByteHelper.ReadByte(value, 5);
                Aa1 = (byte)((tempByte >> 5) & 0x07);
                Ac1 = (byte)((tempByte >> 2) & 0x07);
                temp = ByteHelper.ReadUShort(value, 5);
                d0 = (byte)((temp >> 7) & 0x07);
                Ab0 = (byte)((temp >> 4) & 0x07);
                Ad0 = (byte)((temp >> 1) & 0x07);
                temp = ByteHelper.ReadUShort(value, 6);
                d1 = (byte)((temp >> 6) & 0x07);
                Ab1 = (byte)((temp >> 3) & 0x07);
                Ad1 = (byte)(temp & 0x07);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
