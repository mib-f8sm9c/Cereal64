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
    public class F3DZEX_G_ModifyVtx : F3DZEXCommand
    {
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public enum OverwriteType
        {
            G_MWO_POINT_RGBA = 0x10,
            G_MWO_POINT_ST = 0x14,
            G_MWO_POINT_XYSCREEN = 0x18,
            G_MWO_POINT_ZSCREEN = 0x1C
        }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_MODIFYVTX; } }
        
        [BrowsableAttribute(false)]
        public override string CommandName
        { get { return "G_MODIFYVTX"; } }

        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Modifies a portion of vertex attributes in RSP";

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Enumerated set of values specifying what to change")]
        public OverwriteType Type { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Vertex buffer index of vertex to modify"),
        TypeConverter(typeof(UInt16HexTypeConverter))]
        public ushort TargetBufferIndex { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("New value to insert"),
        TypeConverter(typeof(UInt32HexTypeConverter))]
        public uint NewValue { get; set; }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DZEX_G_ModifyVtx(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((uint)Type) << 16 |
                                (uint)(TargetBufferIndex * 2));
                return ByteHelper.CombineIntoBytes(firstHalf, NewValue);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;

                byte type = ByteHelper.ReadByte(value, 1);
                if (!Enum.IsDefined(typeof(OverwriteType), (int)type))
                    return;

                Type = (OverwriteType)type;
                TargetBufferIndex = (ushort)(ByteHelper.ReadUShort(value, 2) / 2);
                NewValue = ByteHelper.ReadUInt(value, 4);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
