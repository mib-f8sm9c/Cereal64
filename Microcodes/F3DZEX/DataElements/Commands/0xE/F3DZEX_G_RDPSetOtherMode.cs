using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;
using System.ComponentModel;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_RDPSetOtherMode : N64DataElement, IF3DZEXCommand
    {
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_RDPSETOTHERMODE; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public string CommandName
        { get { return "G_RDPSETOTHERMODE"; } }
        
        [BrowsableAttribute(false)]
        public string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Set state of the RDP Other Modes bits, both halves (combined E2 and E3).";

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Set the other mode higher word bits"),
        TypeConverter(typeof(UInt32HexTypeConverter))]
        public uint OModeH { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Set the other mode lower word bits"),
        TypeConverter(typeof(UInt32HexTypeConverter))]
        public uint OModeL { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public bool IsValid { get; private set; }

        public F3DZEX_G_RDPSetOtherMode(int index, byte[] rawBytes)
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
