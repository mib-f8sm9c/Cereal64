using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cereal64.Common;
using Cereal64.Common.Utils;
using System.ComponentModel;
using Cereal64.Common.DataElements;
using Cereal64.Common.Rom;

namespace Cereal64.Microcodes.F3DEX.DataElements.Commands
{
    //NOTE: I SWITCHED THE VERTEX COUNT & TARGET BUFFER INDEX, SHOULD BE OKAY, BUT THIS IS FOR MARIO KART!!!

    public class F3DEX_G_Vtx : F3DEXCommand
    {
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DEXIDTypeConverter))]
        public override F3DEXCommandID CommandID
        { get { return F3DEXCommandID.F3DEX_G_VTX; } }
        
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_VTX"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Loads vertices to RSP";
        
        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Number of vertices to load"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte VertexCount { get { return _vertexCount; } set { _vertexCount = value; Updated(); } }
        private byte _vertexCount;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Index of vertex buffer to begin storing vertices to"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte TargetBufferIndex { get { return _targetBufferIndex; } set { _targetBufferIndex = value; Updated(); } }
        private byte _targetBufferIndex;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Address of vertices")]
        public DmaAddress VertexSourceAddress { get { return _vertexSourceAddress; } set { _vertexSourceAddress = value; Updated(); } }
        private DmaAddress _vertexSourceAddress;
        
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DEX_G_Vtx(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                uint firstHalf = (uint)(((uint)CommandID) << 24 |
                                (((uint)TargetBufferIndex) & 0x7F) << 17 |
                                (((uint)VertexCount) & 0x3F) << 10);
                return ByteHelper.CombineIntoBytes(firstHalf, VertexSourceAddress.GetAsUInt());
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;
                TargetBufferIndex = (byte)(ByteHelper.ReadByte(value, 1) >> 1);
                VertexCount = (byte)(ByteHelper.ReadByte(value, 2) >> 2);
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
