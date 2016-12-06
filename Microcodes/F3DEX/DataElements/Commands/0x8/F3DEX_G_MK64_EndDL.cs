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
    public class F3DEX_G_MK64_EndDL : F3DEXCommand
    {
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DEXIDTypeConverter))]
        public override F3DEXCommandID CommandID
        { get { return F3DEXCommandID.F3DEX_G_MK64_ENDDL; } }
        
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_MK64_ENDDL"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc
        { get { return _commandDesc; } }
        private const string _commandDesc = "Custom-made entry for Mario Kart 64, presumably ending the current DL";

        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DEX_G_MK64_EndDL(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)0x00, (byte)0x00, (byte)0x00,
                    (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
