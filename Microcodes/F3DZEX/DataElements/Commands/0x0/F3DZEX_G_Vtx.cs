using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;
using System.ComponentModel;
using Cereal64.Common.DataElements;
using Cereal64.Common.Rom;

namespace Cereal64.Microcodes.F3DZEX.DataElements.Commands
{
    public class F3DZEX_G_Vtx : F3DZEXCommand
    {
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DZEXIDTypeConverter))]
        public override F3DZEXCommandID CommandID
        { get { return F3DZEXCommandID.F3DZEX_G_VTX; } }
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_VTX"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Loads vertices to RSP";
        
        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Number of vertices to load"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte VertexCount { get { return _vertexCount; } set { _vertexCount = value; Updated(); } }
        private byte _vertexCount;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Index of vertex buffer to begin storing vertices to"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte TargetBufferIndex { get { return _targetBufferIndex; } set { _targetBufferIndex = value; Updated(); } }//Cloud Modding suggests subtracting VertexCount from this
        private byte _targetBufferIndex;

        [CategoryAttribute("F3DZEX Settings"),
        DescriptionAttribute("Address of vertices")]
        public DmaAddress VertexSourceAddress { get { return _vertexSourceAddress; } set { _vertexSourceAddress = value; Updated(); } }
        private DmaAddress _vertexSourceAddress;
        
        [CategoryAttribute("F3DZEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

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
                                (byte)(((TargetBufferIndex + VertexCount) & 0x7F) * 2));
                return ByteHelper.CombineIntoBytes(firstHalf, VertexSourceAddress.GetAsUInt());
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;
                VertexCount = (byte)(ByteHelper.ReadUShort(value, 1) >> 4);
                TargetBufferIndex = (byte)(ByteHelper.ReadByte(value, 3) / 2 - VertexCount);
                VertexSourceAddress = new DmaAddress(ByteHelper.ReadUInt(value, 4));

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
