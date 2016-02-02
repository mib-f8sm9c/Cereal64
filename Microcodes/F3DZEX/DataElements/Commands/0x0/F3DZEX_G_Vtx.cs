using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;
using System.ComponentModel;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_Vtx : N64DataElement, IF3DZEXCommand
    {
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_VTX; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public string CommandName
        { get { return "G_VTX"; } }
        
        [BrowsableAttribute(false)]
        public string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Loads vertices to RSP";
        
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Number of vertices to load"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte VertexCount { get; set; }

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Index of vertex buffer to begin storing vertices to"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte TargetBufferIndex { get; set; } //Cloud Modding suggests subtracting VertexCount from this

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Address of vertices"),
        TypeConverter(typeof(UInt32HexTypeConverter))]
        public uint VertexSourceAddress { get; set; }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public bool IsValid { get; private set; }

        public F3DZEX_G_Vtx(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                ((uint)VertexCount) << 12 |
                                (byte)(((TargetBufferIndex) & 0x7F) * 2));
                return ByteHelper.CombineIntoBytes(firstHalf, VertexSourceAddress);
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;
                VertexCount = (byte)(ByteHelper.ReadUShort(value, 1) >> 4);
                TargetBufferIndex = (byte)(ByteHelper.ReadByte(value, 3) / 2);
                VertexSourceAddress = ByteHelper.ReadUInt(value, 4);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
