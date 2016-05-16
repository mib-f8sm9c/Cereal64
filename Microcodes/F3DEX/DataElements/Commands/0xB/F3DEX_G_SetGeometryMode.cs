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
    public class F3DEX_G_SetGeometryMode : F3DEXCommand
    {
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DEXIDTypeConverter))]
        public override F3DEXCommandID CommandID
        { get { return F3DEXCommandID.F3DEX_G_SETGEOMETRYMODE; } }
        
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_SETGEOMETRYMODE"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Configures RSP Geometry Mode";

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Geometry mode bits to clear"),
        TypeConverter(typeof(UInt32HexTypeConverter))]
        public uint ClearBits { get { return _clearBits; } set { _clearBits = value; Updated(); } }
        private uint _clearBits;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Geometry mode bits to set"),
        TypeConverter(typeof(UInt32HexTypeConverter))]
        public uint SetBits { get { return _setBits; } set { _setBits = value; Updated(); } }
        private uint _setBits;

        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DEX_G_SetGeometryMode(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((((uint)CommandID) << 24) | (~ClearBits),
                    SetBits);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                ClearBits = ~(ByteHelper.ReadUInt(value, 0) & 0x00FFFFFF);
                SetBits = ByteHelper.ReadUInt(value, 4);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
