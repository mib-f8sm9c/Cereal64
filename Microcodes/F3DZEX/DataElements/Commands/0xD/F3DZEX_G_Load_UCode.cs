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
    public class F3DZEX_G_Load_UCode : F3DZEXCommand
    {
        //NOTE: THIS CODE FOLLOWS A E1 COMMAND AND NEEDS IT TO FUNCTION

        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public override F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_LOAD_UCODE; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_LOAD_UCODE"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Jump or \"call\" another display list";

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Size of data section"),
        TypeConverter(typeof(UInt16HexTypeConverter))]
        public ushort DSize { get { return _dsize; } set { _dsize = value; Updated(); } }
        private ushort _dsize;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Start of text section"),
        TypeConverter(typeof(UInt32HexTypeConverter))]
        public uint TStart { get { return _tstart; } set { _tstart = value; Updated(); } }
        private uint _tstart;
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DZEX_G_Load_UCode(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)(0x00), DSize,
                    TStart);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                DSize = ByteHelper.ReadUShort(value, 2);
                TStart = ByteHelper.ReadUInt(value, 4);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
