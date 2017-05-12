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
    public class F3DEX_G_Tri1 : F3DEXCommand
    {
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc),
        TypeConverter(typeof(F3DEXIDTypeConverter))]
        public override F3DEXCommandID CommandID
        { get { return F3DEXCommandID.F3DEX_G_TRI1; } }
        
        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute(_commandDesc)]
        public override string CommandName
        { get { return "G_TRI1"; } }
        
        [BrowsableAttribute(false)]
        public override string CommandDesc //Copied from CloudModding
        { get { return _commandDesc; } }
        private const string _commandDesc = "Draw a triangle";

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Vertex buffer index of first vertex of the triangle"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Vertex1 { get { return _vertex1; } set { _vertex1 = value; Updated(); } }
        private byte _vertex1;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Vertex buffer index of second vertex of the triangle"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Vertex2 { get { return _vertex2; } set { _vertex2 = value; Updated(); } }
        private byte _vertex2;

        [CategoryAttribute("F3DEX Settings"),
        DescriptionAttribute("Vertex buffer index of third vertex of the triangle"),
        TypeConverter(typeof(ByteHexTypeConverter))]
        public byte Vertex3 { get { return _vertex3; } set { _vertex3 = value; Updated(); } }
        private byte _vertex3;

        [BrowsableAttribute(false)]
        public Vertex Vertex1Reference { get; set; }

        [BrowsableAttribute(false)]
        public Vertex Vertex2Reference { get; set; }

        [BrowsableAttribute(false)]
        public Vertex Vertex3Reference { get; set; }

        [CategoryAttribute("F3DEX Settings"),
        ReadOnlyAttribute(true),
        DescriptionAttribute("True if the command was loaded without errors")]
        public override bool IsValid { get; protected set; }

        public F3DEX_G_Tri1(int index, byte[] rawBytes)
            : base (index, rawBytes)
        {
        }

        public override byte[] RawData
        {
            get
            {
                return ByteHelper.CombineIntoBytes((byte)CommandID, (byte)0x00, (byte)0x00, (byte)0x00,
                    (byte)0x00, (byte)(Vertex1 * 2), (byte)(Vertex2 * 2), (byte)(Vertex3 * 2));
            }
            set
            {
                IsValid = false;

                if (value.Length < 8 || value[0] != (byte)CommandID) return;
                Vertex1 = (byte)(ByteHelper.ReadByte(value, 5) / 2);
                Vertex2 = (byte)(ByteHelper.ReadByte(value, 6) / 2);
                Vertex3 = (byte)(ByteHelper.ReadByte(value, 7) / 2);

                IsValid = true;
            }
        }

        public override int RawDataSize
        {
            get { return 0x08; }
        }
    }
}
