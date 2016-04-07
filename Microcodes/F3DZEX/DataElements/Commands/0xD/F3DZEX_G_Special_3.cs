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
    public class F3DZEX_G_Special_3 : F3DZEXCommand
    {
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public override F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_SPECIAL_3; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_SPECIAL_3"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Explicitly reserved opcode... or is it?";

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Unknown"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Unknown1 { get { return _unknown1; } set { _unknown1 = value; Updated(); } }
        private byte _unknown1;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Unknown"),
        TypeConverter(typeof(UInt16HexTypeConverter))]
        public ushort Unknown2 { get { return _unknown2; } set { _unknown2 = value; Updated(); } }
        private ushort _unknown2;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Unknown"),
        TypeConverter(typeof(UInt32HexTypeConverter))]
        public uint Unknown3 { get { return _unknown3; } set { _unknown3 = value; Updated(); } }
        private uint _unknown3;
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DZEX_G_Special_3(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, Unknown1, Unknown2, Unknown3);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;
                Unknown1 = ByteHelper.ReadByte(value, 1);
                Unknown2 = ByteHelper.ReadUShort(value, 2);
                Unknown3 = ByteHelper.ReadUInt(value, 4);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
