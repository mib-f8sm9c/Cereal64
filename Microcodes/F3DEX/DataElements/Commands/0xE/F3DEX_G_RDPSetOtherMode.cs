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
    public class F3DEX_G_RDPSetOtherMode : F3DEXCommand
    {
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DEXIDTypeConverter))]
        public override F3DEXCommandID CommandID
        { get { return F3DEXCommandID.F3DEX_G_RDPSETOTHERMODE; } }
        
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_RDPSETOTHERMODE"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Set state of the RDP Other Modes bits, both halves (combined E2 and E3).";

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Set the other mode higher word bits"),
        TypeConverter(typeof(UInt32HexTypeConverter))]
        public uint OModeH { get { return _omodeH; } set { _omodeH = value; Updated(); } }
        private uint _omodeH;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Set the other mode lower word bits"),
        TypeConverter(typeof(UInt32HexTypeConverter))]
        public uint OModeL { get { return _omodeL; } set { _omodeL = value; Updated(); } }
        private uint _omodeL;

        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DEX_G_RDPSetOtherMode(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((((uint)CommandID) << 24) | (OModeH & 0x00FFFFFF),
                    OModeL);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                OModeH = ByteHelper.ReadUInt(value, 0) & 0x00FFFFFF;
                OModeL = ByteHelper.ReadUInt(value, 4);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
