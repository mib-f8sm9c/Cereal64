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
    public class F3DEX_G_SetConvert : F3DEXCommand
    {
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DEXIDTypeConverter))]
        public override F3DEXCommandID CommandID
        { get { return F3DEXCommandID.F3DEX_G_SETCONVERT; } }
        
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_SETCONVERT"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Set conversion factors for YUV to RGB conversion";
        
        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("K0 term of conversion matrix"),
        TypeConverter(typeof(UInt16HexTypeConverter))]
        public ushort K0 { get { return _k0; } set { _k0 = value; Updated(); } }
        private ushort _k0;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("K1 term of conversion matrix"),
        TypeConverter(typeof(UInt16HexTypeConverter))]
        public ushort K1 { get { return _k1; } set { _k1 = value; Updated(); } }
        private ushort _k1;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("K2 term of conversion matrix"),
        TypeConverter(typeof(UInt16HexTypeConverter))]
        public ushort K2 { get { return _k2; } set { _k2 = value; Updated(); } }
        private ushort _k2;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("K3 term of conversion matrix"),
        TypeConverter(typeof(UInt16HexTypeConverter))]
        public ushort K3 { get { return _k3; } set { _k3 = value; Updated(); } }
        private ushort _k3;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("K4 term of conversion matrix"),
        TypeConverter(typeof(UInt16HexTypeConverter))]
        public ushort K4 { get { return _k4; } set { _k4 = value; Updated(); } }
        private ushort _k4;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("K5 term of conversion matrix"),
        TypeConverter(typeof(UInt16HexTypeConverter))]
        public ushort K5 { get { return _k5; } set { _k5 = value; Updated(); } }
        private ushort _k5;
        
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DEX_G_SetConvert(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((((uint)K0) & 0x01FF) << 13) |
                                ((((uint)K1) & 0x01FF) << 4) |
                                ((((uint)K2) >> 5) & 0x000F));
                uint secondHalf = (uint)(((uint)K2) << 27 |
                                ((((uint)K3) & 0x01FF) << 18) |
                                ((((uint)K4) & 0x01FF) << 9) |
                                (((uint)K5) & 0x01FF));

                return ByteHelper.CombineIntoBytes(firstHalf, secondHalf);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                K0 = (ushort)((ByteHelper.ReadUShort(value, 1) >> 5) & 0x01FF);
                K1 = (ushort)((ByteHelper.ReadUShort(value, 2) >> 4) & 0x01FF);
                K2 = (ushort)((ByteHelper.ReadUShort(value, 3) >> 3) & 0x01FF);
                K3 = (ushort)((ByteHelper.ReadUShort(value, 4) >> 2) & 0x01FF);
                K4 = (ushort)((ByteHelper.ReadUShort(value, 5) >> 1) & 0x01FF);
                K5 = (ushort)(ByteHelper.ReadUShort(value, 6) & 0x01FF);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
