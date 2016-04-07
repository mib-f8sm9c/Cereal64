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
    public class F3DZEX_G_Branch_Z : F3DZEXCommand
    {
        // NOTE: THIS COMMAND FOLLOWS AN E1 COMMAND

        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public override F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_BRANCH_Z; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_BRANCH_Z"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Branch to another display list if vertex is close enough to screen";

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Vertex buffer index of vertex to test"),
        TypeConverter(typeof(UInt16HexTypeConverter))]
        public ushort BufferIndex1 { get { return _bufferIndex1; } set { _bufferIndex1 = value; Updated(); } }
        private ushort _bufferIndex1;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Vertex buffer index of vertex to test"),
        TypeConverter(typeof(UInt16HexTypeConverter))]
        public ushort BufferIndex2 { get { return _bufferIndex2; } set { _bufferIndex2 = value; Updated(); } }
        private ushort _bufferIndex2;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Z value to test against"),
        TypeConverter(typeof(UInt32HexTypeConverter))]
        public uint ZVal { get { return _zval; } set { _zval = value; Updated(); } }
        private uint _zval;
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DZEX_G_Branch_Z(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((((uint)BufferIndex1 * 5) & 0x0FFF) << 12) |
                                ((uint)(BufferIndex2 * 2) & 0x0FFF));
                return ByteHelper.CombineIntoBytes(firstHalf, ZVal);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                BufferIndex1 = (ushort)((ByteHelper.ReadUShort(value, 1) >> 4) / 5);
                BufferIndex2 = (ushort)((ByteHelper.ReadUShort(value, 2) & 0xFFF) / 2);
                ZVal = ByteHelper.ReadUInt(value, 4);

                if (BufferIndex1 != BufferIndex2)
                    return; //Invalid, the two indices need to be equal

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
